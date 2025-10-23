using Tools.CodeDocGenerator;

var repositoryRoot = args.Length > 0 
    ? args[0] 
    : FindRepositoryRoot();

var outputPath = args.Length > 1 
    ? args[1] 
    : Path.Combine(repositoryRoot, "docs");

Console.WriteLine("╔════════════════════════════════════════════════════════════════════════╗");
Console.WriteLine("║  Code Documentation Generator - Roslyn-based Markdown Documentation   ║");
Console.WriteLine("╚════════════════════════════════════════════════════════════════════════╝");
Console.WriteLine();
Console.WriteLine($"Repository Root: {repositoryRoot}");
Console.WriteLine($"Output Path:     {outputPath}");
Console.WriteLine();

var analyzer = new CodeAnalyzer(repositoryRoot);
var types = await analyzer.AnalyzeAsync();

Console.WriteLine($"✓ Analyzed {types.Count} types across the repository");
Console.WriteLine();

var generator = new MarkdownGenerator(outputPath, repositoryRoot);
await generator.GenerateAsync(types);

Console.WriteLine();
Console.WriteLine("✓ Documentation generation complete!");
Console.WriteLine($"  Output location: {outputPath}");
Console.WriteLine();
Console.WriteLine("You can now commit the generated documentation to your repository.");

static string FindRepositoryRoot()
{
    // Start from the current directory and search upward for .git folder
    var currentDir = Directory.GetCurrentDirectory();
    
    while (currentDir != null)
    {
        if (Directory.Exists(Path.Combine(currentDir, ".git")))
        {
            return currentDir;
        }
        
        var parent = Directory.GetParent(currentDir);
        if (parent == null)
            break;
        currentDir = parent.FullName;
    }
    
    // Fallback to current directory if no .git found
    return Directory.GetCurrentDirectory();
}
