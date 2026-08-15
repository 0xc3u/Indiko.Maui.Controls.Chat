using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Indiko.Maui.Controls.Chat.Models;
using Microsoft.Maui;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using Windows.Media.Core;
using ScrollBarVisibility = Microsoft.UI.Xaml.Controls.ScrollBarVisibility;
using Visibility = Microsoft.UI.Xaml.Visibility;

namespace Indiko.Maui.Controls.Chat.Platforms.Windows;

/// <summary>
/// Maps the cross-platform <see cref="ChatView"/> to a WinUI <see cref="ListView"/> hosted in a
/// <see cref="ChatViewContainer"/> (list + floating scroll-to-bottom button). Newest message is at
/// the bottom; the ItemsStackPanel's <see cref="ItemsUpdatingScrollMode"/> is switched per
/// collection change so load-more prepends keep the viewport stable and appends follow the
/// conversation when the user is near the newest message.
/// </summary>
public class ChatViewHandler : ViewHandler<ChatView, ChatViewContainer>
{
    private ObservableCollection<ChatMessage> _items;
    private ObservableRangeCollection<ChatMessage> _itemsSource;
    private ScrollViewer _scrollViewer;
    private WeakReference<ChatView> _weakChatView;
    private ChatContextMenu _contextMenu;
    private readonly ConditionalWeakTable<FrameworkElement, RowState> _rowStates = new();
    private int _unreadCount;
    private bool _initialScrolled;

    public static IPropertyMapper<ChatView, ChatViewHandler> PropertyMapper = new PropertyMapper<ChatView, ChatViewHandler>(ViewHandler.ViewMapper)
    {
        [nameof(ChatView.Messages)] = MapProperties,
        [nameof(ChatView.ShowScrollToBottomButton)] = MapFab,
        [nameof(ChatView.ScrollToBottomButtonBackgroundColor)] = MapFab,
        [nameof(ChatView.ScrollToBottomButtonIconColor)] = MapFab,
        [nameof(ChatView.ScrollToBottomButtonSize)] = MapFab,
        [nameof(ChatView.ScrollToBottomButtonMargin)] = MapFab,
        [nameof(ChatView.ShowScrollToBottomBadge)] = MapFab,
        [nameof(ChatView.ScrollToBottomBadgeBackgroundColor)] = MapFab,
        [nameof(ChatView.ScrollToBottomBadgeTextColor)] = MapFab,
        [nameof(ChatView.ScrollToBottomBadgeFontSize)] = MapFab,
        [nameof(ChatView.OwnMessageBackgroundColor)] = MapProperties,
        [nameof(ChatView.OtherMessageBackgroundColor)] = MapProperties,
        [nameof(ChatView.OwnMessageTextColor)] = MapProperties,
        [nameof(ChatView.OtherMessageTextColor)] = MapProperties,
        [nameof(ChatView.DateTextColor)] = MapProperties,
        [nameof(ChatView.MessageTimeTextColor)] = MapProperties,
        [nameof(ChatView.MessageFontSize)] = MapProperties,
        [nameof(ChatView.MessageSpacing)] = MapProperties,
        [nameof(ChatView.DateTextFontSize)] = MapProperties,
        [nameof(ChatView.MessageTimeFontSize)] = MapProperties,
        [nameof(ChatView.NewMessagesSeperatorFontSize)] = MapProperties,
        [nameof(ChatView.ShowNewMessagesSeperator)] = MapProperties,
        [nameof(ChatView.NewMessagesSeperatorTextColor)] = MapProperties,
        [nameof(ChatView.AvatarSize)] = MapProperties,
        [nameof(ChatView.ScrollToFirstNewMessage)] = MapProperties,
        [nameof(ChatView.AvatarBackgroundColor)] = MapProperties,
        [nameof(ChatView.AvatarTextColor)] = MapProperties,
        [nameof(ChatView.EmojiReactionFontSize)] = MapProperties,
        [nameof(ChatView.EmojiReactionTextColor)] = MapProperties,
        [nameof(ChatView.ReplyMessageBackgroundColor)] = MapProperties,
        [nameof(ChatView.ReplyMessageTextColor)] = MapProperties,
        [nameof(ChatView.ReplyMessageFontSize)] = MapProperties,
        [nameof(ChatView.SystemMessageBackgroundColor)] = MapProperties,
        [nameof(ChatView.SystemMessageFontSize)] = MapProperties,
        [nameof(ChatView.SystemMessageTextColor)] = MapProperties,
        [nameof(ChatView.SenderNameTextColor)] = MapProperties,
        [nameof(ChatView.SenderNameFontSize)] = MapProperties,
        [nameof(ChatView.LinkTextColor)] = MapProperties,
        [nameof(ChatView.EnableLinkPreview)] = MapProperties,
    };

