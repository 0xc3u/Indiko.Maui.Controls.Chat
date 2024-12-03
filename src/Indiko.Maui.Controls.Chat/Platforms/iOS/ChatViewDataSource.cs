using System.Collections.ObjectModel;
using Foundation;
using Indiko.Maui.Controls.Chat.Models;
using UIKit;

namespace Indiko.Maui.Controls.Chat.Platforms.iOS;

public class ChatViewDataSource : UICollectionViewDataSource
{

    private readonly ChatView _virtualView;
    private readonly IMauiContext _mauiContext;
    private IList<ChatMessage> _messages;

    
    public ChatViewDataSource(ChatView virtualView, IMauiContext mauiContext)
    {
        _virtualView = virtualView;
        _mauiContext = mauiContext;
        _messages = virtualView.Messages;
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

            if (message.IsDateSeperator)
            {
                var cell = collectionView.DequeueReusableCell(DateGroupSeperatorCell.Key, indexPath) as DateGroupSeperatorCell;
                cell.Update((int)indexPath.Item, message, _virtualView, _mauiContext);
                return cell;
            }

            if (message.MessageType == MessageType.Text && message.IsOwnMessage)
            {
                var cell = collectionView.DequeueReusableCell(OwnTextMessageCell.Key, indexPath) as OwnTextMessageCell;
                cell.Update((int)indexPath.Item, message, _virtualView, _mauiContext);
                return cell;
            }
            else if(message.MessageType == MessageType.Text && !message.IsOwnMessage)
            {
                var cell = collectionView.DequeueReusableCell(OtherTextMessageCell.Key, indexPath) as OtherTextMessageCell;
                cell.Update((int)indexPath.Item, message, _virtualView, _mauiContext);
                return cell;
            }
            else if(message.MessageType == MessageType.Image && !message.IsOwnMessage)
            {
                var cell = collectionView.DequeueReusableCell(OtherImageMessageCell.Key, indexPath) as OtherImageMessageCell;
                cell.Update((int)indexPath.Item, message, _virtualView, _mauiContext);
                return cell;
            }
            else
            {
                var cell = collectionView.DequeueReusableCell(DateGroupSeperatorCell.Key, indexPath) as DateGroupSeperatorCell;
                cell.Update((int)indexPath.Item, message, _virtualView, _mauiContext);
                return cell;
            }

        }
        catch(Exception ex)
        {
            Console.WriteLine($"Error in {nameof(ChatViewDataSource)}.{nameof(GetCell)}: {ex.Message}");
            throw;
        }
    }
}