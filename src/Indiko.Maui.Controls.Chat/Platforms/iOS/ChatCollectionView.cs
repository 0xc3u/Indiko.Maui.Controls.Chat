using CoreGraphics;
using UIKit;

namespace Indiko.Maui.Controls.Chat.Platforms.iOS;

/// <summary>
/// The chat's inverted <see cref="UICollectionView"/>, subclassed only to signal when it has been
/// added to a window. That is the first point at which its <c>Superview</c> exists, so the handler
/// uses it to attach the floating scroll-to-bottom button as a sibling overlay.
/// </summary>
internal sealed class ChatCollectionView : UICollectionView
{
    public Action MovedToWindowAction;

    public ChatCollectionView(CGRect frame, UICollectionViewLayout layout) : base(frame, layout)
    {
    }

    public override void MovedToWindow()
    {
        base.MovedToWindow();
        if (Window != null)
            MovedToWindowAction?.Invoke();
    }
}
