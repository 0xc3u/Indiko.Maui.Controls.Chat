# Class: ChatViewFlowLayout

| Property | Value |
|----------|-------|
| Namespace | `Indiko.Maui.Controls.Chat.Platforms.iOS` |
| Type | Class |
| Source File | `src/Indiko.Maui.Controls.Chat/Platforms/iOS/ChatViewFlowLayout.cs` |
| Modifiers | public |
| Generated | 2025-10-23 05:41:50 UTC |

## Signature

```csharp
public class ChatViewFlowLayout : UICollectionViewFlowLayout
```

## Relationships

**Inherits from:** [UICollectionViewFlowLayout](UICollectionViewFlowLayout.md)

**Dependencies:**
- [bool](bool.md)
- [CGRect](CGRect.md)
- [nfloat](nfloat.md)
- [NSIndexPath](NSIndexPath.md)
- [UICollectionViewFlowLayout](UICollectionViewFlowLayout.md)
- [UICollectionViewLayoutAttributes](UICollectionViewLayoutAttributes.md)
- [void](void.md)

## Constructors

### ChatViewFlowLayout

```csharp
public .ctor()
```

## Fields

### _calculatedItemWidth

```csharp
private nfloat _calculatedItemWidth
```

**Returns:** `nfloat`

## Methods

### PrepareLayout

```csharp
public override void PrepareLayout()
```

### LayoutAttributesForItem

```csharp
public override UICollectionViewLayoutAttributes LayoutAttributesForItem(NSIndexPath indexPath)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `indexPath` | `NSIndexPath` |  | `` |

**Returns:** `UICollectionViewLayoutAttributes`

### ShouldInvalidateLayoutForBoundsChange

```csharp
public override bool ShouldInvalidateLayoutForBoundsChange(CGRect newBounds)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `newBounds` | `CGRect` |  | `` |

**Returns:** `bool`

