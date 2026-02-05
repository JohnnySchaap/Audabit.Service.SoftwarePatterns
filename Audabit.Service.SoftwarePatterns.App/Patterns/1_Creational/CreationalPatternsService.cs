namespace Audabit.Service.SoftwarePatterns.App.Patterns._1_Creational;

/// <summary>
/// Service implementation for demonstrating Creational Design Patterns.
/// </summary>
public sealed class CreationalPatternsService : ICreationalPatternsService
{
    public void Singleton()
    {
        Console.WriteLine("\n=== Singleton Pattern Demo ===\n");
        Patterns.Singleton.Run();
        Console.WriteLine("\n=== Singleton Pattern: Demonstration complete.===\n");
    }

    public void LazyInitialization()
    {
        Console.WriteLine("\n=== Lazy Initialization Pattern Demo ===\n");
        Patterns.LazyInitialization.Run();
        Console.WriteLine("\n=== Lazy Initialization Pattern: Demonstration complete.===\n");
    }

    public void SimpleFactory()
    {
        Console.WriteLine("\n=== Simple Factory Pattern Demo ===\n");
        Patterns.SimpleFactory.Run();
        Console.WriteLine("\n=== Simple Factory Pattern: Demonstration complete.===\n");
    }

    public void FactoryMethod()
    {
        Console.WriteLine("\n=== Factory Method Pattern Demo ===\n");
        Patterns.FactoryMethod.Run();
        Console.WriteLine("\n=== Factory Method Pattern: Demonstration complete.===\n");
    }

    public void AbstractFactory()
    {
        Console.WriteLine("\n=== Abstract Factory Pattern Demo ===\n");
        Patterns.AbstractFactory.Run();
        Console.WriteLine("\n=== Abstract Factory Pattern: Demonstration complete.===\n");
    }

    public void Builder()
    {
        Console.WriteLine("\n=== Builder Pattern Demo ===\n");
        Patterns.Builder.Run();
        Console.WriteLine("\n=== Builder Pattern: Demonstration complete.===\n");
    }

    public void Prototype()
    {
        Console.WriteLine("\n=== Prototype Pattern Demo ===\n");
        Patterns.Prototype.Run();
        Console.WriteLine("\n=== Prototype Pattern: Demonstration complete.===\n");
    }

    public void ObjectPool()
    {
        Console.WriteLine("\n=== Object Pool Pattern Demo ===\n");
        Patterns.ObjectPool.Run();
        Console.WriteLine("\n=== Object Pool Pattern: Demonstration complete.===\n");
    }

    public void DependencyInjection()
    {
        Console.WriteLine("\n=== Dependency Injection Pattern Demo ===\n");
        Patterns.DependencyInjection.Run();
        Console.WriteLine("\n=== Dependency Injection Pattern: Demonstration complete.===\n");
    }
}