using AndroidX.RecyclerView.Widget;

namespace Indiko.Maui.Controls.Chat.Platforms.Android;

// Custom OnScrollListener to detect scrolling to the top
public class OnScrollListener : RecyclerView.OnScrollListener
{
    private readonly Action<ScrolledArgs> _onScrolled;
    private readonly Action _onScrolledToTop;
    private readonly Action _onScrolledToBottom;

    public OnScrollListener(Action<ScrolledArgs> onScrolled, Action onScrolledToTop, Action onScrolledToBottom)
    {
        _onScrolled = onScrolled;
        _onScrolledToTop = onScrolledToTop;
        _onScrolledToBottom = onScrolledToBottom;
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

        if (layoutManager != null && layoutManager.FindLastVisibleItemPosition() == (layoutManager.ItemCount - 1))
        {
            _onScrolledToBottom.Invoke();
        }
    }
}
