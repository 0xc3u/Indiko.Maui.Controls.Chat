using Android.Graphics;
using Paint = Android.Graphics.Paint;
using Rect = Android.Graphics.Rect;
using RectF = Android.Graphics.RectF;

namespace Indiko.Maui.Controls.Chat.Platforms.Android;

public static class BitmapUtils
{
    public static Bitmap CreateCircularBitmap(Bitmap bitmap)
    {
        int size = Math.Min(bitmap.Width, bitmap.Height);
        Bitmap output = Bitmap.CreateBitmap(size, size, Bitmap.Config.Argb8888);

        Canvas canvas = new Canvas(output);
        Paint paint = new Paint
        {
            AntiAlias = true,
            FilterBitmap = true
        };

        Rect srcRect = new Rect(0, 0, bitmap.Width, bitmap.Height);
        RectF destRect = new RectF(0, 0, size, size);
        canvas.DrawARGB(0, 0, 0, 0); // Transparent background
        canvas.DrawCircle(size / 2f, size / 2f, size / 2f, paint);

        paint.SetXfermode(new PorterDuffXfermode(PorterDuff.Mode.SrcIn));
        canvas.DrawBitmap(bitmap, srcRect, destRect, paint);

        return output;
    }

    
}
