using CoreGraphics;
using Foundation;
using Indiko.Maui.Controls.Chat.Models;
using Microsoft.Maui.Platform;
using UIKit;

namespace Indiko.Maui.Controls.Chat.Platforms.iOS;

internal class OwnTextMessageCell : BaseTextMessageCell
{
    public static readonly NSString Key = new(nameof(OwnTextMessageCell));

    public OwnTextMessageCell(ObjCRuntime.NativeHandle handle) : base(handle)
    {
    }

    protected override void SetupLayout()
    {
        // Nachrichtenblase (Hintergrund)
        _bubbleView = new UIView
        {
            TranslatesAutoresizingMaskIntoConstraints = false,
            BackgroundColor = UIColor.FromRGBA(113 / 255.0f, 0 / 255.0f, 223 / 255.0f, 1.0f), // Dunkles Lila
            ClipsToBounds = true
        };
        _bubbleView.Layer.CornerRadius = 16; // Abgerundete Ecken

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

        // Nachrichtentext
        _messageLabel = new UILabel
        {
            Lines = 0, // Mehrzeiliger Text
            LineBreakMode = UILineBreakMode.WordWrap,
            TranslatesAutoresizingMaskIntoConstraints = false,
            TextAlignment = UITextAlignment.Left,
            TextColor = UIColor.White // Weißer Text für Kontrast
        };

        // Zeitstempel
        _timeLabel = new UILabel
        {
            Font = UIFont.SystemFontOfSize(12),
            TextColor = UIColor.LightGray,
            TranslatesAutoresizingMaskIntoConstraints = false,
            TextAlignment = UITextAlignment.Right // Rechtsbündig
        };

        // Reaktionsstack (Horizontale Emoji-Liste)
        _reactionsStackView = new UIStackView
        {
            Axis = UILayoutConstraintAxis.Horizontal,
            Distribution = UIStackViewDistribution.Fill,
            Alignment = UIStackViewAlignment.Center,
            Spacing = 8,
            TranslatesAutoresizingMaskIntoConstraints = false
        };

        // Delivery state setup
        _deliveryStateImageView = new UIImageView
        {
            TranslatesAutoresizingMaskIntoConstraints = false,
            ContentMode = UIViewContentMode.ScaleAspectFit,
            ClipsToBounds = true
        };

        // Add child views into hierarchical order
        ContentView.AddSubviews(_bubbleView, _messageLabel, _replyView, _replySenderTextLabel, _replyPreviewTextLabel, _timeLabel, _deliveryStateImageView, _reactionsStackView);

        // Layout-Constraints
        NSLayoutConstraint.ActivateConstraints(new[]
        {
            // Chat bubble
            _bubbleView.TrailingAnchor.ConstraintEqualTo(ContentView.TrailingAnchor, -10),
            _bubbleView.LeadingAnchor.ConstraintGreaterThanOrEqualTo(ContentView.LeadingAnchor, 50),
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

            // Emoji-Reaktionen
            _reactionsStackView.TopAnchor.ConstraintEqualTo(_bubbleView.BottomAnchor, 4),
            _reactionsStackView.LeadingAnchor.ConstraintEqualTo(_bubbleView.LeadingAnchor),
            _reactionsStackView.TrailingAnchor.ConstraintLessThanOrEqualTo(_bubbleView.TrailingAnchor),
            _reactionsStackView.WidthAnchor.ConstraintLessThanOrEqualTo(_bubbleView.WidthAnchor, 0.5f), // limit width to 50% of the chat bubble width

            // Zeitstempel
            _timeLabel.TopAnchor.ConstraintEqualTo(_reactionsStackView.TopAnchor, 4),
            _timeLabel.TrailingAnchor.ConstraintEqualTo(_deliveryStateImageView.LeadingAnchor, -4),
            _timeLabel.LeadingAnchor.ConstraintEqualTo(_bubbleView.LeadingAnchor),
            _timeLabel.BottomAnchor.ConstraintEqualTo(ContentView.BottomAnchor, -10),

            _deliveryStateImageView.CenterYAnchor.ConstraintEqualTo(_timeLabel.CenterYAnchor),
            _deliveryStateImageView.TrailingAnchor.ConstraintEqualTo(_bubbleView.TrailingAnchor),
            _deliveryStateImageView.WidthAnchor.ConstraintEqualTo(16),
            _deliveryStateImageView.HeightAnchor.ConstraintEqualTo(16)
        });
    }

    protected override UIColor GetBubbleBackgroundColor()
    {
        return _chatView.OwnMessageBackgroundColor.ToPlatform();
    }

    protected override UIColor GetMessageTextColor()
    {
        return _chatView.OwnMessageTextColor.ToPlatform();
    }
}


//internal class OwnTextMessageCell : UICollectionViewCell
//{
//    public static readonly NSString Key = new(nameof(OwnTextMessageCell));
//    private ChatView _chatView;
//    private ChatMessage _message;

//    private UILabel _messageLabel;
//    private UIView _bubbleView;
//    private UILabel _timeLabel;
//    private UIStackView _reactionsStackView;
//    private UIImageView _deliveryStateImageView;

