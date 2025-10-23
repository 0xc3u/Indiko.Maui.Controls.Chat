using AViews = Android.Views;
using AGraphics = Android.Graphics;
using AndroidX.RecyclerView.Widget;
using Indiko.Maui.Controls.Chat.Models;
using Microsoft.Maui.Handlers;
using System.Collections.Specialized;
using Android.Widget;
using Android.Views;
using Android.Graphics.Drawables;
using Microsoft.Maui.Platform;

namespace Indiko.Maui.Controls.Chat.Platforms.Android;

public class ChatViewHandler : ViewHandler<ChatView, RecyclerView>
{
    private ChatMessageAdapter _adapter;
    private WeakReference<ChatView> _weakChatView;

    private BlurOverlayView _blurOverlay;
    private FrameLayout _messagePopupContainer;
    private TextView _focusedMessageView;
    private LinearLayout _emojiPanel;
    private LinearLayout _contextPanel;

    public static IPropertyMapper<ChatView, ChatViewHandler> PropertyMapper = new PropertyMapper<ChatView, ChatViewHandler>(ViewHandler.ViewMapper)
    {
        [nameof(ChatView.Messages)] = MapProperties,
        [nameof(ChatView.OwnMessageBackgroundColor)] = MapProperties,
        [nameof(ChatView.OtherMessageBackgroundColor)] = MapProperties,
        [nameof(ChatView.DateTextColor)] = MapProperties,
        [nameof(ChatView.MessageTimeTextColor)] = MapProperties,
        [nameof(ChatView.MessageFontSize)] = MapProperties,
        [nameof(ChatView.MessageSpacing)] = MapProperties,
        [nameof(ChatView.DateTextFontSize)] = MapProperties,
        [nameof(ChatView.MessageTimeFontSize)] = MapProperties,
        [nameof(ChatView.NewMessagesSeperatorFontSize)] = MapProperties,
        [nameof(ChatView.ShowNewMessagesSeperator)] = MapProperties,
        [nameof(ChatView.NewMessagesSeperatorTextColor)] = MapProperties,
        [nameof(ChatView.AvatarSize)] = MapProperties,
        [nameof(ChatView.ScrollToFirstNewMessage)] = MapProperties,
        [nameof(ChatView.AvatarBackgroundColor)] = MapProperties,
        [nameof(ChatView.AvatarTextColor)] = MapProperties,
        [nameof(ChatView.EmojiReactionFontSize)] = MapProperties,
        [nameof(ChatView.EmojiReactionTextColor)] = MapProperties,
        [nameof(ChatView.ReplyMessageBackgroundColor)] = MapProperties,
        [nameof(ChatView.ReplyMessageTextColor)] = MapProperties,
        [nameof(ChatView.ReplyMessageFontSize)] = MapProperties,
        [nameof(ChatView.SystemMessageBackgroundColor)] = MapProperties,
        [nameof(ChatView.SystemMessageFontSize)] = MapProperties,
        [nameof(ChatView.SystemMessageTextColor)] = MapProperties,

    };

    public static CommandMapper<ChatView, ChatViewHandler> CommandMapper = new CommandMapper<ChatView, ChatViewHandler>()
    {
        [nameof(ChatView.LoadMoreMessagesCommand)] = MapCommands,
        [nameof(ChatView.MessageTappedCommand)] = MapCommands,
        [nameof(ChatView.ScrolledCommand)] = MapCommands
    };

    public ChatViewHandler() : base(PropertyMapper, CommandMapper)
    {
    }

    protected override RecyclerView CreatePlatformView()
    {
        var recyclerView = new RecyclerView(Context)
        {
            LayoutParameters = new RecyclerView.LayoutParams(
                AViews.ViewGroup.LayoutParams.MatchParent,
                AViews.ViewGroup.LayoutParams.MatchParent)
        };

        recyclerView.AddItemDecoration(new SpacingItemDecoration(VirtualView.MessageSpacing));

        var layoutManager = new LinearLayoutManager(Context);
        recyclerView.SetLayoutManager(layoutManager);

        // Add a scroll listener to detect when the user scrolls to the top
        recyclerView.AddOnScrollListener(new OnScrollListener(
           onScrolled: (args) =>
             {
                VirtualView?.ScrolledCommand?.Execute(args); 
             },
            onScrolledToTop: () =>
            {
                VirtualView?.LoadMoreMessagesCommand?.Execute(null); 
            },
            onScrolledToBottom: () =>
            {
                VirtualView?.ScrolledToLastMessageCommand?.Execute(null);
            }
            ));

        RenderMessages(recyclerView, MauiContext);
        return recyclerView;
    }

