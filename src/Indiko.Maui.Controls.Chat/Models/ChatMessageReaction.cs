namespace Indiko.Maui.Controls.Chat.Models;

public class ChatMessageReaction
{
    public string Emoji { get; set; } // Emoji identifier (e.g., "😊")
    public int Count { get; set; } // Number of reactions for this emoji
    public List<string> ParticipantIds { get; set; } = new List<string>(); // IDs of users who reacted
}
