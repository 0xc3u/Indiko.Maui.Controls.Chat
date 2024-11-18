using CoreGraphics;
using Foundation;
using Indiko.Maui.Controls.Chat.Models;
using Microsoft.Maui.Platform;
using UIKit;

namespace Indiko.Maui.Controls.Chat.Platforms.iOS;

using CoreGraphics;
using Foundation;
using Indiko.Maui.Controls.Chat.Models;
using UIKit;

public class ChatMessageCell : UICollectionViewCell
{
    public static readonly NSString Key = new NSString(nameof(ChatMessageCell));

    private UILabel _textLabel;
    private UIImageView _avatarImageView;
    private UILabel _timestampLabel;
    private UIView _bubbleBackground;

    public ChatMessageCell(IntPtr handle) : base(handle)
    {
        InitializeViews();
        SetupConstraints();
    }

    private void InitializeViews()
    {
        _bubbleBackground = new UIView
        {
            Layer = { CornerRadius = 16 },
            ClipsToBounds = false,
            TranslatesAutoresizingMaskIntoConstraints = true,
        };

        _textLabel = new UILabel
        {
            Lines = 0, // Allow multi-line text
            LineBreakMode = UILineBreakMode.WordWrap, // Wrap text instead of truncating
            TranslatesAutoresizingMaskIntoConstraints = false
        };

        _avatarImageView = new UIImageView
        {
            Layer = { CornerRadius = 18 },
            ClipsToBounds = true,
            ContentMode = UIViewContentMode.ScaleAspectFill,
            TranslatesAutoresizingMaskIntoConstraints = false
        };

        _timestampLabel = new UILabel
        {
            Font = UIFont.SystemFontOfSize(10),
            TextColor = UIColor.Gray,
            TranslatesAutoresizingMaskIntoConstraints = false
        };

        ContentView.AddSubviews(_bubbleBackground, _textLabel, _avatarImageView, _timestampLabel);
    }

    private void SetupConstraints()
    {
        // Avatar constraints
        NSLayoutConstraint.ActivateConstraints(new[]
        {
            _avatarImageView.LeadingAnchor.ConstraintEqualTo(ContentView.LeadingAnchor, 10),
            _avatarImageView.TopAnchor.ConstraintEqualTo(ContentView.TopAnchor, 10),
            _avatarImageView.WidthAnchor.ConstraintEqualTo(36),
            _avatarImageView.HeightAnchor.ConstraintEqualTo(36)
        });

        // Bubble background constraints
        NSLayoutConstraint.ActivateConstraints(new[]
        {
            _bubbleBackground.TopAnchor.ConstraintEqualTo(_avatarImageView.TopAnchor),
            _bubbleBackground.LeadingAnchor.ConstraintEqualTo(_avatarImageView.TrailingAnchor, 10),
            _bubbleBackground.TrailingAnchor.ConstraintLessThanOrEqualTo(ContentView.TrailingAnchor, -10),
            _bubbleBackground.BottomAnchor.ConstraintEqualTo(_textLabel.BottomAnchor, 10)
        });

        // Text label constraints
        NSLayoutConstraint.ActivateConstraints(new[]
        {
            _textLabel.TopAnchor.ConstraintEqualTo(_bubbleBackground.TopAnchor, 10),
            _textLabel.LeadingAnchor.ConstraintEqualTo(_bubbleBackground.LeadingAnchor, 10),
            _textLabel.TrailingAnchor.ConstraintEqualTo(_bubbleBackground.TrailingAnchor, -10),
            _textLabel.BottomAnchor.ConstraintEqualTo(_timestampLabel.TopAnchor, -10)
        });

        // Timestamp constraints
        NSLayoutConstraint.ActivateConstraints(new[]
        {
            _timestampLabel.LeadingAnchor.ConstraintEqualTo(_bubbleBackground.LeadingAnchor, 10),
            _timestampLabel.BottomAnchor.ConstraintEqualTo(_bubbleBackground.BottomAnchor, -10)
        });
    }

    public void Update(ChatMessage message, ChatView chatView)
    {
        _textLabel.Text = message.TextContent;

        _textLabel.TextColor = message.IsOwnMessage
            ? chatView.OwnMessageTextColor.ToPlatform()
            : chatView.OtherMessageTextColor.ToPlatform();

        _bubbleBackground.BackgroundColor = message.IsOwnMessage
            ? chatView.OwnMessageBackgroundColor.ToPlatform()
            : chatView.OtherMessageBackgroundColor.ToPlatform();

        if (message.SenderAvatar != null)
        {
            _avatarImageView.Image = UIImage.LoadFromData(NSData.FromArray(message.SenderAvatar));
            _avatarImageView.Hidden = false;
        }
        else
        {
            _avatarImageView.Hidden = true;
        }

        
        _timestampLabel.Text = message.Timestamp.ToString("HH:mm");
        _timestampLabel.TextColor = chatView.MessageTimeTextColor.ToPlatform();
        _timestampLabel.Font = UIFont.SystemFontOfSize(chatView.MessageTimeFontSize);


        LayoutIfNeeded();
    }
}

