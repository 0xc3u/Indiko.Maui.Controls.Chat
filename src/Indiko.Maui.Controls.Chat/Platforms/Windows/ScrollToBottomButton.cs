using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using WindowsColor = Windows.UI.Color;

namespace Indiko.Maui.Controls.Chat.Platforms.Windows;

/// <summary>
/// A circular floating "scroll to bottom" button with an optional unread-count badge overhanging
/// the top-trailing corner. Colors, badge styling, size and margin are driven by the
/// <see cref="ChatView"/> bindable properties via <see cref="ApplyStyle"/>. Mirrors the
/// Android/iOS scroll-to-bottom buttons.
/// </summary>
internal sealed class ScrollToBottomButton : Button
{
    private readonly FontIcon _chevron;
    private readonly Border _badge;
    private readonly TextBlock _badgeText;

    private bool _showBadge = true;
    private int _unreadCount;

    public ScrollToBottomButton()
    {
        HorizontalAlignment = HorizontalAlignment.Right;
        VerticalAlignment = VerticalAlignment.Bottom;
        Padding = new Thickness(0);

        _chevron = new FontIcon
        {
            Glyph = "\uE70E",
            FontSize = 14,
            Foreground = new SolidColorBrush(WindowsColor.FromArgb(255, 0, 0, 0)),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
        };

        _badgeText = new TextBlock
        {
            Foreground = new SolidColorBrush(WindowsColor.FromArgb(255, 255, 255, 255)),
            FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(5, 1, 5, 1),
        };

        _badge = new Border
        {
            MinWidth = 18,
            MinHeight = 18,
            CornerRadius = new CornerRadius(9),
            Background = new SolidColorBrush(WindowsColor.FromArgb(255, 255, 0, 0)),
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Top,
            RenderTransform = new TranslateTransform { X = 3, Y = -3 },
            Visibility = Visibility.Collapsed,
            Child = _badgeText,
        };

        var content = new Grid();
        content.Children.Add(_chevron);
        content.Children.Add(_badge);
        Content = content;
    }

    /// <summary>Size (diameter) and margin are applied by the handler through the layout.</summary>
    public void ApplyStyle(
        WindowsColor backgroundColor,
        WindowsColor iconColor,
        WindowsColor badgeBackgroundColor,
        WindowsColor badgeTextColor,
        double badgeFontSize,
        bool showBadge,
        double size,
        double margin)
    {
        Width = size;
        Height = size;
        CornerRadius = new CornerRadius(size / 2);
        Background = new SolidColorBrush(backgroundColor);
        Margin = new Thickness(0, 0, margin, margin);

        _chevron.Foreground = new SolidColorBrush(iconColor);
        _badge.Background = new SolidColorBrush(badgeBackgroundColor);
        _badgeText.Foreground = new SolidColorBrush(badgeTextColor);
        _badgeText.FontSize = badgeFontSize;
        _badge.CornerRadius = new CornerRadius(Math.Max(9, size * 0.28));
        _showBadge = showBadge;
        RefreshBadge();
    }

    public int UnreadCount
    {
        set
        {
            if (_unreadCount == value)
                return;
            _unreadCount = value;
            RefreshBadge();
        }
    }

    private void RefreshBadge()
    {
        var visible = _showBadge && _unreadCount > 0;
        _badge.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
        if (!visible)
            return;

        _badgeText.Text = _unreadCount > 99 ? "99+" : _unreadCount.ToString();
    }
}
