namespace Audabit.Service.SoftwarePatterns.App.Patterns._1_Creational.Patterns;

// Abstract Factory Pattern: Provides an interface for creating families of related or dependent objects without specifying their concrete classes.
// In this example:
// - We create UI components (buttons and checkboxes) for different platforms (Windows and macOS).
// - The Abstract Factory defines the interface for creating these components, while concrete factories implement the creation logic for each platform.
// - The key here is that the abstract factory creates multiple related objects (button and checkbox) that belong to the same family (platform).
// - This ensures that the client code can work with different families of products without being coupled to their concrete classes.
// - The difference from Factory Method is that Abstract Factory deals with multiple related products, while Factory Method focuses on a single product type.
// - The downside is increased complexity due to more interfaces and classes. The client must also be aware of the specific factory to use for the desired product family.
//
// MODERN C# USAGE:
// This pattern creates significant complexity with multiple abstract classes and factory hierarchies.
// In modern .NET with Dependency Injection, you rarely need this level of abstraction:
// - Register all implementations in DI grouped by family/theme.
// - Use a Simple Factory with IEnumerable<IButton>, IEnumerable<ICheckbox> injected.
// - Select matching family members: buttons.First(b => b.Theme == theme), checkboxes.First(c => c.Theme == theme).
// - Adding new themes only requires DI registration - no new abstract factory classes needed.
// - This traditional pattern is valuable for learning design patterns and working with legacy code.
// - For production .NET code, prefer composition with DI over complex inheritance hierarchies.
// - The DI container itself acts as a sophisticated factory, and you only need simple selection logic.
public static class AbstractFactory
{
    public static void Run()
    {
        Console.WriteLine("Abstract Factory Pattern: Creating UI components for different platforms...");
        Console.WriteLine("Abstract Factory Pattern: Each factory creates a family of related UI components.\n");

        Console.WriteLine("Abstract Factory Pattern: --- Using Windows UI Factory ---");
        IUIFactory windowsFactory = new WindowsUIFactory();
        RenderUI(windowsFactory, "Windows");

        Console.WriteLine();
        Console.WriteLine("Abstract Factory Pattern: --- Using Mac UI Factory ---");
        IUIFactory macFactory = new MacUIFactory();
        RenderUI(macFactory, "macOS");
    }

    private static void RenderUI(IUIFactory factory, string platform)
    {
        var button = factory.CreateButton();
        var checkbox = factory.CreateCheckbox();

        Console.WriteLine($"Abstract Factory Pattern: Rendering UI for {platform}:");
        button.Render();
        checkbox.Render();
    }
}

public interface IUIFactory
{
    IButton CreateButton();

    ICheckbox CreateCheckbox();
}

public sealed class WindowsUIFactory : IUIFactory
{
    public IButton CreateButton()
    {
        return new WindowsButton();
    }

    public ICheckbox CreateCheckbox()
    {
        return new WindowsCheckbox();
    }
}

public sealed class MacUIFactory : IUIFactory
{
    public IButton CreateButton()
    {
        return new MacButton();
    }

    public ICheckbox CreateCheckbox()
    {
        return new MacCheckbox();
    }
}

public interface IButton
{
    void Render();
}

public interface ICheckbox
{
    void Render();
}

public sealed class WindowsButton : IButton
{
    public void Render()
    {
        Console.WriteLine("Abstract Factory Pattern: [Windows Button] - Flat, modern design");
    }
}

public sealed class WindowsCheckbox : ICheckbox
{
    public void Render()
    {
        Console.WriteLine("Abstract Factory Pattern: [Windows Checkbox] - Square, modern design");
    }
}

public sealed class MacButton : IButton
{
    public void Render()
    {
        Console.WriteLine("Abstract Factory Pattern: [Mac Button] - Rounded, aqua design");
    }
}

public sealed class MacCheckbox : ICheckbox
{
    public void Render()
    {
        Console.WriteLine("Abstract Factory Pattern: [Mac Checkbox] - Rounded, aqua design");
    }
}