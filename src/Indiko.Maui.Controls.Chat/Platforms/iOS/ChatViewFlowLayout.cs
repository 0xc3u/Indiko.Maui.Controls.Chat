using CoreGraphics;
using Foundation;
using UIKit;

namespace Indiko.Maui.Controls.Chat.Platforms.iOS;

public class ChatViewFlowLayout : UICollectionViewFlowLayout
{
    private nfloat _calculatedItemWidth;

    public ChatViewFlowLayout() : base()
    {
        // Enable self-sizing via PreferredLayoutAttributesFittingAttributes.
        // A non-zero estimate lets UICollectionView compute ContentSize up-front
        // and only adjust incrementally (cheap) instead of from scratch.
        EstimatedItemSize = new CGSize(UIScreen.MainScreen.Bounds.Width, 80);
        MinimumInteritemSpacing = 0f;
        MinimumLineSpacing = 10f;
        ScrollDirection = UICollectionViewScrollDirection.Vertical;
        SectionInset = new UIEdgeInsets(10, 0, 10, 0);
    }

    public override void PrepareLayout()
    {
        base.PrepareLayout();

        if (CollectionView?.Bounds.Width > 0)
        {
            _calculatedItemWidth = CollectionView.Bounds.Width - SectionInset.Left - SectionInset.Right;
        }
    }
    public override UICollectionViewLayoutAttributes LayoutAttributesForItem(NSIndexPath indexPath)
    {
        var layoutAttributes = base.LayoutAttributesForItem(indexPath);

        if (_calculatedItemWidth > 0)
        {
            layoutAttributes.Frame = new CGRect(
                layoutAttributes.Frame.X,
                layoutAttributes.Frame.Y,
                _calculatedItemWidth,
                layoutAttributes.Frame.Height
            );
        }

        return layoutAttributes;
    }


    public override bool ShouldInvalidateLayoutForBoundsChange(CGRect newBounds)
    {
        // Only invalidate layout if width changes
        if (CollectionView != null && CollectionView.Bounds.Width != _calculatedItemWidth)
        {
            _calculatedItemWidth = CollectionView.Bounds.Width - SectionInset.Left - SectionInset.Right;
            CellSizingHelper.ClearCache();
            return true;
        }
        return false; // Do not invalidate during scrolling
    }


}