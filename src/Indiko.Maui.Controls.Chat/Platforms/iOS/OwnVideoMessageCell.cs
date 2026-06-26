using CoreGraphics;
using Foundation;
using Indiko.Maui.Controls.Chat.Models;
using Microsoft.Maui.Platform;
using UIKit;

namespace Indiko.Maui.Controls.Chat.Platforms.iOS;

internal class OwnVideoMessageCell : UICollectionViewCell
{
    public static readonly NSString Key = new(nameof(OwnVideoMessageCell));
    private ChatView _chatView;
    private ChatMessage _message;

    private VideoBubbleView _videoBubble;

    private UIView _bubbleView;
    private UILabel _timeLabel;
    private UIStackView _reactionsStackView;
    private UIImageView _deliveryStateImageView;

    private UIView _replyView;
    private UILabel _replyPreviewTextLabel;
    private UILabel _replySenderTextLabel;

    private NSLayoutConstraint _messageVideoTopConstraint;
    private UILongPressGestureRecognizer _longPressGesture;

    public OwnVideoMessageCell(ObjCRuntime.NativeHandle handle) : base(handle)
    {
        SetupLayout();
    }

    public override UICollectionViewLayoutAttributes PreferredLayoutAttributesFittingAttributes(UICollectionViewLayoutAttributes layoutAttributes)
    {
        return CellSizingHelper.CalculateFittingAttributes(layoutAttributes, ContentView, _message?.MessageId);
    }

    public override void PrepareForReuse()
    {
        base.PrepareForReuse();
        _videoBubble?.ResetToPoster();
    }

