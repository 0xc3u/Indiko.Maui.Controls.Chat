using CoreGraphics;
using Foundation;
using Indiko.Maui.Controls.Chat.Models;
using Microsoft.Maui.Platform;
using UIKit;

namespace Indiko.Maui.Controls.Chat.Platforms.iOS;

internal sealed class OtherImageMessageCell : UICollectionViewCell
{
    public static readonly NSString Key = new(nameof(OtherImageMessageCell));
    private ChatView _chatView;
    private ChatMessage _message;

    private UIImageView _imageView;

    private UIImageView _avatarImageView;
    private UIView _bubbleView;
    private UILabel _timeLabel;
    private UIStackView _reactionsStackView;
    private UIImageView _deliveryStateImageView;

    private UIView _replyView;
    private UILabel _replyPreviewTextLabel;
    private UILabel _replySenderTextLabel;

    private NSLayoutConstraint _messageImageTopConstraint;
    private NSLayoutConstraint _imageAspectConstraint;
    private UILongPressGestureRecognizer _longPressGesture;

    private UILabel _captionLabel;
    private NSLayoutConstraint _imageBottomToBubble;
    private NSLayoutConstraint _imageBottomToCaption;
    private NSLayoutConstraint _captionBottomToBubble;

    private UILabel _senderNameLabel;
    private NSLayoutConstraint _replyTopConstraint;

    // Cap the displayed image height so a tall photo can't blow up the bubble.
    private const float MaxImageHeight = 280f;

    public OtherImageMessageCell(ObjCRuntime.NativeHandle handle) : base(handle)
    {
        SetupLayout();
    }

    public override UICollectionViewLayoutAttributes PreferredLayoutAttributesFittingAttributes(UICollectionViewLayoutAttributes layoutAttributes)
    {
        return CellSizingHelper.CalculateFittingAttributes(layoutAttributes, ContentView, _message?.MessageId);
    }

