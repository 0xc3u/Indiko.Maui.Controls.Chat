# Class: SpacingItemDecoration

| Property | Value |
|----------|-------|
| Namespace | `Indiko.Maui.Controls.Chat.Platforms.Android` |
| Type | Class |
| Source File | `src/Indiko.Maui.Controls.Chat/Platforms/Android/SpacingItemDecoration.cs` |
| Modifiers | public |
| Generated | 2025-10-23 05:41:50 UTC |

## Signature

```csharp
public class SpacingItemDecoration : RecyclerView.ItemDecoration
```

## Relationships

**Inherits from:** [RecyclerView.ItemDecoration](RecyclerView.ItemDecoration.md)

**Dependencies:**
- [Android.Graphics.Rect](Android.Graphics.Rect.md)
- [Android.Views.View](Android.Views.View.md)
- [int](int.md)
- [RecyclerView](RecyclerView.md)
- [RecyclerView.ItemDecoration](RecyclerView.ItemDecoration.md)
- [RecyclerView.State](RecyclerView.State.md)
- [void](void.md)

## Constructors

### SpacingItemDecoration

```csharp
public .ctor(int verticalSpacing)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `verticalSpacing` | `int` |  | `` |

## Fields

### _verticalSpacing

```csharp
private int _verticalSpacing
```

**Returns:** `int`

## Methods

### GetItemOffsets

```csharp
public override void GetItemOffsets(Android.Graphics.Rect outRect, Android.Views.View view, RecyclerView parent, RecyclerView.State state)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `outRect` | `Android.Graphics.Rect` |  | `` |
| `view` | `Android.Views.View` |  | `` |
| `parent` | `RecyclerView` |  | `` |
| `state` | `RecyclerView.State` |  | `` |

