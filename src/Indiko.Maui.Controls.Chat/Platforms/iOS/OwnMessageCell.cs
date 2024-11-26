
using CoreGraphics;
using Foundation;
using Indiko.Maui.Controls.Chat.Models;
using Microsoft.Maui.Platform;
using UIKit;

namespace Indiko.Maui.Controls.Chat.Platforms.iOS;

public class OwnMessageCell : UICollectionViewCell
{
    public static readonly NSString Key = new(nameof(OwnMessageCell));

    private UILabel _messageLabel;
    private UIView _bubbleView;
    private UILabel _timeLabel;

    public OwnMessageCell(ObjCRuntime.NativeHandle handle) : base(handle)
    {
        SetupLayout();
    }

    private void SetupLayout()
    {
        // Nachrichtenblase (Hintergrund)
        _bubbleView = new UIView
        {
            TranslatesAutoresizingMaskIntoConstraints = false,
            BackgroundColor = UIColor.FromRGBA(113 / 255.0f, 0 / 255.0f, 223 / 255.0f, 1.0f), // Dunkles Lila
            ClipsToBounds = true
        };
        _bubbleView.Layer.CornerRadius = 16; // Abgerundete Ecken

        // Nachrichtentext
        _messageLabel = new UILabel
        {
            Lines = 0, // Mehrzeiliger Text
            LineBreakMode = UILineBreakMode.WordWrap,
            TranslatesAutoresizingMaskIntoConstraints = false,
            TextAlignment = UITextAlignment.Left, // Kann angepasst werden
            TextColor = UIColor.White // Weißer Text für Kontrast
        };

        // Zeitstempel
        _timeLabel = new UILabel
        {
            Font = UIFont.SystemFontOfSize(12),
            TextColor = UIColor.LightGray,
            TranslatesAutoresizingMaskIntoConstraints = false,
            TextAlignment = UITextAlignment.Right // Rechtsbündig für eigene Nachrichten
        };

        // Hierarchische Ansicht
        ContentView.AddSubviews(_bubbleView, _messageLabel, _timeLabel);

        // Layout-Constraints
        NSLayoutConstraint.ActivateConstraints(new[]
        {
            // Nachrichtenblase
            _bubbleView.TrailingAnchor.ConstraintEqualTo(ContentView.TrailingAnchor, -10),
            _bubbleView.LeadingAnchor.ConstraintGreaterThanOrEqualTo(ContentView.LeadingAnchor, 10),
            _bubbleView.TopAnchor.ConstraintEqualTo(ContentView.TopAnchor, 10),
            _bubbleView.BottomAnchor.ConstraintEqualTo(_timeLabel.TopAnchor, -4),

            // Nachrichtentext innerhalb der Blase
            _messageLabel.TopAnchor.ConstraintEqualTo(_bubbleView.TopAnchor, 10),
            _messageLabel.BottomAnchor.ConstraintEqualTo(_bubbleView.BottomAnchor, -10),
            _messageLabel.LeadingAnchor.ConstraintEqualTo(_bubbleView.LeadingAnchor, 10),
            _messageLabel.TrailingAnchor.ConstraintEqualTo(_bubbleView.TrailingAnchor, -10),

            // Zeitstempel
            _timeLabel.TopAnchor.ConstraintEqualTo(_bubbleView.BottomAnchor, 4),
            _timeLabel.TrailingAnchor.ConstraintEqualTo(_bubbleView.TrailingAnchor),
            _timeLabel.LeadingAnchor.ConstraintEqualTo(_bubbleView.LeadingAnchor),
            _timeLabel.BottomAnchor.ConstraintEqualTo(ContentView.BottomAnchor, -10)
        });
    }

    public void Update(int index, ChatMessage message, ChatView chatView, IMauiContext mauiContext)
    {
        if (chatView == null || message == null || mauiContext == null)
        {
            return;
        }

        try
        {
            var displayMetrics = ContentView.Bounds;
            var maxWidth = (nfloat)(displayMetrics.Width * 0.65);
            _bubbleView.WidthAnchor.ConstraintLessThanOrEqualTo(maxWidth).Active = true;


            // Nachrichtentext setzen
            _messageLabel.Text = message.TextContent;

            // Blasenhintergrund und Textfarbe
            _bubbleView.BackgroundColor = chatView.OwnMessageBackgroundColor.ToPlatform();
            _messageLabel.TextColor = chatView.OwnMessageTextColor.ToPlatform();

            // Zeitstempel setzen
            _timeLabel.Text = message.Timestamp.ToString("HH:mm");

            SetNeedsLayout();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in {nameof(OwnMessageCell)}.{nameof(Update)}: {ex.Message}");
        }
    }
}



