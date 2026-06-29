namespace Indiko.Maui.Controls.Chat.Models;

/// <summary>
/// The payload raised by <c>ChatInputView.SendCommand</c> when the user sends a message. The input
/// control is render/input-only: it never persists or transmits anything. Your app receives this
/// result, builds a <see cref="ChatMessage"/> from it (persisting / sending as needed) and adds it
/// to the bound <c>Messages</c> collection.
/// </summary>
public class ChatComposeResult
{
    /// <summary>The composed text (may be empty when only media/audio is sent).</summary>
    public string Text { get; set; }

    /// <summary>Attached media or recorded audio bytes, if any.</summary>
    public byte[] MediaBytes { get; set; }

    /// <summary>
    /// The kind of <see cref="MediaBytes"/> — <see cref="MessageType.Image"/>,
    /// <see cref="MessageType.Video"/> or <see cref="MessageType.Audio"/>. Null for a text-only message.
    /// </summary>
    public MessageType? MediaType { get; set; }

    /// <summary>Duration of the recorded voice note when <see cref="MediaType"/> is Audio.</summary>
    public TimeSpan? AudioDuration { get; set; }

    /// <summary>The message being replied to, if the composer was in reply mode; otherwise null.</summary>
    public ChatMessage ReplyingTo { get; set; }

    /// <summary>True when there is nothing to send (no text, media or audio).</summary>
    public bool IsEmpty => string.IsNullOrWhiteSpace(Text) && (MediaBytes == null || MediaBytes.Length == 0);
}
