using CoreGraphics;
using Foundation;
using Indiko.Maui.Controls.Chat.Models;
using Microsoft.Maui.Platform;
using UIKit;

namespace Indiko.Maui.Controls.Chat.Platforms.iOS;

public class ChatMessageCell : UICollectionViewCell
{
    public static readonly NSString Key = new NSString(nameof(ChatMessageCell));

    private UILabel _textLabel;
    private UIImageView _avatarImageView;
    private UILabel _timestampLabel;
    private UIView _bubbleBackground;

    public ChatMessageCell(IntPtr handle) : base(handle)
    {
        InitializeViews();
    }

    private void InitializeViews()
    {
        _bubbleBackground = new UIView
        {
            Layer = { CornerRadius = 16 },
            ClipsToBounds = true
        };

        _textLabel = new UILabel
        {
            Lines = 0
        };

        _avatarImageView = new UIImageView
        {
            Layer = { CornerRadius = 18 },
            ClipsToBounds = true,
            ContentMode = UIViewContentMode.ScaleAspectFill
        };

        _timestampLabel = new UILabel
        {
            Font = UIFont.SystemFontOfSize(10),
            TextColor = UIColor.Gray
        };

        ContentView.AddSubviews(_bubbleBackground, _textLabel, _avatarImageView, _timestampLabel);
    }

    public void Update(ChatMessage message, ChatView chatView)
    {
        _textLabel.Text = message.TextContent;
        _textLabel.TextColor = message.IsOwnMessage ? chatView.OwnMessageTextColor.ToPlatform() : chatView.OtherMessageTextColor.ToPlatform();

        _bubbleBackground.BackgroundColor = message.IsOwnMessage ? chatView.OwnMessageBackgroundColor.ToPlatform() : chatView.OtherMessageBackgroundColor.ToPlatform();

        if (message.SenderAvatar != null)
        {
            _avatarImageView.Image = UIImage.LoadFromData(NSData.FromArray(message.SenderAvatar));
            _avatarImageView.Hidden = false;
        }
        else
        {
            _avatarImageView.Hidden = true;
        }

        _timestampLabel.Text = message.Timestamp.ToString("HH:mm");

        // Layout dynamically based on ownership
        LayoutSubviews(message.IsOwnMessage);
    }

    private void LayoutSubviews(bool isOwnMessage)
    {
        var padding = 10;
        var bubbleWidth = ContentView.Frame.Width * 0.65;

        if (isOwnMessage)
        {
            _avatarImageView.Hidden = true;
            _bubbleBackground.Frame = new CGRect(ContentView.Frame.Width - bubbleWidth - padding, padding, bubbleWidth, _textLabel.IntrinsicContentSize.Height + padding * 2);
        }
        else
        {
            _avatarImageView.Hidden = false;
            _avatarImageView.Frame = new CGRect(padding, padding, 36, 36);
            _bubbleBackground.Frame = new CGRect(_avatarImageView.Frame.Right + padding, padding, bubbleWidth, _textLabel.IntrinsicContentSize.Height + padding * 2);
        }

        _textLabel.Frame = new CGRect(_bubbleBackground.Frame.Left + padding, _bubbleBackground.Frame.Top + padding, bubbleWidth - padding * 2, _textLabel.IntrinsicContentSize.Height);

        _timestampLabel.Frame = new CGRect(_bubbleBackground.Frame.Left, _bubbleBackground.Frame.Bottom + 5, bubbleWidth, 15);
    }
}
