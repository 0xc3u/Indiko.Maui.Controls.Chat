# Class: UIImageExtensions

| Property | Value |
|----------|-------|
| Namespace | `Indiko.Maui.Controls.Chat.Platforms.iOS` |
| Type | Class |
| Source File | `src/Indiko.Maui.Controls.Chat/Platforms/iOS/UIImageExtensions.cs` |
| Modifiers | public, static |
| Generated | 2025-10-23 05:54:47 UTC |

## Signature

```csharp
public static class UIImageExtensions
```

## Relationships

**Dependencies:**
- [byte](byte.md)
- [CGColor](CGColor.md)
- [ImageSource](ImageSource.md)
- [IMauiContext](IMauiContext.md)
- [nfloat](nfloat.md)
- [object](object.md)
- [string](string.md)
- [UIColor](UIColor.md)
- [UIImage](UIImage.md)

## Methods

### CreateCircularImage

```csharp
public static UIImage CreateCircularImage(UIImage image)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `image` | `UIImage` |  | `` |

**Returns:** `UIImage`

### CreateInitialsImage

```csharp
public static UIImage CreateInitialsImage(string initials, nfloat width, nfloat height, UIColor textColor, CGColor backgroundColor)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `initials` | `string` |  | `` |
| `width` | `nfloat` |  | `` |
| `height` | `nfloat` |  | `` |
| `textColor` | `UIColor` |  | `` |
| `backgroundColor` | `CGColor` |  | `` |

**Returns:** `UIImage`

### GetImageFromImageSource

```csharp
public static UIImage GetImageFromImageSource(IMauiContext mauiContext, ImageSource imageSource)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `mauiContext` | `IMauiContext` |  | `` |
| `imageSource` | `ImageSource` |  | `` |

**Returns:** `UIImage`

### GetOrSetCachedImage

```csharp
public static UIImage GetOrSetCachedImage(string cacheId, byte[] content)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `cacheId` | `string` |  | `` |
| `content` | `byte[]` |  | `` |

**Returns:** `UIImage`

