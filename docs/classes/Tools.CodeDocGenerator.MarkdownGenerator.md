# Class: MarkdownGenerator

| Property | Value |
|----------|-------|
| Namespace | `Tools.CodeDocGenerator` |
| Type | Class |
| Source File | `tools/Tools.CodeDocGenerator/MarkdownGenerator.cs` |
| Modifiers | public |
| Generated | 2025-10-23 05:54:47 UTC |

## Signature

```csharp
public class MarkdownGenerator
```

## Relationships

**Dependencies:**
- [IEnumerable<>](IEnumerable__.md)
- [IGrouping<, >](IGrouping__ _.md)
- [List<>](List__.md)
- [MemberInfo](MemberInfo.md)
- [object](object.md)
- [string](string.md)
- [Task](Task.md)
- [TypeInfo](TypeInfo.md)
- [void](void.md)

## Constructors

### MarkdownGenerator

```csharp
public .ctor(string outputPath, string repositoryRoot)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `outputPath` | `string` |  | `` |
| `repositoryRoot` | `string` |  | `` |

## Fields

### _outputPath

```csharp
private string _outputPath
```

**Returns:** `string`

### _repositoryRoot

```csharp
private string _repositoryRoot
```

**Returns:** `string`

## Methods

### GenerateAsync

```csharp
public async Task GenerateAsync(List<TypeInfo> types)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `types` | `List<TypeInfo>` |  | `` |

**Returns:** `Task`

### GenerateTypeDocumentationAsync

```csharp
private async Task GenerateTypeDocumentationAsync(TypeInfo type, string classesDir)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `type` | `TypeInfo` |  | `` |
| `classesDir` | `string` |  | `` |

**Returns:** `Task`

### AppendMember

```csharp
private void AppendMember(System.Text.StringBuilder sb, MemberInfo member)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `sb` | `System.Text.StringBuilder` |  | `` |
| `member` | `MemberInfo` |  | `` |

### GenerateNamespaceDocumentationAsync

```csharp
private async Task GenerateNamespaceDocumentationAsync(string namespaceName, List<TypeInfo> types, string namespacesDir)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `namespaceName` | `string` |  | `` |
| `types` | `List<TypeInfo>` |  | `` |
| `namespacesDir` | `string` |  | `` |

**Returns:** `Task`

### GenerateDiagramsAsync

```csharp
private async Task GenerateDiagramsAsync(List<TypeInfo> types, string diagramsDir)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `types` | `List<TypeInfo>` |  | `` |
| `diagramsDir` | `string` |  | `` |

**Returns:** `Task`

### GenerateClassHierarchyDiagramAsync

```csharp
private async Task GenerateClassHierarchyDiagramAsync(List<TypeInfo> types, string diagramsDir)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `types` | `List<TypeInfo>` |  | `` |
| `diagramsDir` | `string` |  | `` |

**Returns:** `Task`

### GenerateNamespaceDependencyDiagramAsync

```csharp
private async Task GenerateNamespaceDependencyDiagramAsync(List<TypeInfo> types, string diagramsDir)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `types` | `List<TypeInfo>` |  | `` |
| `diagramsDir` | `string` |  | `` |

**Returns:** `Task`

### GenerateIndexAsync

```csharp
private async Task GenerateIndexAsync(List<TypeInfo> types, IEnumerable<IGrouping<string, TypeInfo>> typesByNamespace)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `types` | `List<TypeInfo>` |  | `` |
| `typesByNamespace` | `IEnumerable<IGrouping<string, TypeInfo>>` |  | `` |

**Returns:** `Task`

### BuildTypeSignature

```csharp
private string BuildTypeSignature(TypeInfo type)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `type` | `TypeInfo` |  | `` |

**Returns:** `string`

### SanitizeFileName

```csharp
private string SanitizeFileName(string name)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `name` | `string` |  | `` |

**Returns:** `string`

### SanitizeId

```csharp
private string SanitizeId(string name)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `name` | `string` |  | `` |

**Returns:** `string`

### GetShortName

```csharp
private string GetShortName(string fullName)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `fullName` | `string` |  | `` |

**Returns:** `string`

### GetTypeLink

```csharp
private string GetTypeLink(string typeName)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `typeName` | `string` |  | `` |

**Returns:** `string`

