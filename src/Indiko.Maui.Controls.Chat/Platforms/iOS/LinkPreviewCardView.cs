using System;
using CoreGraphics;
using Foundation;
using Indiko.Maui.Controls.Chat.Models;
using Microsoft.Maui.Platform;
using UIKit;

namespace Indiko.Maui.Controls.Chat.Platforms.iOS;

/// <summary>
/// A tappable link-preview ("unfurl") card: an optional thumbnail across the top, then a site
/// label, title and description. Styled from the <see cref="ChatView"/> link-preview bindable
/// properties via <see cref="Configure"/>. Shared by the own/other text cells.
/// </summary>
internal sealed class LinkPreviewCardView : UIControl
{
    private const float ImageHeight = 140f;

    private readonly UIImageView _image;
    private readonly UILabel _site;
    private readonly UILabel _title;
    private readonly UILabel _desc;
    private readonly NSLayoutConstraint _imageHeight;

    public Action Tapped;

    public LinkPreviewCardView()
    {
        TranslatesAutoresizingMaskIntoConstraints = false;
        ClipsToBounds = true;
        Layer.CornerRadius = 10;

        _image = new UIImageView
        {
            TranslatesAutoresizingMaskIntoConstraints = false,
            ContentMode = UIViewContentMode.ScaleAspectFill,
            ClipsToBounds = true,
            UserInteractionEnabled = false,
        };
        _site = MakeLabel(1);
        _title = MakeLabel(2);
        _desc = MakeLabel(2);

        AddSubviews(_image, _site, _title, _desc);

        _imageHeight = _image.HeightAnchor.ConstraintEqualTo(0);

        NSLayoutConstraint.ActivateConstraints(new[]
        {
            _image.TopAnchor.ConstraintEqualTo(TopAnchor),
            _image.LeadingAnchor.ConstraintEqualTo(LeadingAnchor),
            _image.TrailingAnchor.ConstraintEqualTo(TrailingAnchor),
            _imageHeight,

            _site.TopAnchor.ConstraintEqualTo(_image.BottomAnchor, 8),
            _site.LeadingAnchor.ConstraintEqualTo(LeadingAnchor, 10),
            _site.TrailingAnchor.ConstraintEqualTo(TrailingAnchor, -10),

            _title.TopAnchor.ConstraintEqualTo(_site.BottomAnchor, 2),
            _title.LeadingAnchor.ConstraintEqualTo(LeadingAnchor, 10),
            _title.TrailingAnchor.ConstraintEqualTo(TrailingAnchor, -10),

            _desc.TopAnchor.ConstraintEqualTo(_title.BottomAnchor, 2),
            _desc.LeadingAnchor.ConstraintEqualTo(LeadingAnchor, 10),
            _desc.TrailingAnchor.ConstraintEqualTo(TrailingAnchor, -10),
            _desc.BottomAnchor.ConstraintEqualTo(BottomAnchor, -10),
        });

        AddTarget((s, e) => Tapped?.Invoke(), UIControlEvent.TouchUpInside);
    }

    private static UILabel MakeLabel(int lines) => new()
    {
        TranslatesAutoresizingMaskIntoConstraints = false,
        Lines = lines,
        LineBreakMode = UILineBreakMode.TailTruncation,
        UserInteractionEnabled = false,
    };

    public void Configure(LinkPreview preview, ChatView chatView)
    {
        BackgroundColor = chatView.LinkPreviewBackgroundColor.ToPlatform();

        SetText(_site, preview.SiteName, chatView.LinkPreviewSiteNameColor.ToPlatform(),
            UIFont.SystemFontOfSize((nfloat)chatView.LinkPreviewSiteNameFontSize));
        SetText(_title, preview.Title, chatView.LinkPreviewTitleColor.ToPlatform(),
            UIFont.BoldSystemFontOfSize((nfloat)chatView.LinkPreviewTitleFontSize));
        SetText(_desc, preview.Description, chatView.LinkPreviewDescriptionColor.ToPlatform(),
            UIFont.SystemFontOfSize((nfloat)chatView.LinkPreviewDescriptionFontSize));

        if (preview.ImageBytes is { Length: > 0 })
        {
            _image.Image = UIImage.LoadFromData(NSData.FromArray(preview.ImageBytes));
            _image.Hidden = false;
            _imageHeight.Constant = ImageHeight;
        }
        else
        {
            _image.Image = null;
            _image.Hidden = true;
            _imageHeight.Constant = 0;
        }
    }

    private static void SetText(UILabel label, string text, UIColor color, UIFont font)
    {
        label.Text = text ?? string.Empty;
        label.TextColor = color;
        label.Font = font;
        label.Hidden = string.IsNullOrEmpty(text);
    }
}
