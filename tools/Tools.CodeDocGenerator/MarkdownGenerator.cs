using System.Text;

namespace Tools.CodeDocGenerator;

public class MarkdownGenerator
{
    private readonly string _outputPath;
    private readonly string _repositoryRoot;

    public MarkdownGenerator(string outputPath, string repositoryRoot)
    {
        _outputPath = outputPath;
        _repositoryRoot = repositoryRoot;
    }

    public async Task GenerateAsync(List<TypeInfo> types)
    {
        // Create directory structure
        Directory.CreateDirectory(_outputPath);
        var namespacesDir = Path.Combine(_outputPath, "namespaces");
        var classesDir = Path.Combine(_outputPath, "classes");
        var diagramsDir = Path.Combine(_outputPath, "diagrams");

        Directory.CreateDirectory(namespacesDir);
        Directory.CreateDirectory(classesDir);
        Directory.CreateDirectory(diagramsDir);

        Console.WriteLine("Generating documentation files...");

        // Generate type documentation
        var typesByNamespace = types.GroupBy(t => t.Namespace).OrderBy(g => g.Key);
        
        foreach (var type in types)
        {
            await GenerateTypeDocumentationAsync(type, classesDir);
        }

        Console.WriteLine($"  ✓ Generated {types.Count} type documentation files");

        // Generate namespace documentation
        foreach (var nsGroup in typesByNamespace)
        {
            await GenerateNamespaceDocumentationAsync(nsGroup.Key, nsGroup.ToList(), namespacesDir);
        }

        Console.WriteLine($"  ✓ Generated {typesByNamespace.Count()} namespace documentation files");

        // Generate diagrams
        await GenerateDiagramsAsync(types, diagramsDir);
        Console.WriteLine($"  ✓ Generated Mermaid diagrams");

        // Generate index
        await GenerateIndexAsync(types, typesByNamespace);
        Console.WriteLine($"  ✓ Generated index.md");
    }

    private async Task GenerateTypeDocumentationAsync(TypeInfo type, string classesDir)
    {
        var sb = new StringBuilder();
        var fileName = $"{SanitizeFileName(type.FullName)}.md";
        var filePath = Path.Combine(classesDir, fileName);

        // Header
        sb.AppendLine($"# {type.Kind}: {type.Name}");
        sb.AppendLine();

        // Metadata table
        sb.AppendLine("| Property | Value |");
        sb.AppendLine("|----------|-------|");
        sb.AppendLine($"| Namespace | `{type.Namespace}` |");
        sb.AppendLine($"| Type | {type.Kind} |");
        sb.AppendLine($"| Source File | `{type.FilePath}` |");
        if (type.Modifiers.Any())
            sb.AppendLine($"| Modifiers | {string.Join(", ", type.Modifiers)} |");
        sb.AppendLine($"| Generated | {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC |");
        sb.AppendLine();

        // Summary
        if (!string.IsNullOrWhiteSpace(type.Summary))
        {
            sb.AppendLine("## Summary");
            sb.AppendLine();
            sb.AppendLine(type.Summary);
            sb.AppendLine();
        }

        // Signature
        sb.AppendLine("## Signature");
        sb.AppendLine();
        sb.AppendLine("```csharp");
        sb.AppendLine(BuildTypeSignature(type));
        sb.AppendLine("```");
        sb.AppendLine();

        // Relationships
        if (type.BaseType != null || type.Interfaces.Any() || type.Dependencies.Any())
        {
            sb.AppendLine("## Relationships");
            sb.AppendLine();

            if (type.BaseType != null)
            {
                sb.AppendLine($"**Inherits from:** [{type.BaseType}]({GetTypeLink(type.BaseType)})");
                sb.AppendLine();
            }

            if (type.Interfaces.Any())
            {
                sb.AppendLine("**Implements:**");
                foreach (var iface in type.Interfaces)
                {
                    sb.AppendLine($"- [{iface}]({GetTypeLink(iface)})");
                }
                sb.AppendLine();
            }

            if (type.Dependencies.Any())
            {
                sb.AppendLine("**Dependencies:**");
                foreach (var dep in type.Dependencies)
                {
                    sb.AppendLine($"- [{dep}]({GetTypeLink(dep)})");
                }
                sb.AppendLine();
            }
        }

        // Attributes
        if (type.Attributes.Any())
        {
            sb.AppendLine("## Attributes");
            sb.AppendLine();
            foreach (var attr in type.Attributes)
            {
                var args = attr.Arguments.Any() ? $"({string.Join(", ", attr.Arguments)})" : "";
                sb.AppendLine($"- `[{attr.Name}{args}]`");
            }
            sb.AppendLine();
        }

        // Enum Members
        if (type.Kind == TypeKind.Enum && type.EnumMembers.Any())
        {
            sb.AppendLine("## Members");
            sb.AppendLine();
            sb.AppendLine("| Name | Value |");
            sb.AppendLine("|------|-------|");
            foreach (var member in type.EnumMembers)
            {
                var parts = member.Split(" = ");
                if (parts.Length == 2)
                {
                    sb.AppendLine($"| `{parts[0]}` | `{parts[1]}` |");
                }
                else
                {
                    sb.AppendLine($"| `{member}` | |");
                }
            }
            sb.AppendLine();
        }

        // Constructors
        if (type.Constructors.Any())
        {
            sb.AppendLine("## Constructors");
            sb.AppendLine();
            foreach (var ctor in type.Constructors)
            {
                AppendMember(sb, ctor);
            }
        }

        // Fields
        if (type.Fields.Any())
        {
            sb.AppendLine("## Fields");
            sb.AppendLine();
            foreach (var field in type.Fields)
            {
                AppendMember(sb, field);
            }
        }

        // Properties
        if (type.Properties.Any())
        {
            sb.AppendLine("## Properties");
            sb.AppendLine();
            foreach (var prop in type.Properties)
            {
                AppendMember(sb, prop);
            }
        }

        // Methods
        if (type.Methods.Any())
        {
            sb.AppendLine("## Methods");
            sb.AppendLine();
            foreach (var method in type.Methods)
            {
                AppendMember(sb, method);
            }
        }

        // Events
        if (type.Events.Any())
        {
            sb.AppendLine("## Events");
            sb.AppendLine();
            foreach (var ev in type.Events)
            {
                AppendMember(sb, ev);
            }
        }

        await File.WriteAllTextAsync(filePath, sb.ToString());
    }

