using UIKit;
using CoreGraphics;
using Indiko.Maui.Controls.Chat.Models;
using Microsoft.Maui.Graphics;
using Foundation;
using Microsoft.Maui.Platform;

namespace Indiko.Maui.Controls.Chat.Platforms.iOS
{
    public class ChatMessageCell : UICollectionViewCell
    {
        public static readonly NSString CellId = new NSString("ChatMessageCell");

        private UIImageView _avatarView;
        private UILabel _dateLabel;
        private UILabel _messageLabel;
        private UIImageView _imageView;
        private UIView _videoContainer;
        private UIImageView _videoThumbnail;
        private UILabel _timestampLabel;
        private UIView _frameView;
        private UILabel _newMessagesSeparatorLabel;
        private UIView _reactionContainer;
        private UIImageView _deliveryStatusIcon;
        private UILabel _replySenderLabel;
        private UILabel _replyPreviewLabel;

        public ChatMessageCell(CGRect frame) : base(frame)
        {
            ContentView.BackgroundColor = UIColor.Clear;

            _avatarView = new UIImageView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Layer = { CornerRadius = 48, MasksToBounds = true }
            };

            _dateLabel = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                TextAlignment = UITextAlignment.Center,
                Font = UIFont.BoldSystemFontOfSize(12),
                TextColor = UIColor.Gray
            };

