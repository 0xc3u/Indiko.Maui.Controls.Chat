using System.Runtime.InteropServices;
using CoreGraphics;
using Foundation;
using Indiko.Maui.Controls.Chat.Models;
using Microsoft.Maui.Handlers;
using UIKit;

namespace Indiko.Maui.Controls.Chat.Platforms.iOS;

public class ChatViewHandler : ViewHandler<ChatView, UICollectionView>
{
    public static CommandMapper<ChatView, ChatViewHandler> CommandMapper = new CommandMapper<ChatView, ChatViewHandler>()
    {
        [nameof(ChatView.LoadMoreMessagesCommand)] = MapCommands,
        [nameof(ChatView.MessageTappedCommand)] = MapCommands,
        [nameof(ChatView.ScrolledCommand)] = MapCommands
    };

    public static IPropertyMapper<ChatView, ChatViewHandler> PropertyMapper = new PropertyMapper<ChatView, ChatViewHandler>(ViewHandler.ViewMapper)
    {
        [nameof(ChatView.Messages)] = MapProperties,
        [nameof(ChatView.OwnMessageBackgroundColor)] = MapProperties,
        [nameof(ChatView.OtherMessageBackgroundColor)] = MapProperties,
        [nameof(ChatView.DateTextColor)] = MapProperties,
        [nameof(ChatView.MessageTimeTextColor)] = MapProperties,
        [nameof(ChatView.NewMessagesSeperatorTextColor)] = MapProperties,
        [nameof(ChatView.MessageFontSize)] = MapProperties,
        [nameof(ChatView.DateTextFontSize)] = MapProperties,
        [nameof(ChatView.MessageTimeFontSize)] = MapProperties,
        [nameof(ChatView.NewMessagesSeperatorFontSize)] = MapProperties,
        [nameof(ChatView.AvatarSize)] = MapProperties,
        [nameof(ChatView.ScrollToFirstNewMessage)] = MapProperties,
        [nameof(ChatView.AvatarBackgroundColor)] = MapProperties,
        [nameof(ChatView.AvatarTextColor)] = MapProperties,
    };

    public ChatViewHandler() : base(PropertyMapper, CommandMapper)
    {
    }

    private ChatMessageCollectionViewSource _dataSource;

    protected override UICollectionView CreatePlatformView()
    {
        // Create UICollectionViewCompositionalLayout
        var layout = CreateCompositionalLayout();

        var collectionView = new UICollectionView(CGRect.Empty, layout)
        {
            BackgroundColor = UIColor.Clear
        };

        _dataSource = new ChatMessageCollectionViewSource(VirtualView);
        collectionView.DataSource = _dataSource;
        collectionView.Delegate = _dataSource;

        collectionView.RegisterClassForCell(typeof(ChatMessageCell), ChatMessageCell.Key);

        return collectionView;
    }

    private UICollectionViewCompositionalLayout CreateCompositionalLayout()
    {
        return new UICollectionViewCompositionalLayout((sectionIndex, layoutEnvironment) =>
        {
            // Define the item size
            var itemSize = NSCollectionLayoutSize.Create(
                NSCollectionLayoutDimension.CreateFractionalWidth((NFloat)1.0), // Full width
                NSCollectionLayoutDimension.CreateEstimated((NFloat)100)); // Estimated height for dynamic content

            // Create the item
            var item = NSCollectionLayoutItem.Create(itemSize);

            // Define the group size (one item per group in a vertical list)
            var groupSize = NSCollectionLayoutSize.Create(
                NSCollectionLayoutDimension.CreateFractionalWidth((NFloat)1.0), // Full width
                NSCollectionLayoutDimension.CreateEstimated((NFloat)100)); // Matches item height dynamically

            // Create the group
            var group = NSCollectionLayoutGroup.CreateVertical(groupSize, new[] { item });

            // Define the section
            var section = NSCollectionLayoutSection.Create(group);

            // Set spacing and insets
            section.InterGroupSpacing = new NFloat(2); // Space between messages
            section.ContentInsets = new NSDirectionalEdgeInsets(new NFloat(5), new NFloat(5), new NFloat(5), new NFloat(5)); // Padding for the section

            return section;
        });
    }




    private static void MapProperties(ChatViewHandler handler, ChatView chatView)
    {
        handler.UpdateMessages();
    }

    private static void MapCommands(ChatViewHandler handler, ChatView chatView, object? args)
    {
        // Map command logic here if needed
    }

    private void UpdateMessages()
    {
        if (PlatformView != null)
        {
            _dataSource.UpdateMessages(VirtualView.Messages);
            PlatformView.ReloadData();

            if (VirtualView.ScrollToFirstNewMessage)
            {
                ScrollToFirstNewMessage();
            }
        }
    }

    private void ScrollToFirstNewMessage()
    {
        var index = VirtualView.Messages.TakeWhile(m => m.ReadState != MessageReadState.New).Count();
        if (index < VirtualView.Messages.Count)
        {
            PlatformView.ScrollToItem(NSIndexPath.FromRowSection(index, 0), UICollectionViewScrollPosition.Top, true);
        }
    }
}
