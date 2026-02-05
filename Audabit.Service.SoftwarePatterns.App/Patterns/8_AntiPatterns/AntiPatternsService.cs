namespace Audabit.Service.SoftwarePatterns.App.Patterns._8_AntiPatterns;

/// <summary>
/// Service implementation for demonstrating Anti-Patterns (what to avoid).
/// </summary>
public sealed class AntiPatternsService : IAntiPatternsService
{
    public void SpaghettiCode()
    {
        Console.WriteLine("\n=== Spaghetti Code Anti-Pattern Demo ===\n");
        Patterns.SpaghettiCode.Run();
        Console.WriteLine("\n=== Spaghetti Code Anti-Pattern: Demonstration complete.===\n");
    }

    public void GodObject()
    {
        Console.WriteLine("\n=== God Object Anti-Pattern Demo ===\n");
        Patterns.GodObject.Run();
        Console.WriteLine("\n=== God Object Anti-Pattern: Demonstration complete.===\n");
    }

    public void GoldenHammer()
    {
        Console.WriteLine("\n=== Golden Hammer Anti-Pattern Demo ===\n");
        Patterns.GoldenHammer.Run();
        Console.WriteLine("\n=== Golden Hammer Anti-Pattern: Demonstration complete.===\n");
    }

    public void ReinventingTheWheel()
    {
        Console.WriteLine("\n=== Reinventing the Wheel Anti-Pattern Demo ===\n");
        Patterns.ReinventingTheWheel.Run();
        Console.WriteLine("\n=== Reinventing the Wheel Anti-Pattern: Demonstration complete.===\n");
    }

    public void CopyPasteProgramming()
    {
        Console.WriteLine("\n=== Copy Paste Programming Anti-Pattern Demo ===\n");
        Patterns.CopyPasteProgramming.Run();
        Console.WriteLine("\n=== Copy Paste Programming Anti-Pattern: Demonstration complete.===\n");
    }
}