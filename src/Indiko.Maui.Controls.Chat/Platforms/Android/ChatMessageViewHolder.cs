﻿using aViews = Android.Views;
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

    public ImageView AvatarView { get; } // New avatar view

    public ChatMessageViewHolder(aViews.View itemView, TextView dateTextView, TextView textView, ImageView imageView, FrameLayout videoContainer, VideoView videoView, TextView timestampTextView, FrameLayout frameLayout, TextView newMessagesSeparatorTextView, ImageView avatarView)
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
    }
}

//public class ChatMessageViewHolder : RecyclerView.ViewHolder
//{
//    public TextView DateTextView { get; }
//    public TextView TextView { get; }
//    public ImageView ImageView { get; }
//    public VideoView VideoView { get; }
//    public FrameLayout VideoContainer { get; }
//    public TextView TimestampTextView { get; }
//    public FrameLayout FrameLayout { get; }
//    public TextView NewMessagesSeparatorTextView { get; }
//    public aViews.View LeftLine { get; }
//    public aViews.View RightLine { get; }

//    public ChatMessageViewHolder(aViews.View itemView, TextView dateTextView, TextView textView, ImageView imageView, FrameLayout videoContainer, VideoView videoView, TextView timestampTextView, FrameLayout frameLayout, TextView newMessagesSeparatorTextView, aViews.View leftLine, aViews.View rightLine)
//        : base(itemView)
//    {
//        DateTextView = dateTextView;
//        TextView = textView;
//        ImageView = imageView;
//        VideoContainer = videoContainer;  // Initialize the video container
//        VideoView = videoView;  // Initialize VideoView
//        TimestampTextView = timestampTextView;
//        FrameLayout = frameLayout;
//        NewMessagesSeparatorTextView = newMessagesSeparatorTextView;
//        LeftLine = leftLine;
//        RightLine = rightLine;
//    }
//}
