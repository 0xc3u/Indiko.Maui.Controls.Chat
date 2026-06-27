using CoreGraphics;
using Foundation;
using UIKit;

namespace Indiko.Maui.Controls.Chat.Platforms.iOS;

/// <summary>
/// Renders message text and makes detected URLs, phone numbers and emails tappable. Only
/// touches that land on a link are captured — everything else falls through to the bubble so
/// message-tap and long-press-to-react keep working on text messages.
/// </summary>
internal sealed class LinkTextView : UITextView
{
    public LinkTextView()
    {
        Editable = false;
        ScrollEnabled = false;
        Selectable = true; // required for link interaction
        BackgroundColor = UIColor.Clear;
        TextContainerInset = UIEdgeInsets.Zero;
        TextContainer.LineFragmentPadding = 0;
    }

    public void SetMessage(string text, UIFont font, UIColor textColor, UIColor linkColor, bool detectLinks)
    {
        var content = text ?? string.Empty;
        var attributed = new NSMutableAttributedString(content);
        var full = new NSRange(0, content.Length);

        if (content.Length > 0)
        {
            attributed.AddAttribute(UIStringAttributeKey.Font, font, full);
            attributed.AddAttribute(UIStringAttributeKey.ForegroundColor, textColor, full);

            if (detectLinks)
            {
                var detector = NSDataDetector.Create(NSTextCheckingType.Link | NSTextCheckingType.PhoneNumber, out var error);
                if (error == null && detector != null)
                {
                    foreach (var match in detector.GetMatches((NSString)content, (NSMatchingOptions)0, full))
                    {
                        var url = match.Url;
                        if (url == null && match.PhoneNumber != null)
                            url = new NSUrl("tel:" + match.PhoneNumber.Replace(" ", string.Empty));
                        if (url != null)
                            attributed.AddAttribute(UIStringAttributeKey.Link, url, match.Range);
                    }
                }
            }
        }

        WeakLinkTextAttributes = new UIStringAttributes
        {
            ForegroundColor = linkColor,
            UnderlineStyle = NSUnderlineStyle.Single
        }.Dictionary;

        AttributedText = attributed;
    }

    public override UIView HitTest(CGPoint point, UIEvent uievent)
    {
        return IsPointOnLink(point) ? base.HitTest(point, uievent) : null;
    }

    private bool IsPointOnLink(CGPoint point)
    {
        if (AttributedText == null || AttributedText.Length == 0)
            return false;

        var location = new CGPoint(point.X - TextContainerInset.Left, point.Y - TextContainerInset.Top);
        var index = LayoutManager.GetCharacterIndex(location, TextContainer, out _);
        if (index >= (nuint)AttributedText.Length)
            return false;

        return AttributedText.GetAttribute(UIStringAttributeKey.Link, (nint)index, out _) != null;
    }
}
