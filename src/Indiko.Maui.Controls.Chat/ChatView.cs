using System.Windows.Input;
using Indiko.Maui.Controls.Chat.Models;

namespace Indiko.Maui.Controls.Chat;
public class ChatView : View
{
    public static readonly BindableProperty ScrolledCommandProperty = BindableProperty.Create(nameof(ScrolledCommand), typeof(ICommand), typeof(ChatView), default(ICommand));
    public ICommand ScrolledCommand
    {
        get => (ICommand)GetValue(ScrolledCommandProperty);
        set => SetValue(ScrolledCommandProperty, value);
    }

    public static readonly BindableProperty MessageTappedCommandProperty = BindableProperty.Create(nameof(MessageTappedCommand), typeof(ICommand), typeof(ChatView), default(ICommand));
    public ICommand MessageTappedCommand
    {
        get => (ICommand)GetValue(MessageTappedCommandProperty);
        set => SetValue(MessageTappedCommandProperty, value);
    }


    public static readonly BindableProperty AvatarTappedCommandProperty = BindableProperty.Create(nameof(AvatarTappedCommand), typeof(ICommand), typeof(ChatView), default(ICommand));
    public ICommand AvatarTappedCommand
    {
        get => (ICommand)GetValue(AvatarTappedCommandProperty);
        set => SetValue(AvatarTappedCommandProperty, value);
    }

    public static readonly BindableProperty EmojiReactionTappedCommandProperty = BindableProperty.Create(nameof(EmojiReactionTappedCommand), typeof(ICommand), typeof(ChatView), default(ICommand));
    public ICommand EmojiReactionTappedCommand
    {
        get => (ICommand)GetValue(EmojiReactionTappedCommandProperty);
        set => SetValue(EmojiReactionTappedCommandProperty, value);
    }

    public static readonly BindableProperty LoadMoreMessagesCommandProperty = BindableProperty.Create(nameof(LoadMoreMessagesCommand), typeof(ICommand), typeof(ChatView), default(ICommand));
    public ICommand LoadMoreMessagesCommand
    {
        get => (ICommand)GetValue(LoadMoreMessagesCommandProperty);
        set => SetValue(LoadMoreMessagesCommandProperty, value);
    }


    public static readonly BindableProperty ScrolledToLastMessageCommandProperty = BindableProperty.Create(nameof(ScrolledToLastMessageCommand), typeof(ICommand), typeof(ChatView), default(ICommand));
    public ICommand ScrolledToLastMessageCommand
    {
        get => (ICommand)GetValue(ScrolledToLastMessageCommandProperty);
        set => SetValue(ScrolledToLastMessageCommandProperty, value);
    }

    public static readonly BindableProperty MessagesProperty = BindableProperty.Create(nameof(Messages), typeof(ObservableRangeCollection<ChatMessage>), typeof(ChatView), default(ObservableRangeCollection<ChatMessage>));

    public ObservableRangeCollection<ChatMessage> Messages
    {
        get => (ObservableRangeCollection<ChatMessage>)GetValue(MessagesProperty);
        set => SetValue(MessagesProperty, value);
    }


    public static readonly BindableProperty SystemMessageBackgroundColorProperty = BindableProperty.Create(nameof(SystemMessageBackgroundColor), typeof(Color), typeof(ChatView), Colors.LightYellow);
    public Color SystemMessageBackgroundColor
    {
        get => (Color)GetValue(SystemMessageBackgroundColorProperty);
        set => SetValue(SystemMessageBackgroundColorProperty, value);
    }

    public static readonly BindableProperty SystemMessageTextColorProperty = BindableProperty.Create(nameof(SystemMessageTextColor), typeof(Color), typeof(ChatView), Colors.Red);
    public Color SystemMessageTextColor
    {
        get => (Color)GetValue(SystemMessageTextColorProperty);
        set => SetValue(SystemMessageTextColorProperty, value);
    }

    public static readonly BindableProperty SystemMessageFontSizeProperty = BindableProperty.Create(nameof(SystemMessageFontSize), typeof(double), typeof(ChatView), 14d);
    public double SystemMessageFontSize
    {
        get => (double)GetValue(SystemMessageFontSizeProperty);
        set => SetValue(SystemMessageFontSizeProperty, value);
    }


    public static readonly BindableProperty OwnMessageBackgroundColorProperty = BindableProperty.Create(nameof(OwnMessageBackgroundColor), typeof(Color), typeof(ChatView), Colors.LightBlue);
    public Color OwnMessageBackgroundColor
    {
        get => (Color)GetValue(OwnMessageBackgroundColorProperty);
        set => SetValue(OwnMessageBackgroundColorProperty, value);
    }

