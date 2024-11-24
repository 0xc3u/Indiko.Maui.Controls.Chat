using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Widget;
using AndroidX.ConstraintLayout.Widget;
using AndroidX.RecyclerView.Widget;
using Indiko.Maui.Controls.Chat.Models;
using Microsoft.Maui.Platform;
using aIO = Java.IO;
using ANet = Android.Net;
using AViews = Android.Views;
using Color = Microsoft.Maui.Graphics.Color;
using Paint = Android.Graphics.Paint;
using Rect = Android.Graphics.Rect;
using RectF = Android.Graphics.RectF;

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
    }

    public override int ItemCount => _messages.Count;

    public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
    {
        var constraintLayout = new ConstraintLayout(_context)
        {
            LayoutParameters = new ViewGroup.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent)
        };

        // Avatar ImageView
        var avatarView = new ImageView(_context)
        {
            Id = AViews.View.GenerateViewId(),
            LayoutParameters = new ConstraintLayout.LayoutParams(96, 96) // Fixed size
        };
        avatarView.SetScaleType(ImageView.ScaleType.CenterCrop); // Center and crop image
        constraintLayout.AddView(avatarView);

        // Add a circular shape drawable
        var avatarBackground = new GradientDrawable();
        avatarBackground.SetShape(ShapeType.Oval);
        avatarBackground.SetColor(AvatarBackgroundColor.ToPlatform());
        avatarView.Background = avatarBackground;


        // Date TextView
        var dateTextView = new TextView(_context)
        {
            Id = AViews.View.GenerateViewId(),
            TextSize = DateTextFontSize,
            Typeface = Typeface.DefaultBold,
            Visibility = ViewStates.Gone // Initially hidden
        };
        dateTextView.SetTextColor(DateTextColor.ToPlatform());
        constraintLayout.AddView(dateTextView);

        // New Messages Separator TextView
        var newMessagesSeparatorTextView = new TextView(_context)
        {
            Id = AViews.View.GenerateViewId(),
            Text = NewMessagesSeperatorText,
            TextSize = NewMessagesSeperatorFontSize,
            TextAlignment = AViews.TextAlignment.Center,
            Visibility = ViewStates.Gone // Initially hidden
        };
        newMessagesSeparatorTextView.SetTextColor(NewMessagesSeperatorTextColor.ToPlatform());
        constraintLayout.AddView(newMessagesSeparatorTextView);

        // FrameLayout for message content (Text or Image)
        var frameLayout = new FrameLayout(_context)
        {
            Id = AViews.View.GenerateViewId()
        };
        constraintLayout.AddView(frameLayout);

        // LinearLayout for two rows inside FrameLayout
        var linearLayout = new LinearLayout(_context)
        {
            Orientation = Orientation.Vertical,
            LayoutParameters = new FrameLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent)
        };
        frameLayout.AddView(linearLayout);

        // Reply summary container
        var replySummaryFrame = new LinearLayout(_context)
        {
            Id = AViews.View.GenerateViewId(),
            Orientation = Orientation.Vertical,
            Visibility = ViewStates.Gone // Initially hidden
        };
        replySummaryFrame.SetPadding(32, 16, 32, 16); // Padding for the reply summary
        linearLayout.AddView(replySummaryFrame, new LinearLayout.LayoutParams(
            ViewGroup.LayoutParams.MatchParent,
            ViewGroup.LayoutParams.WrapContent));

        // Row 1: Sender name
        var replySenderTextView = new TextView(_context)
        {
            Id = AViews.View.GenerateViewId(),
            TextSize = ReplyMessageFontSize, // Smaller font size for the sender name
            Typeface = Typeface.DefaultBold,
        };
        replySummaryFrame.AddView(replySenderTextView);

        // Row 2: Message preview
        var replyPreviewTextView = new TextView(_context)
        {
            Id = AViews.View.GenerateViewId(),
            TextSize = ReplyMessageFontSize, // Smaller font size for the preview
        };
        replySummaryFrame.AddView(replyPreviewTextView);

        // Message TextView (chat bubble)
        var textView = new TextView(_context)
        {
            Id = AViews.View.GenerateViewId(),
            TextSize = MessageFontSize,
            Visibility = ViewStates.Gone
        };
        textView.SetPadding(32, 16, 32, 16); // Padding inside the bubble
        linearLayout.AddView(textView, new LinearLayout.LayoutParams(
            ViewGroup.LayoutParams.MatchParent,
            ViewGroup.LayoutParams.WrapContent));

        // Message ImageView (for image messages)
        var imageView = new ImageView(_context)
        {
            Id = AViews.View.GenerateViewId(),
            Visibility = ViewStates.Gone // Initially hidden
        };
        linearLayout.AddView(imageView, new LinearLayout.LayoutParams(
            ViewGroup.LayoutParams.MatchParent,
            ViewGroup.LayoutParams.WrapContent));

        // Inside OnCreateViewHolder, add the VideoView wrapper
        var videoContainer = new FrameLayout(_context)
        {
            Id = AViews.View.GenerateViewId(),
            Visibility = ViewStates.Gone
        };

        // Create the VideoView
        var videoView = new VideoView(_context)
        {
            Id = AViews.View.GenerateViewId()
        };

        // Add the VideoView to the container
        videoContainer.AddView(videoView);
        linearLayout.AddView(videoContainer, new LinearLayout.LayoutParams(
            ViewGroup.LayoutParams.MatchParent,
            ViewGroup.LayoutParams.WrapContent));

        // Timestamp TextView (displayed below the message bubble)
        var timestampTextView = new TextView(_context)
        {
            Id = AViews.View.GenerateViewId(),
            TextSize = MessageTimeFontSize
        };
        timestampTextView.SetTextColor(MessageTimeTextColor.ToPlatform());
        constraintLayout.AddView(timestampTextView);

        // Add the reaction container (LinearLayout)
        var reactionContainer = new LinearLayout(_context)
        {
            Id = AViews.View.GenerateViewId(),
            Orientation = Orientation.Horizontal,
            LayoutParameters = new ConstraintLayout.LayoutParams(
                ViewGroup.LayoutParams.WrapContent,
                ViewGroup.LayoutParams.WrapContent)
        };

        // Set margin or padding for the reaction container if needed
        reactionContainer.SetPadding(16, 8, 16, 8);
        constraintLayout.AddView(reactionContainer);

        // Delivery status icon
        var deliveryStatusIcon = new ImageView(_context)
        {
            Id = AViews.View.GenerateViewId(),
            LayoutParameters = new ConstraintLayout.LayoutParams(32, 32) // Set the size of the icon
        };
        constraintLayout.AddView(deliveryStatusIcon);

        // Set up constraints for views
        var constraintSet = new ConstraintSet();
        constraintSet.Clone(constraintLayout);

        // Constraints for DeliveryStatusIcon
        constraintSet.Connect(deliveryStatusIcon.Id, ConstraintSet.Top, timestampTextView.Id, ConstraintSet.Top);
        constraintSet.Connect(deliveryStatusIcon.Id, ConstraintSet.Start, timestampTextView.Id, ConstraintSet.End, 8); // Add spacing

        // Constraints for the reaction container (below the message bubble)
        constraintSet.Connect(reactionContainer.Id, ConstraintSet.Top, frameLayout.Id, ConstraintSet.Bottom, 4);
        constraintSet.Connect(reactionContainer.Id, ConstraintSet.Start, frameLayout.Id, ConstraintSet.Start);
        constraintSet.Connect(reactionContainer.Id, ConstraintSet.End, frameLayout.Id, ConstraintSet.End);

        // Constraints for avatarView
        constraintSet.Connect(avatarView.Id, ConstraintSet.Top, ConstraintSet.ParentId, ConstraintSet.Top, 0);
        constraintSet.Connect(avatarView.Id, ConstraintSet.Start, ConstraintSet.ParentId, ConstraintSet.Start, 32);

        // Constraints for dateTextView
        constraintSet.Connect(dateTextView.Id, ConstraintSet.Top, ConstraintSet.ParentId, ConstraintSet.Top, 16);
        constraintSet.Connect(dateTextView.Id, ConstraintSet.Start, ConstraintSet.ParentId, ConstraintSet.Start, 16);
        constraintSet.Connect(dateTextView.Id, ConstraintSet.End, ConstraintSet.ParentId, ConstraintSet.End, 16);

        // Constraints for the "New Messages" separator text
        constraintSet.Connect(newMessagesSeparatorTextView.Id, ConstraintSet.Top, dateTextView.Id, ConstraintSet.Bottom, 8);
        constraintSet.Connect(newMessagesSeparatorTextView.Id, ConstraintSet.Start, ConstraintSet.ParentId, ConstraintSet.Start);
        constraintSet.Connect(newMessagesSeparatorTextView.Id, ConstraintSet.End, ConstraintSet.ParentId, ConstraintSet.End);

        // Constraints for frameLayout (container for textView and imageView)
        constraintSet.Connect(frameLayout.Id, ConstraintSet.Top, newMessagesSeparatorTextView.Id, ConstraintSet.Bottom, 8);
        constraintSet.ConstrainPercentWidth(frameLayout.Id, 0.65f);

        // Constraints for timestampTextView below frameLayout
        constraintSet.Connect(timestampTextView.Id, ConstraintSet.Top, frameLayout.Id, ConstraintSet.Bottom, 4);
        constraintSet.Connect(timestampTextView.Id, ConstraintSet.Start, frameLayout.Id, ConstraintSet.Start);

        constraintSet.ApplyTo(constraintLayout);

        return new ChatMessageViewHolder(constraintLayout, dateTextView, textView, imageView, videoContainer, videoView, timestampTextView, frameLayout,
            newMessagesSeparatorTextView, avatarView, reactionContainer, deliveryStatusIcon, replySummaryFrame, replyPreviewTextView, replySenderTextView);
    }




    public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
    {
        if (holder is ChatMessageViewHolder chatHolder)
        {
            var message = _messages[position];

            chatHolder.DetachEventHandlers(); // Ensure no previous handlers are attached
            chatHolder.AttachEventHandlers(message, VirtualView);

            // Check if this is the first "New" message in the list
            bool isFirstNewMessage = message.ReadState == MessageReadState.New &&
                                      (position == 0 || _messages[position - 1].ReadState != MessageReadState.New);

            // Display the New Messages Separator and lines only above the first "New" message
            if (isFirstNewMessage)
            {
                chatHolder.NewMessagesSeparatorTextView.Visibility = ViewStates.Visible;
                chatHolder.NewMessagesSeparatorTextView.Text = NewMessagesSeperatorText;
            }
            else
            {
                chatHolder.NewMessagesSeparatorTextView.Visibility = ViewStates.Gone;
            }

            // Avatar handling
            if (!message.IsOwnMessage && message.SenderAvatar != null)
            {
                chatHolder.AvatarView.Visibility = ViewStates.Visible;

                // Load avatar as bitmap
                var originalBitmap = BitmapFactory.DecodeByteArray(message.SenderAvatar, 0, message.SenderAvatar.Length);

                // Crop the bitmap into a circular shape
                var circularBitmap = CreateCircularBitmap(originalBitmap);

                // Set the circular bitmap to the avatar
                chatHolder.AvatarView.SetImageBitmap(circularBitmap);
            }
            else if (!message.IsOwnMessage)
            {
                chatHolder.AvatarView.Visibility = ViewStates.Visible;

                if (!string.IsNullOrWhiteSpace(message.SenderInitials))
                {
                    // Draw initials in a circular bitmap
                    var initialsBitmap = CreateInitialsBitmap(message.SenderInitials, 96, 96); // 96x96 size
                    chatHolder.AvatarView.SetImageBitmap(initialsBitmap);
                }
                else
                {
                    // Default placeholder if initials are not available
                    var avatarPlaceholder = new GradientDrawable();
                    avatarPlaceholder.SetShape(ShapeType.Oval);
                    avatarPlaceholder.SetColor(AvatarBackgroundColor.ToPlatform()); // Placeholder color
                    chatHolder.AvatarView.Background = avatarPlaceholder;
                    chatHolder.AvatarView.SetImageBitmap(null);
                }
            }
            else
            {
                chatHolder.AvatarView.Visibility = ViewStates.Gone;
            }

            // Set message type handling
            if (message.MessageType == MessageType.Text)
            {
                chatHolder.ImageView.Visibility = ViewStates.Gone;
                chatHolder.VideoContainer.Visibility = ViewStates.Gone;
                chatHolder.VideoView.Visibility = ViewStates.Gone;
                chatHolder.TextView.Visibility = ViewStates.Visible;
                chatHolder.TextView.Text = message.TextContent;
                chatHolder.TextView.SetTextColor(message.IsOwnMessage ? OwnMessageTextColor.ToPlatform() : OtherMessageTextColor.ToPlatform());
            }
            else if (message.MessageType == MessageType.Image)
            {
                if (message.BinaryContent != null)
                {
                    chatHolder.TextView.Visibility = ViewStates.Gone;
                    chatHolder.ImageView.Visibility = ViewStates.Visible;
                    chatHolder.VideoContainer.Visibility = ViewStates.Gone;
                    chatHolder.VideoView.Visibility = ViewStates.Gone;

                    // Decode the bitmap and get its dimensions
                    var bitmap = BitmapFactory.DecodeByteArray(message.BinaryContent, 0, message.BinaryContent.Length);
                    chatHolder.ImageView.SetImageBitmap(bitmap);

                   // Calculate the dimensions for the image bubble
                    var imageDisplayMetrics = _context.Resources.DisplayMetrics;
                    int imagemaxWidth = (int)(imageDisplayMetrics.WidthPixels * 0.65); // Limit width to 65% of screen
                    float aspectRatio = (float)bitmap.Height / bitmap.Width;
                    int adjustedHeight = (int)(imagemaxWidth * aspectRatio);

                    // Set the ImageView's layout parameters to size the bubble to the image
                    chatHolder.ImageView.LayoutParameters = new LinearLayout.LayoutParams(imagemaxWidth, adjustedHeight);

                    chatHolder.ImageView.SetPadding(32, 16, 32, 16);
                }
                else
                {
                    chatHolder.TextView.Visibility = ViewStates.Gone;
                    chatHolder.ImageView.Visibility = ViewStates.Gone;
                    chatHolder.VideoContainer.Visibility = ViewStates.Gone;
                    chatHolder.VideoView.Visibility = ViewStates.Gone;
                }
            }
            else if (message.MessageType == MessageType.Video)
            {
                if (message.BinaryContent != null)
                {
                    chatHolder.TextView.Visibility = ViewStates.Gone;
                    chatHolder.ImageView.Visibility = ViewStates.Gone;
                    chatHolder.VideoContainer.Visibility = ViewStates.Visible;
                    chatHolder.VideoView.Visibility = ViewStates.Visible;

                    // Define the file path based on MessageId
                    var tempFile = new aIO.File(_context.CacheDir, $"{message.MessageId}.mp4");

                    // Check if file exists; if not, create it
                    if (!tempFile.Exists())
                    {
                        using (var fileStream = new FileStream(tempFile.AbsolutePath, FileMode.Create, FileAccess.Write))
                        {
                            fileStream.Write(message.BinaryContent, 0, message.BinaryContent.Length);
                        }
                    }

                    // Set the VideoView to play the temporary video file
                    var videoUri = ANet.Uri.FromFile(tempFile);
                    chatHolder.VideoView.SetVideoURI(videoUri);
                    chatHolder.VideoView.RequestFocus();

                     // Calculate dimensions for the video bubble
                    var videodisplayMetrics = _context.Resources.DisplayMetrics;
                    int videomaxWidth = (int)(videodisplayMetrics.WidthPixels * 0.65);
                    float aspectRatio = 9f / 16f;
                    int adjustedHeight = (int)(videomaxWidth * aspectRatio);

                    // Set layout parameters for the container to size the bubble
                    chatHolder.VideoContainer.LayoutParameters = new LinearLayout.LayoutParams(videomaxWidth, adjustedHeight);

                    // Set padding on the container for consistent styling
                    chatHolder.VideoContainer.SetPadding(32, 16, 32, 16);

                    // Start playback when the video is ready
                    chatHolder.VideoView.Start();
                }
                else
                {
                    chatHolder.TextView.Visibility = ViewStates.Gone;
                    chatHolder.ImageView.Visibility = ViewStates.Gone;
                    chatHolder.VideoContainer.Visibility = ViewStates.Gone;
                    chatHolder.VideoView.Visibility = ViewStates.Gone;
                }
            }

            if (chatHolder.ReactionContainer.ChildCount > 0)
            {
                chatHolder.ReactionContainer.RemoveAllViews(); // Clear existing reactions
            }

            if (message.Reactions != null && message.Reactions.Any())
            {
                foreach (var reaction in message.Reactions)
                {
                    // Create a TextView for each reaction
                    var reactionTextView = new TextView(_context)
                    {
                        Text = $"{reaction.Emoji} {reaction.Count}",
                        TextSize = EmojiReactionFontSize
                    };
                    reactionTextView.SetTextColor(EmojiReactionTextColor.ToPlatform());

                    // Optional: Add padding or margins
                    var layoutParams = new LinearLayout.LayoutParams(
                        ViewGroup.LayoutParams.WrapContent,
                        ViewGroup.LayoutParams.WrapContent)
                    {
                        LeftMargin = 8 // Add spacing between reactions
                    };

                    reactionTextView.LayoutParameters = layoutParams;

                    chatHolder.ReactionContainer.AddView(reactionTextView);
                }
            }

            if (message.DeliveryState == MessageDeliveryState.Sent && SendIcon != null)
            {
                SetImageSourceToImageView(SendIcon, chatHolder.DeliveryStatusIcon);
            }
            else if (message.DeliveryState == MessageDeliveryState.Delivered && DeliveredIcon != null)
            {
                SetImageSourceToImageView(DeliveredIcon, chatHolder.DeliveryStatusIcon);
            }
            else if (message.DeliveryState == MessageDeliveryState.Read && ReadIcon != null)
            {
                SetImageSourceToImageView(ReadIcon, chatHolder.DeliveryStatusIcon);
            }

            // Set dynamic width for the message bubble (65% of screen width)
            var displayMetrics = _context.Resources.DisplayMetrics;
            int maxWidth = (int)(displayMetrics.WidthPixels * 0.65);
            chatHolder.FrameLayout.LayoutParameters.Width = maxWidth;


            var frameLayoutBackgroundDrawable = new GradientDrawable();
            frameLayoutBackgroundDrawable.SetColor(message.IsOwnMessage ? OwnMessageBackgroundColor.ToPlatform() : OtherMessageBackgroundColor.ToPlatform());
            frameLayoutBackgroundDrawable.SetCornerRadius(24f); // Same corner radius as text message
            chatHolder.FrameLayout.Background = frameLayoutBackgroundDrawable;

            if (message.IsRepliedMessage && message.ReplyToMessage != null)
            {
                chatHolder.ReplySummaryFrame.Visibility = ViewStates.Visible;

                // Set sender name and preview
                chatHolder.ReplySenderTextView.Text = message.ReplyToMessage.SenderId;
                chatHolder.ReplySenderTextView.SetTextColor(ReplyMessageTextColor.ToPlatform());

                chatHolder.ReplyPreviewTextView.Text = RepliedMessage.GenerateTextPreview(message.ReplyToMessage.TextPreview);
                chatHolder.ReplyPreviewTextView.SetTextColor(ReplyMessageTextColor.ToPlatform());

                var replyBackground = new GradientDrawable();
                replyBackground.SetColor(ReplyMessageBackgroundColor.ToPlatform());
                replyBackground.SetCornerRadius(24f);
                chatHolder.ReplySummaryFrame.Background = replyBackground;
            }
            else
            {
                chatHolder.ReplySummaryFrame.Visibility = ViewStates.Gone;
            }

            // Set date and time
            bool isFirstMessageOfDate = position == 0 || _messages[position - 1].Timestamp.Date != message.Timestamp.Date;
            chatHolder.DateTextView.Visibility = isFirstMessageOfDate ? ViewStates.Visible : ViewStates.Gone;
            if (isFirstMessageOfDate)
            {
                chatHolder.DateTextView.Text = message.Timestamp.ToString("dddd MMM dd");
            }

            chatHolder.TimestampTextView.Text = message.Timestamp.ToString("HH:mm");

            // Set alignment based on IsOwnMessage
            var constraintSet = new ConstraintSet();
            constraintSet.Clone((ConstraintLayout)holder.ItemView);

            if (message.IsOwnMessage)
            {
                constraintSet.Clear(chatHolder.AvatarView.Id, ConstraintSet.Start);
                constraintSet.Clear(chatHolder.FrameLayout.Id, ConstraintSet.Start);
                constraintSet.Connect(chatHolder.FrameLayout.Id, ConstraintSet.End, ConstraintSet.ParentId, ConstraintSet.End, 16);

                constraintSet.Clear(chatHolder.TimestampTextView.Id, ConstraintSet.Start);
                constraintSet.Connect(chatHolder.TimestampTextView.Id, ConstraintSet.End, chatHolder.FrameLayout.Id, ConstraintSet.End);
            }
            else
            {
                constraintSet.Connect(chatHolder.AvatarView.Id, ConstraintSet.Start, ConstraintSet.ParentId, ConstraintSet.Start, 16);
                constraintSet.Connect(chatHolder.AvatarView.Id, ConstraintSet.Top, chatHolder.FrameLayout.Id, ConstraintSet.Top);

                constraintSet.Clear(chatHolder.FrameLayout.Id, ConstraintSet.End);
                constraintSet.Connect(chatHolder.FrameLayout.Id, ConstraintSet.Start, chatHolder.AvatarView.Id, ConstraintSet.End, 28);

                constraintSet.Clear(chatHolder.TimestampTextView.Id, ConstraintSet.End);
                constraintSet.Connect(chatHolder.TimestampTextView.Id, ConstraintSet.Start, chatHolder.FrameLayout.Id, ConstraintSet.Start);
            }

            // Align the ReactionContainer based on IsOwnMessage
            if (message.IsOwnMessage)
            {
                // Align ReactionContainer to the left of the chat bubble
                constraintSet.Clear(chatHolder.ReactionContainer.Id, ConstraintSet.End);
                constraintSet.Connect(chatHolder.ReactionContainer.Id, ConstraintSet.Start, chatHolder.FrameLayout.Id, ConstraintSet.Start);
            }
            else
            {
                // Align ReactionContainer to the right of the chat bubble
                constraintSet.Clear(chatHolder.ReactionContainer.Id, ConstraintSet.Start);
                constraintSet.Connect(chatHolder.ReactionContainer.Id, ConstraintSet.End, chatHolder.FrameLayout.Id, ConstraintSet.End);
            }

            constraintSet.ApplyTo((ConstraintLayout)holder.ItemView);
        }
    }



    private void SetImageSourceToImageView(ImageSource imageSource, ImageView imageView)
    {
        if (imageSource == null || imageView == null || _mauiContext == null)
            return;

        try
        {
            ImageSourceExtensions.LoadImage(imageSource, _mauiContext, (img) =>
            {
                if (img.Value != null)
                {
                    imageView.SetImageDrawable(img.Value);
                }
            });
        }
        catch (Exception ex)
        {
            // Handle exceptions, e.g., invalid image sources
            Console.WriteLine($"Error resolving ImageSource: {ex.Message}");
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