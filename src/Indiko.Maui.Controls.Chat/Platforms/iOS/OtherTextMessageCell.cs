using CoreGraphics;
using Foundation;
using Indiko.Maui.Controls.Chat.Models;
using Microsoft.Maui.Platform;
using UIKit;

namespace Indiko.Maui.Controls.Chat.Platforms.iOS;

internal class OtherTextMessageCell : UICollectionViewCell
{
    public static readonly NSString Key = new(nameof(OtherTextMessageCell));

    private UILabel _messageLabel;
    private UIImageView _avatarImageView;
    private UIView _bubbleView;
    private UILabel _timeLabel;
    private UIStackView _reactionsStackView;
    private ChatView _chatView;
    private UIImageView _deliveryStateImageView;

    private UIView _replyView;
    private UILabel _replyPreviewTextLabel;
    private UILabel _replySenderTextLabel;

    private NSLayoutConstraint _messageLabelTopConstraint;
    private ChatMessage _message;

    public OtherTextMessageCell(ObjCRuntime.NativeHandle handle) : base(handle)
    {
        SetupLayout();
    }

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

        // Message text
        _messageLabel = new UILabel
        {
            Lines = 0, // Allows unlimited lines
            LineBreakMode = UILineBreakMode.WordWrap,
            TranslatesAutoresizingMaskIntoConstraints = false,
            TextAlignment = UITextAlignment.Left,
            TextColor = UIColor.Black
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
            TextAlignment = UITextAlignment.Left // Zeitstempel links ausrichten
        };

        // Add these lines to set Compression Resistance and Hugging Priorities
        _messageLabel.SetContentCompressionResistancePriority((float)UILayoutPriority.Required, UILayoutConstraintAxis.Vertical);
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

        // Add tap gesture recognizers
        var avatarTapGesture = new UITapGestureRecognizer(() => AvatarTapped(new WeakReference<ChatMessage>(_message)))
        {
            NumberOfTapsRequired = 1
        };
        _avatarImageView.AddGestureRecognizer(avatarTapGesture);
        _avatarImageView.UserInteractionEnabled = true;

        var messageTapGesture = new UITapGestureRecognizer(() => MessageTapped(new WeakReference<ChatMessage>(_message)))
        {
            NumberOfTapsRequired = 1
        };
        _bubbleView.AddGestureRecognizer(messageTapGesture);
        _bubbleView.UserInteractionEnabled = true;

        var emojiTapGesture = new UITapGestureRecognizer(() => EmojiReactionTapped(new WeakReference<ChatMessage>(_message)))
        {
            NumberOfTapsRequired = 1
        };
        _reactionsStackView.AddGestureRecognizer(emojiTapGesture);
        _reactionsStackView.UserInteractionEnabled = true;

        // add child views into hierarchical order
        ContentView.AddSubviews(_avatarImageView, _bubbleView, _messageLabel, _replyView, _replySenderTextLabel, _replyPreviewTextLabel, _timeLabel, _deliveryStateImageView, _reactionsStackView);

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
            _replyView.TopAnchor.ConstraintEqualTo(_bubbleView.TopAnchor, 10),
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

            // Message text inside chat bubble
            _messageLabelTopConstraint = _messageLabel.TopAnchor.ConstraintEqualTo(_replyView.BottomAnchor, 10),

            _messageLabel.BottomAnchor.ConstraintEqualTo(_bubbleView.BottomAnchor, -10),
            _messageLabel.LeadingAnchor.ConstraintEqualTo(_bubbleView.LeadingAnchor, 10),
            _messageLabel.TrailingAnchor.ConstraintEqualTo(_bubbleView.TrailingAnchor, -10),

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
            _bubbleView.BackgroundColor = chatView.OtherMessageBackgroundColor.ToPlatform();

            _messageLabel.Font = UIFont.SystemFontOfSize(chatView.MessageFontSize);
            _messageLabel.TextColor = chatView.OtherMessageTextColor.ToPlatform();
            _messageLabel.Text = message.TextContent;