    public static readonly BindableProperty OtherMessageBackgroundColorProperty = BindableProperty.Create(nameof(OtherMessageBackgroundColor), typeof(Color), typeof(ChatView), Colors.LightGray);
    public Color OtherMessageBackgroundColor
    {
        get => (Color)GetValue(OtherMessageBackgroundColorProperty);
        set => SetValue(OtherMessageBackgroundColorProperty, value);
    }

    public static readonly BindableProperty OwnMessageTextColorProperty = BindableProperty.Create(nameof(OwnMessageTextColor), typeof(Color), typeof(ChatView), Colors.Black);
    public Color OwnMessageTextColor
    {
        get => (Color)GetValue(OwnMessageTextColorProperty);
        set => SetValue(OwnMessageTextColorProperty, value);
    }

    public static readonly BindableProperty OtherMessageTextColorProperty = BindableProperty.Create(nameof(OtherMessageTextColor), typeof(Color), typeof(ChatView), Colors.Black);
    public Color OtherMessageTextColor
    {
        get => (Color)GetValue(OtherMessageTextColorProperty);
        set => SetValue(OtherMessageTextColorProperty, value);
    }

    public static readonly BindableProperty MessageFontSizeProperty = BindableProperty.Create(nameof(MessageFontSize), typeof(double), typeof(ChatView), 14d);
    public double MessageFontSize
    {
        get => (double)GetValue(MessageFontSizeProperty);
        set => SetValue(MessageFontSizeProperty, value);
    }

    public static readonly BindableProperty DateTextFontSizeProperty = BindableProperty.Create(nameof(DateTextFontSize), typeof(double), typeof(ChatView), 14d);
    public double DateTextFontSize
    {
        get => (double)GetValue(DateTextFontSizeProperty);
        set => SetValue(DateTextFontSizeProperty, value);
    }

    public static readonly BindableProperty DateTextColorProperty = BindableProperty.Create(nameof(DateTextColor), typeof(Color), typeof(ChatView), Colors.LightGray);
    public Color DateTextColor
    {
        get => (Color)GetValue(DateTextColorProperty);
        set => SetValue(DateTextColorProperty, value);
    }

    public static readonly BindableProperty MessageTimeFontSizeProperty = BindableProperty.Create(nameof(MessageTimeFontSize), typeof(double), typeof(ChatView), 12d);
    public double MessageTimeFontSize
    {
        get => (double)GetValue(MessageTimeFontSizeProperty);
        set => SetValue(MessageTimeFontSizeProperty, value);
    }

    public static readonly BindableProperty MessageTimeTextColorProperty = BindableProperty.Create(nameof(MessageTimeTextColor), typeof(Color), typeof(ChatView), Colors.LightGray);
    public Color MessageTimeTextColor
    {
        get => (Color)GetValue(MessageTimeTextColorProperty);
        set => SetValue(MessageTimeTextColorProperty, value);
    }

    public static readonly BindableProperty NewMessagesSeperatorTextProperty = BindableProperty.Create(nameof(NewMessagesSeperatorText), typeof(string), typeof(ChatView), "New Messages");
    public string NewMessagesSeperatorText
    {
        get => (string)GetValue(NewMessagesSeperatorTextProperty);
        set => SetValue(NewMessagesSeperatorTextProperty, value);
    }

    public static readonly BindableProperty NewMessagesSeperatorFontSizeProperty = BindableProperty.Create(nameof(NewMessagesSeperatorFontSize), typeof(double), typeof(ChatView), 14d);
    public double NewMessagesSeperatorFontSize
    {
        get => (double)GetValue(NewMessagesSeperatorFontSizeProperty);
        set => SetValue(NewMessagesSeperatorFontSizeProperty, value);
    }

    public static readonly BindableProperty NewMessagesSeperatorTextColorProperty = BindableProperty.Create(nameof(NewMessagesSeperatorTextColor), typeof(Color), typeof(ChatView), Colors.LightGray);
    public Color NewMessagesSeperatorTextColor
    {
        get => (Color)GetValue(NewMessagesSeperatorTextColorProperty);
        set => SetValue(NewMessagesSeperatorTextColorProperty, value);
    }

    public static readonly BindableProperty ShowNewMessagesSeperatorProperty = BindableProperty.Create(nameof(ShowNewMessagesSeperator), typeof(bool), typeof(ChatView), false);
    public bool ShowNewMessagesSeperator
    {
        get => (bool)GetValue(ShowNewMessagesSeperatorProperty);
        set => SetValue(ShowNewMessagesSeperatorProperty, value);
    }


    public static readonly BindableProperty EmojiReactionFontSizeProperty = BindableProperty.Create(nameof(EmojiReactionFontSize), typeof(double), typeof(ChatView), 10d);
    public double EmojiReactionFontSize
    {
        get => (double)GetValue(EmojiReactionFontSizeProperty);
        set => SetValue(EmojiReactionFontSizeProperty, value);
    }