    private void SetupLayout()
    {
        // Avatar setup
        _avatarImageView = new UIImageView
        {
            TranslatesAutoresizingMaskIntoConstraints = false,
            ContentMode = UIViewContentMode.ScaleAspectFill,
            ClipsToBounds = true
        };
        _avatarImageView.Layer.CornerRadius = 20;

        // Chat bubble setup
        _bubbleView = new UIView
        {
            TranslatesAutoresizingMaskIntoConstraints = false,
            BackgroundColor = UIColor.FromRGBA(230 / 255.0f, 223 / 255.0f, 255 / 255.0f, 1.0f),
            ClipsToBounds = true
        };
        _bubbleView.Layer.CornerRadius = 16;

        // Message Image
        _imageView = new UIImageView
                        {
                            TranslatesAutoresizingMaskIntoConstraints = false,
                            ContentMode = UIViewContentMode.ScaleAspectFit,
                            ClipsToBounds = true
                        };

        // Chat reply view setup
        _replyView = new UIView
        {
            TranslatesAutoresizingMaskIntoConstraints = false,
            BackgroundColor = UIColor.FromRGBA(230 / 255.0f, 223 / 255.0f, 255 / 255.0f, 1.0f),
            ClipsToBounds = true
        };
        _replyView.Layer.CornerRadius = 4;
        _replyPreviewTextLabel = new UILabel
        {
            Lines = 0, // Allows unlimited lines
            LineBreakMode = UILineBreakMode.WordWrap,
            TranslatesAutoresizingMaskIntoConstraints = false,
            TextAlignment = UITextAlignment.Left,
            TextColor = UIColor.Black
        };

        _replySenderTextLabel = new UILabel
        {
            Lines = 1,
            LineBreakMode = UILineBreakMode.TailTruncation,
            TranslatesAutoresizingMaskIntoConstraints = false,
            TextAlignment = UITextAlignment.Left,
            TextColor = UIColor.Black
        };

        // Message timestamp
        _timeLabel = new UILabel
        {
            Font = UIFont.SystemFontOfSize(12),
            TextColor = UIColor.LightGray,
            TranslatesAutoresizingMaskIntoConstraints = false,
            TextAlignment = UITextAlignment.Left
        };

        // The image's intrinsic (raw pixel) size must not drive the bubble height —
        // a per-image aspect-ratio constraint and the max-height cap do that instead.
        _imageView.SetContentCompressionResistancePriority((float)UILayoutPriority.DefaultLow, UILayoutConstraintAxis.Vertical);
        _imageView.SetContentHuggingPriority((float)UILayoutPriority.DefaultLow, UILayoutConstraintAxis.Vertical);
        _timeLabel.SetContentHuggingPriority((float)UILayoutPriority.DefaultLow, UILayoutConstraintAxis.Horizontal);

        // Message reaction stack (horizontal Emoji-List)
        _reactionsStackView = new UIStackView
        {
            Axis = UILayoutConstraintAxis.Horizontal,
            Distribution = UIStackViewDistribution.Fill,
            Alignment = UIStackViewAlignment.Center,
            Spacing = 8,
            TranslatesAutoresizingMaskIntoConstraints = false,
        };

        // delivery state setup
        _deliveryStateImageView = new UIImageView
        {
            TranslatesAutoresizingMaskIntoConstraints = false,
            ContentMode = UIViewContentMode.ScaleAspectFit,
            ClipsToBounds = true
        };

        // add child views into hierarchical order
        // Optional caption shown under the image (from the message's TextContent).
        _captionLabel = new UILabel
        {
            Lines = 0,
            LineBreakMode = UILineBreakMode.WordWrap,
            TranslatesAutoresizingMaskIntoConstraints = false,
            TextAlignment = UITextAlignment.Left
        };

        // Sender name shown at the top of the bubble for group chats (first of a sender run).
        _senderNameLabel = new UILabel
        {
            Lines = 1,
            LineBreakMode = UILineBreakMode.TailTruncation,
            TranslatesAutoresizingMaskIntoConstraints = false,
            TextAlignment = UITextAlignment.Left
        };

        ContentView.AddSubviews(_avatarImageView, _bubbleView, _senderNameLabel, _imageView, _captionLabel, _replyView, _replySenderTextLabel, _replyPreviewTextLabel, _timeLabel, _deliveryStateImageView, _reactionsStackView);

        // Layout-Constraints
        NSLayoutConstraint.ActivateConstraints(new[]
        {
            // Avatar
            _avatarImageView.LeadingAnchor.ConstraintEqualTo(ContentView.LeadingAnchor, 10),
            _avatarImageView.TopAnchor.ConstraintEqualTo(ContentView.TopAnchor, 10),
            _avatarImageView.WidthAnchor.ConstraintEqualTo(40),
            _avatarImageView.HeightAnchor.ConstraintEqualTo(40),

            // Chat bubble
            _bubbleView.LeadingAnchor.ConstraintEqualTo(_avatarImageView.TrailingAnchor, 10),
            _bubbleView.TrailingAnchor.ConstraintLessThanOrEqualTo(ContentView.TrailingAnchor, -50),
            _bubbleView.TopAnchor.ConstraintEqualTo(ContentView.TopAnchor, 10),
            _bubbleView.BottomAnchor.ConstraintEqualTo(_reactionsStackView.TopAnchor, -4),

            // Message reply view inside chat bubble
            // Sender name at the top of the bubble (content tops retarget to it when shown)
            _senderNameLabel.TopAnchor.ConstraintEqualTo(_bubbleView.TopAnchor, 8),
            _senderNameLabel.LeadingAnchor.ConstraintEqualTo(_bubbleView.LeadingAnchor, 10),
            _senderNameLabel.TrailingAnchor.ConstraintEqualTo(_bubbleView.TrailingAnchor, -10),

            _replyView.LeadingAnchor.ConstraintEqualTo(_bubbleView.LeadingAnchor, 10),
            _replyView.TrailingAnchor.ConstraintEqualTo(_bubbleView.TrailingAnchor, -10),

            // Reply sender text inside reply view
            _replySenderTextLabel.TopAnchor.ConstraintEqualTo(_replyView.TopAnchor, 10),
            _replySenderTextLabel.LeadingAnchor.ConstraintEqualTo(_replyView.LeadingAnchor, 10),
            _replySenderTextLabel.TrailingAnchor.ConstraintEqualTo(_replyView.TrailingAnchor, -10),

            // Reply preview text inside reply view
            _replyPreviewTextLabel.TopAnchor.ConstraintEqualTo(_replySenderTextLabel.BottomAnchor, 4),
            _replyPreviewTextLabel.LeadingAnchor.ConstraintEqualTo(_replyView.LeadingAnchor, 10),
            _replyPreviewTextLabel.TrailingAnchor.ConstraintEqualTo(_replyView.TrailingAnchor, -10),
            _replyPreviewTextLabel.BottomAnchor.ConstraintEqualTo(_replyView.BottomAnchor, -10),

            // Message Image inside chat bubble (bottom managed dynamically for the caption)
            _messageImageTopConstraint = _imageView.TopAnchor.ConstraintEqualTo(_replyView.BottomAnchor, 10),

            _imageView.LeadingAnchor.ConstraintEqualTo(_bubbleView.LeadingAnchor, 10),
            _imageView.TrailingAnchor.ConstraintEqualTo(_bubbleView.TrailingAnchor, -10),
            _imageView.HeightAnchor.ConstraintLessThanOrEqualTo(MaxImageHeight),

            // Caption under the image
            _captionLabel.LeadingAnchor.ConstraintEqualTo(_bubbleView.LeadingAnchor, 10),
            _captionLabel.TrailingAnchor.ConstraintEqualTo(_bubbleView.TrailingAnchor, -10),

            // Message Emoji-reactions
            _reactionsStackView.TopAnchor.ConstraintEqualTo(_bubbleView.BottomAnchor, 4),
            _reactionsStackView.TrailingAnchor.ConstraintEqualTo(_bubbleView.TrailingAnchor),
            _reactionsStackView.LeadingAnchor.ConstraintGreaterThanOrEqualTo(_bubbleView.LeadingAnchor),
            _reactionsStackView.WidthAnchor.ConstraintLessThanOrEqualTo(_bubbleView.WidthAnchor, 0.5f), // limit width to 50% of the chat bubble width

            // Message time stamp
            _timeLabel.TopAnchor.ConstraintEqualTo(_reactionsStackView.TopAnchor, 4),
            _timeLabel.LeadingAnchor.ConstraintEqualTo(_bubbleView.LeadingAnchor, 10),
            _timeLabel.BottomAnchor.ConstraintEqualTo(ContentView.BottomAnchor, -10),

            // Delivery state icon
            _deliveryStateImageView.CenterYAnchor.ConstraintEqualTo(_timeLabel.CenterYAnchor),
            _deliveryStateImageView.LeadingAnchor.ConstraintEqualTo(_timeLabel.TrailingAnchor, 8),
            _deliveryStateImageView.WidthAnchor.ConstraintEqualTo(16),
            _deliveryStateImageView.HeightAnchor.ConstraintEqualTo(16)
        });
        
        // Image bottom is pinned either to the bubble (no caption) or to the caption (with caption).
        _imageBottomToBubble = _imageView.BottomAnchor.ConstraintEqualTo(_bubbleView.BottomAnchor, -10);
        _imageBottomToCaption = _imageView.BottomAnchor.ConstraintEqualTo(_captionLabel.TopAnchor, -8);
        _captionBottomToBubble = _captionLabel.BottomAnchor.ConstraintEqualTo(_bubbleView.BottomAnchor, -10);
        _imageBottomToBubble.Active = true;

        // Default reply top (no sender name) — retargeted in LayoutContentTop.
        _replyTopConstraint = _replyView.TopAnchor.ConstraintEqualTo(_bubbleView.TopAnchor, 10);
        _replyTopConstraint.Active = true;

        // Initialize long press gesture
        _longPressGesture = new UILongPressGestureRecognizer(LongPressHandler);
        _bubbleView.AddGestureRecognizer(_longPressGesture);

        // Tap the reply preview to jump to the original message.
        _replyView.UserInteractionEnabled = true;
        _replyView.AddGestureRecognizer(new UITapGestureRecognizer(() =>
        {
            if (_message?.ReplyToMessage == null)
                return;
            _replyView.AnimateFade();
            _chatView?.NotifyRepliedMessageTapped(_message.ReplyToMessage.MessageId);
        }));

        // Tap the image to open the full-screen zoomable viewer.
        _imageView.UserInteractionEnabled = true;
        _imageView.AddGestureRecognizer(new UITapGestureRecognizer(OnImageTapped));

        ContentView.Transform = CoreGraphics.CGAffineTransform.MakeScale(1, -1);
    }

