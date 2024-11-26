using UIKit;
using CoreGraphics;
using Foundation;
using Microsoft.Maui.Graphics.Platform;

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

    public static UIImage CreateInitialsImage(string initials, nfloat width, nfloat height, UIColor textColor, CGColor backgroundColor)
    {
        UIGraphics.BeginImageContext(new CGSize(width, height));
        var context = UIGraphics.GetCurrentContext();
        var path = new CGPath();
        path.AddArc(width / 2, height / 2, width / 2, 0, (float)(2 * Math.PI), true);
        context.AddPath(path);
        context.Clip();


        context.SetFillColor(backgroundColor);
        context.FillPath();

        var textFont = UIFont.BoldSystemFontOfSize(width / 3);
        var textSize = new NSString(initials).GetSizeUsingAttributes(new UIStringAttributes { Font = textFont });
        var textPoint = new CGPoint((width - textSize.Width) / 2, (height - textSize.Height) / 2);

        initials.DrawString(textPoint, textFont);

        var image = UIGraphics.GetImageFromCurrentImageContext();
        UIGraphics.EndImageContext();
        return image;
    }

    public static UIImage GetImageFromImageSource(IMauiContext mauiContext, ImageSource imageSource)
    {
        UIImage uIImage = null;

        if (imageSource == null && mauiContext == null)
            return null;

        try
        {
            ImageSourceExtensions.LoadImage(imageSource, mauiContext, (img) =>
            {
                if (img.Value != null)
                {
                    uIImage = img.Value;
                }
            });
        }
        catch (Exception ex)
        {
            // Handle exceptions, e.g., invalid image sources
            Console.WriteLine($"Error resolving ImageSource: {ex.Message}");
        }
        return uIImage;
    }

}
