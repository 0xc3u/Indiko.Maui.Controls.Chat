using System.Collections.Generic;
using Indiko.Maui.Controls.Chat.Models;

namespace Indiko.Maui.Controls.Chat;

internal static class MessageGrouping
{
    /// <summary>
    /// True when the message at <paramref name="chronologicalIndex"/> starts a new run from its
    /// sender — i.e. it's the first message, follows a date/system separator, or the previous
    /// message is from a different sender. Used to show the sender name only once per run.
    /// </summary>
    public static bool IsFirstOfSenderRun(IList<ChatMessage> messages, int chronologicalIndex)
    {
        if (messages == null || chronologicalIndex <= 0 || chronologicalIndex >= messages.Count)
            return true;

        var message = messages[chronologicalIndex];
        var previous = messages[chronologicalIndex - 1];

        if (previous.MessageType == MessageType.Date || previous.MessageType == MessageType.System)
            return true;

        return previous.IsOwnMessage != message.IsOwnMessage || !SameSender(previous, message);
    }

    // Compares by SenderId when available, otherwise falls back to SenderName so grouping
    // still works for consumers that only populate the display name.
    private static bool SameSender(ChatMessage a, ChatMessage b)
    {
        if (!string.IsNullOrEmpty(a.SenderId) || !string.IsNullOrEmpty(b.SenderId))
            return a.SenderId == b.SenderId;
        return a.SenderName == b.SenderName;
    }
}