            if (message.IsRepliedMessage && message.ReplyToMessage != null)
            {
                _replyView.BackgroundColor = chatView.ReplyMessageBackgroundColor.ToPlatform();

                _replyPreviewTextLabel.Text = RepliedMessage.GenerateTextPreview(message.ReplyToMessage.TextPreview);
                _replyPreviewTextLabel.TextColor = chatView.ReplyMessageTextColor.ToPlatform();
                _replyPreviewTextLabel.Font = UIFont.SystemFontOfSize(chatView.ReplyMessageFontSize);

                _replySenderTextLabel.Text = RepliedMessage.GenerateTextPreview(message.ReplyToMessage.SenderId);
                _replySenderTextLabel.TextColor = chatView.ReplyMessageTextColor.ToPlatform();
                _replySenderTextLabel.Font = UIFont.SystemFontOfSize(chatView.ReplyMessageFontSize);

                _replyView.Hidden = false;
                _replySenderTextLabel.Hidden = false;
                _replyPreviewTextLabel.Hidden = false;

                // Update the top constraint of the message label
                _messageLabelTopConstraint.Active = false;
                _messageLabelTopConstraint = _messageLabel.TopAnchor.ConstraintEqualTo(_replyView.BottomAnchor, 10);
                _messageLabelTopConstraint.Active = true;
            }
            else
            {
                _replyView.Hidden = true;
                _replySenderTextLabel.Hidden = true;
                _replyPreviewTextLabel.Hidden = true;

                // Update the top constraint of the message label
                _messageLabelTopConstraint.Active = false;
                _messageLabelTopConstraint = _messageLabel.TopAnchor.ConstraintEqualTo(_bubbleView.TopAnchor, 10);
                _messageLabelTopConstraint.Active = true;
            }

            _timeLabel.Font = UIFont.SystemFontOfSize(chatView.MessageTimeFontSize);
            _timeLabel.TextColor = chatView.MessageTimeTextColor.ToPlatform();
            _timeLabel.Text = message.Timestamp.ToString("HH:mm");

            EmojiHelper.UpdateReactions(_reactionsStackView, message.Reactions, chatView);

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
            Console.WriteLine($"Error in {nameof(OtherTextMessageCell)}.{nameof(Update)}: {ex.Message}");
        }
    }

    private void AvatarTapped(WeakReference<ChatMessage> messageRef)
    {
        if (messageRef.TryGetTarget(out var message) && _chatView.AvatarTappedCommand?.CanExecute(message) == true)
        {
            _chatView.AvatarTappedCommand.Execute(message);
            AnimateFade(_avatarImageView);
        }
    }

    private void MessageTapped(WeakReference<ChatMessage> messageRef)
    {
        if (messageRef.TryGetTarget(out var message) && _chatView.MessageTappedCommand?.CanExecute(message) == true)
        {
            _chatView.MessageTappedCommand.Execute(message);
            AnimateFade(_bubbleView);
        }
    }

    private void EmojiReactionTapped(WeakReference<ChatMessage> messageRef)
    {
        if (messageRef.TryGetTarget(out var message) && _chatView.EmojiReactionTappedCommand?.CanExecute(message) == true)
        {
            _chatView.EmojiReactionTappedCommand.Execute(message);
            AnimateFade(_reactionsStackView);
        }
    }

    private void AnimateFade(UIView view)
    {
        UIView.Animate(0.2, 0, UIViewAnimationOptions.CurveEaseInOut, () =>
        {
            view.Alpha = 0.7f;
        }, () =>
        {
            UIView.Animate(0.2, 0, UIViewAnimationOptions.CurveEaseInOut, () =>
            {
                view.Alpha = 1.0f;
            }, null);
        });
    }
}



//internal sealed class OtherTextMessageCell : UICollectionViewCell
//{
//    public static readonly NSString Key = new(nameof(OtherTextMessageCell));

//    private UILabel _messageLabel;
//    private UIImageView _avatarImageView;
//    private UIView _bubbleView;
//    private UILabel _timeLabel;
//    private UIStackView _reactionsStackView;
//    private ChatView _chatView;
//    private UIImageView _deliveryStateImageView;

//    private UIView _replyView;
//    private UILabel _replyPreviewTextLabel;
//    private UILabel _replySenderTextLabel;

//    private NSLayoutConstraint _messageLabelTopConstraint;

//    public OtherTextMessageCell(ObjCRuntime.NativeHandle handle) : base(handle)
//    {
//        SetupLayout();
//    }

//    public override UICollectionViewLayoutAttributes PreferredLayoutAttributesFittingAttributes(UICollectionViewLayoutAttributes layoutAttributes)
//    {
//        // Update the layout attributes for auto-sizing
//        SetNeedsLayout();
//        LayoutIfNeeded();

//        var widthConstraint = ContentView.WidthAnchor.ConstraintEqualTo(layoutAttributes.Frame.Width);
//        widthConstraint.Active = true;

//        // Calculate the size fitting the content
//        var size = ContentView.SystemLayoutSizeFittingSize(UIView.UILayoutFittingCompressedSize);
//        widthConstraint.Active = false;

