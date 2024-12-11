using System.Collections.ObjectModel;
using Foundation;
using Indiko.Maui.Controls.Chat.Models;
using UIKit;

namespace Indiko.Maui.Controls.Chat.Platforms.iOS
{
    public class ChatViewDataSource : UICollectionViewDiffableDataSource<ChatSection, ChatMessageItem>
    {
        private readonly ChatView _virtualView;
        private readonly IMauiContext _mauiContext;

        public ChatViewDataSource(ChatView virtualView, IMauiContext mauiContext, UICollectionView collectionView)
            : base(collectionView, (collectionView, indexPath, item) =>
            {
                var chatMessage = item as ChatMessageItem;

                if (chatMessage == null)
                {
                    throw new System.Exception("ChatMessageItem is null");
                }

                var cell = collectionView.DequeueReusableCell(GetCellIdentifier(chatMessage.Message), indexPath) as UICollectionViewCell;
                if (cell is DateGroupSeperatorCell dateCell)
                {
                    dateCell.Update((int)indexPath.Item, chatMessage.Message, virtualView, mauiContext);
                }
                else if (cell is OwnTextMessageCell ownTextCell)
                {
                    ownTextCell.Update((int)indexPath.Item, chatMessage.Message, virtualView, mauiContext);
                }
                else if (cell is OtherTextMessageCell otherTextCell)
                {
                    otherTextCell.Update((int)indexPath.Item, chatMessage.Message, virtualView, mauiContext);
                }
                else if (cell is OtherImageMessageCell otherImageCell)
                {
                    otherImageCell.Update((int)indexPath.Item, chatMessage.Message, virtualView, mauiContext);
                }
                else if (cell is OwnImageMessageCell ownImageCell)
                {
                    ownImageCell.Update((int)indexPath.Item, chatMessage.Message, virtualView, mauiContext);
                }
                else if (cell is OtherVideoMessageCell otherVideoCell)
                {
                    otherVideoCell.Update((int)indexPath.Item, chatMessage.Message, virtualView, mauiContext);
                }
                else if (cell is OwnVideoMessageCell ownVideoCell)
                {
                    ownVideoCell.Update((int)indexPath.Item, chatMessage.Message, virtualView, mauiContext);
                }
                return cell;
            })
        {
            _virtualView = virtualView;
            _mauiContext = mauiContext;
        }

        public void UpdateMessages(ObservableCollection<ChatMessage> messages)
        {
            var snapshot = new NSDiffableDataSourceSnapshot<ChatSection, ChatMessageItem>();
            snapshot.AppendSections(new[] { new ChatSection("Messages") });
            snapshot.AppendItems(messages.Select(message => new ChatMessageItem(message)).ToArray());
            ApplySnapshot(snapshot, true);
        }

        private static string GetCellIdentifier(ChatMessage message)
        {
            if (message.IsDateSeperator)
            {
                return DateGroupSeperatorCell.Key;
            }
            if (message.MessageType == MessageType.Text && message.IsOwnMessage)
            {
                return OwnTextMessageCell.Key;
            }
            if (message.MessageType == MessageType.Text && !message.IsOwnMessage)
            {
                return OtherTextMessageCell.Key;
            }
            if (message.MessageType == MessageType.Image && !message.IsOwnMessage)
            {
                return OtherImageMessageCell.Key;
            }
            if (message.MessageType == MessageType.Image && message.IsOwnMessage)
            {
                return OwnImageMessageCell.Key;
            }
            if (message.MessageType == MessageType.Video && !message.IsOwnMessage)
            {
                return OtherVideoMessageCell.Key;
            }
            if (message.MessageType == MessageType.Video && message.IsOwnMessage)
            {
                return OwnVideoMessageCell.Key;
            }
            return DateGroupSeperatorCell.Key;
        }
    }

    public class ChatSection : NSObject
    {
        public string Title { get; }

        public ChatSection(string title)
        {
            Title = title;
        }

        public new bool Equals(NSObject obj)
        {
            if (obj is ChatSection other)
            {
                return Title.Equals(other.Title);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Title.GetHashCode();
        }
    }

    public class ChatMessageItem : NSObject
    {
        public ChatMessage Message { get; }