            _messageLabel = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap
            };

            _imageView = new UIImageView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                ContentMode = UIViewContentMode.ScaleAspectFill,
                ClipsToBounds = true
            };

            _videoContainer = new UIView
            {
                TranslatesAutoresizingMaskIntoConstraints = false
            };

            _videoThumbnail = new UIImageView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                ContentMode = UIViewContentMode.ScaleAspectFill,
                ClipsToBounds = true
            };

            _timestampLabel = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Font = UIFont.SystemFontOfSize(10),
                TextColor = UIColor.Gray
            };

            _frameView = new UIView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Layer = { CornerRadius = 12, MasksToBounds = true }
            };

            _newMessagesSeparatorLabel = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                TextAlignment = UITextAlignment.Center,
                Font = UIFont.BoldSystemFontOfSize(12),
                TextColor = UIColor.Gray
            };

            _reactionContainer = new UIView
            {
                TranslatesAutoresizingMaskIntoConstraints = false
            };

            _deliveryStatusIcon = new UIImageView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                ContentMode = UIViewContentMode.ScaleAspectFit
            };

            _replySenderLabel = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Font = UIFont.BoldSystemFontOfSize(12),
                TextColor = UIColor.Gray
            };

            _replyPreviewLabel = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Font = UIFont.SystemFontOfSize(12),
                TextColor = UIColor.Gray
            };

            // Add subviews
            ContentView.AddSubviews(_avatarView, _dateLabel, _frameView, _newMessagesSeparatorLabel, _timestampLabel, _deliveryStatusIcon);
            _frameView.AddSubviews(_messageLabel, _imageView, _videoContainer, _reactionContainer, _replySenderLabel, _replyPreviewLabel);
            _videoContainer.AddSubview(_videoThumbnail);

            // Set up constraints
            SetupConstraints();
        }

        private void SetupConstraints()
        {
            // AvatarView constraints
            _avatarView.WidthAnchor.ConstraintEqualTo(96).Active = true;
            _avatarView.HeightAnchor.ConstraintEqualTo(96).Active = true;
            _avatarView.LeadingAnchor.ConstraintEqualTo(ContentView.LeadingAnchor, 16).Active = true;
            _avatarView.TopAnchor.ConstraintEqualTo(ContentView.TopAnchor, 16).Active = true;

            // DateLabel constraints
            _dateLabel.CenterXAnchor.ConstraintEqualTo(ContentView.CenterXAnchor).Active = true;
            _dateLabel.TopAnchor.ConstraintEqualTo(ContentView.TopAnchor, 8).Active = true;

            // NewMessagesSeparatorLabel constraints
            _newMessagesSeparatorLabel.CenterXAnchor.ConstraintEqualTo(ContentView.CenterXAnchor).Active = true;
            _newMessagesSeparatorLabel.TopAnchor.ConstraintEqualTo(_dateLabel.BottomAnchor, 8).Active = true;

            // FrameView constraints
            _frameView.LeadingAnchor.ConstraintEqualTo(_avatarView.TrailingAnchor, 16).Active = true;
            _frameView.TopAnchor.ConstraintEqualTo(ContentView.TopAnchor, 16).Active = true;
            _frameView.TrailingAnchor.ConstraintEqualTo(ContentView.TrailingAnchor, 16).Active = true;

            // MessageLabel constraints
            _messageLabel.LeadingAnchor.ConstraintEqualTo(_frameView.LeadingAnchor, 16).Active = true;
            _messageLabel.TopAnchor.ConstraintEqualTo(_frameView.TopAnchor, 16).Active = true;
            _messageLabel.TrailingAnchor.ConstraintEqualTo(_frameView.TrailingAnchor, 16).Active = true;

            // ImageView constraints
            _imageView.LeadingAnchor.ConstraintEqualTo(_frameView.LeadingAnchor, 16).Active = true;
            _imageView.TopAnchor.ConstraintEqualTo(_frameView.TopAnchor, 16).Active = true;
            _imageView.TrailingAnchor.ConstraintEqualTo(_frameView.TrailingAnchor, 16).Active = true;
            _imageView.HeightAnchor.ConstraintEqualTo(_imageView.WidthAnchor, 0.5625f).Active = true; // 16:9 aspect ratio

            // VideoContainer constraints
            _videoContainer.LeadingAnchor.ConstraintEqualTo(_frameView.LeadingAnchor, 16).Active = true;
            _videoContainer.TopAnchor.ConstraintEqualTo(_frameView.TopAnchor, 16).Active = true;
            _videoContainer.TrailingAnchor.ConstraintEqualTo(_frameView.TrailingAnchor, 16).Active = true;
            _videoContainer.HeightAnchor.ConstraintEqualTo(_videoContainer.WidthAnchor, 0.5625f).Active = true; // 16:9 aspect ratio

            // VideoThumbnail constraints
            _videoThumbnail.LeadingAnchor.ConstraintEqualTo(_videoContainer.LeadingAnchor).Active = true;
            _videoThumbnail.TopAnchor.ConstraintEqualTo(_videoContainer.TopAnchor).Active = true;
            _videoThumbnail.TrailingAnchor.ConstraintEqualTo(_videoContainer.TrailingAnchor).Active = true;
            _videoThumbnail.BottomAnchor.ConstraintEqualTo(_videoContainer.BottomAnchor).Active = true;

            // TimestampLabel constraints
            _timestampLabel.LeadingAnchor.ConstraintEqualTo(_frameView.LeadingAnchor, 16).Active = true;
            _timestampLabel.TopAnchor.ConstraintEqualTo(_frameView.BottomAnchor, 8).Active = true;

            // DeliveryStatusIcon constraints
            _deliveryStatusIcon.LeadingAnchor.ConstraintEqualTo(_timestampLabel.TrailingAnchor, 8).Active = true;
            _deliveryStatusIcon.CenterYAnchor.ConstraintEqualTo(_timestampLabel.CenterYAnchor).Active = true;
            _deliveryStatusIcon.WidthAnchor.ConstraintEqualTo(16).Active = true;
            _deliveryStatusIcon.HeightAnchor.ConstraintEqualTo(16).Active = true;

            // ReplySenderLabel constraints
            _replySenderLabel.LeadingAnchor.ConstraintEqualTo(_frameView.LeadingAnchor, 16).Active = true;
            _replySenderLabel.TopAnchor.ConstraintEqualTo(_frameView.TopAnchor, 16).Active = true;

            // ReplyPreviewLabel constraints
            _replyPreviewLabel.LeadingAnchor.ConstraintEqualTo(_frameView.LeadingAnchor, 16).Active = true;
            _replyPreviewLabel.TopAnchor.ConstraintEqualTo(_replySenderLabel.BottomAnchor, 8).Active = true;
        }

        public void Bind(ChatMessage message, ChatView chatView, int index)
        {
            // Bind data to views
            _dateLabel.Text = message.Timestamp.ToString("dddd MMM dd");
            _messageLabel.Text = message.TextContent;
            _timestampLabel.Text = message.Timestamp.ToString("HH:mm");

            // Set avatar
            if (!message.IsOwnMessage && message.SenderAvatar != null)
            {
                _avatarView.Image = UIImageExtensions.CreateCircularImage(UIImage.LoadFromData(NSData.FromArray(message.SenderAvatar)));
            }
            else if (!message.IsOwnMessage)
            {
                _avatarView.Image = CreateInitialsImage(message.SenderInitials, 96, 96);
            }
            else
            {
                _avatarView.Image = null;
            }

            // Set message type handling
            if (message.MessageType == MessageType.Text)
            {
                _imageView.Hidden = true;
                _videoContainer.Hidden = true;
                _messageLabel.Hidden = false;
                _messageLabel.TextColor = message.IsOwnMessage ? chatView.OwnMessageTextColor.ToPlatform() : chatView.OtherMessageTextColor.ToPlatform();
            }
            else if (message.MessageType == MessageType.Image)
            {
                if (message.BinaryContent != null)
                {
                    _messageLabel.Hidden = true;
                    _imageView.Hidden = false;
                    _videoContainer.Hidden = true;
                    _imageView.Image = UIImage.LoadFromData(NSData.FromArray(message.BinaryContent));
                }
                else
                {
                    _messageLabel.Hidden = true;
                    _imageView.Hidden = true;
                    _videoContainer.Hidden = true;
                }
            }
            else if (message.MessageType == MessageType.Video)
            {
                if (message.BinaryContent != null)
                {
                    _messageLabel.Hidden = true;
                    _imageView.Hidden = true;
                    _videoContainer.Hidden = false;
                    _videoThumbnail.Image = UIImage.LoadFromData(NSData.FromArray(message.BinaryContent));
                }
                else
                {
                    _messageLabel.Hidden = true;
                    _imageView.Hidden = true;
                    _videoContainer.Hidden = true;
                }
            }

            // Set delivery status icon
            //if (message.DeliveryState == MessageDeliveryState.Sent && chatView.SendIcon != null)
            //{
            //    _deliveryStatusIcon.Image = UIImage.LoadFromData(NSData.FromArray(chatView.SendIcon.ToBytes()));
            //}
            //else if (message.DeliveryState == MessageDeliveryState.Delivered && chatView.DeliveredIcon != null)
            //{
            //    _deliveryStatusIcon.Image = UIImage.LoadFromData(NSData.FromArray(chatView.DeliveredIcon.ToBytes()));
            //}
            //else if (message.DeliveryState == MessageDeliveryState.Read && chatView.ReadIcon != null)
            //{
            //    _deliveryStatusIcon.Image = UIImage.LoadFromData(NSData.FromArray(chatView.ReadIcon.ToBytes()));
            //}

            // Set dynamic width for the message bubble (65% of screen width)
            var displayMetrics = UIScreen.MainScreen.Bounds;
            var maxWidth = (nfloat)(displayMetrics.Width * 0.65);
            _frameView.Frame = new CGRect(_frameView.Frame.X, _frameView.Frame.Y, maxWidth, _frameView.Frame.Height);

            // Set background color for the message bubble
            _frameView.BackgroundColor = message.IsOwnMessage ? chatView.OwnMessageBackgroundColor.ToPlatform() : chatView.OtherMessageBackgroundColor.ToPlatform();

            // Set reply message
            if (message.IsRepliedMessage && message.ReplyToMessage != null)
            {
                _replySenderLabel.Text = message.ReplyToMessage.SenderId;
                _replyPreviewLabel.Text = RepliedMessage.GenerateTextPreview(message.ReplyToMessage.TextPreview);
                _replySenderLabel.Hidden = false;
                _replyPreviewLabel.Hidden = false;
            }
            else
            {
                _replySenderLabel.Hidden = true;
                _replyPreviewLabel.Hidden = true;
            }

            // Set date and time
            bool isFirstMessageOfDate = index == 0 || chatView.Messages[index - 1].Timestamp.Date != message.Timestamp.Date;
            _dateLabel.Hidden = !isFirstMessageOfDate;
        }

        private UIImage CreateInitialsImage(string initials, nfloat width, nfloat height)
        {
            UIGraphics.BeginImageContext(new CGSize(width, height));
            var context = UIGraphics.GetCurrentContext();
            var path = new CGPath();
            path.AddArc(width / 2, height / 2, width / 2, 0, (float)(2 * Math.PI), true);
            context.AddPath(path);
            context.Clip();

            var backgroundColor = UIColor.FromRGBA(255, 255, 255, 1); // Placeholder color
            context.SetFillColor(backgroundColor.CGColor);
            context.FillPath();

            var textColor = UIColor.Black; // Placeholder color
            var textFont = UIFont.BoldSystemFontOfSize(width / 3);
            var textSize = new NSString(initials).GetSizeUsingAttributes(new UIStringAttributes { Font = textFont });
            var textPoint = new CGPoint((width - textSize.Width) / 2, (height - textSize.Height) / 2);

            initials.DrawString(textPoint, textFont);

            var image = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();
            return image;
        }
    }
}




