# Class: ChatViewDataSource

| Property | Value |
|----------|-------|
| Namespace | `Indiko.Maui.Controls.Chat.Platforms.iOS` |
| Type | Class |
| Source File | `src/Indiko.Maui.Controls.Chat/Platforms/iOS/ChatViewDataSource.cs` |
| Modifiers | public |
| Generated | 2025-10-23 05:41:50 UTC |

## Signature

```csharp
public class ChatViewDataSource : UICollectionViewDiffableDataSource<Indiko.Maui.Controls.Chat.Platforms.iOS.ChatSection, Indiko.Maui.Controls.Chat.Platforms.iOS.ChatMessageItem>
```

## Relationships

**Inherits from:** [UICollectionViewDiffableDataSource<Indiko.Maui.Controls.Chat.Platforms.iOS.ChatSection, Indiko.Maui.Controls.Chat.Platforms.iOS.ChatMessageItem>](UICollectionViewDiffableDataSource_Indiko.Maui.Controls.Chat.Platforms.iOS.ChatSection_ Indiko.Maui.Controls.Chat.Platforms.iOS.ChatMessageItem_.md)

**Dependencies:**
- [ChatMessage](ChatMessage.md)
- [ChatView](ChatView.md)
- [IMauiContext](IMauiContext.md)
- [ObservableCollection<>](ObservableCollection__.md)
- [string](string.md)
- [UICollectionView](UICollectionView.md)
- [UICollectionViewDiffableDataSource<Indiko.Maui.Controls.Chat.Platforms.iOS.ChatSection, Indiko.Maui.Controls.Chat.Platforms.iOS.ChatMessageItem>](UICollectionViewDiffableDataSource_Indiko.Maui.Controls.Chat.Platforms.iOS.ChatSection_ Indiko.Maui.Controls.Chat.Platforms.iOS.ChatMessageItem_.md)
- [void](void.md)

## Constructors

### ChatViewDataSource

```csharp
public .ctor(ChatView virtualView, IMauiContext mauiContext, UICollectionView collectionView)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `virtualView` | `ChatView` |  | `` |
| `mauiContext` | `IMauiContext` |  | `` |
| `collectionView` | `UICollectionView` |  | `` |

## Fields

### _virtualView

```csharp
private ChatView _virtualView
```

**Returns:** `ChatView`

### _mauiContext

```csharp
private IMauiContext _mauiContext
```

**Returns:** `IMauiContext`

## Methods

### UpdateMessages

```csharp
public void UpdateMessages(ObservableCollection<ChatMessage> messages)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `messages` | `ObservableCollection<ChatMessage>` |  | `` |

### GetCellIdentifier

```csharp
private static string GetCellIdentifier(ChatMessage message)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `message` | `ChatMessage` |  | `` |

**Returns:** `string`

