using System.Windows.Input;
using Indiko.Maui.Controls.Chat.Models;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Layouts;

namespace Indiko.Maui.Controls.Chat;

/// <summary>
/// An optional, fully styleable message composer to pair with <see cref="ChatView"/>. It is
/// input-only: it never persists or sends anything. When the user sends, it raises
/// <see cref="SendCommand"/> with a <see cref="ChatComposeResult"/> (text + attached media/voice +
/// the reply target); your app builds and persists the <see cref="ChatMessage"/> and adds it to the
/// bound <c>Messages</c> collection.
///
/// Features: auto-growing text entry, attachments (built-in <c>MediaPicker</c>), an emoji picker,
/// tap-to-record voice notes (built-in), a reply banner and a selected-media preview.
/// </summary>
public class ChatInputView : ContentView
{
    private readonly Editor _editor;
    private readonly Border _replyBanner;
    private readonly Label _replyTitle;
    private readonly Label _replyPreview;
    private readonly Grid _mediaPreview;
    private readonly Image _mediaThumb;
    private readonly ScrollView _emojiPanel;
    private readonly Grid _recordingBar;
    private readonly Label _recordingTimer;
    private readonly Button _emojiButton;
    private readonly Button _attachButton;
    private readonly Button _sendButton;
    private readonly Grid _micView;     // press-and-hold to record (not a Button: needs press/release)
    private readonly Label _micGlyph;
    private readonly Image _micImage;

    private bool _isRecording;
    private bool _syncingText;
    private DateTime _recordStart;

    public ChatInputView()
    {
        Padding = new Thickness(8, 6);

        // Reply banner -------------------------------------------------------------------------
        _replyTitle = new Label { FontSize = 12, FontAttributes = FontAttributes.Bold };
        _replyPreview = new Label { FontSize = 13, LineBreakMode = LineBreakMode.TailTruncation, MaxLines = 1 };
        var replyAccent = new BoxView { WidthRequest = 3, CornerRadius = 2 };
        replyAccent.SetBinding(BoxView.ColorProperty, new Binding(nameof(AccentColor), source: this));
        var replyClose = MakeGlyphButton("✕");
        replyClose.Clicked += (s, e) => ReplyingTo = null;

        var replyTexts = new VerticalStackLayout { VerticalOptions = LayoutOptions.Center, Children = { _replyTitle, _replyPreview } };
        var replyGrid = new Grid { ColumnSpacing = 8 };
        replyGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
        replyGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
        replyGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
        Add(replyGrid, replyAccent, 0);
        Add(replyGrid, replyTexts, 1);
        Add(replyGrid, replyClose, 2);
        _replyBanner = new Border
        {
            StrokeThickness = 0,
            Padding = new Thickness(10, 6),
            Margin = new Thickness(0, 0, 0, 6),
            IsVisible = false,
            Content = replyGrid,
            StrokeShape = new RoundRectangle { CornerRadius = 8 },
        };

        // Selected-media preview ---------------------------------------------------------------
        _mediaThumb = new Image { WidthRequest = 48, HeightRequest = 48, Aspect = Aspect.AspectFill };
        var mediaClose = MakeGlyphButton("✕");
        mediaClose.HorizontalOptions = LayoutOptions.Start;
        mediaClose.Clicked += (s, e) => SelectedMedia = null;
        _mediaPreview = new Grid { IsVisible = false, Margin = new Thickness(0, 0, 0, 6), ColumnSpacing = 4 };
        _mediaPreview.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
        _mediaPreview.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
        Add(_mediaPreview, _mediaThumb, 0);
        Add(_mediaPreview, mediaClose, 1);

        // Emoji picker panel -------------------------------------------------------------------
        _emojiPanel = new ScrollView { Orientation = ScrollOrientation.Vertical, HeightRequest = 132, IsVisible = false };

        // Recording bar ------------------------------------------------------------------------
        _recordingTimer = new Label { VerticalOptions = LayoutOptions.Center, FontSize = 14, Text = "0:00" };
        var recDot = new BoxView { WidthRequest = 10, HeightRequest = 10, CornerRadius = 5, Color = Colors.Red, VerticalOptions = LayoutOptions.Center };
        var recCancel = new Button { Text = "Cancel", BackgroundColor = Colors.Transparent, HorizontalOptions = LayoutOptions.End };
        recCancel.SetBinding(Button.TextColorProperty, new Binding(nameof(AccentColor), source: this));
        recCancel.Clicked += (s, e) => CancelRecording();
        _recordingBar = new Grid { IsVisible = false, Margin = new Thickness(4, 0, 0, 6), ColumnSpacing = 8 };
        _recordingBar.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
        _recordingBar.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
        _recordingBar.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
        Add(_recordingBar, recDot, 0);
        Add(_recordingBar, _recordingTimer, 1);
        Add(_recordingBar, recCancel, 2);

        // Input row ----------------------------------------------------------------------------
        _editor = new Editor { AutoSize = EditorAutoSizeOption.TextChanges, VerticalOptions = LayoutOptions.Center };
        _editor.TextChanged += OnEditorTextChanged;

        _emojiButton = MakeGlyphButton("\U0001F642");
        _emojiButton.Clicked += (s, e) => ToggleEmojiPanel();
        _attachButton = MakeGlyphButton("\U0001F4CE");
        _attachButton.Clicked += async (s, e) => await PickMediaAsync();
        _sendButton = MakeGlyphButton("➤");
        _sendButton.Clicked += (s, e) => Send(null, null);

        // Mic: press-and-hold to record, release to send, slide off to cancel.
        _micGlyph = new Label { Text = "\U0001F3A4", FontSize = 18, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center };
        _micImage = new Image { Aspect = Aspect.AspectFit, IsVisible = false, Margin = new Thickness(8) };
        _micView = new Grid { WidthRequest = 40, HeightRequest = 40, VerticalOptions = LayoutOptions.End, Children = { _micGlyph, _micImage } };
        var micPress = new PointerGestureRecognizer();
        micPress.PointerPressed += async (s, e) => await StartRecordingAsync();
        micPress.PointerReleased += async (s, e) => { if (_isRecording) await StopRecordingAndSendAsync(); };
        micPress.PointerExited += (s, e) => { if (_isRecording) CancelRecording(); };
        _micView.GestureRecognizers.Add(micPress);

        var inputRow = new Grid { ColumnSpacing = 2 };
        inputRow.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
        inputRow.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
        inputRow.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
        inputRow.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
        Add(inputRow, _emojiButton, 0);
        Add(inputRow, _editor, 1);
        Add(inputRow, _attachButton, 2);
        Add(inputRow, _micView, 3);
        Add(inputRow, _sendButton, 3); // mic/send share the trailing slot (only one visible)

        Content = new VerticalStackLayout { Children = { _replyBanner, _mediaPreview, _emojiPanel, _recordingBar, inputRow } };

        BuildEmojiPanel();
        ApplyStyle();
        ApplyIcons();
        UpdateButtons();
    }

