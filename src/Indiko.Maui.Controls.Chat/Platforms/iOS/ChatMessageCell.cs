using AVFoundation;
using AVKit;
using Foundation;
using Indiko.Maui.Controls.Chat.Models;
using Microsoft.Maui.Platform;
using UIKit;

namespace Indiko.Maui.Controls.Chat.Platforms.iOS;

public class ChatMessageCell : UICollectionViewCell
{
    public static readonly NSString Key = new NSString(nameof(ChatMessageCell));

    private UILabel _textLabel;
    private UIImageView _avatarImageView;
    private UILabel _timestampLabel;
    private UIView _bubbleBackground;
    private UIImageView _imageView;
    private UIView _videoContainer;
    private AVPlayerViewController _videoPlayer;


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

        // Image view for image messages
        _imageView = new UIImageView
        {
            ContentMode = UIViewContentMode.ScaleAspectFit,
            TranslatesAutoresizingMaskIntoConstraints = false,
            Hidden = true // Initially hidden
        };

        // Video container for video messages
        _videoContainer = new UIView
        {
            Layer = { CornerRadius = 16 },
            ClipsToBounds = true,
            TranslatesAutoresizingMaskIntoConstraints = false,
            Hidden = true // Initially hidden
        };

        // Video player
        _videoPlayer = new AVPlayerViewController
        {
            View =
            {
                TranslatesAutoresizingMaskIntoConstraints = false
            }
        };
        _videoContainer.AddSubview(_videoPlayer.View);

        ContentView.AddSubviews(_bubbleBackground, _textLabel, _avatarImageView, _imageView, _videoContainer, _timestampLabel);
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
        _textLabel.Hidden = true;
        _imageView.Hidden = true;
        _videoContainer.Hidden = true;


        if (message.MessageType == MessageType.Text)
        {
            _textLabel.Text = message.TextContent;
            _textLabel.TextColor = message.IsOwnMessage
                ? chatView.OwnMessageTextColor.ToPlatform()
                : chatView.OtherMessageTextColor.ToPlatform();
            _textLabel.Hidden = false;
        }
        else if (message.MessageType == MessageType.Image && message.BinaryContent != null)
        {
            _imageView.Image = UIImage.LoadFromData(NSData.FromArray(message.BinaryContent));
            _imageView.Hidden = false;
        }
        else if (message.MessageType == MessageType.Video && message.BinaryContent != null)
        {
            var tempFile = Path.Combine(Path.GetTempPath(), $"{message.MessageId}.mp4");
            File.WriteAllBytes(tempFile, message.BinaryContent);

            var player = new AVPlayer(NSUrl.FromFilename(tempFile));
            _videoPlayer.Player = player;
            _videoContainer.Hidden = false;
        }

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

