# Class: RepliedMessage

| Property | Value |
|----------|-------|
| Namespace | `Indiko.Maui.Controls.Chat.Models` |
| Type | Class |
| Source File | `src/Indiko.Maui.Controls.Chat/Models/RepliedMessage.cs` |
| Modifiers | public |
| Generated | 2025-10-23 05:41:50 UTC |

## Signature

```csharp
public class RepliedMessage
```

## Relationships

**Dependencies:**
- [int](int.md)
- [string](string.md)
- [void](void.md)

## Properties

### MessageId

```csharp
public string MessageId { get; set; }
```

**Returns:** `string`

### TextPreview

```csharp
public string TextPreview { get; set; }
```

**Returns:** `string`

### SenderId

```csharp
public string SenderId { get; set; }
```

**Returns:** `string`

## Methods

### GenerateTextPreview

```csharp
public static string GenerateTextPreview(string text, int maxLength = 50)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `text` | `string` |  | `` |
| `maxLength` | `int` |  | `50` |

**Returns:** `string`

