namespace Audabit.Service.SoftwarePatterns.App.Patterns._2_Structural.Patterns;

// Composite Pattern: Composes objects into tree structures to represent part-whole hierarchies, allowing individual objects and compositions to be treated uniformly.
// In this example:
// - We create a file system structure with files and directories.
// - Both File and Directory implement IFileSystemItem, allowing uniform treatment.
// - Directories can contain files or other directories (tree structure).
// - This is NOT just about objects containing other objects (regular composition).
// - This is about objects containing other objects OF THE SAME TYPE (through the same interface).
// - Directory contains List<IFileSystemItem> - which can be Files OR other Directories.
// - Both containers (Directory) and items (File) share the same interface (IFileSystemItem).
// - The key here is that we can treat individual objects (files) and compositions (directories) through the same interface, creating a tree structure where you can nest infinitely.
//
// MODERN C# USAGE:
// Composite pattern is commonly used in modern .NET for hierarchical data structures:
// - ASP.NET Core: Middleware pipeline (each middleware can contain other middleware)
// - EF Core: Expression trees for LINQ queries (BinaryExpression contains left/right expressions)
// - UI frameworks: WPF/MAUI visual tree (Panel contains Controls, which can be other Panels)
// - File systems, organization charts, menu structures all use this pattern
// Modern C# features like collection expressions [] and pattern matching make working with composites easier.
public static class Composite
{
    public static void Run()
    {
        Console.WriteLine("Composite Pattern: Building tree structures with uniform component treatment...");
        Console.WriteLine("Composite Pattern: Files and directories are treated uniformly through the same interface.\n");

        Console.WriteLine("Composite Pattern: --- Creating File System Structure ---");
        var file1 = new File("document.txt", 100);
        var file2 = new File("image.png", 500);
        var file3 = new File("data.json", 200);

        var subDir = new Directory("SubFolder");
        subDir.Add(file3);

        var rootDir = new Directory("Root");
        rootDir.Add(file1);
        rootDir.Add(file2);
        rootDir.Add(subDir);

        Console.WriteLine("\nComposite Pattern: --- Displaying File System ---");
        rootDir.Display(0);

        Console.WriteLine($"\nComposite Pattern: --- Calculating Total Size ---");
        Console.WriteLine($"Composite Pattern: Total size: {rootDir.GetSize()} KB");
    }
}

// Component interface
public interface IFileSystemItem
{
    string Name { get; }
    int GetSize();
    void Display(int indent);
}

// Leaf
public sealed class File(string name, int size) : IFileSystemItem
{
    public string Name { get; } = name;
    private int Size { get; } = size;

    public int GetSize()
    {
        return Size;
    }

    public void Display(int indent)
    {
        Console.WriteLine($"{new string(' ', indent)}Composite Pattern: [File] {Name} ({Size} KB)");
    }
}

// Composite
public sealed class Directory(string name) : IFileSystemItem
{
    public string Name { get; } = name;
    private readonly List<IFileSystemItem> _items = [];

    public void Add(IFileSystemItem item)
    {
        _items.Add(item);
    }

    public void Remove(IFileSystemItem item)
    {
        _items.Remove(item);
    }

    public int GetSize()
    {
        return _items.Sum(item => item.GetSize());
    }

    public void Display(int indent)
    {
        Console.WriteLine($"{new string(' ', indent)}Composite Pattern: [Directory] {Name}/");
        foreach (var item in _items)
        {
            item.Display(indent + 2);
        }
    }
}