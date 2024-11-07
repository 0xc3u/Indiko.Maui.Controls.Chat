using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Indiko.Maui.Controls.Chat.Models;

namespace Indiko.Maui.Controls.Chat.Sample.ViewModels;
public partial class MainPageViewModel : BaseViewModel
{

    [ObservableProperty]
    ObservableCollection<ChatMessage> chatMessages;


    public override void OnAppearing(object param)
	{


        List<ChatMessage> messages = new List<ChatMessage>();
        DateTime timestamp = DateTime.Now.AddDays(-3);

        // Actors
        var actors = new[]
        {
            new { Name = "Alex", IsOwnMessage = true, SenderInitials="AL" },
            new { Name = "Sam", IsOwnMessage = false,  SenderInitials="SM" },
            new { Name = "Jordan", IsOwnMessage = false, SenderInitials ="JO" }
        };

        // Conversation messages
        var messageTemplates = new[]
        {
            "Hey everyone, are we still on for the mountain hike tomorrow?",
            "Yes, I’m all packed up and ready! Can't wait for the adventure.",
            "Same here! I checked the weather forecast; it looks perfect for a hike.",
            "Awesome, what time are we meeting up?",
            "How about 7 AM? We want to make the most of the day.",
            "7 AM sounds great to me. Are we bringing lunch or just snacks?",
            "I think we should pack some sandwiches. It might be a long day.",
            "Good idea! I’ll bring some energy bars and fruit as well.",
            "I’ll take care of the sandwiches then. Any preference for fillings?",
            "Turkey and cheese would be perfect for me, thanks!",
            "I’m good with anything vegetarian if that’s okay.",
            "Got it, one turkey and cheese, and one vegetarian. I’ll sort it out.",
            "By the way, did we finalize the trail we’re taking?",
            "I was thinking we could start with the North Ridge trail, then loop back via the river.",
            "That route sounds amazing! I’ve heard the views are breathtaking.",
            "Yeah, and we’ll get to see the waterfall halfway through.",
            "Perfect! Just make sure to bring your cameras.",
            "Oh, definitely! I’m hoping to capture some wildlife shots too.",
            "Same here, but I hope we don’t run into any bears!",
            "Haha, let's hope not! But I think we’ll be safe if we stick together.",
            "True, and I’m bringing a whistle just in case.",
            "Great thinking! Better safe than sorry.",
            "What about water? How many bottles are you guys bringing?",
            "I’m packing two big bottles and a water filter just in case.",
            "Smart move. I’ll bring a hydration pack and some electrolyte tablets.",
            "Sounds like we’re all set then! Are we carpooling?",
            "Yes, I can drive if you both want to chip in for gas.",
            "Thanks, Alex! I’m in for carpooling and happy to pitch in.",
            "Same here, much appreciated! I’ll bring some road snacks too.",
            "Alright, see you both tomorrow bright and early!",
            "Looking forward to it! Let’s make it an unforgettable hike."
        };

        // Generate messages
        for (int i = 0; i < 30; i++)
        {

            MessageReadState messageReadState = MessageReadState.Read;

            if (i > 20)
            {
                messageReadState = MessageReadState.New;
            }

            var actor = actors[i % 3];
            messages.Add(new ChatMessage
            {
                TextContent = messageTemplates[i],
                IsOwnMessage = actor.IsOwnMessage,
                Timestamp = timestamp.AddHours(i * 5),
                SenderAvatar = null,
                SenderInitials = actor.SenderInitials,
                MessageId = Guid.NewGuid().ToString(),
                MessageType = MessageType.Text,
                ReadState = messageReadState
            });
        }

        ChatMessages = new ObservableCollection<ChatMessage>(messages);

    }
}
