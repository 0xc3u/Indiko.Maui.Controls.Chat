using Indiko.Maui.Controls.Chat.Models;
using Indiko.Maui.Controls.Chat.Sample.Models;
using Indiko.Maui.Controls.Chat.Sample.Utils;

namespace Indiko.Maui.Controls.Chat.Sample.Services;

public interface IMessageService
{
    Task<List<ChatMessage>> GetMessagesAsync(DateTime? from, DateTime? until);
}

public class MessageService : IMessageService
{
    private readonly List<User> _userList;
    private readonly List<string> _imageList;

    public MessageService()
    {
        var availableImages = EmbeddedResourceHelper.GetAvailableImages().ToList();
        var hikerImages = availableImages.Where(e => e.Contains("hiker")).ToArray();
        _userList =
        [
            new User
            {
                Avatar = EmbeddedResourceHelper.GetBytes(availableImages[0]),
                Name = "Alex Crowford",
                Initials = "AC",
                IsOwnMessage = false
            },
            new User
            {
                Avatar = EmbeddedResourceHelper.GetBytes(availableImages[1]),
                Name = "Sam Maxwell",
                Initials = "SM",
                IsOwnMessage = false,
            },
            new User
            {
                Avatar = EmbeddedResourceHelper.GetBytes(availableImages[2]),
                Name = "Michael Fitch",
                Initials = "MF",
                IsOwnMessage = false
            },
            new User
            {
                Avatar = EmbeddedResourceHelper.GetBytes(availableImages[3]),
                Name = "Mara Mc.Kellogs",
                Initials = "MK",
                IsOwnMessage = true
            }
        ];

        _imageList = [.. availableImages];
    }

