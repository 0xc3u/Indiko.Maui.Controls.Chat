# Class: ChatViewHandler

| Property | Value |
|----------|-------|
| Namespace | `Indiko.Maui.Controls.Chat.Platforms.Android` |
| Type | Class |
| Source File | `src/Indiko.Maui.Controls.Chat/Platforms/Android/ChatViewHandler.cs` |
| Modifiers | public |
| Generated | 2025-10-23 05:41:50 UTC |

## Signature

```csharp
public class ChatViewHandler : ViewHandler<ChatView, RecyclerView>
```

## Relationships

**Inherits from:** [ViewHandler<ChatView, RecyclerView>](ViewHandler_ChatView_ RecyclerView_.md)

**Dependencies:**
- [Action](Action.md)
- [Android.Views.View](Android.Views.View.md)
- [BlurOverlayView](BlurOverlayView.md)
- [bool](bool.md)
- [ChatMessage](ChatMessage.md)
- [ChatMessageAdapter](ChatMessageAdapter.md)
- [ChatView](ChatView.md)
- [CommandMapper<, >](CommandMapper__ _.md)
- [FrameLayout](FrameLayout.md)
- [IMauiContext](IMauiContext.md)
- [Indiko.Maui.Controls.Chat.Platforms.Android.ChatViewHandler](Indiko.Maui.Controls.Chat.Platforms.Android.ChatViewHandler.md)
- [IPropertyMapper<, >](IPropertyMapper__ _.md)
- [LinearLayout](LinearLayout.md)
- [NotifyCollectionChangedEventArgs](NotifyCollectionChangedEventArgs.md)
- [object](object.md)
- [object?](object?.md)
- [RecyclerView](RecyclerView.md)
- [string](string.md)
- [TextView](TextView.md)
- [ViewHandler<ChatView, RecyclerView>](ViewHandler_ChatView_ RecyclerView_.md)
- [void](void.md)
- [WeakReference<>](WeakReference__.md)

## Constructors

### ChatViewHandler

```csharp
public .ctor()
```

## Fields

### _adapter

```csharp
private ChatMessageAdapter _adapter
```

**Returns:** `ChatMessageAdapter`

### _weakChatView

```csharp
private WeakReference<ChatView> _weakChatView
```

**Returns:** `WeakReference<ChatView>`

### _blurOverlay

```csharp
private BlurOverlayView _blurOverlay
```

**Returns:** `BlurOverlayView`

### _messagePopupContainer

```csharp
private FrameLayout _messagePopupContainer
```

**Returns:** `FrameLayout`

### _focusedMessageView

```csharp
private TextView _focusedMessageView
```

**Returns:** `TextView`

### _emojiPanel

```csharp
private LinearLayout _emojiPanel
```

**Returns:** `LinearLayout`

### _contextPanel

```csharp
private LinearLayout _contextPanel
```

**Returns:** `LinearLayout`

### PropertyMapper

```csharp
public static IPropertyMapper<ChatView, Indiko.Maui.Controls.Chat.Platforms.Android.ChatViewHandler> PropertyMapper
```

**Returns:** `IPropertyMapper<ChatView, Indiko.Maui.Controls.Chat.Platforms.Android.ChatViewHandler>`

### CommandMapper

```csharp
public static CommandMapper<ChatView, Indiko.Maui.Controls.Chat.Platforms.Android.ChatViewHandler> CommandMapper
```

**Returns:** `CommandMapper<ChatView, Indiko.Maui.Controls.Chat.Platforms.Android.ChatViewHandler>`

## Methods

### CreatePlatformView

```csharp
protected override RecyclerView CreatePlatformView()
```

**Returns:** `RecyclerView`

### MapCommands

```csharp
private static void MapCommands(Indiko.Maui.Controls.Chat.Platforms.Android.ChatViewHandler handler, ChatView view, object? args)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `handler` | `Indiko.Maui.Controls.Chat.Platforms.Android.ChatViewHandler` |  | `` |
| `view` | `ChatView` |  | `` |
| `args` | `object?` |  | `` |

### MapProperties

```csharp
private static void MapProperties(Indiko.Maui.Controls.Chat.Platforms.Android.ChatViewHandler handler, ChatView chatView)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `handler` | `Indiko.Maui.Controls.Chat.Platforms.Android.ChatViewHandler` |  | `` |
| `chatView` | `ChatView` |  | `` |

### RenderMessages

```csharp
private void RenderMessages(RecyclerView recyclerView, IMauiContext mauiContext)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `recyclerView` | `RecyclerView` |  | `` |
| `mauiContext` | `IMauiContext` |  | `` |

### OnMessagesCollectionChanged

```csharp
private void OnMessagesCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `sender` | `object` |  | `` |
| `args` | `NotifyCollectionChangedEventArgs` |  | `` |

### HandleContextAction

```csharp
private void HandleContextAction(ChatMessage message, string action)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `message` | `ChatMessage` |  | `` |
| `action` | `string` |  | `` |

### HandleDestructiveAction

```csharp
private void HandleDestructiveAction(ChatMessage message, string action)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `message` | `ChatMessage` |  | `` |
| `action` | `string` |  | `` |

### HandleEmojiReaction

```csharp
private void HandleEmojiReaction(ChatMessage message, string emoji)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `message` | `ChatMessage` |  | `` |
| `emoji` | `string` |  | `` |

### DisconnectHandler

```csharp
protected override void DisconnectHandler(RecyclerView platformView)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `platformView` | `RecyclerView` |  | `` |

### ShowContextMenu

```csharp
public void ShowContextMenu(ChatMessage message, Android.Views.View anchorView)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `message` | `ChatMessage` |  | `` |
| `anchorView` | `Android.Views.View` |  | `` |

### CreateEmojiPanel

```csharp
private void CreateEmojiPanel(ChatMessage message, LinearLayout parent)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `message` | `ChatMessage` |  | `` |
| `parent` | `LinearLayout` |  | `` |

### AddDivider

```csharp
private void AddDivider(LinearLayout parent)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `parent` | `LinearLayout` |  | `` |

### AddMenuItem

```csharp
private void AddMenuItem(LinearLayout parent, string text, bool isDestructive, Action onClick)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `parent` | `LinearLayout` |  | `` |
| `text` | `string` |  | `` |
| `isDestructive` | `bool` |  | `` |
| `onClick` | `Action` |  | `` |

### CreateContextPanel

```csharp
private void CreateContextPanel(ChatMessage message, Android.Views.View anchorView)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `message` | `ChatMessage` |  | `` |
| `anchorView` | `Android.Views.View` |  | `` |

### DismissContextMenu

```csharp
private void DismissContextMenu()
```