    public static CommandMapper<ChatView, ChatViewHandler> CommandMapper = new CommandMapper<ChatView, ChatViewHandler>()
    {
        [nameof(ChatView.LoadMoreMessagesCommand)] = MapCommands,
        [nameof(ChatView.MessageTappedCommand)] = MapCommands,
        [nameof(ChatView.ScrolledCommand)] = MapCommands
    };

    public ChatViewHandler() : base(PropertyMapper, CommandMapper)
    {
    }

    protected override ChatViewContainer CreatePlatformView() => new();

    protected override void ConnectHandler(ChatViewContainer platformView)
    {
        base.ConnectHandler(platformView);

        platformView.Handler = this;
        ChatViewState.SetHandler(platformView.MessagesList, this);

        _items = new ObservableCollection<ChatMessage>();
        _itemsSource = VirtualView.Messages;
        if (VirtualView.Messages != null)
        {
            foreach (var message in VirtualView.Messages)
                _items.Add(message);
        }
        platformView.MessagesList.ItemsSource = _items;

        platformView.MessagesList.Loaded += OnListLoaded;

        _weakChatView = new WeakReference<ChatView>(VirtualView);
        if (VirtualView.Messages != null)
        {
            VirtualView.Messages.CollectionChanged -= OnMessagesCollectionChanged;
            VirtualView.Messages.CollectionChanged += OnMessagesCollectionChanged;
        }
        VirtualView.RepliedMessageJumpRequested += OnJumpToRepliedMessage;

        ApplyFabStyle();
    }

    protected override void DisconnectHandler(ChatViewContainer platformView)
    {
        if (_weakChatView != null && _weakChatView.TryGetTarget(out var chatView))
        {
            if (chatView.Messages != null)
                chatView.Messages.CollectionChanged -= OnMessagesCollectionChanged;
            chatView.RepliedMessageJumpRequested -= OnJumpToRepliedMessage;
        }

        if (_scrollViewer != null)
            _scrollViewer.ViewChanged -= OnViewChanged;
        _scrollViewer = null;

        _contextMenu?.Dismiss();
        _contextMenu = null;

        if (platformView?.MessagesList != null)
            platformView.MessagesList.Loaded -= OnListLoaded;

        base.DisconnectHandler(platformView);
    }

    private static void MapCommands(ChatViewHandler handler, ChatView view, object? args)
    {
    }

    private static void MapProperties(ChatViewHandler handler, ChatView chatView)
    {
        if (handler.PlatformView == null || handler.VirtualView == null || handler._items == null)
            return;

        // The Messages collection instance was swapped — resync from scratch.
        if (chatView.Messages != null && !ReferenceEquals(handler._itemsSource, chatView.Messages))
        {
            handler.SyncAll(chatView.Messages);
            return;
        }

        handler.RefreshVisibleRows();
    }

    private static void MapFab(ChatViewHandler handler, ChatView chatView)
    {
        handler.ApplyFabStyle();
        handler.UpdateFab();
    }

    // ---- Initial wiring ---------------------------------------------------------------------

    private void OnListLoaded(object sender, RoutedEventArgs e)
    {
        var list = PlatformView?.MessagesList;
        if (list == null)
            return;

        list.Loaded -= OnListLoaded;
        _scrollViewer = ChatViewState.GetScrollViewer(list);
        if (_scrollViewer != null)
            _scrollViewer.ViewChanged += OnViewChanged;

        list.DispatcherQueue.TryEnqueue(() => InitialScroll(0));
    }

    private void InitialScroll(int attempt)
    {
        if (_initialScrolled || _items == null || _items.Count == 0)
            return;

        // Wait for the first layout pass so the panel is measured before positioning.
        if (PlatformView.MessagesList.ActualHeight <= 0 && attempt < 20)
        {
            PlatformView.DispatcherQueue.TryEnqueue(() => InitialScroll(attempt + 1));
            return;
        }

        _initialScrolled = true;

        if (VirtualView.ScrollToFirstNewMessage)
        {
            var firstNew = _items.FirstOrDefault(m => m.ReadState == MessageReadState.New);
            if (firstNew != null)
            {
                PlatformView.MessagesList.ScrollIntoView(firstNew);
                return;
            }
        }

        PlatformView.MessagesList.ScrollIntoView(_items[_items.Count - 1]);
    }

    // ---- Collection sync --------------------------------------------------------------------

    private void SyncAll(ObservableRangeCollection<ChatMessage> messages)
    {
        _itemsSource = messages;
        if (messages != null)
        {
            messages.CollectionChanged -= OnMessagesCollectionChanged;
            messages.CollectionChanged += OnMessagesCollectionChanged;
        }

        _items.Clear();
        if (messages != null)
        {
            foreach (var message in messages)
                _items.Add(message);
        }
        UpdateFab();
    }

    private void OnMessagesCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
    {
        if (!_weakChatView.TryGetTarget(out var chatView))
            return;

        if (PlatformView == null)
            return;

        // Marshal to the UI thread; the change may arrive from a background thread.
        PlatformView.DispatcherQueue.TryEnqueue(() => ApplyCollectionChange(chatView, args));
    }

