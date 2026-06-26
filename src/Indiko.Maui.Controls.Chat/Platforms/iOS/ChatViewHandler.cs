using System.Collections.Specialized;
using CoreAnimation;
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
    private bool _didInitialJump;

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

        // Initial populate without animation, then defer the initial scroll
        // to after the snapshot has been applied and the layout pass has
        // finalized. This ensures ContentSize reflects real cell heights
        // (via PreferredLayoutAttributesFittingAttributes), not estimates.
        _dataSource.UpdateMessages(VirtualView.Messages, animate: false, completion: () =>
        {
            TryInitialJump();
        });

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

        // PREPEND: keep viewport stable by adjusting offset *after* layout
        // completes. Computing the delta synchronously is wrong because
        // ContentSize still reflects estimated heights for off-screen cells.
        // The ApplySnapshot completion fires after the batch update finishes,
        // so ContentSize is accurate there.
        if (e.Action == NotifyCollectionChangedAction.Add && e.NewStartingIndex == 0)
        {
            var oldOffset = PlatformView.ContentOffset;
            var oldHeight = PlatformView.ContentSize.Height;

            _dataSource.UpdateMessages(chatView.Messages, animate: false, completion: () =>
            {
                PlatformView.LayoutIfNeeded();
                var newHeight = PlatformView.ContentSize.Height;
                var delta = newHeight - oldHeight;
                PlatformView.SetContentOffset(new CGPoint(oldOffset.X, oldOffset.Y + delta), false);
            });

            return;
        }

        // APPEND: only auto-scroll if the user is already near the bottom.
        // Use animated:false to avoid the "swoosh down" when a new message arrives.
        if (e.Action == NotifyCollectionChangedAction.Add && e.NewStartingIndex >= (chatView.Messages.Count - 1))
        {
            _dataSource.UpdateMessages(chatView.Messages, animate: false, completion: () =>
            {
                if (IsNearBottom())
                {
                    ScrollToNewest(animated: false);
                }
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

    private void TryInitialJump()
    {
        if (_didInitialJump) return;
        _didInitialJump = true;

        // Called from the ApplySnapshot completion handler, so layout is
        // already finalized and ContentSize is accurate. No need for
        // another LayoutIfNeeded or PerformBatchUpdates here.
        if (VirtualView.ScrollToFirstNewMessage)
            ScrollToFirstNewMessage(animated: false);
        else
            JumpToBottom(animated: false);
    }

    private void JumpToBottom(bool animated)
    {
        var count = VirtualView.Messages?.Count ?? 0;
        if (count <= 0) return;

        CATransaction.Begin();
        CATransaction.DisableActions = !animated;

        var bottomY = Math.Max(
            0,
            PlatformView.ContentSize.Height
            - PlatformView.Bounds.Height
            + PlatformView.AdjustedContentInset.Bottom);

        PlatformView.SetContentOffset(new CGPoint(0, bottomY), animated);

        CATransaction.Commit();
    }

    /// <summary>
    /// Returns true when the user is currently near the bottom of the chat.
    /// Used to decide whether to auto-scroll when a new message arrives.
    /// </summary>
    private bool IsNearBottom()
    {
        var contentHeight = PlatformView.ContentSize.Height;
        var visibleHeight = PlatformView.Bounds.Height
            - PlatformView.AdjustedContentInset.Top
            - PlatformView.AdjustedContentInset.Bottom;

        if (visibleHeight <= 0) return true;

        var currentOffset = PlatformView.ContentOffset.Y + PlatformView.AdjustedContentInset.Top;
        var distanceFromBottom = contentHeight - currentOffset - visibleHeight;

        // Within half a screen height of the bottom → near bottom
        return distanceFromBottom <= visibleHeight * 0.5;
    }

}