    private static void MapCommands(ChatViewHandler handler, ChatView view, object? args)
    {

    }

    private static void MapProperties(ChatViewHandler handler, ChatView chatView)
    {
        handler.RenderMessages(handler.PlatformView, handler.MauiContext);
    }

    private void RenderMessages(RecyclerView recyclerView, IMauiContext mauiContext)
    {
        if (VirtualView.Messages == null)
            return;

        _adapter = new ChatMessageAdapter(Context, mauiContext, VirtualView,this);
        recyclerView.SetAdapter(_adapter);

        // Check if ScrollToFirstNewMessage is enabled and there are messages
        if (VirtualView.ScrollToFirstNewMessage)
        {
            // Find the position of the first "New" message
            var firstNewMessageIndex = VirtualView.Messages
                .TakeWhile(m => m.ReadState != MessageReadState.New)
                .Count();

            // If a "New" message is found, scroll to its position
            if (firstNewMessageIndex < VirtualView.Messages.Count)
            {
                recyclerView.Post(() => recyclerView.SmoothScrollToPosition(firstNewMessageIndex));
            }
        }

        // Create a weak reference to the ChatView
        _weakChatView = new WeakReference<ChatView>(VirtualView);

        // Listen for changes in the Messages collection using a weak event reference
        VirtualView.Messages.CollectionChanged += OnMessagesCollectionChanged;
    }

