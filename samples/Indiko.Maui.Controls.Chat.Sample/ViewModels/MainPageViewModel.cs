using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Indiko.Maui.Controls.Chat.Models;
using Indiko.Maui.Controls.Chat.Sample.Messages;
using Indiko.Maui.Controls.Chat.Sample.Services;

namespace Indiko.Maui.Controls.Chat.Sample.ViewModels;
public partial class MainPageViewModel : BaseViewModel
{
    private readonly IMessageService _messageService;

    [ObservableProperty]
    string newMessage;

    [ObservableProperty]
    byte[] selectedMedia;

    [ObservableProperty]
    ObservableRangeCollection<ChatMessage> chatMessages;
    

    public MainPageViewModel(IMessageService messageService)
    {
        _messageService = messageService;
    }

    public override async Task OnAppearing(object param)
    {
        var messages = await _messageService.GetMessagesAsync(DateTime.Now.AddHours(-2), DateTime.Now);
        ChatMessages = new ObservableRangeCollection<ChatMessage>(messages);
    }

    private int _olderPagesLoaded;
    private bool _isLoadingOlder;

    [RelayCommand]
    private void LoadOlderMessages()
    {
        // Demo of infinite-scroll load-more. Fires when the user scrolls to the top
        // (the oldest message). Prepend a page of older messages via InsertRange(0, ...)
        // so the handler keeps the viewport stable instead of jumping. Capped so the
        // demo doesn't grow without bound.
        if (ChatMessages == null || _isLoadingOlder || _olderPagesLoaded >= 3)
            return;

        _isLoadingOlder = true;
        _olderPagesLoaded++;

        var oldest = ChatMessages.Count > 0 ? ChatMessages[0].Timestamp : DateTime.Now;

        const int pageSize = 10;
        var older = new List<ChatMessage>(pageSize);
        for (int i = 0; i < pageSize; i++)
        {
            // i == 0 is the earliest; all earlier than the current oldest message.
            var isOwn = i % 3 == 0;
            older.Add(new ChatMessage
            {
                TextContent = $"Older message (page {_olderPagesLoaded}, #{i + 1})",
                IsOwnMessage = isOwn,
                Timestamp = oldest.AddMinutes(-(pageSize - i)),
                SenderInitials = isOwn ? "JD" : "AB",
                MessageId = Guid.NewGuid().ToString(),
                MessageType = MessageType.Text,
                ReadState = MessageReadState.Read,
                DeliveryState = MessageDeliveryState.Read,
                Reactions = [],
            });
        }

        ChatMessages.InsertRange(0, older);
        _isLoadingOlder = false;
    }


    [RelayCommand]
    private void Scrolled(ScrolledArgs scrolledArgs)
    {

    }

