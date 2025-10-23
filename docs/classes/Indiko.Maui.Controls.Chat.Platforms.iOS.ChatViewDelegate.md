# Class: ChatViewDelegate

| Property | Value |
|----------|-------|
| Namespace | `Indiko.Maui.Controls.Chat.Platforms.iOS` |
| Type | Class |
| Source File | `src/Indiko.Maui.Controls.Chat/Platforms/iOS/ChatViewDelegate.cs` |
| Modifiers | public |
| Generated | 2025-10-23 05:48:17 UTC |

## Signature

```csharp
public class ChatViewDelegate : UICollectionViewDelegateFlowLayout
```

## Relationships

**Inherits from:** [UICollectionViewDelegateFlowLayout](UICollectionViewDelegateFlowLayout.md)

**Dependencies:**
- [CGSize](CGSize.md)
- [ChatView](ChatView.md)
- [ChatViewFlowLayout](ChatViewFlowLayout.md)
- [IMauiContext](IMauiContext.md)
- [NSIndexPath](NSIndexPath.md)
- [UICollectionView](UICollectionView.md)
- [UICollectionViewDelegateFlowLayout](UICollectionViewDelegateFlowLayout.md)
- [UICollectionViewLayout](UICollectionViewLayout.md)
- [UIScrollView](UIScrollView.md)
- [void](void.md)

## Constructors

### ChatViewDelegate

```csharp
public .ctor(ChatView virtualView, IMauiContext mauiContext, ChatViewFlowLayout chatViewFlowLayout)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `virtualView` | `ChatView` |  | `` |
| `mauiContext` | `IMauiContext` |  | `` |
| `chatViewFlowLayout` | `ChatViewFlowLayout` |  | `` |

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

### _flowLayout

```csharp
private ChatViewFlowLayout _flowLayout
```

**Returns:** `ChatViewFlowLayout`

## Methods

### GetSizeForItem

```csharp
public override CGSize GetSizeForItem(UICollectionView collectionView, UICollectionViewLayout layout, NSIndexPath indexPath)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `collectionView` | `UICollectionView` |  | `` |
| `layout` | `UICollectionViewLayout` |  | `` |
| `indexPath` | `NSIndexPath` |  | `` |

**Returns:** `CGSize`

### Scrolled

```csharp
public override void Scrolled(UIScrollView scrollView)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `scrollView` | `UIScrollView` |  | `` |

