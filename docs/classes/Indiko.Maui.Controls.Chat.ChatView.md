# Class: ChatView

| Property | Value |
|----------|-------|
| Namespace | `Indiko.Maui.Controls.Chat` |
| Type | Class |
| Source File | `src/Indiko.Maui.Controls.Chat/ChatView.cs` |
| Modifiers | public |
| Generated | 2025-10-23 05:54:47 UTC |

## Signature

```csharp
public class ChatView : View
```

## Relationships

**Inherits from:** [View](View.md)

**Dependencies:**
- [BindableProperty](BindableProperty.md)
- [bool](bool.md)
- [ChatMessage](ChatMessage.md)
- [Color](Color.md)
- [ContextMenuItem](ContextMenuItem.md)
- [float](float.md)
- [ICommand](ICommand.md)
- [ImageSource](ImageSource.md)
- [int](int.md)
- [List<>](List__.md)
- [ObservableRangeCollection<>](ObservableRangeCollection__.md)
- [string](string.md)
- [View](View.md)
- [void](void.md)

## Constructors

### ChatView

```csharp
public .ctor()
```

## Fields

### ScrolledCommandProperty

```csharp
public static BindableProperty ScrolledCommandProperty
```

**Returns:** `BindableProperty`

### MessageTappedCommandProperty

```csharp
public static BindableProperty MessageTappedCommandProperty
```

**Returns:** `BindableProperty`

### AvatarTappedCommandProperty

```csharp
public static BindableProperty AvatarTappedCommandProperty
```

**Returns:** `BindableProperty`

### EmojiReactionTappedCommandProperty

```csharp
public static BindableProperty EmojiReactionTappedCommandProperty
```

**Returns:** `BindableProperty`

### LoadMoreMessagesCommandProperty

```csharp
public static BindableProperty LoadMoreMessagesCommandProperty
```

**Returns:** `BindableProperty`

### ScrolledToLastMessageCommandProperty

```csharp
public static BindableProperty ScrolledToLastMessageCommandProperty
```

**Returns:** `BindableProperty`

### MessagesProperty

```csharp
public static BindableProperty MessagesProperty
```

**Returns:** `BindableProperty`

### SystemMessageBackgroundColorProperty

```csharp
public static BindableProperty SystemMessageBackgroundColorProperty
```

**Returns:** `BindableProperty`

### SystemMessageTextColorProperty

```csharp
public static BindableProperty SystemMessageTextColorProperty
```

**Returns:** `BindableProperty`

### SystemMessageFontSizeProperty

```csharp
public static BindableProperty SystemMessageFontSizeProperty
```

**Returns:** `BindableProperty`

### OwnMessageBackgroundColorProperty

```csharp
public static BindableProperty OwnMessageBackgroundColorProperty
```

**Returns:** `BindableProperty`

### OtherMessageBackgroundColorProperty

```csharp
public static BindableProperty OtherMessageBackgroundColorProperty
```

**Returns:** `BindableProperty`

### OwnMessageTextColorProperty

```csharp
public static BindableProperty OwnMessageTextColorProperty
```

**Returns:** `BindableProperty`

### OtherMessageTextColorProperty

```csharp
public static BindableProperty OtherMessageTextColorProperty
```

**Returns:** `BindableProperty`

### MessageFontSizeProperty

```csharp
public static BindableProperty MessageFontSizeProperty
```

**Returns:** `BindableProperty`

### DateTextFontSizeProperty

```csharp
public static BindableProperty DateTextFontSizeProperty
```

**Returns:** `BindableProperty`

### DateTextColorProperty

```csharp
public static BindableProperty DateTextColorProperty
```

**Returns:** `BindableProperty`

### MessageTimeFontSizeProperty

```csharp
public static BindableProperty MessageTimeFontSizeProperty
```

**Returns:** `BindableProperty`

### MessageTimeTextColorProperty

```csharp
public static BindableProperty MessageTimeTextColorProperty
```

**Returns:** `BindableProperty`

### NewMessagesSeperatorTextProperty

```csharp
public static BindableProperty NewMessagesSeperatorTextProperty
```

**Returns:** `BindableProperty`

### NewMessagesSeperatorFontSizeProperty

```csharp
public static BindableProperty NewMessagesSeperatorFontSizeProperty
```

**Returns:** `BindableProperty`

### NewMessagesSeperatorTextColorProperty

```csharp
public static BindableProperty NewMessagesSeperatorTextColorProperty
```

**Returns:** `BindableProperty`

### ShowNewMessagesSeperatorProperty

```csharp
public static BindableProperty ShowNewMessagesSeperatorProperty
```

**Returns:** `BindableProperty`

### EmojiReactionFontSizeProperty

```csharp
public static BindableProperty EmojiReactionFontSizeProperty
```

**Returns:** `BindableProperty`

### EmojiReactionTextColorProperty

