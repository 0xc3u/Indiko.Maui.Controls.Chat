using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using CoreGraphics;
using Foundation;
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
        SectionInset = new UIEdgeInsets(10, 0, 10, 0);
    }

    public override UICollectionViewLayoutAttributes LayoutAttributesForItem(NSIndexPath path)
    {
        var layoutAttributes = base.LayoutAttributesForItem(path);

        NFloat width = CollectionView.Bounds.Width - SectionInset.Left - SectionInset.Right;

        layoutAttributes.Frame = new CGRect(SectionInset.Left, layoutAttributes.Frame.Y, width, layoutAttributes.Frame.Height);
        return layoutAttributes;
    }
        
}
