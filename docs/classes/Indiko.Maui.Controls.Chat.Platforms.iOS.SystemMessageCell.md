# Class: SystemMessageCell

| Property | Value |
|----------|-------|
| Namespace | `Indiko.Maui.Controls.Chat.Platforms.iOS` |
| Type | Class |
| Source File | `src/Indiko.Maui.Controls.Chat/Platforms/iOS/SystemMessageCell.cs` |
| Modifiers | public |
| Generated | 2025-10-23 05:41:50 UTC |

## Signature

```csharp
public class SystemMessageCell : UICollectionViewCell
```

## Relationships

**Inherits from:** [UICollectionViewCell](UICollectionViewCell.md)

**Dependencies:**
- [ChatMessage](ChatMessage.md)
- [ChatView](ChatView.md)
- [IMauiContext](IMauiContext.md)
- [int](int.md)
- [NSString](NSString.md)
- [ObjCRuntime.NativeHandle](ObjCRuntime.NativeHandle.md)
- [UICollectionViewCell](UICollectionViewCell.md)
- [UICollectionViewLayoutAttributes](UICollectionViewLayoutAttributes.md)
- [UILabel](UILabel.md)
- [UIView](UIView.md)
- [void](void.md)

## Constructors

### SystemMessageCell

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

### _bubbleView

```csharp
private UIView _bubbleView
```

**Returns:** `UIView`

### _systemMessageLabel

```csharp
private UILabel _systemMessageLabel
```

**Returns:** `UILabel`

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

