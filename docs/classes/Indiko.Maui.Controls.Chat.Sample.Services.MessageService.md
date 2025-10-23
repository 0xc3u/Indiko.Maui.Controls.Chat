# Class: MessageService

| Property | Value |
|----------|-------|
| Namespace | `Indiko.Maui.Controls.Chat.Sample.Services` |
| Type | Class |
| Source File | `samples/Indiko.Maui.Controls.Chat.Sample/Services/MessageService.cs` |
| Modifiers | public |
| Generated | 2025-10-23 05:48:17 UTC |

## Signature

```csharp
public class MessageService : Indiko.Maui.Controls.Chat.Sample.Services.IMessageService
```

## Relationships

**Implements:**
- [Indiko.Maui.Controls.Chat.Sample.Services.IMessageService](Indiko.Maui.Controls.Chat.Sample.Services.IMessageService.md)

**Dependencies:**
- [ChatMessage](ChatMessage.md)
- [DateTime](DateTime.md)
- [Indiko.Maui.Controls.Chat.Sample.Services.IMessageService](Indiko.Maui.Controls.Chat.Sample.Services.IMessageService.md)
- [List<>](List__.md)
- [object](object.md)
- [string](string.md)
- [Task<>](Task__.md)
- [User](User.md)
- [void](void.md)

## Constructors

### MessageService

```csharp
public .ctor()
```

## Fields

### _userList

```csharp
private List<User> _userList
```

**Returns:** `List<User>`

### _imageList

```csharp
private List<string> _imageList
```

**Returns:** `List<string>`

## Methods

### GetMessagesAsync

```csharp
public Task<List<ChatMessage>> GetMessagesAsync(DateTime? from, DateTime? until)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `from` | `DateTime?` |  | `` |
| `until` | `DateTime?` |  | `` |

**Returns:** `Task<List<ChatMessage>>`

