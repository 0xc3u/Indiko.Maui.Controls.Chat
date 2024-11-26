using CoreGraphics;
using Foundation;
using System.Runtime.InteropServices;
using UIKit;

namespace Indiko.Maui.Controls.Chat.Platforms.iOS;
internal class ChatCollectionViewLayout : UICollectionViewFlowLayout
{
    public ChatCollectionViewLayout() : base()
    {
        EstimatedItemSize = UICollectionViewFlowLayout.AutomaticSize;
        MinimumInteritemSpacing = 0f;
        MinimumLineSpacing = 10f;
        ScrollDirection = UICollectionViewScrollDirection.Vertical;
        SectionInset = new UIEdgeInsets(5, 0, 5, 0);
    }


    public override UICollectionViewLayoutAttributes InitialLayoutAttributesForAppearingItem(NSIndexPath itemIndexPath)
    {
        var layoutAttributes = base.InitialLayoutAttributesForAppearingItem(itemIndexPath);

        NFloat maxWidth = UIScreen.MainScreen.Bounds.Width;

        layoutAttributes.Frame = new CGRect(
                layoutAttributes.Frame.X,
                layoutAttributes.Frame.Y,
                maxWidth,
                layoutAttributes.Frame.Height
            );

        return layoutAttributes;
    }

    public override UICollectionViewLayoutAttributes LayoutAttributesForItem(NSIndexPath indexPath)
    {
        var layoutAttributes = base.LayoutAttributesForItem(indexPath);

        NFloat maxWidth = UIScreen.MainScreen.Bounds.Width * 0.65f;

        if (layoutAttributes.Frame.Width > maxWidth)
        {
            layoutAttributes.Frame = new CGRect(
                layoutAttributes.Frame.X,
                layoutAttributes.Frame.Y,
                maxWidth,
                layoutAttributes.Frame.Height
            );
        }

        return layoutAttributes;
    }
}
