using System.Collections.Specialized;
using CoreGraphics;
using Foundation;
using Indiko.Maui.Controls.Chat.Models;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using UIKit;

namespace Indiko.Maui.Controls.Chat.Platforms.iOS;

public class ChatViewHandler : ViewHandler<ChatView, UICollectionView>
{
    private ChatViewDataSource _dataSource;
    private ChatViewDelegate _delegate;
    private ChatViewFlowLayout _flowLayout;
    private WeakReference<ChatView> _weakChatView;
    private System.Collections.Specialized.INotifyCollectionChanged _observedMessages;
    private bool _didInitialJump;

    public static CommandMapper<ChatView, ChatViewHandler> CommandMapper = new CommandMapper<ChatView, ChatViewHandler>()
    {
        [nameof(ChatView.LoadMoreMessagesCommand)] = MapCommands,
        [nameof(ChatView.MessageTappedCommand)] = MapCommands,
        [nameof(ChatView.ScrolledCommand)] = MapCommands
    };

    private ScrollToBottomButton _fab;
    private NSLayoutConstraint _fabWidth;
    private NSLayoutConstraint _fabHeight;
    private NSLayoutConstraint _fabTrailing;
    private NSLayoutConstraint _fabBottom;
    private int _unreadCount;

    public static IPropertyMapper<ChatView, ChatViewHandler> PropertyMapper = new PropertyMapper<ChatView, ChatViewHandler>(ViewHandler.ViewMapper)
    {
        [nameof(ChatView.Messages)] = MapProperties,
        [nameof(ChatView.ScrollToFirstNewMessage)] = MapProperties,
        [nameof(ChatView.ShowScrollToBottomButton)] = MapFab,
        [nameof(ChatView.ScrollToBottomButtonBackgroundColor)] = MapFab,
        [nameof(ChatView.ScrollToBottomButtonIconColor)] = MapFab,
        [nameof(ChatView.ScrollToBottomButtonSize)] = MapFab,
        [nameof(ChatView.ScrollToBottomButtonMargin)] = MapFab,
        [nameof(ChatView.ShowScrollToBottomBadge)] = MapFab,
        [nameof(ChatView.ScrollToBottomBadgeBackgroundColor)] = MapFab,
        [nameof(ChatView.ScrollToBottomBadgeTextColor)] = MapFab,
        [nameof(ChatView.ScrollToBottomBadgeFontSize)] = MapFab,
    };

    public ChatViewHandler() : base(PropertyMapper, CommandMapper)
    {
    }

