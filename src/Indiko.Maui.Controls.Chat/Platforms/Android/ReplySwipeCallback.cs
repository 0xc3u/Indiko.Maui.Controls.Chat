using Android.Graphics;
using AndroidX.RecyclerView.Widget;
using Indiko.Maui.Controls.Chat.Models;

namespace Indiko.Maui.Controls.Chat.Platforms.Android;

/// <summary>
/// Swipe a chat row to the right to reply: clamps the drag, and on release past the threshold
/// raises the same event as the context menu's "Reply" item — <see cref="ChatView.LongPressedCommand"/>
/// with a <see cref="ContextAction"/> named <see cref="ChatView.SwipeReplyActionName"/> — then
/// springs the row back (no removal).
/// </summary>
internal sealed class ReplySwipeCallback : ItemTouchHelper.SimpleCallback
{
    private readonly WeakReference<ChatView> _chatView;
    private readonly RecyclerView _recyclerView;

    public ReplySwipeCallback(ChatView chatView, RecyclerView recyclerView)
        : base(0, ItemTouchHelper.Right)
    {
        _chatView = new WeakReference<ChatView>(chatView);
        _recyclerView = recyclerView;
    }

    public override int GetSwipeDirs(RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder)
    {
        if (!_chatView.TryGetTarget(out var chatView) || !chatView.EnableSwipeToReply || chatView.LongPressedCommand == null)
            return 0;

        var message = MessageAt(chatView, viewHolder.BindingAdapterPosition);
        if (message == null || message.MessageType == MessageType.Date || message.MessageType == MessageType.System)
            return 0;

        return base.GetSwipeDirs(recyclerView, viewHolder);
    }

    public override bool OnMove(RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder, RecyclerView.ViewHolder target) => false;

    public override float GetSwipeThreshold(RecyclerView.ViewHolder viewHolder) => 0.25f;

    public override void OnChildDraw(Canvas canvas, RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder,
        float dX, float dY, int actionState, bool isCurrentlyActive)
    {
        // Clamp how far the row visually slides so it reads as a reply hint, not a dismiss.
        var max = recyclerView.Width * 0.35f;
        var clamped = Math.Min(dX, max);
        base.OnChildDraw(canvas, recyclerView, viewHolder, clamped, dY, actionState, isCurrentlyActive);
    }

    public override void OnSwiped(RecyclerView.ViewHolder viewHolder, int direction)
    {
        var position = viewHolder.BindingAdapterPosition;
        if (_chatView.TryGetTarget(out var chatView))
        {
            var message = MessageAt(chatView, position);
            if (message != null)
            {
                // Raise the same event as the context menu's "Reply" item.
                var action = new ContextAction { Name = chatView.SwipeReplyActionName, Message = message };
                if (chatView.LongPressedCommand?.CanExecute(action) == true)
                    chatView.LongPressedCommand.Execute(action);
            }
        }

        // Restore the row — this is a reply gesture, not a removal.
        if (position >= 0)
            _recyclerView.GetAdapter()?.NotifyItemChanged(position);
    }

    private static ChatMessage MessageAt(ChatView chatView, int position)
    {
        if (position < 0 || chatView.Messages == null || position >= chatView.Messages.Count)
            return null;
        return chatView.Messages[position];
    }
}
