using System.Collections.Specialized;
using CoreGraphics;
using Foundation;
using Microsoft.Maui.Handlers;
using UIKit;

namespace Indiko.Maui.Controls.Chat.Platforms.iOS;

public class ChatViewHandler : ViewHandler<ChatView, UICollectionView>
{
    private ChatViewDataSource _dataSource;
    private ChatViewDelegate _delegate;
    private ChatViewFlowLayout _flowLayout;
    private WeakReference<ChatView> _weakChatView;

    public static CommandMapper<ChatView, ChatViewHandler> CommandMapper = new CommandMapper<ChatView, ChatViewHandler>()
    {
        [nameof(ChatView.LoadMoreMessagesCommand)] = MapCommands,
        [nameof(ChatView.MessageTappedCommand)] = MapCommands,
        [nameof(ChatView.ScrolledCommand)] = MapCommands
    };

    public static IPropertyMapper<ChatView, ChatViewHandler> PropertyMapper = new PropertyMapper<ChatView, ChatViewHandler>(ViewHandler.ViewMapper)
    {
        [nameof(ChatView.Messages)] = MapProperties,
        [nameof(ChatView.ScrollToFirstNewMessage)] = MapProperties,
    };

    public ChatViewHandler() : base(PropertyMapper, CommandMapper)
    {
    }

    protected override UICollectionView CreatePlatformView()
    {
        _flowLayout = new ChatViewFlowLayout();
        var collectionView = new UICollectionView(CGRect.Empty, _flowLayout)
        {
            BackgroundColor = UIColor.Clear,
            AllowsSelection = true,
            AllowsMultipleSelection = false,
            BouncesHorizontally = false,
            BouncesVertically = true,
            AlwaysBounceVertical = true,
            // Inverted layout: item[0] (newest) appears at the visual bottom without any
            // programmatic scrolling. Each cell's ContentView carries the inverse transform
            // to restore correct visual orientation.
            Transform = CGAffineTransform.MakeScale(1, -1)
        };
        return collectionView;
    }

    private static void MapProperties(ChatViewHandler handler, ChatView chatView)
    {
        handler.UpdateMessages();
    }

    private static void MapCommands(ChatViewHandler handler, ChatView chatView, object? args)
    {
    }

    private void UpdateMessages(bool animate = false)
    {
        if (_dataSource == null) return;
        _dataSource.UpdateMessages(VirtualView.Messages, animate);
        if (VirtualView.ScrollToFirstNewMessage)
            ScrollToFirstNewMessage(animated: false);
    }

    protected override void ConnectHandler(UICollectionView platformView)
    {
        base.ConnectHandler(platformView);

        _dataSource = new ChatViewDataSource(VirtualView, MauiContext, platformView);
        _delegate = new ChatViewDelegate(VirtualView, MauiContext, _flowLayout);

        platformView.RegisterClassForCell(typeof(DateGroupSeperatorCell), DateGroupSeperatorCell.Key);
        platformView.RegisterClassForCell(typeof(SystemMessageCell), SystemMessageCell.Key);
        platformView.RegisterClassForCell(typeof(OwnTextMessageCell), OwnTextMessageCell.Key);
        platformView.RegisterClassForCell(typeof(OwnImageMessageCell), OwnImageMessageCell.Key);
        platformView.RegisterClassForCell(typeof(OwnVideoMessageCell), OwnVideoMessageCell.Key);
        platformView.RegisterClassForCell(typeof(OtherTextMessageCell), OtherTextMessageCell.Key);
        platformView.RegisterClassForCell(typeof(OtherImageMessageCell), OtherImageMessageCell.Key);
        platformView.RegisterClassForCell(typeof(OtherVideoMessageCell), OtherVideoMessageCell.Key);
        platformView.RegisterClassForCell(typeof(OtherAudioMessageCell), OtherAudioMessageCell.Key);
        platformView.RegisterClassForCell(typeof(OwnAudioMessageCell), OwnAudioMessageCell.Key);

        platformView.Delegate = _delegate;

        // With the inverted transform, item[0] is at the visual bottom by default — no
        // initial scroll-to-bottom is needed regardless of when bounds are finalised.
        UIView.PerformWithoutAnimation(() =>
        {
            _dataSource.UpdateMessages(VirtualView.Messages, animate: false);
            platformView.LayoutIfNeeded();
        });

        if (VirtualView.ScrollToFirstNewMessage)
            ScrollToFirstNewMessage(animated: false);

        _weakChatView = new WeakReference<ChatView>(VirtualView);
        VirtualView.Messages.CollectionChanged += OnMessagesCollectionChanged;
    }

