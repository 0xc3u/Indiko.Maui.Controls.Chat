# Class: DateGroupSeperatorCell

| Property | Value |
|----------|-------|
| Namespace | `Indiko.Maui.Controls.Chat.Platforms.iOS` |
| Type | Class |
| Source File | `src/Indiko.Maui.Controls.Chat/Platforms/iOS/DateGroupSeperatorCell.cs` |
| Modifiers | public |
| Generated | 2025-10-23 05:48:17 UTC |

## Signature

```csharp
public class DateGroupSeperatorCell : UICollectionViewCell
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
- [UILabel](UILabel.md)
- [void](void.md)

## Constructors

### DateGroupSeperatorCell

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

### _dateLabel

```csharp
private UILabel _dateLabel
```

**Returns:** `UILabel`

## Methods

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

