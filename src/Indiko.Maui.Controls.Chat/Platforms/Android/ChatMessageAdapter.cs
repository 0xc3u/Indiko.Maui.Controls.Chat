using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.Content;
using aViews = Android.Views;
using Android.Widget;
using AndroidX.ConstraintLayout.Widget;
using AndroidX.RecyclerView.Widget;
using Indiko.Maui.Controls.Chat.Models;
using Android.Views;
using Android.Util;
using Microsoft.Maui.Platform;
using Android.Graphics.Drawables;
using Microsoft.Maui.Controls.Platform;
using aIO = Java.IO;
using ANet = Android.Net;
using Color = Microsoft.Maui.Graphics.Color;
using Paint = Android.Graphics.Paint;
using Rect = Android.Graphics.Rect;
using RectF = Android.Graphics.RectF;
using Android.Graphics;

namespace Indiko.Maui.Controls.Chat.Platforms.Android;
public class ChatMessageAdapter : RecyclerView.Adapter
{
    private readonly Context _context;
    private readonly IList<ChatMessage> _messages;

    public Color OwnMessageBackgroundColor { get; }
    public Color OtherMessageBackgroundColor { get; }
    public Color OwnMessageTextColor { get; }
    public Color OtherMessageTextColor { get; }

    public float MessageFontSize { get; }

    public Color MessageTimeTextColor { get; }
    public float MessageTimeFontSize { get; }

    public Color DateTextColor { get; }
    public float DateTextFontSize { get; }

    public Color NewMessagesSeperatorTextColor { get; }
    public float NewMessagesSeperatorFontSize { get; }
    public string NewMessagesSeperatorText { get; }
    public bool ShowNewMessagesSeperator { get; }

    public float AvatarSize { get; }
    public Color AvatarBackgroundColor { get; }
    public Color AvatarTextColor { get; }
    public bool ScrollToFirstNewMessage { get; }
    public Color EmojiReactionTextColor { get; }
    public float EmojiReactionFontSize { get; }

    public ImageSource SendIcon { get; }
    public ImageSource DeliveredIcon { get; }
    public ImageSource ReadIcon { get; }

    public Color ReplyMessageBackgroundColor { get; }
    public Color ReplyMessageTextColor { get; }
    public float ReplyMessageFontSize { get; }


    public Color SystemMessageBackgroundColor { get; }
    public Color SystemMessageTextColor { get; }
    public float SystemMessageFontSize { get; }


    private readonly IMauiContext _mauiContext;
    private readonly ChatView VirtualView; // Add reference to ChatView

    public ChatMessageAdapter(Context context, IMauiContext mauiContext, ChatView virtualView)
    {
        _context = context;
        _messages = virtualView.Messages;
        _mauiContext = mauiContext;
        VirtualView = virtualView;

        OwnMessageBackgroundColor = VirtualView.OwnMessageBackgroundColor;
        OtherMessageBackgroundColor = VirtualView.OtherMessageBackgroundColor;
        OwnMessageTextColor = VirtualView.OwnMessageTextColor;
        OtherMessageTextColor = VirtualView.OtherMessageTextColor;
        DateTextColor = VirtualView.DateTextColor;
        MessageTimeTextColor = VirtualView.MessageTimeTextColor;
        DateTextFontSize = VirtualView.DateTextFontSize;
        MessageTimeFontSize = VirtualView.MessageTimeFontSize;
        MessageFontSize = VirtualView.MessageFontSize;
        NewMessagesSeperatorFontSize = VirtualView.NewMessagesSeperatorFontSize;
        NewMessagesSeperatorTextColor = VirtualView.NewMessagesSeperatorTextColor;
        NewMessagesSeperatorText = VirtualView.NewMessagesSeperatorText;
        ScrollToFirstNewMessage = VirtualView.ScrollToFirstNewMessage;
        AvatarSize = VirtualView.AvatarSize;
        AvatarBackgroundColor = VirtualView.AvatarBackgroundColor;
        AvatarTextColor = VirtualView.AvatarTextColor;
        EmojiReactionTextColor = VirtualView.EmojiReactionTextColor;
        EmojiReactionFontSize = VirtualView.EmojiReactionFontSize;
        SendIcon = VirtualView.SendIcon;
        DeliveredIcon = VirtualView.DeliveredIcon;
        ReadIcon = VirtualView.ReadIcon;
        ReplyMessageBackgroundColor = VirtualView.ReplyMessageBackgroundColor;
        ReplyMessageTextColor = VirtualView.ReplyMessageTextColor;
        ReplyMessageFontSize = VirtualView.ReplyMessageFontSize;
        ShowNewMessagesSeperator = VirtualView.ShowNewMessagesSeperator;

        SystemMessageBackgroundColor = VirtualView.SystemMessageBackgroundColor;
        SystemMessageTextColor = VirtualView.SystemMessageTextColor;
        SystemMessageFontSize = VirtualView.SystemMessageFontSize;
    }
    public override int ItemCount => _messages.Count;

