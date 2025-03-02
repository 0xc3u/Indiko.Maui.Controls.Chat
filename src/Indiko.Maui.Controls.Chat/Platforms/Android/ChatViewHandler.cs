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
    private PopupMenu _contextMenu;
    private LinearLayout _emojiPanel;
    private LinearLayout _contextPanel;
    private ChatMessage _selectedMessage;

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

    private void HandleReply(ChatMessage message)
    {
        VirtualView?.LongPressedCommand?.Execute(new ContextAction { Name = "Reply", Message = message });
        DismissContextMenu();
    }

    private void HandleDelete(ChatMessage message)
    {
        VirtualView?.LongPressedCommand?.Execute(new ContextAction { Name = "Delete", Message = message });
        VirtualView?.Messages.Remove(message); // Remove the message from the collection
        DismissContextMenu();
    }

    private void HandleEmojiReaction(ChatMessage message, string emoji)
    {
        VirtualView?.LongPressedCommand?.Execute(new ContextAction { Name = "React", Message = message, AdditionalData=emoji });
        var existingReaction = message.Reactions.FirstOrDefault(r => r.Emoji == emoji);
        if (existingReaction != null)
        {
            existingReaction.Count++;
        }
        else
        {
            message.Reactions.Add(new ChatMessageReaction { Emoji = emoji, Count = 1 });
        }

        var index = VirtualView?.Messages.IndexOf(message);
        if (index >= 0)
        {
            _adapter.NotifyItemChanged(index.Value);
        }
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
        _selectedMessage = message;

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
        _focusedMessageView.TextSize = VirtualView.MessageFontSize;

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
        CreateContextPanel(message);

        _blurOverlay.Click += (s, e) => DismissContextMenu();
    }

    private void CreateEmojiPanel(ChatMessage message, LinearLayout parent)
    {
        // Create a HorizontalScrollView to enable scrolling
        var scrollView = new HorizontalScrollView(Context)
        {
            HorizontalScrollBarEnabled = false // Hide the scrollbar for cleaner UI
        };

        // Create a linear layout for emojis inside the scroll view
        var emojiPanel = new LinearLayout(Context)
        {
            Orientation = Orientation.Horizontal,
        };

        foreach (var emoji in VirtualView.EmojiReactions)
        {
            var emojiTextView = new TextView(Context)
            {
                Text = emoji,
                TextSize = 24f,
                Gravity = GravityFlags.Center
            };

            emojiTextView.SetPadding(16, 8, 16, 8);
            emojiTextView.Click += (s, e) => HandleEmojiReaction(message, emoji);
            emojiPanel.AddView(emojiTextView);
        }

        // Add the emoji panel to the scroll view
        scrollView.AddView(emojiPanel);

        // Add the scroll view to the parent container
        parent.AddView(scrollView);
    }


    private void AddDivider(LinearLayout parent)
    {
        var divider = new AViews.View(Context)
        {
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent, 2) // 2px height
        };
        divider.SetBackgroundColor(AGraphics.Color.LightGray);
        parent.AddView(divider);
    }

    private void AddMenuItem(LinearLayout parent, string text, Action onClick)
    {
        var menuItem = new TextView(Context)
        {
            Text = text,
            TextSize = 18f,
            Gravity = GravityFlags.CenterHorizontal
        };
        menuItem.SetPadding(32, 16, 32, 16);
        menuItem.SetTextColor(AGraphics.Color.Black);

        menuItem.Click += (s, e) =>
        {
            onClick.Invoke();
            DismissContextMenu();
        };

        parent.AddView(menuItem);
    }

    private void CreateContextPanel(ChatMessage message)
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
        backgroundDrawable.SetColor(AGraphics.Color.White);
        backgroundDrawable.SetCornerRadius(40f);
        _contextPanel.Background = backgroundDrawable;

        // Add Emoji Panel
        CreateEmojiPanel(message, _contextPanel);

        // Add horizontal divider
        AddDivider(_contextPanel);

        // Add menu items
        AddMenuItem(_contextPanel, "Reply", () => HandleReply(message));
        AddDivider(_contextPanel);
        AddMenuItem(_contextPanel, "Delete", () => HandleDelete(message));

        // Positioning: **Below the Highlighted Message**
        var location = new int[2];
        _messagePopupContainer.GetLocationOnScreen(location);
        int messageBottomY = location[1] + _messagePopupContainer.Height;

        var screenHeight = rootView.Height;

        // Ensure the panel does not go out of screen bounds
        int availableSpaceBelow = screenHeight - messageBottomY;
        int contextMenuHeight = 250; // Approximate height

        // Check if there is enough space below the message
        int topMargin;
        if (availableSpaceBelow > contextMenuHeight)
        {
            // Enough space below the message
            topMargin = messageBottomY + 20; // Add some spacing
        }
        else
        {
            // Not enough space, position it **above the message** instead
            topMargin = location[1] - contextMenuHeight - 20;
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
        (PlatformView.RootView as ViewGroup)?.RemoveView(_contextPanel); // Remove new context panel
        _selectedMessage = null;
        _focusedMessageView = null;
        _contextPanel = null;
        _emojiPanel = null;
    }

}