# Code Documentation Generator

A .NET console tool that analyzes C# source code using Roslyn and generates comprehensive Markdown documentation for the entire repository.

## Features

- **Comprehensive Analysis**: Analyzes all C# files in the repository
  - Classes, interfaces, structs, enums, and records
  - Methods, properties, fields, events, and constructors
  - XML documentation comments
  - Inheritance and interface implementations
  - Type dependencies and relationships

- **Markdown Documentation**: Generates clean, readable Markdown files
  - Individual files for each type
  - Namespace overview pages
  - Cross-references between types
  - Method signatures with parameters and return types
  - GitHub-compatible formatting

- **Mermaid Diagrams**: Generates visual diagrams
  - Class hierarchy diagrams
  - Namespace dependency graphs

- **Index Generation**: Creates a comprehensive index
  - Statistics overview
  - Hierarchical navigation by namespace
  - Alphabetical type listing

## Usage

### Basic Usage

Navigate to the repository root and run:

```bash
dotnet run --project tools/Tools.CodeDocGenerator/Tools.CodeDocGenerator.csproj
```

This will:
- Analyze all C# files in the repository
- Generate documentation in the `/docs` folder

### Custom Paths

You can specify custom paths for the repository root and output directory:

```bash
dotnet run --project tools/Tools.CodeDocGenerator/Tools.CodeDocGenerator.csproj -- <repository-path> <output-path>
```

**Example:**
```bash
dotnet run --project tools/Tools.CodeDocGenerator/Tools.CodeDocGenerator.csproj -- /path/to/repo /path/to/output
```

## Output Structure

The tool generates the following directory structure:

```
/docs
├── index.md                          # Main index with overview
├── namespaces/                       # Namespace documentation
│   ├── Namespace.One.md
│   └── Namespace.Two.md
├── classes/                          # Type documentation
│   ├── ClassName.md
│   ├── InterfaceName.md
│   └── EnumName.md
└── diagrams/                         # Mermaid diagrams
    ├── class-hierarchy.mmd
    └── namespace-dependencies.mmd
```

## Documentation Format

### Type Documentation

Each type gets its own Markdown file with:

- **Header**: Namespace, type kind, source file
- **Summary**: XML documentation summary
- **Signature**: Complete C# signature
- **Relationships**: Base types, interfaces, dependencies
- **Members**: Fields, properties, methods, events, constructors
- **Cross-references**: Links to related types

### Namespace Documentation

Each namespace gets an overview page with:

- Type statistics
- List of all types in the namespace
- Links to individual type documentation

### Index

The main index provides:

- Repository-wide statistics
- Links to all namespaces
- Complete alphabetical type listing
- Links to diagrams

## Requirements

- .NET 9.0 SDK
- Microsoft.CodeAnalysis.CSharp 4.11.0 (Roslyn)

## Building

```bash
cd tools/Tools.CodeDocGenerator
dotnet build
```

## How It Works

1. **Analysis Phase**:
   - Scans repository for all `.cs` files
   - Parses files using Roslyn syntax trees
   - Extracts type information and relationships
   - Reads XML documentation comments

2. **Generation Phase**:
   - Creates directory structure
   - Generates Markdown for each type
   - Generates namespace overviews
   - Creates Mermaid diagrams
   - Builds comprehensive index

3. **Output**:
   - All documentation written to `/docs` folder
   - Ready to commit to repository
   - Viewable directly in GitHub

## Benefits

- **Version Controlled**: Documentation lives in the repository
- **Always Up-to-Date**: Regenerate anytime code changes
- **GitHub Native**: Uses Markdown and Mermaid (GitHub-supported)
- **No Build Dependencies**: Pure Markdown, no external viewers needed
- **Diff Friendly**: Changes in docs tracked via Git
- **Cross-Referenced**: Easy navigation between related types

## Notes

- The tool skips `bin/`, `obj/`, and `.git/` directories
- Invalid or incomplete files are logged but don't stop generation
- Mermaid diagrams work natively in GitHub's Markdown renderer
- Cross-references use relative links for portability