    protected override UICollectionView CreatePlatformView()
    {
        _flowLayout = new ChatViewFlowLayout();
        var collectionView = new ChatCollectionView(CGRect.Empty, _flowLayout)
        {
            MovedToWindowAction = EnsureFab,
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

    private static void MapFab(ChatViewHandler handler, ChatView chatView)
    {
        handler.EnsureFab();
    }

    private static void MapCommands(ChatViewHandler handler, ChatView chatView, object? args)
    {
    }

    private void UpdateMessages(bool animate = false)
    {
        if (_dataSource == null) return;
        HookMessages();
        // Defer the scroll to the snapshot completion so ContentSize reflects real
        // cell heights before we position. Without ScrollToFirstNewMessage, rest at
        // the newest message (visual bottom = contentOffset 0 in the inverted layout).
        _dataSource.UpdateMessages(VirtualView.Messages, animate, completion: () =>
        {
            if (VirtualView.ScrollToFirstNewMessage)
                ScrollToFirstNewMessage(animated: false);
            else
                ScrollToNewest(animated: false);
        });
    }

    // Subscribe to CollectionChanged on the *current* Messages instance, moving the
    // subscription if the consumer assigns a new collection after ConnectHandler ran.
    // Without this, runtime add/prepend on a reassigned collection is never observed.
    private void HookMessages()
    {
        var current = VirtualView?.Messages;
        if (ReferenceEquals(current, _observedMessages)) return;

        if (_observedMessages != null)
            _observedMessages.CollectionChanged -= OnMessagesCollectionChanged;

        _observedMessages = current;

        if (_observedMessages != null)
            _observedMessages.CollectionChanged += OnMessagesCollectionChanged;
    }

    protected override void ConnectHandler(UICollectionView platformView)
    {
        base.ConnectHandler(platformView);

        _dataSource = new ChatViewDataSource(VirtualView, MauiContext, platformView);
        _delegate = new ChatViewDelegate(VirtualView, MauiContext, _flowLayout)
        {
            ScrollChanged = _ => UpdateFab(),
        };

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

        // Swipe a bubble sideways to reply.
        var replySwipe = new UISwipeGestureRecognizer(HandleSwipeToReply)
        {
            Direction = UISwipeGestureRecognizerDirection.Right
        };
        platformView.AddGestureRecognizer(replySwipe);

        // Initial populate without animation, then defer the initial scroll
        // to after the snapshot has been applied and the layout pass has
        // finalized. This ensures ContentSize reflects real cell heights
        // (via PreferredLayoutAttributesFittingAttributes), not estimates.
        _dataSource.UpdateMessages(VirtualView.Messages, animate: false, completion: () =>
        {
            TryInitialJump();
        });

        _weakChatView = new WeakReference<ChatView>(VirtualView);
        HookMessages();
        EnsureFab();

        VirtualView.RepliedMessageJumpRequested += OnJumpToRepliedMessage;
    }

    protected override void DisconnectHandler(UICollectionView nativeView)
    {
        if (_observedMessages != null)
        {
            _observedMessages.CollectionChanged -= OnMessagesCollectionChanged;
            _observedMessages = null;
        }
        if (VirtualView != null)
            VirtualView.RepliedMessageJumpRequested -= OnJumpToRepliedMessage;
        base.DisconnectHandler(nativeView);
    }

    // Scroll the original message into view (centered) and briefly flash it.
    private void OnJumpToRepliedMessage(ChatMessage original)
    {
        if (VirtualView?.Messages == null) return;

        var chrono = VirtualView.Messages.IndexOf(original);
        if (chrono < 0) return;

        // Reversed data source: chronological index i maps to (Count - 1 - i).
        var inverted = VirtualView.Messages.Count - 1 - chrono;
        var indexPath = NSIndexPath.FromRowSection(inverted, 0);
        PlatformView.ScrollToItem(indexPath, UICollectionViewScrollPosition.CenteredVertically, true);

        var highlight = VirtualView.RepliedMessageHighlightColor;
        if (highlight == null || highlight.Alpha <= 0) return;

        var color = highlight.ToPlatform();
        // Flash once the scroll has brought the cell on screen; retry while it animates in.
        FlashCellWhenReady(indexPath, color, 0);
    }

    private void FlashCellWhenReady(NSIndexPath indexPath, UIColor color, int attempt)
    {
        var cell = PlatformView.CellForItem(indexPath);
        if (cell == null)
        {
            if (attempt < 14)
                NSTimer.CreateScheduledTimer(0.12, false, _ => FlashCellWhenReady(indexPath, color, attempt + 1));
            return;
        }
        FlashCell(cell, color);
    }

    private void FlashCell(UICollectionViewCell cell, UIColor color)
    {

        // The cell's ContentView carries the inverse transform, so an overlay added here renders
        // upright over the whole row.
        var overlay = new UIView(cell.ContentView.Bounds)
        {
            BackgroundColor = color,
            UserInteractionEnabled = false,
            AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight,
        };
        overlay.Layer.CornerRadius = 8;
        cell.ContentView.AddSubview(overlay);

        UIView.Animate(0.9, 0.2, UIViewAnimationOptions.CurveEaseOut,
            () => overlay.Alpha = 0f,
            () => overlay.RemoveFromSuperview());
    }

    private void OnMessagesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (!_weakChatView.TryGetTarget(out var chatView)) return;

        // LOAD-MORE (older messages prepended at Messages[0]). In the reversed snapshot
        // these map to the *end* of the data (visual top), beyond the current viewport.
        // Appending there does not move any visible item, so NO contentOffset
        // compensation is needed — doing so would itself cause the dreaded jump.
        if (e.Action == NotifyCollectionChangedAction.Add && e.NewStartingIndex == 0)
        {
            UIView.PerformWithoutAnimation(() =>
            {
                _dataSource.UpdateMessages(chatView.Messages, animate: false, completion: () =>
                {
                    PlatformView.LayoutIfNeeded();
                });
            });
            return;
        }

        // NEW MESSAGE (appended at the end of Messages). In the reversed snapshot it is
        // inserted at index 0 (visual bottom), which pushes every existing item up in
        // content space. If the user is near the newest message, follow it; otherwise
        // compensate the offset by the inserted height so their view stays put.
        if (e.Action == NotifyCollectionChangedAction.Add)
        {
            var oldOffset = PlatformView.ContentOffset;
            var oldHeight = PlatformView.ContentSize.Height;
            var nearBottom = IsNearBottom();

            _dataSource.UpdateMessages(chatView.Messages, animate: false, completion: () =>
            {
                PlatformView.LayoutIfNeeded();
                if (nearBottom)
                {
                    // animated:false avoids the "swoosh down" when a message arrives.
                    ScrollToNewest(animated: false);
                }
                else
                {
                    var delta = PlatformView.ContentSize.Height - oldHeight;
                    PlatformView.SetContentOffset(new CGPoint(oldOffset.X, oldOffset.Y + delta), false);

                    // New message arrived while scrolled up: count it on the FAB badge.
                    _unreadCount++;
                    if (_fab != null) _fab.UnreadCount = _unreadCount;
                    UpdateFab();
                }
            });
            return;
        }

        // REPLACE of a single item (e.g. an in-place edit): the diffable keys cells by MessageId,
        // so reload that specific cell to reflect the changed content.
        if (e.Action == NotifyCollectionChangedAction.Replace && e.NewItems?.Count == 1
            && e.NewItems[0] is ChatMessage changed)
        {
            UIView.PerformWithoutAnimation(() => _dataSource.ReloadMessage(chatView.Messages, changed));
            return;
        }

        // Reset / Replace / Remove: re-apply the snapshot without forcing a scroll so the
        // user's current position is preserved (the diffable diff keeps cells in place).
        UIView.PerformWithoutAnimation(() =>
        {
            _dataSource.UpdateMessages(chatView.Messages, animate: false, completion: () =>
            {
                PlatformView.LayoutIfNeeded();
            });
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

    private void HandleSwipeToReply(UISwipeGestureRecognizer recognizer)
    {
        if (VirtualView == null || !VirtualView.EnableSwipeToReply) return;
        var command = VirtualView.LongPressedCommand;
        if (command == null) return;

        var indexPath = PlatformView.IndexPathForItemAtPoint(recognizer.LocationInView(PlatformView));
        if (indexPath == null) return;

        // Data source is reversed (index 0 = newest); map back to the chronological message.
        var chronoIndex = VirtualView.Messages.Count - 1 - (int)indexPath.Item;
        if (chronoIndex < 0 || chronoIndex >= VirtualView.Messages.Count) return;

        var message = VirtualView.Messages[chronoIndex];
        if (message.MessageType == Models.MessageType.Date || message.MessageType == Models.MessageType.System)
            return;

        // Raise the same event as the context menu's "Reply" item so consumers handle reply once.
        var action = new Models.ContextAction { Name = VirtualView.SwipeReplyActionName, Message = message };
        if (command.CanExecute(action))
        {
            command.Execute(action);
            using var feedback = new UIImpactFeedbackGenerator(UIImpactFeedbackStyle.Light);
            feedback.Prepare();
            feedback.ImpactOccurred();
        }
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
            ScrollToNewest(animated: false);

        // Superview exists once the first layout pass has run, so attach the FAB here too.
        EnsureFab();
    }

    // ---- Scroll-to-bottom button ----------------------------------------------------------------

    private void EnsureFab()
    {
        if (VirtualView == null) return;

        if (!VirtualView.ShowScrollToBottomButton)
        {
            if (_fab != null) _fab.Hidden = true;
            return;
        }

        if (_fab == null)
        {
            _fab = new ScrollToBottomButton { Hidden = true };
            _fab.Tapped = OnFabTapped;
        }

        // The FAB is a sibling of the collection view in its superview so it stays fixed while
        // the list scrolls. Superview may be null until the view is in the hierarchy.
        var parent = PlatformView?.Superview;
        if (_fab.Superview == null && parent != null)
        {
            parent.AddSubview(_fab);
            _fabWidth = _fab.WidthAnchor.ConstraintEqualTo(0);
            _fabHeight = _fab.HeightAnchor.ConstraintEqualTo(0);
            _fabTrailing = _fab.TrailingAnchor.ConstraintEqualTo(PlatformView.TrailingAnchor, 0);
            _fabBottom = _fab.BottomAnchor.ConstraintEqualTo(PlatformView.BottomAnchor, 0);
            NSLayoutConstraint.ActivateConstraints(new[] { _fabWidth, _fabHeight, _fabTrailing, _fabBottom });
        }

        ApplyFabStyle();
        UpdateFab();
    }

    private void ApplyFabStyle()
    {
        if (_fab == null || VirtualView == null) return;

        var size = (nfloat)VirtualView.ScrollToBottomButtonSize;
        var margin = (nfloat)VirtualView.ScrollToBottomButtonMargin;
        if (_fabWidth != null) _fabWidth.Constant = size;
        if (_fabHeight != null) _fabHeight.Constant = size;
        if (_fabTrailing != null) _fabTrailing.Constant = -margin;
        if (_fabBottom != null) _fabBottom.Constant = -margin;

        _fab.ApplyStyle(
            VirtualView.ScrollToBottomButtonBackgroundColor.ToPlatform(),
            VirtualView.ScrollToBottomButtonIconColor.ToPlatform(),
            VirtualView.ScrollToBottomBadgeBackgroundColor.ToPlatform(),
            VirtualView.ScrollToBottomBadgeTextColor.ToPlatform(),
            (nfloat)VirtualView.ScrollToBottomBadgeFontSize,
            VirtualView.ShowScrollToBottomBadge);
    }

    // Toggle visibility based on scroll position; reset the unread badge once the user is back
    // at the newest message.
    private void UpdateFab()
    {
        if (_fab == null || VirtualView == null) return;

        var nearBottom = IsNearBottom();
        if (nearBottom && _unreadCount != 0)
        {
            _unreadCount = 0;
            _fab.UnreadCount = 0;
        }

        var shouldShow = VirtualView.ShowScrollToBottomButton
            && (VirtualView.Messages?.Count ?? 0) > 0
            && !nearBottom;

        _fab.Hidden = !shouldShow;
    }

    private void OnFabTapped()
    {
        _unreadCount = 0;
        if (_fab != null)
        {
            _fab.UnreadCount = 0;
            _fab.Hidden = true;
        }
        ScrollToNewest(animated: true);
    }

    /// <summary>
    /// Returns true when the user is currently near the newest message (the visual
    /// bottom of the chat). In the inverted layout the newest message sits at
    /// contentOffset.Y ≈ 0, so "near bottom" means the offset is close to the top of
    /// the content. Used to decide whether to auto-scroll when a new message arrives.
    /// </summary>
    private bool IsNearBottom()
    {
        var visibleHeight = PlatformView.Bounds.Height
            - PlatformView.AdjustedContentInset.Top
            - PlatformView.AdjustedContentInset.Bottom;

        if (visibleHeight <= 0) return true;

        // 0 at the newest message; grows as the user scrolls toward older messages.
        var currentOffset = PlatformView.ContentOffset.Y + PlatformView.AdjustedContentInset.Top;

        // Within half a screen height of the newest message → near bottom.
        return currentOffset <= visibleHeight * 0.5;
    }

}
