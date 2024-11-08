using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.Content;
using Android.Graphics.Drawables;
using aWidgets = Android.Widget;
using AndroidX.RecyclerView.Widget;
using Microsoft.Maui.Platform;
using Android.Widget;
using AViews = Android.Views;
using Android.Graphics;
using Paint = Android.Graphics.Paint;
using Rect = Android.Graphics.Rect;
using RectF = Android.Graphics.RectF;
using AColor = Android.Graphics.Color;
using Color = Microsoft.Maui.Graphics.Color;
using AndroidX.ConstraintLayout.Widget;
using Android.Views;
using Indiko.Maui.Controls.Chat.Models;

namespace Indiko.Maui.Controls.Chat.Platforms.Android;
// Android Part
public class ChatMessageAdapter : RecyclerView.Adapter
{
    private readonly Context _context;
    private readonly IList<ChatMessage> _messages;

    public Color OwnMessageBackgroundColor { get; set; }
    public Color OtherMessageBackgroundColor { get; set; }
    public Color OwnMessageTextColor { get; set; }
    public Color OtherMessageTextColor { get; set; }

    public float MessageFontSize { get; set; }

    public Color MessageTimeTextColor { get; set; }
    public float MessageTimeFontSize { get; set; }

    public Color DateTextColor { get; set; }
    public float DateTextFontSize { get; set; }

    public Color NewMessagesSeperatorTextColor { get; set; }
    public float NewMessagesSeperatorFontSize { get; set; }
    public string NewMessagesSeperatorText { get; set; }
    public float AvatarSize { get; set; }
    public bool ScrollToFirstNewMessage { get; set; }

    public ChatMessageAdapter(Context context, IList<ChatMessage> messages)
    {
        _context = context;
        _messages = messages;
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

        // Left line for "New Messages" separator
        var leftLine = new AViews.View(_context)
        {
            Id = AViews.View.GenerateViewId(),
            LayoutParameters = new ConstraintLayout.LayoutParams(0, 2)
        };
        leftLine.SetBackgroundColor(NewMessagesSeperatorTextColor.ToPlatform());
        constraintLayout.AddView(leftLine);

        // Right line for "New Messages" separator
        var rightLine = new AViews.View(_context)
        {
            Id = AViews.View.GenerateViewId(),
            LayoutParameters = new ConstraintLayout.LayoutParams(0, 2)
        };
        rightLine.SetBackgroundColor(NewMessagesSeperatorTextColor.ToPlatform());
        constraintLayout.AddView(rightLine);

        // FrameLayout for message content (Text or Image)
        var frameLayout = new FrameLayout(_context)
        {
            Id = AViews.View.GenerateViewId()
        };
        constraintLayout.AddView(frameLayout);

        // Message TextView (chat bubble)
        var textView = new TextView(_context)
        {
            Id = AViews.View.GenerateViewId(),
            TextSize = MessageFontSize,
            Visibility = ViewStates.Gone
        };
        textView.SetPadding(32, 16, 32, 16); // Padding inside the bubble
        frameLayout.AddView(textView);

        // Message ImageView (for image messages)
        var imageView = new ImageView(_context)
        {
            Id = AViews.View.GenerateViewId(),
            Visibility = ViewStates.Gone // Initially hidden
        };
        frameLayout.AddView(imageView);

        // Timestamp TextView (displayed below the message bubble)
        var timestampTextView = new TextView(_context)
        {
            Id = AViews.View.GenerateViewId(),
            TextSize = MessageTimeFontSize
        };
        timestampTextView.SetTextColor(MessageTimeTextColor.ToPlatform());
        constraintLayout.AddView(timestampTextView);

        // Set up constraints for views
        var constraintSet = new ConstraintSet();
        constraintSet.Clone(constraintLayout);

        // Constraints for dateTextView
        constraintSet.Connect(dateTextView.Id, ConstraintSet.Top, ConstraintSet.ParentId, ConstraintSet.Top, 16);
        constraintSet.Connect(dateTextView.Id, ConstraintSet.Start, ConstraintSet.ParentId, ConstraintSet.Start, 16);
        constraintSet.Connect(dateTextView.Id, ConstraintSet.End, ConstraintSet.ParentId, ConstraintSet.End, 16);

        // Constraints for the "New Messages" separator text
        constraintSet.Connect(newMessagesSeparatorTextView.Id, ConstraintSet.Top, dateTextView.Id, ConstraintSet.Bottom, 8);
        constraintSet.Connect(newMessagesSeparatorTextView.Id, ConstraintSet.Start, leftLine.Id, ConstraintSet.End, 8);
        constraintSet.Connect(newMessagesSeparatorTextView.Id, ConstraintSet.End, rightLine.Id, ConstraintSet.Start, 8);

        // Constraints for left line
        constraintSet.Connect(leftLine.Id, ConstraintSet.Start, ConstraintSet.ParentId, ConstraintSet.Start, 16);
        constraintSet.Connect(leftLine.Id, ConstraintSet.End, newMessagesSeparatorTextView.Id, ConstraintSet.Start, 8);
        constraintSet.ConstrainWidth(leftLine.Id, 0);
        constraintSet.SetHorizontalWeight(leftLine.Id, 1f);

        // Constraints for right line
        constraintSet.Connect(rightLine.Id, ConstraintSet.Start, newMessagesSeparatorTextView.Id, ConstraintSet.End, 8);
        constraintSet.Connect(rightLine.Id, ConstraintSet.End, ConstraintSet.ParentId, ConstraintSet.End, 16);
        constraintSet.ConstrainWidth(rightLine.Id, 0);
        constraintSet.SetHorizontalWeight(rightLine.Id, 1f);

        // Constraints for frameLayout (container for textView and imageView)
        constraintSet.Connect(frameLayout.Id, ConstraintSet.Top, newMessagesSeparatorTextView.Id, ConstraintSet.Bottom, 8);
        constraintSet.ConstrainPercentWidth(frameLayout.Id, 0.65f);

        // Constraints for timestampTextView below frameLayout
        constraintSet.Connect(timestampTextView.Id, ConstraintSet.Top, frameLayout.Id, ConstraintSet.Bottom, 4);
        constraintSet.Connect(timestampTextView.Id, ConstraintSet.Start, frameLayout.Id, ConstraintSet.Start);

        constraintSet.ApplyTo(constraintLayout);

        return new ChatMessageViewHolder(constraintLayout, dateTextView, textView, imageView, timestampTextView, frameLayout, newMessagesSeparatorTextView, leftLine, rightLine);
    }

