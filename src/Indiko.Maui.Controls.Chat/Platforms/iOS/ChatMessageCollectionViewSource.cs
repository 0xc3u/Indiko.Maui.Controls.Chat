using System.Collections.ObjectModel;
using System.Linq;
using CoreGraphics;
using Foundation;
using Indiko.Maui.Controls.Chat.Models;
using UIKit;

namespace Indiko.Maui.Controls.Chat.Platforms.iOS;

public class ChatMessageCollectionViewSource : UICollectionViewDataSource, IUICollectionViewDelegateFlowLayout, IUIScrollViewDelegate
{
    private readonly ChatView _chatView;
    private ObservableCollection<ChatMessage> _messages;

    public ChatMessageCollectionViewSource(ChatView chatView)
    {
        _chatView = chatView;
        _messages = new ObservableCollection<ChatMessage>();
    }

    public void UpdateMessages(ObservableCollection<ChatMessage> messages)
    {
        _messages = messages;
    }

    public override nint GetItemsCount(UICollectionView collectionView, nint section) => _messages?.Count ?? 0;

    public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
    {
        var message = _messages[indexPath.Row];
        var cell = (ChatMessageCell)collectionView.DequeueReusableCell(ChatMessageCell.Key, indexPath);

        cell.Update(message, _chatView);
        return cell;
    }

    [Export("collectionView:didSelectItemAtIndexPath:")]
    public void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
    {
        var message = _messages[indexPath.Row];
        //_chatView.RaiseMessageTappedEvent(message);

        if (_chatView.MessageTappedCommand?.CanExecute(message) == true)
        {
            _chatView.MessageTappedCommand.Execute(message);
        }
    }

    [Export("scrollViewDidScroll:")]
    public void Scrolled(UIScrollView scrollView)
    {

        ScrolledArgs args = new ()
        {
            Y = (int)scrollView.ContentOffset.Y,
            X = (int)scrollView.ContentOffset.X
        };

        if (_chatView.ScrolledCommand?.CanExecute(args) == true)
        {
            _chatView.ScrolledCommand.Execute(args);
        }

        if (scrollView.ContentOffset.Y <= 0)
        {

            if (_chatView.LoadMoreMessagesCommand?.CanExecute(null) == true)
            {
                _chatView.LoadMoreMessagesCommand.Execute(null);
            }
        }
    }
}