        public ChatMessageItem(ChatMessage message)
        {
            Message = message;
        }

        public new bool Equals(NSObject obj)
        {
            if (obj is ChatMessageItem other)
            {
                return Message.Equals(other.Message);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Message.GetHashCode();
        }
    }
}



//
// public class ChatViewDataSource : UICollectionViewDataSource
// {
//     private readonly ChatView _virtualView;
//     private readonly IMauiContext _mauiContext;
//     private IList<ChatMessage> _messages;
//
//
//     public ChatViewDataSource(ChatView virtualView, IMauiContext mauiContext)
//     {
//         _virtualView = virtualView;
//         _mauiContext = mauiContext;
//         _messages = virtualView.Messages;
//     }
//
//     public override nint NumberOfSections(UICollectionView collectionView)
//         => 1;
//
//     public void UpdateMessages(ObservableCollection<ChatMessage> messages)
//     {
//         _messages = messages;
//     }
//
//     public override nint GetItemsCount(UICollectionView collectionView, nint section) => _messages?.Count ?? 0;
//
//     public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
//     {
//         try
//         {
//             var message = _messages[(int)indexPath.Item];
//
//             if (message == null)
//             {
//                 return new UICollectionViewCell();
//             }
//
//             if (message.IsDateSeperator)
//             {
//                 var cell =
//                     collectionView.DequeueReusableCell(DateGroupSeperatorCell.Key, indexPath) as DateGroupSeperatorCell;
//                 cell.Update((int)indexPath.Item, message, _virtualView, _mauiContext);
//                 return cell;
//             }
//
//             if (message.MessageType == MessageType.Text && message.IsOwnMessage)
//             {
//                 var cell = collectionView.DequeueReusableCell(OwnTextMessageCell.Key, indexPath) as OwnTextMessageCell;
//                 cell.Update((int)indexPath.Item, message, _virtualView, _mauiContext);
//                 return cell;
//             }
//             else if (message.MessageType == MessageType.Text && !message.IsOwnMessage)
//             {
//                 var cell =
//                     collectionView.DequeueReusableCell(OtherTextMessageCell.Key, indexPath) as OtherTextMessageCell;
//                 cell.Update((int)indexPath.Item, message, _virtualView, _mauiContext);
//                 return cell;
//             }
//             else if (message.MessageType == MessageType.Image && !message.IsOwnMessage)
//             {
//                 var cell =
//                     collectionView.DequeueReusableCell(OtherImageMessageCell.Key, indexPath) as OtherImageMessageCell;
//                 cell.Update((int)indexPath.Item, message, _virtualView, _mauiContext);
//                 return cell;
//             }
//             else if (message.MessageType == MessageType.Image && message.IsOwnMessage)
//             {
//                 var cell =
//                     collectionView.DequeueReusableCell(OwnImageMessageCell.Key, indexPath) as OwnImageMessageCell;
//                 cell.Update((int)indexPath.Item, message, _virtualView, _mauiContext);
//                 return cell;
//             }
//             else if (message.MessageType == MessageType.Video && !message.IsOwnMessage)
//             {
//                 var cell =
//                     collectionView.DequeueReusableCell(OtherVideoMessageCell.Key, indexPath) as OtherVideoMessageCell;
//                 cell.Update((int)indexPath.Item, message, _virtualView, _mauiContext);
//                 return cell;
//             }
//             else if (message.MessageType == MessageType.Video && message.IsOwnMessage)
//             {
//                 var cell =
//                     collectionView.DequeueReusableCell(OwnVideoMessageCell.Key, indexPath) as OwnVideoMessageCell;
//                 cell.Update((int)indexPath.Item, message, _virtualView, _mauiContext);
//                 return cell;
//             }
//             else
//             {
//                 var cell =
//                     collectionView.DequeueReusableCell(DateGroupSeperatorCell.Key, indexPath) as DateGroupSeperatorCell;
//                 cell.Update((int)indexPath.Item, message, _virtualView, _mauiContext);
//                 return cell;
//             }
//         }
//         catch (Exception ex)
//         {
//             Console.WriteLine($"Error in {nameof(ChatViewDataSource)}.{nameof(GetCell)}: {ex.Message}");
//             throw;
//         }
//     }
// }