//using AVFoundation;
//using AVKit;
//using Foundation;
//using Indiko.Maui.Controls.Chat.Models;
//using Microsoft.Maui.Platform;
//using UIKit;

//namespace Indiko.Maui.Controls.Chat.Platforms.iOS;

//public class ChatMessageCell : UICollectionViewCell
//{
//    public static readonly NSString Key = new NSString(nameof(ChatMessageCell));

//    private UILabel _textLabel;
//    private UIImageView _avatarImageView;
//    private UILabel _timestampLabel;
//    private UIView _bubbleBackground;
//    private UIImageView _imageView;
//    private UIView _videoContainer;
//    private AVPlayerViewController _videoPlayer;


//    public ChatMessageCell(IntPtr handle) : base(handle)
//    {
//        InitializeViews();
//        SetupConstraints();
//    }

//    private void InitializeViews()
//    {
//        _bubbleBackground = new UIView
//        {
//            Layer = { CornerRadius = 16 },
//            ClipsToBounds = false,
//            TranslatesAutoresizingMaskIntoConstraints = true,
//        };

//        _textLabel = new UILabel
//        {
//            Lines = 0, // Allow multi-line text
//            LineBreakMode = UILineBreakMode.WordWrap, // Wrap text instead of truncating
//            TranslatesAutoresizingMaskIntoConstraints = false
//        };

