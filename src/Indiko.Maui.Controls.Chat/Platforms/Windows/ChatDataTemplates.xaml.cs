using System;
using Indiko.Maui.Controls.Chat.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;

namespace Indiko.Maui.Controls.Chat.Platforms.Windows;

/// <summary>
/// Code-behind for the chat's row templates. Every interaction handler resolves the owning
/// <see cref="ChatViewHandler"/> through the <see cref="ChatViewState.HandlerProperty"/> attached
/// to the hosting <see cref="ListView"/> and then raises the matching command on the virtual view.
/// </summary>
public sealed partial class ChatDataTemplates : ResourceDictionary
{
    public ChatDataTemplates()
    {
        InitializeComponent();
    }

    private static ChatViewHandler ResolveHandler(object sender)
    {
        if (sender is not DependencyObject element)
            return null;

        var listView = ChatViewState.FindAncestor<ListViewBase>(element);
        return listView == null ? null : ChatViewState.GetHandler(listView);
    }

    private static ChatMessage GetMessage(object sender) => (sender as FrameworkElement)?.DataContext as ChatMessage;

    // Tap anywhere on the row → MessageTappedCommand + brief visual feedback.
    private void OnRowTapped(object sender, TappedRoutedEventArgs e)
    {
        if (e.Handled)
            return;

        var handler = ResolveHandler(sender);
        if (sender is FrameworkElement element && element.DataContext is ChatMessage message && handler?.VirtualView != null)
        {
            handler.VirtualView.MessageTappedCommand?.Execute(message);
            _ = FeedbackAsync(element);
        }
    }

    private void OnRowRightTapped(object sender, RightTappedRoutedEventArgs e)
    {
        e.Handled = true;
        ShowContextMenu(sender);
    }

    private void OnRowHolding(object sender, HoldingRoutedEventArgs e)
    {
        if (e.HoldingState == Microsoft.UI.Input.HoldingState.Started)
            ShowContextMenu(sender);
    }

    private void ShowContextMenu(object sender)
    {
        var handler = ResolveHandler(sender);
        if (sender is FrameworkElement element && element.DataContext is ChatMessage message && handler != null)
            handler.ShowContextMenu(message, element);
    }

    private void OnAvatarTapped(object sender, TappedRoutedEventArgs e)
    {
        e.Handled = true;
        var handler = ResolveHandler(sender);
        var message = GetMessage(sender);
        if (handler?.VirtualView != null && message != null)
        {
            handler.VirtualView.AvatarTappedCommand?.Execute(message);
            if (sender is FrameworkElement element)
                _ = FeedbackAsync(element);
        }
    }

    private void OnReactionsTapped(object sender, TappedRoutedEventArgs e)
    {
        e.Handled = true;
        var handler = ResolveHandler(sender);
        var message = GetMessage(sender);
        if (handler?.VirtualView != null && message != null)
            handler.VirtualView.EmojiReactionTappedCommand?.Execute(message);
    }

    private void OnReplyPreviewTapped(object sender, TappedRoutedEventArgs e)
    {
        e.Handled = true;
        var handler = ResolveHandler(sender);
        var message = GetMessage(sender);
        if (handler?.VirtualView != null && message?.ReplyToMessage != null)
            handler.VirtualView.NotifyRepliedMessageTapped(message.ReplyToMessage.MessageId);
    }

    private void OnLinkCardTapped(object sender, TappedRoutedEventArgs e)
    {
        e.Handled = true;
        var handler = ResolveHandler(sender);
        var message = GetMessage(sender);
        if (handler?.VirtualView != null && message?.LinkPreview != null)
            handler.VirtualView.NotifyLinkPreviewTapped(message.LinkPreview);
    }

    private void OnImageTapped(object sender, TappedRoutedEventArgs e)
    {
        e.Handled = true;
        var handler = ResolveHandler(sender);
        var message = GetMessage(sender);
        if (handler?.VirtualView != null && message != null)
        {
            handler.VirtualView.MessageTappedCommand?.Execute(message);
            handler.OpenImage(message, sender as UIElement);
        }
    }

    private void OnVideoPlayTapped(object sender, TappedRoutedEventArgs e)
    {
        e.Handled = true;
        var handler = ResolveHandler(sender);
        var message = GetMessage(sender);
        if (handler != null && message != null)
            handler.HandleVideoPlay(message, sender as UIElement);
    }

    // Consumes the tap so the row handler doesn't fire MessageTappedCommand; the row's
    // VoiceNotePlayer (Button.Click) handles playback.
    private void OnAudioPlayTapped(object sender, TappedRoutedEventArgs e)
    {
        e.Handled = true;
    }

    private void OnReplySwipeInvoked(SwipeItem sender, SwipeItemInvokedEventArgs args)
    {
        if (args.SwipeControl.DataContext is not ChatMessage message)
            return;

        var handler = ResolveHandler(args.SwipeControl);
        if (handler?.VirtualView == null || !handler.VirtualView.EnableSwipeToReply)
            return;

        var command = handler.VirtualView.LongPressedCommand;
        if (command == null)
            return;

        // Same event as the context menu's "Reply" item so consumers handle reply once.
        var action = new ContextAction { Name = handler.VirtualView.SwipeReplyActionName, Message = message };
        if (command.CanExecute(action))
            command.Execute(action);
    }

    private static async Task FeedbackAsync(FrameworkElement element)
    {
        try
        {
            element.Opacity = 0.7;
            await Task.Delay(100);
            element.Opacity = 1;
        }
        catch
        {
            // Element recycled meanwhile — nothing to do.
        }
    }
}

/// <summary>Formats a message timestamp as "HH:mm".</summary>
public sealed class TimeFormatConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
        => value is DateTime time ? time.ToString("HH:mm") : string.Empty;

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotSupportedException();
}


