using Android.Views;
using AndroidX.RecyclerView.Widget;
using Indiko.Maui.Controls.Chat.Models;
using Microsoft.Maui.Handlers;

namespace Indiko.Maui.Controls.Chat.Platforms.Android;
public class ChatViewHandler : ViewHandler<ChatView, RecyclerView>
{
    public static IPropertyMapper<ChatView, ChatViewHandler> Mapper = new PropertyMapper<ChatView, ChatViewHandler>(ViewHandler.ViewMapper)
    {
        [nameof(ChatView.Messages)] = MapAdapter,
        [nameof(ChatView.OwnMessageBackgroundColor)] = MapAdapter,
        [nameof(ChatView.OtherMessageBackgroundColor)] = MapAdapter,
        [nameof(ChatView.DateTextColor)] = MapAdapter,
        [nameof(ChatView.MessageTimeTextColor)] = MapAdapter,
        [nameof(ChatView.NewMessagesSeperatorTextColor)] = MapAdapter,
        [nameof(ChatView.MessageFontSize)] = MapAdapter,
        [nameof(ChatView.DateTextFontSize)] = MapAdapter,
        [nameof(ChatView.MessageTimeFontSize)] = MapAdapter,
        [nameof(ChatView.NewMessagesSeperatorFontSize)] = MapAdapter,
        [nameof(ChatView.AvatarSize)] = MapAdapter,
        [nameof(ChatView.ScrollToFirstNewMessage)] = MapAdapter,

    };

    public ChatViewHandler() : base(Mapper)
    {
    }

    protected override RecyclerView CreatePlatformView()
    {
        var recyclerView = new RecyclerView(Context)
        {
            LayoutParameters = new RecyclerView.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.MatchParent)
        };

        var layoutManager = new LinearLayoutManager(Context);
        recyclerView.SetLayoutManager(layoutManager);

        // Add a scroll listener to detect when the user scrolls to the top
        recyclerView.AddOnScrollListener(new OnScrollListener(() =>
        {
            VirtualView?.LoadMoreMessages(); // Trigger the event in ChatView
        }));

        RenderMessages(recyclerView);
        return recyclerView;
    }

    private static void MapAdapter(ChatViewHandler handler, ChatView chatView)
    {
        handler.RenderMessages(handler.PlatformView);
    }

    private void RenderMessages(RecyclerView recyclerView)
    {
        if (VirtualView.Messages == null)
            return;

        var adapter = new ChatMessageAdapter(Context, VirtualView.Messages)
        {
            OwnMessageBackgroundColor = VirtualView.OwnMessageBackgroundColor,
            OtherMessageBackgroundColor = VirtualView.OtherMessageBackgroundColor,
            OwnMessageTextColor = VirtualView.OwnMessageTextColor,
            OtherMessageTextColor = VirtualView.OtherMessageTextColor,
            DateTextColor = VirtualView.DateTextColor,
            MessageTimeTextColor = VirtualView.MessageTimeTextColor,
            DateTextFontSize = VirtualView.DateTextFontSize,
            MessageTimeFontSize = VirtualView.MessageTimeFontSize,
            MessageFontSize = VirtualView.MessageFontSize,
            NewMessagesSeperatorFontSize = VirtualView.NewMessagesSeperatorFontSize,
            NewMessagesSeperatorTextColor = VirtualView.NewMessagesSeperatorTextColor,
            NewMessagesSeperatorText = VirtualView.NewMessagesSeperatorText,
            AvatarSize = VirtualView.AvatarSize,
            ScrollToFirstNewMessage = VirtualView.ScrollToFirstNewMessage
        };

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
    private readonly Action _onScrolledToTop;

    public OnScrollListener(Action onScrolledToTop)
    {
        _onScrolledToTop = onScrolledToTop;
    }

    public override void OnScrolled(RecyclerView recyclerView, int dx, int dy)
    {
        base.OnScrolled(recyclerView, dx, dy);

        var layoutManager = recyclerView.GetLayoutManager() as LinearLayoutManager;
        if (layoutManager != null && layoutManager.FindFirstVisibleItemPosition() == 0)
        {
            _onScrolledToTop.Invoke();
        }
    }
}