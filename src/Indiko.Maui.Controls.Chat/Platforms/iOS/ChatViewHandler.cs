using System.Collections.Specialized;
using CoreFoundation;
using CoreGraphics;
using Foundation;
using Indiko.Maui.Controls.Chat.Models;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Platform;
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
        [nameof(ChatView.OwnMessageBackgroundColor)] = MapProperties,
        [nameof(ChatView.OtherMessageBackgroundColor)] = MapProperties,
        [nameof(ChatView.DateTextColor)] = MapProperties,
        [nameof(ChatView.MessageTimeTextColor)] = MapProperties,
        [nameof(ChatView.NewMessagesSeperatorTextColor)] = MapProperties,
        [nameof(ChatView.MessageFontSize)] = MapProperties,
        [nameof(ChatView.DateTextFontSize)] = MapProperties,
        [nameof(ChatView.MessageTimeFontSize)] = MapProperties,
        [nameof(ChatView.NewMessagesSeperatorFontSize)] = MapProperties,
        [nameof(ChatView.AvatarSize)] = MapProperties,
        [nameof(ChatView.ScrollToFirstNewMessage)] = MapProperties,
        [nameof(ChatView.ScrollToLastMessage)] = MapProperties,
        [nameof(ChatView.AvatarBackgroundColor)] = MapProperties,
        [nameof(ChatView.AvatarTextColor)] = MapProperties,
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


        // Force layout update after initial load
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

            // Ensure UI updates are on the main thread
            DispatchQueue.MainQueue.DispatchAsync(() =>
            {
                _dataSource.UpdateMessages(VirtualView.Messages);
                PlatformView.ReloadData();

                RestoreScrollPosition(); // Restore position
            });
        }
    }


    private void ScrollToLastMessage()
    {
        if (VirtualView.ScrollToLastMessage)
        {
            PlatformView.ScrollToItem(NSIndexPath.FromRowSection(VirtualView.Messages.Count - 1, 0), UICollectionViewScrollPosition.Bottom, true);
        }
    }

    private void ScrollToFirstNewMessage()
    {
        if (VirtualView.ScrollToFirstNewMessage)
        {
            var index = VirtualView.Messages.TakeWhile(m => m.ReadState != MessageReadState.New).Count();
            if (index < VirtualView.Messages.Count)
            {
                PlatformView.ScrollToItem(NSIndexPath.FromRowSection(index, 0), UICollectionViewScrollPosition.Top, true);
            }
            else
            {
                ScrollToLastMessage();
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
        
        _dataSource = new ChatViewDataSource(VirtualView, MauiContext);
        _delegate = new ChatViewDelegate(VirtualView, MauiContext,_flowLayout);
        
        
        platformView.RegisterClassForCell(typeof(DateGroupSeperatorCell), DateGroupSeperatorCell.Key);
        platformView.RegisterClassForCell(typeof(OwnTextMessageCell), OwnTextMessageCell.Key);
        platformView.RegisterClassForCell(typeof(OtherTextMessageCell), OtherTextMessageCell.Key);
        platformView.RegisterClassForCell(typeof(OtherImageMessageCell), OtherImageMessageCell.Key);
        platformView.RegisterClassForCell(typeof(OwnImageMessageCell), OwnImageMessageCell.Key);
        platformView.RegisterClassForCell(typeof(OtherVideoMessageCell), OtherVideoMessageCell.Key);
        platformView.RegisterClassForCell(typeof(OwnVideoMessageCell), OwnVideoMessageCell.Key);

        
        platformView.DataSource = _dataSource;
        platformView.Delegate = _delegate;

        platformView.LayoutIfNeeded();
        platformView.ReloadData();
        
        // Create a weak reference to the ChatView
        _weakChatView = new WeakReference<ChatView>(VirtualView);

        // Listen for changes in the Messages collection using a weak event reference
        VirtualView.Messages.CollectionChanged += OnMessagesCollectionChanged;
    }

    private void OnMessagesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (_weakChatView.TryGetTarget(out var chatView))
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var item in e.NewItems)
                    {
                        var index = chatView.Messages.IndexOf((ChatMessage)item);
                        //.NotifyItemInserted(index);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var item in e.OldItems)
                    {
                        var index = e.OldStartingIndex;
                        //.NotifyItemRemoved(index);
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    foreach (var item in e.NewItems)
                    {
                        var index = chatView.Messages.IndexOf((ChatMessage)item);
                        //NotifyItemChanged(index);
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    //NotifyItemMoved(e.OldStartingIndex, e.NewStartingIndex);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    // NotifyDataSetChanged();
                    break;
            }

            // Scroll to the bottom when a new message is added
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
               
            }
        }
    }

    protected override void DisconnectHandler(UICollectionView nativeView)
    {
        nativeView.DataSource.Dispose();
        nativeView.Delegate.Dispose();
        nativeView.CollectionViewLayout.Dispose();
        
        _delegate.Dispose();
        _dataSource.Dispose();
        _flowLayout.Dispose();

        if (_weakChatView.TryGetTarget(out var chatView))
        {
            chatView.Messages.CollectionChanged -= OnMessagesCollectionChanged;
        }
        
        base.DisconnectHandler(nativeView);
    }
    
    
}