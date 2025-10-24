using System.Collections.Specialized;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using Microsoft.Maui.Handlers;
using UIKit;

namespace Indiko.Maui.Controls.Chat.Platforms.iOS;

public class ChatViewHandler : ViewHandler<ChatView, UICollectionView>
{
    private bool _didInitialJump;


    private ChatViewDataSource _dataSource;
    private ChatViewDelegate _delegate;
    private ChatViewFlowLayout _flowLayout;
    private CGPoint _lastContentOffset;
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
            AlwaysBounceVertical = true
        };
        collectionView.LayoutIfNeeded();
        return collectionView;
    }

    private static void MapProperties(ChatViewHandler handler, ChatView chatView)
    {
        handler.UpdateMessages();
    }

    private static void MapCommands(ChatViewHandler handler, ChatView chatView, object? args)
    {
        // Map command logic here if needed
    }

    //private void UpdateMessages()
    //{
    //    if (_dataSource != null)
    //    {
    //        SaveScrollPosition(); // Save current position
    //        _dataSource.UpdateMessages(VirtualView.Messages);

    //        // Check if ScrollToFirstNewMessage is enabled
    //        if (VirtualView.ScrollToFirstNewMessage)
    //        {
    //            ScrollToFirstNewMessage();
    //        }
    //        else
    //        {
    //            RestoreScrollPosition(); // Restore position
    //        }
    //    }
    //}

    private void UpdateMessages(bool animate = true)
    {
        if (_dataSource == null) return;

        SaveScrollPosition();
        _dataSource.UpdateMessages(VirtualView.Messages, animate);
        if (VirtualView.ScrollToFirstNewMessage)
            ScrollToFirstNewMessage(animated: false); // avoid swoosh on updates too
        else
            RestoreScrollPosition();
    }

    private void SaveScrollPosition()
    {
        _lastContentOffset = PlatformView.ContentOffset;
    }

    private void RestoreScrollPosition()
    {
        PlatformView.SetContentOffset(_lastContentOffset, false);
    }

    //protected override void ConnectHandler(UICollectionView platformView)
    //{
    //    base.ConnectHandler(platformView);

    //    _dataSource = new ChatViewDataSource(VirtualView, MauiContext, platformView);
    //    _delegate = new ChatViewDelegate(VirtualView, MauiContext, _flowLayout);

    //    platformView.RegisterClassForCell(typeof(DateGroupSeperatorCell), DateGroupSeperatorCell.Key);
    //    platformView.RegisterClassForCell(typeof(SystemMessageCell), SystemMessageCell.Key);
    //    platformView.RegisterClassForCell(typeof(OwnTextMessageCell), OwnTextMessageCell.Key);
    //    platformView.RegisterClassForCell(typeof(OwnImageMessageCell), OwnImageMessageCell.Key);
    //    platformView.RegisterClassForCell(typeof(OwnVideoMessageCell), OwnVideoMessageCell.Key);
    //    platformView.RegisterClassForCell(typeof(OtherTextMessageCell), OtherTextMessageCell.Key);
    //    platformView.RegisterClassForCell(typeof(OtherImageMessageCell), OtherImageMessageCell.Key);
    //    platformView.RegisterClassForCell(typeof(OtherVideoMessageCell), OtherVideoMessageCell.Key);
    //    platformView.RegisterClassForCell(typeof(OtherAudioMessageCell), OtherAudioMessageCell.Key);
    //    platformView.RegisterClassForCell(typeof(OwnAudioMessageCell), OwnAudioMessageCell.Key);

    //    platformView.Delegate = _delegate;
    //    platformView.LayoutIfNeeded();
    //    _dataSource.UpdateMessages(VirtualView.Messages);

    //    // Check if ScrollToFirstNewMessage is enabled on initial load
    //    if (VirtualView.ScrollToFirstNewMessage)
    //    {
    //        ScrollToFirstNewMessage();
    //    }

    //    _weakChatView = new WeakReference<ChatView>(VirtualView);
    //    VirtualView.Messages.CollectionChanged += OnMessagesCollectionChanged;
    //}

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

        // Initial populate WITHOUT animation to avoid flashing/jump
        UIView.PerformWithoutAnimation(() =>
        {
            _dataSource.UpdateMessages(VirtualView.Messages, animate: false);
            platformView.LayoutIfNeeded();
        });

        // One-time, no-animation initial jump
        TryInitialJump();

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


    //private void OnMessagesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    //{
    //    if (_weakChatView.TryGetTarget(out var chatView))
    //    {
    //        _dataSource.UpdateMessages(chatView.Messages);

    //        // Scroll to the bottom when a new message is added
    //        if (e.Action == NotifyCollectionChangedAction.Add)
    //        {
    //            ScrollToLastMessage();
    //        }
    //    }
    //}

    private void OnMessagesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (!_weakChatView.TryGetTarget(out var chatView)) return;

        // PREPEND: keep viewport stable
        if (e.Action == NotifyCollectionChangedAction.Add && e.NewStartingIndex == 0)
        {
            var oldOffset = PlatformView.ContentOffset;
            var oldHeight = PlatformView.ContentSize.Height;

            CATransaction.Begin();
            CATransaction.DisableActions = true;
            UIView.PerformWithoutAnimation(() =>
            {
                _dataSource.UpdateMessages(chatView.Messages, animate: false);
                PlatformView.LayoutIfNeeded();
            });
            CATransaction.Commit();

            var newHeight = PlatformView.ContentSize.Height;
            var delta = newHeight - oldHeight;
            PlatformView.SetContentOffset(new CGPoint(oldOffset.X, oldOffset.Y + delta), false);

            return;
        }

        // Default path: update (no animation to avoid flicker)
        UIView.PerformWithoutAnimation(() =>
        {
            _dataSource.UpdateMessages(chatView.Messages, animate: false);
            PlatformView.LayoutIfNeeded();
        });

        // APPEND: scroll to bottom (animated if you like)
        if (e.Action == NotifyCollectionChangedAction.Add && e.NewStartingIndex >= (chatView.Messages.Count - 1))
        {
            // New message at the end -> smooth scroll
            ScrollToLastMessage(animated: true);
        }
    }


    //private void ScrollToLastMessage()
    //{
    //    var lastIndexPath = NSIndexPath.FromRowSection(VirtualView.Messages.Count - 1, 0);
    //    PlatformView.ScrollToItem(lastIndexPath, UICollectionViewScrollPosition.Bottom, true);
    //}

    private void ScrollToLastMessage(bool animated)
    {
        var count = VirtualView.Messages?.Count ?? 0;
        if (count <= 0) return;

        var lastIndexPath = NSIndexPath.FromRowSection(count - 1, 0);
        PlatformView.ScrollToItem(lastIndexPath, UICollectionViewScrollPosition.Bottom, animated);
    }


    //private void ScrollToFirstNewMessage()
    //{
    //    if (VirtualView?.Messages == null || VirtualView.Messages.Count == 0)
    //        return;

    //    // Find the index of the first message with ReadState == MessageReadState.New
    //    var firstNewMessageIndex = -1;
    //    for (int i = 0; i < VirtualView.Messages.Count; i++)
    //    {
    //        if (VirtualView.Messages[i].ReadState == Models.MessageReadState.New)
    //        {
    //            firstNewMessageIndex = i;
    //            break;
    //        }
    //    }

    //    // If a "New" message is found, scroll to its position
    //    if (firstNewMessageIndex >= 0)
    //    {
    //        var indexPath = NSIndexPath.FromRowSection(firstNewMessageIndex, 0);

    //        // Defer the scroll operation until after the UICollectionView has completed its layout
    //        // This ensures that all cells are properly sized and positioned before scrolling
    //        PlatformView.PerformBatchUpdates(() =>
    //        {
    //            // Empty batch update to ensure layout is complete
    //        }, (finished) =>
    //        {
    //            if (finished)
    //            {
    //                // Scroll to the first new message with animation
    //                PlatformView.ScrollToItem(indexPath, UICollectionViewScrollPosition.CenteredVertically, true);
    //            }
    //        });
    //    }
    //}

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

        var indexPath = NSIndexPath.FromRowSection(firstNewIndex, 0);

        // Ensure layout is ready, then scroll (optionally without animation)
        PlatformView.PerformBatchUpdates(() => { }, _ =>
        {
            CATransaction.Begin();
            CATransaction.DisableActions = !animated;
            UIView.PerformWithoutAnimation(() =>
            {
                PlatformView.ScrollToItem(indexPath, UICollectionViewScrollPosition.CenteredVertically, animated);
            });
            CATransaction.Commit();
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
        PlatformView.LayoutIfNeeded();

        if (VirtualView.ScrollToFirstNewMessage)
            ScrollToFirstNewMessage(animated: false);
        else
            JumpToBottom(animated: false);

        _didInitialJump = true;
    }

    private void JumpToBottom(bool animated)
    {
        var count = VirtualView.Messages?.Count ?? 0;
        if (count <= 0) return;

        var path = NSIndexPath.FromRowSection(count - 1, 0);

        CATransaction.Begin();
        CATransaction.DisableActions = !animated;
        UIView.PerformWithoutAnimation(() =>
        {
            PlatformView.ScrollToItem(path, UICollectionViewScrollPosition.Bottom, animated);

            var bottomY = Math.Max(
                0,
                PlatformView.ContentSize.Height
                - PlatformView.Bounds.Height
                + PlatformView.AdjustedContentInset.Bottom);

            PlatformView.SetContentOffset(new CGPoint(0, bottomY), animated);
        });
        CATransaction.Commit();

    }

}