    private void AppendMember(StringBuilder sb, MemberInfo member)
    {
        sb.AppendLine($"### {member.Name}");
        sb.AppendLine();

        if (!string.IsNullOrWhiteSpace(member.Summary))
        {
            sb.AppendLine(member.Summary);
            sb.AppendLine();
        }

        sb.AppendLine("```csharp");
        sb.AppendLine(member.Signature);
        sb.AppendLine("```");
        sb.AppendLine();

        if (member.Parameters.Any())
        {
            sb.AppendLine("**Parameters:**");
            sb.AppendLine();
            sb.AppendLine("| Name | Type | Description | Default |");
            sb.AppendLine("|------|------|-------------|---------|");
            foreach (var param in member.Parameters)
            {
                var desc = param.Description ?? "";
                var defaultVal = param.DefaultValue ?? "";
                sb.AppendLine($"| `{param.Name}` | `{param.Type}` | {desc} | `{defaultVal}` |");
            }
            sb.AppendLine();
        }

        if (!string.IsNullOrWhiteSpace(member.Returns))
        {
            sb.AppendLine($"**Returns:** `{member.Type}` — {member.Returns}");
            sb.AppendLine();
        }
        else if (member.Type != "constructor" && member.Type != "void")
        {
            sb.AppendLine($"**Returns:** `{member.Type}`");
            sb.AppendLine();
        }
    }

    private async Task GenerateNamespaceDocumentationAsync(string namespaceName, List<TypeInfo> types, string namespacesDir)
    {
        var sb = new StringBuilder();
        var fileName = $"{SanitizeFileName(namespaceName)}.md";
        var filePath = Path.Combine(namespacesDir, fileName);

        sb.AppendLine($"# Namespace: {namespaceName}");
        sb.AppendLine();

        sb.AppendLine("## Overview");
        sb.AppendLine();
        sb.AppendLine($"This namespace contains {types.Count} type(s).");
        sb.AppendLine();

        // Statistics
        var stats = types.GroupBy(t => t.Kind).OrderBy(g => g.Key);
        sb.AppendLine("## Type Statistics");
        sb.AppendLine();
        sb.AppendLine("| Type Kind | Count |");
        sb.AppendLine("|-----------|-------|");
        foreach (var group in stats)
        {
            sb.AppendLine($"| {group.Key} | {group.Count()} |");
        }
        sb.AppendLine();

        // Types by kind
        foreach (var group in stats)
        {
            sb.AppendLine($"## {group.Key} Types");
            sb.AppendLine();
            foreach (var type in group.OrderBy(t => t.Name))
            {
                var link = $"../classes/{SanitizeFileName(type.FullName)}.md";
                sb.AppendLine($"### [{type.Name}]({link})");
                sb.AppendLine();
                if (!string.IsNullOrWhiteSpace(type.Summary))
                {
                    sb.AppendLine(type.Summary);
                    sb.AppendLine();
                }
                sb.AppendLine($"**Source:** `{type.FilePath}`");
                sb.AppendLine();
            }
        }

        await File.WriteAllTextAsync(filePath, sb.ToString());
    }

