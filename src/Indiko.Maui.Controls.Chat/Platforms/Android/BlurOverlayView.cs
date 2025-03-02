using Android.Content;
using Android.Graphics;
using Android.Renderscripts;
using Android.Util;
using Android.Widget;
using aContent = Android.Content;
using aGraphics = Android.Graphics;
using aViews = Android.Views;
using Element = Android.Renderscripts.Element;


namespace Indiko.Maui.Controls.Chat.Platforms.Android;

public class BlurOverlayView : FrameLayout
{
    private Bitmap _blurredBackground;
    private readonly aGraphics.Paint _paint = new aGraphics.Paint();
    private ImageView _blurImageView;

    public BlurOverlayView(Context context) : base(context)
    {
        Init();
    }

    public BlurOverlayView(Context context, IAttributeSet attrs) : base(context, attrs)
    {
        Init();
    }

    private void Init()
    {
        SetBackgroundColor(aGraphics.Color.Argb(100, 0, 0, 0)); // Semi-transparent background
        _blurImageView = new ImageView(Context);
        AddView(_blurImageView, new LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent));
    }

    public void SetBlurredBackground(Bitmap bitmap)
    {
        _blurredBackground = bitmap;
        _blurImageView.SetImageBitmap(bitmap);
    }

    public void ApplyBlur(aViews.View rootView)
    {
        if (_blurredBackground == null)
        {
            _blurredBackground = BlurHelper.CaptureAndBlur(rootView);
            _blurImageView.SetImageBitmap(_blurredBackground);
        }
    }

    public void ClearBlur()
    {
        _blurredBackground?.Recycle();
        _blurredBackground = null;
        _blurImageView.SetImageBitmap(null);
    }
}

public static class BlurHelper
{
    public static Bitmap CaptureAndBlur(aViews.View rootView)
    {
        var bitmap = Bitmap.CreateBitmap(rootView.Width, rootView.Height, Bitmap.Config.Argb8888);
        Canvas canvas = new Canvas(bitmap);
        rootView.Draw(canvas);

        return ApplyBlur(bitmap, rootView.Context);
    }

    private static Bitmap ApplyBlur(Bitmap bitmap, aContent.Context context)
    {
        RenderScript rs = RenderScript.Create(context);
        Allocation input = Allocation.CreateFromBitmap(rs, bitmap, Allocation.MipmapControl.MipmapNone, AllocationUsage.Script);
        Allocation output = Allocation.CreateTyped(rs, input.Type);

        ScriptIntrinsicBlur script = ScriptIntrinsicBlur.Create(rs, Element.U8_4(rs));
        script.SetInput(input);
        script.SetRadius(10); // Blur intensity
        script.ForEach(output);

        output.CopyTo(bitmap);

        rs.Destroy();
        return bitmap;
    }
}