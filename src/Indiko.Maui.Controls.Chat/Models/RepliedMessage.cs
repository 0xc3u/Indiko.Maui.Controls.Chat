namespace Indiko.Maui.Controls.Chat.Models;

public class RepliedMessage
{
    public string MessageId { get; set; }
    public string TextPreview { get; set; } // Short preview of the original message
    public string SenderId { get; set; }

    // helper method to generate the TextPreview for the RepliedMessage, especially if the original text is long.
    public static string GenerateTextPreview(string text, int maxLength = 50)
    {
        if (string.IsNullOrEmpty(text)) return string.Empty;
        return text.Length > maxLength ? text[..maxLength] + "..." : text;
    }
}