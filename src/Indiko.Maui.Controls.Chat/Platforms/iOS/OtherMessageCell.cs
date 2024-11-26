using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Foundation;
using Indiko.Maui.Controls.Chat.Models;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Platform;
using UIKit;

namespace Indiko.Maui.Controls.Chat.Platforms.iOS;
public class OtherMessageCell : UICollectionViewCell
{
    public static readonly NSString Key = new(nameof(OtherMessageCell));
    private UILabel _messageLabel;
    private UIImageView _avatarImageView;

    public OtherMessageCell(ObjCRuntime.NativeHandle handle) : base(handle)
    {
        SetupLayout();
    }

    private void SetupLayout()
    {
        _avatarImageView = new UIImageView
        {
            TranslatesAutoresizingMaskIntoConstraints = false,
            ContentMode = UIViewContentMode.ScaleAspectFill,
            ClipsToBounds = true
        };
        _avatarImageView.Layer.CornerRadius = 20;

        _messageLabel = new UILabel
        {
            Lines = 0,
            LineBreakMode = UILineBreakMode.WordWrap,
            TextAlignment = UITextAlignment.Left,
            BackgroundColor = UIColor.FromRGBA(0.9f, 0.9f, 0.9f, 1.0f),
            TranslatesAutoresizingMaskIntoConstraints = false
        };

        ContentView.AddSubviews(_messageLabel, _avatarImageView);

        NSLayoutConstraint.ActivateConstraints(new[]
        {
            _avatarImageView.LeadingAnchor.ConstraintEqualTo(ContentView.LeadingAnchor, 10),
            _avatarImageView.TopAnchor.ConstraintEqualTo(ContentView.TopAnchor, 10),
            _avatarImageView.WidthAnchor.ConstraintEqualTo(40),
            _avatarImageView.HeightAnchor.ConstraintEqualTo(40),

            _messageLabel.LeadingAnchor.ConstraintEqualTo(_avatarImageView.TrailingAnchor, 10),
            _messageLabel.TrailingAnchor.ConstraintLessThanOrEqualTo(ContentView.TrailingAnchor, -50),
            _messageLabel.TopAnchor.ConstraintEqualTo(ContentView.TopAnchor, 10),
            _messageLabel.BottomAnchor.ConstraintEqualTo(ContentView.BottomAnchor, -10)
        });
    }

    public void Update(int index, ChatMessage message, ChatView chatView, IMauiContext mauiContext)
    {
        if (chatView == null || message == null || mauiContext == null)
        {
            return;
        }

        try
        {

            _messageLabel.Text = message.TextContent;
            _messageLabel.TextColor = chatView.OtherMessageTextColor.ToPlatform();
            _messageLabel.BackgroundColor = chatView.OtherMessageBackgroundColor.ToPlatform();

            if (message.SenderAvatar != null)
            {
                _avatarImageView.Image = UIImage.LoadFromData(NSData.FromArray(message.SenderAvatar));
            }
            else
            {
                if (string.IsNullOrEmpty(message.SenderInitials))
                {
                    _avatarImageView.Image = UIImageExtensions.CreateInitialsImage(message.SenderId.Trim().Substring(0,2).ToUpperInvariant(), 40f, 40f,
                        chatView.AvatarTextColor.ToPlatform(), chatView.AvatarBackgroundColor.ToCGColor());
                }
                else
                {
                    _avatarImageView.Image = UIImageExtensions.CreateInitialsImage(message.SenderInitials, 40f, 40f,
                    chatView.AvatarTextColor.ToPlatform(), chatView.AvatarBackgroundColor.ToCGColor());
                }
            }


            SetNeedsLayout();
        }
        catch(Exception ex)
        {
            Console.WriteLine($"Error in {nameof(OtherMessageCell)}.{nameof(Update)}: {ex.Message}");
        }
    }
}
