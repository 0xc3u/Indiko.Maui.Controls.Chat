using CoreGraphics;
using Foundation;
using Indiko.Maui.Controls.Chat.Models;
using Microsoft.Maui.Platform;
using UIKit;

namespace Indiko.Maui.Controls.Chat.Platforms.iOS;
internal abstract class BaseTextMessageCell : UICollectionViewCell
{
    protected UILabel _messageLabel;
    protected UIImageView _avatarImageView;
    protected UIView _bubbleView;
    protected UILabel _timeLabel;
    protected UIStackView _reactionsStackView;
    protected ChatView _chatView;
    protected UIImageView _deliveryStateImageView;

    protected UIView _replyView;
    protected UILabel _replyPreviewTextLabel;
    protected UILabel _replySenderTextLabel;

    protected NSLayoutConstraint _messageLabelTopConstraint;
    protected ChatMessage _message;

    protected BaseTextMessageCell(ObjCRuntime.NativeHandle handle) : base(handle)
    {
        SetupLayout();
    }

    protected abstract void SetupLayout();

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
            _bubbleView.BackgroundColor = GetBubbleBackgroundColor();

            _messageLabel.Font = UIFont.SystemFontOfSize(chatView.MessageFontSize);
            _messageLabel.TextColor = GetMessageTextColor();
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

            // Add tap gesture recognizers using the extension method
            _avatarImageView.AddWeakTapGestureRecognizerWithCommand(message, chatView.AvatarTappedCommand);
            _bubbleView.AddWeakTapGestureRecognizerWithCommand(message, chatView.MessageTappedCommand);
            _reactionsStackView.AddWeakTapGestureRecognizerWithCommand(message, chatView.EmojiReactionTappedCommand);

            // Force layout refresh
            SetNeedsLayout();
            LayoutIfNeeded();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in {GetType().Name}.{nameof(Update)}: {ex.Message}");
        }
    }

    protected abstract UIColor GetBubbleBackgroundColor();
    protected abstract UIColor GetMessageTextColor();
}
