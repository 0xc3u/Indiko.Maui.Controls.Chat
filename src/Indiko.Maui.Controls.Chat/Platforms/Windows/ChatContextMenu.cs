using Indiko.Maui.Controls.Chat.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Windows.Foundation;
using WindowsColor = Windows.UI.Color;

namespace Indiko.Maui.Controls.Chat.Platforms.Windows;

/// <summary>
/// The long-press / right-click context menu: a dimmed overlay with a preview bubble of the
/// pressed message and, below it, an emoji reaction row plus the configured context items.
/// Mirrors the Android blur-popup and the iOS ChatContextMenuView.
/// </summary>
internal sealed class ChatContextMenu
{
    private readonly Popup _popup = new();
    private readonly Grid _root = new();
    private readonly Border _overlay = new();
    private readonly Border _previewBubble = new();
    private readonly TextBlock _previewText = new();
    private readonly Border _panel = new();
    private readonly StackPanel _panelContent = new();
    private readonly ScrollViewer _emojiScroll = new();
    private readonly StackPanel _emojiRow = new();
    private readonly StackPanel _actions = new();

    private ChatViewHandler _handler;
    private ChatView _chatView;

    public ChatContextMenu()
    {
        _overlay.Background = new SolidColorBrush(WindowsColor.FromArgb(110, 15, 15, 15));
        _overlay.Tapped += (s, e) => Dismiss();

        _previewText.TextWrapping = TextWrapping.Wrap;
        _previewText.MaxWidth = 280;
        _previewBubble.Padding = new Thickness(12, 8, 12, 8);
        _previewBubble.CornerRadius = new CornerRadius(12);
        _previewBubble.HorizontalAlignment = HorizontalAlignment.Left;
        _previewBubble.VerticalAlignment = VerticalAlignment.Top;
        _previewBubble.Child = _previewText;

        _emojiRow.Orientation = Orientation.Horizontal;
        _emojiScroll.Content = _emojiRow;
        _emojiScroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
        _emojiScroll.VerticalScrollMode = ScrollMode.Disabled;
        _emojiScroll.HorizontalScrollMode = ScrollMode.Enabled;

        _actions.Orientation = Orientation.Vertical;

        _panelContent.Orientation = Orientation.Vertical;
        _panelContent.Children.Add(_emojiScroll);
        _panelContent.Children.Add(new Border
        {
            Height = 1,
            Background = new SolidColorBrush(WindowsColor.FromArgb(255, 200, 200, 200)),
            Margin = new Thickness(0, 6, 0, 0),
        });
        _panelContent.Children.Add(_actions);

        _panel.CornerRadius = new CornerRadius(12);
        _panel.Padding = new Thickness(8, 6, 8, 8);
        _panel.HorizontalAlignment = HorizontalAlignment.Left;
        _panel.VerticalAlignment = VerticalAlignment.Top;
        _panel.Child = _panelContent;

        _root.Children.Add(_overlay);
        _root.Children.Add(_previewBubble);
        _root.Children.Add(_panel);

        _popup.Child = _root;
        _popup.IsLightDismissEnabled = true;
        _popup.Closed += (s, e) => _handler = null;
    }

    public void Show(ChatViewHandler handler, ChatMessage message, FrameworkElement anchor)
    {
        _handler = handler;
        _chatView = handler.VirtualView;
        var xamlRoot = anchor.XamlRoot;
        if (_chatView == null || xamlRoot == null)
            return;

        BuildContent(message);
        Position(anchor, xamlRoot.Size);

        _popup.XamlRoot = xamlRoot;
        _popup.IsOpen = true;
    }

    public void Dismiss()
    {
        if (_popup.IsOpen)
            _popup.IsOpen = false;
    }

