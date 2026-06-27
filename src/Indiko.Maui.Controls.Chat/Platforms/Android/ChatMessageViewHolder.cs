using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Indiko.Maui.Controls.Chat.Models;
using Microsoft.Maui.Handlers;
using aViews = Android.Views;
using ImageButton = Android.Widget.ImageButton;

namespace Indiko.Maui.Controls.Chat.Platforms.Android;

public class ChatMessageViewHolder : RecyclerView.ViewHolder, IDisposable
{
    public TextView DateTextView { get; }
    public TextView TextView { get; }
    public ImageView ImageView { get; }
    public VideoView VideoView { get; }
    public FrameLayout VideoContainer { get; }
    public TextView TimestampTextView { get; }
    public FrameLayout FrameLayout { get; }
    public TextView NewMessagesSeparatorTextView { get; }
    public ImageView AvatarView { get; }
    public LinearLayout ReactionContainer { get; }
    public ImageView DeliveryStatusIcon { get; }
    public LinearLayout ReplySummaryFrame { get; }
    public TextView ReplySenderTextView { get; }
    public TextView ReplyPreviewTextView { get; }
    public TextView SystemMessageTextView { get; }

    public LinearLayout AudioContainer { get; }
    public ImageButton AudioPlayButton { get; }
    public WaveformView AudioWaveform { get; }
    public TextView AudioDurationTextView { get; }
    public VoiceNotePlayer VoicePlayer { get; }

    public ImageView VideoPosterView { get; }
    public ImageButton VideoPlayButton { get; }
    public TextView CaptionTextView { get; }
    public TextView SenderNameTextView { get; }
    private EventHandler _videoPlayHandler;

    private global::Android.Graphics.Bitmap _imageBitmap;
    private bool _openImageFullScreen;

    private EventHandler _avatarClickHandler;
    private EventHandler _textBubbleClickHandler;
    private EventHandler _imageBubbleClickHandler;
    private EventHandler _videoBubbleClickHandler;
    private EventHandler _audioBubbleClickHandler;
    private EventHandler _emojiReactionClickHandler;
    private EventHandler _replyClickHandler;

    private EventHandler<aViews.View.LongClickEventArgs> _longPressHandler;


    public ChatMessageViewHolder(
        aViews.View itemView,
        TextView dateTextView,
        TextView textView,
        ImageView imageView,
        FrameLayout videoContainer,
        VideoView videoView,
        TextView timestampTextView,
        FrameLayout frameLayout,
        TextView newMessagesSeparatorTextView,
        ImageView avatarView,
        LinearLayout reactionContainer,
        ImageView deliveryStatusIcon,
        LinearLayout replySummaryFrame,
        TextView replyPreviewTextView,
        TextView replySenderTextView,
        TextView systemMessageTextView,
        LinearLayout audioContainer,
        ImageButton audioPlayButton,
        WaveformView audioWaveform,
        TextView audioDurationTextView,
        ImageView videoPosterView,
        ImageButton videoPlayButton,
        TextView captionTextView,
        TextView senderNameTextView)
        : base(itemView)
    {
        DateTextView = dateTextView;
        TextView = textView;
        ImageView = imageView;
        VideoContainer = videoContainer;
        VideoView = videoView;
        TimestampTextView = timestampTextView;
        FrameLayout = frameLayout;
        NewMessagesSeparatorTextView = newMessagesSeparatorTextView;
        AvatarView = avatarView;
        ReactionContainer = reactionContainer;
        DeliveryStatusIcon = deliveryStatusIcon;

        ReplySummaryFrame = replySummaryFrame;
        ReplySenderTextView = replySenderTextView;
        ReplyPreviewTextView = replyPreviewTextView;
        SystemMessageTextView = systemMessageTextView;

        AudioContainer = audioContainer;
        AudioPlayButton = audioPlayButton;
        AudioWaveform = audioWaveform;
        AudioDurationTextView = audioDurationTextView;
        VoicePlayer = new VoiceNotePlayer(audioPlayButton, audioWaveform, audioDurationTextView);

        VideoPosterView = videoPosterView;
        VideoPlayButton = videoPlayButton;
        CaptionTextView = captionTextView;
        SenderNameTextView = senderNameTextView;
    }

    /// <summary>
    /// Shows the blurred poster + play button over the (stopped) VideoView and wires the
    /// tap that starts inline playback. Called per bind; no auto-play.
    /// </summary>
    public void SetupVideoPoster(global::Android.Graphics.Bitmap poster, bool openFullScreen, string filePath)
    {
        try { VideoView.StopPlayback(); } catch { /* not playing */ }
        VideoView.Visibility = aViews.ViewStates.Gone;

        VideoPosterView.Visibility = aViews.ViewStates.Visible;
        VideoPlayButton.Visibility = aViews.ViewStates.Visible;
        VideoPosterView.SetImageBitmap(poster);

        if (OperatingSystem.IsAndroidVersionAtLeast(31))
        {
            VideoPosterView.SetRenderEffect(
                global::Android.Graphics.RenderEffect.CreateBlurEffect(25f, 25f, global::Android.Graphics.Shader.TileMode.Clamp));
        }

        if (_videoPlayHandler != null)
            VideoPlayButton.Click -= _videoPlayHandler;
        _videoPlayHandler = openFullScreen
            ? (s, e) => OpenFullScreenVideo(filePath)
            : (s, e) => StartVideoPlayback();
        VideoPlayButton.Click += _videoPlayHandler;
    }