//        var updatedAttributes = layoutAttributes.Copy() as UICollectionViewLayoutAttributes;
//        updatedAttributes.Frame = new CGRect(0, updatedAttributes.Frame.Y, layoutAttributes.Frame.Width, size.Height);

//        return updatedAttributes;
//    }

//    private void SetupLayout()
//    {
//        // Avatar setup
//        _avatarImageView = new UIImageView
//        {
//            TranslatesAutoresizingMaskIntoConstraints = false,
//            ContentMode = UIViewContentMode.ScaleAspectFill,
//            ClipsToBounds = true
//        };
//        _avatarImageView.Layer.CornerRadius = 20;

//        // Chat bubble setup
//        _bubbleView = new UIView
//        {
//            TranslatesAutoresizingMaskIntoConstraints = false,
//            BackgroundColor = UIColor.FromRGBA(230 / 255.0f, 223 / 255.0f, 255 / 255.0f, 1.0f),
//            ClipsToBounds = true
//        };
//        _bubbleView.Layer.CornerRadius = 16;

//        // Message text
//        _messageLabel = new UILabel
//        {
//            Lines = 0, // Allows unlimited lines
//            LineBreakMode = UILineBreakMode.WordWrap,
//            TranslatesAutoresizingMaskIntoConstraints = false,
//            TextAlignment = UITextAlignment.Left,
//            TextColor = UIColor.Black
//        };

//        // Chat reply view setup
//        _replyView = new UIView
//        {
//            TranslatesAutoresizingMaskIntoConstraints = false,
//            BackgroundColor = UIColor.FromRGBA(230 / 255.0f, 223 / 255.0f, 255 / 255.0f, 1.0f),
//            ClipsToBounds = true
//        };
//        _replyView.Layer.CornerRadius = 4;
//        _replyPreviewTextLabel = new UILabel
//        {
//            Lines = 0, // Allows unlimited lines
//            LineBreakMode = UILineBreakMode.WordWrap,
//            TranslatesAutoresizingMaskIntoConstraints = false,
//            TextAlignment = UITextAlignment.Left,
//            TextColor = UIColor.Black
//        };

//        _replySenderTextLabel = new UILabel
//        {
//            Lines = 1,
//            LineBreakMode = UILineBreakMode.TailTruncation,
//            TranslatesAutoresizingMaskIntoConstraints = false,
//            TextAlignment = UITextAlignment.Left,
//            TextColor = UIColor.Black
//        };

//        // Message timestamp
//        _timeLabel = new UILabel
//        {
//            Font = UIFont.SystemFontOfSize(12),
//            TextColor = UIColor.LightGray,
//            TranslatesAutoresizingMaskIntoConstraints = false,
//            TextAlignment = UITextAlignment.Left // Zeitstempel links ausrichten
//        };

//        // Add these lines to set Compression Resistance and Hugging Priorities
//        _messageLabel.SetContentCompressionResistancePriority((float)UILayoutPriority.Required, UILayoutConstraintAxis.Vertical);
//        _timeLabel.SetContentHuggingPriority((float)UILayoutPriority.DefaultLow, UILayoutConstraintAxis.Horizontal);

//        // Message reaction stack (horizontal Emoji-List)
//        _reactionsStackView = new UIStackView
//        {
//            Axis = UILayoutConstraintAxis.Horizontal,
//            Distribution = UIStackViewDistribution.Fill,
//            Alignment = UIStackViewAlignment.Center,
//            Spacing = 8,
//            TranslatesAutoresizingMaskIntoConstraints = false,
//        };

//        // delivery state setup
//        _deliveryStateImageView = new UIImageView
//        {
//            TranslatesAutoresizingMaskIntoConstraints = false,
//            ContentMode = UIViewContentMode.ScaleAspectFit,
//            ClipsToBounds = true
//        };

//        // add child views into hierarchical order
//        ContentView.AddSubviews(_avatarImageView, _bubbleView, _messageLabel, _replyView, _replySenderTextLabel, _replyPreviewTextLabel, _timeLabel, _deliveryStateImageView, _reactionsStackView);

//        // Layout-Constraints
//        NSLayoutConstraint.ActivateConstraints(new[]
//        {
//            // Avatar
//            _avatarImageView.LeadingAnchor.ConstraintEqualTo(ContentView.LeadingAnchor, 10),
//            _avatarImageView.TopAnchor.ConstraintEqualTo(ContentView.TopAnchor, 10),
//            _avatarImageView.WidthAnchor.ConstraintEqualTo(40),
//            _avatarImageView.HeightAnchor.ConstraintEqualTo(40),