    public Task<List<ChatMessage>> GetMessagesAsync(DateTime? from, DateTime? until)
    {

        List<ChatMessage> messages = [];

        // The sample video ships as an embedded resource (loaded synchronously) instead of an
        // inline base64 blob.
        var videoBytes = EmbeddedResourceHelper.GetBytesByFileName("sample_video.mp4");

        var mountainImages = _imageList.Where(e => e.Contains("mountain")).ToArray();

        DateTime timestamp = from.Value.AddDays(-3);

        string[] messageTemplates =
        [
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
            "Looking forward to it! Let’s make it an unforgettable hike.",
        ];

        DateTime? lastDateAdded = null;
        int min = 0;
        for (int i = 0; i < messageTemplates.Length; i++)
        {

            MessageReadState messageReadState = MessageReadState.Read;
            MessageDeliveryState messageDeliveryState = MessageDeliveryState.Read;

            timestamp = timestamp.AddMinutes(min);

            if (i > 28)
            {
                messageReadState = MessageReadState.New;
                messageDeliveryState = MessageDeliveryState.Sent;
            }
            var actor = _userList[i % 4];

            ChatMessage chatMessage = new()
            {
                TextContent = messageTemplates[i],
                IsOwnMessage = actor.IsOwnMessage,
                Timestamp = timestamp,
                SenderAvatar = actor.Avatar,
                SenderInitials = actor.Initials,
                SenderName = actor.IsOwnMessage ? null : actor.Name, // group-chat sender name above incoming bubbles
                MessageId = Guid.NewGuid().ToString(),
                MessageType = MessageType.Text,
                ReadState = messageReadState,
                DeliveryState = messageDeliveryState,
            };


            // Add emoji reactions to specific messages
            if (i == 1)
            {
                chatMessage.Reactions.Add(new ChatMessageReaction
                {
                    Emoji = "😊",
                    Count = 3,
                    ParticipantIds = ["user1", "user2", "user3"]
                });
                chatMessage.Reactions.Add(new ChatMessageReaction
                {
                    Emoji = "❤️",
                    Count = 2,
                    ParticipantIds = ["user4", "user5"]
                });
            }
            else if (i == 2)
            {
                chatMessage.Reactions.Add(new ChatMessageReaction
                {
                    Emoji = "😂",
                    Count = 4,
                    ParticipantIds = new List<string> { "user1", "user2", "user3" }
                });
                chatMessage.Reactions.Add(new ChatMessageReaction
                {
                    Emoji = "❤️",
                    Count = 10,
                    ParticipantIds = new List<string> { "user4", "user5" }
                });
                chatMessage.Reactions.Add(new ChatMessageReaction
                {
                    Emoji = "👍",
                    Count = 4,
                    ParticipantIds = new List<string> { "user2" }
                });
            }
            else if (i == 15)
            {
                chatMessage.Reactions.Add(new ChatMessageReaction
                {
                    Emoji = "👍",
                    Count = 1,
                    ParticipantIds = new List<string> { "user2" }
                });
            }

            // Add replies for specific messages
            if (i == 5)
            {
                chatMessage.ReplyToMessage = new RepliedMessage
                {
                    MessageId = messages[4].MessageId,
                    TextPreview = RepliedMessage.GenerateTextPreview(messages[4].TextContent),
                    SenderId = messages[4].SenderInitials
                };
            }
            else if (i == 11)
            {
                chatMessage.ReplyToMessage = new RepliedMessage
                {
                    MessageId = messages[9].MessageId,
                    TextPreview = RepliedMessage.GenerateTextPreview(messages[9].TextContent),
                    SenderId = messages[9].SenderInitials
                };

                chatMessage.Reactions.Add(new ChatMessageReaction
                {
                    Emoji = "👍",
                    Count = 2,
                    ParticipantIds = new List<string> { "user2", "user3" }
                });
            }

            else if (i == 16)
            {
                chatMessage.Reactions.Add(new ChatMessageReaction
                {
                    Emoji = "😊",
                    Count = 1,
                    ParticipantIds = new List<string> { "user1", }
                });
                chatMessage.Reactions.Add(new ChatMessageReaction
                {
                    Emoji = "❤️",
                    Count = 2,
                    ParticipantIds = new List<string> { "user4", "user5" }
                });
            }

            messages.Add(chatMessage);
            min += 15;
        }

        ChatMessage imageMessage = new()
        {
            BinaryContent = EmbeddedResourceHelper.GetBytes(mountainImages[0]),
            TextContent = "Nice picture, isn't it?",
            IsOwnMessage = false,
            Timestamp = messages[14].Timestamp.AddMinutes(-1),
            SenderAvatar = _userList[1].Avatar,
            SenderInitials = _userList[1].Initials,
            SenderId = Guid.NewGuid().ToString(),
            MessageId = Guid.NewGuid().ToString(),
            MessageType = MessageType.Image,
            ReadState = MessageReadState.Read,
            DeliveryState = MessageDeliveryState.Read,
        };
        imageMessage.Reactions.Add(new ChatMessageReaction
        {
            Emoji = "❤️",
            Count = 3,
            ParticipantIds = ["user4", "user5", "user6"]
        });
        messages.Insert(14, imageMessage);

        ChatMessage videoMessage = new()
        {
            BinaryContent = videoBytes,
            IsOwnMessage = true,
            Timestamp = messages[^1].Timestamp.AddMinutes(5),
            SenderAvatar = null,
            SenderInitials = "AS",
            MessageId = Guid.NewGuid().ToString(),
            MessageType = MessageType.Video,
            ReadState = MessageReadState.New,
            DeliveryState = MessageDeliveryState.Delivered,
        };
        videoMessage.Reactions.Add(new ChatMessageReaction
        {
            Emoji = "😊",
            Count = 5,
            ParticipantIds = ["user4", "user5"]
        });

        messages.Add(videoMessage);

        // Voice note (audio) message — shows the play/pause + waveform + duration cell.
        var (voiceBytes, voiceDuration) = Utils.SampleAudioGenerator.GenerateWav(seconds: 4, frequency: 420);
        messages.Add(new ChatMessage
        {
            BinaryContent = voiceBytes,
            AudioDuration = voiceDuration,
            IsOwnMessage = false,
            Timestamp = messages[^1].Timestamp.AddMinutes(2),
            SenderAvatar = _userList[0].Avatar,
            SenderInitials = _userList[0].Initials,
            SenderName = _userList[0].Name,
            MessageId = Guid.NewGuid().ToString(),
            MessageType = MessageType.Audio,
            ReadState = MessageReadState.New,
            DeliveryState = MessageDeliveryState.Delivered,
        });

        // Text message with a link-preview card (the app supplies the unfurled data).
        messages.Add(new ChatMessage
        {
            TextContent = "Found a great trail guide: https://learn.microsoft.com/dotnet/maui/",
            IsOwnMessage = false,
            Timestamp = messages[^1].Timestamp.AddMinutes(3),
            SenderAvatar = _userList[2].Avatar,
            SenderInitials = _userList[2].Initials,
            SenderName = _userList[2].Name,
            MessageId = Guid.NewGuid().ToString(),
            MessageType = MessageType.Text,
            ReadState = MessageReadState.New,
            DeliveryState = MessageDeliveryState.Delivered,
            LinkPreview = new LinkPreview
            {
                Url = "https://learn.microsoft.com/dotnet/maui/",
                SiteName = "learn.microsoft.com",
                Title = ".NET MAUI documentation",
                Description = "Build cross-platform native apps for Android, iOS, macOS and Windows from one C# codebase.",
                ImageBytes = Utils.SampleImageGenerator.GenerateBmp(width: 320, height: 180, r: 0x51, g: 0x2B, b: 0xD4),
            },
        });


        var systemMessage = new ChatMessage()
        {
            DeliveryState = MessageDeliveryState.Delivered,
            IsOwnMessage = false,
            MessageId = Guid.NewGuid().ToString("N"),
            MessageType = MessageType.System,
            Reactions = [],
            TextContent = "This is a message from the system."
        };

        messages.Insert(15, systemMessage);

        //// insert date separators
        for (int i = 0; i < messages.OrderBy(e => e.Timestamp).ToList().Count; i++)
        {
            var message = messages[i];
            if (lastDateAdded == null || message.Timestamp.Date != lastDateAdded)
            {
                messages.Insert(i, new ChatMessage
                {
                    DeliveryState = MessageDeliveryState.Delivered,
                    IsOwnMessage = false,
                    MessageId = Guid.NewGuid().ToString(),
                    MessageType = MessageType.Date,
                    Reactions = [],
                    TextContent = message.Timestamp.ToString("dddd, MMMM d, yyyy"),
                    Timestamp = message.Timestamp.Date,
                });
                lastDateAdded = message.Timestamp.Date;
                i++;
            }
        }


        return Task.FromResult(messages);
    }
}
