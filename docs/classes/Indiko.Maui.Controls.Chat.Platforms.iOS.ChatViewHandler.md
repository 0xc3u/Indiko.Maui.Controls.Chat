# Class: ChatViewHandler

| Property | Value |
|----------|-------|
| Namespace | `Indiko.Maui.Controls.Chat.Platforms.iOS` |
| Type | Class |
| Source File | `src/Indiko.Maui.Controls.Chat/Platforms/iOS/ChatViewHandler.cs` |
| Modifiers | public |
| Generated | 2025-10-23 05:41:50 UTC |

## Signature

```csharp
public class ChatViewHandler : ViewHandler<ChatView, UICollectionView>
```

## Relationships

**Inherits from:** [ViewHandler<ChatView, UICollectionView>](ViewHandler_ChatView_ UICollectionView_.md)

**Dependencies:**
- [CGPoint](CGPoint.md)
- [ChatView](ChatView.md)
- [ChatViewDataSource](ChatViewDataSource.md)
- [ChatViewDelegate](ChatViewDelegate.md)
- [ChatViewFlowLayout](ChatViewFlowLayout.md)
- [CommandMapper<, >](CommandMapper__ _.md)
- [Indiko.Maui.Controls.Chat.Platforms.iOS.ChatViewHandler](Indiko.Maui.Controls.Chat.Platforms.iOS.ChatViewHandler.md)
- [IPropertyMapper<, >](IPropertyMapper__ _.md)
- [NotifyCollectionChangedEventArgs](NotifyCollectionChangedEventArgs.md)
- [object](object.md)
- [object?](object?.md)
- [UICollectionView](UICollectionView.md)
- [ViewHandler<ChatView, UICollectionView>](ViewHandler_ChatView_ UICollectionView_.md)
- [void](void.md)
- [WeakReference<>](WeakReference__.md)

## Constructors

### ChatViewHandler

```csharp
public .ctor()
```

## Fields

### _dataSource

```csharp
private ChatViewDataSource _dataSource
```

**Returns:** `ChatViewDataSource`

### _delegate

```csharp
private ChatViewDelegate _delegate
```

**Returns:** `ChatViewDelegate`

### _flowLayout

```csharp
private ChatViewFlowLayout _flowLayout
```

**Returns:** `ChatViewFlowLayout`

### _lastContentOffset

```csharp
private CGPoint _lastContentOffset
```

**Returns:** `CGPoint`

### _weakChatView

```csharp
private WeakReference<ChatView> _weakChatView
```

**Returns:** `WeakReference<ChatView>`

### CommandMapper

```csharp
public static CommandMapper<ChatView, Indiko.Maui.Controls.Chat.Platforms.iOS.ChatViewHandler> CommandMapper
```

**Returns:** `CommandMapper<ChatView, Indiko.Maui.Controls.Chat.Platforms.iOS.ChatViewHandler>`

### PropertyMapper

```csharp
public static IPropertyMapper<ChatView, Indiko.Maui.Controls.Chat.Platforms.iOS.ChatViewHandler> PropertyMapper
```

**Returns:** `IPropertyMapper<ChatView, Indiko.Maui.Controls.Chat.Platforms.iOS.ChatViewHandler>`

## Methods

### CreatePlatformView

```csharp
protected override UICollectionView CreatePlatformView()
```

**Returns:** `UICollectionView`

### MapProperties

```csharp
private static void MapProperties(Indiko.Maui.Controls.Chat.Platforms.iOS.ChatViewHandler handler, ChatView chatView)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `handler` | `Indiko.Maui.Controls.Chat.Platforms.iOS.ChatViewHandler` |  | `` |
| `chatView` | `ChatView` |  | `` |

### MapCommands

```csharp
private static void MapCommands(Indiko.Maui.Controls.Chat.Platforms.iOS.ChatViewHandler handler, ChatView chatView, object? args)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `handler` | `Indiko.Maui.Controls.Chat.Platforms.iOS.ChatViewHandler` |  | `` |
| `chatView` | `ChatView` |  | `` |
| `args` | `object?` |  | `` |

### UpdateMessages

```csharp
private void UpdateMessages()
```

### SaveScrollPosition

```csharp
private void SaveScrollPosition()
```

### RestoreScrollPosition

```csharp
private void RestoreScrollPosition()
```

### ConnectHandler

```csharp
protected override void ConnectHandler(UICollectionView platformView)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `platformView` | `UICollectionView` |  | `` |

### OnMessagesCollectionChanged

```csharp
private void OnMessagesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `sender` | `object` |  | `` |
| `e` | `NotifyCollectionChangedEventArgs` |  | `` |

### ScrollToLastMessage

```csharp
private void ScrollToLastMessage()
```

### DisconnectHandler

```csharp
protected override void DisconnectHandler(UICollectionView nativeView)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `nativeView` | `UICollectionView` |  | `` |

### ApplyBlurEffect

```csharp
private void ApplyBlurEffect()
```

### RemoveBlurEffect

```csharp
private void RemoveBlurEffect()
```