    // The message currently being replied to (set by a swipe or the context-menu "Reply"
    // action). When non-null, the next sent message carries a ReplyToMessage preview.
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsReplying))]
    ChatMessage replyingTo;

    public bool IsReplying => ReplyingTo != null;

    // Display text for the reply banner above the composer.
    public string ReplyingToPreview => ReplyingTo == null
        ? string.Empty
        : (string.IsNullOrEmpty(ReplyingTo.TextContent) ? ReplyingTo.MessageType.ToString() : ReplyingTo.TextContent);

    partial void OnReplyingToChanged(ChatMessage value) => OnPropertyChanged(nameof(ReplyingToPreview));

    // Starts a reply to the given message — both the swipe gesture and the context-menu
    // "Reply" item route here through LongPressedCommand (Name == "reply").
    private void BeginReply(ChatMessage message)
    {
        if (message == null)
            return;
        ReplyingTo = message;
    }

    [RelayCommand]
    private void CancelReply() => ReplyingTo = null;

    // The message being edited (set from the "Edit" context-menu action); bound to ChatInputView.EditingMessage.
    [ObservableProperty]
    ChatMessage editingMessage;

    // Receives the composed message from ChatInputView and turns it into a ChatMessage. This is
    // where your app would persist / send. The composer is input-only; it never does this itself.
    [RelayCommand]
    private void SendComposed(ChatComposeResult result)
    {
        if (ChatMessages == null || result == null)
            return;

        // Edit mode: update the existing message's text in place.
        if (result.IsEdit)
        {
            var idx = ChatMessages.IndexOf(result.EditingMessage);
            if (idx >= 0)
            {
                result.EditingMessage.TextContent = result.Text;
                ChatMessages[idx] = result.EditingMessage; // raises Replace so the cell re-renders
            }
            return;
        }

        if (result.IsEmpty)
            return;

        // Mark existing unread as read (we're sending, so we're at the bottom).
        for (int n = 0; n < ChatMessages.Count; n++)
            if (ChatMessages[n].ReadState == MessageReadState.New)
                ChatMessages[n].ReadState = MessageReadState.Read;

        ChatMessages.Add(new ChatMessage
        {
            TextContent = result.Text,
            BinaryContent = result.MediaBytes,
            MessageType = result.MediaType ?? MessageType.Text,
            AudioDuration = result.AudioDuration,
            IsOwnMessage = true,
            Timestamp = DateTime.Now,
            SenderInitials = "JD",
            MessageId = Guid.NewGuid().ToString(),
            ReadState = MessageReadState.Read,
            DeliveryState = MessageDeliveryState.Sent,
            Reactions = [],
            ReplyToMessage = result.ReplyingTo == null ? null : new RepliedMessage
            {
                MessageId = result.ReplyingTo.MessageId,
                SenderId = result.ReplyingTo.IsOwnMessage ? "You" : (result.ReplyingTo.SenderName ?? result.ReplyingTo.SenderInitials),
                TextPreview = RepliedMessage.GenerateTextPreview(
                    string.IsNullOrEmpty(result.ReplyingTo.TextContent) ? result.ReplyingTo.MessageType.ToString() : result.ReplyingTo.TextContent),
            },
        });
    }

    // Appends an incoming "other" message (newest). Use it while scrolled up to verify
    // the handler keeps the viewport stable, and while at the bottom to verify it follows.
    [RelayCommand]
    private void SimulateIncoming()
    {
        if (ChatMessages == null)
            return;

        ChatMessages.Add(new ChatMessage
        {
            TextContent = "See https://dotnet.microsoft.com or email test@example.com or call +1 202 555 0100",
            IsOwnMessage = false,
            Timestamp = DateTime.Now,
            SenderId = "alex",
            SenderInitials = "AB",
            SenderName = "Alex Berg",
            MessageId = Guid.NewGuid().ToString(),
            MessageType = MessageType.Text,
            ReadState = MessageReadState.New,
            DeliveryState = MessageDeliveryState.Delivered,
            Reactions = [],
        });
    }

    // Posts an own and an incoming voice note so both audio cells can be exercised.
    [RelayCommand]
    private void SendVoiceNote()
    {
        if (ChatMessages == null)
            return;

        var (ownBytes, ownDuration) = Utils.SampleAudioGenerator.GenerateWav(seconds: 3, frequency: 440);
        ChatMessages.Add(new ChatMessage
        {
            IsOwnMessage = true,
            Timestamp = DateTime.Now,
            SenderInitials = "JD",
            MessageId = Guid.NewGuid().ToString(),
            MessageType = MessageType.Audio,
            BinaryContent = ownBytes,
            AudioDuration = ownDuration,
            ReadState = MessageReadState.Read,
            DeliveryState = MessageDeliveryState.Sent,
            Reactions = [],
        });

        var (otherBytes, otherDuration) = Utils.SampleAudioGenerator.GenerateWav(seconds: 2, frequency: 330);
        ChatMessages.Add(new ChatMessage
        {
            IsOwnMessage = false,
            Timestamp = DateTime.Now,
            SenderInitials = "AB",
            MessageId = Guid.NewGuid().ToString(),
            MessageType = MessageType.Audio,
            BinaryContent = otherBytes,
            AudioDuration = otherDuration,
            ReadState = MessageReadState.New,
            DeliveryState = MessageDeliveryState.Delivered,
            Reactions = [],
        });
    }

    // Posts an own and an incoming image, each with a caption, to exercise media captions.
    [RelayCommand]
    private void SendCaptionedImage()
    {
        if (ChatMessages == null)
            return;

        ChatMessages.Add(new ChatMessage
        {
            IsOwnMessage = true,
            Timestamp = DateTime.Now,
            SenderInitials = "JD",
            MessageId = Guid.NewGuid().ToString(),
            MessageType = MessageType.Image,
            BinaryContent = Utils.SampleImageGenerator.GenerateBmp(),
            TextContent = "Check out this view from the trail!",
            ReadState = MessageReadState.Read,
            DeliveryState = MessageDeliveryState.Sent,
            Reactions = [],
        });

        ChatMessages.Add(new ChatMessage
        {
            IsOwnMessage = false,
            Timestamp = DateTime.Now,
            SenderInitials = "AB",
            MessageId = Guid.NewGuid().ToString(),
            MessageType = MessageType.Image,
            BinaryContent = Utils.SampleImageGenerator.GenerateBmp(width: 200, height: 200, r: 0x10, g: 0x80, b: 0x40),
            TextContent = "Nice! Here's mine 🌄",
            ReadState = MessageReadState.New,
            DeliveryState = MessageDeliveryState.Delivered,
            Reactions = [],
        });
    }

    // Posts an incoming text message carrying a link preview (the app supplies the unfurled data;
    // the control only renders the card).
    [RelayCommand]
    private void SendLinkPreview()
    {
        if (ChatMessages == null)
            return;

        ChatMessages.Add(new ChatMessage
        {
            TextContent = "Check out the .NET MAUI docs: https://learn.microsoft.com/dotnet/maui/",
            IsOwnMessage = false,
            Timestamp = DateTime.Now,
            SenderId = "alex",
            SenderInitials = "AB",
            SenderName = "Alex Berg",
            MessageId = Guid.NewGuid().ToString(),
            MessageType = MessageType.Text,
            ReadState = MessageReadState.New,
            DeliveryState = MessageDeliveryState.Delivered,
            Reactions = [],
            LinkPreview = new LinkPreview
            {
                Url = "https://learn.microsoft.com/dotnet/maui/",
                SiteName = "learn.microsoft.com",
                Title = ".NET MAUI documentation",
                Description = "Build cross-platform native apps for Android, iOS, macOS and Windows from a single C# codebase.",
                ImageBytes = Utils.SampleImageGenerator.GenerateBmp(width: 320, height: 180, r: 0x51, g: 0x2B, b: 0xD4),
            },
        });
    }

    [RelayCommand]
    private void ScrolledToLastMessage()
    {
        // mark all existing messages as read
        for (int n = 0; n < ChatMessages.Count; n++)
        {
            if (ChatMessages[n].ReadState == MessageReadState.New)
            {
                ChatMessages[n].ReadState = MessageReadState.Read;
            }
        }
    }


    [RelayCommand]
    private void OnAvatarTapped(ChatMessage message)
    {
        Console.WriteLine($"Avatar tapped for message: {message.MessageId}");
    }

    [RelayCommand]
    private void OnMessageTapped(ChatMessage message)
    {
        Console.WriteLine($"Message tapped for message: {message.MessageId}");
    }

    [RelayCommand]
    private void OnEmojiReactionTapped(ChatMessage message)
    {
        Console.WriteLine($"Emoji Reaction tapped: {message.MessageId}");
    }

    [RelayCommand]
    public void LongPressed(ContextAction contextAction)
    {
        switch (contextAction.Name)
        {
            case "reply":
                // Same handler for the context-menu "Reply" item and the swipe-to-reply gesture.
                BeginReply(contextAction.Message);
                break;
            case "edit":
                // Puts the composer into edit mode (prefills text, shows the editing banner).
                EditingMessage = contextAction.Message;
                break;
            case "delete":
                Console.WriteLine($"Delete message: {contextAction.Message.MessageId}");
                break;
            case "copy":
                Console.WriteLine($"Copy message: {contextAction.Message.MessageId}");
                break;
            case "react":
                ChatMessageReaction chatMessageReaction = contextAction.AdditionalData as ChatMessageReaction;
                Console.WriteLine($"React to message: {contextAction.Message.MessageId}, Additional Data: {chatMessageReaction.Emoji}");
                break;
        }
    }


    [RelayCommand]
    private void SendMessage()
    {
        // Allow sending an image on its own — only block when there is neither text nor media.
        if (string.IsNullOrWhiteSpace(NewMessage) && SelectedMedia == null)
            return;



        var newChatMessage = new ChatMessage
        {
            TextContent = NewMessage,
            IsOwnMessage = true,
            Timestamp = DateTime.Now,
            SenderAvatar = null,
            SenderInitials = "JD",
            MessageId = Guid.NewGuid().ToString(),
            MessageType = MessageType.Text,
            ReadState = MessageReadState.New,
            DeliveryState = MessageDeliveryState.Sent,
            Reactions = [],
            ReplyToMessage = ReplyingTo == null ? null : new RepliedMessage
            {
                MessageId = ReplyingTo.MessageId,
                SenderId = ReplyingTo.IsOwnMessage ? "You" : (ReplyingTo.SenderName ?? ReplyingTo.SenderInitials),
                TextPreview = RepliedMessage.GenerateTextPreview(
                    string.IsNullOrEmpty(ReplyingTo.TextContent) ? ReplyingTo.MessageType.ToString() : ReplyingTo.TextContent),
            }
        };

        if (SelectedMedia != null)
        {
            newChatMessage.BinaryContent = SelectedMedia;
            newChatMessage.MessageType = MessageType.Image;
        }

        for (int n = 0; n < ChatMessages.Count; n++)
        {
            if (ChatMessages[n].ReadState == MessageReadState.New)
            {
                ChatMessages[n].ReadState = MessageReadState.Read;
            }
        }
        ChatMessages.Add(newChatMessage);
        NewMessage = string.Empty;
        SelectedMedia = null;
        ReplyingTo = null;

        WeakReferenceMessenger.Default.Send<HideKeyboardMessage>(new HideKeyboardMessage());
    }


    [RelayCommand]
    private void ClearMedia()
    {
        SelectedMedia = null;
    }

    [RelayCommand]
    private async Task PickMedia()
    {
        var mediaPickerOptions = new MediaPickerOptions()
        {
            Title = "Please pick a photo"
        };

        var mediaPickerResult = await MediaPicker.Default.PickPhotoAsync(mediaPickerOptions);

        if (mediaPickerResult != null)
        {
            var stream = await mediaPickerResult.OpenReadAsync();
            byte[] media = new byte[stream.Length];
            await stream.ReadAsync(media, 0, (int)stream.Length);
            SelectedMedia = media;
        }
    }

    
}
