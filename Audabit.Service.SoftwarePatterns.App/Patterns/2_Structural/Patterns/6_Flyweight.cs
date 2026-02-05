namespace Audabit.Service.SoftwarePatterns.App.Patterns._2_Structural.Patterns;

// Flyweight Pattern: Reduces memory usage by sharing common state between multiple objects instead of storing it in each object.
// In this example:
// - We have many Tree objects in a forest, but trees of the same type share common properties (color, texture).
// - TreeType is the flyweight that contains intrinsic state (shared data).
// - Tree objects only store extrinsic state (unique data like position).
// - The key here is that we minimize memory by sharing immutable common data across many objects.
//
// MODERN C# USAGE:
// String interning is a built-in flyweight: string.Intern("text") reuses the same reference for identical strings.
// Record types with value equality are excellent for flyweights (immutable, structural equality).
// Memory<T> and Span<T> enable efficient memory sharing without copying.
// Use ConcurrentDictionary<TKey, Lazy<TValue>> for thread-safe flyweight factories in production code.
public static class Flyweight
{
    public static void Run()
    {
        Console.WriteLine("Flyweight Pattern: Sharing common state to reduce memory usage...");
        Console.WriteLine("Flyweight Pattern: Tree types are shared flyweights, individual trees only store unique position data.\n");

        var forest = new Forest();

        Console.WriteLine("Flyweight Pattern: --- Planting Trees ---");
        forest.PlantTree(10, 20, "Oak", "Green", "Oak texture");
        forest.PlantTree(15, 25, "Oak", "Green", "Oak texture");
        forest.PlantTree(30, 40, "Pine", "Dark Green", "Pine texture");
        forest.PlantTree(35, 45, "Pine", "Dark Green", "Pine texture");
        forest.PlantTree(50, 60, "Birch", "White", "Birch texture");

        Console.WriteLine("\nFlyweight Pattern: --- Drawing Forest ---");
        forest.Draw();

        Console.WriteLine($"\nFlyweight Pattern: --- Memory Statistics ---");
        Console.WriteLine($"Flyweight Pattern: Total trees planted: 5");
        Console.WriteLine($"Flyweight Pattern: Unique tree types (flyweights): {forest.GetTreeTypeCount()}");
        Console.WriteLine($"Flyweight Pattern: Memory saved by sharing common tree type data!");
    }
}

// Flyweight - intrinsic state (shared)
public sealed class TreeType(string name, string color, string texture)
{
    public string Name { get; } = name;
    public string Color { get; } = color;
    public string Texture { get; } = texture;

    public void Draw(int x, int y)
    {
        Console.WriteLine($"Flyweight Pattern: [TreeType] Drawing {Name} tree at ({x}, {y}) - Color: {Color}, Texture: {Texture}");
    }
}

// Context - extrinsic state (unique to each object)
public sealed class Tree(int x, int y, TreeType type)
{
    private int X { get; } = x;
    private int Y { get; } = y;
    private TreeType Type { get; } = type;

    public void Draw()
    {
        Type.Draw(X, Y);
    }
}

// Flyweight factory
public sealed class TreeFactory
{
    private readonly Dictionary<string, TreeType> _treeTypes = [];

    public TreeType GetTreeType(string name, string color, string texture)
    {
        var key = $"{name}_{color}_{texture}";

        if (!_treeTypes.TryGetValue(key, out var value))
        {
            Console.WriteLine($"Flyweight Pattern: [Factory] Creating new TreeType flyweight: {name}");
            value = new TreeType(name, color, texture);
            _treeTypes[key] = value;
        }
        else
        {
            Console.WriteLine($"Flyweight Pattern: [Factory] Reusing existing TreeType flyweight: {name}");
        }

        return value;
    }

    public int GetTreeTypeCount()
    {
        return _treeTypes.Count;
    }
}

// Client
public sealed class Forest
{
    private readonly List<Tree> _trees = [];
    private readonly TreeFactory _factory = new();

    public void PlantTree(int x, int y, string name, string color, string texture)
    {
        var type = _factory.GetTreeType(name, color, texture);
        var tree = new Tree(x, y, type);
        _trees.Add(tree);
    }

    public void Draw()
    {
        foreach (var tree in _trees)
        {
            tree.Draw();
        }
    }

    public int GetTreeTypeCount()
    {
        return _factory.GetTreeTypeCount();
    }
}