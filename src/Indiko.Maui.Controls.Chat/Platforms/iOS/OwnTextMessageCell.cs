using CoreGraphics;
using Foundation;
using Indiko.Maui.Controls.Chat.Models;
using Microsoft.Maui.Platform;
using UIKit;

namespace Indiko.Maui.Controls.Chat.Platforms.iOS;

internal class OwnTextMessageCell : UICollectionViewCell
{
    public static readonly NSString Key = new(nameof(OwnTextMessageCell));
    ChatView _chatView;

    private UILabel _messageLabel;
    private UIView _bubbleView;
    private UILabel _timeLabel;
    private UIStackView _reactionsStackView;
    private UIImageView _deliveryStateImageView;

    public OwnTextMessageCell(ObjCRuntime.NativeHandle handle) : base(handle)
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
        // Nachrichtenblase (Hintergrund)
        _bubbleView = new UIView
        {
            TranslatesAutoresizingMaskIntoConstraints = false,
            BackgroundColor = UIColor.FromRGBA(113 / 255.0f, 0 / 255.0f, 223 / 255.0f, 1.0f), // Dunkles Lila
            ClipsToBounds = true
        };
        _bubbleView.Layer.CornerRadius = 16; // Abgerundete Ecken

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

        // delivery state setup
        _deliveryStateImageView = new UIImageView
        {
            TranslatesAutoresizingMaskIntoConstraints = false,
            ContentMode = UIViewContentMode.ScaleAspectFit,
            ClipsToBounds = true
        };

        ContentView.AddSubviews(_bubbleView, _messageLabel, _timeLabel, _deliveryStateImageView, _reactionsStackView);

        NSLayoutConstraint.ActivateConstraints(new[]
        {
            // Nachrichtenblase
            _bubbleView.TrailingAnchor.ConstraintEqualTo(ContentView.TrailingAnchor, -10),
            _bubbleView.LeadingAnchor.ConstraintGreaterThanOrEqualTo(ContentView.LeadingAnchor, 50),
            _bubbleView.TopAnchor.ConstraintEqualTo(ContentView.TopAnchor, 10),
            _bubbleView.BottomAnchor.ConstraintEqualTo(_reactionsStackView.TopAnchor, -4),

            // Nachrichtentext innerhalb der Blase
            _messageLabel.TopAnchor.ConstraintEqualTo(_bubbleView.TopAnchor, 10),
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

        /*// Message Emoji-reactions
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
            _deliveryStateImageView.HeightAnchor.ConstraintEqualTo(16)*/

    }

    public void Update(int index, ChatMessage message, ChatView chatView, IMauiContext mauiContext)
    {
        if (chatView == null || message == null || mauiContext == null)
        {
            return;
        }

        _chatView = chatView;

        try
        {
            // set width to 65% of the _bubbleView
            // var width = UIScreen.MainScreen.Bounds.Width * 0.65f;
            // _bubbleView.WidthAnchor.ConstraintGreaterThanOrEqualTo(width).Active = true;

            _messageLabel.Font = UIFont.SystemFontOfSize(chatView.MessageFontSize);
            _messageLabel.TextColor = chatView.OwnMessageTextColor.ToPlatform();
            _messageLabel.Text = message.TextContent;

            _timeLabel.Font = UIFont.SystemFontOfSize(chatView.MessageTimeFontSize);
            _timeLabel.TextColor = chatView.MessageTimeTextColor.ToPlatform();
            _timeLabel.Text = message.Timestamp.ToString("HH:mm");

            _bubbleView.BackgroundColor = chatView.OwnMessageBackgroundColor.ToPlatform();
            
            EmojiHelper.UpdateReactions(_reactionsStackView, message.Reactions, chatView);

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
            Console.WriteLine($"Error in {nameof(OwnTextMessageCell)}.{nameof(Update)}: {ex.Message}");
        }
    }
}