    private async Task GenerateDiagramsAsync(List<TypeInfo> types, string diagramsDir)
    {
        // Class hierarchy diagram
        await GenerateClassHierarchyDiagramAsync(types, diagramsDir);

        // Namespace dependency diagram
        await GenerateNamespaceDependencyDiagramAsync(types, diagramsDir);
    }

    private async Task GenerateClassHierarchyDiagramAsync(List<TypeInfo> types, string diagramsDir)
    {
        var sb = new StringBuilder();
        sb.AppendLine("graph TD");

        var processedRelationships = new HashSet<string>();

        foreach (var type in types.Where(t => t.Kind == TypeKind.Class || t.Kind == TypeKind.Interface))
        {
            var typeId = SanitizeId(type.FullName);

            if (type.BaseType != null)
            {
                var baseId = SanitizeId(type.BaseType);
                var rel = $"{baseId}_{typeId}";
                if (!processedRelationships.Contains(rel))
                {
                    sb.AppendLine($"    {baseId}[\"{GetShortName(type.BaseType)}\"] --> {typeId}[\"{type.Name}\"]");
                    processedRelationships.Add(rel);
                }
            }

            foreach (var iface in type.Interfaces)
            {
                var ifaceId = SanitizeId(iface);
                var rel = $"{ifaceId}_{typeId}";
                if (!processedRelationships.Contains(rel))
                {
                    sb.AppendLine($"    {ifaceId}[\"{GetShortName(iface)}\"] -.-> {typeId}[\"{type.Name}\"]");
                    processedRelationships.Add(rel);
                }
            }
        }

        await File.WriteAllTextAsync(Path.Combine(diagramsDir, "class-hierarchy.mmd"), sb.ToString());
    }

    private async Task GenerateNamespaceDependencyDiagramAsync(List<TypeInfo> types, string diagramsDir)
    {
        var sb = new StringBuilder();
        sb.AppendLine("graph LR");

        var namespaces = types.Select(t => t.Namespace).Distinct().OrderBy(n => n).ToList();
        var dependencies = new HashSet<string>();

        // Extract namespace-to-namespace dependencies
        foreach (var type in types)
        {
            var sourceNs = type.Namespace;
            
            // Check dependencies from base type, interfaces, and members
            var allDependencies = new HashSet<string>();
            
            if (type.BaseType != null)
                allDependencies.Add(type.BaseType);
            allDependencies.UnionWith(type.Interfaces);
            allDependencies.UnionWith(type.Dependencies);

            foreach (var dep in allDependencies)
            {
                // Extract namespace from dependency
                var lastDot = dep.LastIndexOf('.');
                if (lastDot > 0)
                {
                    var targetNs = dep.Substring(0, lastDot);
                    if (sourceNs != targetNs && namespaces.Contains(targetNs))
                    {
                        var depKey = $"{SanitizeId(sourceNs)}__{SanitizeId(targetNs)}";
                        dependencies.Add(depKey);
                    }
                }
            }
        }

        // Define all nodes
        foreach (var ns in namespaces)
        {
            sb.AppendLine($"    {SanitizeId(ns)}[\"{ns}\"]");
        }

        // Add dependencies
        foreach (var dep in dependencies.OrderBy(d => d))
        {
            var parts = dep.Split("__");
            if (parts.Length == 2)
            {
                sb.AppendLine($"    {parts[0]} --> {parts[1]}");
            }
        }

        await File.WriteAllTextAsync(Path.Combine(diagramsDir, "namespace-dependencies.mmd"), sb.ToString());
    }

