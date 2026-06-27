using Android.Content;
using Android.Views;
using Android.Widget;

namespace Indiko.Maui.Controls.Chat.Platforms.Android;

/// <summary>
/// A <see cref="FrameLayout"/> that always fills the width/height it is offered, so its
/// match-parent child (the chat <c>RecyclerView</c>) spans the full area and the floating
/// scroll-to-bottom button truly overlays the list. A plain FrameLayout would report its
/// content's (narrower) width under an <c>AT_MOST</c> measure, leaving the list shifted with the
/// button beside it rather than on top.
/// </summary>
internal sealed class ChatOverlayContainer : FrameLayout
{
    public ChatOverlayContainer(Context context) : base(context)
    {
    }

    protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
    {
        widthMeasureSpec = ForceExact(widthMeasureSpec);
        heightMeasureSpec = ForceExact(heightMeasureSpec);
        base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
    }

    // Turn an AT_MOST/EXACTLY spec into EXACTLY at the offered size so the container (and its
    // match-parent child) fill the available space. UNSPECIFIED is left alone to avoid collapsing.
    private static int ForceExact(int measureSpec)
    {
        var mode = MeasureSpec.GetMode(measureSpec);
        if (mode == MeasureSpecMode.Unspecified)
            return measureSpec;

        var size = MeasureSpec.GetSize(measureSpec);
        return MeasureSpec.MakeMeasureSpec(size, MeasureSpecMode.Exactly);
    }
}
