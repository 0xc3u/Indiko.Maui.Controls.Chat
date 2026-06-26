using CoreGraphics;
using UIKit;

namespace Indiko.Maui.Controls.Chat.Platforms.iOS;

internal static class CellSizingHelper
{
    private static readonly Dictionary<string, nfloat> _heightCache = new();
    private static readonly object _lock = new();

    /// <summary>
    /// Returns fitting layout attributes for a self-sizing cell, caching the
    /// measured height by message ID so the expensive
    /// <see cref="UIView.SystemLayoutSizeFittingSize"/> AutoLayout solve
    /// runs only once per message instead of on every scroll frame.
    /// </summary>
    public static UICollectionViewLayoutAttributes CalculateFittingAttributes(
        UICollectionViewLayoutAttributes layoutAttributes,
        UIView contentView,
        string messageId)
    {
        if (!string.IsNullOrEmpty(messageId))
        {
            lock (_lock)
            {
                if (_heightCache.TryGetValue(messageId, out var cached))
                {
                    var cachedAttr = layoutAttributes.Copy() as UICollectionViewLayoutAttributes;
                    cachedAttr.Frame = new CGRect(0, cachedAttr.Frame.Y, layoutAttributes.Frame.Width, cached);
                    return cachedAttr;
                }
            }
        }

        contentView.SetNeedsLayout();
        contentView.LayoutIfNeeded();

        var widthConstraint = contentView.WidthAnchor.ConstraintEqualTo(layoutAttributes.Frame.Width);
        widthConstraint.Active = true;
        var size = contentView.SystemLayoutSizeFittingSize(UIView.UILayoutFittingCompressedSize);
        widthConstraint.Active = false;

        if (!string.IsNullOrEmpty(messageId))
        {
            lock (_lock)
            {
                _heightCache[messageId] = size.Height;
            }
        }

        var updated = layoutAttributes.Copy() as UICollectionViewLayoutAttributes;
        updated.Frame = new CGRect(0, updated.Frame.Y, layoutAttributes.Frame.Width, size.Height);
        return updated;
    }

    /// <summary>
    /// Clears the height cache. Call when the collection view width changes
    /// (e.g. device rotation) so heights are re-measured at the new width.
    /// </summary>
    public static void ClearCache()
    {
        lock (_lock)
        {
            _heightCache.Clear();
        }
    }
}
