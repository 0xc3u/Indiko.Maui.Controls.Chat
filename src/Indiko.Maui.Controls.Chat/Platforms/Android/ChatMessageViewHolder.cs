using aViews = Android.Views;
using AndroidX.RecyclerView.Widget;
using Android.Widget;
using Indiko.Maui.Controls.Chat.Models;

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

    private EventHandler _avatarClickHandler;
    private EventHandler _textBubbleClickHandler;
    private EventHandler _imageBubbleClickHandler;
    private EventHandler _videoBubbleClickHandler;
    private EventHandler _emojiReactionClickHandler;

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
        ImageView deliveryStatusIcon)
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
    }

    public void AttachEventHandlers(ChatMessage message, ChatView chatView)
    {
        DetachEventHandlers();

        WeakReference<ChatView> weakChatView = new(chatView);

        _avatarClickHandler = (s, e) =>
        {
            if (weakChatView.TryGetTarget(out var target))
            {
                target.AvatarTappedCommand?.Execute(message);
            }
        };
        AvatarView.Click += _avatarClickHandler;

        _textBubbleClickHandler = (s, e) =>
        {
            if (weakChatView.TryGetTarget(out var target))
            {
                target.MessageTappedCommand?.Execute(message);
            }
        };
        TextView.Click += _textBubbleClickHandler;

        _imageBubbleClickHandler = (s, e) =>
        {
            if (weakChatView.TryGetTarget(out var target))
            {
                target.MessageTappedCommand?.Execute(message);
            }
        };
        ImageView.Click += _imageBubbleClickHandler;

        _videoBubbleClickHandler = (s, e) =>
        {
            if (weakChatView.TryGetTarget(out var target))
            {
                target.MessageTappedCommand?.Execute(message);
            }
        };
        VideoContainer.Click += _videoBubbleClickHandler;

        _emojiReactionClickHandler = (s, e) =>
        {
            if (weakChatView.TryGetTarget(out var target))
            {
                target.MessageTappedCommand?.Execute(message);
            }
        };
        ReactionContainer.Click += _emojiReactionClickHandler;
    }

    public void DetachEventHandlers()
    {
        if (_avatarClickHandler != null && AvatarView != null)
        {
            AvatarView.Click -= _avatarClickHandler;
            _avatarClickHandler = null;
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

        if (_emojiReactionClickHandler != null && ReactionContainer != null)
        {
            ReactionContainer.Click -= _emojiReactionClickHandler;
            _emojiReactionClickHandler = null;
        }
    }

    public new void Dispose()
    {
        DetachEventHandlers();
        base.Dispose();
    }
}