    protected override void DisconnectHandler(UICollectionView nativeView)
    {
        if (_weakChatView.TryGetTarget(out var chatView))
        {
            chatView.Messages.CollectionChanged -= OnMessagesCollectionChanged;
        }
        base.DisconnectHandler(nativeView);
    }

    private void OnMessagesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (!_weakChatView.TryGetTarget(out var chatView)) return;

        // New outgoing/incoming message appended to the chronological list.
        // In the reversed data source it lands at index 0 (visual bottom).
        // The collection view is already positioned at contentOffset.Y≈0, so the
        // message appears there naturally; we just ensure we scroll into view.
        if (e.Action == NotifyCollectionChangedAction.Add
            && e.NewStartingIndex >= chatView.Messages.Count - 1)
        {
            _dataSource.UpdateMessages(chatView.Messages, animate: false, completion: () =>
            {
                PlatformView.PerformBatchUpdates(() => { }, _ => ScrollToNewest(animated: true));
            });
            return;
        }

        // Older messages prepended (infinite-scroll load-more).
        // In the reversed snapshot they are appended at the end (visual top).
        // Appending beyond the current viewport does NOT shift existing item positions,
        // so no contentOffset compensation is required.
        UIView.PerformWithoutAnimation(() =>
        {
            _dataSource.UpdateMessages(chatView.Messages, animate: false, completion: null);
            PlatformView.LayoutIfNeeded();
        });
    }

    // Scroll to contentOffset.Y = 0, which with the inverted transform is the visual bottom
    // (newest messages). No ScrollToItem needed — the position is always index 0.
    private void ScrollToNewest(bool animated)
    {
        if ((VirtualView.Messages?.Count ?? 0) <= 0) return;
        var topInset = -PlatformView.AdjustedContentInset.Top;
        PlatformView.SetContentOffset(new CGPoint(0, topInset), animated);
    }

    private void ScrollToFirstNewMessage(bool animated)
    {
        if (VirtualView?.Messages == null || VirtualView.Messages.Count == 0) return;

        var firstNewIndex = -1;
        for (int i = 0; i < VirtualView.Messages.Count; i++)
        {
            if (VirtualView.Messages[i].ReadState == Models.MessageReadState.New)
            {
                firstNewIndex = i;
                break;
            }
        }
        if (firstNewIndex < 0) return;

        // In the reversed data source, chronological index i maps to (Count - 1 - i).
        var invertedIndex = VirtualView.Messages.Count - 1 - firstNewIndex;
        var indexPath = NSIndexPath.FromRowSection(invertedIndex, 0);

        PlatformView.PerformBatchUpdates(() => { }, _ =>
        {
            PlatformView.ScrollToItem(indexPath, UICollectionViewScrollPosition.CenteredVertically, animated);
        });
    }

    private void ApplyBlurEffect()
    {
        var blurEffect = UIBlurEffect.FromStyle(UIBlurEffectStyle.Dark);
        var blurView = new UIVisualEffectView(blurEffect)
        {
            Frame = PlatformView.Bounds,
            AutoresizingMask = UIViewAutoresizing.FlexibleDimensions
        };
        PlatformView.InsertSubview(blurView, 0);
    }

    private void RemoveBlurEffect()
    {
        foreach (var subview in PlatformView.Subviews)
        {
            if (subview is UIVisualEffectView)
            {
                subview.RemoveFromSuperview();
            }
        }
    }
}
