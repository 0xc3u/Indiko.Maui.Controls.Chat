using Foundation;
using Indiko.Maui.Controls.Chat.Models;
using Microsoft.Maui.Platform;
using UIKit;

namespace Indiko.Maui.Controls.Chat.Platforms.iOS;
public class DateGroupSeperatorCell : UICollectionViewCell
{
    public static readonly NSString Key = new(nameof(DateGroupSeperatorCell));

    private UILabel _dateLabel;

    public DateGroupSeperatorCell(ObjCRuntime.NativeHandle handle) : base(handle)
    {
        SetupLayout();
    }

    private void SetupLayout()
    {
        _dateLabel = new UILabel
        {
            TranslatesAutoresizingMaskIntoConstraints = false,
            TextColor = UIColor.LightGray,
            BackgroundColor = UIColor.Clear,
            TextAlignment = UITextAlignment.Center,
            Font = UIFont.SystemFontOfSize(12)
        };
        ContentView.AddSubview(_dateLabel);

        _dateLabel.TopAnchor.ConstraintEqualTo(ContentView.TopAnchor).Active = true;
        _dateLabel.CenterXAnchor.ConstraintEqualTo(ContentView.CenterXAnchor).Active = true;
        _dateLabel.BottomAnchor.ConstraintEqualTo(ContentView.BottomAnchor).Active = true;
    }

    public void Update(int index, ChatMessage message, ChatView chatView, IMauiContext mauiContext)
    {
        if (chatView == null || message == null || mauiContext == null)
        {
            return;
        }

        try
        {
            _dateLabel.Text = message.TextContent;
            _dateLabel.TextColor = chatView.DateTextColor.ToPlatform();
            _dateLabel.Font = UIFont.SystemFontOfSize(chatView.DateTextFontSize);

            var font = _dateLabel.Font;
            var traits = font.FontDescriptor.SymbolicTraits | UIFontDescriptorSymbolicTraits.Bold;
            _dateLabel.Font = UIFont.FromDescriptor(font.FontDescriptor.CreateWithTraits(traits), font.PointSize);

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in {nameof(DateGroupSeperatorCell)}.{nameof(Update)}: {ex.Message}");
        }
    }
}