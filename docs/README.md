# Documentation Guide

This repository includes comprehensive auto-generated documentation for all C# code.

## Viewing Documentation

- **Browse Online**: Navigate to the `/docs` folder on GitHub
- **Start Here**: [docs/index.md](index.md) — Main documentation index
- **By Namespace**: [docs/namespaces/](namespaces/) — Browse by namespace
- **By Type**: [docs/classes/](classes/) — Individual type documentation

## Documentation Structure

```
/docs
├── index.md                    # Main index with statistics and navigation
├── namespaces/                 # Namespace overview pages
│   ├── Indiko.Maui.Controls.Chat.md
│   ├── Indiko.Maui.Controls.Chat.Models.md
│   └── ...
├── classes/                    # Individual type documentation
│   ├── ChatView.md
│   ├── ChatMessage.md
│   └── ...
└── diagrams/                   # Mermaid diagrams
    ├── class-hierarchy.mmd
    └── namespace-dependencies.mmd
```

## What's Documented

For each type in the codebase, you'll find:

- **Type Information**: Name, namespace, source file location
- **Signature**: Complete C# signature with modifiers
- **Summary**: XML documentation comments
- **Relationships**: Base types, interfaces, dependencies
- **Members**: 
  - Properties with accessors
  - Methods with parameters and return types
  - Fields and events
  - Constructors

## Diagrams

The documentation includes Mermaid diagrams that visualize:

- **Class Hierarchy**: Inheritance and interface implementations
- **Namespace Dependencies**: Cross-namespace references

These diagrams are embedded in the index and render directly in GitHub.

## Regenerating Documentation

To regenerate the documentation after code changes:

```bash
cd /path/to/repository
dotnet run --project tools/Tools.CodeDocGenerator/Tools.CodeDocGenerator.csproj
```

This will:
1. Analyze all C# files in the repository
2. Extract type information using Roslyn
3. Generate/update Markdown documentation in `/docs`
4. Create Mermaid diagrams

See [tools/Tools.CodeDocGenerator/README.md](../tools/Tools.CodeDocGenerator/README.md) for more details.

## Contributing

When adding or modifying code:

1. Add XML documentation comments to your code
2. Regenerate documentation using the tool
3. Commit both code and documentation changes together

This keeps documentation in sync with the code and makes reviews easier.

## Features

- ✓ GitHub-native Markdown format
- ✓ Mermaid diagrams (render natively in GitHub)
- ✓ Cross-references between types
- ✓ XML documentation integration
- ✓ Version controlled (track changes via Git)
- ✓ No external dependencies for viewing
- ✓ Searchable and navigable

## Statistics

Current documentation coverage:
- 67 types documented
- 12 namespaces
- 184 methods
- 160 properties
- 78 documentation files

*Last updated: 2025-10-23*