    /// <summary>Stores the data used to open the full-screen image viewer on image tap.</summary>
    public void SetImageViewerData(global::Android.Graphics.Bitmap bitmap, bool openFullScreen)
    {
        _imageBitmap = bitmap;
        _openImageFullScreen = openFullScreen;
    }

    // Opens a full-screen image viewer with pinch-to-zoom / pan / double-tap.
    private void OpenFullScreenImage(global::Android.Graphics.Bitmap bitmap)
    {
        var context = ItemView.Context;
        var dialog = new global::Android.App.Dialog(context, global::Android.Resource.Style.ThemeBlackNoTitleBarFullScreen);

        var frame = new FrameLayout(context);
        frame.SetBackgroundColor(global::Android.Graphics.Color.Black);

        var zoomImage = new ZoomableImageView(context);
        zoomImage.SetImageBitmap(bitmap);
        frame.AddView(zoomImage, new FrameLayout.LayoutParams(
            aViews.ViewGroup.LayoutParams.MatchParent,
            aViews.ViewGroup.LayoutParams.MatchParent));

        // Close button.
        var closeButton = new ImageButton(context);
        closeButton.SetImageResource(global::Android.Resource.Drawable.IcMenuCloseClearCancel);
        closeButton.SetColorFilter(global::Android.Graphics.Color.White);
        closeButton.SetBackgroundColor(global::Android.Graphics.Color.Argb(90, 0, 0, 0));
        var closeSize = PixelExtensions.DpToPx(40, context);
        var closeParams = new FrameLayout.LayoutParams(closeSize, closeSize)
        {
            Gravity = aViews.GravityFlags.Top | aViews.GravityFlags.Left,
            LeftMargin = PixelExtensions.DpToPx(12, context),
            TopMargin = PixelExtensions.DpToPx(12, context)
        };
        closeButton.Click += (s, e) => dialog.Dismiss();
        frame.AddView(closeButton, closeParams);

        dialog.SetContentView(frame);
        dialog.Show();
    }

    // Plays the video full screen in a dialog with native media controls (play/pause + seek).
    private void OpenFullScreenVideo(string filePath)
    {
        var context = ItemView.Context;
        var dialog = new global::Android.App.Dialog(context, global::Android.Resource.Style.ThemeBlackNoTitleBarFullScreen);

        var frame = new FrameLayout(context);
        frame.SetBackgroundColor(global::Android.Graphics.Color.Black);

        var videoView = new VideoView(context);
        frame.AddView(videoView, new FrameLayout.LayoutParams(
            aViews.ViewGroup.LayoutParams.MatchParent,
            aViews.ViewGroup.LayoutParams.MatchParent)
        {
            Gravity = aViews.GravityFlags.Center
        });

        var mediaController = new MediaController(context);
        mediaController.SetAnchorView(videoView);
        videoView.SetMediaController(mediaController);
        videoView.SetVideoPath(filePath);

        dialog.SetContentView(frame);
        dialog.Show();
        videoView.RequestFocus();
        videoView.Start();

        dialog.DismissEvent += (s, e) => { try { videoView.StopPlayback(); } catch { /* already stopped */ } };
    }

    private void StartVideoPlayback()
    {
        if (OperatingSystem.IsAndroidVersionAtLeast(31))
            VideoPosterView.SetRenderEffect(null);

        VideoPosterView.Visibility = aViews.ViewStates.Gone;
        VideoPlayButton.Visibility = aViews.ViewStates.Gone;
        VideoView.Visibility = aViews.ViewStates.Visible;
        VideoView.RequestFocus();
        VideoView.Start();
    }