    private void ApplyCollectionChange(ChatView chatView, NotifyCollectionChangedEventArgs args)
    {
        // LOAD-MORE: older messages prepended at Messages[0]. Insert at the top of the items
        // source with KeepItemsInView so the viewport stays anchored to what the user is
        // looking at instead of jumping to the new top.
        if (args.Action == NotifyCollectionChangedAction.Add && args.NewStartingIndex == 0 && args.NewItems != null)
        {
            SetItemsUpdateMode(ItemsUpdatingScrollMode.KeepItemsInView);
            for (int i = 0; i < args.NewItems.Count; i++)
                _items.Insert(i, (ChatMessage)args.NewItems[i]);
            return;
        }

        // NEW MESSAGE: appended at the end. Follow the conversation when the user is near the
        // newest message; otherwise keep their position and surface the message on the FAB badge.
        if (args.Action == NotifyCollectionChangedAction.Add &&
            args.NewStartingIndex >= chatView.Messages.Count - (args.NewItems?.Count ?? 1) && args.NewItems != null)
        {
            var nearBottom = IsNearBottom();
            SetItemsUpdateMode(nearBottom ? ItemsUpdatingScrollMode.KeepLastItemInView : ItemsUpdatingScrollMode.KeepItemsInView);
            foreach (var item in args.NewItems.Cast<ChatMessage>())
                _items.Add(item);

            if (!nearBottom)
            {
                _unreadCount += args.NewItems.Count;
                if (PlatformView.Fab != null)
                    PlatformView.Fab.UnreadCount = _unreadCount;
                UpdateFab();
            }
            return;
        }

        // REPLACE of a single item (e.g. an in-place edit): rebind that row.
        if (args.Action == NotifyCollectionChangedAction.Replace && args.OldItems?.Count == 1 && args.NewItems?.Count == 1)
        {
            var index = _items.IndexOf((ChatMessage)args.OldItems[0]);
            if (index >= 0)
            {
                SetItemsUpdateMode(ItemsUpdatingScrollMode.KeepItemsInView);
                _items.RemoveAt(index);
                _items.Insert(index, (ChatMessage)args.NewItems[0]);
            }
            return;
        }

        // REMOVE: drop the rows.
        if (args.Action == NotifyCollectionChangedAction.Remove && args.OldItems != null)
        {
            SetItemsUpdateMode(ItemsUpdatingScrollMode.KeepItemsInView);
            foreach (var item in args.OldItems.Cast<ChatMessage>())
            {
                var index = _items.IndexOf(item);
                if (index >= 0)
                    _items.RemoveAt(index);
            }
            UpdateFab();
            return;
        }

        // Reset / anything else: rebuild from the live collection.
        SetItemsUpdateMode(ItemsUpdatingScrollMode.KeepItemsInView);
        _items.Clear();
        foreach (var message in chatView.Messages)
            _items.Add(message);
        UpdateFab();
    }

    private void SetItemsUpdateMode(ItemsUpdatingScrollMode mode)
    {
        if (PlatformView?.MessagesList?.ItemsPanelRoot is ItemsStackPanel panel)
            panel.ItemsUpdatingScrollMode = mode;
    }

    // ---- Row configuration (mirrors Android's OnBindViewHolder) -------------------------------

    internal void OnContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
    {
        if (args.InRecycleQueue)
        {
            ReleaseRowMedia(args.ItemContainer);
            return;
        }

        if (args.Item is not ChatMessage message)
            return;

        ConfigureRow(args.ItemContainer, message, args.ItemIndex);
    }

    private void ConfigureRow(SelectorItem container, ChatMessage message, int index)
    {
        if (container.ContentTemplateRoot is not FrameworkElement root)
            return;

        // Reset transient visuals before binding the new message.
        var highlight = root.FindName("HighlightBorder") as Border;
        if (highlight != null)
        {
            highlight.Visibility = Visibility.Collapsed;
            highlight.Opacity = 1;
        }

        switch (message.MessageType)
        {
            case MessageType.System:
                ConfigureSystemRow(root, message);
                return;
            case MessageType.Date:
                ConfigureDateRow(root, message);
                return;
        }

        ConfigureCommon(root, message, index);

        switch (message.MessageType)
        {
            case MessageType.Text:
                ConfigureTextRow(root, message, index);
                break;
            case MessageType.Image:
                ConfigureImageRow(root, message);
                break;
            case MessageType.Video:
                ConfigureVideoRow(root, message);
                break;
            case MessageType.Audio:
                ConfigureAudioRow(root, message);
                break;
        }
    }

