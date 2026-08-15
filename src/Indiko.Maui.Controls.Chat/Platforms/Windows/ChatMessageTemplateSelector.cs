using Indiko.Maui.Controls.Chat.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Indiko.Maui.Controls.Chat.Platforms.Windows;

/// <summary>
/// Selects the row template for a <see cref="ChatMessage"/>: one template per message type,
/// with own/other variants for the bubble types (mirroring the iOS cell set).
/// </summary>
public sealed class ChatMessageTemplateSelector : DataTemplateSelector
{
    private readonly ChatDataTemplates _templates = new();

    protected override DataTemplate SelectTemplateCore(object item) => SelectTemplateCore(item, null);

    protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
    {
        if (item is not ChatMessage message)
            return null;

        var key = message.MessageType switch
        {
            MessageType.System => "SystemTemplate",
            MessageType.Date => "DateTemplate",
            MessageType.Text => message.IsOwnMessage ? "OwnTextTemplate" : "OtherTextTemplate",
            MessageType.Image => message.IsOwnMessage ? "OwnImageTemplate" : "OtherImageTemplate",
            MessageType.Video => message.IsOwnMessage ? "OwnVideoTemplate" : "OtherVideoTemplate",
            MessageType.Audio => message.IsOwnMessage ? "OwnAudioTemplate" : "OtherAudioTemplate",
            _ => "OtherTextTemplate",
        };

        return _templates[key] as DataTemplate;
    }
}