    public void AttachEventHandlers(ChatMessage message, ChatView chatView, ChatViewHandler handler)
    {
        DetachEventHandlers();

        WeakReference<ChatView> weakChatView = new(chatView);

        _avatarClickHandler = (s, e) =>
        {
            if (weakChatView.TryGetTarget(out var target))
            {
                target.AvatarTappedCommand?.Execute(message);
                ApplyVisualFeedbackToAvatar();
            }
        };
        AvatarView.Click += _avatarClickHandler;


        // Long Press Handler
        _longPressHandler = (object sender, aViews.View.LongClickEventArgs e) =>
        {
            if (weakChatView.TryGetTarget(out var target))
            {
                handler.ShowContextMenu(message, FrameLayout); // Show menu on long press
            }
        };

        TextView.LongClick += _longPressHandler;
        ImageView.LongClick += _longPressHandler;
        VideoContainer.LongClick += _longPressHandler;
        VideoPosterView.LongClick += _longPressHandler;   // poster overlays the container
        AudioContainer.LongClick += _longPressHandler;
        ReactionContainer.LongClick += _longPressHandler;

        _textBubbleClickHandler = (s, e) =>
        {
            if (weakChatView.TryGetTarget(out var target))
            {
                target.MessageTappedCommand?.Execute(message);
                ApplyVisualFeedbackToChatBubble();
            }
        };
        TextView.Click += _textBubbleClickHandler;

        _imageBubbleClickHandler = (s, e) =>
        {
            if (weakChatView.TryGetTarget(out var target))
            {
                target.MessageTappedCommand?.Execute(message);
                ApplyVisualFeedbackToChatBubble();
            }

            if (_openImageFullScreen && _imageBitmap != null)
            {
                OpenFullScreenImage(_imageBitmap);
            }
        };
        ImageView.Click += _imageBubbleClickHandler;

        _videoBubbleClickHandler = (s, e) =>
        {
            if (weakChatView.TryGetTarget(out var target))
            {
                target.MessageTappedCommand?.Execute(message);
                ApplyVisualFeedbackToChatBubble();
            }
        };
        VideoContainer.Click += _videoBubbleClickHandler;

        _audioBubbleClickHandler = (s, e) =>
        {
            if (weakChatView.TryGetTarget(out var target))
            {
                target.MessageTappedCommand?.Execute(message);
                ApplyVisualFeedbackToChatBubble();
            }
        };
        AudioContainer.Click += _audioBubbleClickHandler;

        _emojiReactionClickHandler = (s, e) =>
        {
            if (weakChatView.TryGetTarget(out var target))
            {
                target.MessageTappedCommand?.Execute(message);
                ApplyVisualFeedbackToEmojiReaction();
            }
        };
        ReactionContainer.Click += _emojiReactionClickHandler;

        // Tap the reply preview to jump to the original message.
        _replyClickHandler = (s, e) =>
        {
            if (weakChatView.TryGetTarget(out var target) && message?.ReplyToMessage != null)
                target.NotifyRepliedMessageTapped(message.ReplyToMessage.MessageId);
        };
        ReplySummaryFrame.Click += _replyClickHandler;
    }

   
    public void DetachEventHandlers()
    {
        if (_avatarClickHandler != null && AvatarView != null)
        {
            AvatarView.Click -= _avatarClickHandler;
            _avatarClickHandler = null;
        }

        if (_longPressHandler != null)
        {
            TextView.LongClick -= _longPressHandler;
            ImageView.LongClick -= _longPressHandler;
            VideoContainer.LongClick -= _longPressHandler;
            VideoPosterView.LongClick -= _longPressHandler;
            AudioContainer.LongClick -= _longPressHandler;
            ReactionContainer.LongClick -= _longPressHandler;
            _longPressHandler = null;
        }

        if (_textBubbleClickHandler != null && TextView != null)
        {
            TextView.Click -= _textBubbleClickHandler;
            _textBubbleClickHandler = null;
        }

        if (_imageBubbleClickHandler != null && ImageView != null)
        {
            ImageView.Click -= _imageBubbleClickHandler;
            _imageBubbleClickHandler = null;
        }

        if (_videoBubbleClickHandler != null && VideoContainer != null)
        {
            VideoContainer.Click -= _videoBubbleClickHandler;
            _videoBubbleClickHandler = null;
        }

        if (_audioBubbleClickHandler != null && AudioContainer != null)
        {
            AudioContainer.Click -= _audioBubbleClickHandler;
            _audioBubbleClickHandler = null;
        }

        // Stop any in-progress playback when the row is rebound/recycled.
        VoicePlayer?.Stop();

        if (_videoPlayHandler != null && VideoPlayButton != null)
        {
            VideoPlayButton.Click -= _videoPlayHandler;
            _videoPlayHandler = null;
        }
        try { VideoView?.StopPlayback(); } catch { /* not playing */ }

        if (_replyClickHandler != null && ReplySummaryFrame != null)
        {
            ReplySummaryFrame.Click -= _replyClickHandler;
            _replyClickHandler = null;
        }

        if (_emojiReactionClickHandler != null && ReactionContainer != null)
        {
            ReactionContainer.Click -= _emojiReactionClickHandler;
            _emojiReactionClickHandler = null;
        }
    }

    public async void ApplyVisualFeedbackToChatBubble()
    {
        if (FrameLayout != null)
        {
            FrameLayout.Alpha = 0.7f;
            await Task.Delay(100);
            FrameLayout.Alpha = 1f;
        }
    }

    public async void ApplyVisualFeedbackToEmojiReaction()
    {
        if (ReactionContainer != null)
        {
            ReactionContainer.Alpha = 0.7f;
            await Task.Delay(100);
            ReactionContainer.Alpha = 1f;
        }
    }
    
    public async void ApplyVisualFeedbackToAvatar()
    {
        if (AvatarView != null)
        {
            AvatarView.Alpha = 0.7f;
            await Task.Delay(100);
            AvatarView.Alpha = 1f;
        }
    }


    public new void Dispose()
    {
        DetachEventHandlers();
        base.Dispose();
    }
}