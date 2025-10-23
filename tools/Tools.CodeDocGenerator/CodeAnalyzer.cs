using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Xml.Linq;

namespace Tools.CodeDocGenerator;

public class CodeAnalyzer
{
    private readonly string _repositoryRoot;

    public CodeAnalyzer(string repositoryRoot)
    {
        _repositoryRoot = repositoryRoot;
    }

    public async Task<List<TypeInfo>> AnalyzeAsync()
    {
        var types = new List<TypeInfo>();
        var csFiles = Directory.GetFiles(_repositoryRoot, "*.cs", SearchOption.AllDirectories)
            .Where(f => !f.Contains("/bin/") && !f.Contains("/obj/") && !f.Contains("/.git/"))
            .ToList();

        Console.WriteLine($"Found {csFiles.Count} C# files to analyze...");

        foreach (var filePath in csFiles)
        {
            try
            {
                var code = await File.ReadAllTextAsync(filePath);
                var tree = CSharpSyntaxTree.ParseText(code);
                var root = await tree.GetRootAsync();

                var compilation = CSharpCompilation.Create("Analysis")
                    .AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
                    .AddSyntaxTrees(tree);

                var semanticModel = compilation.GetSemanticModel(tree);

                var typeDeclarations = root.DescendantNodes()
                    .Where(n => n is ClassDeclarationSyntax or InterfaceDeclarationSyntax or 
                               StructDeclarationSyntax or EnumDeclarationSyntax or 
                               RecordDeclarationSyntax)
                    .ToList();

                foreach (var typeDecl in typeDeclarations)
                {
                    var typeInfo = AnalyzeType(typeDecl, semanticModel, filePath);
                    if (typeInfo != null)
                    {
                        types.Add(typeInfo);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Failed to analyze {filePath}: {ex.Message}");
            }
        }

        return types;
    }

    private TypeInfo? AnalyzeType(SyntaxNode typeDecl, SemanticModel semanticModel, string filePath)
    {
        try
        {
            var symbol = semanticModel.GetDeclaredSymbol(typeDecl);
            if (symbol is not INamedTypeSymbol typeSymbol)
                return null;

            var kind = typeDecl switch
            {
                ClassDeclarationSyntax => TypeKind.Class,
                InterfaceDeclarationSyntax => TypeKind.Interface,
                StructDeclarationSyntax => TypeKind.Struct,
                EnumDeclarationSyntax => TypeKind.Enum,
                RecordDeclarationSyntax => TypeKind.Record,
                _ => TypeKind.Class
            };

            var typeInfo = new TypeInfo
            {
                Name = typeSymbol.Name,
                FullName = typeSymbol.ToDisplayString(),
                Namespace = typeSymbol.ContainingNamespace?.ToDisplayString() ?? "",
                FilePath = GetRelativePath(filePath),
                Kind = kind,
                Summary = GetXmlDocSummary(typeSymbol),
                Modifiers = GetModifiers(typeDecl),
                TypeParameters = typeSymbol.TypeParameters.Select(tp => tp.Name).ToList(),
                BaseType = typeSymbol.BaseType?.ToDisplayString() != "object" && typeSymbol.BaseType != null
                    ? typeSymbol.BaseType.ToDisplayString()
                    : null,
                Interfaces = typeSymbol.Interfaces.Select(i => i.ToDisplayString()).ToList(),
                Attributes = GetAttributes(typeSymbol)
            };

            if (kind == TypeKind.Enum)
            {
                if (typeDecl is EnumDeclarationSyntax enumDecl)
                {
                    typeInfo.EnumMembers = enumDecl.Members
                        .Select(m => m.Identifier.Text + 
                               (m.EqualsValue != null ? $" = {m.EqualsValue.Value}" : ""))
                        .ToList();
                }
            }
            else
            {
                // Analyze members
                typeInfo.Fields = GetFields(typeSymbol).ToList();
                typeInfo.Properties = GetProperties(typeSymbol).ToList();
                typeInfo.Methods = GetMethods(typeSymbol).ToList();
                typeInfo.Events = GetEvents(typeSymbol).ToList();
                typeInfo.Constructors = GetConstructors(typeSymbol).ToList();
            }

            // Extract dependencies
            typeInfo.Dependencies = ExtractDependencies(typeSymbol).ToList();

            return typeInfo;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Warning: Failed to analyze type in {filePath}: {ex.Message}");
            return null;
        }
    }

    private List<string> GetModifiers(SyntaxNode node)
    {
        var modifiers = new List<string>();
        
        var tokenList = node switch
        {
            ClassDeclarationSyntax c => c.Modifiers,
            InterfaceDeclarationSyntax i => i.Modifiers,
            StructDeclarationSyntax s => s.Modifiers,
            EnumDeclarationSyntax e => e.Modifiers,
            RecordDeclarationSyntax r => r.Modifiers,
            _ => default
        };

        foreach (var modifier in tokenList)
        {
            modifiers.Add(modifier.Text);
        }

        return modifiers;
    }

    private string? GetXmlDocSummary(ISymbol symbol)
    {
        var xml = symbol.GetDocumentationCommentXml();
        if (string.IsNullOrWhiteSpace(xml))
            return null;

        try
        {
            var doc = XDocument.Parse($"<root>{xml}</root>");
            var summary = doc.Root?.Element("summary")?.Value.Trim();
            return summary;
        }
        catch
        {
            return null;
        }
    }

    private List<AttributeInfo> GetAttributes(ISymbol symbol)
    {
        return symbol.GetAttributes()
            .Select(attr => new AttributeInfo
            {
                Name = attr.AttributeClass?.Name ?? "Unknown",
                Arguments = attr.ConstructorArguments
                    .Select(a => a.ToCSharpString())
                    .ToList()
            })
            .ToList();
    }

    private IEnumerable<MemberInfo> GetFields(INamedTypeSymbol typeSymbol)
    {
        return typeSymbol.GetMembers()
            .OfType<IFieldSymbol>()
            .Where(f => !f.IsImplicitlyDeclared)
            .Select(field => new MemberInfo
            {
                Name = field.Name,
                Type = field.Type.ToDisplayString(),
                Signature = $"{string.Join(" ", GetSymbolModifiers(field))} {field.Type.ToDisplayString()} {field.Name}",
                Summary = GetXmlDocSummary(field),
                Modifiers = GetSymbolModifiers(field)
            });
    }

    private IEnumerable<MemberInfo> GetProperties(INamedTypeSymbol typeSymbol)
    {
        return typeSymbol.GetMembers()
            .OfType<IPropertySymbol>()
            .Where(p => !p.IsImplicitlyDeclared)
            .Select(prop => new MemberInfo
            {
                Name = prop.Name,
                Type = prop.Type.ToDisplayString(),
                Signature = BuildPropertySignature(prop),
                Summary = GetXmlDocSummary(prop),
                Modifiers = GetSymbolModifiers(prop)
            });
    }

    private IEnumerable<MemberInfo> GetMethods(INamedTypeSymbol typeSymbol)
    {
        return typeSymbol.GetMembers()
            .OfType<IMethodSymbol>()
            .Where(m => !m.IsImplicitlyDeclared && 
                       m.MethodKind == MethodKind.Ordinary &&
                       !m.Name.StartsWith("get_") && 
                       !m.Name.StartsWith("set_") &&
                       !m.Name.StartsWith("add_") &&
                       !m.Name.StartsWith("remove_"))
            .Select(method => new MemberInfo
            {
                Name = method.Name,
                Type = method.ReturnType.ToDisplayString(),
                Signature = BuildMethodSignature(method),
                Summary = GetXmlDocSummary(method),
                Returns = GetXmlDocReturns(method),
                Parameters = method.Parameters.Select(p => new ParameterInfo
                {
                    Name = p.Name,
                    Type = p.Type.ToDisplayString(),
                    Description = GetParameterDescription(method, p.Name),
                    DefaultValue = p.HasExplicitDefaultValue ? p.ExplicitDefaultValue?.ToString() : null
                }).ToList(),
                Modifiers = GetSymbolModifiers(method)
            });
    }

    private IEnumerable<MemberInfo> GetEvents(INamedTypeSymbol typeSymbol)
    {
        return typeSymbol.GetMembers()
            .OfType<IEventSymbol>()
            .Select(ev => new MemberInfo
            {
                Name = ev.Name,
                Type = ev.Type.ToDisplayString(),
                Signature = $"{string.Join(" ", GetSymbolModifiers(ev))} event {ev.Type.ToDisplayString()} {ev.Name}",
                Summary = GetXmlDocSummary(ev),
                Modifiers = GetSymbolModifiers(ev)
            });
    }

    private IEnumerable<MemberInfo> GetConstructors(INamedTypeSymbol typeSymbol)
    {
        return typeSymbol.Constructors
            .Where(c => !c.IsImplicitlyDeclared)
            .Select(ctor => new MemberInfo
            {
                Name = typeSymbol.Name,
                Type = "constructor",
                Signature = BuildMethodSignature(ctor),
                Summary = GetXmlDocSummary(ctor),
                Parameters = ctor.Parameters.Select(p => new ParameterInfo
                {
                    Name = p.Name,
                    Type = p.Type.ToDisplayString(),
                    Description = GetParameterDescription(ctor, p.Name),
                    DefaultValue = p.HasExplicitDefaultValue ? p.ExplicitDefaultValue?.ToString() : null
                }).ToList(),
                Modifiers = GetSymbolModifiers(ctor)
            });
    }

    private List<string> GetSymbolModifiers(ISymbol symbol)
    {
        var modifiers = new List<string>();

        if (symbol.DeclaredAccessibility != Accessibility.NotApplicable)
        {
            modifiers.Add(symbol.DeclaredAccessibility.ToString().ToLower());
        }

        if (symbol.IsStatic)
            modifiers.Add("static");
        if (symbol.IsAbstract)
            modifiers.Add("abstract");
        if (symbol.IsVirtual)
            modifiers.Add("virtual");
        if (symbol.IsOverride)
            modifiers.Add("override");
        if (symbol.IsSealed)
            modifiers.Add("sealed");

        if (symbol is IMethodSymbol method && method.IsAsync)
            modifiers.Add("async");

        return modifiers;
    }

    private string BuildPropertySignature(IPropertySymbol prop)
    {
        var modifiers = string.Join(" ", GetSymbolModifiers(prop));
        var accessors = new List<string>();
        
        if (prop.GetMethod != null)
            accessors.Add("get");
        if (prop.SetMethod != null)
        {
            var setModifier = prop.SetMethod.DeclaredAccessibility != prop.DeclaredAccessibility
                ? $"{prop.SetMethod.DeclaredAccessibility.ToString().ToLower()} "
                : "";
            accessors.Add($"{setModifier}set");
        }

        return $"{modifiers} {prop.Type.ToDisplayString()} {prop.Name} {{ {string.Join("; ", accessors)}; }}";
    }

    private string BuildMethodSignature(IMethodSymbol method)
    {
        var modifiers = string.Join(" ", GetSymbolModifiers(method));
        var typeParams = method.TypeParameters.Length > 0
            ? $"<{string.Join(", ", method.TypeParameters.Select(tp => tp.Name))}>"
            : "";
        var parameters = string.Join(", ", method.Parameters.Select(p => 
            $"{p.Type.ToDisplayString()} {p.Name}" + 
            (p.HasExplicitDefaultValue ? $" = {FormatDefaultValue(p.ExplicitDefaultValue)}" : "")));

        var returnType = method.MethodKind == MethodKind.Constructor 
            ? "" 
            : $"{method.ReturnType.ToDisplayString()} ";

        return $"{modifiers} {returnType}{method.Name}{typeParams}({parameters})";
    }

    private string FormatDefaultValue(object? value)
    {
        if (value == null) return "null";
        if (value is string s) return $"\"{s}\"";
        if (value is bool b) return b.ToString().ToLower();
        return value.ToString() ?? "null";
    }

    private string? GetXmlDocReturns(IMethodSymbol method)
    {
        var xml = method.GetDocumentationCommentXml();
        if (string.IsNullOrWhiteSpace(xml))
            return null;

        try
        {
            var doc = XDocument.Parse($"<root>{xml}</root>");
            var returns = doc.Root?.Element("returns")?.Value.Trim();
            return returns;
        }
        catch
        {
            return null;
        }
    }

    private string? GetParameterDescription(IMethodSymbol method, string paramName)
    {
        var xml = method.GetDocumentationCommentXml();
        if (string.IsNullOrWhiteSpace(xml))
            return null;

        try
        {
            var doc = XDocument.Parse($"<root>{xml}</root>");
            var param = doc.Root?.Elements("param")
                .FirstOrDefault(e => e.Attribute("name")?.Value == paramName);
            return param?.Value.Trim();
        }
        catch
        {
            return null;
        }
    }

    private IEnumerable<string> ExtractDependencies(INamedTypeSymbol typeSymbol)
    {
        var dependencies = new HashSet<string>();

        // Base type
        if (typeSymbol.BaseType != null && typeSymbol.BaseType.ToString() != "object")
        {
            var baseTypeName = typeSymbol.BaseType.ToString();
            if (baseTypeName != null)
                dependencies.Add(baseTypeName);
        }

        // Interfaces
        foreach (var iface in typeSymbol.Interfaces)
        {
            var ifaceName = iface.ToString();
            if (ifaceName != null)
                dependencies.Add(ifaceName);
        }

        // Field and property types
        foreach (var member in typeSymbol.GetMembers())
        {
            if (member is IFieldSymbol field && !field.IsImplicitlyDeclared)
            {
                AddTypeDependency(dependencies, field.Type);
            }
            else if (member is IPropertySymbol prop && !prop.IsImplicitlyDeclared)
            {
                AddTypeDependency(dependencies, prop.Type);
            }
            else if (member is IMethodSymbol method && !method.IsImplicitlyDeclared)
            {
                AddTypeDependency(dependencies, method.ReturnType);
                foreach (var param in method.Parameters)
                {
                    AddTypeDependency(dependencies, param.Type);
                }
            }
        }

        return dependencies
            .Where(d => !d.StartsWith("System.") && !d.StartsWith("Microsoft."))
            .OrderBy(d => d);
    }

    private void AddTypeDependency(HashSet<string> dependencies, ITypeSymbol type)
    {
        if (type is INamedTypeSymbol namedType)
        {
            if (!namedType.IsGenericType)
            {
                var typeName = namedType.ToString();
                if (typeName != null)
                    dependencies.Add(typeName);
            }
            else
            {
                // Add the generic type itself
                var genericName = namedType.OriginalDefinition.ToString();
                if (genericName != null)
                    dependencies.Add(genericName);

                // Add type arguments
                foreach (var typeArg in namedType.TypeArguments)
                {
                    AddTypeDependency(dependencies, typeArg);
                }
            }
        }
        else if (type is IArrayTypeSymbol arrayType)
        {
            AddTypeDependency(dependencies, arrayType.ElementType);
        }
    }

    private string GetRelativePath(string fullPath)
    {
        var relativePath = Path.GetRelativePath(_repositoryRoot, fullPath);
        return relativePath.Replace('\\', '/');
    }
}