//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Foundation;
//using Indiko.Maui.Controls.Chat.Models;
//using Microsoft.Maui.Platform;
//using UIKit;

//namespace Indiko.Maui.Controls.Chat.Platforms.iOS;
//public class OwnMessageCell : UICollectionViewCell
//{
//    public static readonly NSString Key = new(nameof(OwnMessageCell));

//    private UILabel _messageLabel;
//    private UIImageView _avatarImageView;

//    public OwnMessageCell(ObjCRuntime.NativeHandle handle) : base(handle)
//    {
//        SetupLayout();
//    }

//    private void SetupLayout()
//    {
//        _avatarImageView = new UIImageView
//        {
//            TranslatesAutoresizingMaskIntoConstraints = false,
//            ContentMode = UIViewContentMode.ScaleAspectFill,
//            ClipsToBounds = true
//        };
//        _avatarImageView.Layer.CornerRadius = 20;

//        _messageLabel = new UILabel
//        {
//            Lines = 0,
//            LineBreakMode = UILineBreakMode.WordWrap,
//            TextAlignment = UITextAlignment.Left,
//            LineBreakStrategy = NSLineBreakStrategy.HangulWordPriority,
//            BackgroundColor = UIColor.FromRGBA(0.8f, 0.9f, 1.0f, 1.0f),
//            TranslatesAutoresizingMaskIntoConstraints = false
//        };

//        ContentView.AddSubviews(_messageLabel, _avatarImageView);

//        NSLayoutConstraint.ActivateConstraints(new[]
//        {
//            _avatarImageView.TrailingAnchor.ConstraintEqualTo(ContentView.TrailingAnchor, -10),
//            _avatarImageView.TopAnchor.ConstraintEqualTo(ContentView.TopAnchor, 10),
//            _avatarImageView.WidthAnchor.ConstraintEqualTo(40),
//            _avatarImageView.HeightAnchor.ConstraintEqualTo(40),

//            _messageLabel.TrailingAnchor.ConstraintEqualTo(_avatarImageView.LeadingAnchor, -10),
//            _messageLabel.LeadingAnchor.ConstraintGreaterThanOrEqualTo(ContentView.LeadingAnchor, 50),
//            _messageLabel.TopAnchor.ConstraintEqualTo(ContentView.TopAnchor, 10),
//            _messageLabel.BottomAnchor.ConstraintEqualTo(ContentView.BottomAnchor, -10)
//        });
//    }

//    public void Update(int index, ChatMessage message, ChatView chatView, IMauiContext mauiContext)
//    {
//        if (chatView == null || message == null || mauiContext == null)
//        {
//            return;
//        }

//        try
//        {

//            _messageLabel.Text = message.TextContent;
//            _messageLabel.TextColor = chatView.OwnMessageTextColor.ToPlatform();
//            _messageLabel.BackgroundColor = chatView.OwnMessageBackgroundColor.ToPlatform();

//            if (message.SenderAvatar != null)
//            {
//                _avatarImageView.Image = UIImage.LoadFromData(NSData.FromArray(message.SenderAvatar));
//            }
//            else
//            {
//                if (string.IsNullOrEmpty(message.SenderInitials))
//                {
//                    _avatarImageView.Image = UIImageExtensions.CreateInitialsImage(message.SenderId.Trim().Substring(0, 2).ToUpperInvariant(), 40f, 40f,
//                        chatView.AvatarTextColor.ToPlatform(), chatView.AvatarBackgroundColor.ToCGColor());
//                }
//                else
//                {
//                    _avatarImageView.Image = UIImageExtensions.CreateInitialsImage(message.SenderInitials, 40f, 40f,
//                    chatView.AvatarTextColor.ToPlatform(), chatView.AvatarBackgroundColor.ToCGColor());
//                }
//            }


//            SetNeedsLayout();
//        }
//        catch (Exception ex)
//        {
//            Console.WriteLine($"Error in {nameof(OwnMessageCell)}.{nameof(Update)}: {ex.Message}");
//        }
//    }
//}
