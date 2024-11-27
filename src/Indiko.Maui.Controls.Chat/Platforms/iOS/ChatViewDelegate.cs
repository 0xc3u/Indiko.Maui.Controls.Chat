using CoreGraphics;
using Foundation;
using Indiko.Maui.Controls.Chat.Models;
using UIKit;

namespace Indiko.Maui.Controls.Chat.Platforms.iOS;

public class ChatViewDelegate : UICollectionViewDelegateFlowLayout
{
    private readonly ChatView _virtualView;
    private readonly IMauiContext _mauiContext;

    public ChatViewDelegate(ChatView virtualView, IMauiContext mauiContext)
    {
        _virtualView = virtualView;
        _mauiContext = mauiContext;
    }
  
    
    public override CGSize GetSizeForItem(UICollectionView collectionView, UICollectionViewLayout layout, NSIndexPath indexPath)
    {
        var message = _virtualView.Messages[indexPath.Row];
        
        
        var width = collectionView.Bounds.Width;
        var height = MeasureItemHeight(message, width);
        return new CGSize(width, height);
        
        // var width = collectionView.Bounds.Width;
        // return new CGSize(width, UICollectionViewFlowLayout.AutomaticSize.Height);
    }

    
    private nfloat MeasureItemHeight(ChatMessage message, nfloat width)
    {
        
        return 1;
        // Implement the logic to measure the height of the item based on the message content and width
        // This is a placeholder implementation
        // return UICollectionViewFlowLayout.AutomaticSize.Height;
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