//    private UIView _replyView;
//    private UILabel _replyPreviewTextLabel;
//    private UILabel _replySenderTextLabel;
//    private NSLayoutConstraint _messageLabelTopConstraint;

//    public OwnTextMessageCell(ObjCRuntime.NativeHandle handle) : base(handle)
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
//        // Chat bubble setup
//        _bubbleView = new UIView
//        {
//            TranslatesAutoresizingMaskIntoConstraints = false,
//            BackgroundColor = UIColor.Clear,
//            ClipsToBounds = true
//        };
//        _bubbleView.Layer.CornerRadius = 16;

//        // Chat reply view setup
//        _replyView = new UIView
//        {
//            TranslatesAutoresizingMaskIntoConstraints = false,
//            BackgroundColor = UIColor.Clear,
//            ClipsToBounds = true
//        };
//        _replyView.Layer.CornerRadius = 4;
//        _replyPreviewTextLabel = new UILabel
//        {
//            Lines = 0,
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

//        // Message text
//        _messageLabel = new UILabel
//        {
//            Lines = 0, 
//            LineBreakMode = UILineBreakMode.WordWrap,
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
//            TextAlignment = UITextAlignment.Right
//        };

//        // Message reaction stack (horizontal Emoji-List)
//        _reactionsStackView = new UIStackView
//        {
//            Axis = UILayoutConstraintAxis.Horizontal,
//            Distribution = UIStackViewDistribution.Fill,
//            Alignment = UIStackViewAlignment.Center,
//            Spacing = 8,
//            TranslatesAutoresizingMaskIntoConstraints = false
//        };

//        // delivery state setup
//        _deliveryStateImageView = new UIImageView
//        {
//            TranslatesAutoresizingMaskIntoConstraints = false,
//            ContentMode = UIViewContentMode.ScaleAspectFit,
//            ClipsToBounds = true
//        };

//        ContentView.AddSubviews(_bubbleView, _messageLabel, _replyView, _replySenderTextLabel, _replyPreviewTextLabel, _timeLabel, _deliveryStateImageView, _reactionsStackView);

//        NSLayoutConstraint.ActivateConstraints(new[]
//        {
//            // Chat bubble
//            _bubbleView.TrailingAnchor.ConstraintEqualTo(ContentView.TrailingAnchor, -10),
//            _bubbleView.LeadingAnchor.ConstraintGreaterThanOrEqualTo(ContentView.LeadingAnchor, 50),
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

//            // Emoji-Reaktionen
//            _reactionsStackView.TopAnchor.ConstraintEqualTo(_bubbleView.BottomAnchor, 4),
//            _reactionsStackView.LeadingAnchor.ConstraintEqualTo(_bubbleView.LeadingAnchor),
//            _reactionsStackView.TrailingAnchor.ConstraintLessThanOrEqualTo(_bubbleView.TrailingAnchor),
//            _reactionsStackView.WidthAnchor.ConstraintLessThanOrEqualTo(_bubbleView.WidthAnchor, 0.5f), // limit width to 50% of the chat bubble width

//            // Zeitstempel
//            _timeLabel.TopAnchor.ConstraintEqualTo(_reactionsStackView.TopAnchor, 4),
//            _timeLabel.TrailingAnchor.ConstraintEqualTo(_deliveryStateImageView.LeadingAnchor, -4),
//            _timeLabel.LeadingAnchor.ConstraintEqualTo(_bubbleView.LeadingAnchor),
//            _timeLabel.BottomAnchor.ConstraintEqualTo(ContentView.BottomAnchor, -10),

//            _deliveryStateImageView.CenterYAnchor.ConstraintEqualTo(_timeLabel.CenterYAnchor),
//            _deliveryStateImageView.TrailingAnchor.ConstraintEqualTo(_bubbleView.TrailingAnchor),
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

//        _message = message;
//        _chatView = chatView;

//        try
//        {
//            // set width to 65% of the _bubbleView
//            // var width = UIScreen.MainScreen.Bounds.Width * 0.65f;
//            // _bubbleView.WidthAnchor.ConstraintGreaterThanOrEqualTo(width).Active = true;

//            _bubbleView.AddWeakTapGestureRecognizerWithCommand(_message, _chatView.MessageTappedCommand);
//            _bubbleView.UserInteractionEnabled = true;

//            _reactionsStackView.AddWeakTapGestureRecognizerWithCommand(_message, _chatView.EmojiReactionTappedCommand);
//            _reactionsStackView.UserInteractionEnabled = true;


//            _messageLabel.Font = UIFont.SystemFontOfSize(chatView.MessageFontSize);
//            _messageLabel.TextColor = chatView.OwnMessageTextColor.ToPlatform();
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

//            _bubbleView.BackgroundColor = chatView.OwnMessageBackgroundColor.ToPlatform();

//            _reactionsStackView.UpdateReactions(message.Reactions, chatView);

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
//            Console.WriteLine($"Error in {nameof(OwnTextMessageCell)}.{nameof(Update)}: {ex.Message}");
//        }
//    }
//}