    private void OnMessagesCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
    {
        if (_weakChatView.TryGetTarget(out var chatView))
        {
            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Add:

                    foreach (var item in args.NewItems)
                    {
                        var index = chatView.Messages.IndexOf((ChatMessage)item);
                        _adapter.NotifyItemInserted(index);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var item in args.OldItems)
                    {
                        var index = args.OldStartingIndex;
                        _adapter.NotifyItemRemoved(index);
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    foreach (var item in args.NewItems)
                    {
                        var index = chatView.Messages.IndexOf((ChatMessage)item);
                        _adapter.NotifyItemChanged(index);
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    _adapter.NotifyItemMoved(args.OldStartingIndex, args.NewStartingIndex);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    _adapter.NotifyDataSetChanged();
                    break;
            }

            // Scroll to the bottom when a new message is added
            if (args.Action == NotifyCollectionChangedAction.Add)
            {
                PlatformView.Post(() => PlatformView.SmoothScrollToPosition(chatView.Messages.Count - 1));
            }
        }
    }

    private void HandleContextAction(ChatMessage message, string action)
    {
        VirtualView?.LongPressedCommand?.Execute(new ContextAction { Name = action, Message = message });
        DismissContextMenu();
    }

    private void HandleDestructiveAction(ChatMessage message, string action)
    {
        VirtualView?.LongPressedCommand?.Execute(new ContextAction { Name = action, Message = message });
        VirtualView?.Messages.Remove(message); // Remove the message from the collection
        DismissContextMenu();
    }

    private void HandleEmojiReaction(ChatMessage message, string emoji)
    {
        var existingReaction = message.Reactions.FirstOrDefault(r => r.Emoji == emoji);
        if (existingReaction != null)
        {
            existingReaction.Count++;
        }
        else
        {
            existingReaction = new ChatMessageReaction { Emoji = emoji, Count = 1 };
            message.Reactions.Add(existingReaction);
        }

        var index = VirtualView?.Messages.IndexOf(message);
        if (index >= 0)
        {
            _adapter.NotifyItemChanged(index.Value);
        }

        VirtualView?.LongPressedCommand?.Execute(new ContextAction { Name = "react", Message = message, AdditionalData = existingReaction });

        DismissContextMenu();
    }

    protected override void DisconnectHandler(RecyclerView platformView)
    {
        base.DisconnectHandler(platformView);
        if (_weakChatView.TryGetTarget(out var chatView))
        {
            chatView.Messages.CollectionChanged -= OnMessagesCollectionChanged;
        }
    }

    public void ShowContextMenu(ChatMessage message, AViews.View anchorView)
    {
        if(!VirtualView.EnableContextMenu)
        {
            return;
        }

        if (_blurOverlay == null)
        {
            _blurOverlay = new BlurOverlayView(Context);
        }

        var rootView = PlatformView.RootView;
        (rootView as ViewGroup)?.AddView(_blurOverlay, new ViewGroup.LayoutParams(
            ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent));

        _blurOverlay.ApplyBlur(rootView);

        if (_messagePopupContainer == null)
        {
            _messagePopupContainer = new FrameLayout(Context)
            {
                Background = new AGraphics.Drawables.ColorDrawable(AGraphics.Color.Transparent)
            };
        }

        AGraphics.Color? bgColor = message.IsOwnMessage ? VirtualView?.OwnMessageBackgroundColor.ToPlatform() : VirtualView?.OtherMessageBackgroundColor.ToPlatform();
        AGraphics.Color? textColor = message.IsOwnMessage ? VirtualView?.OwnMessageTextColor.ToPlatform() : VirtualView?.OtherMessageTextColor.ToPlatform();

        _focusedMessageView = new TextView(Context)
        {
            TextSize = 18f,
            TextAlignment = AViews.TextAlignment.TextStart,
            Gravity = GravityFlags.CenterHorizontal
        };
        _focusedMessageView.SetPadding(64, 32, 64, 32);
        _focusedMessageView.Text = message.TextContent;
        _focusedMessageView.SetTextColor(textColor.Value);
        _focusedMessageView.TextSize = (float)VirtualView.MessageFontSize;

        var backgroundDrawable = new GradientDrawable();
        backgroundDrawable.SetShape(ShapeType.Rectangle);
        backgroundDrawable.SetColor(bgColor.Value);
        backgroundDrawable.SetCornerRadius(40f);
        _focusedMessageView.Background = backgroundDrawable;

        _messagePopupContainer.RemoveAllViews();
        _messagePopupContainer.AddView(_focusedMessageView, new ViewGroup.LayoutParams(
            ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent));

        var layoutParams = new FrameLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
        layoutParams.Gravity = GravityFlags.Center;
        _messagePopupContainer.LayoutParameters = layoutParams;

        (rootView as ViewGroup)?.AddView(_messagePopupContainer);

        // Show new custom context panel
        CreateContextPanel(message, anchorView);

        _blurOverlay.Click += (s, e) => DismissContextMenu();
    }

    private void CreateEmojiPanel(ChatMessage message, LinearLayout parent)
    {
        var scrollView = new HorizontalScrollView(Context)
        {
            HorizontalScrollBarEnabled = false
        };

        var emojiPanel = new LinearLayout(Context)
        {
            Orientation = Orientation.Horizontal,
        };

        WeakReference<ChatViewHandler> weakHandler = new(this);

        foreach (var emoji in VirtualView.EmojiReactions)
        {
            var emojiTextView = new TextView(Context)
            {
                Text = emoji,
                TextSize = (float)VirtualView.ContextMenuReactionFontSize,
                Gravity = GravityFlags.Center
            };

            emojiTextView.SetPadding(16, 8, 16, 8);

            emojiTextView.Click += (s, e) =>
            {
                if (weakHandler.TryGetTarget(out var handler))
                {
                    handler.HandleEmojiReaction(message, emoji);
                }
            };

            emojiPanel.AddView(emojiTextView);
        }

        scrollView.AddView(emojiPanel);
        parent.AddView(scrollView);
    }

    private void AddDivider(LinearLayout parent)
    {
        var divider = new AViews.View(Context)
        {
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent, VirtualView.ContextMenuDividerHeight)
        };
        divider.SetBackgroundColor(VirtualView.ContextMenuDividerColor.ToPlatform());
        parent.AddView(divider);
    }

