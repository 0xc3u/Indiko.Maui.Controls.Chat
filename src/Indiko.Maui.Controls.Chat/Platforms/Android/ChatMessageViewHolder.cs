using aViews = Android.Views;
using AndroidX.RecyclerView.Widget;
using Android.Widget;


namespace Indiko.Maui.Controls.Chat.Platforms.Android;
public class ChatMessageViewHolder : RecyclerView.ViewHolder
{
    public TextView DateTextView { get; }
    public TextView TextView { get; }
    public TextView TimestampTextView { get; }
    public TextView NewMessagesSeperatorTextView { get; }
    public aViews.View LeftLine { get; }
    public aViews.View RightLine { get; }

    public ChatMessageViewHolder(
        aViews.View itemView,
        TextView dateTextView,
        TextView textView,
        TextView timestampTextView,
        TextView newMessagesSeperatorTextView,
        aViews.View leftLine,
        aViews.View rightLine)
        : base(itemView)
    {
        DateTextView = dateTextView;
        TextView = textView;
        TimestampTextView = timestampTextView;
        NewMessagesSeperatorTextView = newMessagesSeperatorTextView;
        LeftLine = leftLine;
        RightLine = rightLine;
    }
}