    public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
    {
        if (holder is ChatMessageViewHolder chatHolder)
        {
            var message = _messages[position];

            // Check if this is the first "New" message in the list
            bool isFirstNewMessage = message.ReadState == MessageReadState.New &&
                                     (position == 0 || _messages[position - 1].ReadState != MessageReadState.New);

            // Display the New Messages Separator and lines only above the first "New" message
            if (isFirstNewMessage)
            {
                chatHolder.NewMessagesSeparatorTextView.Visibility = ViewStates.Visible;
                chatHolder.NewMessagesSeparatorTextView.Text = NewMessagesSeperatorText;
                chatHolder.LeftLine.Visibility = ViewStates.Visible;
                chatHolder.RightLine.Visibility = ViewStates.Visible;
            }
            else
            {
                chatHolder.NewMessagesSeparatorTextView.Visibility = ViewStates.Gone;
                chatHolder.LeftLine.Visibility = ViewStates.Gone;
                chatHolder.RightLine.Visibility = ViewStates.Gone;
            }

            // Set message type handling
            if (message.MessageType == MessageType.Text)
            {
                chatHolder.ImageView.Visibility = ViewStates.Gone;
                chatHolder.TextView.Visibility = ViewStates.Visible;
                chatHolder.TextView.Text = message.TextContent;
                chatHolder.TextView.SetTextColor(message.IsOwnMessage ? OwnMessageTextColor.ToPlatform() : OtherMessageTextColor.ToPlatform());

                var backgroundColor = message.IsOwnMessage ? OwnMessageBackgroundColor.ToPlatform() : OtherMessageBackgroundColor.ToPlatform();
                var backgroundDrawable = new GradientDrawable();
                backgroundDrawable.SetColor(backgroundColor);
                backgroundDrawable.SetCornerRadius(24f);
                chatHolder.TextView.SetBackgroundDrawable(backgroundDrawable);
            }
            else if (message.MessageType == MessageType.Image)
            {
                chatHolder.TextView.Visibility = ViewStates.Gone;
                chatHolder.ImageView.Visibility = ViewStates.Visible;

                // Decode the bitmap and get its dimensions
                var bitmap = BitmapFactory.DecodeByteArray(message.BinaryContent, 0, message.BinaryContent.Length);
                chatHolder.ImageView.SetImageBitmap(bitmap);

                // Create a drawable for rounded corners
                var imageBackgroundDrawable = new GradientDrawable();
                imageBackgroundDrawable.SetColor(message.IsOwnMessage ? OwnMessageBackgroundColor.ToPlatform() : OtherMessageBackgroundColor.ToPlatform());
                imageBackgroundDrawable.SetCornerRadius(24f); // Same corner radius as text message
                chatHolder.ImageView.SetBackgroundDrawable(imageBackgroundDrawable);

                // Calculate the dimensions for the image bubble
                var imageDisplayMetrics = _context.Resources.DisplayMetrics;
                int imagemaxWidth = (int)(imageDisplayMetrics.WidthPixels * 0.65); // Limit width to 65% of screen
                float aspectRatio = (float)bitmap.Height / bitmap.Width;
                int adjustedHeight = (int)(imagemaxWidth * aspectRatio);

                // Set the ImageView's layout parameters to size the bubble to the image
                chatHolder.ImageView.LayoutParameters = new FrameLayout.LayoutParams(imagemaxWidth, adjustedHeight);

                chatHolder.ImageView.SetPadding(32, 16, 32, 16);
            }


            // Set dynamic width for the message bubble (65% of screen width)
            var displayMetrics = _context.Resources.DisplayMetrics;
            int maxWidth = (int)(displayMetrics.WidthPixels * 0.65);
            chatHolder.FrameLayout.LayoutParameters.Width = maxWidth;

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
                constraintSet.Clear(chatHolder.FrameLayout.Id, ConstraintSet.Start);
                constraintSet.Connect(chatHolder.FrameLayout.Id, ConstraintSet.End, ConstraintSet.ParentId, ConstraintSet.End, 16);

                constraintSet.Clear(chatHolder.TimestampTextView.Id, ConstraintSet.Start);
                constraintSet.Connect(chatHolder.TimestampTextView.Id, ConstraintSet.End, chatHolder.FrameLayout.Id, ConstraintSet.End);
            }
            else
            {
                constraintSet.Clear(chatHolder.FrameLayout.Id, ConstraintSet.End);
                constraintSet.Connect(chatHolder.FrameLayout.Id, ConstraintSet.Start, ConstraintSet.ParentId, ConstraintSet.Start, 16);

                constraintSet.Clear(chatHolder.TimestampTextView.Id, ConstraintSet.End);
                constraintSet.Connect(chatHolder.TimestampTextView.Id, ConstraintSet.Start, chatHolder.FrameLayout.Id, ConstraintSet.Start);
            }

            constraintSet.ApplyTo((ConstraintLayout)holder.ItemView);
        }
    }
}