```csharp
public static BindableProperty EmojiReactionTextColorProperty
```

**Returns:** `BindableProperty`

### AvatarSizeProperty

```csharp
public static BindableProperty AvatarSizeProperty
```

**Returns:** `BindableProperty`

### AvatarBackgroundColorProperty

```csharp
public static BindableProperty AvatarBackgroundColorProperty
```

**Returns:** `BindableProperty`

### AvatarTextColorProperty

```csharp
public static BindableProperty AvatarTextColorProperty
```

**Returns:** `BindableProperty`

### ScrollToFirstNewMessageProperty

```csharp
public static BindableProperty ScrollToFirstNewMessageProperty
```

**Returns:** `BindableProperty`

### ScrollToLastMessageProperty

```csharp
public static BindableProperty ScrollToLastMessageProperty
```

**Returns:** `BindableProperty`

### MessageSpacingProperty

```csharp
public static BindableProperty MessageSpacingProperty
```

**Returns:** `BindableProperty`

### SendIconProperty

```csharp
public static BindableProperty SendIconProperty
```

**Returns:** `BindableProperty`

### ReadIconProperty

```csharp
public static BindableProperty ReadIconProperty
```

**Returns:** `BindableProperty`

### DeliveredIconProperty

```csharp
public static BindableProperty DeliveredIconProperty
```

**Returns:** `BindableProperty`

### ReplyMessageBackgroundColorProperty

```csharp
public static BindableProperty ReplyMessageBackgroundColorProperty
```

**Returns:** `BindableProperty`

### ReplyMessageTextColorProperty

```csharp
public static BindableProperty ReplyMessageTextColorProperty
```

**Returns:** `BindableProperty`

### ReplyMessageFontSizeProperty

```csharp
public static BindableProperty ReplyMessageFontSizeProperty
```

**Returns:** `BindableProperty`

### LongPressedCommandProperty

```csharp
public static BindableProperty LongPressedCommandProperty
```

**Returns:** `BindableProperty`

### EmojiReactionsProperty

```csharp
public static BindableProperty EmojiReactionsProperty
```

**Returns:** `BindableProperty`

### ContextMenuBackgroundColorProperty

```csharp
public static BindableProperty ContextMenuBackgroundColorProperty
```

**Returns:** `BindableProperty`

### ContextMenuTextColorProperty

```csharp
public static BindableProperty ContextMenuTextColorProperty
```

**Returns:** `BindableProperty`

### ContextMenuDestructiveTextColorProperty

```csharp
public static BindableProperty ContextMenuDestructiveTextColorProperty
```

**Returns:** `BindableProperty`

### ContextMenuDividerColorProperty

```csharp
public static BindableProperty ContextMenuDividerColorProperty
```

**Returns:** `BindableProperty`

### ContextMenuDividerHeightProperty

```csharp
public static BindableProperty ContextMenuDividerHeightProperty
```

**Returns:** `BindableProperty`

### ContextMenuFontSizeProperty

```csharp
public static BindableProperty ContextMenuFontSizeProperty
```

**Returns:** `BindableProperty`

### ContextMenuReactionFontSizeProperty

```csharp
public static BindableProperty ContextMenuReactionFontSizeProperty
```

**Returns:** `BindableProperty`

### EnableContextMenuProperty

```csharp
public static BindableProperty EnableContextMenuProperty
```

**Returns:** `BindableProperty`

### ContextMenuItemsProperty

```csharp
public static BindableProperty ContextMenuItemsProperty
```

**Returns:** `BindableProperty`

## Properties

### ScrolledCommand

```csharp
public ICommand ScrolledCommand { get; set; }
```

**Returns:** `ICommand`

### MessageTappedCommand

```csharp
public ICommand MessageTappedCommand { get; set; }
```

**Returns:** `ICommand`

### AvatarTappedCommand

```csharp
public ICommand AvatarTappedCommand { get; set; }
```

**Returns:** `ICommand`

### EmojiReactionTappedCommand

```csharp
public ICommand EmojiReactionTappedCommand { get; set; }
```

**Returns:** `ICommand`

### LoadMoreMessagesCommand

```csharp
public ICommand LoadMoreMessagesCommand { get; set; }
```

**Returns:** `ICommand`

### ScrolledToLastMessageCommand

```csharp
public ICommand ScrolledToLastMessageCommand { get; set; }
```

**Returns:** `ICommand`

### Messages

```csharp
public ObservableRangeCollection<ChatMessage> Messages { get; set; }
```

**Returns:** `ObservableRangeCollection<ChatMessage>`

### SystemMessageBackgroundColor

```csharp
public Color SystemMessageBackgroundColor { get; set; }
```

**Returns:** `Color`

### SystemMessageTextColor

```csharp
public Color SystemMessageTextColor { get; set; }
```

**Returns:** `Color`

### SystemMessageFontSize

```csharp
public float SystemMessageFontSize { get; set; }
```