    public override int GetItemViewType(int position)
    {
        return (int)_messages[position].MessageType;
    }

    public override RecyclerView.ViewHolder OnCreateViewHolder(aViews.ViewGroup parent, int viewType)
    {
        aViews.View itemView;
        int messageTextViewId, timeTextViewId, messageImageViewId, messageVideoViewId, systemMessageTextViewId, dateTextViewId, avatarViewId;
        bool isOwnMessage = false; // Default value, will be set in OnBindViewHolder

        switch (viewType)
        {
            case (int)MessageType.Text:
                itemView = CreateTextMessageView(parent, out messageTextViewId, out timeTextViewId, out avatarViewId, isOwnMessage);
                return new TextMessageViewHolder(itemView, messageTextViewId, timeTextViewId, avatarViewId);
            case (int)MessageType.Image:
                itemView = CreateImageMessageView(parent, out messageImageViewId, out timeTextViewId, out avatarViewId, isOwnMessage);
                return new ImageMessageViewHolder(itemView, messageImageViewId, timeTextViewId, avatarViewId);
            case (int)MessageType.Video:
                itemView = CreateVideoMessageView(parent, out messageVideoViewId, out timeTextViewId, out avatarViewId, isOwnMessage);
                return new VideoMessageViewHolder(itemView, messageVideoViewId, timeTextViewId, avatarViewId);
            case (int)MessageType.System:
                itemView = CreateSystemMessageView(parent, out systemMessageTextViewId);
                return new SystemMessageViewHolder(itemView, systemMessageTextViewId);
            case (int)MessageType.Date:
                itemView = CreateDateMessageView(parent, out dateTextViewId);
                return new DateMessageViewHolder(itemView, dateTextViewId);
            default:
                throw new NotSupportedException($"Message type {viewType} is not supported");
        }
    }


    private aViews.View CreateTextMessageView(ViewGroup parent, out int messageTextViewId, out int timeTextViewId, out int avatarViewId, bool isOwnMessage)
    {
        var constraintLayout = new ConstraintLayout(parent.Context)
        {
            LayoutParameters = new ViewGroup.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent)
        };

        var avatarView = new ImageView(_context)
        {
            Id = aViews.View.GenerateViewId(),
            LayoutParameters = new ConstraintLayout.LayoutParams(96, 96) // Fixed size
        };
        avatarView.SetScaleType(ImageView.ScaleType.CenterCrop); // Center and crop image
        
        // Add a circular shape drawable
        var avatarBackground = new GradientDrawable();
        avatarBackground.SetShape(ShapeType.Oval);
        avatarBackground.SetColor(AvatarBackgroundColor.ToPlatform());
        avatarView.Background = avatarBackground;


        var messageTextView = new TextView(parent.Context)
        {
            LayoutParameters = new ViewGroup.LayoutParams(
                ViewGroup.LayoutParams.WrapContent,
                ViewGroup.LayoutParams.WrapContent),
            Id = aViews.View.GenerateViewId()
        };

        var timeTextView = new TextView(parent.Context)
        {
            LayoutParameters = new ViewGroup.LayoutParams(
                ViewGroup.LayoutParams.WrapContent,
                ViewGroup.LayoutParams.WrapContent),
            Id = aViews.View.GenerateViewId()
        };

        messageTextViewId = messageTextView.Id;
        timeTextViewId = timeTextView.Id;
        avatarViewId = avatarView.Id;

        messageTextView.SetPadding(16, 16, 16, 16);
        timeTextView.SetPadding(16, 8, 16, 8);

        constraintLayout.AddView(avatarView);
        constraintLayout.AddView(messageTextView);
        constraintLayout.AddView(timeTextView);

