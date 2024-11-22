using AViews = Android.Views;
using AGraphics = Android.Graphics;
using AndroidX.RecyclerView.Widget;
using Indiko.Maui.Controls.Chat.Models;
using Microsoft.Maui.Handlers;

namespace Indiko.Maui.Controls.Chat.Platforms.Android;
public class ChatViewHandler : ViewHandler<ChatView, RecyclerView>
{
    public static IPropertyMapper<ChatView, ChatViewHandler> PropertyMapper = new PropertyMapper<ChatView, ChatViewHandler>(ViewHandler.ViewMapper)
    {
        [nameof(ChatView.Messages)] = MapProperties,
        [nameof(ChatView.OwnMessageBackgroundColor)] = MapProperties,
        [nameof(ChatView.OtherMessageBackgroundColor)] = MapProperties,
        [nameof(ChatView.DateTextColor)] = MapProperties,
        [nameof(ChatView.MessageTimeTextColor)] = MapProperties,
        [nameof(ChatView.NewMessagesSeperatorTextColor)] = MapProperties,
        [nameof(ChatView.MessageFontSize)] = MapProperties,
        [nameof(ChatView.MessageSpacing)] = MapProperties,
        [nameof(ChatView.DateTextFontSize)] = MapProperties,
        [nameof(ChatView.MessageTimeFontSize)] = MapProperties,
        [nameof(ChatView.NewMessagesSeperatorFontSize)] = MapProperties,
        [nameof(ChatView.AvatarSize)] = MapProperties,
        [nameof(ChatView.ScrollToFirstNewMessage)] = MapProperties,
        [nameof(ChatView.AvatarBackgroundColor)] = MapProperties,
        [nameof(ChatView.AvatarTextColor)] = MapProperties,
        [nameof(ChatView.EmojiReactionFontSize)] = MapProperties,
        [nameof(ChatView.EmojiReactionTextColor)] = MapProperties,

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
             (args) =>
             {
                 VirtualView?.ScrolledCommand?.Execute(args); // Trigger the command in ChatView
             },
            () =>
            {
                VirtualView?.LoadMoreMessagesCommand?.Execute(null); // Trigger the command in ChatView
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

        var adapter = new ChatMessageAdapter(Context, mauiContext, VirtualView);

        recyclerView.SetAdapter(adapter);

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
    }

}

// Custom OnScrollListener to detect scrolling to the top
public class OnScrollListener : RecyclerView.OnScrollListener
{
    private readonly Action<ScrolledArgs> _onScrolled;
    private readonly Action _onScrolledToTop;

    public OnScrollListener(Action<ScrolledArgs> onScrolled, Action onScrolledToTop)
    {
        _onScrolled = onScrolled;
        _onScrolledToTop = onScrolledToTop;
    }

    public override void OnScrolled(RecyclerView recyclerView, int dx, int dy)
    {
        base.OnScrolled(recyclerView, dx, dy);

        var scrolledArgs = new ScrolledArgs { X = dx, Y = dy };
        _onScrolled.Invoke(scrolledArgs);

        var layoutManager = recyclerView.GetLayoutManager() as LinearLayoutManager;
        if (layoutManager != null && layoutManager.FindFirstVisibleItemPosition() == 0)
        {
            _onScrolledToTop.Invoke();
        }
    }
}


public class SpacingItemDecoration : RecyclerView.ItemDecoration
{
    private readonly int _verticalSpacing;

    public SpacingItemDecoration(int verticalSpacing)
    {
        _verticalSpacing = verticalSpacing;
    }

    public override void GetItemOffsets(AGraphics.Rect outRect, AViews.View view, RecyclerView parent, RecyclerView.State state)
    {
        // Apply vertical spacing to all items except the last one
        if (parent.GetChildAdapterPosition(view) != state.ItemCount - 1)
        {
            outRect.Bottom = _verticalSpacing;
        }

        // Optional: Add top margin for the first item
        if (parent.GetChildAdapterPosition(view) == 0)
        {
            outRect.Top = _verticalSpacing;
        }
    }
}