    private void SetupLayout()
    {
        _bubbleView = new UIView
        {
            TranslatesAutoresizingMaskIntoConstraints = false,
            BackgroundColor = UIColor.FromRGBA(113 / 255.0f, 0 / 255.0f, 223 / 255.0f, 1.0f),
            ClipsToBounds = true
        };
        _bubbleView.Layer.CornerRadius = 16;

        _replyView = new UIView
        {
            TranslatesAutoresizingMaskIntoConstraints = false,
            BackgroundColor = UIColor.FromRGBA(230 / 255.0f, 223 / 255.0f, 255 / 255.0f, 1.0f),
            ClipsToBounds = true
        };
        _replyView.Layer.CornerRadius = 4;
        _replyPreviewTextLabel = new UILabel
        {
            Lines = 0,
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

        // Inline video bubble: blurred poster + play button, plays on tap (no auto-play).
        _videoBubble = new VideoBubbleView
        {
            TranslatesAutoresizingMaskIntoConstraints = false
        };

        _timeLabel = new UILabel
        {
            Font = UIFont.SystemFontOfSize(12),
            TextColor = UIColor.LightGray,
            TranslatesAutoresizingMaskIntoConstraints = false,
            TextAlignment = UITextAlignment.Right
        };

        _timeLabel.SetContentHuggingPriority((float)UILayoutPriority.DefaultLow, UILayoutConstraintAxis.Horizontal);

        _reactionsStackView = new UIStackView
        {
            Axis = UILayoutConstraintAxis.Horizontal,
            Distribution = UIStackViewDistribution.Fill,
            Alignment = UIStackViewAlignment.Center,
            Spacing = 8,
            TranslatesAutoresizingMaskIntoConstraints = false
        };

        _deliveryStateImageView = new UIImageView
        {
            TranslatesAutoresizingMaskIntoConstraints = false,
            ContentMode = UIViewContentMode.ScaleAspectFit,
            ClipsToBounds = true
        };

        ContentView.AddSubviews(_bubbleView, _videoBubble, _replyView, _replySenderTextLabel, _replyPreviewTextLabel, _timeLabel, _deliveryStateImageView, _reactionsStackView);

        NSLayoutConstraint.ActivateConstraints(new[]
        {
            _bubbleView.TrailingAnchor.ConstraintEqualTo(ContentView.TrailingAnchor, -10),
            _bubbleView.LeadingAnchor.ConstraintGreaterThanOrEqualTo(ContentView.LeadingAnchor, 50),
            _bubbleView.TopAnchor.ConstraintEqualTo(ContentView.TopAnchor, 10),
            _bubbleView.BottomAnchor.ConstraintEqualTo(_reactionsStackView.TopAnchor, -4),

            _replyView.TopAnchor.ConstraintEqualTo(_bubbleView.TopAnchor, 10),
            _replyView.LeadingAnchor.ConstraintEqualTo(_bubbleView.LeadingAnchor, 10),
            _replyView.TrailingAnchor.ConstraintEqualTo(_bubbleView.TrailingAnchor, -10),

            _replySenderTextLabel.TopAnchor.ConstraintEqualTo(_replyView.TopAnchor, 10),
            _replySenderTextLabel.LeadingAnchor.ConstraintEqualTo(_replyView.LeadingAnchor, 10),
            _replySenderTextLabel.TrailingAnchor.ConstraintEqualTo(_replyView.TrailingAnchor, -10),

            _replyPreviewTextLabel.TopAnchor.ConstraintEqualTo(_replySenderTextLabel.BottomAnchor, 4),
            _replyPreviewTextLabel.LeadingAnchor.ConstraintEqualTo(_replyView.LeadingAnchor, 10),
            _replyPreviewTextLabel.TrailingAnchor.ConstraintEqualTo(_replyView.TrailingAnchor, -10),
            _replyPreviewTextLabel.BottomAnchor.ConstraintEqualTo(_replyView.BottomAnchor, -10),

            // Inline video bubble inside the chat bubble
            _messageVideoTopConstraint = _videoBubble.TopAnchor.ConstraintEqualTo(_bubbleView.TopAnchor, 10),
            _videoBubble.BottomAnchor.ConstraintEqualTo(_bubbleView.BottomAnchor, -10),
            _videoBubble.LeadingAnchor.ConstraintEqualTo(_bubbleView.LeadingAnchor, 10),
            _videoBubble.TrailingAnchor.ConstraintEqualTo(_bubbleView.TrailingAnchor, -10),
            _videoBubble.WidthAnchor.ConstraintEqualTo(240),
            _videoBubble.HeightAnchor.ConstraintEqualTo(160),

            _reactionsStackView.TopAnchor.ConstraintEqualTo(_bubbleView.BottomAnchor, 4),
            _reactionsStackView.LeadingAnchor.ConstraintEqualTo(_bubbleView.LeadingAnchor),
            _reactionsStackView.TrailingAnchor.ConstraintLessThanOrEqualTo(_bubbleView.TrailingAnchor),
            _reactionsStackView.WidthAnchor.ConstraintLessThanOrEqualTo(_bubbleView.WidthAnchor, 0.5f),

            _timeLabel.TopAnchor.ConstraintEqualTo(_reactionsStackView.TopAnchor, 4),
            _timeLabel.TrailingAnchor.ConstraintEqualTo(_deliveryStateImageView.LeadingAnchor, -4),
            _timeLabel.LeadingAnchor.ConstraintEqualTo(_bubbleView.LeadingAnchor),
            _timeLabel.BottomAnchor.ConstraintEqualTo(ContentView.BottomAnchor, -10),

            _deliveryStateImageView.CenterYAnchor.ConstraintEqualTo(_timeLabel.CenterYAnchor),
            _deliveryStateImageView.TrailingAnchor.ConstraintEqualTo(_bubbleView.TrailingAnchor),
            _deliveryStateImageView.WidthAnchor.ConstraintEqualTo(16),
            _deliveryStateImageView.HeightAnchor.ConstraintEqualTo(16)
        });

        _longPressGesture = new UILongPressGestureRecognizer(LongPressHandler);
        _bubbleView.AddGestureRecognizer(_longPressGesture);

        ContentView.Transform = CoreGraphics.CGAffineTransform.MakeScale(1, -1);
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

            _bubbleView.BackgroundColor = chatView.OwnMessageBackgroundColor.ToPlatform();

            if (message.BinaryContent != null)
            {
                var tempFile = Path.Combine(FileSystem.Current.CacheDirectory, $"{message.MessageId}.mp4");
                if (!File.Exists(tempFile))
                {
                    File.WriteAllBytes(tempFile, message.BinaryContent);
                }

                _videoBubble.Hidden = false;
                _videoBubble.Configure(tempFile);
            }
            else
            {
                _videoBubble.Hidden = true;
            }

            if (message.IsRepliedMessage && message.ReplyToMessage != null)
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

                _messageVideoTopConstraint.Active = false;
                _messageVideoTopConstraint = _videoBubble.TopAnchor.ConstraintEqualTo(_replyView.BottomAnchor, 10);
                _messageVideoTopConstraint.Active = true;
            }
            else
            {
                _replyView.Hidden = true;
                _replySenderTextLabel.Hidden = true;
                _replyPreviewTextLabel.Hidden = true;

                _messageVideoTopConstraint.Active = false;
                _messageVideoTopConstraint = _videoBubble.TopAnchor.ConstraintEqualTo(_bubbleView.TopAnchor, 10);
                _messageVideoTopConstraint.Active = true;
            }

            _timeLabel.Font = UIFont.SystemFontOfSize((nfloat)chatView.MessageTimeFontSize);
            _timeLabel.TextColor = chatView.MessageTimeTextColor.ToPlatform();
            _timeLabel.Text = message.Timestamp.ToString("HH:mm");

            _reactionsStackView.UpdateReactions(message.Reactions, chatView);

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

            SetNeedsLayout();
            LayoutIfNeeded();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in {nameof(OwnVideoMessageCell)}.{nameof(Update)}: {ex.Message}");
        }
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
