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

    /// <summary>
    /// Optional display name of the sender. When set, it is shown above incoming (other)
    /// message bubbles — useful for group chats — and de-duplicated for consecutive messages
    /// from the same sender. Leave null/empty (e.g. in 1:1 chats) to hide it.
    /// </summary>
    public string SenderName { get; set; }
    public MessageType MessageType { get; set; }
    public MessageReadState ReadState { get; set; }
    public MessageDeliveryState DeliveryState { get; set; }
    public bool IsRepliedMessage => ReplyToMessage != null;
    public RepliedMessage ReplyToMessage { get; set; }
    public List<ChatMessageReaction> Reactions { get; set; } = [];

    /// <summary>
    /// Total length of an <see cref="MessageType.Audio"/> message. Optional — when not set
    /// the player derives it from the audio file once loaded.
    /// </summary>
    public TimeSpan? AudioDuration { get; set; }

    /// <summary>
    /// Optional normalized (0..1) amplitude samples for an <see cref="MessageType.Audio"/>
    /// message, used to draw the voice-note waveform. When null the control renders a
    /// stable pseudo-waveform derived from the message so bars still vary per clip.
    /// </summary>
    public float[] AudioWaveform { get; set; }
}