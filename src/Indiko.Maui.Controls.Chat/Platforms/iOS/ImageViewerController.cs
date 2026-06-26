using System.Linq;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Indiko.Maui.Controls.Chat.Platforms.iOS;

/// <summary>
/// Full-screen image viewer with pinch-to-zoom, pan and double-tap-to-zoom, presented when the
/// user taps an image message.
/// </summary>
internal sealed class ImageViewerController : UIViewController, IUIScrollViewDelegate
{
    private readonly UIImage _image;
    private UIScrollView _scrollView;
    private UIImageView _imageView;

    public ImageViewerController(UIImage image)
    {
        _image = image;
        ModalPresentationStyle = UIModalPresentationStyle.OverFullScreen;
        ModalTransitionStyle = UIModalTransitionStyle.CrossDissolve;
    }

    public override void ViewDidLoad()
    {
        base.ViewDidLoad();
        View.BackgroundColor = UIColor.Black;

        _scrollView = new UIScrollView(View.Bounds)
        {
            AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight,
            MinimumZoomScale = 1f,
            MaximumZoomScale = 5f,
            ShowsHorizontalScrollIndicator = false,
            ShowsVerticalScrollIndicator = false,
            WeakDelegate = this
        };
        View.AddSubview(_scrollView);

        _imageView = new UIImageView(_image)
        {
            ContentMode = UIViewContentMode.ScaleAspectFit,
            Frame = _scrollView.Bounds,
            AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight
        };
        _scrollView.AddSubview(_imageView);

        // Single tap dismisses; double tap toggles zoom.
        var doubleTap = new UITapGestureRecognizer(OnDoubleTap) { NumberOfTapsRequired = 2 };
        _scrollView.AddGestureRecognizer(doubleTap);

        var singleTap = new UITapGestureRecognizer(() => DismissViewController(true, null)) { NumberOfTapsRequired = 1 };
        singleTap.RequireGestureRecognizerToFail(doubleTap);
        _scrollView.AddGestureRecognizer(singleTap);

        var closeButton = new UIButton(UIButtonType.System)
        {
            TintColor = UIColor.White,
            BackgroundColor = UIColor.FromWhiteAlpha(0f, 0.35f)
        };
        closeButton.SetImage(UIImage.GetSystemImage("xmark")?.ApplyTintColor(UIColor.White), UIControlState.Normal);
        closeButton.Layer.CornerRadius = 18;
        closeButton.TranslatesAutoresizingMaskIntoConstraints = false;
        closeButton.TouchUpInside += (_, _) => DismissViewController(true, null);
        View.AddSubview(closeButton);

        NSLayoutConstraint.ActivateConstraints(new[]
        {
            closeButton.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, 8),
            closeButton.LeadingAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeadingAnchor, 12),
            closeButton.WidthAnchor.ConstraintEqualTo(36),
            closeButton.HeightAnchor.ConstraintEqualTo(36),
        });
    }

    [Export("viewForZoomingInScrollView:")]
    public UIView ViewForZoomingInScrollView(UIScrollView scrollView) => _imageView;

    private void OnDoubleTap(UITapGestureRecognizer recognizer)
    {
        if (_scrollView.ZoomScale > _scrollView.MinimumZoomScale)
        {
            _scrollView.SetZoomScale(_scrollView.MinimumZoomScale, true);
        }
        else
        {
            var point = recognizer.LocationInView(_imageView);
            var targetScale = _scrollView.MaximumZoomScale / 2f;
            var size = _scrollView.Bounds.Size;
            var w = size.Width / targetScale;
            var h = size.Height / targetScale;
            var rect = new CGRect(point.X - w / 2f, point.Y - h / 2f, w, h);
            _scrollView.ZoomToRect(rect, true);
        }
    }

    public static void Present(UIImage image)
    {
        if (image == null) return;

        var window = UIApplication.SharedApplication.ConnectedScenes
            .OfType<UIWindowScene>()
            .SelectMany(s => s.Windows)
            .FirstOrDefault(w => w.IsKeyWindow)
            ?? UIApplication.SharedApplication.KeyWindow;

        var controller = window?.RootViewController;
        while (controller?.PresentedViewController != null)
            controller = controller.PresentedViewController;

        controller?.PresentViewController(new ImageViewerController(image), true, null);
    }
}
