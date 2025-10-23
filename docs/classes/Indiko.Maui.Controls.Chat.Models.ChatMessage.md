# Class: ChatMessage

| Property | Value |
|----------|-------|
| Namespace | `Indiko.Maui.Controls.Chat.Models` |
| Type | Class |
| Source File | `src/Indiko.Maui.Controls.Chat/Models/ChatMessage.cs` |
| Modifiers | public |
| Generated | 2025-10-23 05:48:17 UTC |

## Signature

```csharp
public class ChatMessage
```

## Relationships

**Dependencies:**
- [bool](bool.md)
- [byte](byte.md)
- [ChatMessageReaction](ChatMessageReaction.md)
- [DateTime](DateTime.md)
- [List<>](List__.md)
- [MessageDeliveryState](MessageDeliveryState.md)
- [MessageReadState](MessageReadState.md)
- [MessageType](MessageType.md)
- [object](object.md)
- [RepliedMessage](RepliedMessage.md)
- [string](string.md)
- [void](void.md)

## Properties

### MessageId

```csharp
public string MessageId { get; set; }
```

**Returns:** `string`

### Timestamp

```csharp
public DateTime Timestamp { get; set; }
```

**Returns:** `DateTime`

### TextContent

```csharp
public string TextContent { get; set; }
```

**Returns:** `string`

### BinaryContent

```csharp
public byte[] BinaryContent { get; set; }
```

**Returns:** `byte[]`

### IsOwnMessage

```csharp
public bool IsOwnMessage { get; set; }
```

**Returns:** `bool`

### SenderId

```csharp
public string SenderId { get; set; }
```

**Returns:** `string`

### SenderAvatar

```csharp
public byte[] SenderAvatar { get; set; }
```

**Returns:** `byte[]`

### SenderInitials

```csharp
public string SenderInitials { get; set; }
```

**Returns:** `string`

### MessageType

```csharp
public MessageType MessageType { get; set; }
```

**Returns:** `MessageType`

### ReadState

```csharp
public MessageReadState ReadState { get; set; }
```

**Returns:** `MessageReadState`

### DeliveryState

```csharp
public MessageDeliveryState DeliveryState { get; set; }
```

**Returns:** `MessageDeliveryState`

### IsRepliedMessage

```csharp
public bool IsRepliedMessage { get; }
```

**Returns:** `bool`

### ReplyToMessage

```csharp
public RepliedMessage ReplyToMessage { get; set; }
```

**Returns:** `RepliedMessage`

### Reactions

```csharp
public List<ChatMessageReaction> Reactions { get; set; }
```

**Returns:** `List<ChatMessageReaction>`

