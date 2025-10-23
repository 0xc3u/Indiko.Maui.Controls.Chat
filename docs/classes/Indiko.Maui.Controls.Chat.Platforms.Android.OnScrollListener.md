# Class: OnScrollListener

| Property | Value |
|----------|-------|
| Namespace | `Indiko.Maui.Controls.Chat.Platforms.Android` |
| Type | Class |
| Source File | `src/Indiko.Maui.Controls.Chat/Platforms/Android/OnScrollListener.cs` |
| Modifiers | public |
| Generated | 2025-10-23 05:48:17 UTC |

## Signature

```csharp
public class OnScrollListener : RecyclerView.OnScrollListener
```

## Relationships

**Inherits from:** [RecyclerView.OnScrollListener](RecyclerView.OnScrollListener.md)

**Dependencies:**
- [Action](Action.md)
- [Action<>](Action__.md)
- [int](int.md)
- [RecyclerView](RecyclerView.md)
- [RecyclerView.OnScrollListener](RecyclerView.OnScrollListener.md)
- [ScrolledArgs](ScrolledArgs.md)
- [void](void.md)

## Constructors

### OnScrollListener

```csharp
public .ctor(Action<ScrolledArgs> onScrolled, Action onScrolledToTop, Action onScrolledToBottom)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `onScrolled` | `Action<ScrolledArgs>` |  | `` |
| `onScrolledToTop` | `Action` |  | `` |
| `onScrolledToBottom` | `Action` |  | `` |

## Fields

### _onScrolled

```csharp
private Action<ScrolledArgs> _onScrolled
```

**Returns:** `Action<ScrolledArgs>`

### _onScrolledToTop

```csharp
private Action _onScrolledToTop
```

**Returns:** `Action`

### _onScrolledToBottom

```csharp
private Action _onScrolledToBottom
```

**Returns:** `Action`

## Methods

### OnScrolled

```csharp
public override void OnScrolled(RecyclerView recyclerView, int dx, int dy)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `recyclerView` | `RecyclerView` |  | `` |
| `dx` | `int` |  | `` |
| `dy` | `int` |  | `` |

