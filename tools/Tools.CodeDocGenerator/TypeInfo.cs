namespace Tools.CodeDocGenerator;

public class TypeInfo
{
    public required string Name { get; set; }
    public required string FullName { get; set; }
    public required string Namespace { get; set; }
    public required string FilePath { get; set; }
    public required TypeKind Kind { get; set; }
    public string? Summary { get; set; }
    public string? Remarks { get; set; }
    public List<string> Modifiers { get; set; } = new();
    public List<string> TypeParameters { get; set; } = new();
    public string? BaseType { get; set; }
    public List<string> Interfaces { get; set; } = new();
    public List<MemberInfo> Fields { get; set; } = new();
    public List<MemberInfo> Properties { get; set; } = new();
    public List<MemberInfo> Methods { get; set; } = new();
    public List<MemberInfo> Events { get; set; } = new();
    public List<MemberInfo> Constructors { get; set; } = new();
    public List<string> EnumMembers { get; set; } = new();
    public List<AttributeInfo> Attributes { get; set; } = new();
    public List<string> Dependencies { get; set; } = new();
}

public enum TypeKind
{
    Class,
    Interface,
    Struct,
    Enum,
    Record,
    Delegate
}

public class MemberInfo
{
    public required string Name { get; set; }
    public required string Type { get; set; }
    public required string Signature { get; set; }
    public string? Summary { get; set; }
    public string? Returns { get; set; }
    public List<ParameterInfo> Parameters { get; set; } = new();
    public List<string> Modifiers { get; set; } = new();
    public string? Value { get; set; } // For fields with initializers or enum members
}

public class ParameterInfo
{
    public required string Name { get; set; }
    public required string Type { get; set; }
    public string? Description { get; set; }
    public string? DefaultValue { get; set; }
}

public class AttributeInfo
{
    public required string Name { get; set; }
    public List<string> Arguments { get; set; } = new();
}