//        _avatarImageView = new UIImageView
//        {
//            Layer = { CornerRadius = 18 },
//            ClipsToBounds = true,
//            ContentMode = UIViewContentMode.ScaleAspectFill,
//            TranslatesAutoresizingMaskIntoConstraints = false
//        };

//        _timestampLabel = new UILabel
//        {
//            Font = UIFont.SystemFontOfSize(10),
//            TextColor = UIColor.Gray,
//            TranslatesAutoresizingMaskIntoConstraints = false
//        };

//        // Image view for image messages
//        _imageView = new UIImageView
//        {
//            ContentMode = UIViewContentMode.ScaleAspectFit,
//            TranslatesAutoresizingMaskIntoConstraints = false,
//            Hidden = true // Initially hidden
//        };

//        // Video container for video messages
//        _videoContainer = new UIView
//        {
//            Layer = { CornerRadius = 16 },
//            ClipsToBounds = true,
//            TranslatesAutoresizingMaskIntoConstraints = false,
//            Hidden = true // Initially hidden
//        };

//        // Video player
//        _videoPlayer = new AVPlayerViewController
//        {
//            View =
//            {
//                TranslatesAutoresizingMaskIntoConstraints = false
//            }
//        };
//        _videoContainer.AddSubview(_videoPlayer.View);

//        ContentView.AddSubviews(_bubbleBackground, _textLabel, _avatarImageView, _imageView, _videoContainer, _timestampLabel);
//    }