//            // Chat bubble
//            _bubbleView.LeadingAnchor.ConstraintEqualTo(_avatarImageView.TrailingAnchor, 10),
//            _bubbleView.TrailingAnchor.ConstraintLessThanOrEqualTo(ContentView.TrailingAnchor, -50),
//            _bubbleView.TopAnchor.ConstraintEqualTo(ContentView.TopAnchor, 10),
//            _bubbleView.BottomAnchor.ConstraintEqualTo(_reactionsStackView.TopAnchor, -4),

//            // Message reply view inside chat bubble
//            _replyView.TopAnchor.ConstraintEqualTo(_bubbleView.TopAnchor, 10),
//            _replyView.LeadingAnchor.ConstraintEqualTo(_bubbleView.LeadingAnchor, 10),
//            _replyView.TrailingAnchor.ConstraintEqualTo(_bubbleView.TrailingAnchor, -10),

//            // Reply sender text inside reply view
//            _replySenderTextLabel.TopAnchor.ConstraintEqualTo(_replyView.TopAnchor, 10),
//            _replySenderTextLabel.LeadingAnchor.ConstraintEqualTo(_replyView.LeadingAnchor, 10),
//            _replySenderTextLabel.TrailingAnchor.ConstraintEqualTo(_replyView.TrailingAnchor, -10),

//            // Reply preview text inside reply view
//            _replyPreviewTextLabel.TopAnchor.ConstraintEqualTo(_replySenderTextLabel.BottomAnchor, 4),
//            _replyPreviewTextLabel.LeadingAnchor.ConstraintEqualTo(_replyView.LeadingAnchor, 10),
//            _replyPreviewTextLabel.TrailingAnchor.ConstraintEqualTo(_replyView.TrailingAnchor, -10),
//            _replyPreviewTextLabel.BottomAnchor.ConstraintEqualTo(_replyView.BottomAnchor, -10),

//            // Message text inside chat bubble
//            _messageLabelTopConstraint = _messageLabel.TopAnchor.ConstraintEqualTo(_replyView.BottomAnchor, 10),

//            _messageLabel.BottomAnchor.ConstraintEqualTo(_bubbleView.BottomAnchor, -10),
//            _messageLabel.LeadingAnchor.ConstraintEqualTo(_bubbleView.LeadingAnchor, 10),
//            _messageLabel.TrailingAnchor.ConstraintEqualTo(_bubbleView.TrailingAnchor, -10),

//            // Message Emoji-reactions
//            _reactionsStackView.TopAnchor.ConstraintEqualTo(_bubbleView.BottomAnchor, 4),
//            _reactionsStackView.TrailingAnchor.ConstraintEqualTo(_bubbleView.TrailingAnchor),
//            _reactionsStackView.LeadingAnchor.ConstraintGreaterThanOrEqualTo(_bubbleView.LeadingAnchor),
//            _reactionsStackView.WidthAnchor.ConstraintLessThanOrEqualTo(_bubbleView.WidthAnchor, 0.5f), // limit width to 50% of the chat bubble width

//            // Message time stamp
//            _timeLabel.TopAnchor.ConstraintEqualTo(_reactionsStackView.TopAnchor, 4),
//            _timeLabel.LeadingAnchor.ConstraintEqualTo(_bubbleView.LeadingAnchor, 10),
//            _timeLabel.BottomAnchor.ConstraintEqualTo(ContentView.BottomAnchor, -10),

//            // Delivery state icon
//            _deliveryStateImageView.CenterYAnchor.ConstraintEqualTo(_timeLabel.CenterYAnchor),
//            _deliveryStateImageView.LeadingAnchor.ConstraintEqualTo(_timeLabel.TrailingAnchor, 8),
//            _deliveryStateImageView.WidthAnchor.ConstraintEqualTo(16),
//            _deliveryStateImageView.HeightAnchor.ConstraintEqualTo(16)
//        });
//    }

//    public void Update(int index, ChatMessage message, ChatView chatView, IMauiContext mauiContext)
//    {
//        if (chatView == null || message == null || mauiContext == null)
//        {
//            return;
//        }

//        try
//        {
//            _chatView = chatView;

//            _bubbleView.BackgroundColor = chatView.OtherMessageBackgroundColor.ToPlatform();

//            _messageLabel.Font = UIFont.SystemFontOfSize(chatView.MessageFontSize);
//            _messageLabel.TextColor = chatView.OtherMessageTextColor.ToPlatform();
//            _messageLabel.Text = message.TextContent;

