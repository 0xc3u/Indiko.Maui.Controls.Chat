﻿using CoreGraphics;
using Foundation;
using Indiko.Maui.Controls.Chat.Models;
using Microsoft.Maui.Platform;
using UIKit;

namespace Indiko.Maui.Controls.Chat.Platforms.iOS;

public class OtherMessageCell : UICollectionViewCell
{
    public static readonly NSString Key = new(nameof(OtherMessageCell));

    private UILabel _messageLabel;
    private UIImageView _avatarImageView;
    private UIView _bubbleView;
    private UILabel _timeLabel;

    public OtherMessageCell(ObjCRuntime.NativeHandle handle) : base(handle)
    {
        SetupLayout();
    }

    private void SetupLayout()
    {
        // Avatar-Setup
        _avatarImageView = new UIImageView
        {
            TranslatesAutoresizingMaskIntoConstraints = false,
            ContentMode = UIViewContentMode.ScaleAspectFill,
            ClipsToBounds = true
        };
        _avatarImageView.Layer.CornerRadius = 20; // Runde Form für 40x40 Größe

        // Nachrichtenblase (Hintergrund)
        _bubbleView = new UIView
        {
            TranslatesAutoresizingMaskIntoConstraints = false,
            BackgroundColor = UIColor.FromRGBA(230 / 255.0f, 223 / 255.0f, 255 / 255.0f, 1.0f), // Helles Lila
            ClipsToBounds = true
        };
        _bubbleView.Layer.CornerRadius = 16; // Abgerundete Ecken

        // Nachrichtentext
        _messageLabel = new UILabel
        {
            Lines = 0, // Mehrzeiliger Text
            LineBreakMode = UILineBreakMode.WordWrap,
            TranslatesAutoresizingMaskIntoConstraints = false,
            TextAlignment = UITextAlignment.Left,
            TextColor = UIColor.Black
        };

        // Zeitstempel
        _timeLabel = new UILabel
        {
            Font = UIFont.SystemFontOfSize(12),
            TextColor = UIColor.LightGray,
            TranslatesAutoresizingMaskIntoConstraints = false,
            TextAlignment = UITextAlignment.Left // Zeitstempel links ausrichten
        };

        // Hierarchische Ansicht
        ContentView.AddSubviews(_avatarImageView, _bubbleView, _messageLabel, _timeLabel);

        // Layout-Constraints
        NSLayoutConstraint.ActivateConstraints(new[]
        {
            // Avatar
            _avatarImageView.LeadingAnchor.ConstraintEqualTo(ContentView.LeadingAnchor, 10),
            _avatarImageView.TopAnchor.ConstraintEqualTo(ContentView.TopAnchor, 10),
            _avatarImageView.WidthAnchor.ConstraintEqualTo(40),
            _avatarImageView.HeightAnchor.ConstraintEqualTo(40),

            // Nachrichtenblase
            _bubbleView.LeadingAnchor.ConstraintEqualTo(_avatarImageView.TrailingAnchor, 10),
            _bubbleView.TrailingAnchor.ConstraintLessThanOrEqualTo(ContentView.TrailingAnchor, -50),
            _bubbleView.TopAnchor.ConstraintEqualTo(ContentView.TopAnchor, 10),
            _bubbleView.BottomAnchor.ConstraintEqualTo(_timeLabel.TopAnchor, -4),

            // Nachrichtentext innerhalb der Blase
            _messageLabel.TopAnchor.ConstraintEqualTo(_bubbleView.TopAnchor, 10),
            _messageLabel.BottomAnchor.ConstraintEqualTo(_bubbleView.BottomAnchor, -10),
            _messageLabel.LeadingAnchor.ConstraintEqualTo(_bubbleView.LeadingAnchor, 10),
            _messageLabel.TrailingAnchor.ConstraintEqualTo(_bubbleView.TrailingAnchor, -10),

            // Zeitstempel
            _timeLabel.TopAnchor.ConstraintEqualTo(_bubbleView.BottomAnchor, 4),
            _timeLabel.LeadingAnchor.ConstraintEqualTo(_bubbleView.LeadingAnchor),
            _timeLabel.TrailingAnchor.ConstraintEqualTo(_bubbleView.TrailingAnchor),
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
            _bubbleView.BackgroundColor = chatView.OtherMessageBackgroundColor.ToPlatform();
            _messageLabel.TextColor = chatView.OtherMessageTextColor.ToPlatform();

            // Zeitstempel setzen
            _timeLabel.Text = message.Timestamp.ToString("HH:mm");

            // Avatar setzen
            if (message.SenderAvatar != null)
            {
                _avatarImageView.Image = UIImage.LoadFromData(NSData.FromArray(message.SenderAvatar));
            }
            else
            {
                if (string.IsNullOrEmpty(message.SenderInitials))
                {
                    // Initialen anzeigen, falls kein Avatar verfügbar
                    _avatarImageView.Image = UIImageExtensions.CreateInitialsImage(message.SenderId.Trim().Substring(0, 2).ToUpperInvariant(), 40f, 40f,
                        chatView.AvatarTextColor.ToPlatform(), chatView.AvatarBackgroundColor.ToCGColor());
                }
                else
                {
                    _avatarImageView.Image = UIImageExtensions.CreateInitialsImage(message.SenderInitials, 40f, 40f,
                        chatView.AvatarTextColor.ToPlatform(), chatView.AvatarBackgroundColor.ToCGColor());
                }
            }

            SetNeedsLayout();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in {nameof(OtherMessageCell)}.{nameof(Update)}: {ex.Message}");
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
//using Microsoft.Maui.Controls;
//using Microsoft.Maui.Platform;
//using UIKit;

//namespace Indiko.Maui.Controls.Chat.Platforms.iOS;
//public class OtherMessageCell : UICollectionViewCell
//{
//    public static readonly NSString Key = new(nameof(OtherMessageCell));
//    private UILabel _messageLabel;
//    private UIImageView _avatarImageView;

//    public OtherMessageCell(ObjCRuntime.NativeHandle handle) : base(handle)
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
//            LineBreakStrategy = NSLineBreakStrategy.HangulWordPriority,
//            TextAlignment = UITextAlignment.Left,
//            BackgroundColor = UIColor.FromRGBA(0.9f, 0.9f, 0.9f, 1.0f),
//            TranslatesAutoresizingMaskIntoConstraints = false
//        };

//        ContentView.AddSubviews(_messageLabel, _avatarImageView);

//        NSLayoutConstraint.ActivateConstraints(new[]
//        {
//            _avatarImageView.LeadingAnchor.ConstraintEqualTo(ContentView.LeadingAnchor, 10),
//            _avatarImageView.TopAnchor.ConstraintEqualTo(ContentView.TopAnchor, 10),
//            _avatarImageView.WidthAnchor.ConstraintEqualTo(40),
//            _avatarImageView.HeightAnchor.ConstraintEqualTo(40),

//            _messageLabel.LeadingAnchor.ConstraintEqualTo(_avatarImageView.TrailingAnchor, 10),
//            _messageLabel.TrailingAnchor.ConstraintLessThanOrEqualTo(ContentView.TrailingAnchor, -50),
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
//            _messageLabel.TextColor = chatView.OtherMessageTextColor.ToPlatform();
//            _messageLabel.BackgroundColor = chatView.OtherMessageBackgroundColor.ToPlatform();

//            if (message.SenderAvatar != null)
//            {
//                _avatarImageView.Image = UIImage.LoadFromData(NSData.FromArray(message.SenderAvatar));
//            }
//            else
//            {
//                if (string.IsNullOrEmpty(message.SenderInitials))
//                {
//                    _avatarImageView.Image = UIImageExtensions.CreateInitialsImage(message.SenderId.Trim().Substring(0,2).ToUpperInvariant(), 40f, 40f,
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
//        catch(Exception ex)
//        {
//            Console.WriteLine($"Error in {nameof(OtherMessageCell)}.{nameof(Update)}: {ex.Message}");
//        }
//    }
//}