    private async Task GenerateIndexAsync(List<TypeInfo> types, IEnumerable<IGrouping<string, TypeInfo>> typesByNamespace)
    {
        var sb = new StringBuilder();
        var indexPath = Path.Combine(_outputPath, "index.md");

        sb.AppendLine("# Code Documentation Index");
        sb.AppendLine();
        sb.AppendLine($"**Generated:** {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
        sb.AppendLine();

        // Statistics
        sb.AppendLine("## Overview");
        sb.AppendLine();
        sb.AppendLine("| Metric | Count |");
        sb.AppendLine("|--------|-------|");
        sb.AppendLine($"| Total Types | {types.Count} |");
        sb.AppendLine($"| Namespaces | {typesByNamespace.Count()} |");
        sb.AppendLine($"| Classes | {types.Count(t => t.Kind == TypeKind.Class)} |");
        sb.AppendLine($"| Interfaces | {types.Count(t => t.Kind == TypeKind.Interface)} |");
        sb.AppendLine($"| Structs | {types.Count(t => t.Kind == TypeKind.Struct)} |");
        sb.AppendLine($"| Enums | {types.Count(t => t.Kind == TypeKind.Enum)} |");
        sb.AppendLine($"| Records | {types.Count(t => t.Kind == TypeKind.Record)} |");
        sb.AppendLine($"| Methods | {types.Sum(t => t.Methods.Count)} |");
        sb.AppendLine($"| Properties | {types.Sum(t => t.Properties.Count)} |");
        sb.AppendLine();

        // Diagrams
        sb.AppendLine("## Diagrams");
        sb.AppendLine();
        sb.AppendLine("### Class Hierarchy");
        sb.AppendLine();
        sb.AppendLine("```mermaid");
        var hierarchyDiagram = await File.ReadAllTextAsync(Path.Combine(_outputPath, "diagrams", "class-hierarchy.mmd"));
        sb.AppendLine(hierarchyDiagram.Trim());
        sb.AppendLine("```");
        sb.AppendLine();
        sb.AppendLine("### Namespace Dependencies");
        sb.AppendLine();
        sb.AppendLine("```mermaid");
        var namespaceDiagram = await File.ReadAllTextAsync(Path.Combine(_outputPath, "diagrams", "namespace-dependencies.mmd"));
        sb.AppendLine(namespaceDiagram.Trim());
        sb.AppendLine("```");
        sb.AppendLine();

        // Namespaces
        sb.AppendLine("## Namespaces");
        sb.AppendLine();
        foreach (var nsGroup in typesByNamespace)
        {
            var nsLink = $"namespaces/{SanitizeFileName(nsGroup.Key)}.md";
            sb.AppendLine($"### [{nsGroup.Key}]({nsLink})");
            sb.AppendLine();
            sb.AppendLine($"Contains {nsGroup.Count()} type(s)");
            sb.AppendLine();

            // List types in namespace
            foreach (var type in nsGroup.OrderBy(t => t.Name))
            {
                var typeLink = $"classes/{SanitizeFileName(type.FullName)}.md";
                var icon = type.Kind switch
                {
                    TypeKind.Class => "🔷",
                    TypeKind.Interface => "🔶",
                    TypeKind.Struct => "🔸",
                    TypeKind.Enum => "🔹",
                    TypeKind.Record => "📋",
                    _ => "📄"
                };
                sb.AppendLine($"- {icon} [{type.Name}]({typeLink}) — {type.Kind}");
            }
            sb.AppendLine();
        }

        // All types alphabetically
        sb.AppendLine("## All Types (Alphabetical)");
        sb.AppendLine();
        foreach (var type in types.OrderBy(t => t.Name))
        {
            var typeLink = $"classes/{SanitizeFileName(type.FullName)}.md";
            sb.AppendLine($"- [{type.Name}]({typeLink}) — `{type.Namespace}` ({type.Kind})");
        }

        await File.WriteAllTextAsync(indexPath, sb.ToString());
    }

    private string BuildTypeSignature(TypeInfo type)
    {
        var sb = new StringBuilder();

        // Attributes
        foreach (var attr in type.Attributes)
        {
            var args = attr.Arguments.Any() ? $"({string.Join(", ", attr.Arguments)})" : "";
            sb.AppendLine($"[{attr.Name}{args}]");
        }

        // Modifiers
        if (type.Modifiers.Any())
        {
            sb.Append(string.Join(" ", type.Modifiers));
            sb.Append(" ");
        }

        // Type kind and name
        sb.Append($"{type.Kind.ToString().ToLower()} {type.Name}");

        // Type parameters
        if (type.TypeParameters.Any())
        {
            sb.Append($"<{string.Join(", ", type.TypeParameters)}>");
        }

        // Base type and interfaces
        var inheritance = new List<string>();
        if (type.BaseType != null)
        {
            inheritance.Add(type.BaseType);
        }
        inheritance.AddRange(type.Interfaces);

        if (inheritance.Any())
        {
            sb.Append($" : {string.Join(", ", inheritance)}");
        }

        return sb.ToString();
    }

    private string SanitizeFileName(string name)
    {
        // Replace invalid characters with underscores
        var invalid = Path.GetInvalidFileNameChars();
        var sanitized = name;
        foreach (var c in invalid)
        {
            sanitized = sanitized.Replace(c, '_');
        }
        // Also replace angle brackets and other special chars
        sanitized = sanitized.Replace('<', '_').Replace('>', '_').Replace(',', '_');
        return sanitized;
    }

    private string SanitizeId(string name)
    {
        // Create a valid Mermaid ID
        return name.Replace(".", "_").Replace("<", "_").Replace(">", "_").Replace(",", "_").Replace(" ", "_");
    }

    private string GetShortName(string fullName)
    {
        var lastDot = fullName.LastIndexOf('.');
        return lastDot > 0 ? fullName.Substring(lastDot + 1) : fullName;
    }

    private string GetTypeLink(string typeName)
    {
        // Try to create a link to the type documentation
        return $"{SanitizeFileName(typeName)}.md";
    }
}