//public class ChatMessageAdapter : RecyclerView.Adapter
//{
//    private readonly Context _context;
//    private readonly IList<ChatMessage> _messages;

//    public Color OwnMessageBackgroundColor { get; set; }
//    public Color OtherMessageBackgroundColor { get; set; }
//    public Color OwnMessageTextColor { get; set; }
//    public Color OtherMessageTextColor { get; set; }

//    public float MessageFontSize { get; set; }

//    public Color MessageTimeTextColor { get; set; }
//    public float MessageTimeFontSize { get; set; }

//    public Color DateTextColor { get; set; }
//    public float DateTextFontSize { get; set; }

//    public Color NewMessagesSeperatorTextColor { get; set; }
//    public float NewMessagesSeperatorFontSize { get; set; }
//    public string NewMessagesSeperatorText { get; set; }
//    public float AvatarSize { get; set; }
//    public bool ScrollToFirstNewMessage { get; set; }

//    public ChatMessageAdapter(Context context, IList<ChatMessage> messages)
//    {
//        _context = context;
//        _messages = messages;
//    }

//    public override int ItemCount => _messages.Count;


//    public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
//    {
//        var constraintLayout = new ConstraintLayout(_context)
//        {
//            LayoutParameters = new ViewGroup.LayoutParams(
//                ViewGroup.LayoutParams.MatchParent,
//                ViewGroup.LayoutParams.WrapContent)
//        };

