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
        // Reversed so that index 0 = newest message, which sits at the visual bottom of the
        // inverted UICollectionView without requiring any programmatic scroll.
        snapshot.AppendItems(messages.Reverse().Select(m => new ChatMessageItem(m)).ToArray());
        ApplySnapshot(snapshot, true);
    }

    public void UpdateMessages(ObservableCollection<ChatMessage> messages, bool animate)
    {
        var snapshot = new NSDiffableDataSourceSnapshot<ChatSection, ChatMessageItem>();
        snapshot.AppendSections(new[] { new ChatSection("Messages") });
        snapshot.AppendItems(messages.Reverse().Select(m => new ChatMessageItem(m)).ToArray());
        ApplySnapshot(snapshot, animate);
    }

    public void UpdateMessages(ObservableCollection<ChatMessage> messages, bool animate, Action? completion)
    {
        var snapshot = new NSDiffableDataSourceSnapshot<ChatSection, ChatMessageItem>();
        snapshot.AppendSections(new[] { new ChatSection("Messages") });
        snapshot.AppendItems(messages.Reverse().Select(m => new ChatMessageItem(m)).ToArray());
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

    // Must override IsEqual (exported as isEqual:) and GetHashCode — the diffable
    // data source identifies items/sections natively via those, not via the managed
    // Equals. A `new` Equals is never seen by the diff, so every snapshot would be
    // treated as all-new identifiers and trigger a full reload.
    public override bool IsEqual(NSObject anObject)
    {
        if (ReferenceEquals(this, anObject)) return true;
        return anObject is ChatSection other && Title == other.Title;
    }

    // See ChatMessageItem: the diff hashes via native -hash (GetNativeHash), so it must
    // be overridden alongside GetHashCode and stay consistent with IsEqual.
    public override nuint GetNativeHash() => (nuint)(uint)(Title?.GetHashCode() ?? 0);

    public override int GetHashCode() => Title?.GetHashCode() ?? 0;
}

public class ChatMessageItem : NSObject
{
    public ChatMessage Message { get; }

    public ChatMessageItem(ChatMessage message)
    {
        Message = message;
    }

    // Identity is keyed on MessageId so the same logical message keeps a stable
    // identifier across snapshots (the ChatMessage instance is re-wrapped on every
    // update). This lets the diffable data source compute an incremental diff instead
    // of a full reload — essential for stable scroll position and no flicker.
    // Messages without an id (e.g. date separators) fall back to reference identity.
    public override bool IsEqual(NSObject anObject)
    {
        if (ReferenceEquals(this, anObject)) return true;
        if (anObject is not ChatMessageItem other) return false;
        if (string.IsNullOrEmpty(Message?.MessageId) || string.IsNullOrEmpty(other.Message?.MessageId))
            return ReferenceEquals(Message, other.Message);
        return string.Equals(Message.MessageId, other.Message.MessageId, System.StringComparison.Ordinal);
    }

    // The diffable data source hashes via the native -hash selector, which is bridged by
    // GetNativeHash() — NOT managed GetHashCode(). Two identifiers that IsEqual must also
    // return the same hash, so this mirrors IsEqual exactly: keyed on MessageId, or the
    // identity of the wrapped Message instance (not the wrapper) for id-less rows.
    private int StableHash() =>
        string.IsNullOrEmpty(Message?.MessageId)
            ? System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(Message)
            : Message.MessageId.GetHashCode();

    public override nuint GetNativeHash() => (nuint)(uint)StableHash();

    public override int GetHashCode() => StableHash();
}