using Android.Content;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Widget;
using Indiko.Maui.Controls.Chat.Models;
using Microsoft.Maui.Platform;
using AViews = Android.Views;

namespace Indiko.Maui.Controls.Chat.Platforms.Android;

/// <summary>
/// A tappable link-preview ("unfurl") card: an optional thumbnail across the top, then a site
/// label, title and description. Styled from the <see cref="ChatView"/> link-preview bindable
/// properties via <see cref="Configure"/>. Mirrors the iOS card so both platforms match.
/// </summary>
public sealed class LinkPreviewCardView : LinearLayout
{
    private readonly ImageView _image;
    private readonly TextView _site;
    private readonly TextView _title;
    private readonly TextView _desc;
    private readonly float _density;

    public LinkPreviewCardView(Context context) : base(context)
    {
        _density = context.Resources.DisplayMetrics.Density;
        Orientation = Orientation.Vertical;
        Clickable = true;
        Focusable = true;
        ClipToOutline = true; // round the thumbnail's top corners to the card's rounded background

        _image = new ImageView(context) { Visibility = ViewStates.Gone };
        _image.SetScaleType(ImageView.ScaleType.CenterCrop);
        AddView(_image, new LayoutParams(LayoutParams.MatchParent, Dp(140)));

        var textColumn = new LinearLayout(context) { Orientation = Orientation.Vertical };
        textColumn.SetPadding(Dp(12), Dp(8), Dp(12), Dp(8));

        _site = new TextView(context);
        _title = new TextView(context);
        _title.SetTypeface(_title.Typeface, global::Android.Graphics.TypefaceStyle.Bold);
        _desc = new TextView(context) { Visibility = ViewStates.Gone };
        _desc.SetMaxLines(2);
        _desc.Ellipsize = global::Android.Text.TextUtils.TruncateAt.End;
        _title.SetMaxLines(2);
        _title.Ellipsize = global::Android.Text.TextUtils.TruncateAt.End;
        _site.SetMaxLines(1);
        _site.Ellipsize = global::Android.Text.TextUtils.TruncateAt.End;

        textColumn.AddView(_site, new LayoutParams(LayoutParams.MatchParent, LayoutParams.WrapContent));
        textColumn.AddView(_title, new LayoutParams(LayoutParams.MatchParent, LayoutParams.WrapContent));
        textColumn.AddView(_desc, new LayoutParams(LayoutParams.MatchParent, LayoutParams.WrapContent));
        AddView(textColumn, new LayoutParams(LayoutParams.MatchParent, LayoutParams.WrapContent));
    }

    public void Configure(LinkPreview preview, ChatView chatView)
    {
        var bg = new GradientDrawable();
        bg.SetShape(ShapeType.Rectangle);
        bg.SetColor(chatView.LinkPreviewBackgroundColor.ToPlatform().ToArgb());
        bg.SetCornerRadius(Dp(10));
        Background = bg;

        SetText(_site, preview.SiteName, chatView.LinkPreviewSiteNameColor.ToPlatform(), (float)chatView.LinkPreviewSiteNameFontSize);
        SetText(_title, preview.Title, chatView.LinkPreviewTitleColor.ToPlatform(), (float)chatView.LinkPreviewTitleFontSize);
        SetText(_desc, preview.Description, chatView.LinkPreviewDescriptionColor.ToPlatform(), (float)chatView.LinkPreviewDescriptionFontSize);

        if (preview.ImageBytes is { Length: > 0 })
        {
            var bmp = global::Android.Graphics.BitmapFactory.DecodeByteArray(preview.ImageBytes, 0, preview.ImageBytes.Length);
            _image.SetImageBitmap(bmp);
            _image.Visibility = ViewStates.Visible;
        }
        else
        {
            _image.SetImageBitmap(null);
            _image.Visibility = ViewStates.Gone;
        }
    }

    private static void SetText(TextView label, string text, global::Android.Graphics.Color color, float sizeSp)
    {
        label.Text = text ?? string.Empty;
        label.SetTextColor(color);
        label.SetTextSize(global::Android.Util.ComplexUnitType.Sp, sizeSp);
        label.Visibility = string.IsNullOrEmpty(text) ? ViewStates.Gone : ViewStates.Visible;
    }

    private int Dp(int value) => (int)(value * _density);
}