//        // Left line for the "New Messages" separator
//        var leftLine = new AViews.View(_context)
//        {
//            Id = AViews.View.GenerateViewId(),
//            LayoutParameters = new ConstraintLayout.LayoutParams(0, 2) // Height of 2px for a thin line
//        };
//        leftLine.SetBackgroundColor(NewMessagesSeperatorTextColor.ToPlatform()); // Set the line color
//        constraintLayout.AddView(leftLine);

//        // New Messages Separator TextView
//        var newMessagesSeparatorTextView = new TextView(_context)
//        {
//            Id = AViews.View.GenerateViewId(),
//            Text = NewMessagesSeperatorText, // Placeholder text for separator
//            TextSize = NewMessagesSeperatorFontSize,
//            TextAlignment = AViews.TextAlignment.Center,
//            Visibility = ViewStates.Gone // Initially hidden
//        };
//        newMessagesSeparatorTextView.SetTextColor(NewMessagesSeperatorTextColor.ToPlatform());
//        newMessagesSeparatorTextView.SetMinimumHeight(48); // Adjust the height as necessary
//        constraintLayout.AddView(newMessagesSeparatorTextView);

//        // Right line for the "New Messages" separator
//        var rightLine = new AViews.View(_context)
//        {
//            Id = AViews.View.GenerateViewId(),
//            LayoutParameters = new ConstraintLayout.LayoutParams(0, 2) // Height of 2px for a thin line
//        };
//        rightLine.SetBackgroundColor(NewMessagesSeperatorTextColor.ToPlatform());
//        constraintLayout.AddView(rightLine);

//        // Date TextView (centered horizontally at the top)
//        var dateTextView = new TextView(_context)
//        {
//            Id = AViews.View.GenerateViewId(),
//            TextSize = DateTextFontSize,
//            Typeface = Typeface.DefaultBold,
//            Visibility = ViewStates.Gone // Initially hidden
//        };
//        dateTextView.SetTextColor(DateTextColor.ToPlatform());
//        constraintLayout.AddView(dateTextView);

//        // FrameLayout to contain the message bubble
//        var frameLayout = new FrameLayout(_context)
//        {
//            Id = AViews.View.GenerateViewId()
//        };
//        constraintLayout.AddView(frameLayout);

//        // Message TextView (chat bubble)
//        var textView = new TextView(_context)
//        {
//            Id = AViews.View.GenerateViewId(),
//            TextSize = MessageFontSize
//        };
//        textView.SetPadding(32, 16, 32, 16); // Padding inside the bubble
//        frameLayout.AddView(textView);

//        // Timestamp TextView (displayed below the message bubble)
//        var timestampTextView = new TextView(_context)
//        {
//            Id = AViews.View.GenerateViewId(),
//            TextSize = MessageTimeFontSize // Smaller font size for timestamp
//        };
//        timestampTextView.SetTextColor(MessageTimeTextColor.ToPlatform());
//        constraintLayout.AddView(timestampTextView);

//        // Set up constraints
//        var constraintSet = new ConstraintSet();
//        constraintSet.Clone(constraintLayout);

//        // Constraints for the left line
//        constraintSet.Connect(leftLine.Id, ConstraintSet.Start, ConstraintSet.ParentId, ConstraintSet.Start, 16);
//        constraintSet.Connect(leftLine.Id, ConstraintSet.End, newMessagesSeparatorTextView.Id, ConstraintSet.Start, 8);
//        constraintSet.Connect(leftLine.Id, ConstraintSet.Top, newMessagesSeparatorTextView.Id, ConstraintSet.Top);
//        constraintSet.Connect(leftLine.Id, ConstraintSet.Bottom, newMessagesSeparatorTextView.Id, ConstraintSet.Bottom);
//        constraintSet.ConstrainWidth(leftLine.Id, 0); // Width is set dynamically
//        constraintSet.SetHorizontalWeight(leftLine.Id, 1f); // Proportional width

//        // Constraints for the "New Messages" separator text (with increased top and bottom margins)
//        constraintSet.Connect(newMessagesSeparatorTextView.Id, ConstraintSet.Top, ConstraintSet.ParentId, ConstraintSet.Top, 36);
//        constraintSet.Connect(newMessagesSeparatorTextView.Id, ConstraintSet.Bottom, ConstraintSet.ParentId, ConstraintSet.Top, 36);
//        constraintSet.Connect(newMessagesSeparatorTextView.Id, ConstraintSet.Start, leftLine.Id, ConstraintSet.End, 8);
//        constraintSet.Connect(newMessagesSeparatorTextView.Id, ConstraintSet.End, rightLine.Id, ConstraintSet.Start, 8);

