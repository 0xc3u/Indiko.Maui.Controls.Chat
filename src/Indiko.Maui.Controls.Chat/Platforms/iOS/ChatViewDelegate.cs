using CoreGraphics;
using Foundation;
using Indiko.Maui.Controls.Chat.Models;
using UIKit;

namespace Indiko.Maui.Controls.Chat.Platforms.iOS;

public class ChatViewDelegate : UICollectionViewDelegateFlowLayout
{
    private readonly ChatView _virtualView;
    private readonly IMauiContext _mauiContext;
    private readonly ChatViewFlowLayout _flowLayout;

    public ChatViewDelegate(ChatView virtualView, IMauiContext mauiContext, ChatViewFlowLayout chatViewFlowLayout)
    {
        _virtualView = virtualView;
        _mauiContext = mauiContext;
        _flowLayout = chatViewFlowLayout;
    }


    public override CGSize GetSizeForItem(UICollectionView collectionView, UICollectionViewLayout layout, NSIndexPath indexPath)
    {
        var width = collectionView.Bounds.Width - _flowLayout.SectionInset.Left - _flowLayout.SectionInset.Right;
        nfloat estimatedHeight = 80; // chat bubble's average height
        return new CGSize(width, estimatedHeight);
    }



    public override void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
    {
        base.ItemSelected(collectionView, indexPath);
        
        var message = _virtualView.Messages[indexPath.Row];
        //_chatView.RaiseMessageTappedEvent(message);

        if (_virtualView.MessageTappedCommand?.CanExecute(message) == true)
        {
            _virtualView.MessageTappedCommand.Execute(message);
        }
    }
}