    private void AddMenuItem(LinearLayout parent, string text, bool isDestructive, Action onClick)
    {
        var menuItem = new TextView(Context)
        {
            Text = text,
            TextSize = (float)VirtualView.ContextMenuFontSize,
            Gravity = GravityFlags.CenterHorizontal
        };

        menuItem.SetPadding(32, 16, 32, 16);
        if (isDestructive)
        {
            menuItem.SetTextColor(VirtualView.ContextMenuDestructiveTextColor.ToPlatform());
        }
        else
        {
            menuItem.SetTextColor(VirtualView.ContextMenuTextColor.ToPlatform());
        }

        WeakReference<ChatViewHandler> weakHandler = new(this);

        menuItem.Click += (s, e) =>
        {
            if (weakHandler.TryGetTarget(out var handler))
            {
                onClick.Invoke();
                handler.DismissContextMenu();
            }
        };

        parent.AddView(menuItem);
    }

    private void CreateContextPanel(ChatMessage message, AViews.View anchorView)
    {
        var rootView = PlatformView.RootView;

        // Remove existing context panel if any
        if (_contextPanel != null)
        {
            (rootView as ViewGroup)?.RemoveView(_contextPanel);
        }

        // Create main panel
        _contextPanel = new LinearLayout(Context)
        {
            Orientation = Orientation.Vertical,
        };

        // Apply padding and rounded background
        _contextPanel.SetPadding(32, 16, 32, 16);
        var backgroundDrawable = new GradientDrawable();
        backgroundDrawable.SetShape(ShapeType.Rectangle);
        backgroundDrawable.SetColor(VirtualView.ContextMenuBackgroundColor.ToPlatform());
        backgroundDrawable.SetCornerRadius(40f);
        _contextPanel.Background = backgroundDrawable;

        // Add Emoji Panel
        CreateEmojiPanel(message, _contextPanel);

        // Add horizontal divider
        AddDivider(_contextPanel);


        if (VirtualView.ContextMenuItems.Count > 0)
        {
            int n = 0;
            foreach(ContextMenuItem item in VirtualView.ContextMenuItems)
            {
                if (item.IsDestructive)
                {
                    AddMenuItem(_contextPanel, item.Name, item.IsDestructive, () => HandleDestructiveAction(message, item.Tag));
                }
                else
                {
                    AddMenuItem(_contextPanel, item.Name, item.IsDestructive, () => HandleContextAction(message, item.Tag));
                }
                if (n < VirtualView.ContextMenuItems.Count - 1)
                {
                    AddDivider(_contextPanel);
                }
                n++;
            }
        }

        // Positioning: **Below the pressed message**
        var location = new int[2];
        anchorView.GetLocationOnScreen(location);
        int messageBottomY = location[1] + anchorView.Height;

        var screenHeight = rootView.Height;

        // Always position the context menu below the pressed message.
        // This avoids the previous behaviour where the menu could appear
        // above the message if there was not enough space below.
        int topMargin = messageBottomY + 20; // Add some spacing

        // Prevent the menu from going off screen when the message is near the
        // bottom. If there isn't enough space, clamp the position so that the
        // menu stays within the visible area while remaining below the message.
        int contextMenuHeight = 250; // Approximate height
        int maxTopMargin = screenHeight - contextMenuHeight - 20;
        if (topMargin > maxTopMargin)
        {
            topMargin = maxTopMargin;
        }

        // Set layout params
        var contextPanelLayoutParams = new FrameLayout.LayoutParams(
            ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
        contextPanelLayoutParams.TopMargin = topMargin;
        contextPanelLayoutParams.Gravity = GravityFlags.Top | GravityFlags.CenterHorizontal;
        _contextPanel.LayoutParameters = contextPanelLayoutParams;

        // Add context panel to the root view
        (rootView as ViewGroup)?.AddView(_contextPanel);
    }

    private void DismissContextMenu()
    {
        _blurOverlay?.ClearBlur();
        (PlatformView.RootView as ViewGroup)?.RemoveView(_blurOverlay);
        (PlatformView.RootView as ViewGroup)?.RemoveView(_messagePopupContainer);
        (PlatformView.RootView as ViewGroup)?.RemoveView(_contextPanel);

        _focusedMessageView = null;
        _contextPanel = null;

        if (_emojiPanel != null)
        {
            for (int i = 0; i < _emojiPanel.ChildCount; i++)
            {
                var child = _emojiPanel.GetChildAt(i);
                if (child is TextView textView)
                {
                    textView.Click -= (s, e) => { }; // Remove previous handlers
                }
            }
            _emojiPanel = null;
        }
    }
}