    private void OnImageTapped()
    {
        if (_chatView?.OpenImageFullScreen == true && _imageView.Image != null)
        {
            ImageViewerController.Present(_imageView.Image);
        }
    }

    // Pins the topmost bubble content (reply or image) to the sender name when shown,
    // otherwise to the bubble top.
    private void LayoutContentTop(bool showName, bool hasReply)
    {
        if (_replyTopConstraint != null) _replyTopConstraint.Active = false;
        _messageImageTopConstraint.Active = false;

        var topAnchor = showName ? _senderNameLabel.BottomAnchor : _bubbleView.TopAnchor;
        var topPad = showName ? (nfloat)2 : (nfloat)10;

        if (hasReply)
        {
            _replyTopConstraint = _replyView.TopAnchor.ConstraintEqualTo(topAnchor, topPad);
            _replyTopConstraint.Active = true;
            _messageImageTopConstraint = _imageView.TopAnchor.ConstraintEqualTo(_replyView.BottomAnchor, 10);
            _messageImageTopConstraint.Active = true;
        }
        else
        {
            _messageImageTopConstraint = _imageView.TopAnchor.ConstraintEqualTo(topAnchor, topPad);
            _messageImageTopConstraint.Active = true;
        }
    }

    public void Update(int index, ChatMessage message, ChatView chatView, IMauiContext mauiContext)
    {
        if (chatView == null || message == null || mauiContext == null)
        {
            return;
        }

        try
        {
            _chatView = chatView;
            _message = message;

            _avatarImageView.AddWeakTapGestureRecognizerWithCommand(_message, _chatView.AvatarTappedCommand);
            _avatarImageView.UserInteractionEnabled = true;

            _bubbleView.AddWeakTapGestureRecognizerWithCommand(_message, _chatView.MessageTappedCommand);
            _bubbleView.UserInteractionEnabled = true;

            _reactionsStackView.AddWeakTapGestureRecognizerWithCommand(_message, _chatView.EmojiReactionTappedCommand);
            _reactionsStackView.UserInteractionEnabled = true;

            _bubbleView.BackgroundColor = chatView.OtherMessageBackgroundColor.ToPlatform();

            if (message.BinaryContent != null)
            {
                var tempFile = Path.Combine(FileSystem.Current.CacheDirectory, $"{message.MessageId}.png");

                if (!File.Exists(tempFile))
                {
                    File.WriteAllBytes(tempFile, message.BinaryContent);
                }

                var image = UIImage.FromFile(tempFile);
                _imageView.Image = image;
                _imageView.Hidden = false;
                ApplyImageAspect(image);
            }
            else
            {
                _imageView.Image = null;
                _imageView.Hidden = true;
                ApplyImageAspect(null);
            }

            // Optional caption under the image (from TextContent).
            if (!string.IsNullOrEmpty(message.TextContent))
            {
                _captionLabel.Hidden = false;
                _captionLabel.Text = message.TextContent;
                _captionLabel.TextColor = chatView.OtherMessageTextColor.ToPlatform();
                _captionLabel.Font = UIFont.SystemFontOfSize((nfloat)chatView.MessageFontSize);

                _imageBottomToBubble.Active = false;
                _imageBottomToCaption.Active = true;
                _captionBottomToBubble.Active = true;
            }
            else
            {
                _captionLabel.Hidden = true;
                _captionLabel.Text = null;
                _imageBottomToCaption.Active = false;
                _captionBottomToBubble.Active = false;
                _imageBottomToBubble.Active = true;
            }

            var chronoIndex = chatView.Messages.Count - 1 - index;
            var showName = chatView.ShowSenderName
                && !string.IsNullOrEmpty(message.SenderName)
                && MessageGrouping.IsFirstOfSenderRun(chatView.Messages, chronoIndex);

            _senderNameLabel.Hidden = !showName;
            if (showName)
            {
                _senderNameLabel.Text = message.SenderName;
                _senderNameLabel.TextColor = chatView.SenderNameTextColor.ToPlatform();
                _senderNameLabel.Font = UIFont.BoldSystemFontOfSize((nfloat)chatView.SenderNameFontSize);
            }
            else
            {
                _senderNameLabel.Text = null;
            }

            var hasReply = message.IsRepliedMessage && message.ReplyToMessage != null;
            if (hasReply)
            {
                _replyView.BackgroundColor = chatView.ReplyMessageBackgroundColor.ToPlatform();

                _replyPreviewTextLabel.Text = RepliedMessage.GenerateTextPreview(message.ReplyToMessage.TextPreview);
                _replyPreviewTextLabel.TextColor = chatView.ReplyMessageTextColor.ToPlatform();
                _replyPreviewTextLabel.Font = UIFont.SystemFontOfSize((nfloat)chatView.ReplyMessageFontSize);

                _replySenderTextLabel.Text = RepliedMessage.GenerateTextPreview(message.ReplyToMessage.SenderId);
                _replySenderTextLabel.TextColor = chatView.ReplyMessageTextColor.ToPlatform();
                _replySenderTextLabel.Font = UIFont.SystemFontOfSize((nfloat)chatView.ReplyMessageFontSize);

                _replyView.Hidden = false;
                _replySenderTextLabel.Hidden = false;
                _replyPreviewTextLabel.Hidden = false;
            }
            else
            {
                _replyView.Hidden = true;
                _replySenderTextLabel.Hidden = true;
                _replyPreviewTextLabel.Hidden = true;
            }

            LayoutContentTop(showName, hasReply);

            _timeLabel.Font = UIFont.SystemFontOfSize((nfloat)chatView.MessageTimeFontSize);
            _timeLabel.TextColor = chatView.MessageTimeTextColor.ToPlatform();
            _timeLabel.Text = message.Timestamp.ToString("HH:mm");

            _reactionsStackView.UpdateReactions(message.Reactions, chatView);

            if (message.SenderAvatar != null)
            {
                _avatarImageView.Image = UIImage.LoadFromData(NSData.FromArray(message.SenderAvatar));
            }
            else
            {
                if (string.IsNullOrEmpty(message.SenderInitials))
                {
                    _avatarImageView.Image = UIImageExtensions.CreateInitialsImage(message.SenderId.Trim()[..2].ToUpperInvariant(), 40f, 40f,
                        chatView.AvatarTextColor.ToPlatform(), chatView.AvatarBackgroundColor.ToCGColor());
                }
                else
                {
                    _avatarImageView.Image = UIImageExtensions.CreateInitialsImage(message.SenderInitials, 40f, 40f,
                        chatView.AvatarTextColor.ToPlatform(), chatView.AvatarBackgroundColor.ToCGColor());
                }
            }

            // Delivery state
            _deliveryStateImageView.Image = null;
            _deliveryStateImageView.Hidden = true;

            switch (message.DeliveryState)
            {
                case MessageDeliveryState.Sent:
                    if (chatView.SendIcon != null)
                    {
                        _deliveryStateImageView.Image = UIImageExtensions.GetImageFromImageSource(mauiContext, chatView.SendIcon);
                    }
                    break;
                case MessageDeliveryState.Delivered:
                    if (chatView.DeliveredIcon != null)
                    {
                        _deliveryStateImageView.Image = UIImageExtensions.GetImageFromImageSource(mauiContext, chatView.DeliveredIcon);
                    }
                    break;
                case MessageDeliveryState.Read:
                    if (chatView.ReadIcon != null)
                    {
                        _deliveryStateImageView.Image = UIImageExtensions.GetImageFromImageSource(mauiContext, chatView.ReadIcon);
                    }
                    break;
                default:
                    break;
            }

            if (_deliveryStateImageView.Image != null)
            {
                _deliveryStateImageView.Hidden = false;
            }

            // Force layout refresh
            SetNeedsLayout();
            LayoutIfNeeded();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in {nameof(OtherImageMessageCell)}.{nameof(Update)}: {ex.Message}");
        }
    }
    