        var constraintSet = new ConstraintSet();
        constraintSet.Clone(constraintLayout);
              

        // Set constraints for messageTextView
        constraintSet.Connect(avatarView.Id, ConstraintSet.Top, ConstraintSet.ParentId, ConstraintSet.Top, 0);
        constraintSet.Connect(avatarView.Id, ConstraintSet.Start, ConstraintSet.ParentId, ConstraintSet.Start, 32);


        constraintSet.Connect(messageTextViewId, ConstraintSet.Top, ConstraintSet.ParentId, ConstraintSet.Top);
        constraintSet.Connect(messageTextViewId, isOwnMessage ? ConstraintSet.End : ConstraintSet.Start, ConstraintSet.ParentId, isOwnMessage ? ConstraintSet.End : ConstraintSet.Start);
        constraintSet.ConstrainPercentWidth(messageTextViewId, 0.65f);

        // Set constraints for timeTextView
        constraintSet.Connect(timeTextViewId, ConstraintSet.Top, messageTextViewId, ConstraintSet.Bottom, 8);
        constraintSet.Connect(timeTextViewId, isOwnMessage ? ConstraintSet.End : ConstraintSet.Start, ConstraintSet.ParentId, isOwnMessage ? ConstraintSet.End : ConstraintSet.Start);

        constraintSet.ApplyTo(constraintLayout);