    private void ApplyIcons()
    {
        SetIcon(_sendButton, SendIcon, "➤");
        SetIcon(_attachButton, AttachIcon, "\U0001F4CE");
        SetIcon(_emojiButton, EmojiIcon, "\U0001F642");

        // Mic is a press-and-hold view, not a Button: swap glyph label / custom image.
        if (MicIcon != null)
        {
            _micImage.Source = MicIcon;
            _micImage.IsVisible = true;
            _micGlyph.IsVisible = false;
        }
        else
        {
            _micImage.Source = null;
            _micImage.IsVisible = false;
            _micGlyph.IsVisible = true;
            _micGlyph.FontSize = IconFontSize;
        }
    }

    private void SetIcon(Button button, ImageSource icon, string glyph)
    {
        if (icon != null)
        {
            button.ImageSource = icon;
            button.Text = string.Empty;
        }
        else
        {
            button.ImageSource = null;
            button.Text = glyph;
            button.FontSize = IconFontSize;
        }
    }

    private static void Add(Grid grid, View child, int column)
    {
        Grid.SetColumn(child, column);
        grid.Children.Add(child);
    }

    private static Button MakeGlyphButton(string glyph) => new()
    {
        Text = glyph,
        FontSize = 18,
        BackgroundColor = Colors.Transparent,
        Padding = 0,
        WidthRequest = 40,
        HeightRequest = 40,
        VerticalOptions = LayoutOptions.End,
    };

    // ---- Bindable properties --------------------------------------------------------------------

