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

    private void UpdateMessages()
    {
        if (_dataSource != null)
        {
            SaveScrollPosition(); // Save current position
            _dataSource.UpdateMessages(VirtualView.Messages);
            
            // Check if ScrollToFirstNewMessage is enabled
            if (VirtualView.ScrollToFirstNewMessage)
            {
                ScrollToFirstNewMessage();
            }
            else
            {
                RestoreScrollPosition(); // Restore position
            }
        }
    }

    private void SaveScrollPosition()
    {
        _lastContentOffset = PlatformView.ContentOffset;
    }

    private void RestoreScrollPosition()
    {
        PlatformView.SetContentOffset(_lastContentOffset, false);
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
        platformView.LayoutIfNeeded();
        _dataSource.UpdateMessages(VirtualView.Messages);

        // Check if ScrollToFirstNewMessage is enabled on initial load
        if (VirtualView.ScrollToFirstNewMessage)
        {
            ScrollToFirstNewMessage();
        }

        _weakChatView = new WeakReference<ChatView>(VirtualView);
        VirtualView.Messages.CollectionChanged += OnMessagesCollectionChanged;
    }

    private void OnMessagesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (_weakChatView.TryGetTarget(out var chatView))
        {
            _dataSource.UpdateMessages(chatView.Messages);

            // Scroll to the bottom when a new message is added
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                ScrollToLastMessage();
            }
        }
    }

    private void ScrollToLastMessage()
    {
        var lastIndexPath = NSIndexPath.FromRowSection(VirtualView.Messages.Count - 1, 0);
        PlatformView.ScrollToItem(lastIndexPath, UICollectionViewScrollPosition.Bottom, true);
    }

    private void ScrollToFirstNewMessage()
    {
        if (VirtualView?.Messages == null || VirtualView.Messages.Count == 0)
            return;

        // Find the index of the first message with ReadState == MessageReadState.New
        var firstNewMessageIndex = -1;
        for (int i = 0; i < VirtualView.Messages.Count; i++)
        {
            if (VirtualView.Messages[i].ReadState == Models.MessageReadState.New)
            {
                firstNewMessageIndex = i;
                break;
            }
        }

        // If a "New" message is found, scroll to its position
        if (firstNewMessageIndex >= 0)
        {
            var indexPath = NSIndexPath.FromRowSection(firstNewMessageIndex, 0);
            
            // Defer the scroll operation until after the UICollectionView has completed its layout
            // This ensures that all cells are properly sized and positioned before scrolling
            PlatformView.PerformBatchUpdates(() =>
            {
                // Empty batch update to ensure layout is complete
            }, (finished) =>
            {
                if (finished)
                {
                    // Scroll to the first new message with animation
                    PlatformView.ScrollToItem(indexPath, UICollectionViewScrollPosition.CenteredVertically, true);
                }
            });
        }
    }

    protected override void DisconnectHandler(UICollectionView nativeView)
    {
        if (_weakChatView.TryGetTarget(out var chatView))
        {
            chatView.Messages.CollectionChanged -= OnMessagesCollectionChanged;
        }
        base.DisconnectHandler(nativeView);
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