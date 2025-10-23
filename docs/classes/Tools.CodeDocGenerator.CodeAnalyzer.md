# Class: CodeAnalyzer

| Property | Value |
|----------|-------|
| Namespace | `Tools.CodeDocGenerator` |
| Type | Class |
| Source File | `tools/Tools.CodeDocGenerator/CodeAnalyzer.cs` |
| Modifiers | public |
| Generated | 2025-10-23 05:54:47 UTC |

## Signature

```csharp
public class CodeAnalyzer
```

## Relationships

**Dependencies:**
- [AttributeInfo](AttributeInfo.md)
- [HashSet<>](HashSet__.md)
- [IEnumerable<>](IEnumerable__.md)
- [IMethodSymbol](IMethodSymbol.md)
- [INamedTypeSymbol](INamedTypeSymbol.md)
- [IPropertySymbol](IPropertySymbol.md)
- [ISymbol](ISymbol.md)
- [ITypeSymbol](ITypeSymbol.md)
- [List<>](List__.md)
- [MemberInfo](MemberInfo.md)
- [object](object.md)
- [SemanticModel](SemanticModel.md)
- [string](string.md)
- [SyntaxNode](SyntaxNode.md)
- [Task<>](Task__.md)
- [TypeInfo](TypeInfo.md)
- [void](void.md)

## Constructors

### CodeAnalyzer

```csharp
public .ctor(string repositoryRoot)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `repositoryRoot` | `string` |  | `` |

## Fields

### _repositoryRoot

```csharp
private string _repositoryRoot
```

**Returns:** `string`

## Methods

### AnalyzeAsync

```csharp
public async Task<List<TypeInfo>> AnalyzeAsync()
```

**Returns:** `Task<List<TypeInfo>>`

### AnalyzeType

```csharp
private TypeInfo? AnalyzeType(SyntaxNode typeDecl, SemanticModel semanticModel, string filePath)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `typeDecl` | `SyntaxNode` |  | `` |
| `semanticModel` | `SemanticModel` |  | `` |
| `filePath` | `string` |  | `` |

**Returns:** `TypeInfo?`

### GetModifiers

```csharp
private List<string> GetModifiers(SyntaxNode node)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `node` | `SyntaxNode` |  | `` |

**Returns:** `List<string>`

### GetXmlDocSummary

```csharp
private string? GetXmlDocSummary(ISymbol symbol)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `symbol` | `ISymbol` |  | `` |

**Returns:** `string?`

### GetAttributes

```csharp
private List<AttributeInfo> GetAttributes(ISymbol symbol)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `symbol` | `ISymbol` |  | `` |

**Returns:** `List<AttributeInfo>`

### GetFields

```csharp
private IEnumerable<MemberInfo> GetFields(INamedTypeSymbol typeSymbol)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `typeSymbol` | `INamedTypeSymbol` |  | `` |

**Returns:** `IEnumerable<MemberInfo>`

### GetProperties

```csharp
private IEnumerable<MemberInfo> GetProperties(INamedTypeSymbol typeSymbol)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `typeSymbol` | `INamedTypeSymbol` |  | `` |

**Returns:** `IEnumerable<MemberInfo>`

### GetMethods

```csharp
private IEnumerable<MemberInfo> GetMethods(INamedTypeSymbol typeSymbol)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `typeSymbol` | `INamedTypeSymbol` |  | `` |

**Returns:** `IEnumerable<MemberInfo>`

### GetEvents

```csharp
private IEnumerable<MemberInfo> GetEvents(INamedTypeSymbol typeSymbol)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `typeSymbol` | `INamedTypeSymbol` |  | `` |

**Returns:** `IEnumerable<MemberInfo>`

### GetConstructors

```csharp
private IEnumerable<MemberInfo> GetConstructors(INamedTypeSymbol typeSymbol)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `typeSymbol` | `INamedTypeSymbol` |  | `` |

**Returns:** `IEnumerable<MemberInfo>`

### GetSymbolModifiers

```csharp
private List<string> GetSymbolModifiers(ISymbol symbol)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `symbol` | `ISymbol` |  | `` |

**Returns:** `List<string>`

### BuildPropertySignature

```csharp
private string BuildPropertySignature(IPropertySymbol prop)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `prop` | `IPropertySymbol` |  | `` |

**Returns:** `string`

### BuildMethodSignature

```csharp
private string BuildMethodSignature(IMethodSymbol method)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `method` | `IMethodSymbol` |  | `` |

**Returns:** `string`

### FormatDefaultValue

```csharp
private string FormatDefaultValue(object? value)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `value` | `object?` |  | `` |

**Returns:** `string`

### GetXmlDocReturns

```csharp
private string? GetXmlDocReturns(IMethodSymbol method)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `method` | `IMethodSymbol` |  | `` |

**Returns:** `string?`

### GetParameterDescription

```csharp
private string? GetParameterDescription(IMethodSymbol method, string paramName)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `method` | `IMethodSymbol` |  | `` |
| `paramName` | `string` |  | `` |

**Returns:** `string?`

### ExtractDependencies

```csharp
private IEnumerable<string> ExtractDependencies(INamedTypeSymbol typeSymbol)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `typeSymbol` | `INamedTypeSymbol` |  | `` |

**Returns:** `IEnumerable<string>`

### AddTypeDependency

```csharp
private void AddTypeDependency(HashSet<string> dependencies, ITypeSymbol type)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `dependencies` | `HashSet<string>` |  | `` |
| `type` | `ITypeSymbol` |  | `` |

### GetRelativePath

```csharp
private string GetRelativePath(string fullPath)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `fullPath` | `string` |  | `` |

**Returns:** `string`

