using Indiko.Maui.Controls.Chat.Models;
using Microsoft.Maui.Platform;
using UIKit;

namespace Indiko.Maui.Controls.Chat.Platforms.iOS;
public static class EmojiHelper
{
    public static void UpdateReactions(this UIStackView reactionsStackView, List<ChatMessageReaction> reactions, ChatView chatView)
    {
        if(reactionsStackView == null || reactions == null || chatView == null)
        {
            return;
        }

        // remove all existing emoji reactions
        foreach (var view in reactionsStackView.ArrangedSubviews)
        {
            view.RemoveFromSuperview();
        }

        // add emoji reactions
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
        reactionsStackView.Hidden = reactions.Count == 0;
    }
}