    public static readonly BindableProperty EmojiReactionTextColorProperty = BindableProperty.Create(nameof(EmojiReactionTextColor), typeof(Color), typeof(ChatView), Colors.LightGray);
    public Color EmojiReactionTextColor
    {
        get => (Color)GetValue(EmojiReactionTextColorProperty);
        set => SetValue(EmojiReactionTextColorProperty, value);
    }

    public static readonly BindableProperty AvatarSizeProperty = BindableProperty.Create(nameof(AvatarSize), typeof(float), typeof(ChatView), 36f);
    public float AvatarSize
    {
        get => (float)GetValue(AvatarSizeProperty);
        set => SetValue(AvatarSizeProperty, value);
    }

    public static readonly BindableProperty AvatarBackgroundColorProperty = BindableProperty.Create(nameof(AvatarBackgroundColor), typeof(Color), typeof(ChatView), Colors.LightBlue);
    public Color AvatarBackgroundColor
    {
        get => (Color)GetValue(AvatarBackgroundColorProperty);
        set => SetValue(AvatarBackgroundColorProperty, value);
    }

    public static readonly BindableProperty AvatarTextColorProperty = BindableProperty.Create(nameof(AvatarTextColor), typeof(Color), typeof(ChatView), Colors.White);
    public Color AvatarTextColor
    {
        get => (Color)GetValue(AvatarTextColorProperty);
        set => SetValue(AvatarTextColorProperty, value);
    }

    public static readonly BindableProperty ScrollToFirstNewMessageProperty = BindableProperty.Create(nameof(ScrollToFirstNewMessage), typeof(bool), typeof(ChatView), true);
    public bool ScrollToFirstNewMessage
    {
        get => (bool)GetValue(ScrollToFirstNewMessageProperty);
        set => SetValue(ScrollToFirstNewMessageProperty, value);
    }

    public static readonly BindableProperty ScrollToLastMessageProperty = BindableProperty.Create(nameof(ScrollToLastMessage), typeof(bool), typeof(ChatView), true);
    public bool ScrollToLastMessage
    {
        get => (bool)GetValue(ScrollToLastMessageProperty);
        set => SetValue(ScrollToLastMessageProperty, value);
    }

    public static readonly BindableProperty MessageSpacingProperty = BindableProperty.Create(nameof(MessageSpacing), typeof(int), typeof(ChatView), 32);
    public int MessageSpacing
    {
        get => (int)GetValue(MessageSpacingProperty);
        set => SetValue(MessageSpacingProperty, value);
    }

    public static readonly BindableProperty SendIconProperty = BindableProperty.Create(nameof(SendIcon), typeof(ImageSource), typeof(ChatView));
    public ImageSource SendIcon
    {
        get => (ImageSource)GetValue(SendIconProperty);
        set => SetValue(SendIconProperty, value);
    }

    public static readonly BindableProperty ReadIconProperty = BindableProperty.Create(nameof(ReadIcon), typeof(ImageSource), typeof(ChatView));
    public ImageSource ReadIcon
    {
        get => (ImageSource)GetValue(ReadIconProperty);
        set => SetValue(ReadIconProperty, value);
    }

    public static readonly BindableProperty DeliveredIconProperty = BindableProperty.Create(nameof(DeliveredIcon), typeof(ImageSource), typeof(ChatView));
    public ImageSource DeliveredIcon
    {
        get => (ImageSource)GetValue(DeliveredIconProperty);
        set => SetValue(DeliveredIconProperty, value);
    }

    public static readonly BindableProperty ReplyMessageBackgroundColorProperty = BindableProperty.Create(nameof(ReplyMessageBackgroundColor), typeof(Color), typeof(ChatView), Colors.LightYellow);
    public Color ReplyMessageBackgroundColor
    {
        get => (Color)GetValue(ReplyMessageBackgroundColorProperty);
        set => SetValue(ReplyMessageBackgroundColorProperty, value);
    }

    public static readonly BindableProperty ReplyMessageTextColorProperty = BindableProperty.Create(nameof(ReplyMessageTextColor), typeof(Color), typeof(ChatView), Colors.Black);
    public Color ReplyMessageTextColor
    {
        get => (Color)GetValue(ReplyMessageTextColorProperty);
        set => SetValue(ReplyMessageTextColorProperty, value);
    }

    public static readonly BindableProperty ReplyMessageFontSizeProperty = BindableProperty.Create(nameof(ReplyMessageFontSize), typeof(double), typeof(ChatView), 10d);
    public double ReplyMessageFontSize
    {
        get => (double)GetValue(ReplyMessageFontSizeProperty);
        set => SetValue(ReplyMessageFontSizeProperty, value);
    }

