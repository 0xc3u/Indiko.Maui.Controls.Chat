using UIKit;
using CoreGraphics;

namespace Indiko.Maui.Controls.Chat.Platforms.iOS;
public static class UIImageExtensions
{
    public static UIImage CreateCircularImage(UIImage image)
    {
        var size = Math.Min(image.Size.Width, image.Size.Height);
        UIGraphics.BeginImageContextWithOptions(new CGSize(size, size), false, 0);
        var context = UIGraphics.GetCurrentContext();
        var path = new CGPath();
        path.AddArc((float)size / 2, (float)size / 2, (float)size / 2, 0, (float)(2 * Math.PI), true);
        context.AddPath(path);
        context.Clip();
        image.Draw(new CGRect(0, 0, size, size));
        var circularImage = UIGraphics.GetImageFromCurrentImageContext();
        UIGraphics.EndImageContext();
        return circularImage;
    }
}
