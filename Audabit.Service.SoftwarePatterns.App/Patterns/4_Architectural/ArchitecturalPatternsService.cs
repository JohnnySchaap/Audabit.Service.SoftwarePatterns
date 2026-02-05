namespace Audabit.Service.SoftwarePatterns.App.Patterns._4_Architectural;

/// <summary>
/// Service implementation for demonstrating Architectural / System Patterns.
/// </summary>
public sealed class ArchitecturalPatternsService : IArchitecturalPatternsService
{
    public void Layered()
    {
        Console.WriteLine("\n=== Layered Pattern Demo ===\n");
        Patterns.Layered.Run();
        Console.WriteLine("\n=== Layered Pattern: Demonstration complete.===\n");
    }

    public void Hexagonal()
    {
        Console.WriteLine("\n=== Hexagonal Pattern Demo ===\n");
        Patterns.Hexagonal.Run();
        Console.WriteLine("\n=== Hexagonal Pattern: Demonstration complete.===\n");
    }

    public void Microservices()
    {
        Console.WriteLine("\n=== Microservices Pattern Demo ===\n");
        Patterns.Microservices.Run();
        Console.WriteLine("\n=== Microservices Pattern: Demonstration complete.===\n");
    }

    public void EventDriven()
    {
        Console.WriteLine("\n=== Event-Driven Pattern Demo ===\n");
        Patterns.EventDriven.Run();
        Console.WriteLine("\n=== Event-Driven Pattern: Demonstration complete.===\n");
    }

    public void CQRS()
    {
        Console.WriteLine("\n=== CQRS Pattern Demo ===\n");
        Patterns.CQRS.Run();
        Console.WriteLine("\n=== CQRS Pattern: Demonstration complete.===\n");
    }

    public void EventSourcing()
    {
        Console.WriteLine("\n=== Event Sourcing Pattern Demo ===\n");
        Patterns.EventSourcing.Run();
        Console.WriteLine("\n=== Event Sourcing Pattern: Demonstration complete.===\n");
    }

    public void Saga()
    {
        Console.WriteLine("\n=== Saga Pattern Demo ===\n");
        Patterns.Saga.Run();
        Console.WriteLine("\n=== Saga Pattern: Demonstration complete.===\n");
    }

    public void ApiGateway()
    {
        Console.WriteLine("\n=== API Gateway Pattern Demo ===\n");
        Patterns.ApiGateway.Run();
        Console.WriteLine("\n=== API Gateway Pattern: Demonstration complete.===\n");
    }

    public void StranglerFig()
    {
        Console.WriteLine("\n=== Strangler Fig Pattern Demo ===\n");
        Patterns.StranglerFig.Run();
        Console.WriteLine("\n=== Strangler Fig Pattern: Demonstration complete.===\n");
    }
}