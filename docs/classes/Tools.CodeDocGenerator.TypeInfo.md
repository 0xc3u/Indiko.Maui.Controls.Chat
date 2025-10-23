# Class: TypeInfo

| Property | Value |
|----------|-------|
| Namespace | `Tools.CodeDocGenerator` |
| Type | Class |
| Source File | `tools/Tools.CodeDocGenerator/TypeInfo.cs` |
| Modifiers | public |
| Generated | 2025-10-23 05:48:17 UTC |

## Signature

```csharp
public class TypeInfo
```

## Relationships

**Dependencies:**
- [List<>](List__.md)
- [object](object.md)
- [string](string.md)
- [Tools.CodeDocGenerator.AttributeInfo](Tools.CodeDocGenerator.AttributeInfo.md)
- [Tools.CodeDocGenerator.MemberInfo](Tools.CodeDocGenerator.MemberInfo.md)
- [Tools.CodeDocGenerator.TypeKind](Tools.CodeDocGenerator.TypeKind.md)
- [void](void.md)

## Properties

### Name

```csharp
public string Name { get; set; }
```

**Returns:** `string`

### FullName

```csharp
public string FullName { get; set; }
```

**Returns:** `string`

### Namespace

```csharp
public string Namespace { get; set; }
```

**Returns:** `string`

### FilePath

```csharp
public string FilePath { get; set; }
```

**Returns:** `string`

### Kind

```csharp
public Tools.CodeDocGenerator.TypeKind Kind { get; set; }
```

**Returns:** `Tools.CodeDocGenerator.TypeKind`

### Summary

```csharp
public string? Summary { get; set; }
```

**Returns:** `string?`

### Remarks

```csharp
public string? Remarks { get; set; }
```

**Returns:** `string?`

### Modifiers

```csharp
public List<string> Modifiers { get; set; }
```

**Returns:** `List<string>`

### TypeParameters

```csharp
public List<string> TypeParameters { get; set; }
```

**Returns:** `List<string>`

### BaseType

```csharp
public string? BaseType { get; set; }
```

**Returns:** `string?`

### Interfaces

```csharp
public List<string> Interfaces { get; set; }
```

**Returns:** `List<string>`

### Fields

```csharp
public List<Tools.CodeDocGenerator.MemberInfo> Fields { get; set; }
```

**Returns:** `List<Tools.CodeDocGenerator.MemberInfo>`

### Properties

```csharp
public List<Tools.CodeDocGenerator.MemberInfo> Properties { get; set; }
```

**Returns:** `List<Tools.CodeDocGenerator.MemberInfo>`

### Methods

```csharp
public List<Tools.CodeDocGenerator.MemberInfo> Methods { get; set; }
```

**Returns:** `List<Tools.CodeDocGenerator.MemberInfo>`

### Events

```csharp
public List<Tools.CodeDocGenerator.MemberInfo> Events { get; set; }
```

**Returns:** `List<Tools.CodeDocGenerator.MemberInfo>`

### Constructors

```csharp
public List<Tools.CodeDocGenerator.MemberInfo> Constructors { get; set; }
```

**Returns:** `List<Tools.CodeDocGenerator.MemberInfo>`

### EnumMembers

```csharp
public List<string> EnumMembers { get; set; }
```

**Returns:** `List<string>`

### Attributes

```csharp
public List<Tools.CodeDocGenerator.AttributeInfo> Attributes { get; set; }
```

**Returns:** `List<Tools.CodeDocGenerator.AttributeInfo>`

### Dependencies

```csharp
public List<string> Dependencies { get; set; }
```

**Returns:** `List<string>`