    public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(string), typeof(ChatInputView), string.Empty, BindingMode.TwoWay, propertyChanged: OnTextChanged);
    public string Text { get => (string)GetValue(TextProperty); set => SetValue(TextProperty, value); }

    public static readonly BindableProperty PlaceholderProperty = BindableProperty.Create(nameof(Placeholder), typeof(string), typeof(ChatInputView), "Type a message…", propertyChanged: (b, o, n) => ((ChatInputView)b)._editor.Placeholder = (string)n);
    public string Placeholder { get => (string)GetValue(PlaceholderProperty); set => SetValue(PlaceholderProperty, value); }

    /// <summary>Selected/attached media bytes (set by the built-in picker or by your app). Shown as a preview and included in the next <see cref="ChatComposeResult"/>.</summary>
    public static readonly BindableProperty SelectedMediaProperty = BindableProperty.Create(nameof(SelectedMedia), typeof(byte[]), typeof(ChatInputView), null, BindingMode.TwoWay, propertyChanged: (b, o, n) => ((ChatInputView)b).OnSelectedMediaChanged());
    public byte[] SelectedMedia { get => (byte[])GetValue(SelectedMediaProperty); set => SetValue(SelectedMediaProperty, value); }

    /// <summary>The kind of <see cref="SelectedMedia"/> (Image/Video/Audio). Defaults to Image when media is picked.</summary>
    public static readonly BindableProperty SelectedMediaTypeProperty = BindableProperty.Create(nameof(SelectedMediaType), typeof(MessageType?), typeof(ChatInputView), null);
    public MessageType? SelectedMediaType { get => (MessageType?)GetValue(SelectedMediaTypeProperty); set => SetValue(SelectedMediaTypeProperty, value); }

    /// <summary>When set, a reply banner is shown; the value flows into the next <see cref="ChatComposeResult.ReplyingTo"/>.</summary>
    public static readonly BindableProperty ReplyingToProperty = BindableProperty.Create(nameof(ReplyingTo), typeof(ChatMessage), typeof(ChatInputView), null, BindingMode.TwoWay, propertyChanged: (b, o, n) => ((ChatInputView)b).OnReplyingToChanged());
    public ChatMessage ReplyingTo { get => (ChatMessage)GetValue(ReplyingToProperty); set => SetValue(ReplyingToProperty, value); }

    /// <summary>Invoked when the user sends, with a <see cref="ChatComposeResult"/>. Your app persists/sends and appends the message.</summary>
    public static readonly BindableProperty SendCommandProperty = BindableProperty.Create(nameof(SendCommand), typeof(ICommand), typeof(ChatInputView), null);
    public ICommand SendCommand { get => (ICommand)GetValue(SendCommandProperty); set => SetValue(SendCommandProperty, value); }

    /// <summary>When true (default), the composer clears its text/media/reply after raising <see cref="SendCommand"/>.</summary>
    public static readonly BindableProperty ClearOnSendProperty = BindableProperty.Create(nameof(ClearOnSend), typeof(bool), typeof(ChatInputView), true);
    public bool ClearOnSend { get => (bool)GetValue(ClearOnSendProperty); set => SetValue(ClearOnSendProperty, value); }

    public static readonly BindableProperty EnableAttachmentsProperty = BindableProperty.Create(nameof(EnableAttachments), typeof(bool), typeof(ChatInputView), true, propertyChanged: (b, o, n) => ((ChatInputView)b).UpdateButtons());
    public bool EnableAttachments { get => (bool)GetValue(EnableAttachmentsProperty); set => SetValue(EnableAttachmentsProperty, value); }

    public static readonly BindableProperty EnableVoiceRecordingProperty = BindableProperty.Create(nameof(EnableVoiceRecording), typeof(bool), typeof(ChatInputView), true, propertyChanged: (b, o, n) => ((ChatInputView)b).UpdateButtons());
    public bool EnableVoiceRecording { get => (bool)GetValue(EnableVoiceRecordingProperty); set => SetValue(EnableVoiceRecordingProperty, value); }

    public static readonly BindableProperty EnableEmojiPickerProperty = BindableProperty.Create(nameof(EnableEmojiPicker), typeof(bool), typeof(ChatInputView), true, propertyChanged: (b, o, n) => ((ChatInputView)b).UpdateButtons());
    public bool EnableEmojiPicker { get => (bool)GetValue(EnableEmojiPickerProperty); set => SetValue(EnableEmojiPickerProperty, value); }

    public static readonly BindableProperty EmojiListProperty = BindableProperty.Create(nameof(EmojiList), typeof(IList<string>), typeof(ChatInputView), null, propertyChanged: (b, o, n) => ((ChatInputView)b).BuildEmojiPanel());
    public IList<string> EmojiList { get => (IList<string>)GetValue(EmojiListProperty); set => SetValue(EmojiListProperty, value); }

    // ---- Styling --------------------------------------------------------------------------------

    public static readonly BindableProperty EntryBackgroundColorProperty = BindableProperty.Create(nameof(EntryBackgroundColor), typeof(Color), typeof(ChatInputView), Color.FromRgb(0xF0, 0xF0, 0xF0), propertyChanged: (b, o, n) => ((ChatInputView)b).ApplyStyle());
    public Color EntryBackgroundColor { get => (Color)GetValue(EntryBackgroundColorProperty); set => SetValue(EntryBackgroundColorProperty, value); }

    public static readonly BindableProperty TextColorProperty = BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(ChatInputView), Colors.Black, propertyChanged: (b, o, n) => ((ChatInputView)b).ApplyStyle());
    public Color TextColor { get => (Color)GetValue(TextColorProperty); set => SetValue(TextColorProperty, value); }

    public static readonly BindableProperty PlaceholderColorProperty = BindableProperty.Create(nameof(PlaceholderColor), typeof(Color), typeof(ChatInputView), Colors.Gray, propertyChanged: (b, o, n) => ((ChatInputView)b).ApplyStyle());
    public Color PlaceholderColor { get => (Color)GetValue(PlaceholderColorProperty); set => SetValue(PlaceholderColorProperty, value); }

    /// <summary>Tint for the send/attach/mic/emoji glyphs and the reply accent.</summary>
    public static readonly BindableProperty AccentColorProperty = BindableProperty.Create(nameof(AccentColor), typeof(Color), typeof(ChatInputView), Colors.RoyalBlue, propertyChanged: (b, o, n) => ((ChatInputView)b).ApplyStyle());
    public Color AccentColor { get => (Color)GetValue(AccentColorProperty); set => SetValue(AccentColorProperty, value); }

    public static readonly BindableProperty ReplyBarBackgroundColorProperty = BindableProperty.Create(nameof(ReplyBarBackgroundColor), typeof(Color), typeof(ChatInputView), Color.FromRgb(0xEC, 0xEC, 0xEC), propertyChanged: (b, o, n) => ((ChatInputView)b).ApplyStyle());
    public Color ReplyBarBackgroundColor { get => (Color)GetValue(ReplyBarBackgroundColorProperty); set => SetValue(ReplyBarBackgroundColorProperty, value); }

    public static readonly BindableProperty ReplyBarTextColorProperty = BindableProperty.Create(nameof(ReplyBarTextColor), typeof(Color), typeof(ChatInputView), Colors.Black, propertyChanged: (b, o, n) => ((ChatInputView)b).ApplyStyle());
    public Color ReplyBarTextColor { get => (Color)GetValue(ReplyBarTextColorProperty); set => SetValue(ReplyBarTextColorProperty, value); }

    public static readonly BindableProperty InputFontSizeProperty = BindableProperty.Create(nameof(InputFontSize), typeof(double), typeof(ChatInputView), 16d, propertyChanged: (b, o, n) => ((ChatInputView)b).ApplyStyle());
    public double InputFontSize { get => (double)GetValue(InputFontSizeProperty); set => SetValue(InputFontSizeProperty, value); }

    /// <summary>Font size of the built-in glyph icons (ignored for a button that has a custom icon image).</summary>
    public static readonly BindableProperty IconFontSizeProperty = BindableProperty.Create(nameof(IconFontSize), typeof(double), typeof(ChatInputView), 18d, propertyChanged: (b, o, n) => ((ChatInputView)b).ApplyIcons());
    public double IconFontSize { get => (double)GetValue(IconFontSizeProperty); set => SetValue(IconFontSizeProperty, value); }

    // ---- Icons (optional custom images; fall back to built-in glyphs when null) ------------------

    /// <summary>Custom send-button icon. When null a default "➤" glyph (tinted with <see cref="AccentColor"/>) is used.</summary>
    public static readonly BindableProperty SendIconProperty = BindableProperty.Create(nameof(SendIcon), typeof(ImageSource), typeof(ChatInputView), null, propertyChanged: (b, o, n) => ((ChatInputView)b).ApplyIcons());
    public ImageSource SendIcon { get => (ImageSource)GetValue(SendIconProperty); set => SetValue(SendIconProperty, value); }

    /// <summary>Custom attachment-button icon. When null a default "📎" glyph is used.</summary>
    public static readonly BindableProperty AttachIconProperty = BindableProperty.Create(nameof(AttachIcon), typeof(ImageSource), typeof(ChatInputView), null, propertyChanged: (b, o, n) => ((ChatInputView)b).ApplyIcons());
    public ImageSource AttachIcon { get => (ImageSource)GetValue(AttachIconProperty); set => SetValue(AttachIconProperty, value); }

    /// <summary>Custom voice-record-button icon. When null a default "🎤" glyph is used.</summary>
    public static readonly BindableProperty MicIconProperty = BindableProperty.Create(nameof(MicIcon), typeof(ImageSource), typeof(ChatInputView), null, propertyChanged: (b, o, n) => ((ChatInputView)b).ApplyIcons());
    public ImageSource MicIcon { get => (ImageSource)GetValue(MicIconProperty); set => SetValue(MicIconProperty, value); }

    /// <summary>Custom emoji-button icon. When null a default "🙂" glyph is used.</summary>
    public static readonly BindableProperty EmojiIconProperty = BindableProperty.Create(nameof(EmojiIcon), typeof(ImageSource), typeof(ChatInputView), null, propertyChanged: (b, o, n) => ((ChatInputView)b).ApplyIcons());
    public ImageSource EmojiIcon { get => (ImageSource)GetValue(EmojiIconProperty); set => SetValue(EmojiIconProperty, value); }

    // ---- Text sync ------------------------------------------------------------------------------

    private static void OnTextChanged(BindableObject b, object oldVal, object newVal)
    {
        var self = (ChatInputView)b;
        if (!self._syncingText && self._editor.Text != (string)newVal)
            self._editor.Text = (string)newVal;
        self.UpdateButtons();
    }

    private void OnEditorTextChanged(object sender, TextChangedEventArgs e)
    {
        _syncingText = true;
        Text = e.NewTextValue;
        _syncingText = false;
        UpdateButtons();
    }

    private void OnSelectedMediaChanged()
    {
        var has = SelectedMedia is { Length: > 0 };
        _mediaPreview.IsVisible = has;
        if (has)
        {
            var bytes = SelectedMedia;
            _mediaThumb.Source = ImageSource.FromStream(() => new MemoryStream(bytes));
            SelectedMediaType ??= MessageType.Image;
        }
        else if (!_isRecording)
        {
            SelectedMediaType = null;
        }
        UpdateButtons();
    }

    private void OnReplyingToChanged()
    {
        var m = ReplyingTo;
        _replyBanner.IsVisible = m != null;
        if (m == null) return;

        var who = m.SenderName ?? m.SenderInitials ?? (m.IsOwnMessage ? "yourself" : string.Empty);
        _replyTitle.Text = $"Replying to {who}".TrimEnd();
        _replyPreview.Text = string.IsNullOrEmpty(m.TextContent) ? m.MessageType.ToString() : m.TextContent;
    }

    private void ApplyStyle()
    {
        _editor.BackgroundColor = EntryBackgroundColor;
        _editor.TextColor = TextColor;
        _editor.PlaceholderColor = PlaceholderColor;
        _editor.Placeholder = Placeholder;
        _editor.FontSize = InputFontSize;

        foreach (var btn in new[] { _emojiButton, _attachButton, _sendButton })
            btn.TextColor = AccentColor;
        _micGlyph.TextColor = AccentColor;
        _micGlyph.FontSize = IconFontSize;

        _replyBanner.BackgroundColor = ReplyBarBackgroundColor;
        _replyTitle.TextColor = AccentColor;
        _replyPreview.TextColor = ReplyBarTextColor;
        _recordingTimer.TextColor = TextColor;
    }

    private void UpdateButtons()
    {
        _emojiButton.IsVisible = EnableEmojiPicker && !_isRecording;
        _attachButton.IsVisible = EnableAttachments && !_isRecording;
        _editor.IsVisible = !_isRecording;

        var hasContent = !string.IsNullOrWhiteSpace(Text) || SelectedMedia is { Length: > 0 };
        var micCapable = EnableVoiceRecording && !hasContent;
        // Keep the mic visible while recording so the press-and-hold release lands on it.
        _micView.IsVisible = _isRecording || micCapable;
        // Send shows only when there's content and we're not recording (release sends the voice note).
        _sendButton.IsVisible = !micCapable && !_isRecording;
    }

    // ---- Send -----------------------------------------------------------------------------------

    private void Send(byte[] audioBytes, TimeSpan? audioDuration)
    {
        var hasAudio = audioBytes is { Length: > 0 };
        var result = new ChatComposeResult
        {
            Text = Text,
            MediaBytes = hasAudio ? audioBytes : SelectedMedia,
            MediaType = hasAudio ? MessageType.Audio
                      : SelectedMedia is { Length: > 0 } ? (SelectedMediaType ?? MessageType.Image)
                      : null,
            AudioDuration = hasAudio ? audioDuration : null,
            ReplyingTo = ReplyingTo,
        };
        if (result.IsEmpty) return;

        if (SendCommand?.CanExecute(result) == true)
            SendCommand.Execute(result);

        if (ClearOnSend)
        {
            Text = string.Empty;
            _editor.Text = string.Empty;
            SelectedMedia = null;
            ReplyingTo = null;
            CloseEmojiPanel();
        }
    }

    // ---- Attachments (built-in MediaPicker) -----------------------------------------------------

    private async Task PickMediaAsync()
    {
        try
        {
            var photo = await Microsoft.Maui.Media.MediaPicker.Default.PickPhotoAsync();
            if (photo == null) return;
            using var stream = await photo.OpenReadAsync();
            using var ms = new MemoryStream();
            await stream.CopyToAsync(ms);
            SelectedMediaType = MessageType.Image;
            SelectedMedia = ms.ToArray();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"ChatInputView.PickMediaAsync: {ex.Message}");
        }
    }

    // ---- Voice recording (built-in) -------------------------------------------------------------

    private AudioRecorderService _recorder;

    private async Task StartRecordingAsync()
    {
        _recorder ??= new AudioRecorderService();
        if (!await _recorder.StartAsync()) return; // permission denied / unavailable

        _isRecording = true;
        _recordStart = DateTime.Now;
        _recordingBar.IsVisible = true;
        _recordingTimer.Text = "0:00";
        UpdateButtons();

        Dispatcher.StartTimer(TimeSpan.FromSeconds(1), () =>
        {
            if (!_isRecording) return false;
            var elapsed = DateTime.Now - _recordStart;
            _recordingTimer.Text = $"{(int)elapsed.TotalMinutes}:{elapsed.Seconds:D2}";
            return true;
        });
    }

    private async Task StopRecordingAndSendAsync()
    {
        _isRecording = false;
        _recordingBar.IsVisible = false;
        UpdateButtons();

        var recording = _recorder == null ? null : await _recorder.StopAsync();
        if (recording is { Bytes.Length: > 0 })
            Send(recording.Value.Bytes, recording.Value.Duration);
    }

    private void CancelRecording()
    {
        _isRecording = false;
        _recordingBar.IsVisible = false;
        _recorder?.Cancel();
        UpdateButtons();
    }

    // ---- Emoji picker ---------------------------------------------------------------------------

    private static readonly string[] DefaultEmoji =
    [
        "😀","😂","😍","😊","😉","😎","🥳","🤔","😢","😡","👍","👎","👏","🙌","🙏","🔥","❤️","💯","🎉","✅",
    ];

    private void BuildEmojiPanel()
    {
        var emojis = EmojiList ?? DefaultEmoji;
        var flex = new FlexLayout { Wrap = FlexWrap.Wrap, Direction = FlexDirection.Row };
        foreach (var emoji in emojis)
        {
            var b = new Button { Text = emoji, FontSize = 22, BackgroundColor = Colors.Transparent, Padding = 0, WidthRequest = 44, HeightRequest = 44 };
            b.Clicked += (s, e) => InsertText(emoji);
            flex.Children.Add(b);
        }
        _emojiPanel.Content = flex;
    }

    private void InsertText(string s)
    {
        Text = (Text ?? string.Empty) + s;
        _editor.Text = Text;
    }

    private void ToggleEmojiPanel() => _emojiPanel.IsVisible = !_emojiPanel.IsVisible;
    private void CloseEmojiPanel() => _emojiPanel.IsVisible = false;
}
