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
            RestoreScrollPosition(); // Restore position
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
        
        platformView.Delegate = _delegate;
        platformView.LayoutIfNeeded();
        _dataSource.UpdateMessages(VirtualView.Messages);

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

    protected override void DisconnectHandler(UICollectionView nativeView)
    {
        if (_weakChatView.TryGetTarget(out var chatView))
        {
            chatView.Messages.CollectionChanged -= OnMessagesCollectionChanged;
        }
        base.DisconnectHandler(nativeView);
    }
}