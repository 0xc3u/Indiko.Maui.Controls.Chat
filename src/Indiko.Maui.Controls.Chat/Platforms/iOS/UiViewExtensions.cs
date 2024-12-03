using System.Windows.Input;
using Indiko.Maui.Controls.Chat.Models;
using UIKit;

namespace Indiko.Maui.Controls.Chat.Platforms.iOS;
internal static class UiViewExtensions
{
    public static void AddWeakTapGestureRecognizerWithCommand(this UIView uiView, ChatMessage message, ICommand command)
    {
        if(uiView== null || message == null || command == null)
        {
            return;
        }

        var messageRef = new WeakReference<ChatMessage>(message);

        var tapGestureRecognizer = new UITapGestureRecognizer(() => HandleViewTapped(uiView, messageRef, command))
        {
            NumberOfTapsRequired = 1
        };
        uiView.AddGestureRecognizer(tapGestureRecognizer);
    }

    private static void HandleViewTapped(UIView uiView, WeakReference<ChatMessage> messageRef, ICommand command)
    {
        if (messageRef.TryGetTarget(out var message) && command?.CanExecute(message) == true)
        {
            command.Execute(message);
            uiView.AnimateFade();
        }
    }

    public static void AnimateFade(this UIView view)
    {
        if(view == null)
        {
            return;
        }

        UIView.Animate(0.2, 0, UIViewAnimationOptions.CurveEaseInOut, () =>
        {
            view.Alpha = 0.7f;
        }, () =>
        {
            UIView.Animate(0.2, 0, UIViewAnimationOptions.CurveEaseInOut, () =>
            {
                view.Alpha = 1.0f;
            }, null);
        });
    }
}