//        // Constraints for the right line
//        constraintSet.Connect(rightLine.Id, ConstraintSet.Start, newMessagesSeparatorTextView.Id, ConstraintSet.End, 8);
//        constraintSet.Connect(rightLine.Id, ConstraintSet.End, ConstraintSet.ParentId, ConstraintSet.End, 16);
//        constraintSet.Connect(rightLine.Id, ConstraintSet.Top, newMessagesSeparatorTextView.Id, ConstraintSet.Top);
//        constraintSet.Connect(rightLine.Id, ConstraintSet.Bottom, newMessagesSeparatorTextView.Id, ConstraintSet.Bottom);
//        constraintSet.ConstrainWidth(rightLine.Id, 0); // Width is set dynamically
//        constraintSet.SetHorizontalWeight(rightLine.Id, 1f); // Proportional width

//        // Constraints for dateTextView
//        constraintSet.Connect(dateTextView.Id, ConstraintSet.Top, newMessagesSeparatorTextView.Id, ConstraintSet.Bottom, 8);
//        constraintSet.Connect(dateTextView.Id, ConstraintSet.Start, ConstraintSet.ParentId, ConstraintSet.Start);
//        constraintSet.Connect(dateTextView.Id, ConstraintSet.End, ConstraintSet.ParentId, ConstraintSet.End);

//        // Constraints for frameLayout (container for textView)
//        constraintSet.Connect(frameLayout.Id, ConstraintSet.Top, dateTextView.Id, ConstraintSet.Bottom, 8);
//        constraintSet.ConstrainPercentWidth(frameLayout.Id, 0.65f); // Set width to 65% of the parent's width

//        // Constraints for timestampTextView below the frameLayout
//        constraintSet.Connect(timestampTextView.Id, ConstraintSet.Top, frameLayout.Id, ConstraintSet.Bottom, 4);
//        constraintSet.Connect(timestampTextView.Id, ConstraintSet.Start, frameLayout.Id, ConstraintSet.Start);

//        constraintSet.ApplyTo(constraintLayout);

//        return new ChatMessageViewHolder(constraintLayout, dateTextView, textView, timestampTextView, newMessagesSeparatorTextView, leftLine, rightLine);
//    }

//    public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
//    {
//        if (holder is ChatMessageViewHolder chatHolder)
//        {
//            var message = _messages[position];

//            // Check if this message is the first "New" message in the list
//            bool isFirstNewMessage = message.ReadState == MessageReadState.New &&
//                                     (position == 0 || _messages[position - 1].ReadState != MessageReadState.New);

//            // Display the New Messages Separator and lines only above the first "New" message
//            if (isFirstNewMessage)
//            {
//                chatHolder.NewMessagesSeperatorTextView.Visibility = ViewStates.Visible;
//                chatHolder.NewMessagesSeperatorTextView.Text = NewMessagesSeperatorText;
//                chatHolder.NewMessagesSeperatorTextView.SetTextColor(NewMessagesSeperatorTextColor.ToPlatform());
//                chatHolder.NewMessagesSeperatorTextView.TextSize = NewMessagesSeperatorFontSize;

//                // Set the separator lines' visibility to visible for this row
//                chatHolder.LeftLine.Visibility = ViewStates.Visible;
//                chatHolder.RightLine.Visibility = ViewStates.Visible;
//            }
//            else
//            {
//                // Hide the New Messages Separator and lines for other rows
//                chatHolder.NewMessagesSeperatorTextView.Visibility = ViewStates.Gone;
//                chatHolder.LeftLine.Visibility = ViewStates.Gone;
//                chatHolder.RightLine.Visibility = ViewStates.Gone;
//            }

//            // Check if the message is the first of a new date and set date display
//            bool isFirstMessageOfDate = position == 0 || _messages[position - 1].Timestamp.Date != message.Timestamp.Date;
//            chatHolder.DateTextView.Visibility = isFirstMessageOfDate ? ViewStates.Visible : ViewStates.Gone;
//            if (isFirstMessageOfDate)
//            {
//                chatHolder.DateTextView.Text = message.Timestamp.ToString("dddd MMM dd");
//            }