**Returns:** `float`

### OwnMessageBackgroundColor

```csharp
public Color OwnMessageBackgroundColor { get; set; }
```

**Returns:** `Color`

### OtherMessageBackgroundColor

```csharp
public Color OtherMessageBackgroundColor { get; set; }
```

**Returns:** `Color`

### OwnMessageTextColor

```csharp
public Color OwnMessageTextColor { get; set; }
```

**Returns:** `Color`

### OtherMessageTextColor

```csharp
public Color OtherMessageTextColor { get; set; }
```

**Returns:** `Color`

### MessageFontSize

```csharp
public float MessageFontSize { get; set; }
```

**Returns:** `float`

### DateTextFontSize

```csharp
public float DateTextFontSize { get; set; }
```

**Returns:** `float`

### DateTextColor

```csharp
public Color DateTextColor { get; set; }
```

**Returns:** `Color`

### MessageTimeFontSize

```csharp
public float MessageTimeFontSize { get; set; }
```

**Returns:** `float`

### MessageTimeTextColor

```csharp
public Color MessageTimeTextColor { get; set; }
```

**Returns:** `Color`

### NewMessagesSeperatorText

```csharp
public string NewMessagesSeperatorText { get; set; }
```

**Returns:** `string`

### NewMessagesSeperatorFontSize

```csharp
public float NewMessagesSeperatorFontSize { get; set; }
```

**Returns:** `float`

### NewMessagesSeperatorTextColor

```csharp
public Color NewMessagesSeperatorTextColor { get; set; }
```

**Returns:** `Color`

### ShowNewMessagesSeperator

```csharp
public bool ShowNewMessagesSeperator { get; set; }
```

**Returns:** `bool`

### EmojiReactionFontSize

```csharp
public float EmojiReactionFontSize { get; set; }
```

**Returns:** `float`

### EmojiReactionTextColor

```csharp
public Color EmojiReactionTextColor { get; set; }
```

**Returns:** `Color`

### AvatarSize

```csharp
public float AvatarSize { get; set; }
```

**Returns:** `float`

### AvatarBackgroundColor

```csharp
public Color AvatarBackgroundColor { get; set; }
```

**Returns:** `Color`

### AvatarTextColor

```csharp
public Color AvatarTextColor { get; set; }
```

**Returns:** `Color`

### ScrollToFirstNewMessage

```csharp
public bool ScrollToFirstNewMessage { get; set; }
```

**Returns:** `bool`

### ScrollToLastMessage

```csharp
public bool ScrollToLastMessage { get; set; }
```

**Returns:** `bool`

### MessageSpacing

```csharp
public int MessageSpacing { get; set; }
```

**Returns:** `int`

### SendIcon

```csharp
public ImageSource SendIcon { get; set; }
```

**Returns:** `ImageSource`

### ReadIcon

```csharp
public ImageSource ReadIcon { get; set; }
```

**Returns:** `ImageSource`

### DeliveredIcon

```csharp
public ImageSource DeliveredIcon { get; set; }
```

**Returns:** `ImageSource`

### ReplyMessageBackgroundColor

```csharp
public Color ReplyMessageBackgroundColor { get; set; }
```

**Returns:** `Color`

### ReplyMessageTextColor

```csharp
public Color ReplyMessageTextColor { get; set; }
```

**Returns:** `Color`

### ReplyMessageFontSize

```csharp
public float ReplyMessageFontSize { get; set; }
```

**Returns:** `float`

### LongPressedCommand

```csharp
public ICommand LongPressedCommand { get; set; }
```

**Returns:** `ICommand`

### EmojiReactions

```csharp
public List<string> EmojiReactions { get; set; }
```

**Returns:** `List<string>`

### ContextMenuBackgroundColor

```csharp
public Color ContextMenuBackgroundColor { get; set; }
```

**Returns:** `Color`

### ContextMenuTextColor

```csharp
public Color ContextMenuTextColor { get; set; }
```

**Returns:** `Color`

### ContextMenuDestructiveTextColor

```csharp
public Color ContextMenuDestructiveTextColor { get; set; }
```

**Returns:** `Color`

### ContextMenuDividerColor

```csharp
public Color ContextMenuDividerColor { get; set; }
```

**Returns:** `Color`

### ContextMenuDividerHeight

```csharp
public int ContextMenuDividerHeight { get; set; }
```

**Returns:** `int`

### ContextMenuFontSize

```csharp
public float ContextMenuFontSize { get; set; }
```

**Returns:** `float`

### ContextMenuReactionFontSize

```csharp
public float ContextMenuReactionFontSize { get; set; }
```

**Returns:** `float`

### EnableContextMenu

```csharp
public bool EnableContextMenu { get; set; }
```

**Returns:** `bool`

### ContextMenuItems

```csharp
public List<ContextMenuItem> ContextMenuItems { get; set; }
```

**Returns:** `List<ContextMenuItem>`

