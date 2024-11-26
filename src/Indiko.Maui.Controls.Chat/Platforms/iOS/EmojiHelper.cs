using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Indiko.Maui.Controls.Chat.Models;
using Microsoft.Maui.Platform;
using UIKit;

namespace Indiko.Maui.Controls.Chat.Platforms.iOS;
public static class EmojiHelper
{
    public static void UpdateReactions(UIStackView reactionsStackView, List<ChatMessageReaction> reactions, ChatView chatView)
    {
        // Entferne alle vorhandenen Reaktionen
        foreach (var view in reactionsStackView.ArrangedSubviews)
        {
            view.RemoveFromSuperview();
        }

        // Füge neue Reaktionen hinzu
        foreach (var reaction in reactions)
        {
            var reactionLabel = new UILabel
            {
                Text = $"{reaction.Emoji} {reaction.Count}",
                Font = UIFont.SystemFontOfSize(chatView.EmojiReactionFontSize),
                TextColor = chatView.EmojiReactionTextColor.ToPlatform()
            };

            reactionsStackView.AddArrangedSubview(reactionLabel);
        }

        reactionsStackView.Hidden = reactions.Count == 0; // Verstecke, falls keine Reaktionen vorhanden sind
    }
}
