using CoreGraphics;
using Foundation;
using UIKit;

namespace Indiko.Maui.Controls.Chat.Platforms.iOS;

public class ChatViewFlowLayout : UICollectionViewFlowLayout
{
    private nfloat _calculatedItemWidth;

    public ChatViewFlowLayout() : base()
    {
        EstimatedItemSize = UICollectionViewFlowLayout.AutomaticSize;
        MinimumInteritemSpacing = 0f;
        MinimumLineSpacing = 15f;
        ScrollDirection = UICollectionViewScrollDirection.Vertical;
        SectionInset = new UIEdgeInsets(10, 0, 10, 0);
    }

    public override void PrepareLayout()
    {
        base.PrepareLayout();

        if (CollectionView?.Bounds.Width > 0)
        {
            // Calculate the item width based on the collection view bounds
            _calculatedItemWidth = CollectionView.Bounds.Width - SectionInset.Left - SectionInset.Right;
        }
    }

    public override UICollectionViewLayoutAttributes LayoutAttributesForItem(NSIndexPath indexPath)
    {
        var layoutAttributes = base.LayoutAttributesForItem(indexPath);

        var x = SectionInset.Left;
       

        
        if (_calculatedItemWidth > 0)
        {
            layoutAttributes.Frame = new CGRect(
                x,
                layoutAttributes.Frame.Y,
                _calculatedItemWidth,
                layoutAttributes.Frame.Height
            );
        }

        return layoutAttributes;
    }

    public override bool ShouldInvalidateLayoutForBoundsChange(CGRect newBounds)
    {
        // Recalculate item width when bounds change
        if (CollectionView != null && CollectionView.Bounds.Width != _calculatedItemWidth)
        {
            _calculatedItemWidth = CollectionView.Bounds.Width - SectionInset.Left - SectionInset.Right;
            return true;
        }
        return base.ShouldInvalidateLayoutForBoundsChange(newBounds);
    }
}