    private void ReleaseRowMedia(SelectorItem container)
    {
        if (container?.ContentTemplateRoot is not FrameworkElement root)
            return;

        if (_rowStates.TryGetValue(root, out var state))
        {
            state.VoicePlayer?.Release();
            state.VoicePlayer = null;
        }

        var video = root.FindName("VideoPlayer") as MediaPlayerElement;
        if (video != null)
        {
            video.AutoPlay = false;
            video.Source = null;
            video.Visibility = Visibility.Collapsed;
        }
    }

    private void ConfigureCommon(FrameworkElement root, ChatMessage message, int index)
    {
        var bubble = root.FindName("Bubble") as Border;
        if (bubble != null)
            bubble.Background = ChatViewState.Brush(message.IsOwnMessage ? VirtualView.OwnMessageBackgroundColor : VirtualView.OtherMessageBackgroundColor);

        // "New messages" separator above the first unread message.
        var separator = root.FindName("NewMessagesSeparator") as Border;
        if (separator != null)
        {
            var isFirstNew = VirtualView.ShowNewMessagesSeperator
                && message.ReadState == MessageReadState.New
                && (index <= 0 || _items[index - 1].ReadState != MessageReadState.New);

            separator.Visibility = isFirstNew ? Visibility.Visible : Visibility.Collapsed;
            if (isFirstNew)
            {
                var label = root.FindName("NewMessagesSeparatorText") as TextBlock;
                if (label != null)
                {
                    label.Text = VirtualView.NewMessagesSeperatorText;
                    label.Foreground = ChatViewState.Brush(VirtualView.NewMessagesSeperatorTextColor);
                    label.FontSize = VirtualView.NewMessagesSeperatorFontSize;
                }
            }
        }

        // Timestamp.
        var timestamp = root.FindName("Timestamp") as TextBlock;
        if (timestamp != null)
        {
            timestamp.Foreground = ChatViewState.Brush(VirtualView.MessageTimeTextColor);
            timestamp.FontSize = VirtualView.MessageTimeFontSize;
        }

        UpdateDeliveryIcon(root, message);

        // Emoji reactions row.
        var reactions = root.FindName("ReactionsList") as ItemsControl;
        if (reactions != null)
        {
            if (message.Reactions is { Count: > 0 })
            {
                reactions.ItemsSource = message.Reactions;
                reactions.Visibility = Visibility.Visible;
            }
            else
            {
                reactions.ItemsSource = null;
                reactions.Visibility = Visibility.Collapsed;
            }
        }

        ConfigureAvatar(root, message);
    }