        return constraintLayout;
    }

    private aViews.View CreateImageMessageView(ViewGroup parent, out int messageImageViewId, out int timeTextViewId, out int avatarViewId, bool isOwnMessage)
    {
        var constraintLayout = new ConstraintLayout(parent.Context)
        {
            LayoutParameters = new ViewGroup.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent)
        };

        var messageImageView = new ImageView(parent.Context)
        {
            LayoutParameters = new ViewGroup.LayoutParams(
                ViewGroup.LayoutParams.WrapContent,
                ViewGroup.LayoutParams.WrapContent)
        };

        var timeTextView = new TextView(parent.Context)
        {
            LayoutParameters = new ViewGroup.LayoutParams(
                ViewGroup.LayoutParams.WrapContent,
                ViewGroup.LayoutParams.WrapContent)
        };

        messageImageViewId = aViews.View.GenerateViewId();
        timeTextViewId = aViews.View.GenerateViewId();
        avatarViewId = aViews.View.GenerateViewId();

        messageImageView.Id = messageImageViewId;
        timeTextView.Id = timeTextViewId;

        messageImageView.SetPadding(10, 10, 10, 10);
        timeTextView.SetPadding(10, 10, 10, 10);

        constraintLayout.AddView(messageImageView);
        constraintLayout.AddView(timeTextView);

        var constraintSet = new ConstraintSet();
        constraintSet.Clone(constraintLayout);

        // Set constraints for messageImageView
        constraintSet.Connect(messageImageViewId, ConstraintSet.Top, ConstraintSet.ParentId, ConstraintSet.Top);
        constraintSet.Connect(messageImageViewId, isOwnMessage ? ConstraintSet.End : ConstraintSet.Start, ConstraintSet.ParentId, isOwnMessage ? ConstraintSet.End : ConstraintSet.Start);
        constraintSet.ConstrainPercentWidth(messageImageViewId, 0.65f);

        // Set constraints for timeTextView
        constraintSet.Connect(timeTextViewId, ConstraintSet.Top, messageImageViewId, ConstraintSet.Bottom, 8);
        constraintSet.Connect(timeTextViewId, isOwnMessage ? ConstraintSet.End : ConstraintSet.Start, ConstraintSet.ParentId, isOwnMessage ? ConstraintSet.End : ConstraintSet.Start);

        constraintSet.ApplyTo(constraintLayout);

        return constraintLayout;
    }

    private aViews.View CreateVideoMessageView(ViewGroup parent, out int messageVideoViewId, out int timeTextViewId, out int avatarViewId, bool isOwnMessage)
    {
        var constraintLayout = new ConstraintLayout(parent.Context)
        {
            LayoutParameters = new ViewGroup.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent)
        };

        var messageVideoView = new VideoView(parent.Context)
        {
            LayoutParameters = new ViewGroup.LayoutParams(
                ViewGroup.LayoutParams.WrapContent,
                ViewGroup.LayoutParams.WrapContent)
        };

        var timeTextView = new TextView(parent.Context)
        {
            LayoutParameters = new ViewGroup.LayoutParams(
                ViewGroup.LayoutParams.WrapContent,
                ViewGroup.LayoutParams.WrapContent)
        };

        messageVideoViewId = aViews.View.GenerateViewId();
        timeTextViewId = aViews.View.GenerateViewId();
        avatarViewId = aViews.View.GenerateViewId();

        messageVideoView.Id = messageVideoViewId;
        timeTextView.Id = timeTextViewId;

        messageVideoView.SetPadding(10, 10, 10, 10);
        timeTextView.SetPadding(10, 10, 10, 10);

        constraintLayout.AddView(messageVideoView);
        constraintLayout.AddView(timeTextView);

        var constraintSet = new ConstraintSet();
        constraintSet.Clone(constraintLayout);

        // Set constraints for messageVideoView
        constraintSet.Connect(messageVideoViewId, ConstraintSet.Top, ConstraintSet.ParentId, ConstraintSet.Top);
        constraintSet.Connect(messageVideoViewId, isOwnMessage ? ConstraintSet.End : ConstraintSet.Start, ConstraintSet.ParentId, isOwnMessage ? ConstraintSet.End : ConstraintSet.Start);
        constraintSet.ConstrainPercentWidth(messageVideoViewId, 0.65f);

        // Set constraints for timeTextView
        constraintSet.Connect(timeTextViewId, ConstraintSet.Top, messageVideoViewId, ConstraintSet.Bottom, 8);
        constraintSet.Connect(timeTextViewId, isOwnMessage ? ConstraintSet.End : ConstraintSet.Start, ConstraintSet.ParentId, isOwnMessage ? ConstraintSet.End : ConstraintSet.Start);

        constraintSet.ApplyTo(constraintLayout);

        return constraintLayout;
    }


    private aViews.View CreateSystemMessageView(ViewGroup parent, out int systemMessageTextViewId)
    {
        var layout = new LinearLayout(parent.Context)
        {
            Orientation = Orientation.Vertical,
            LayoutParameters = new ViewGroup.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent)
        };

        var systemMessageTextView = new TextView(parent.Context)
        {
            LayoutParameters = new ViewGroup.LayoutParams(
                ViewGroup.LayoutParams.WrapContent,
                ViewGroup.LayoutParams.WrapContent)
        };

        systemMessageTextViewId = aViews.View.GenerateViewId();
        systemMessageTextView.Id = systemMessageTextViewId;

        layout.AddView(systemMessageTextView);

        return layout;
    }

    private aViews.View CreateDateMessageView(ViewGroup parent, out int dateTextViewId)
    {
        var layout = new LinearLayout(parent.Context)
        {
            Orientation = Orientation.Vertical,
            LayoutParameters = new ViewGroup.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent)
        };

        var dateTextView = new TextView(parent.Context)
        {
            LayoutParameters = new ViewGroup.LayoutParams(
                ViewGroup.LayoutParams.WrapContent,
                ViewGroup.LayoutParams.WrapContent)
        };

        dateTextViewId = aViews.View.GenerateViewId();
        dateTextView.Id = dateTextViewId;

        layout.AddView(dateTextView);

        return layout;
    }

    private Drawable CreateChatBubbleDrawable(Color backgroundColor, float cornerRadius)
    {
        var gradientDrawable = new GradientDrawable();
        gradientDrawable.SetColor(backgroundColor.ToPlatform());
        gradientDrawable.SetCornerRadius(cornerRadius);
        return gradientDrawable;
    }

    public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
    {
        var message = _messages[position];
        bool isOwnMessage = message.IsOwnMessage;
        Color backgroundColor = isOwnMessage ? OwnMessageBackgroundColor : OtherMessageBackgroundColor;
        float cornerRadius = 16; // Adjust the corner radius as needed

        switch (message.MessageType)
        {
            case MessageType.Text:
                if (holder is TextMessageViewHolder textHolder)
                {
                    textHolder.MessageTextView.Text = message.TextContent;
                    textHolder.TimeTextView.Text = message.Timestamp.ToString();
                    textHolder.MessageTextView.SetTextColor(isOwnMessage ? OwnMessageTextColor.ToPlatform() : OtherMessageTextColor.ToPlatform());
                    textHolder.MessageTextView.SetTextSize(ComplexUnitType.Sp, MessageFontSize);
                    textHolder.TimeTextView.SetTextColor(MessageTimeTextColor.ToPlatform());
                    textHolder.TimeTextView.SetTextSize(ComplexUnitType.Sp, MessageTimeFontSize);

                    // Set the background to the chat bubble drawable
                    textHolder.MessageTextView.SetBackground(CreateChatBubbleDrawable(backgroundColor, cornerRadius));

                    // Update constraints based on message ownership
                    var constraintSet = new ConstraintSet();
                    constraintSet.Clone((ConstraintLayout)textHolder.ItemView);
                    constraintSet.Connect(textHolder.MessageTextView.Id, ConstraintSet.Top, ConstraintSet.ParentId, ConstraintSet.Top);
                    constraintSet.Connect(textHolder.MessageTextView.Id, isOwnMessage ? ConstraintSet.End : ConstraintSet.Start, ConstraintSet.ParentId, isOwnMessage ? ConstraintSet.End : ConstraintSet.Start);
                    constraintSet.ConstrainPercentWidth(textHolder.MessageTextView.Id, 0.65f);
                    constraintSet.Connect(textHolder.TimeTextView.Id, ConstraintSet.Top, textHolder.MessageTextView.Id, ConstraintSet.Bottom, 8);
                    constraintSet.Connect(textHolder.TimeTextView.Id, isOwnMessage ? ConstraintSet.End : ConstraintSet.Start, ConstraintSet.ParentId, isOwnMessage ? ConstraintSet.End : ConstraintSet.Start);
                    constraintSet.ApplyTo((ConstraintLayout)textHolder.ItemView);

                    // Set the avatar image
                    if (!message.IsOwnMessage && message.SenderAvatar != null)
                    {
                        textHolder.AvatarView.Visibility = ViewStates.Visible;
                        // Load avatar as bitmap
                        var originalBitmap = BitmapFactory.DecodeByteArray(message.SenderAvatar, 0, message.SenderAvatar.Length);

                        // Crop the bitmap into a circular shape
                        var circularBitmap = CreateCircularBitmap(originalBitmap);

                        // Set the circular bitmap to the avatar
                        textHolder.AvatarView.SetImageBitmap(circularBitmap);
                    }
                    else if (!message.IsOwnMessage)
                    {
                        textHolder.AvatarView.Visibility = ViewStates.Visible;

                        if (!string.IsNullOrWhiteSpace(message.SenderInitials))
                        {
                            // Draw initials in a circular bitmap
                            var initialsBitmap = CreateInitialsBitmap(message.SenderInitials, 96, 96); // 96x96 size
                            textHolder.AvatarView.SetImageBitmap(initialsBitmap);
                        }
                        else
                        {
                            // Default placeholder if initials are not available
                            var avatarPlaceholder = new GradientDrawable();
                            avatarPlaceholder.SetShape(ShapeType.Oval);
                            avatarPlaceholder.SetColor(AvatarBackgroundColor.ToPlatform()); // Placeholder color
                            textHolder.AvatarView.Background = avatarPlaceholder;
                            textHolder.AvatarView.SetImageBitmap(null);
                        }
                    }
                    else
                    {
                        textHolder.AvatarView.Visibility = ViewStates.Gone;
                    }
                }
                break;
            case MessageType.Image:
                if (holder is ImageMessageViewHolder imageHolder)
                {
                    // Load the image into the ImageView
                    //imageHolder.MessageImageView.SetImageBitmap(message.ImageBitmap);
                    imageHolder.TimeTextView.Text = message.Timestamp.ToString();
                    imageHolder.TimeTextView.SetTextColor(MessageTimeTextColor.ToPlatform());
                    imageHolder.TimeTextView.SetTextSize(ComplexUnitType.Sp, MessageTimeFontSize);

                    // Set the background to the chat bubble drawable
                    imageHolder.MessageImageView.SetBackground(CreateChatBubbleDrawable(backgroundColor, cornerRadius));

                    // Update constraints based on message ownership
                    var constraintSet = new ConstraintSet();
                    constraintSet.Clone((ConstraintLayout)imageHolder.ItemView);
                    constraintSet.Connect(imageHolder.MessageImageView.Id, ConstraintSet.Top, ConstraintSet.ParentId, ConstraintSet.Top);
                    constraintSet.Connect(imageHolder.MessageImageView.Id, isOwnMessage ? ConstraintSet.End : ConstraintSet.Start, ConstraintSet.ParentId, isOwnMessage ? ConstraintSet.End : ConstraintSet.Start);
                    constraintSet.ConstrainPercentWidth(imageHolder.MessageImageView.Id, 0.65f);
                    constraintSet.Connect(imageHolder.TimeTextView.Id, ConstraintSet.Top, imageHolder.MessageImageView.Id, ConstraintSet.Bottom, 8);
                    constraintSet.Connect(imageHolder.TimeTextView.Id, isOwnMessage ? ConstraintSet.End : ConstraintSet.Start, ConstraintSet.ParentId, isOwnMessage ? ConstraintSet.End : ConstraintSet.Start);
                    constraintSet.ApplyTo((ConstraintLayout)imageHolder.ItemView);
                }
                break;
            case MessageType.Video:
                if (holder is VideoMessageViewHolder videoHolder)
                {
                    // Load the video into the VideoView
                    //videoHolder.MessageVideoView.SetVideoURI(message.VideoUri);
                    videoHolder.TimeTextView.Text = message.Timestamp.ToString();
                    videoHolder.TimeTextView.SetTextColor(MessageTimeTextColor.ToPlatform());
                    videoHolder.TimeTextView.SetTextSize(ComplexUnitType.Sp, MessageTimeFontSize);

                    // Set the background to the chat bubble drawable
                    videoHolder.MessageVideoView.SetBackground(CreateChatBubbleDrawable(backgroundColor, cornerRadius));

                    // Update constraints based on message ownership
                    var constraintSet = new ConstraintSet();
                    constraintSet.Clone((ConstraintLayout)videoHolder.ItemView);
                    constraintSet.Connect(videoHolder.MessageVideoView.Id, ConstraintSet.Top, ConstraintSet.ParentId, ConstraintSet.Top);
                    constraintSet.Connect(videoHolder.MessageVideoView.Id, isOwnMessage ? ConstraintSet.End : ConstraintSet.Start, ConstraintSet.ParentId, isOwnMessage ? ConstraintSet.End : ConstraintSet.Start);
                    constraintSet.ConstrainPercentWidth(videoHolder.MessageVideoView.Id, 0.65f);
                    constraintSet.Connect(videoHolder.TimeTextView.Id, ConstraintSet.Top, videoHolder.MessageVideoView.Id, ConstraintSet.Bottom, 8);
                    constraintSet.Connect(videoHolder.TimeTextView.Id, isOwnMessage ? ConstraintSet.End : ConstraintSet.Start, ConstraintSet.ParentId, isOwnMessage ? ConstraintSet.End : ConstraintSet.Start);
                    constraintSet.ApplyTo((ConstraintLayout)videoHolder.ItemView);
                }
                break;
            case MessageType.System:
                if (holder is SystemMessageViewHolder systemHolder)
                {
                    systemHolder.SystemMessageTextView.Text = message.TextContent;
                    systemHolder.SystemMessageTextView.SetTextColor(SystemMessageTextColor.ToPlatform());
                    systemHolder.SystemMessageTextView.SetTextSize(ComplexUnitType.Sp, SystemMessageFontSize);

                    systemHolder.SystemMessageTextView.SetBackground(CreateChatBubbleDrawable(SystemMessageBackgroundColor, cornerRadius));

                }
                break;
            case MessageType.Date:
                if (holder is DateMessageViewHolder dateHolder)
                {
                    dateHolder.DateTextView.Text = message.TextContent.ToString();
                    dateHolder.DateTextView.SetTextColor(DateTextColor.ToPlatform());
                    dateHolder.DateTextView.SetTextSize(ComplexUnitType.Sp, DateTextFontSize);
                }
                break;
            default:
                throw new NotSupportedException($"Message type {message.MessageType} is not supported");
        }
    }

    private static Bitmap CreateCircularBitmap(Bitmap bitmap)
    {
        int size = Math.Min(bitmap.Width, bitmap.Height);
        Bitmap output = Bitmap.CreateBitmap(size, size, Bitmap.Config.Argb8888);

        Canvas canvas = new Canvas(output);
        Paint paint = new Paint
        {
            AntiAlias = true,
            FilterBitmap = true
        };

        Rect srcRect = new Rect(0, 0, bitmap.Width, bitmap.Height);
        RectF destRect = new RectF(0, 0, size, size);
        canvas.DrawARGB(0, 0, 0, 0); // Transparent background
        canvas.DrawCircle(size / 2f, size / 2f, size / 2f, paint);

        paint.SetXfermode(new PorterDuffXfermode(PorterDuff.Mode.SrcIn));
        canvas.DrawBitmap(bitmap, srcRect, destRect, paint);

        return output;
    }

    private Bitmap CreateInitialsBitmap(string initials, int width, int height)
    {
        Bitmap output = Bitmap.CreateBitmap(width, height, Bitmap.Config.Argb8888);

        using (Canvas canvas = new Canvas(output))
        {
            // Draw the circular background
            Paint backgroundPaint = new Paint
            {
                AntiAlias = true,
                Color = AvatarBackgroundColor.ToPlatform() // Background color
            };
            canvas.DrawCircle(width / 2f, height / 2f, width / 2f, backgroundPaint);

            // Draw the initials
            Paint textPaint = new Paint
            {
                AntiAlias = true,
                Color = AvatarTextColor.ToPlatform(), // Text color
                TextAlign = Paint.Align.Center,
                TextSize = width / 3f // Adjust text size based on avatar size
            };

            // Calculate text position
            Rect textBounds = new Rect();
            textPaint.GetTextBounds(initials, 0, initials.Length, textBounds);
            float x = width / 2f;
            float y = (height / 2f) - textBounds.ExactCenterY();

            // Draw the text
            canvas.DrawText(initials, x, y, textPaint);
        }

        return output;
    }


}

