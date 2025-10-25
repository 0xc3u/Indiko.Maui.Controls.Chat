using System.Collections.ObjectModel;
using Foundation;
using Indiko.Maui.Controls.Chat.Models;
using UIKit;

namespace Indiko.Maui.Controls.Chat.Platforms.iOS;
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
            if (cell is SystemMessageCell systemMessageCell)
            {
                systemMessageCell.Update((int)indexPath.Item, chatMessage.Message, virtualView, mauiContext);
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
            else if (cell is OtherAudioMessageCell otherAudioCell)
            {
                otherAudioCell.Update((int)indexPath.Item, chatMessage.Message, virtualView, mauiContext);
            }
            else if (cell is OwnAudioMessageCell ownAudioCell)
            {
                ownAudioCell.Update((int)indexPath.Item, chatMessage.Message, virtualView, mauiContext);
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

    public void UpdateMessages(ObservableCollection<ChatMessage> messages, bool animate)
    {
        var snapshot = new NSDiffableDataSourceSnapshot<ChatSection, ChatMessageItem>();
        snapshot.AppendSections(new[] { new ChatSection("Messages") });
        snapshot.AppendItems(messages.Select(m => new ChatMessageItem(m)).ToArray());
        ApplySnapshot(snapshot, animate);
    }

    public void UpdateMessages(ObservableCollection<ChatMessage> messages, bool animate, Action? completion)
    {
        var snapshot = new NSDiffableDataSourceSnapshot<ChatSection, ChatMessageItem>();
        snapshot.AppendSections(new[] { new ChatSection("Messages") });
        snapshot.AppendItems(messages.Select(m => new ChatMessageItem(m)).ToArray());

        // IMPORTANT: use the completion overload so we know when layout can scroll
        ApplySnapshot(snapshot, animate, completion);
    }


    private static string GetCellIdentifier(ChatMessage message)
    {
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
        if (message.MessageType == MessageType.Audio && !message.IsOwnMessage)
        {
            return OtherAudioMessageCell.Key;
        }
        if (message.MessageType == MessageType.Video && message.IsOwnMessage)
        {
            return OwnVideoMessageCell.Key;
        }
        if (message.MessageType == MessageType.Audio && message.IsOwnMessage)
        {
            return OwnAudioMessageCell.Key;
        }

        if (message.MessageType == MessageType.System)
        {
            return SystemMessageCell.Key;
        }

        if (message.MessageType == MessageType.Date)
        {
            return DateGroupSeperatorCell.Key;
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