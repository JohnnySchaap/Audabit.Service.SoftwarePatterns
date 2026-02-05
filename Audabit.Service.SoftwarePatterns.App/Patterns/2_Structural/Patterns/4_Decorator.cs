namespace Audabit.Service.SoftwarePatterns.App.Patterns._2_Structural.Patterns;

// Decorator Pattern: Adds additional functionality to an object dynamically at runtime without modifying the original class.
// In this example:
// - We start with a simple Coffee and dynamically add decorators (Milk, Sugar).
// - Each decorator wraps the previous object and adds its own behavior.
// - Multiple decorators can be stacked to combine features.
// - The key here is that decorators maintain the same interface as the base object, allowing flexible composition without class explosion.
//
// MODERN C# USAGE:
// In production code with dependency injection, decorators are registered in the DI container:
//   services.AddScoped<IStarWarsService, StarWarsService>();
//   services.Decorate<IStarWarsService, LoggingDecorator>();
//   services.Decorate<IStarWarsService, CachingDecorator>();
// The DI container automatically wires them together. See Scrutor library for .Decorate() extension.
public static class Decorator
{
#pragma warning disable CA1859 // Use concrete types when possible - intentionally using interface to demonstrate pattern
    public static void Run()
    {
        Console.WriteLine("Decorator Pattern: Adding functionality dynamically at runtime...");
        Console.WriteLine("Decorator Pattern: Decorators wrap objects to add behavior without modifying the original class.\n");

        Console.WriteLine("Decorator Pattern: --- Simple Coffee ---");
        IBeverage coffee = new Coffee();
        Console.WriteLine($"Decorator Pattern: {coffee.GetDescription()}");
        Console.WriteLine($"Decorator Pattern: Cost: ${coffee.GetCost():F2}");

        Console.WriteLine("\nDecorator Pattern: --- Coffee with Milk ---");
        IBeverage coffeeWithMilk = new MilkDecorator(new Coffee());
        Console.WriteLine($"Decorator Pattern: {coffeeWithMilk.GetDescription()}");
        Console.WriteLine($"Decorator Pattern: Cost: ${coffeeWithMilk.GetCost():F2}");

        Console.WriteLine("\nDecorator Pattern: --- Coffee with Milk and Sugar ---");
        IBeverage deluxeCoffee = new SugarDecorator(new MilkDecorator(new Coffee()));
        Console.WriteLine($"Decorator Pattern: {deluxeCoffee.GetDescription()}");
        Console.WriteLine($"Decorator Pattern: Cost: ${deluxeCoffee.GetCost():F2}");
    }
#pragma warning restore CA1859 // Use concrete types when possible
}

// Component interface
public interface IBeverage
{
    string GetDescription();
    double GetCost();
}

// Concrete component
public sealed class Coffee : IBeverage
{
    public string GetDescription()
    {
        return "Coffee";
    }

    public double GetCost()
    {
        return 2.00;
    }
}

// Base decorator
public abstract class BeverageDecorator(IBeverage beverage) : IBeverage
{
    protected IBeverage Beverage { get; } = beverage;
    public abstract string GetDescription();
    public abstract double GetCost();
}

// Concrete decorators
public sealed class MilkDecorator(IBeverage beverage) : BeverageDecorator(beverage)
{
    public override string GetDescription()
    {
        return $"{Beverage.GetDescription()}, Milk";
    }

    public override double GetCost()
    {
        return Beverage.GetCost() + 0.50;
    }
}

public sealed class SugarDecorator(IBeverage beverage) : BeverageDecorator(beverage)
{
    public override string GetDescription()
    {
        return $"{Beverage.GetDescription()}, Sugar";
    }

    public override double GetCost()
    {
        return Beverage.GetCost() + 0.25;
    }
}