using CoreGraphics;
using Foundation;
using Indiko.Maui.Controls.Chat.Models;
using Microsoft.Maui.Platform;
using UIKit;

namespace Indiko.Maui.Controls.Chat.Platforms.iOS;

public class SystemMessageCell : UICollectionViewCell
{
    public static readonly NSString Key = new(nameof(SystemMessageCell));
    private ChatView _chatView;
    private ChatMessage _message;

    private UIView _bubbleView;
    private UILabel _systemMessageLabel;

    public override UICollectionViewLayoutAttributes PreferredLayoutAttributesFittingAttributes(UICollectionViewLayoutAttributes layoutAttributes)
    {
        // Update the layout attributes for auto-sizing
        SetNeedsLayout();
        LayoutIfNeeded();

        var widthConstraint = ContentView.WidthAnchor.ConstraintEqualTo(layoutAttributes.Frame.Width);
        widthConstraint.Active = true;

        // Calculate the size fitting the content
        var size = ContentView.SystemLayoutSizeFittingSize(UIView.UILayoutFittingCompressedSize);
        widthConstraint.Active = false;

        var updatedAttributes = layoutAttributes.Copy() as UICollectionViewLayoutAttributes;
        updatedAttributes.Frame = new CGRect(0, updatedAttributes.Frame.Y, layoutAttributes.Frame.Width, size.Height);

        return updatedAttributes;
    }

    public SystemMessageCell(ObjCRuntime.NativeHandle handle) : base(handle)
    {
        SetupLayout();
    }

    private void SetupLayout()
    {
        _bubbleView = new UIView
        {
            TranslatesAutoresizingMaskIntoConstraints = false,
            BackgroundColor = UIColor.Gray,
            ClipsToBounds = true
        };
        _bubbleView.Layer.CornerRadius = 8;

        _systemMessageLabel = new UILabel
        {
            TranslatesAutoresizingMaskIntoConstraints = false,
            TextColor = UIColor.LightGray,
            BackgroundColor = UIColor.Clear,
            TextAlignment = UITextAlignment.Center,
            Font = UIFont.SystemFontOfSize(12),
            Lines = 0
        };
        ContentView.AddSubviews(_bubbleView, _systemMessageLabel);

        // Layout-Constraints
        NSLayoutConstraint.ActivateConstraints(new[]
        {
            
            // System message bubble
            _bubbleView.TopAnchor.ConstraintEqualTo(ContentView.TopAnchor, 10),
            _bubbleView.CenterXAnchor.ConstraintEqualTo(ContentView.CenterXAnchor),
            _bubbleView.BottomAnchor.ConstraintEqualTo(ContentView.BottomAnchor, -10),

            // System message text
            _systemMessageLabel.TopAnchor.ConstraintEqualTo(_bubbleView.TopAnchor, 10),
            _systemMessageLabel.BottomAnchor.ConstraintEqualTo(_bubbleView.BottomAnchor, -10),
            _systemMessageLabel.LeadingAnchor.ConstraintEqualTo(_bubbleView.LeadingAnchor, 10),
            _systemMessageLabel.TrailingAnchor.ConstraintEqualTo(_bubbleView.TrailingAnchor, -10),
        });

    }

    public void Update(int index, ChatMessage message, ChatView chatView, IMauiContext mauiContext)
    {
        if (chatView == null || message == null || mauiContext == null)
        {
            return;
        }

        _message = message;
        _chatView = chatView;

        try
        {
            _bubbleView.BackgroundColor = chatView.SystemMessageBackgroundColor.ToPlatform();

            _systemMessageLabel.Text = message.TextContent;
            _systemMessageLabel.TextColor = chatView.SystemMessageTextColor.ToPlatform();
            _systemMessageLabel.Font = UIFont.SystemFontOfSize(chatView.SystemMessageFontSize);

            var font = _systemMessageLabel.Font;
            var traits = font.FontDescriptor.SymbolicTraits | UIFontDescriptorSymbolicTraits.Bold;
            _systemMessageLabel.Font = UIFont.FromDescriptor(font.FontDescriptor.CreateWithTraits(traits), font.PointSize);


            // Ensure the bubble view is properly sized
            _bubbleView.SetNeedsLayout();
            _bubbleView.LayoutIfNeeded();

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in {nameof(SystemMessageCell)}.{nameof(Update)}: {ex.Message}");
        }
    }
}