    // Pins the image view's height to its width by the image's aspect ratio so the bubble
    // sizes to the picture instead of the raw pixel dimensions. Priority is just below the
    // required max-height cap, so very tall images letterbox within the cap rather than conflict.
    private void ApplyImageAspect(UIImage image)
    {
        if (_imageAspectConstraint != null)
        {
            _imageAspectConstraint.Active = false;
            _imageAspectConstraint = null;
        }

        if (image == null || image.Size.Width <= 0 || image.Size.Height <= 0)
        {
            return;
        }

        var ratio = (nfloat)(image.Size.Height / image.Size.Width);
        _imageAspectConstraint = _imageView.HeightAnchor.ConstraintEqualTo(_imageView.WidthAnchor, multiplier: ratio);
        _imageAspectConstraint.Priority = (float)UILayoutPriority.Required - 1;
        _imageAspectConstraint.Active = true;
    }

    private void LongPressHandler(UILongPressGestureRecognizer recognizer)
    {
        if (recognizer.State == UIGestureRecognizerState.Began)
        {
            var contextMenu = new ChatContextMenuView(_chatView, _message, _bubbleView, DismissContextMenu);
            contextMenu.Show();
        }
    }

    private void DismissContextMenu()
    {
        Console.WriteLine("Context menu dismissed");
    }
}