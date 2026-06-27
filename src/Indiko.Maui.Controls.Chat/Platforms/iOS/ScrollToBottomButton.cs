using System;
using CoreGraphics;
using UIKit;

namespace Indiko.Maui.Controls.Chat.Platforms.iOS;

/// <summary>
/// A circular floating button (chevron-down) with an optional unread-count badge. All colors,
/// sizing and badge styling are driven by the <see cref="ChatView"/> bindable properties via
/// <see cref="ApplyStyle"/>; the diameter is controlled by the superview's width/height
/// constraints. Tapping raises <see cref="Tapped"/>.
/// </summary>
internal sealed class ScrollToBottomButton : UIControl
{
    private readonly UIImageView _chevron;
    private readonly UILabel _badge;

    private UIColor _badgeBackground = UIColor.Red;
    private bool _showBadge = true;
    private int _unreadCount;

    public Action Tapped;

    public ScrollToBottomButton()
    {
        TranslatesAutoresizingMaskIntoConstraints = false;

        _chevron = new UIImageView
        {
            ContentMode = UIViewContentMode.ScaleAspectFit,
            UserInteractionEnabled = false,
            Image = UIImage.GetSystemImage("chevron.down")?
                .ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate),
        };
        AddSubview(_chevron);

        _badge = new UILabel
        {
            TextAlignment = UITextAlignment.Center,
            UserInteractionEnabled = false,
            Hidden = true,
            AdjustsFontSizeToFitWidth = true,
        };
        _badge.Layer.MasksToBounds = true;
        AddSubview(_badge);

        // Subtle drop shadow so the button reads as floating above the list.
        Layer.ShadowColor = UIColor.Black.CGColor;
        Layer.ShadowOpacity = 0.25f;
        Layer.ShadowRadius = 3f;
        Layer.ShadowOffset = new CGSize(0, 1);

        AddTarget((s, e) => Tapped?.Invoke(), UIControlEvent.TouchUpInside);
    }

    public override void LayoutSubviews()
    {
        base.LayoutSubviews();

        Layer.CornerRadius = Bounds.Width / 2f;

        // Chevron centered, ~45% of the diameter.
        var icon = Bounds.Width * 0.45f;
        _chevron.Frame = new CGRect((Bounds.Width - icon) / 2f, (Bounds.Height - icon) / 2f, icon, icon);

        // Badge sized to its text (pill shape), overhanging the top-trailing corner.
        var textSize = _badge.SizeThatFits(new CGSize(nfloat.MaxValue, nfloat.MaxValue));
        var h = (nfloat)Math.Max(18, textSize.Height + 4);
        var w = (nfloat)Math.Max(h, textSize.Width + 10);
        _badge.Frame = new CGRect(Bounds.Width - w + 6, -6, w, h);
        _badge.Layer.CornerRadius = h / 2f;
    }

    public void ApplyStyle(UIColor background, UIColor iconColor, UIColor badgeBackground, UIColor badgeTextColor, nfloat badgeFontSize, bool showBadge)
    {
        BackgroundColor = background;
        _chevron.TintColor = iconColor;
        _badgeBackground = badgeBackground;
        _badge.TextColor = badgeTextColor;
        _badge.Font = UIFont.BoldSystemFontOfSize(badgeFontSize);
        _showBadge = showBadge;
        RefreshBadge();
    }

    public int UnreadCount
    {
        get => _unreadCount;
        set
        {
            if (_unreadCount == value) return;
            _unreadCount = value;
            RefreshBadge();
        }
    }

    private void RefreshBadge()
    {
        var visible = _showBadge && _unreadCount > 0;
        _badge.Hidden = !visible;
        if (!visible) return;

        _badge.BackgroundColor = _badgeBackground;
        _badge.Text = _unreadCount > 99 ? "99+" : _unreadCount.ToString();
        SetNeedsLayout();
    }
}
