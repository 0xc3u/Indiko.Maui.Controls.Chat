# Class: UiViewExtensions

| Property | Value |
|----------|-------|
| Namespace | `Indiko.Maui.Controls.Chat.Platforms.iOS` |
| Type | Class |
| Source File | `src/Indiko.Maui.Controls.Chat/Platforms/iOS/UiViewExtensions.cs` |
| Modifiers | internal, static |
| Generated | 2025-10-23 05:48:17 UTC |

## Signature

```csharp
internal static class UiViewExtensions
```

## Relationships

**Dependencies:**
- [ChatMessage](ChatMessage.md)
- [ICommand](ICommand.md)
- [object](object.md)
- [UIView](UIView.md)
- [void](void.md)
- [WeakReference<>](WeakReference__.md)

## Methods

### AddWeakTapGestureRecognizerWithCommand

```csharp
public static void AddWeakTapGestureRecognizerWithCommand(UIView uiView, ChatMessage message, ICommand command)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `uiView` | `UIView` |  | `` |
| `message` | `ChatMessage` |  | `` |
| `command` | `ICommand` |  | `` |

### HandleViewTapped

```csharp
private static void HandleViewTapped(UIView uiView, WeakReference<ChatMessage> messageRef, ICommand command)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `uiView` | `UIView` |  | `` |
| `messageRef` | `WeakReference<ChatMessage>` |  | `` |
| `command` | `ICommand` |  | `` |

### AnimateFade

```csharp
public static void AnimateFade(UIView view)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `view` | `UIView` |  | `` |

