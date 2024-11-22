using Android.Content;

namespace Indiko.Maui.Controls.Chat.Platforms.Android;
public static class PixelExtensions
{
    public static int DpToPx(this int dp, Context context)
    {
        var density = context.Resources.DisplayMetrics.Density;
        return (int)(dp * density + 0.5f);
    }
}