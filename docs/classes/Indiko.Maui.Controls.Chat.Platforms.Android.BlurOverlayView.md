# Class: BlurOverlayView

| Property | Value |
|----------|-------|
| Namespace | `Indiko.Maui.Controls.Chat.Platforms.Android` |
| Type | Class |
| Source File | `src/Indiko.Maui.Controls.Chat/Platforms/Android/BlurOverlayView.cs` |
| Modifiers | public |
| Generated | 2025-10-23 05:41:50 UTC |

## Signature

```csharp
public class BlurOverlayView : FrameLayout
```

## Relationships

**Inherits from:** [FrameLayout](FrameLayout.md)

**Dependencies:**
- [Android.Graphics.Paint](Android.Graphics.Paint.md)
- [Android.Views.View](Android.Views.View.md)
- [Bitmap](Bitmap.md)
- [Context](Context.md)
- [FrameLayout](FrameLayout.md)
- [IAttributeSet](IAttributeSet.md)
- [ImageView](ImageView.md)
- [void](void.md)

## Constructors

### BlurOverlayView

```csharp
public .ctor(Context context)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `context` | `Context` |  | `` |

### BlurOverlayView

```csharp
public .ctor(Context context, IAttributeSet attrs)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `context` | `Context` |  | `` |
| `attrs` | `IAttributeSet` |  | `` |

## Fields

### _blurredBackground

```csharp
private Bitmap _blurredBackground
```

**Returns:** `Bitmap`

### _paint

```csharp
private Android.Graphics.Paint _paint
```

**Returns:** `Android.Graphics.Paint`

### _blurImageView

```csharp
private ImageView _blurImageView
```

**Returns:** `ImageView`

## Methods

### Init

```csharp
private void Init()
```

### SetBlurredBackground

```csharp
public void SetBlurredBackground(Bitmap bitmap)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `bitmap` | `Bitmap` |  | `` |

### ApplyBlur

```csharp
public void ApplyBlur(Android.Views.View rootView)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `rootView` | `Android.Views.View` |  | `` |

### ClearBlur

```csharp
public void ClearBlur()
```

