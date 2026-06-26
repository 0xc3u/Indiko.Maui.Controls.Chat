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

    // Appends an incoming "other" message (newest). Use it while scrolled up to verify
    // the handler keeps the viewport stable, and while at the bottom to verify it follows.
    [RelayCommand]
    private void SimulateIncoming()
    {
        if (ChatMessages == null)
            return;

        ChatMessages.Add(new ChatMessage
        {
            TextContent = $"Incoming message at {DateTime.Now:HH:mm:ss}",
            IsOwnMessage = false,
            Timestamp = DateTime.Now,
            SenderInitials = "AB",
            MessageId = Guid.NewGuid().ToString(),
            MessageType = MessageType.Text,
            ReadState = MessageReadState.New,
            DeliveryState = MessageDeliveryState.Delivered,
            Reactions = [],
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
                Console.WriteLine($"Reply to message: {contextAction.Message.MessageId}");
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
            ReplyToMessage = null
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
