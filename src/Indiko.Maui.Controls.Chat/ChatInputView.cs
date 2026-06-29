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
/// Features: auto-growing text entry; attachments via a built-in action sheet (photo / video /
/// camera); an emoji picker; WhatsApp-style press-and-hold voice notes (slide to cancel, slide up to
/// lock, live waveform); a reply banner and a selected-media preview.
/// </summary>
public class ChatInputView : ContentView
{
    private readonly Editor _editor;
    private readonly Border _replyBanner;
    private readonly Label _replyTitle;
    private readonly Label _replyPreview;
    private readonly Border _editBanner;
    private readonly Label _editPreview;
    private readonly Grid _mediaPreview;
    private readonly Image _mediaThumb;
    private readonly ScrollView _emojiPanel;
    private readonly Grid _recordingBar;
    private readonly Label _recordingTimer;
    private readonly Label _cancelHint;
    private readonly Button _trashButton;       // delete (locked mode)
    private readonly Button _lockSendButton;    // send (locked mode)
    private readonly RecordingWaveformView _waveform;
    private readonly Button _emojiButton;
    private readonly Button _attachButton;
    private readonly Button _sendButton;
    private readonly Grid _micView;     // press-and-hold to record (not a Button: needs press/release)
    private readonly Label _micGlyph;
    private readonly Image _micImage;

    private bool _isRecording;
    private bool _isLocked;             // hands-free: keeps recording after the finger is lifted
    private bool _willCancel;           // dragged left past the cancel threshold
    private bool _pointerDown;          // finger currently on the mic
    private bool _syncingText;
    private DateTime _recordStart;

    private const double CancelDragThreshold = 90;  // drag left to cancel
    private const double LockDragThreshold = 80;     // drag up to lock
    private const double MinRecordSeconds = 0.8;     // shorter recordings are discarded (accidental taps)

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

