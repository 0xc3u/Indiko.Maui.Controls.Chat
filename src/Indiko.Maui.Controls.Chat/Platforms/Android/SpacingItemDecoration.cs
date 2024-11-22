using AViews = Android.Views;
using AGraphics = Android.Graphics;
using AndroidX.RecyclerView.Widget;

namespace Indiko.Maui.Controls.Chat.Platforms.Android;

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
