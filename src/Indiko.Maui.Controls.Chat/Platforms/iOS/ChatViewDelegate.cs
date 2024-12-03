using CoreGraphics;
using Foundation;
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



    //public override void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
    //{
    //    try
    //    {
    //        base.ItemSelected(collectionView, indexPath);

    //        var message = _virtualView.Messages[indexPath.Row];

    //        if (_virtualView.MessageTappedCommand?.CanExecute(message) == true)
    //        {
    //            _virtualView.MessageTappedCommand.Execute(message);
    //        }

    //    }catch(Exception ex)
    //    {
    //        Console.WriteLine(ex.Message);
    //    }
    //}

    public override void Scrolled(UIScrollView scrollView)
    {
        try
        {
            base.Scrolled(scrollView);

            if (scrollView.ContentOffset.Y <= 0)
            {
                if (_virtualView.LoadMoreMessagesCommand?.CanExecute(null) == true)
                {
                    _virtualView.LoadMoreMessagesCommand.Execute(null);
                }
            }
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}