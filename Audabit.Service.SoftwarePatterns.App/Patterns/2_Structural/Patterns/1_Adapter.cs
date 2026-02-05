namespace Audabit.Service.SoftwarePatterns.App.Patterns._2_Structural.Patterns;

// Adapter Pattern: Converts the interface of a class into another interface that clients expect, allowing incompatible classes to work together.
// In this example:
// - We have a legacy RectangleShape class that doesn't implement our IShape interface.
// - The RectangleAdapter adapts the legacy class to work with our IShape interface.
// - Client code can now use both modern shapes and legacy shapes through the same interface.
// - The key here is that the adapter wraps the legacy object and translates method calls between incompatible interfaces.
//
// MODERN C# USAGE:
// This implementation uses the Object Adapter pattern (composition-based):
// - The adapter CONTAINS an instance of the adaptee (RectangleLegacy) as a field.
// - Uses composition instead of inheritance - follows "composition over inheritance" principle.
// - This is the ONLY practical approach in C# since C# doesn't support multiple inheritance.
// - More flexible - can adapt a class and all its subclasses at runtime.
// Class Adapter (inheritance-based) is NOT possible in C#:
// - Would require inheriting from both IShape AND RectangleLegacy (multiple inheritance).
// - Only available in languages like C++ or Python that support multiple inheritance.
// - Less flexible - can only adapt the specific class inherited from.
// In modern .NET, use dependency injection and interfaces for loose coupling:
//   services.AddScoped<IShape, RectangleAdapter>();
// Adapter pattern is commonly used in ASP.NET Core for integrating legacy libraries with modern interfaces.
public static class Adapter
{
#pragma warning disable CA1859 // Use concrete types when possible - intentionally using interface to demonstrate pattern
    public static void Run()
    {
        Console.WriteLine("Adapter Pattern: Adapting incompatible interfaces to work together...");
        Console.WriteLine("Adapter Pattern: Using adapter to make legacy code compatible with new interfaces.\n");

        Console.WriteLine("Adapter Pattern: --- Using Modern Shape ---");
        IShape circle = new Circle();
        circle.Draw();

        Console.WriteLine("\nAdapter Pattern: --- Using Legacy Shape via Adapter ---");
        RectangleLegacy legacyRectangle = new(10, 20);
        IShape rectangle = new RectangleAdapter(legacyRectangle);
        rectangle.Draw();
    }
#pragma warning restore CA1859 // Use concrete types when possible
}

// Modern interface that all shapes should implement
public interface IShape
{
    void Draw();
}

// Modern implementation
public sealed class Circle : IShape
{
    public void Draw()
    {
        Console.WriteLine("Adapter Pattern: [Circle] Drawing a circle using modern interface");
    }
}

// Legacy class that doesn't implement IShape
public sealed class RectangleLegacy(int width, int height)
{
    public int Width { get; } = width;
    public int Height { get; } = height;

    public void Display()
    {
        Console.WriteLine($"Adapter Pattern: [Legacy Rectangle] Displaying rectangle {Width}x{Height} (legacy method)");
    }
}

// Adapter that makes RectangleLegacy compatible with IShape
public sealed class RectangleAdapter(RectangleLegacy rectangle) : IShape
{
    private readonly RectangleLegacy _rectangle = rectangle;

    public void Draw()
    {
        Console.WriteLine("Adapter Pattern: [Adapter] Adapting legacy rectangle to modern interface...");
        _rectangle.Display();
    }
}