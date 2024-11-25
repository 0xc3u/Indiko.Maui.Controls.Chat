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

        // Constructor required for marshaling
        public ChatMessageCell(IntPtr handle) : base(handle)
        {
            Initialize();
        }

        public ChatMessageCell(CGRect frame) : base(frame)
        {
            Initialize();
        }

        private void Initialize()
        {
            ContentView.BackgroundColor = UIColor.Clear;

            _avatarView = new UIImageView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Layer = { CornerRadius = 24, MasksToBounds = true } // Adjusted avatar size
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
            _avatarView.WidthAnchor.ConstraintEqualTo(28).Active = true; // Adjusted avatar size
            _avatarView.HeightAnchor.ConstraintEqualTo(28).Active = true; // Adjusted avatar size
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
            _messageLabel.BottomAnchor.ConstraintEqualTo(_frameView.BottomAnchor, 16).Active = true;

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

            // ReactionContainer constraints
            _reactionContainer.LeadingAnchor.ConstraintEqualTo(_frameView.LeadingAnchor, 16).Active = true;
            _reactionContainer.TopAnchor.ConstraintEqualTo(_frameView.BottomAnchor, 8).Active = true;
            _reactionContainer.TrailingAnchor.ConstraintEqualTo(_frameView.TrailingAnchor, -16).Active = true;
            _reactionContainer.HeightAnchor.ConstraintGreaterThanOrEqualTo(20).Active = true; // Adjust height if needed

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
                _avatarView.Image = CreateInitialsImage(message.SenderInitials, 28, 28); // Adjusted avatar size
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


            // Clear previous reactions
            foreach (var subview in _reactionContainer.Subviews)
            {
                subview.RemoveFromSuperview();
            }

            // Add reactions if available
            if (message.Reactions != null && message.Reactions.Any())
            {
                foreach (var reaction in message.Reactions)
                {
                    var reactionLabel = new UILabel
                    {
                        Text = $"{reaction.Emoji} {reaction.Count}",
                        Font = UIFont.SystemFontOfSize(12),
                        TextColor = UIColor.Gray,
                        TranslatesAutoresizingMaskIntoConstraints = false
                    };

                    _reactionContainer.AddSubview(reactionLabel);

                    // Set constraints for each reaction (stacked horizontally)
                    if (_reactionContainer.Subviews.Length == 1)
                    {
                        // First reaction: align to the left
                        reactionLabel.LeadingAnchor.ConstraintEqualTo(_reactionContainer.LeadingAnchor).Active = true;
                    }
                    else
                    {
                        // Subsequent reactions: align to the previous reaction
                        var previousLabel = _reactionContainer.Subviews[_reactionContainer.Subviews.Length - 2];
                        reactionLabel.LeadingAnchor.ConstraintEqualTo(previousLabel.TrailingAnchor, 8).Active = true;
                    }

                    // Align vertically to the container
                    reactionLabel.CenterYAnchor.ConstraintEqualTo(_reactionContainer.CenterYAnchor).Active = true;
                }

                // Show the reaction container
                _reactionContainer.Hidden = false;
            }
            else
            {
                // Hide the reaction container if no reactions
                _reactionContainer.Hidden = true;
            }

            // Set delivery status icon
            // if (message.DeliveryState == MessageDeliveryState.Sent && chatView.SendIcon != null)
            // {
            //     _deliveryStatusIcon.Image = UIImage.LoadFromData(NSData.FromArray(chatView.SendIcon.ToBytes()));
            // }
            // else if (message.DeliveryState == MessageDeliveryState.Delivered && chatView.DeliveredIcon != null)
            // {
            //     _deliveryStatusIcon.Image = UIImage.LoadFromData(NSData.FromArray(chatView.DeliveredIcon.ToBytes()));
            // }
            // else if (message.DeliveryState == MessageDeliveryState.Read && chatView.ReadIcon != null)
            // {
            //     _deliveryStatusIcon.Image = UIImage.LoadFromData(NSData.FromArray(chatView.ReadIcon.ToBytes()));
            // }

            // Set dynamic width for the message bubble (65% of screen width)
            var displayMetrics = UIScreen.MainScreen.Bounds;
            var maxWidth = (nfloat)(displayMetrics.Width * 0.65);
            _frameView.WidthAnchor.ConstraintEqualTo(maxWidth).Active = true;

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