public class TextMessageViewHolder : RecyclerView.ViewHolder
{
    public ImageView AvatarView { get; private set; }
    public TextView MessageTextView { get; private set; }
    public TextView TimeTextView { get; private set; }

    public TextMessageViewHolder(aViews.View itemView, int messageTextViewId, int timeTextViewId, int avatarViewId) : base(itemView)
    {
        MessageTextView = itemView.FindViewById<TextView>(messageTextViewId);
        TimeTextView = itemView.FindViewById<TextView>(timeTextViewId);
        AvatarView = itemView.FindViewById<ImageView>(avatarViewId);
    }
}


public class ImageMessageViewHolder : RecyclerView.ViewHolder
{
    public ImageView AvatarView { get; private set; }
    public ImageView MessageImageView { get; private set; }
    public TextView TimeTextView { get; private set; }

    public ImageMessageViewHolder(aViews.View itemView, int messageImageViewId, int timeTextViewId, int avatarViewId) : base(itemView)
    {
        MessageImageView = itemView.FindViewById<ImageView>(messageImageViewId);
        TimeTextView = itemView.FindViewById<TextView>(timeTextViewId);
        AvatarView = itemView.FindViewById<ImageView>(avatarViewId);
    }
}

public class VideoMessageViewHolder : RecyclerView.ViewHolder
{
    public ImageView AvatarView { get; private set; }
    public VideoView MessageVideoView { get; private set; }
    public TextView TimeTextView { get; private set; }

    public VideoMessageViewHolder(aViews.View itemView, int messageVideoViewId, int timeTextViewId, int avatarViewId) : base(itemView)
    {
        MessageVideoView = itemView.FindViewById<VideoView>(messageVideoViewId);
        TimeTextView = itemView.FindViewById<TextView>(timeTextViewId);
        AvatarView = itemView.FindViewById<ImageView>(avatarViewId);
    }
}

public class SystemMessageViewHolder : RecyclerView.ViewHolder
{
    public TextView SystemMessageTextView { get; private set; }

    public SystemMessageViewHolder(aViews.View itemView, int systemMessageTextViewId) : base(itemView)
    {
        SystemMessageTextView = itemView.FindViewById<TextView>(systemMessageTextViewId);
    }
}

public class DateMessageViewHolder : RecyclerView.ViewHolder
{
    public TextView DateTextView { get; private set; }

    public DateMessageViewHolder(aViews.View itemView, int dateTextViewId) : base(itemView)
    {
        DateTextView = itemView.FindViewById<TextView>(dateTextViewId);
    }
}
