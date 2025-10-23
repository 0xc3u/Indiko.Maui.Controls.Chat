using CoreGraphics;
using Indiko.Maui.Controls.Chat.Models;
using Microsoft.Maui.Platform;
using UIKit;

namespace Indiko.Maui.Controls.Chat.Platforms.iOS;

public class ChatContextMenuView : UIView
{
    private readonly ChatView _chatView;
    private readonly ChatMessage _message;
    private readonly UIView _messageView;
    private readonly Action _onDismiss;
    private readonly UIView _contentView;
    private readonly UIView _bubbleView;
    private readonly UILabel _messageLabel;
    private readonly UIScrollView _emojiScrollView;
    private readonly UIStackView _emojiStack;
    private readonly UIStackView _actionStack;
    private readonly UIVisualEffectView _blurView;

    public ChatContextMenuView(ChatView chatView, ChatMessage message, UIView messageView, Action onDismiss)
    {
        _chatView = chatView;
        _message = message;
        _messageView = messageView;
        _onDismiss = onDismiss;

        if(_message == null || _messageView == null)
        {
            return;
        }
        
        BackgroundColor = UIColor.Clear;
        Frame = UIScreen.MainScreen.Bounds;

        // Apply Blur Effect to Background
        _blurView = new UIVisualEffectView(UIBlurEffect.FromStyle(UIBlurEffectStyle.Dark))
        {
            TranslatesAutoresizingMaskIntoConstraints = false,
            Alpha = 0f
        };
        AddSubview(_blurView);

        // Tap gesture to dismiss the context menu
        var tapGesture = new UITapGestureRecognizer(() => DismissContextMenu());
        _blurView.AddGestureRecognizer(tapGesture);

        // Bubble View (Chat Bubble above context menu)
        _bubbleView = new UIView
        {
            BackgroundColor = _message.IsOwnMessage ? _chatView.OwnMessageBackgroundColor.ToPlatform()
                                                    : _chatView.OtherMessageBackgroundColor.ToPlatform(),
            Layer = { CornerRadius = 16f },
            ClipsToBounds = true,
            TranslatesAutoresizingMaskIntoConstraints = false
        };

        // Message Text inside the bubble
        _messageLabel = new UILabel
        {
            Text = _message.TextContent,
            Lines = 0,
            LineBreakMode = UILineBreakMode.WordWrap,
            TextAlignment = UITextAlignment.Left,
            TextColor = _message.IsOwnMessage ? _chatView.OwnMessageTextColor.ToPlatform()
                                              : _chatView.OtherMessageTextColor.ToPlatform(),
            TranslatesAutoresizingMaskIntoConstraints = false,
            Font = UIFont.SystemFontOfSize((nfloat)_chatView.MessageFontSize)
        };

        _bubbleView.AddSubview(_messageLabel);

        _contentView = new UIView
        {
            BackgroundColor = _chatView.ContextMenuBackgroundColor.ToPlatform(),
            Layer = { CornerRadius = 10f },
            ClipsToBounds = true,
            TranslatesAutoresizingMaskIntoConstraints = false
        };

        _emojiScrollView = new UIScrollView
        {
            TranslatesAutoresizingMaskIntoConstraints = false,
            ShowsHorizontalScrollIndicator = false,
            Bounces = true,
            AlwaysBounceHorizontal = true // Ensures scrolling behavior
        };

        _emojiStack = new UIStackView
        {
            Axis = UILayoutConstraintAxis.Horizontal,
            Alignment = UIStackViewAlignment.Center,
            Spacing = 8f,
            TranslatesAutoresizingMaskIntoConstraints = false
        };

        _emojiScrollView.AddSubview(_emojiStack);

        _actionStack = new UIStackView
        {
            Axis = UILayoutConstraintAxis.Vertical,
            Spacing = 0, // No spacing, dividers will handle it
            TranslatesAutoresizingMaskIntoConstraints = false
        };

        foreach (var emoji in _chatView.EmojiReactions)
        {
            var label = new UILabel
            {
                Text = emoji,
                Font = UIFont.SystemFontOfSize((float)_chatView.ContextMenuReactionFontSize),
                UserInteractionEnabled = true
            };
            var emojiTap = new UITapGestureRecognizer(() => AddReaction(emoji));
            label.AddGestureRecognizer(emojiTap);
            _emojiStack.AddArrangedSubview(label);
        }

        InvokeOnMainThread(() =>
        {
            _emojiStack.LayoutIfNeeded();
            var totalWidth = _emojiStack.ArrangedSubviews.Sum(view => view.IntrinsicContentSize.Width) 
                             + ((_emojiStack.ArrangedSubviews.Length - 1) * _emojiStack.Spacing);

            _emojiStack.WidthAnchor.ConstraintEqualTo((nfloat)Math.Max(totalWidth, _emojiScrollView.Frame.Width)).Active = true;
            _emojiScrollView.ContentSize = new CGSize(totalWidth, _emojiScrollView.Frame.Height);
        });

        _emojiStack.WidthAnchor.ConstraintGreaterThanOrEqualTo(_emojiScrollView.WidthAnchor, 1.2f).Active = true;
        _emojiScrollView.HeightAnchor.ConstraintEqualTo(50).Active = true;

        for (int i = 0; i < _chatView.ContextMenuItems.Count; i++)
        {
            var item = _chatView.ContextMenuItems[i];

            var button = new UIButton(UIButtonType.System);
            button.SetTitle(item.Name, UIControlState.Normal);
            button.SetTitleColor(item.IsDestructive ? 
                _chatView.ContextMenuDestructiveTextColor.ToPlatform() : 
                _chatView.ContextMenuTextColor.ToPlatform(), UIControlState.Normal);
            button.TouchUpInside += (s, e) => HandleAction(item.Tag, item.IsDestructive);

            _actionStack.AddArrangedSubview(button);

            if (i >= _chatView.ContextMenuItems.Count - 1) continue;
            var divider = new UIView
            {
                BackgroundColor = _chatView.ContextMenuDividerColor.ToPlatform(),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            divider.HeightAnchor.ConstraintEqualTo(_chatView.ContextMenuDividerHeight).Active = true;
            _actionStack.AddArrangedSubview(divider);
        }

        _emojiStack.WidthAnchor.ConstraintGreaterThanOrEqualTo(_emojiScrollView.WidthAnchor, 1.2f).Active = true;
        _emojiScrollView.HeightAnchor.ConstraintEqualTo(50).Active = true;
        _contentView.AddSubview(_emojiScrollView);

        var dividerView = new UIView
        {
            BackgroundColor = _chatView.ContextMenuDividerColor.ToPlatform(),
            TranslatesAutoresizingMaskIntoConstraints = false
        };
        dividerView.HeightAnchor.ConstraintEqualTo(_chatView.ContextMenuDividerHeight).Active = true;

        var stackView = new UIStackView(new UIView[] { _emojiScrollView, dividerView, _actionStack })
        {
            Axis = UILayoutConstraintAxis.Vertical,
            Spacing = 10f,
            TranslatesAutoresizingMaskIntoConstraints = false
        };

        _contentView.AddSubview(stackView);
        SetupConstraints(stackView);
        _emojiScrollView.SetNeedsLayout();
        _emojiScrollView.LayoutIfNeeded();
        AnimateShow();
    }

    private void SetupConstraints(UIStackView stackView)
{
    _blurView.Frame = UIScreen.MainScreen.Bounds;
     var maxWidth = UIScreen.MainScreen.Bounds.Width - 40;

    AddSubview(_contentView);

    _contentView.WidthAnchor.ConstraintEqualTo(maxWidth).Active = true; 
    _contentView.CenterXAnchor.ConstraintEqualTo(CenterXAnchor).Active = true;
    _contentView.CenterYAnchor.ConstraintEqualTo(CenterYAnchor, 40).Active = true; 
    AddSubview(_bubbleView);
    
    _bubbleView.WidthAnchor.ConstraintEqualTo(_contentView.WidthAnchor, 1.0f).Active = true;
    _bubbleView.CenterXAnchor.ConstraintEqualTo(CenterXAnchor).Active = true;
    _bubbleView.BottomAnchor.ConstraintEqualTo(_contentView.TopAnchor, -10).Active = true;

    _messageLabel.TopAnchor.ConstraintEqualTo(_bubbleView.TopAnchor, 10).Active = true;
    _messageLabel.LeadingAnchor.ConstraintEqualTo(_bubbleView.LeadingAnchor, 10).Active = true;
    _messageLabel.TrailingAnchor.ConstraintEqualTo(_bubbleView.TrailingAnchor, -10).Active = true;
    _messageLabel.BottomAnchor.ConstraintEqualTo(_bubbleView.BottomAnchor, -10).Active = true;

    _emojiScrollView.TopAnchor.ConstraintEqualTo(_contentView.TopAnchor, 10).Active = true;
    _emojiScrollView.LeadingAnchor.ConstraintEqualTo(_contentView.LeadingAnchor, 10).Active = true;
    _emojiScrollView.TrailingAnchor.ConstraintEqualTo(_contentView.TrailingAnchor, -10).Active = true;
    _emojiScrollView.HeightAnchor.ConstraintEqualTo(50).Active = true;
    _emojiStack.WidthAnchor.ConstraintGreaterThanOrEqualTo(_emojiScrollView.WidthAnchor).Active = true;

    stackView.TopAnchor.ConstraintEqualTo(_emojiScrollView.BottomAnchor, 10).Active = true;
    stackView.LeadingAnchor.ConstraintEqualTo(_contentView.LeadingAnchor, 10).Active = true;
    stackView.TrailingAnchor.ConstraintEqualTo(_contentView.TrailingAnchor, -10).Active = true;
    stackView.BottomAnchor.ConstraintEqualTo(_contentView.BottomAnchor, -10).Active = true;
}

    
    private void AddReaction(string emoji)
    {
      
        var existingReaction = _message.Reactions.FirstOrDefault(r => r.Emoji == emoji);
        if (existingReaction != null)
        {
            existingReaction.Count++;
        }
        else
        {
            existingReaction = new ChatMessageReaction { Emoji = emoji, Count = 1 };
            _message.Reactions.Add(existingReaction);
        }

        var index = _chatView?.Messages.IndexOf(_message);
        if (index >= 0)
        {
            // To trigger the update of the message view
            _chatView?.Messages.RemoveAt(index.Value);
            _chatView?.Messages.Insert(index.Value, _message);
        }

        _chatView?.LongPressedCommand?.Execute(new ContextAction { Name = "react", Message = _message, AdditionalData = existingReaction });

        DismissContextMenu();
    }

    
    private void HandleAction(string actionTag, bool isDestructive)
    {
        if (isDestructive)
        {
            _chatView?.LongPressedCommand?.Execute(new ContextAction { Name = actionTag, Message = _message });
            _chatView?.Messages.Remove(_message); // Remove the message from the collection
        }
        else
        {
        _chatView?.LongPressedCommand?.Execute(new ContextAction { Name = actionTag, Message = _message });
        }

        
        DismissContextMenu();
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

    private void DismissContextMenu()
    {
        UIView.Animate(0.3, () => _blurView.Alpha = 0, () =>
        {
            RemoveFromSuperview();
            _onDismiss?.Invoke();
        });
    }
}