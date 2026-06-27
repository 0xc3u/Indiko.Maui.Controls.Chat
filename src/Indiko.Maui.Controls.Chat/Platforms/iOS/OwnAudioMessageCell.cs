using AVFoundation;
using CoreGraphics;
using Foundation;
using Indiko.Maui.Controls.Chat.Models;
using Microsoft.Maui.Platform;
using UIKit;

namespace Indiko.Maui.Controls.Chat.Platforms.iOS;

internal class OwnAudioMessageCell : UICollectionViewCell
{
    public static readonly NSString Key = new(nameof(OwnAudioMessageCell));
    private ChatView _chatView;
    private ChatMessage _message;

    private UIButton _playButton;
    private WaveformView _waveform;
    private UILabel _audioDurationLabel;
    private VoiceNotePlayer _voicePlayer;

    private UIView _bubbleView;
    private UILabel _timeLabel;
    private UIStackView _reactionsStackView;
    private UIImageView _deliveryStateImageView;

    private UIView _replyView;
    private UILabel _replyPreviewTextLabel;
    private UILabel _replySenderTextLabel;

    private NSLayoutConstraint _messageAudioTopConstraint;
    private UILongPressGestureRecognizer _longPressGesture;

    public OwnAudioMessageCell(ObjCRuntime.NativeHandle handle) : base(handle)
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
        _voicePlayer?.Stop();
    }

    private void SetupLayout()
    {
        // Chat bubble setup
        _bubbleView = new UIView
        {
            TranslatesAutoresizingMaskIntoConstraints = false,
            BackgroundColor = UIColor.FromRGBA(113 / 255.0f, 0 / 255.0f, 223 / 255.0f, 1.0f),
            ClipsToBounds = true
        };
        _bubbleView.Layer.CornerRadius = 16;

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

        // Voice-note controls: circular play/pause, seekable waveform, duration label.
        _playButton = new UIButton(UIButtonType.System)
        {
            TranslatesAutoresizingMaskIntoConstraints = false,
            ClipsToBounds = true
        };
        _playButton.Layer.CornerRadius = 20;

        _waveform = new WaveformView
        {
            TranslatesAutoresizingMaskIntoConstraints = false
        };

        _audioDurationLabel = new UILabel
        {
            Font = UIFont.SystemFontOfSize(11),
            TranslatesAutoresizingMaskIntoConstraints = false,
            TextAlignment = UITextAlignment.Left
        };

        _voicePlayer = new VoiceNotePlayer(_playButton, _waveform, _audioDurationLabel);

        // Message timestamp
        _timeLabel = new UILabel
        {
            Font = UIFont.SystemFontOfSize(12),
            TextColor = UIColor.LightGray,
            TranslatesAutoresizingMaskIntoConstraints = false,
            TextAlignment = UITextAlignment.Right
        };

        _timeLabel.SetContentHuggingPriority((float)UILayoutPriority.DefaultLow, UILayoutConstraintAxis.Horizontal);

        // Message reaction stack (horizontal Emoji-List)
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

        // add child views into hierarchical order
        ContentView.AddSubviews(_bubbleView, _playButton, _waveform, _audioDurationLabel, _replyView, _replySenderTextLabel, _replyPreviewTextLabel, _timeLabel, _deliveryStateImageView, _reactionsStackView);

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

            // Play/pause button inside chat bubble
            _messageAudioTopConstraint = _playButton.TopAnchor.ConstraintEqualTo(_bubbleView.TopAnchor, 10),
            _playButton.LeadingAnchor.ConstraintEqualTo(_bubbleView.LeadingAnchor, 10),
            _playButton.WidthAnchor.ConstraintEqualTo(40),
            _playButton.HeightAnchor.ConstraintEqualTo(40),

            // Waveform fills the rest of the row, vertically centred on the button
            _waveform.LeadingAnchor.ConstraintEqualTo(_playButton.TrailingAnchor, 10),
            _waveform.TrailingAnchor.ConstraintEqualTo(_bubbleView.TrailingAnchor, -10),
            _waveform.CenterYAnchor.ConstraintEqualTo(_playButton.CenterYAnchor),
            _waveform.HeightAnchor.ConstraintEqualTo(32),
            _waveform.WidthAnchor.ConstraintGreaterThanOrEqualTo(150),

            // Duration under the waveform
            _audioDurationLabel.TopAnchor.ConstraintEqualTo(_playButton.BottomAnchor, 4),
            _audioDurationLabel.LeadingAnchor.ConstraintEqualTo(_playButton.TrailingAnchor, 10),
            _audioDurationLabel.BottomAnchor.ConstraintEqualTo(_bubbleView.BottomAnchor, -10),

            // Message Emoji-reactions
            _reactionsStackView.TopAnchor.ConstraintEqualTo(_bubbleView.BottomAnchor, 4),
            _reactionsStackView.LeadingAnchor.ConstraintEqualTo(_bubbleView.LeadingAnchor),
            _reactionsStackView.TrailingAnchor.ConstraintLessThanOrEqualTo(_bubbleView.TrailingAnchor),
            _reactionsStackView.WidthAnchor.ConstraintLessThanOrEqualTo(_bubbleView.WidthAnchor, 0.5f),

            // Message time stamp
            _timeLabel.TopAnchor.ConstraintEqualTo(_reactionsStackView.TopAnchor, 4),
            _timeLabel.TrailingAnchor.ConstraintEqualTo(_deliveryStateImageView.LeadingAnchor, -4),
            _timeLabel.LeadingAnchor.ConstraintEqualTo(_bubbleView.LeadingAnchor),
            _timeLabel.BottomAnchor.ConstraintEqualTo(ContentView.BottomAnchor, -10),

            // Delivery state icon
            _deliveryStateImageView.CenterYAnchor.ConstraintEqualTo(_timeLabel.CenterYAnchor),
            _deliveryStateImageView.TrailingAnchor.ConstraintEqualTo(_bubbleView.TrailingAnchor),
            _deliveryStateImageView.WidthAnchor.ConstraintEqualTo(16),
            _deliveryStateImageView.HeightAnchor.ConstraintEqualTo(16)
        });

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

            // No bubble-wide tap command for voice notes: the play button and waveform are
            // interactive, and a parent tap gesture would swallow their touches.
            _bubbleView.UserInteractionEnabled = true;

            _reactionsStackView.AddWeakTapGestureRecognizerWithCommand(_message, _chatView.EmojiReactionTappedCommand);
            _reactionsStackView.UserInteractionEnabled = true;

            _bubbleView.BackgroundColor = chatView.OwnMessageBackgroundColor.ToPlatform();

            // Voice-note: theme colours derived from the bubble's text colour.
            var foreground = chatView.OwnMessageTextColor.ToPlatform();
            _playButton.BackgroundColor = foreground.ColorWithAlpha(0.2f);

            string audioFile = null;
            if (message.BinaryContent != null)
            {
                audioFile = Path.Combine(FileSystem.Current.CacheDirectory, $"{message.MessageId}.audio");
                if (!File.Exists(audioFile))
                {
                    File.WriteAllBytes(audioFile, message.BinaryContent);
                }
            }

            var samples = message.AudioWaveform ?? AudioWaveformHelper.GenerateFallback(message.MessageId);
            _voicePlayer.Configure(
                audioFile,
                message.AudioDuration,
                samples,
                barColor: foreground.ColorWithAlpha(0.4f),
                progressColor: foreground,
                iconColor: foreground,
                textColor: foreground.ColorWithAlpha(0.85f));

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

                _messageAudioTopConstraint.Active = false;
                _messageAudioTopConstraint = _playButton.TopAnchor.ConstraintEqualTo(_replyView.BottomAnchor, 10);
                _messageAudioTopConstraint.Active = true;
            }
            else
            {
                _replyView.Hidden = true;
                _replySenderTextLabel.Hidden = true;
                _replyPreviewTextLabel.Hidden = true;

                _messageAudioTopConstraint.Active = false;
                _messageAudioTopConstraint = _playButton.TopAnchor.ConstraintEqualTo(_bubbleView.TopAnchor, 10);
                _messageAudioTopConstraint.Active = true;
            }

            _timeLabel.Font = UIFont.SystemFontOfSize((nfloat)chatView.MessageTimeFontSize);
            _timeLabel.TextColor = chatView.MessageTimeTextColor.ToPlatform();
            _timeLabel.Text = message.Timestamp.ToString("HH:mm");

            _reactionsStackView.UpdateReactions(message.Reactions, chatView);

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
            Console.WriteLine($"Error in {nameof(OwnAudioMessageCell)}.{nameof(Update)}: {ex.Message}");
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
