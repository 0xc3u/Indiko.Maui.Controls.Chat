# Class: ChatContextMenuView

| Property | Value |
|----------|-------|
| Namespace | `Indiko.Maui.Controls.Chat.Platforms.iOS` |
| Type | Class |
| Source File | `src/Indiko.Maui.Controls.Chat/Platforms/iOS/ChatContextMenuView.cs` |
| Modifiers | public |
| Generated | 2025-10-23 05:41:50 UTC |

## Signature

```csharp
public class ChatContextMenuView : UIView
```

## Relationships

**Inherits from:** [UIView](UIView.md)

**Dependencies:**
- [Action](Action.md)
- [bool](bool.md)
- [ChatMessage](ChatMessage.md)
- [ChatView](ChatView.md)
- [string](string.md)
- [UILabel](UILabel.md)
- [UIScrollView](UIScrollView.md)
- [UIStackView](UIStackView.md)
- [UIView](UIView.md)
- [UIVisualEffectView](UIVisualEffectView.md)
- [void](void.md)

## Constructors

### ChatContextMenuView

```csharp
public .ctor(ChatView chatView, ChatMessage message, UIView messageView, Action onDismiss)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `chatView` | `ChatView` |  | `` |
| `message` | `ChatMessage` |  | `` |
| `messageView` | `UIView` |  | `` |
| `onDismiss` | `Action` |  | `` |

## Fields

### _chatView

```csharp
private ChatView _chatView
```

**Returns:** `ChatView`

### _message

```csharp
private ChatMessage _message
```

**Returns:** `ChatMessage`

### _messageView

```csharp
private UIView _messageView
```

**Returns:** `UIView`

### _onDismiss

```csharp
private Action _onDismiss
```

**Returns:** `Action`

### _contentView

```csharp
private UIView _contentView
```

**Returns:** `UIView`

### _bubbleView

```csharp
private UIView _bubbleView
```

**Returns:** `UIView`

### _messageLabel

```csharp
private UILabel _messageLabel
```

**Returns:** `UILabel`

### _emojiScrollView

```csharp
private UIScrollView _emojiScrollView
```

**Returns:** `UIScrollView`

### _emojiStack

```csharp
private UIStackView _emojiStack
```

**Returns:** `UIStackView`

### _actionStack

```csharp
private UIStackView _actionStack
```

**Returns:** `UIStackView`

### _blurView

```csharp
private UIVisualEffectView _blurView
```

**Returns:** `UIVisualEffectView`

## Methods

### SetupConstraints

```csharp
private void SetupConstraints(UIStackView stackView)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `stackView` | `UIStackView` |  | `` |

### AddReaction

```csharp
private void AddReaction(string emoji)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `emoji` | `string` |  | `` |

### HandleAction

```csharp
private void HandleAction(string actionTag, bool isDestructive)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `actionTag` | `string` |  | `` |
| `isDestructive` | `bool` |  | `` |

### Show

```csharp
public void Show()
```

### AnimateShow

```csharp
private void AnimateShow()
```

### DismissContextMenu

```csharp
private void DismissContextMenu()
```

