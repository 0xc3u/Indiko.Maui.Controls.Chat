# Class: ChatMessageAdapter

| Property | Value |
|----------|-------|
| Namespace | `Indiko.Maui.Controls.Chat.Platforms.Android` |
| Type | Class |
| Source File | `src/Indiko.Maui.Controls.Chat/Platforms/Android/ChatMessageAdapter.cs` |
| Modifiers | public |
| Generated | 2025-10-23 05:41:50 UTC |

## Signature

```csharp
public class ChatMessageAdapter : RecyclerView.Adapter
```

## Relationships

**Inherits from:** [RecyclerView.Adapter](RecyclerView.Adapter.md)

**Dependencies:**
- [Bitmap](Bitmap.md)
- [bool](bool.md)
- [ChatMessage](ChatMessage.md)
- [ChatView](ChatView.md)
- [ChatViewHandler](ChatViewHandler.md)
- [Context](Context.md)
- [float](float.md)
- [IList<>](IList__.md)
- [ImageSource](ImageSource.md)
- [ImageView](ImageView.md)
- [IMauiContext](IMauiContext.md)
- [int](int.md)
- [RecyclerView.Adapter](RecyclerView.Adapter.md)
- [RecyclerView.ViewHolder](RecyclerView.ViewHolder.md)
- [string](string.md)
- [ViewGroup](ViewGroup.md)
- [void](void.md)

## Constructors

### ChatMessageAdapter

```csharp
public .ctor(Context context, IMauiContext mauiContext, ChatView virtualView, ChatViewHandler handler)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `context` | `Context` |  | `` |
| `mauiContext` | `IMauiContext` |  | `` |
| `virtualView` | `ChatView` |  | `` |
| `handler` | `ChatViewHandler` |  | `` |

## Fields

### _context

```csharp
private Context _context
```

**Returns:** `Context`

### _messages

```csharp
private IList<ChatMessage> _messages
```

**Returns:** `IList<ChatMessage>`

### _mauiContext

```csharp
private IMauiContext _mauiContext
```

**Returns:** `IMauiContext`

### VirtualView

```csharp
private ChatView VirtualView
```

**Returns:** `ChatView`

### _handler

```csharp
private ChatViewHandler _handler
```

**Returns:** `ChatViewHandler`

## Properties

### OwnMessageBackgroundColor

```csharp
public Microsoft.Maui.Graphics.Color OwnMessageBackgroundColor { get; }
```

**Returns:** `Microsoft.Maui.Graphics.Color`

### OtherMessageBackgroundColor

```csharp
public Microsoft.Maui.Graphics.Color OtherMessageBackgroundColor { get; }
```

**Returns:** `Microsoft.Maui.Graphics.Color`

### OwnMessageTextColor

```csharp
public Microsoft.Maui.Graphics.Color OwnMessageTextColor { get; }
```

**Returns:** `Microsoft.Maui.Graphics.Color`

### OtherMessageTextColor

```csharp
public Microsoft.Maui.Graphics.Color OtherMessageTextColor { get; }
```

**Returns:** `Microsoft.Maui.Graphics.Color`

### MessageFontSize

```csharp
public float MessageFontSize { get; }
```

**Returns:** `float`

### MessageTimeTextColor

```csharp
public Microsoft.Maui.Graphics.Color MessageTimeTextColor { get; }
```

**Returns:** `Microsoft.Maui.Graphics.Color`

### MessageTimeFontSize

```csharp
public float MessageTimeFontSize { get; }
```

**Returns:** `float`

### DateTextColor

```csharp
public Microsoft.Maui.Graphics.Color DateTextColor { get; }
```

**Returns:** `Microsoft.Maui.Graphics.Color`

### DateTextFontSize

```csharp
public float DateTextFontSize { get; }
```

**Returns:** `float`

### NewMessagesSeperatorTextColor

```csharp
public Microsoft.Maui.Graphics.Color NewMessagesSeperatorTextColor { get; }
```

**Returns:** `Microsoft.Maui.Graphics.Color`

### NewMessagesSeperatorFontSize

```csharp
public float NewMessagesSeperatorFontSize { get; }
```

**Returns:** `float`

### NewMessagesSeperatorText

```csharp
public string NewMessagesSeperatorText { get; }
```

**Returns:** `string`

### ShowNewMessagesSeperator

```csharp
public bool ShowNewMessagesSeperator { get; }
```

**Returns:** `bool`

### AvatarSize

```csharp
public float AvatarSize { get; }
```

**Returns:** `float`

### AvatarBackgroundColor

```csharp
public Microsoft.Maui.Graphics.Color AvatarBackgroundColor { get; }
```

**Returns:** `Microsoft.Maui.Graphics.Color`

### AvatarTextColor

```csharp
public Microsoft.Maui.Graphics.Color AvatarTextColor { get; }
```

**Returns:** `Microsoft.Maui.Graphics.Color`

### ScrollToFirstNewMessage

```csharp
public bool ScrollToFirstNewMessage { get; }
```

**Returns:** `bool`

### EmojiReactionTextColor

```csharp
public Microsoft.Maui.Graphics.Color EmojiReactionTextColor { get; }
```

**Returns:** `Microsoft.Maui.Graphics.Color`

### EmojiReactionFontSize

```csharp
public float EmojiReactionFontSize { get; }
```

**Returns:** `float`

### SendIcon

```csharp
public ImageSource SendIcon { get; }
```

**Returns:** `ImageSource`

### DeliveredIcon

```csharp
public ImageSource DeliveredIcon { get; }
```

**Returns:** `ImageSource`

### ReadIcon

```csharp
public ImageSource ReadIcon { get; }
```

**Returns:** `ImageSource`

### ReplyMessageBackgroundColor

```csharp
public Microsoft.Maui.Graphics.Color ReplyMessageBackgroundColor { get; }
```

**Returns:** `Microsoft.Maui.Graphics.Color`

### ReplyMessageTextColor

```csharp
public Microsoft.Maui.Graphics.Color ReplyMessageTextColor { get; }
```

**Returns:** `Microsoft.Maui.Graphics.Color`

### ReplyMessageFontSize

```csharp
public float ReplyMessageFontSize { get; }
```

**Returns:** `float`

### SystemMessageBackgroundColor

```csharp
public Microsoft.Maui.Graphics.Color SystemMessageBackgroundColor { get; }
```

**Returns:** `Microsoft.Maui.Graphics.Color`

### SystemMessageTextColor

```csharp
public Microsoft.Maui.Graphics.Color SystemMessageTextColor { get; }
```

**Returns:** `Microsoft.Maui.Graphics.Color`

### SystemMessageFontSize

```csharp
public float SystemMessageFontSize { get; }
```

**Returns:** `float`

### ItemCount

```csharp
public override int ItemCount { get; }
```

**Returns:** `int`

## Methods

### OnCreateViewHolder

```csharp
public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `parent` | `ViewGroup` |  | `` |
| `viewType` | `int` |  | `` |

**Returns:** `RecyclerView.ViewHolder`

### OnBindViewHolder

```csharp
public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `holder` | `RecyclerView.ViewHolder` |  | `` |
| `position` | `int` |  | `` |

### SetImageSourceToImageView

```csharp
private void SetImageSourceToImageView(ImageSource imageSource, ImageView imageView)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `imageSource` | `ImageSource` |  | `` |
| `imageView` | `ImageView` |  | `` |

### CreateInitialsBitmap

```csharp
public Bitmap CreateInitialsBitmap(string initials, int width, int height)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `initials` | `string` |  | `` |
| `width` | `int` |  | `` |
| `height` | `int` |  | `` |

**Returns:** `Bitmap`