    private void ConfigureAvatar(FrameworkElement root, ChatMessage message)
    {
        var avatarHost = root.FindName("AvatarHost") as Grid;
        if (avatarHost == null)
            return;

        // Own messages render no avatar (same as Android/iOS).
        if (message.IsOwnMessage)
        {
            avatarHost.Visibility = Visibility.Collapsed;
            return;
        }

        avatarHost.Visibility = Visibility.Visible;
        avatarHost.Width = VirtualView.AvatarSize;
        avatarHost.Height = VirtualView.AvatarSize;

        var circle = root.FindName("AvatarCircle") as Ellipse;
        var initials = root.FindName("AvatarInitials") as TextBlock;
        if (circle == null)
            return;

        circle.Fill = new SolidColorBrush(ChatViewState.ToWindowsColor(VirtualView.AvatarBackgroundColor));
        if (initials != null)
        {
            initials.Foreground = new SolidColorBrush(ChatViewState.ToWindowsColor(VirtualView.AvatarTextColor));
            initials.FontSize = Math.Max(10, VirtualView.AvatarSize * 0.4);
            initials.Visibility = Visibility.Visible;
        }

        // Avatar image (circular clip via the ellipse fill).
        if (message.SenderAvatar is { Length: > 0 })
        {
            var bytes = message.SenderAvatar;
            _ = ChatViewState.BitmapFromBytesAsync(bytes).ContinueWith(t =>
            {
                if (t.Result != null && circle != null)
                {
                    circle.Fill = new ImageBrush { ImageSource = t.Result, Stretch = Stretch.UniformToFill };
                    if (initials != null)
                        initials.Visibility = Visibility.Collapsed;
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
            return;
        }

        if (initials != null)
        {
            initials.Text = !string.IsNullOrWhiteSpace(message.SenderInitials)
                ? message.SenderInitials
                : (!string.IsNullOrWhiteSpace(message.SenderId) && message.SenderId.Trim().Length >= 2
                    ? message.SenderId.Trim()[..2].ToUpperInvariant()
                    : "?");
        }
    }

    private void UpdateDeliveryIcon(FrameworkElement root, ChatMessage message)
    {
        var icon = root.FindName("DeliveryIcon") as Image;
        if (icon == null)
            return;

        var source = message.DeliveryState switch
        {
            MessageDeliveryState.Sent => VirtualView.SendIcon,
            MessageDeliveryState.Delivered => VirtualView.DeliveredIcon,
            MessageDeliveryState.Read => VirtualView.ReadIcon,
            _ => null,
        };

        if (source == null)
        {
            icon.Source = null;
            icon.Visibility = Visibility.Collapsed;
            return;
        }

        try
        {
            ImageSourceExtensions.LoadImage(source, MauiContext, result =>
            {
                if (icon != null && result?.Value is ImageSource winSource)
                {
                    icon.Source = winSource;
                    icon.Visibility = Visibility.Visible;
                }
            });
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error resolving ImageSource: {ex.Message}");
        }
    }

    private void ConfigureTextRow(FrameworkElement root, ChatMessage message, int index)
    {
        var textBlock = root.FindName("MessageText") as RichTextBlock;
        if (textBlock != null)
        {
            var textColor = ChatViewState.ToWindowsColor(message.IsOwnMessage ? VirtualView.OwnMessageTextColor : VirtualView.OtherMessageTextColor);
            ChatViewState.SetMessageText(textBlock, message.TextContent, VirtualView.MessageFontSize, textColor,
                ChatViewState.ToWindowsColor(VirtualView.LinkTextColor), VirtualView.DetectLinks);
        }

        // Sender name (group chats): first message of a sender run on incoming bubbles.
        var senderName = root.FindName("SenderName") as TextBlock;
        if (senderName != null)
        {
            var show = VirtualView.ShowSenderName
                && !message.IsOwnMessage
                && !string.IsNullOrEmpty(message.SenderName)
                && MessageGrouping.IsFirstOfSenderRun(_items, index);

            senderName.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
            if (show)
            {
                senderName.Text = message.SenderName;
                senderName.Foreground = ChatViewState.Brush(VirtualView.SenderNameTextColor);
                senderName.FontSize = VirtualView.SenderNameFontSize;
            }
        }

        // Reply preview.
        var replyBox = root.FindName("ReplyBox") as Border;
        if (replyBox != null)
        {
            var hasReply = message.IsRepliedMessage && message.ReplyToMessage != null;
            replyBox.Visibility = hasReply ? Visibility.Visible : Visibility.Collapsed;
            if (hasReply)
            {
                replyBox.Background = ChatViewState.Brush(VirtualView.ReplyMessageBackgroundColor);
                var replySender = root.FindName("ReplySender") as TextBlock;
                var replyText = root.FindName("ReplyText") as TextBlock;
                if (replySender != null)
                {
                    replySender.Text = RepliedMessage.GenerateTextPreview(message.ReplyToMessage.SenderId);
                    replySender.Foreground = ChatViewState.Brush(VirtualView.ReplyMessageTextColor);
                    replySender.FontSize = VirtualView.ReplyMessageFontSize;
                }
                if (replyText != null)
                {
                    replyText.Text = RepliedMessage.GenerateTextPreview(message.ReplyToMessage.TextPreview);
                    replyText.Foreground = ChatViewState.Brush(VirtualView.ReplyMessageTextColor);
                    replyText.FontSize = VirtualView.ReplyMessageFontSize;
                }
            }
        }

        // Link-preview card.
        var linkCard = root.FindName("LinkCard") as Border;
        if (linkCard != null)
        {
            var hasCard = VirtualView.EnableLinkPreview && message.HasLinkPreview;
            linkCard.Visibility = hasCard ? Visibility.Visible : Visibility.Collapsed;
            if (hasCard)
            {
                linkCard.Background = ChatViewState.Brush(VirtualView.LinkPreviewBackgroundColor);

                var site = root.FindName("LinkSite") as TextBlock;
                var title = root.FindName("LinkTitle") as TextBlock;
                var desc = root.FindName("LinkDescription") as TextBlock;
                var linkImage = root.FindName("LinkImage") as Image;

                if (site != null)
                {
                    site.Text = message.LinkPreview.SiteName ?? string.Empty;
                    site.Foreground = ChatViewState.Brush(VirtualView.LinkPreviewSiteNameColor);
                    site.FontSize = VirtualView.LinkPreviewSiteNameFontSize;
                    site.Visibility = string.IsNullOrEmpty(message.LinkPreview.SiteName) ? Visibility.Collapsed : Visibility.Visible;
                }
                if (title != null)
                {
                    title.Text = message.LinkPreview.Title ?? string.Empty;
                    title.Foreground = ChatViewState.Brush(VirtualView.LinkPreviewTitleColor);
                    title.FontSize = VirtualView.LinkPreviewTitleFontSize;
                    title.Visibility = string.IsNullOrEmpty(message.LinkPreview.Title) ? Visibility.Collapsed : Visibility.Visible;
                }
                if (desc != null)
                {
                    desc.Text = message.LinkPreview.Description ?? string.Empty;
                    desc.Foreground = ChatViewState.Brush(VirtualView.LinkPreviewDescriptionColor);
                    desc.FontSize = VirtualView.LinkPreviewDescriptionFontSize;
                    desc.Visibility = string.IsNullOrEmpty(message.LinkPreview.Description) ? Visibility.Collapsed : Visibility.Visible;
                }
                if (linkImage != null)
                {
                    if (message.LinkPreview.ImageBytes is { Length: > 0 })
                    {
                        var bytes = message.LinkPreview.ImageBytes;
                        _ = ChatViewState.BitmapFromBytesAsync(bytes).ContinueWith(t =>
                        {
                            if (t.Result != null && linkImage != null)
                            {
                                linkImage.Source = t.Result;
                                linkImage.Visibility = Visibility.Visible;
                            }
                        }, TaskScheduler.FromCurrentSynchronizationContext());
                    }
                    else
                    {
                        linkImage.Source = null;
                        linkImage.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }
    }

    private void ConfigureImageRow(FrameworkElement root, ChatMessage message)
    {
        var image = root.FindName("MessageImage") as Image;
        if (image != null)
        {
            if (message.BinaryContent is { Length: > 0 })
            {
                image.Visibility = Visibility.Visible;
                var bytes = message.BinaryContent;
                _ = ChatViewState.BitmapFromBytesAsync(bytes).ContinueWith(t =>
                {
                    if (t.Result != null && image != null)
                        image.Source = t.Result;
                }, TaskScheduler.FromCurrentSynchronizationContext());
            }
            else
            {
                image.Source = null;
                image.Visibility = Visibility.Collapsed;
            }
        }

        ConfigureCaption(root, message);
    }

    private void ConfigureVideoRow(FrameworkElement root, ChatMessage message)
    {
        var player = root.FindName("VideoPlayer") as MediaPlayerElement;
        if (player != null)
        {
            player.AutoPlay = false;
            player.Source = null;
            player.Visibility = Visibility.Collapsed;
        }

        var button = root.FindName("VideoPlayButton") as Button;
        if (button != null)
            button.Visibility = message.BinaryContent is { Length: > 0 } ? Visibility.Visible : Visibility.Collapsed;

        ConfigureCaption(root, message);
    }

    private void ConfigureAudioRow(FrameworkElement root, ChatMessage message)
    {
        var button = root.FindName("AudioPlayButton") as Button;
        var waveform = root.FindName("AudioWaveform") as WaveformView;
        var label = root.FindName("AudioDuration") as TextBlock;

        var foreground = ChatViewState.ToWindowsColor(message.IsOwnMessage ? VirtualView.OwnMessageTextColor : VirtualView.OtherMessageTextColor);
        if (waveform != null)
        {
            waveform.SetColors(ChatViewState.WithAlpha(foreground, message.IsOwnMessage ? 0.4f : 0.3f), foreground);
            waveform.SetSamples(message.AudioWaveform ?? AudioWaveformHelper.GenerateFallback(message.MessageId));
        }

        string path = null;
        if (message.BinaryContent is { Length: > 0 })
            path = ChatViewState.EnsureTempFile($"{message.MessageId}.audio", message.BinaryContent);

        var state = _rowStates.GetOrCreateValue(root);
        state.VoicePlayer ??= new VoiceNotePlayer(button, waveform, label);
        state.VoicePlayer.Configure(path, message.AudioDuration, foreground);
    }

    private void ConfigureCaption(FrameworkElement root, ChatMessage message)
    {
        var caption = root.FindName("Caption") as TextBlock;
        if (caption == null)
            return;

        if (!string.IsNullOrEmpty(message.TextContent))
        {
            caption.Text = message.TextContent;
            caption.Foreground = ChatViewState.Brush(message.IsOwnMessage ? VirtualView.OwnMessageTextColor : VirtualView.OtherMessageTextColor);
            caption.FontSize = VirtualView.MessageFontSize;
            caption.Visibility = Visibility.Visible;
        }
        else
        {
            caption.Visibility = Visibility.Collapsed;
        }
    }

    private void ConfigureSystemRow(FrameworkElement root, ChatMessage message)
    {
        var pill = root.FindName("SystemPill") as Border;
        var text = root.FindName("SystemText") as TextBlock;
        if (pill != null)
            pill.Background = ChatViewState.Brush(VirtualView.SystemMessageBackgroundColor);
        if (text != null)
        {
            text.Text = message.TextContent ?? string.Empty;
            text.Foreground = ChatViewState.Brush(VirtualView.SystemMessageTextColor);
            text.FontSize = VirtualView.SystemMessageFontSize;
        }
    }

    private void ConfigureDateRow(FrameworkElement root, ChatMessage message)
    {
        var pill = root.FindName("DatePill") as Border;
        var text = root.FindName("DateText") as TextBlock;
        if (pill == null || text == null)
            return;

        if (message.Timestamp.Year == 1)
        {
            pill.Visibility = Visibility.Collapsed;
            text.Text = string.Empty;
            return;
        }

        pill.Visibility = Visibility.Visible;
        text.Text = message.TextContent;
        text.Foreground = ChatViewState.Brush(VirtualView.DateTextColor);
        text.FontSize = VirtualView.DateTextFontSize;
    }

    // ---- Scroll handling ----------------------------------------------------------------------

    private void OnViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
    {
        if (VirtualView == null || _scrollViewer == null || _items == null)
            return;

        var args = new ScrolledArgs { X = (int)_scrollViewer.HorizontalOffset, Y = (int)_scrollViewer.VerticalOffset };
        VirtualView.ScrolledCommand?.Execute(args);
        UpdateFab();

        // Older messages end of the list (top) → load more.
        if (_items.Count > 0 && _scrollViewer.VerticalOffset <= 0.5)
            VirtualView.LoadMoreMessagesCommand?.Execute(null);

        // Reached the newest message → notify.
        if (IsNearBottom())
            VirtualView.ScrolledToLastMessageCommand?.Execute(null);
    }

    private bool IsNearBottom()
    {
        if (_scrollViewer == null)
            return true;

        var viewport = _scrollViewer.ViewportHeight;
        if (viewport <= 0)
            return true;

        var remaining = _scrollViewer.ScrollableHeight - _scrollViewer.VerticalOffset;
        return remaining <= viewport * 0.5;
    }

    // ---- Scroll-to-bottom button (FAB) ----------------------------------------------------------

    private void ApplyFabStyle()
    {
        if (PlatformView?.Fab == null || VirtualView == null)
            return;

        PlatformView.Fab.ApplyStyle(
            ChatViewState.ToWindowsColor(VirtualView.ScrollToBottomButtonBackgroundColor),
            ChatViewState.ToWindowsColor(VirtualView.ScrollToBottomButtonIconColor),
            ChatViewState.ToWindowsColor(VirtualView.ScrollToBottomBadgeBackgroundColor),
            ChatViewState.ToWindowsColor(VirtualView.ScrollToBottomBadgeTextColor),
            VirtualView.ScrollToBottomBadgeFontSize,
            VirtualView.ShowScrollToBottomBadge,
            VirtualView.ScrollToBottomButtonSize,
            VirtualView.ScrollToBottomButtonMargin);
        UpdateFab();
    }

    private void UpdateFab()
    {
        if (PlatformView?.Fab == null || VirtualView == null)
            return;

        var nearBottom = IsNearBottom();
        if (nearBottom && _unreadCount != 0)
        {
            _unreadCount = 0;
            PlatformView.Fab.UnreadCount = 0;
        }

        var shouldShow = VirtualView.ShowScrollToBottomButton
            && (_items?.Count ?? 0) > 0
            && !nearBottom;

        PlatformView.Fab.Visibility = shouldShow ? Visibility.Visible : Visibility.Collapsed;
    }

    internal void OnFabTapped()
    {
        _unreadCount = 0;
        if (PlatformView?.Fab != null)
        {
            PlatformView.Fab.UnreadCount = 0;
            PlatformView.Fab.Visibility = Visibility.Collapsed;
        }

        if (_items is { Count: > 0 })
            PlatformView?.MessagesList?.ScrollIntoView(_items[_items.Count - 1]);
    }

    // ---- Context menu / interactions -------------------------------------------------------------

    internal void ShowContextMenu(ChatMessage message, FrameworkElement anchor)
    {
        if (VirtualView == null || !VirtualView.EnableContextMenu)
            return;

        _contextMenu ??= new ChatContextMenu();
        _contextMenu.Show(this, message, anchor);
    }

    internal void HandleContextAction(ChatMessage message, string action)
    {
        VirtualView?.LongPressedCommand?.Execute(new ContextAction { Name = action, Message = message });
    }

    internal void HandleDestructiveAction(ChatMessage message, string action)
    {
        VirtualView?.LongPressedCommand?.Execute(new ContextAction { Name = action, Message = message });
        VirtualView?.Messages.Remove(message);
    }

    internal void HandleEmojiReaction(ChatMessage message, string emoji)
    {
        var existingReaction = message.Reactions.FirstOrDefault(r => r.Emoji == emoji);
        if (existingReaction != null)
        {
            existingReaction.Count++;
        }
        else
        {
            existingReaction = new ChatMessageReaction { Emoji = emoji, Count = 1 };
            message.Reactions.Add(existingReaction);
        }

        // Rebind the row so the new reaction renders.
        var index = _items?.IndexOf(message) ?? -1;
        if (index >= 0)
        {
            SetItemsUpdateMode(ItemsUpdatingScrollMode.KeepItemsInView);
            _items.RemoveAt(index);
            _items.Insert(index, message);
        }

        VirtualView?.LongPressedCommand?.Execute(new ContextAction { Name = "react", Message = message, AdditionalData = existingReaction });
    }

    // ---- Media: image viewer / video playback -----------------------------------------------------

    internal async void OpenImage(ChatMessage message, UIElement anchor)
    {
        if (message?.BinaryContent is not { Length: > 0 })
            return;
        if (!VirtualView.OpenImageFullScreen || anchor?.XamlRoot == null)
            return;

        var bitmap = await ChatViewState.BitmapFromBytesAsync(message.BinaryContent);
        if (bitmap == null)
            return;

        var image = new Image { Source = bitmap, Stretch = Stretch.Uniform };
        var scroll = new ScrollViewer
        {
            Content = image,
            ZoomMode = ZoomMode.Enabled,
            MinZoomFactor = 1,
            MaxZoomFactor = 6,
            HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            HorizontalContentAlignment = HorizontalAlignment.Center,
            VerticalContentAlignment = VerticalAlignment.Center,
            Width = 720,
            Height = 480,
        };

        var dialog = new ContentDialog
        {
            Content = scroll,
            CloseButtonText = "Close",
            XamlRoot = anchor.XamlRoot,
        };
        await dialog.ShowAsync();
    }

    internal void HandleVideoPlay(ChatMessage message, UIElement anchor)
    {
        if (message?.BinaryContent is not { Length: > 0 })
            return;

        var path = ChatViewState.EnsureTempFile($"{message.MessageId}.mp4", message.BinaryContent);
        if (path == null)
            return;

        if (VirtualView.OpenVideoFullScreen)
        {
            OpenFullScreenVideo(path, anchor);
            return;
        }

        // Inline playback inside the bubble.
        if (anchor != null)
        {
            var rowRoot = ChatViewState.FindNamedAncestor(anchor, "RowRoot");
            if (rowRoot != null)
            {
                var player = rowRoot.FindName("VideoPlayer") as MediaPlayerElement;
                var button = rowRoot.FindName("VideoPlayButton") as Button;
                if (player != null)
                {
                    if (button != null)
                        button.Visibility = Visibility.Collapsed;
                    player.Visibility = Visibility.Visible;
                    player.AutoPlay = true;
                    player.Source = MediaSource.CreateFromUri(new Uri(path, UriKind.Absolute));
                }
            }
        }
    }

    private async void OpenFullScreenVideo(string path, UIElement anchor)
    {
        if (anchor?.XamlRoot == null)
            return;

        var player = new MediaPlayerElement
        {
            AutoPlay = true,
            AreTransportControlsEnabled = true,
            Source = MediaSource.CreateFromUri(new Uri(path, UriKind.Absolute)),
            Width = 720,
            Height = 400,
        };

        var dialog = new ContentDialog
        {
            Content = player,
            CloseButtonText = "Close",
            XamlRoot = anchor.XamlRoot,
        };
        await dialog.ShowAsync();

        player.Source = null;
    }

    // ---- Jump to replied message --------------------------------------------------------------------

    private void OnJumpToRepliedMessage(ChatMessage original)
    {
        if (_items == null || !_items.Contains(original) || PlatformView?.MessagesList == null)
            return;

        PlatformView.MessagesList.ScrollIntoView(original);
        _ = FlashWhenRealizedAsync(original);
    }

    private async Task FlashWhenRealizedAsync(ChatMessage message)
    {
        var highlightColor = VirtualView?.RepliedMessageHighlightColor;
        if (highlightColor == null || highlightColor.Alpha <= 0)
            return;

        // Wait until the scroll realizes the row, then flash it briefly.
        for (int attempt = 0; attempt < 20; attempt++)
        {
            await Task.Delay(100);
            var container = PlatformView?.MessagesList?.ContainerFromItem(message) as ListViewItem;
            if (container?.ContentTemplateRoot is not FrameworkElement root)
                continue;

            var highlight = root.FindName("HighlightBorder") as Border;
            if (highlight == null)
                return;

            highlight.Background = new SolidColorBrush(ChatViewState.ToWindowsColor(highlightColor));
            highlight.Visibility = Visibility.Visible;
            await Task.Delay(250);

            for (int step = 1; step <= 8; step++)
            {
                highlight.Opacity = 1 - (step / 8.0);
                await Task.Delay(50);
            }

            highlight.Opacity = 1;
            highlight.Visibility = Visibility.Collapsed;
            return;
        }
    }

    // ---- Styling refresh ---------------------------------------------------------------------

    private void RefreshVisibleRows()
    {
        if (PlatformView?.MessagesList?.ItemsPanelRoot == null || VirtualView == null)
            return;

        var panel = PlatformView.MessagesList.ItemsPanelRoot;
        for (int i = 0; i < panel.Children.Count; i++)
        {
            if (panel.Children[i] is not ListViewItem container)
                continue;

            var item = container.DataContext as ChatMessage;
            var index = PlatformView.MessagesList.IndexFromContainer(container);
            if (item != null && index >= 0)
                ConfigureRow(container, item, index);
        }
    }
}

/// <summary>Per-row state kept alive across container recycling (voice-note player).</summary>
internal sealed class RowState
{
    public VoiceNotePlayer VoicePlayer;
}