//            // Set the message text and colors
//            chatHolder.TextView.Text = message.TextContent;
//            chatHolder.TextView.SetTextColor(message.IsOwnMessage ? OwnMessageTextColor.ToPlatform() : OtherMessageTextColor.ToPlatform());

//            var backgroundColor = message.IsOwnMessage
//                ? OwnMessageBackgroundColor.ToPlatform()
//                : OtherMessageBackgroundColor.ToPlatform();

//            var backgroundDrawable = new GradientDrawable();
//            backgroundDrawable.SetColor(backgroundColor);
//            backgroundDrawable.SetCornerRadius(24f);
//            chatHolder.TextView.SetBackgroundDrawable(backgroundDrawable);

//            // Calculate 65% of the screen width for message bubble width
//            var displayMetrics = _context.Resources.DisplayMetrics;
//            int maxWidth = (int)(displayMetrics.WidthPixels * 0.65);
//            var frameLayout = (AViews.View)chatHolder.TextView.Parent;
//            frameLayout.LayoutParameters.Width = maxWidth;

//            // Set the timestamp text below the message bubble
//            chatHolder.TimestampTextView.Text = message.Timestamp.ToString("HH:mm");

//            // Update alignment for own messages or received messages
//            var constraintSet = new ConstraintSet();
//            constraintSet.Clone((ConstraintLayout)holder.ItemView);

//            if (message.IsOwnMessage)
//            {
//                // Align the message bubble to the right for own messages
//                constraintSet.Clear(frameLayout.Id, ConstraintSet.Start);
//                constraintSet.Connect(frameLayout.Id, ConstraintSet.End, ConstraintSet.ParentId, ConstraintSet.End, 16);

//                // Align timestamp below the bubble, aligned to the right
//                constraintSet.Clear(chatHolder.TimestampTextView.Id, ConstraintSet.Start);
//                constraintSet.Connect(chatHolder.TimestampTextView.Id, ConstraintSet.End, frameLayout.Id, ConstraintSet.End);
//            }
//            else
//            {
//                // Align the message bubble to the left for received messages
//                constraintSet.Clear(frameLayout.Id, ConstraintSet.End);
//                constraintSet.Connect(frameLayout.Id, ConstraintSet.Start, ConstraintSet.ParentId, ConstraintSet.Start, 16);

//                // Align timestamp below the bubble, aligned to the left
//                constraintSet.Clear(chatHolder.TimestampTextView.Id, ConstraintSet.End);
//                constraintSet.Connect(chatHolder.TimestampTextView.Id, ConstraintSet.Start, frameLayout.Id, ConstraintSet.Start);
//            }

//            constraintSet.ApplyTo((ConstraintLayout)holder.ItemView);
//        }
//    }





//    // Helper method to draw sender initials inside a circle
//    private void DrawSenderInitials(ImageView imageView, string senderName)
//    {
//        // Extract initials from the sender's name
//        var initials = GetInitials(senderName);

//        // Create a bitmap to draw on
//        var bitmap = Bitmap.CreateBitmap(64, 64, Bitmap.Config.Argb8888); // 64x64 px for higher quality
//        var canvas = new Canvas(bitmap);

//        // Draw the circle background
//        var paint = new Paint
//        {
//            AntiAlias = true,
//            Color = AColor.LightGray // Background color for the circle
//        };
//        canvas.DrawCircle(32, 32, 32, paint); // Draw circle with radius 32px

//        // Draw the initials
//        paint.Color = AColor.White; // Text color
//        paint.TextSize = 24; // Text size
//        paint.TextAlign = Paint.Align.Center;
//        var textY = (canvas.Height / 2) - ((paint.Descent() + paint.Ascent()) / 2);
//        canvas.DrawText(initials, 32, textY, paint); // Draw initials in the center

//        // Set the bitmap to the ImageView
//        imageView.SetImageBitmap(bitmap);
//    }

//    // Helper method to extract initials from a name
//    private string GetInitials(string name)
//    {
//        if (string.IsNullOrWhiteSpace(name))
//            return "?";

//        var words = name.Split(' ');
//        var initials = "";

//        foreach (var word in words)
//        {
//            if (!string.IsNullOrWhiteSpace(word) && char.IsLetter(word[0]))
//            {
//                initials += char.ToUpper(word[0]);
//            }
//        }

//        return initials.Length > 2 ? initials.Substring(0, 2) : initials;
//    }

//}
