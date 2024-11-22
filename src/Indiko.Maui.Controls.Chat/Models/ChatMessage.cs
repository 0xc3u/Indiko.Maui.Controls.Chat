namespace Indiko.Maui.Controls.Chat.Models;

public class ChatMessage
{
    public string MessageId { get; set; }
    public DateTime Timestamp { get; set; }
    public string TextContent { get; set; }
    public byte[] BinaryContent { get; set; }
    public bool IsOwnMessage { get; set; }
    public string SenderId { get; set; }
    public byte[] SenderAvatar { get; set; }
    public string SenderInitials { get; set; }
    public MessageType MessageType { get; set; }
    public MessageReadState ReadState { get; set; }

    // property for grouped emoji reactions
    public List<ChatMessageReaction> Reactions { get; set; } = new List<ChatMessageReaction>();
}

public class ChatMessageReaction
{
    public string Emoji { get; set; } // Emoji identifier (e.g., "😊")
    public int Count { get; set; } // Number of reactions for this emoji
    public List<string> ParticipantIds { get; set; } = new List<string>(); // IDs of users who reacted
}