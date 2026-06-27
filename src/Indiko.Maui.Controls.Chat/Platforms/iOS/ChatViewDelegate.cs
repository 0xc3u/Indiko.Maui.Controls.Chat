using CoreGraphics;
using Foundation;
using UIKit;

namespace Indiko.Maui.Controls.Chat.Platforms.iOS;

public class ChatViewDelegate : UICollectionViewDelegateFlowLayout
{
    private readonly ChatView _virtualView;
    private readonly IMauiContext _mauiContext;
    private readonly ChatViewFlowLayout _flowLayout;

    // Invoked on every scroll so the handler can update the scroll-to-bottom button.
    public Action<UIScrollView> ScrollChanged;

    public ChatViewDelegate(ChatView virtualView, IMauiContext mauiContext, ChatViewFlowLayout chatViewFlowLayout)
    {
        _virtualView = virtualView;
        _mauiContext = mauiContext;
        _flowLayout = chatViewFlowLayout;
    }


    // Self-sizing is handled by each cell via PreferredLayoutAttributesFittingAttributes.
    // Returning UICollectionViewFlowLayout.AutomaticSize here defers to the cells,
    // which is required now that EstimatedItemSize is non-zero in the flow layout.

    public override void Scrolled(UIScrollView scrollView)
    {
        try
        {
            ScrolledArgs args = new()
            {
                Y = (int)scrollView.ContentOffset.Y,
                X = (int)scrollView.ContentOffset.X
            };

            if (_virtualView.ScrolledCommand?.CanExecute(args) == true)
            {
                _virtualView.ScrolledCommand.Execute(args);
            }

            ScrollChanged?.Invoke(scrollView);

            // With the inverted UICollectionView, contentOffset.Y = 0 is the visual bottom
            // (newest messages) and contentOffset.Y = max is the visual top (oldest messages).
            if (scrollView.ContentOffset.Y <= 0)
            {
                if (_virtualView.ScrolledToLastMessageCommand?.CanExecute(null) == true)
                {
                    _virtualView.ScrolledToLastMessageCommand.Execute(null);
                }
            }
            else if (scrollView.ContentOffset.Y >= scrollView.ContentSize.Height - scrollView.Bounds.Height)
            {
                if (_virtualView.LoadMoreMessagesCommand?.CanExecute(null) == true)
                {
                    _virtualView.LoadMoreMessagesCommand.Execute(null);
                }
            }
        }
        catch(Exception ex)
        {
            Console.WriteLine($"Error in {nameof(ChatViewDelegate)}.{nameof(Scrolled)}: {ex.Message}");
        }
    }
}