    public static readonly BindableProperty LongPressedCommandProperty =
    BindableProperty.Create(nameof(LongPressedCommand), typeof(ICommand), typeof(ChatView), default(ICommand));

    public ICommand LongPressedCommand
    {
        get => (ICommand)GetValue(LongPressedCommandProperty);
        set => SetValue(LongPressedCommandProperty, value);
    }

    public static readonly BindableProperty EmojiReactionsProperty = BindableProperty.Create(nameof(EmojiReactions), typeof(List<string>), typeof(ChatView), default(List<string>));
    public List<string> EmojiReactions
    {
        get => (List<string>)GetValue(EmojiReactionsProperty);
        set => SetValue(EmojiReactionsProperty, value);
    }

    public static readonly BindableProperty ContextMenuBackgroundColorProperty = BindableProperty.Create(nameof(ContextMenuBackgroundColor), typeof(Color), typeof(ChatView), Colors.White);
    public Color ContextMenuBackgroundColor
    {
        get => (Color)GetValue(ContextMenuBackgroundColorProperty);
        set => SetValue(ContextMenuBackgroundColorProperty, value);
    }

    public static readonly BindableProperty ContextMenuTextColorProperty = BindableProperty.Create(nameof(ContextMenuTextColor), typeof(Color), typeof(ChatView), Colors.Black);
    public Color ContextMenuTextColor
    {
        get => (Color)GetValue(ContextMenuTextColorProperty);
        set => SetValue(ContextMenuTextColorProperty, value);
    }


    public static readonly BindableProperty ContextMenuDestructiveTextColorProperty = BindableProperty.Create(nameof(ContextMenuDestructiveTextColor), typeof(Color), typeof(ChatView), Colors.Red);
    public Color ContextMenuDestructiveTextColor
    {
        get => (Color)GetValue(ContextMenuDestructiveTextColorProperty);
        set => SetValue(ContextMenuDestructiveTextColorProperty, value);
    }


    public static readonly BindableProperty ContextMenuDividerColorProperty = BindableProperty.Create(nameof(ContextMenuDividerColor), typeof(Color), typeof(ChatView), Colors.LightGray);
    public Color ContextMenuDividerColor
    {
        get => (Color)GetValue(ContextMenuDividerColorProperty);
        set => SetValue(ContextMenuDividerColorProperty, value);
    }

    public static readonly BindableProperty ContextMenuDividerHeightProperty = BindableProperty.Create(nameof(ContextMenuDividerHeight), typeof(int), typeof(ChatView), 1);
    public int ContextMenuDividerHeight
    {
        get => (int)GetValue(ContextMenuDividerHeightProperty);
        set => SetValue(ContextMenuDividerHeightProperty, value);
    }

    public static readonly BindableProperty ContextMenuFontSizeProperty = BindableProperty.Create(nameof(ContextMenuFontSize), typeof(double), typeof(ChatView), 14d);
    public double ContextMenuFontSize
    {
        get => (double)GetValue(ContextMenuFontSizeProperty);
        set => SetValue(ContextMenuFontSizeProperty, value);
    }

    public static readonly BindableProperty ContextMenuReactionFontSizeProperty = BindableProperty.Create(nameof(ContextMenuReactionFontSize), typeof(double), typeof(ChatView), 18d);
    public double ContextMenuReactionFontSize
    {
        get => (double)GetValue(ContextMenuReactionFontSizeProperty);
        set => SetValue(ContextMenuReactionFontSizeProperty, value);
    }

    public static readonly BindableProperty EnableContextMenuProperty = BindableProperty.Create(nameof(EnableContextMenu), typeof(bool), typeof(ChatView), true);
    public bool EnableContextMenu
    {
        get => (bool)GetValue(EnableContextMenuProperty);
        set => SetValue(EnableContextMenuProperty, value);
    }

    // bindable properties for list of ContextMenuItem
    public static readonly BindableProperty ContextMenuItemsProperty = BindableProperty.Create(nameof(ContextMenuItems), typeof(List<ContextMenuItem>), typeof(ChatView), default(List<ContextMenuItem>));
    public List<ContextMenuItem> ContextMenuItems
    {
        get => (List<ContextMenuItem>)GetValue(ContextMenuItemsProperty);
        set => SetValue(ContextMenuItemsProperty, value);
    }

    public ChatView()
    {
        EmojiReactions =
        [
            "😊",
            "❤️",
            "👍",
            "👎",
            "🔥",
            "👏",
            "😂",
            "😍",
            "😢",
            "😡",
            "🙌",
            "🤔",
            "🤣",
        ];

        ContextMenuItems =
        [
            new() { Name = "Copy", Tag = "copy" },
            new() { Name = "Reply", Tag = "reply" },
            new() { Name = "Delete", Tag = "delete", IsDestructive = true },
        ];
    }
}