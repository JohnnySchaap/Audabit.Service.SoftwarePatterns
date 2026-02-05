namespace Audabit.Service.SoftwarePatterns.App.Patterns._3_Behavioral;

/// <summary>
/// Service implementation for demonstrating Behavioral Design Patterns.
/// </summary>
public sealed class BehavioralPatternsService : IBehavioralPatternsService
{
    public void Observer()
    {
        Console.WriteLine("\n=== Observer Pattern Demo ===\n");
        Patterns.Observer.Run();
        Console.WriteLine("\n=== Observer Pattern: Demonstration complete.===\n");
    }

    public void Strategy()
    {
        Console.WriteLine("\n=== Strategy Pattern Demo ===\n");
        Patterns.Strategy.Run();
        Console.WriteLine("\n=== Strategy Pattern: Demonstration complete.===\n");
    }

    public void Command()
    {
        Console.WriteLine("\n=== Command Pattern Demo ===\n");
        Patterns.Command.Run();
        Console.WriteLine("\n=== Command Pattern: Demonstration complete.===\n");
    }

    public void ChainOfResponsibility()
    {
        Console.WriteLine("\n=== Chain of Responsibility Pattern Demo ===\n");
        Patterns.ChainOfResponsibility.Run();
        Console.WriteLine("\n=== Chain of Responsibility Pattern: Demonstration complete.===\n");
    }

    public void Mediator()
    {
        Console.WriteLine("\n=== Mediator Pattern Demo ===\n");
        Patterns.Mediator.Run();
        Console.WriteLine("\n=== Mediator Pattern: Demonstration complete.===\n");
    }

    public void Memento()
    {
        Console.WriteLine("\n=== Memento Pattern Demo ===\n");
        Patterns.Memento.Run();
        Console.WriteLine("\n=== Memento Pattern: Demonstration complete.===\n");
    }

    public void State()
    {
        Console.WriteLine("\n=== State Pattern Demo ===\n");
        Patterns.State.Run();
        Console.WriteLine("\n=== State Pattern: Demonstration complete.===\n");
    }

    public void TemplateMethod()
    {
        Console.WriteLine("\n=== Template Method Pattern Demo ===\n");
        Patterns.TemplateMethod.Run();
        Console.WriteLine("\n=== Template Method Pattern: Demonstration complete.===\n");
    }

    public void Visitor()
    {
        Console.WriteLine("\n=== Visitor Pattern Demo ===\n");
        Patterns.Visitor.Run();
        Console.WriteLine("\n=== Visitor Pattern: Demonstration complete.===\n");
    }

    public void Iterator()
    {
        Console.WriteLine("\n=== Iterator Pattern Demo ===\n");
        Patterns.Iterator.Run();
        Console.WriteLine("\n=== Iterator Pattern: Demonstration complete.===\n");
    }
}