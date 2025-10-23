# Interface: IMessageService

| Property | Value |
|----------|-------|
| Namespace | `Indiko.Maui.Controls.Chat.Sample.Services` |
| Type | Interface |
| Source File | `samples/Indiko.Maui.Controls.Chat.Sample/Services/MessageService.cs` |
| Modifiers | public |
| Generated | 2025-10-23 05:48:17 UTC |

## Signature

```csharp
public interface IMessageService
```

## Relationships

**Dependencies:**
- [ChatMessage](ChatMessage.md)
- [DateTime](DateTime.md)
- [List<>](List__.md)
- [Task<>](Task__.md)

## Methods

### GetMessagesAsync

```csharp
public abstract Task<List<ChatMessage>> GetMessagesAsync(DateTime? from, DateTime? until)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `from` | `DateTime?` |  | `` |
| `until` | `DateTime?` |  | `` |

**Returns:** `Task<List<ChatMessage>>`