        // Edit banner --------------------------------------------------------------------------
        var editTitle = new Label { Text = "Editing message", FontSize = 12, FontAttributes = FontAttributes.Bold };
        editTitle.SetBinding(Label.TextColorProperty, new Binding(nameof(AccentColor), source: this));
        _editPreview = new Label { FontSize = 13, LineBreakMode = LineBreakMode.TailTruncation, MaxLines = 1 };
        var editAccent = new BoxView { WidthRequest = 3, CornerRadius = 2 };
        editAccent.SetBinding(BoxView.ColorProperty, new Binding(nameof(AccentColor), source: this));
        var editClose = MakeGlyphButton("✕");
        editClose.Clicked += (s, e) => CancelEdit();
        var editTexts = new VerticalStackLayout { VerticalOptions = LayoutOptions.Center, Children = { editTitle, _editPreview } };
        var editGrid = new Grid { ColumnSpacing = 8 };
        editGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
        editGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
        editGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
        Add(editGrid, editAccent, 0);
        Add(editGrid, editTexts, 1);
        Add(editGrid, editClose, 2);
        _editBanner = new Border
        {
            StrokeThickness = 0,
            Padding = new Thickness(10, 6),
            Margin = new Thickness(0, 0, 0, 6),
            IsVisible = false,
            Content = editGrid,
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
        // Recording bar — adapts between "holding" (slide to cancel / up to lock) and "locked"
        // (hands-free: trash + send buttons). Columns: trash | dot | timer | waveform | cancelHint | send.
        _recordingTimer = new Label { VerticalOptions = LayoutOptions.Center, FontSize = 14, Text = "0:00" };
        var recDot = new BoxView { WidthRequest = 10, HeightRequest = 10, CornerRadius = 5, Color = Colors.Red, VerticalOptions = LayoutOptions.Center };
        _cancelHint = new Label { Text = "‹ slide to cancel", VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.End, FontSize = 13 };
        _trashButton = MakeGlyphButton("\U0001F5D1"); // 🗑
        _trashButton.IsVisible = false;
        _trashButton.Clicked += (s, e) => CancelRecording();
        _lockSendButton = MakeGlyphButton("➤");
        _lockSendButton.IsVisible = false;
        _lockSendButton.Clicked += async (s, e) => await StopRecordingAndSendAsync();
        _waveform = new RecordingWaveformView { VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Fill };

        _recordingBar = new Grid { IsVisible = false, Margin = new Thickness(4, 0, 0, 6), ColumnSpacing = 8 };
        _recordingBar.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto)); // 0 trash
        _recordingBar.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto)); // 1 dot
        _recordingBar.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto)); // 2 timer
        _recordingBar.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star)); // 3 waveform
        _recordingBar.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto)); // 4 cancel hint
        _recordingBar.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto)); // 5 lock send
        Add(_recordingBar, _trashButton, 0);
        Add(_recordingBar, recDot, 1);
        Add(_recordingBar, _recordingTimer, 2);
        Add(_recordingBar, _waveform, 3);
        Add(_recordingBar, _cancelHint, 4);
        Add(_recordingBar, _lockSendButton, 5);

        // Input row ----------------------------------------------------------------------------
        _editor = new Editor { AutoSize = EditorAutoSizeOption.TextChanges, VerticalOptions = LayoutOptions.Center };
        _editor.TextChanged += OnEditorTextChanged;

        _emojiButton = MakeGlyphButton("\U0001F642");
        _emojiButton.Clicked += (s, e) => ToggleEmojiPanel();
        _attachButton = MakeGlyphButton("\U0001F4CE");
        _attachButton.Clicked += async (s, e) => await ShowAttachmentSheetAsync();
        _sendButton = MakeGlyphButton("➤");
        _sendButton.Clicked += (s, e) => Send(null, null);

        // Mic: press-and-hold to record, release to send, slide off to cancel.
        _micGlyph = new Label { Text = "\U0001F3A4", FontSize = 18, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center };
        _micImage = new Image { Aspect = Aspect.AspectFit, IsVisible = false, Margin = new Thickness(8) };
        _micView = new Grid { WidthRequest = 40, HeightRequest = 40, VerticalOptions = LayoutOptions.End, Children = { _micGlyph, _micImage } };
        // Pointer for press/release (start/stop — fires immediately on touch-down, no movement needed);
        // Pan for the drag (lock/cancel — reliable for touch on Android, unlike PointerMoved).
        var micPress = new PointerGestureRecognizer();
        micPress.PointerPressed += async (s, e) => { _pointerDown = true; await StartRecordingAsync(); };
        micPress.PointerReleased += async (s, e) => { _pointerDown = false; await OnMicReleasedAsync(); };
        _micView.GestureRecognizers.Add(micPress);

        var micPan = new PanGestureRecognizer();
        micPan.PanUpdated += (s, e) => OnMicPan(e);
        _micView.GestureRecognizers.Add(micPan);

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

        Content = new VerticalStackLayout { Children = { _editBanner, _replyBanner, _mediaPreview, _emojiPanel, _recordingBar, inputRow } };

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

    /// <summary>
    /// When set, the composer enters edit mode: it prefills the text with the message's content and
    /// shows an "Editing message" banner. The next <see cref="SendCommand"/> carries it as
    /// <see cref="ChatComposeResult.EditingMessage"/> so your app updates that message's text.
    /// </summary>
    public static readonly BindableProperty EditingMessageProperty = BindableProperty.Create(nameof(EditingMessage), typeof(ChatMessage), typeof(ChatInputView), null, BindingMode.TwoWay, propertyChanged: (b, o, n) => ((ChatInputView)b).OnEditingMessageChanged((ChatMessage)n));
    public ChatMessage EditingMessage { get => (ChatMessage)GetValue(EditingMessageProperty); set => SetValue(EditingMessageProperty, value); }

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

    /// <summary>When true (default), the attachment sheet offers "Take Photo" (camera capture).</summary>
    public static readonly BindableProperty EnableCameraProperty = BindableProperty.Create(nameof(EnableCamera), typeof(bool), typeof(ChatInputView), true);
    public bool EnableCamera { get => (bool)GetValue(EnableCameraProperty); set => SetValue(EnableCameraProperty, value); }

    /// <summary>Labels for the attachment action sheet: [0]=title, [1]=cancel, [2]=photo, [3]=video, [4]=camera.</summary>
    public static readonly BindableProperty AttachmentSheetLabelsProperty = BindableProperty.Create(nameof(AttachmentSheetLabels), typeof(IList<string>), typeof(ChatInputView), null);
    public IList<string> AttachmentSheetLabels { get => (IList<string>)GetValue(AttachmentSheetLabelsProperty); set => SetValue(AttachmentSheetLabelsProperty, value); }

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

    private void OnEditingMessageChanged(ChatMessage m)
    {
        _editBanner.IsVisible = m != null;
        if (m == null) return;

        // Editing supersedes replying; prefill the editor with the existing text.
        ReplyingTo = null;
        _editPreview.Text = m.TextContent;
        Text = m.TextContent ?? string.Empty;
        _editor.Text = Text;
        _editor.Focus();
    }

    private void CancelEdit()
    {
        EditingMessage = null;
        Text = string.Empty;
        _editor.Text = string.Empty;
    }

    private void ApplyStyle()
    {
        _editor.BackgroundColor = EntryBackgroundColor;
        _editor.TextColor = TextColor;
        _editor.PlaceholderColor = PlaceholderColor;
        _editor.Placeholder = Placeholder;
        _editor.FontSize = InputFontSize;

        foreach (var btn in new[] { _emojiButton, _attachButton, _sendButton, _lockSendButton })
            btn.TextColor = AccentColor;
        _trashButton.TextColor = Colors.Red;
        _micGlyph.TextColor = AccentColor;
        _micGlyph.FontSize = IconFontSize;

        _replyBanner.BackgroundColor = ReplyBarBackgroundColor;
        _replyTitle.TextColor = AccentColor;
        _replyPreview.TextColor = ReplyBarTextColor;
        _editBanner.BackgroundColor = ReplyBarBackgroundColor;
        _editPreview.TextColor = ReplyBarTextColor;
        _recordingTimer.TextColor = TextColor;
        _cancelHint.TextColor = PlaceholderColor;
        _waveform.BarColor = AccentColor;
    }

    private void UpdateButtons()
    {
        _emojiButton.IsVisible = EnableEmojiPicker && !_isRecording;
        _attachButton.IsVisible = EnableAttachments && !_isRecording;
        _editor.IsVisible = !_isRecording;

        var hasContent = !string.IsNullOrWhiteSpace(Text) || SelectedMedia is { Length: > 0 };
        var micCapable = EnableVoiceRecording && !hasContent;
        // While holding (recording, not locked) keep the mic so the release lands on it; once locked
        // the finger is lifted and the locked bar's send/trash take over, so hide the mic.
        var recordingHold = _isRecording && !_isLocked;
        _micView.IsVisible = recordingHold || (micCapable && !_isRecording);
        _sendButton.IsVisible = !micCapable && !_isRecording;
    }

    // Shows/hides the recording-bar pieces for the current state.
    private void UpdateRecordingUi()
    {
        _recordingBar.IsVisible = _isRecording;
        _trashButton.IsVisible = _isRecording && _isLocked;
        _lockSendButton.IsVisible = _isRecording && _isLocked;
        _cancelHint.IsVisible = _isRecording && !_isLocked;
        UpdateButtons();
    }

    // ---- Send -----------------------------------------------------------------------------------

    private void Send(byte[] audioBytes, TimeSpan? audioDuration)
    {
        var editing = EditingMessage;

        // Edit mode: text-only update of an existing message (empty text is a no-op, not a delete).
        if (editing != null)
        {
            if (string.IsNullOrWhiteSpace(Text)) return;

            var editResult = new ChatComposeResult { Text = Text, EditingMessage = editing };
            if (SendCommand?.CanExecute(editResult) == true)
                SendCommand.Execute(editResult);

            if (ClearOnSend)
            {
                EditingMessage = null;
                Text = string.Empty;
                _editor.Text = string.Empty;
                CloseEmojiPanel();
            }
            return;
        }

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

    private async Task ShowAttachmentSheetAsync()
    {
        var labels = AttachmentSheetLabels;
        string Label(int i, string fallback) => labels != null && labels.Count > i && !string.IsNullOrEmpty(labels[i]) ? labels[i] : fallback;
        var title = Label(0, "Attach");
        var cancel = Label(1, "Cancel");
        var photo = Label(2, "Photo");
        var video = Label(3, "Video");
        var camera = Label(4, "Take Photo");

        var page = GetHostPage();
        if (page == null) { await PickPhotoAsync(); return; } // no page to host the sheet — fall back

        var options = EnableCamera ? new[] { photo, video, camera } : new[] { photo, video };
        var choice = await page.DisplayActionSheet(title, cancel, null, options);

        if (choice == photo) await PickPhotoAsync();
        else if (choice == video) await PickVideoAsync();
        else if (choice == camera) await CapturePhotoAsync();
    }

    private async Task PickPhotoAsync() => await PickAsync(() => Microsoft.Maui.Media.MediaPicker.Default.PickPhotoAsync(), MessageType.Image);

    private async Task PickVideoAsync() => await PickAsync(() => Microsoft.Maui.Media.MediaPicker.Default.PickVideoAsync(), MessageType.Video);

    private async Task CapturePhotoAsync()
    {
        if (!Microsoft.Maui.Media.MediaPicker.Default.IsCaptureSupported)
        {
            await PickPhotoAsync();
            return;
        }
        await PickAsync(() => Microsoft.Maui.Media.MediaPicker.Default.CapturePhotoAsync(), MessageType.Image);
    }

    private async Task PickAsync(Func<Task<Microsoft.Maui.Storage.FileResult>> pick, MessageType type)
    {
        try
        {
            var file = await pick();
            if (file == null) return;
            using var stream = await file.OpenReadAsync();
            using var ms = new MemoryStream();
            await stream.CopyToAsync(ms);
            SelectedMediaType = type;
            SelectedMedia = ms.ToArray();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"ChatInputView attachment failed: {ex.Message}");
        }
    }

    // Walks up the visual tree to the hosting Page (needed to present the action sheet).
    private Page GetHostPage()
    {
        Element element = this;
        while (element != null && element is not Page)
            element = element.Parent;
        return element as Page;
    }

    // ---- Voice recording (built-in) -------------------------------------------------------------

    private AudioRecorderService _recorder;

    private async Task StartRecordingAsync()
    {
        if (_isRecording) return;
        _recorder ??= new AudioRecorderService();
        if (!await _recorder.StartAsync()) return; // permission denied / unavailable

        _isRecording = true;
        _isLocked = false;
        _willCancel = false;
        _recordStart = DateTime.Now;
        _recordingTimer.Text = "0:00";
        _cancelHint.Text = "‹ slide to cancel";
        _cancelHint.TextColor = PlaceholderColor;
        _waveform.Reset();
        UpdateRecordingUi();

        // Drive the timer text and the live waveform.
        Dispatcher.StartTimer(TimeSpan.FromMilliseconds(100), () =>
        {
            if (!_isRecording) return false;
            var elapsed = DateTime.Now - _recordStart;
            _recordingTimer.Text = $"{(int)elapsed.TotalMinutes}:{elapsed.Seconds:D2}";
            _waveform.Push(_recorder?.GetLevel() ?? 0f);
            return true;
        });

        // The gesture may have ended before this async start finished (e.g. a quick tap). Finalize
        // it now; the min-duration guard discards anything too short.
        if (!_pointerDown && !_isLocked)
            await StopRecordingAndSendAsync();
    }

    // Drag tracking while holding the mic: up → lock (hands-free), left → cancel.
    private void OnMicPan(PanUpdatedEventArgs e)
    {
        if (!_isRecording || _isLocked || e.StatusType != GestureStatus.Running) return;

        if (e.TotalY <= -LockDragThreshold)
        {
            _isLocked = true;
            UpdateRecordingUi();
            return;
        }

        _willCancel = e.TotalX <= -CancelDragThreshold;
        _cancelHint.Text = _willCancel ? "release to cancel" : "‹ slide to cancel";
        _cancelHint.TextColor = _willCancel ? Colors.Red : PlaceholderColor;
    }

    private async Task OnMicReleasedAsync()
    {
        if (!_isRecording || _isLocked) return; // locked: stays recording; use trash/send in the bar
        if (_willCancel)
            CancelRecording();
        else
            await StopRecordingAndSendAsync();
    }

    private async Task StopRecordingAndSendAsync()
    {
        if (!_isRecording) return;
        var elapsed = DateTime.Now - _recordStart;
        _isRecording = false;
        _isLocked = false;
        _waveform.Reset();
        UpdateRecordingUi();

        var recording = _recorder == null ? null : await _recorder.StopAsync();
        // Discard accidental/too-short recordings (and always release via StopAsync above).
        if (elapsed.TotalSeconds >= MinRecordSeconds && recording is { Bytes.Length: > 0 })
            Send(recording.Value.Bytes, recording.Value.Duration);
    }

    private void CancelRecording()
    {
        _isRecording = false;
        _isLocked = false;
        _willCancel = false;
        _waveform.Reset();
        _recorder?.Cancel();
        UpdateRecordingUi();
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