//    private void SetupConstraints()
//    {
//        // Avatar constraints
//        NSLayoutConstraint.ActivateConstraints(new[]
//        {
//            _avatarImageView.LeadingAnchor.ConstraintEqualTo(ContentView.LeadingAnchor, 10),
//            _avatarImageView.TopAnchor.ConstraintEqualTo(ContentView.TopAnchor, 10),
//            _avatarImageView.WidthAnchor.ConstraintEqualTo(36),
//            _avatarImageView.HeightAnchor.ConstraintEqualTo(36)
//        });

//        // Bubble background constraints
//        NSLayoutConstraint.ActivateConstraints(new[]
//        {
//            _bubbleBackground.TopAnchor.ConstraintEqualTo(_avatarImageView.TopAnchor),
//            _bubbleBackground.LeadingAnchor.ConstraintEqualTo(_avatarImageView.TrailingAnchor, 10),
//            _bubbleBackground.TrailingAnchor.ConstraintLessThanOrEqualTo(ContentView.TrailingAnchor, -10),
//            _bubbleBackground.BottomAnchor.ConstraintEqualTo(_textLabel.BottomAnchor, 10)
//        });

//        // Text label constraints
//        NSLayoutConstraint.ActivateConstraints(new[]
//        {
//            _textLabel.TopAnchor.ConstraintEqualTo(_bubbleBackground.TopAnchor, 10),
//            _textLabel.LeadingAnchor.ConstraintEqualTo(_bubbleBackground.LeadingAnchor, 10),
//            _textLabel.TrailingAnchor.ConstraintEqualTo(_bubbleBackground.TrailingAnchor, -10),
//            _textLabel.BottomAnchor.ConstraintEqualTo(_timestampLabel.TopAnchor, -10)
//        });

//        // Timestamp constraints
//        NSLayoutConstraint.ActivateConstraints(new[]
//        {
//            _timestampLabel.LeadingAnchor.ConstraintEqualTo(_bubbleBackground.LeadingAnchor, 10),
//            _timestampLabel.BottomAnchor.ConstraintEqualTo(_bubbleBackground.BottomAnchor, -10)
//        });
//    }

//    public void Update(ChatMessage message, ChatView chatView)
//    {
//        _textLabel.Hidden = true;
//        _imageView.Hidden = true;
//        _videoContainer.Hidden = true;


//        if (message.MessageType == MessageType.Text)
//        {
//            _textLabel.Text = message.TextContent;
//            _textLabel.TextColor = message.IsOwnMessage
//                ? chatView.OwnMessageTextColor.ToPlatform()
//                : chatView.OtherMessageTextColor.ToPlatform();
//            _textLabel.Hidden = false;
//        }
//        else if (message.MessageType == MessageType.Image && message.BinaryContent != null)
//        {
//            _imageView.Image = UIImage.LoadFromData(NSData.FromArray(message.BinaryContent));
//            _imageView.Hidden = false;
//        }
//        else if (message.MessageType == MessageType.Video && message.BinaryContent != null)
//        {
//            var tempFile = Path.Combine(Path.GetTempPath(), $"{message.MessageId}.mp4");
//            File.WriteAllBytes(tempFile, message.BinaryContent);

//            var player = new AVPlayer(NSUrl.FromFilename(tempFile));
//            _videoPlayer.Player = player;
//            _videoContainer.Hidden = false;
//        }

//        _bubbleBackground.BackgroundColor = message.IsOwnMessage
//             ? chatView.OwnMessageBackgroundColor.ToPlatform()
//             : chatView.OtherMessageBackgroundColor.ToPlatform();


//        if (message.SenderAvatar != null)
//        {
//            _avatarImageView.Image = UIImage.LoadFromData(NSData.FromArray(message.SenderAvatar));
//            _avatarImageView.Hidden = false;
//        }
//        else
//        {
//            _avatarImageView.Hidden = true;
//        }

//        _timestampLabel.Text = message.Timestamp.ToString("HH:mm");
//        _timestampLabel.TextColor = chatView.MessageTimeTextColor.ToPlatform();
//        _timestampLabel.Font = UIFont.SystemFontOfSize(chatView.MessageTimeFontSize);

//        LayoutIfNeeded();
//    }
//}

