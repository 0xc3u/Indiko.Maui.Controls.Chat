using aViews = Android.Views;
using AndroidX.RecyclerView.Widget;
using Android.Widget;

namespace Indiko.Maui.Controls.Chat.Platforms.Android;

public class ChatMessageViewHolder : RecyclerView.ViewHolder
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
    public LinearLayout ReactionContainer { get; } // container for reactions
    public ImageView DeliveryStatusIcon { get; } // icon for delivery status


    public ChatMessageViewHolder(aViews.View itemView, TextView dateTextView, TextView textView, ImageView imageView, FrameLayout videoContainer, 
        VideoView videoView, TextView timestampTextView, FrameLayout frameLayout, TextView newMessagesSeparatorTextView, ImageView avatarView, 
        LinearLayout reactionContainer, ImageView deliveryStatusIcon)
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
}