    private void BuildContent(ChatMessage message)
    {
        // Emoji reaction row.
        _emojiRow.Children.Clear();
        var emojis = _chatView.EmojiReactions ?? [];
        foreach (var emoji in emojis)
        {
            var button = new Button
            {
                Content = emoji,
                FontSize = (double)_chatView.ContextMenuReactionFontSize,
                Background = new SolidColorBrush(WindowsColor.FromArgb(0, 0, 0, 0)),
                Padding = new Thickness(6, 2, 6, 2),
                MinWidth = 0,
                MinHeight = 0,
            };
            button.Click += (s, e) =>
            {
                _handler?.HandleEmojiReaction(message, emoji);
                Dismiss();
            };
            _emojiRow.Children.Add(button);
        }

        // Context items with dividers between them.
        _actions.Children.Clear();
        var items = _chatView.ContextMenuItems ?? [];
        for (int i = 0; i < items.Count; i++)
        {
            var item = items[i];
            var button = new Button
            {
                Content = item.Name,
                FontSize = (double)_chatView.ContextMenuFontSize,
                Foreground = new SolidColorBrush(ChatViewState.ToWindowsColor(
                    item.IsDestructive ? _chatView.ContextMenuDestructiveTextColor : _chatView.ContextMenuTextColor)),
                Background = new SolidColorBrush(WindowsColor.FromArgb(0, 0, 0, 0)),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                Padding = new Thickness(8, 6, 8, 6),
                MinHeight = 0,
            };
            var tag = item.Tag;
            var destructive = item.IsDestructive;
            button.Click += (s, e) =>
            {
                if (destructive)
                    _handler?.HandleDestructiveAction(message, tag);
                else
                    _handler?.HandleContextAction(message, tag);
                Dismiss();
            };
            _actions.Children.Add(button);

            if (i < items.Count - 1)
            {
                _actions.Children.Add(new Border
                {
                    Height = _chatView.ContextMenuDividerHeight,
                    Background = new SolidColorBrush(ChatViewState.ToWindowsColor(_chatView.ContextMenuDividerColor)),
                });
            }
        }

        // Preview bubble of the pressed message.
        _previewText.Text = message.TextContent ?? string.Empty;
        _previewText.FontSize = (double)_chatView.MessageFontSize;
        _previewText.Foreground = new SolidColorBrush(ChatViewState.ToWindowsColor(
            message.IsOwnMessage ? _chatView.OwnMessageTextColor : _chatView.OtherMessageTextColor));
        _previewBubble.Background = new SolidColorBrush(ChatViewState.ToWindowsColor(
            message.IsOwnMessage ? _chatView.OwnMessageBackgroundColor : _chatView.OtherMessageBackgroundColor));

        _panel.Background = new SolidColorBrush(ChatViewState.ToWindowsColor(_chatView.ContextMenuBackgroundColor));
    }

    private void Position(FrameworkElement anchor, Size rootSize)
    {
        _root.Width = rootSize.Width;
        _root.Height = rootSize.Height;

        var panelWidth = Math.Min(300, rootSize.Width - 24);
        _panel.Width = panelWidth;
        _previewText.MaxWidth = panelWidth - 24;

        // Anchor coordinates in window space.
        var transform = anchor.TransformToVisual(null);
        var anchorTopLeft = transform.TransformPoint(new Point(0, 0));

        double left = anchorTopLeft.X + anchor.ActualWidth / 2 - panelWidth / 2;
        left = Math.Clamp(left, 12, Math.Max(12, rootSize.Width - panelWidth - 12));

        var top = anchorTopLeft.Y + anchor.ActualHeight + 8;

        // Measure the preview bubble so the panel can sit below it.
        _previewBubble.Measure(new Size(panelWidth, double.PositiveInfinity));
        var previewHeight = _previewBubble.DesiredSize.Height;
        _previewBubble.Width = panelWidth;
        _previewBubble.Margin = new Thickness(left, top, 0, 0);

        var panelTop = top + previewHeight + 8;
        // Clamp so the menu stays inside the window (estimated menu height).
        var maxPanelTop = Math.Max(12, rootSize.Height - 260);
        if (panelTop > maxPanelTop)
            panelTop = maxPanelTop;

        _panel.Margin = new Thickness(left, panelTop, 0, 0);
    }
}
