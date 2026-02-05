namespace Audabit.Service.SoftwarePatterns.App.Principles._0_Principles;

/// <summary>
/// Service implementation for demonstrating Software Engineering Principles.
/// </summary>
public sealed class PrinciplesService : IPrinciplesService
{
    public void Solid()
    {
        Console.WriteLine("\n=== SOLID Principles Demo ===\n");
        Principles.SOLID.Run();
        Console.WriteLine("\n=== SOLID Principles: Demonstration complete.===\n");
    }

    public void Dry()
    {
        Console.WriteLine("\n=== DRY Principle Demo ===\n");
        Principles.DRY.Run();
        Console.WriteLine("\n=== DRY Principle: Demonstration complete.===\n");
    }

    public void Kiss()
    {
        Console.WriteLine("\n=== KISS Principle Demo ===\n");
        Principles.KISS.Run();
        Console.WriteLine("\n=== KISS Principle: Demonstration complete.===\n");
    }

    public void Yagni()
    {
        Console.WriteLine("\n=== YAGNI Principle Demo ===\n");
        Principles.YAGNI.Run();
        Console.WriteLine("\n=== YAGNI Principle: Demonstration complete.===\n");
    }

    public void SeparationOfConcerns()
    {
        Console.WriteLine("\n=== Separation of Concerns Principle Demo ===\n");
        Principles.SeparationOfConcerns.Run();
        Console.WriteLine("\n=== Separation of Concerns Principle: Demonstration complete.===\n");
    }

    public void Encapsulation()
    {
        Console.WriteLine("\n=== Encapsulation Principle Demo ===\n");
        Principles.Encapsulation.Run();
        Console.WriteLine("\n=== Encapsulation Principle: Demonstration complete.===\n");
    }

    public void CompositionOverInheritance()
    {
        Console.WriteLine("\n=== Composition Over Inheritance Principle Demo ===\n");
        Principles.CompositionOverInheritance.Run();
        Console.WriteLine("\n=== Composition Over Inheritance Principle: Demonstration complete.===\n");
    }
}