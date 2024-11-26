using System.Collections.ObjectModel;
using CoreGraphics;
using Foundation;
using Indiko.Maui.Controls.Chat.Models;
using UIKit;

namespace Indiko.Maui.Controls.Chat.Platforms.iOS;

public class ChatMessageAdapter : UICollectionViewDataSource, IUICollectionViewDelegateFlowLayout, IUIScrollViewDelegate
{
    private readonly ChatView _chatView;
    private readonly IMauiContext _mauiContext;
    private IList<ChatMessage> _messages;

    public ChatMessageAdapter(ChatView chatView, IMauiContext mauiContext)
    {
        _chatView = chatView;
        _mauiContext = mauiContext;
        _messages = chatView.Messages;
    }

    public override nint NumberOfSections(UICollectionView collectionView)
        => 1;

    public void UpdateMessages(ObservableCollection<ChatMessage> messages)
    {
        _messages = messages;
    }

    public override nint GetItemsCount(UICollectionView collectionView, nint section) => _messages?.Count ?? 0;

    public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
    {
        try
        {
            var message = _messages[(int)indexPath.Item];

            if (message == null)
            {
                return new UICollectionViewCell();
            }

            if (message.IsOwnMessage)
            {
                var cell = collectionView.DequeueReusableCell(OwnMessageCell.Key, indexPath) as OwnMessageCell;
                cell.Update((int)indexPath.Item, message, _chatView, _mauiContext);
                return cell;
            }
            else
            {
                var cell = collectionView.DequeueReusableCell(OtherMessageCell.Key, indexPath) as OtherMessageCell;
                cell.Update((int)indexPath.Item, message, _chatView, _mauiContext);
                return cell;
            }

        }
        catch(Exception ex)
        {
            Console.WriteLine($"Error in {nameof(ChatMessageAdapter)}.{nameof(GetCell)}: {ex.Message}");
            throw;
        }
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
