# Class: MainPageViewModel

| Property | Value |
|----------|-------|
| Namespace | `Indiko.Maui.Controls.Chat.Sample.ViewModels` |
| Type | Class |
| Source File | `samples/Indiko.Maui.Controls.Chat.Sample/ViewModels/MainPageViewModel.cs` |
| Modifiers | public, partial |
| Generated | 2025-10-23 05:48:17 UTC |

## Signature

```csharp
public partial class MainPageViewModel : BaseViewModel
```

## Relationships

**Inherits from:** [BaseViewModel](BaseViewModel.md)

**Dependencies:**
- [BaseViewModel](BaseViewModel.md)
- [byte](byte.md)
- [ChatMessage](ChatMessage.md)
- [ContextAction](ContextAction.md)
- [IMessageService](IMessageService.md)
- [object](object.md)
- [ObservableRangeCollection<>](ObservableRangeCollection__.md)
- [ScrolledArgs](ScrolledArgs.md)
- [string](string.md)
- [Task](Task.md)
- [void](void.md)

## Constructors

### MainPageViewModel

```csharp
public .ctor(IMessageService messageService)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `messageService` | `IMessageService` |  | `` |

## Fields

### _messageService

```csharp
private IMessageService _messageService
```

**Returns:** `IMessageService`

### newMessage

```csharp
private string newMessage
```

**Returns:** `string`

### selectedMedia

```csharp
private byte[] selectedMedia
```

**Returns:** `byte[]`

### chatMessages

```csharp
private ObservableRangeCollection<ChatMessage> chatMessages
```

**Returns:** `ObservableRangeCollection<ChatMessage>`

## Methods

### OnAppearing

```csharp
public override async Task OnAppearing(object param)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `param` | `object` |  | `` |

**Returns:** `Task`

### LoadOlderMessages

```csharp
private void LoadOlderMessages()
```

### Scrolled

```csharp
private void Scrolled(ScrolledArgs scrolledArgs)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `scrolledArgs` | `ScrolledArgs` |  | `` |

### ScrolledToLastMessage

```csharp
private void ScrolledToLastMessage()
```

### OnAvatarTapped

```csharp
private void OnAvatarTapped(ChatMessage message)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `message` | `ChatMessage` |  | `` |

### OnMessageTapped

```csharp
private void OnMessageTapped(ChatMessage message)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `message` | `ChatMessage` |  | `` |

### OnEmojiReactionTapped

```csharp
private void OnEmojiReactionTapped(ChatMessage message)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `message` | `ChatMessage` |  | `` |

### LongPressed

```csharp
public void LongPressed(ContextAction contextAction)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `contextAction` | `ContextAction` |  | `` |

### SendMessage

```csharp
private void SendMessage()
```

### ClearMedia

```csharp
private void ClearMedia()
```

### PickMedia

```csharp
private async Task PickMedia()
```

**Returns:** `Task`

