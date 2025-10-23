# Class: BlurHelper

| Property | Value |
|----------|-------|
| Namespace | `Indiko.Maui.Controls.Chat.Platforms.Android` |
| Type | Class |
| Source File | `src/Indiko.Maui.Controls.Chat/Platforms/Android/BlurOverlayView.cs` |
| Modifiers | public, static |
| Generated | 2025-10-23 05:48:17 UTC |

## Signature

```csharp
public static class BlurHelper
```

## Relationships

**Dependencies:**
- [Android.Content.Context](Android.Content.Context.md)
- [Android.Views.View](Android.Views.View.md)
- [Bitmap](Bitmap.md)
- [object](object.md)

## Methods

### CaptureAndBlur

```csharp
public static Bitmap CaptureAndBlur(Android.Views.View rootView)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `rootView` | `Android.Views.View` |  | `` |

**Returns:** `Bitmap`

### ApplyBlur

```csharp
private static Bitmap ApplyBlur(Bitmap bitmap, Android.Content.Context context)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `bitmap` | `Bitmap` |  | `` |
| `context` | `Android.Content.Context` |  | `` |

**Returns:** `Bitmap`

