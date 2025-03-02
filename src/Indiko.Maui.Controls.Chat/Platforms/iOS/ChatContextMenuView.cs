using Indiko.Maui.Controls.Chat.Models;
using UIKit;

namespace Indiko.Maui.Controls.Chat.Platforms.iOS;

public class ChatContextMenuView : UIView
{
    private readonly ChatView _chatView;
    private readonly ChatMessage _message;
    private readonly Action _onDismiss;
    private readonly UIView _contentView;
    private readonly UIStackView _emojiStack;
    private readonly UIStackView _actionStack;
    private readonly UIVisualEffectView _blurView;

    public ChatContextMenuView(ChatView chatView, ChatMessage message, Action onDismiss)
    {
        _chatView = chatView;
        _message = message;
        _onDismiss = onDismiss;

        BackgroundColor = UIColor.Clear;
        Frame = UIScreen.MainScreen.Bounds;

        // Apply Blur Effect to Background
        _blurView = new UIVisualEffectView(UIBlurEffect.FromStyle(UIBlurEffectStyle.Dark))
        {
            Frame = UIScreen.MainScreen.Bounds,
            Alpha = 0f
        };

        AddSubview(_blurView);

        // Background tap to dismiss
        var tapGesture = new UITapGestureRecognizer(() => Dismiss());
        _blurView.AddGestureRecognizer(tapGesture);

        _contentView = new UIView
        {
            BackgroundColor = UIColor.White,
            Layer = { CornerRadius = 10f },
            ClipsToBounds = true
        };

        _emojiStack = new UIStackView
        {
            Axis = UILayoutConstraintAxis.Horizontal,
            Distribution = UIStackViewDistribution.FillProportionally,
            Alignment = UIStackViewAlignment.Center,
            Spacing = 8f,
            TranslatesAutoresizingMaskIntoConstraints = false
        };

        var scrollView = new UIScrollView
        {
            TranslatesAutoresizingMaskIntoConstraints = false,
            ShowsHorizontalScrollIndicator = false
        };
        scrollView.AddSubview(_emojiStack);

        _actionStack = new UIStackView
        {
            Axis = UILayoutConstraintAxis.Vertical,
            Spacing = 8f,
            TranslatesAutoresizingMaskIntoConstraints = false
        };

        // Load emojis into the scrollable emoji stack
        foreach (var emoji in _chatView.EmojiReactions)
        {
            var label = new UILabel
            {
                Text = emoji,
                Font = UIFont.SystemFontOfSize(24),
                UserInteractionEnabled = true
            };
            var emojiTap = new UITapGestureRecognizer(() => AddReaction(emoji));
            label.AddGestureRecognizer(emojiTap);
            _emojiStack.AddArrangedSubview(label);
        }

        // Load context actions
        foreach (var item in _chatView.ContextMenuItems)
        {
            var button = new UIButton(UIButtonType.System);
            button.SetTitle(item.Name, UIControlState.Normal);
            button.SetTitleColor(item.IsDestructive ? UIColor.Red : UIColor.Black, UIControlState.Normal);
            button.TouchUpInside += (s, e) => HandleAction(item.Tag);
            _actionStack.AddArrangedSubview(button);
        }

        var stackView = new UIStackView(new UIView[] { scrollView, _actionStack })
        {
            Axis = UILayoutConstraintAxis.Vertical,
            Spacing = 10f
        };

        _contentView.AddSubview(stackView);
        AddSubview(_contentView);
        SetupConstraints(stackView, scrollView);

        AnimateShow();
    }

    private void SetupConstraints(UIStackView stackView, UIScrollView scrollView)
    {
        _contentView.TranslatesAutoresizingMaskIntoConstraints = false;
        stackView.TranslatesAutoresizingMaskIntoConstraints = false;
        scrollView.TranslatesAutoresizingMaskIntoConstraints = false;

        _contentView.CenterXAnchor.ConstraintEqualTo(CenterXAnchor).Active = true;
        _contentView.CenterYAnchor.ConstraintEqualTo(CenterYAnchor).Active = true;
        _contentView.WidthAnchor.ConstraintEqualTo(250).Active = true;

        scrollView.TopAnchor.ConstraintEqualTo(_contentView.TopAnchor, 10).Active = true;
        scrollView.LeadingAnchor.ConstraintEqualTo(_contentView.LeadingAnchor, 10).Active = true;
        scrollView.TrailingAnchor.ConstraintEqualTo(_contentView.TrailingAnchor, -10).Active = true;
        scrollView.HeightAnchor.ConstraintEqualTo(40).Active = true; // Adjust height for emoji row

        stackView.TopAnchor.ConstraintEqualTo(scrollView.BottomAnchor, 10).Active = true;
        stackView.LeadingAnchor.ConstraintEqualTo(_contentView.LeadingAnchor, 10).Active = true;
        stackView.TrailingAnchor.ConstraintEqualTo(_contentView.TrailingAnchor, -10).Active = true;
        stackView.BottomAnchor.ConstraintEqualTo(_contentView.BottomAnchor, -10).Active = true;
    }

    private void AddReaction(string emoji)
    {
        if (!_message.Reactions.Any(r => r.Emoji == emoji))
        {
            _message.Reactions.Add(new ChatMessageReaction { Emoji = emoji, Count = 1 });
        }
        else
        {
            var reaction = _message.Reactions.First(r => r.Emoji == emoji);
            reaction.Count++;
        }
        _chatView.LongPressedCommand.Execute(new ContextAction { Name = "react", Message = _message, AdditionalData = new ChatMessageReaction { Emoji = emoji } });

        Dismiss();
    }

    private void HandleAction(string actionTag)
    {
        _chatView.LongPressedCommand.Execute(new ContextAction { Name = actionTag, Message = _message });

        Console.WriteLine($"Action triggered: {actionTag}");
        Dismiss();
    }

    public void Show()
    {
        UIApplication.SharedApplication.KeyWindow.AddSubview(this);
    }

    private void AnimateShow()
    {
        _blurView.Alpha = 0;
        UIView.Animate(0.3, () => _blurView.Alpha = 1);
    }

    private void Dismiss()
    {
        UIView.Animate(0.3, () => _blurView.Alpha = 0, () =>
        {
            RemoveFromSuperview();
            _onDismiss?.Invoke();
        });
    }
}
