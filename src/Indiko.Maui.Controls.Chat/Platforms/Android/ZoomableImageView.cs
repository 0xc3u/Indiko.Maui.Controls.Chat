using Android.Content;
using Android.Views;
using AImageView = Android.Widget.ImageView;
using Matrix = Android.Graphics.Matrix;

namespace Indiko.Maui.Controls.Chat.Platforms.Android;

/// <summary>
/// An <see cref="AImageView"/> that supports pinch-to-zoom, pan and double-tap-to-zoom via a
/// transform matrix. Used by the full-screen image viewer.
/// </summary>
public sealed class ZoomableImageView : AImageView
{
    private readonly Matrix _matrix = new();
    private readonly float[] _values = new float[9];
    private ScaleGestureDetector _scaleDetector;
    private GestureDetector _tapDetector;
    private float _baseScale = 1f;
    private bool _initialized;

    private const float MaxZoom = 5f;
    private const float DoubleTapZoom = 2.5f;

    public ZoomableImageView(Context context) : base(context)
    {
        SetScaleType(ScaleType.Matrix);
        Clickable = true;
        _scaleDetector = new ScaleGestureDetector(context, new ScaleListener(this));
        _tapDetector = new GestureDetector(context, new TapListener(this));
    }

    protected override void OnLayout(bool changed, int left, int top, int right, int bottom)
    {
        base.OnLayout(changed, left, top, right, bottom);
        if (!_initialized && Drawable != null && Width > 0 && Height > 0)
        {
            FitCenter();
            _initialized = true;
        }
    }

    private void FitCenter()
    {
        if (Drawable == null) return;
        float dw = Drawable.IntrinsicWidth;
        float dh = Drawable.IntrinsicHeight;
        if (dw <= 0 || dh <= 0) return;

        _baseScale = Math.Min(Width / dw, Height / dh);
        _matrix.Reset();
        _matrix.PostScale(_baseScale, _baseScale);
        _matrix.PostTranslate((Width - dw * _baseScale) / 2f, (Height - dh * _baseScale) / 2f);
        ImageMatrix = _matrix;
    }

    public override bool OnTouchEvent(MotionEvent e)
    {
        _scaleDetector.OnTouchEvent(e);
        _tapDetector.OnTouchEvent(e);
        return true;
    }

    private float CurrentScale()
    {
        _matrix.GetValues(_values);
        return _values[Matrix.MscaleX];
    }

    private void ApplyScale(float factor, float focusX, float focusY)
    {
        var current = CurrentScale();
        var min = _baseScale;
        var max = _baseScale * MaxZoom;
        var target = current * factor;
        if (target < min) factor = min / current;
        else if (target > max) factor = max / current;

        _matrix.PostScale(factor, factor, focusX, focusY);
        FixTranslation();
        ImageMatrix = _matrix;
    }

    private void Pan(float dx, float dy)
    {
        _matrix.PostTranslate(-dx, -dy);
        FixTranslation();
        ImageMatrix = _matrix;
    }

    private void ToggleZoom(float focusX, float focusY)
    {
        if (CurrentScale() > _baseScale * 1.1f)
        {
            FitCenter();
        }
        else
        {
            ApplyScale(DoubleTapZoom, focusX, focusY);
        }
    }

    private void FixTranslation()
    {
        _matrix.GetValues(_values);
        var scale = _values[Matrix.MscaleX];
        var transX = _values[Matrix.MtransX];
        var transY = _values[Matrix.MtransY];
        var contentW = (Drawable?.IntrinsicWidth ?? 0) * scale;
        var contentH = (Drawable?.IntrinsicHeight ?? 0) * scale;
        _matrix.PostTranslate(FixTrans(transX, Width, contentW), FixTrans(transY, Height, contentH));
    }

    private static float FixTrans(float trans, float viewSize, float contentSize)
    {
        float min, max;
        if (contentSize <= viewSize)
        {
            min = max = (viewSize - contentSize) / 2f; // keep centered
        }
        else
        {
            min = viewSize - contentSize;
            max = 0;
        }
        if (trans < min) return min - trans;
        if (trans > max) return max - trans;
        return 0;
    }

    private sealed class ScaleListener(ZoomableImageView view) : ScaleGestureDetector.SimpleOnScaleGestureListener
    {
        public override bool OnScale(ScaleGestureDetector detector)
        {
            view.ApplyScale(detector.ScaleFactor, detector.FocusX, detector.FocusY);
            return true;
        }
    }

    private sealed class TapListener(ZoomableImageView view) : GestureDetector.SimpleOnGestureListener
    {
        public override bool OnDoubleTap(MotionEvent e)
        {
            view.ToggleZoom(e.GetX(), e.GetY());
            return true;
        }

        public override bool OnScroll(MotionEvent e1, MotionEvent e2, float distanceX, float distanceY)
        {
            view.Pan(distanceX, distanceY);
            return true;
        }
    }
}
