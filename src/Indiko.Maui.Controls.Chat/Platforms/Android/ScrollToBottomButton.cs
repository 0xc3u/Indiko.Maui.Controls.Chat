using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Widget;
using AViews = Android.Views;
using APaint = Android.Graphics.Paint;
using AColor = Android.Graphics.Color;
using APath = Android.Graphics.Path;

namespace Indiko.Maui.Controls.Chat.Platforms.Android;

/// <summary>
/// A circular floating "scroll to bottom" button (drawn chevron) with an optional unread-count
/// badge that overhangs the top-trailing corner. Colors, badge styling and visibility are driven
/// by the <see cref="ChatView"/> bindable properties via <see cref="ApplyStyle"/>; the diameter is
/// set by the host through the view's layout params.
/// </summary>
internal sealed class ScrollToBottomButton : FrameLayout
{
    private readonly ChevronView _chevron;
    private readonly TextView _badge;
    private readonly float _density;

    private bool _showBadge = true;
    private int _badgeBackgroundColor = AColor.Red.ToArgb();
    private int _unreadCount;

    public ScrollToBottomButton(Context context) : base(context)
    {
        _density = context.Resources.DisplayMetrics.Density;

        // Let the badge overhang the circle without being clipped.
        SetClipChildren(false);
        SetClipToPadding(false);
        Elevation = 6 * _density;

        _chevron = new ChevronView(context)
        {
            LayoutParameters = new LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent),
        };
        AddView(_chevron);

        _badge = new TextView(context)
        {
            Gravity = GravityFlags.Center,
            Visibility = ViewStates.Gone,
        };
        _badge.SetIncludeFontPadding(false);
        var badgeParams = new LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent)
        {
            Gravity = GravityFlags.Top | GravityFlags.End,
        };
        badgeParams.SetMargins(0, (int)(-4 * _density), (int)(-4 * _density), 0);
        _badge.LayoutParameters = badgeParams;
        AddView(_badge);
    }

    public void ApplyStyle(int backgroundColor, int iconColor, int badgeBackgroundColor, int badgeTextColor, float badgeFontSp, bool showBadge)
    {
        var circle = new GradientDrawable();
        circle.SetShape(ShapeType.Oval);
        circle.SetColor(backgroundColor);
        Background = circle;

        _chevron.SetIconColor(iconColor);

        _badgeBackgroundColor = badgeBackgroundColor;
        _badge.SetTextColor(new AColor(badgeTextColor));
        _badge.SetTextSize(global::Android.Util.ComplexUnitType.Sp, badgeFontSp);
        _badge.SetTypeface(_badge.Typeface, TypefaceStyle.Bold);

        var minSize = (int)(18 * _density);
        _badge.SetMinWidth(minSize);
        _badge.SetMinHeight(minSize);
        var padH = (int)(5 * _density);
        var padV = (int)(1 * _density);
        _badge.SetPadding(padH, padV, padH, padV);

        _showBadge = showBadge;
        RefreshBadge();
    }

    public int UnreadCount
    {
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
        _badge.Visibility = visible ? ViewStates.Visible : ViewStates.Gone;
        if (!visible) return;

        var pill = new GradientDrawable();
        pill.SetShape(ShapeType.Rectangle);
        pill.SetColor(_badgeBackgroundColor);
        pill.SetCornerRadius(100 * _density);
        _badge.Background = pill;
        _badge.Text = _unreadCount > 99 ? "99+" : _unreadCount.ToString();
    }

    // Draws a downward chevron centered in its bounds, tinted by the configured icon color.
    private sealed class ChevronView : AViews.View
    {
        private readonly APaint _paint;

        public ChevronView(Context context) : base(context)
        {
            _paint = new APaint(PaintFlags.AntiAlias)
            {
                Color = AColor.Black,
            };
            _paint.SetStyle(APaint.Style.Stroke);
            _paint.StrokeCap = APaint.Cap.Round;
            _paint.StrokeJoin = APaint.Join.Round;
        }

        public void SetIconColor(int color)
        {
            _paint.Color = new AColor(color);
            Invalidate();
        }

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);

            float cx = Width / 2f;
            float cy = Height / 2f;
            float ext = Width * 0.22f;
            _paint.StrokeWidth = Width * 0.08f;

            using var path = new APath();
            path.MoveTo(cx - ext, cy - ext * 0.55f);
            path.LineTo(cx, cy + ext * 0.55f);
            path.LineTo(cx + ext, cy - ext * 0.55f);
            canvas.DrawPath(path, _paint);
        }
    }
}
