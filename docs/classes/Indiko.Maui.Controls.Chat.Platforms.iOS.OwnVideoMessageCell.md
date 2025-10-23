# Class: OwnVideoMessageCell

| Property | Value |
|----------|-------|
| Namespace | `Indiko.Maui.Controls.Chat.Platforms.iOS` |
| Type | Class |
| Source File | `src/Indiko.Maui.Controls.Chat/Platforms/iOS/OwnVideoMessageCell.cs` |
| Modifiers | internal |
| Generated | 2025-10-23 05:41:50 UTC |

## Signature

```csharp
internal class OwnVideoMessageCell : UICollectionViewCell
```

## Relationships

**Inherits from:** [UICollectionViewCell](UICollectionViewCell.md)

**Dependencies:**
- [AVPlayer](AVPlayer.md)
- [AVPlayerViewController](AVPlayerViewController.md)
- [ChatMessage](ChatMessage.md)
- [ChatView](ChatView.md)
- [IMauiContext](IMauiContext.md)
- [int](int.md)
- [NSLayoutConstraint](NSLayoutConstraint.md)
- [NSString](NSString.md)
- [ObjCRuntime.NativeHandle](ObjCRuntime.NativeHandle.md)
- [UICollectionViewCell](UICollectionViewCell.md)
- [UICollectionViewLayoutAttributes](UICollectionViewLayoutAttributes.md)
- [UIImageView](UIImageView.md)
- [UILabel](UILabel.md)
- [UILongPressGestureRecognizer](UILongPressGestureRecognizer.md)
- [UIStackView](UIStackView.md)
- [UIView](UIView.md)
- [void](void.md)

## Constructors

### OwnVideoMessageCell

```csharp
public .ctor(ObjCRuntime.NativeHandle handle)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `handle` | `ObjCRuntime.NativeHandle` |  | `` |

## Fields

### Key

```csharp
public static NSString Key
```

**Returns:** `NSString`

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

### _player

```csharp
private AVPlayer _player
```

**Returns:** `AVPlayer`

### _playerViewController

```csharp
private AVPlayerViewController _playerViewController
```

**Returns:** `AVPlayerViewController`

### _bubbleView

```csharp
private UIView _bubbleView
```

**Returns:** `UIView`

### _timeLabel

```csharp
private UILabel _timeLabel
```

**Returns:** `UILabel`

### _reactionsStackView

```csharp
private UIStackView _reactionsStackView
```

**Returns:** `UIStackView`

### _deliveryStateImageView

```csharp
private UIImageView _deliveryStateImageView
```

**Returns:** `UIImageView`

### _replyView

```csharp
private UIView _replyView
```

**Returns:** `UIView`

### _replyPreviewTextLabel

```csharp
private UILabel _replyPreviewTextLabel
```

**Returns:** `UILabel`

### _replySenderTextLabel

```csharp
private UILabel _replySenderTextLabel
```

**Returns:** `UILabel`

### _messageVideoTopConstraint

```csharp
private NSLayoutConstraint _messageVideoTopConstraint
```

**Returns:** `NSLayoutConstraint`

### _longPressGesture

```csharp
private UILongPressGestureRecognizer _longPressGesture
```

**Returns:** `UILongPressGestureRecognizer`

## Methods

### PreferredLayoutAttributesFittingAttributes

```csharp
public override UICollectionViewLayoutAttributes PreferredLayoutAttributesFittingAttributes(UICollectionViewLayoutAttributes layoutAttributes)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `layoutAttributes` | `UICollectionViewLayoutAttributes` |  | `` |

**Returns:** `UICollectionViewLayoutAttributes`

### SetupLayout

```csharp
private void SetupLayout()
```

### Update

```csharp
public void Update(int index, ChatMessage message, ChatView chatView, IMauiContext mauiContext)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `index` | `int` |  | `` |
| `message` | `ChatMessage` |  | `` |
| `chatView` | `ChatView` |  | `` |
| `mauiContext` | `IMauiContext` |  | `` |

### LongPressHandler

```csharp
private void LongPressHandler(UILongPressGestureRecognizer recognizer)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `recognizer` | `UILongPressGestureRecognizer` |  | `` |

### DismissContextMenu

```csharp
private void DismissContextMenu()
```

