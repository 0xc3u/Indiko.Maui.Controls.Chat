using AndroidX.RecyclerView.Widget;

namespace Indiko.Maui.Controls.Chat.Platforms.Android;

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
