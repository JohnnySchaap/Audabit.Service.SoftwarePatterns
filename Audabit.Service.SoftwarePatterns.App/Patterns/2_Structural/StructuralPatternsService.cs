namespace Audabit.Service.SoftwarePatterns.App.Patterns._2_Structural;

/// <summary>
/// Service implementation for demonstrating Structural Design Patterns.
/// </summary>
public sealed class StructuralPatternsService : IStructuralPatternsService
{
    public void Adapter()
    {
        Console.WriteLine("\n=== Adapter Pattern Demo ===\n");
        Patterns.Adapter.Run();
        Console.WriteLine("\n=== Adapter Pattern: Demonstration complete.===\n");
    }

    public void Bridge()
    {
        Console.WriteLine("\n=== Bridge Pattern Demo ===\n");
        Patterns.Bridge.Run();
        Console.WriteLine("\n=== Bridge Pattern: Demonstration complete.===\n");
    }

    public void Composite()
    {
        Console.WriteLine("\n=== Composite Pattern Demo ===\n");
        Patterns.Composite.Run();
        Console.WriteLine("\n=== Composite Pattern: Demonstration complete.===\n");
    }

    public void Decorator()
    {
        Console.WriteLine("\n=== Decorator Pattern Demo ===\n");
        Patterns.Decorator.Run();
        Console.WriteLine("\n=== Decorator Pattern: Demonstration complete.===\n");
    }

    public void Facade()
    {
        Console.WriteLine("\n=== Facade Pattern Demo ===\n");
        Patterns.Facade.Run();
        Console.WriteLine("\n=== Facade Pattern: Demonstration complete.===\n");
    }

    public void Flyweight()
    {
        Console.WriteLine("\n=== Flyweight Pattern Demo ===\n");
        Patterns.Flyweight.Run();
        Console.WriteLine("\n=== Flyweight Pattern: Demonstration complete.===\n");
    }

    public void Proxy()
    {
        Console.WriteLine("\n=== Proxy Pattern Demo ===\n");
        Patterns.Proxy.Run();
        Console.WriteLine("\n=== Proxy Pattern: Demonstration complete.===\n");
    }

    public void MarkerInterface()
    {
        Console.WriteLine("\n=== Marker Interface Pattern Demo ===\n");
        Patterns.MarkerInterface.Run();
        Console.WriteLine("\n=== Marker Interface Pattern: Demonstration complete.===\n");
    }

    public void PipesAndFilters()
    {
        Console.WriteLine("\n=== Pipes and Filters Pattern Demo ===\n");
        Patterns.PipesAndFilters.Run();
        Console.WriteLine("\n=== Pipes and Filters Pattern: Demonstration complete.===\n");
    }
}