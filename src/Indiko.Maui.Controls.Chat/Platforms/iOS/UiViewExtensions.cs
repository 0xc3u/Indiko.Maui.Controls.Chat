using UIKit;

namespace Indiko.Maui.Controls.Chat.Platforms.iOS;
internal static class UiViewExtensions
{
    public static void AnimateFade(this UIView view)
    {
        UIView.Animate(0.2, 0, UIViewAnimationOptions.CurveEaseInOut, () =>
        {
            view.Alpha = 0.7f;
        }, () =>
        {
            UIView.Animate(0.2, 0, UIViewAnimationOptions.CurveEaseInOut, () =>
            {
                view.Alpha = 1.0f;
            }, null);
        });
    }
}