//            if (message.IsRepliedMessage && message.ReplyToMessage != null)
//            {
//                _replyView.BackgroundColor = chatView.ReplyMessageBackgroundColor.ToPlatform();

//                _replyPreviewTextLabel.Text = RepliedMessage.GenerateTextPreview(message.ReplyToMessage.TextPreview);
//                _replyPreviewTextLabel.TextColor = chatView.ReplyMessageTextColor.ToPlatform();
//                _replyPreviewTextLabel.Font = UIFont.SystemFontOfSize(chatView.ReplyMessageFontSize);

//                _replySenderTextLabel.Text = RepliedMessage.GenerateTextPreview(message.ReplyToMessage.SenderId);
//                _replySenderTextLabel.TextColor = chatView.ReplyMessageTextColor.ToPlatform();
//                _replySenderTextLabel.Font = UIFont.SystemFontOfSize(chatView.ReplyMessageFontSize);

//                _replyView.Hidden = false;
//                _replySenderTextLabel.Hidden = false;
//                _replyPreviewTextLabel.Hidden = false;

//                // Update the top constraint of the message label
//                _messageLabelTopConstraint.Active = false;
//                _messageLabelTopConstraint = _messageLabel.TopAnchor.ConstraintEqualTo(_replyView.BottomAnchor, 10);
//                _messageLabelTopConstraint.Active = true;
//            }
//            else
//            {
//                _replyView.Hidden = true;
//                _replySenderTextLabel.Hidden = true;
//                _replyPreviewTextLabel.Hidden = true;

//                // Update the top constraint of the message label
//                _messageLabelTopConstraint.Active = false;
//                _messageLabelTopConstraint = _messageLabel.TopAnchor.ConstraintEqualTo(_bubbleView.TopAnchor, 10);
//                _messageLabelTopConstraint.Active = true;
//            }

//            _timeLabel.Font = UIFont.SystemFontOfSize(chatView.MessageTimeFontSize);
//            _timeLabel.TextColor = chatView.MessageTimeTextColor.ToPlatform();
//            _timeLabel.Text = message.Timestamp.ToString("HH:mm");

//            EmojiHelper.UpdateReactions(_reactionsStackView, message.Reactions, chatView);

//            if (message.SenderAvatar != null)
//            {
//                _avatarImageView.Image = UIImage.LoadFromData(NSData.FromArray(message.SenderAvatar));
//            }
//            else
//            {
//                if (string.IsNullOrEmpty(message.SenderInitials))
//                {
//                    _avatarImageView.Image = UIImageExtensions.CreateInitialsImage(message.SenderId.Trim()[..2].ToUpperInvariant(), 40f, 40f,
//                        chatView.AvatarTextColor.ToPlatform(), chatView.AvatarBackgroundColor.ToCGColor());
//                }
//                else
//                {
//                    _avatarImageView.Image = UIImageExtensions.CreateInitialsImage(message.SenderInitials, 40f, 40f,
//                        chatView.AvatarTextColor.ToPlatform(), chatView.AvatarBackgroundColor.ToCGColor());
//                }
//            }

//            // Delivery state
//            _deliveryStateImageView.Image = null;
//            _deliveryStateImageView.Hidden = true;

//            switch (message.DeliveryState)
//            {
//                case MessageDeliveryState.Sent:
//                    if (chatView.SendIcon != null)
//                    {
//                        _deliveryStateImageView.Image = UIImageExtensions.GetImageFromImageSource(mauiContext, chatView.SendIcon);
//                    }
//                    break;
//                case MessageDeliveryState.Delivered:
//                    if (chatView.DeliveredIcon != null)
//                    {
//                        _deliveryStateImageView.Image = UIImageExtensions.GetImageFromImageSource(mauiContext, chatView.DeliveredIcon);
//                    }
//                    break;
//                case MessageDeliveryState.Read:
//                    if (chatView.ReadIcon != null)
//                    {
//                        _deliveryStateImageView.Image = UIImageExtensions.GetImageFromImageSource(mauiContext, chatView.ReadIcon);
//                    }
//                    break;
//                default:
//                    break;
//            }

//            if (_deliveryStateImageView.Image != null)
//            {
//                _deliveryStateImageView.Hidden = false;
//            }

//            // Force layout refresh
//            SetNeedsLayout();
//            LayoutIfNeeded();
//        }
//        catch (Exception ex)
//        {
//            Console.WriteLine($"Error in {nameof(OtherTextMessageCell)}.{nameof(Update)}: {ex.Message}");
//        }
//    }
//}