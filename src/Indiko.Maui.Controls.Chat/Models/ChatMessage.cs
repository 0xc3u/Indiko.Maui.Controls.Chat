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
    public MessageDeliveryState DeliveryState { get; set; }
    public bool IsRepliedMessage => ReplyToMessage != null;
    public RepliedMessage ReplyToMessage { get; set; }

    // property for grouped emoji reactions
    public List<ChatMessageReaction> Reactions { get; set; } = new List<ChatMessageReaction>();
}

public class RepliedMessage
{
    public string MessageId { get; set; }
    public string TextPreview { get; set; } // Short preview of the original message
    public string SenderId { get; set; }

    // helper method to generate the TextPreview for the RepliedMessage, especially if the original text is long.
    public static string GenerateTextPreview(string text, int maxLength = 50)
    {
        if (string.IsNullOrEmpty(text)) return string.Empty;
        return text.Length > maxLength ? text.Substring(0, maxLength) + "..." : text;
    }
}