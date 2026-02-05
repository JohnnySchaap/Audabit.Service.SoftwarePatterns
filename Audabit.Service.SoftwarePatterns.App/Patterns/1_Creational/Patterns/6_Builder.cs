namespace Audabit.Service.SoftwarePatterns.App.Patterns._1_Creational.Patterns;

// Builder Pattern: Separates the construction of a complex object from its representation, allowing the same construction process to create different representations.
// In this example:
// - We build Computer objects with multiple optional components (processor, RAM, GPU, storage) using a fluent interface.
// - The ComputerBuilder class provides methods like WithProcessor(), WithRam() that return the builder itself for method chaining.
// - A ComputerDirector class demonstrates providing predefined configurations (gaming PC, office PC).
// - The key here is that complex object construction is separated into discrete steps, making it easier to create objects with different configurations.
// NOTE: The Director pattern is also demonstrated here as an optional addition to the Builder pattern.
//
// MODERN C# USAGE:
// For simple objects with required properties, use C# 11+ required properties and object initializers:
//   public record Computer { public required string CPU { get; init; } public string? GPU { get; init; } }
//   var pc = new Computer { CPU = "i9", GPU = "RTX 4090" }; // Required props enforced at compile time
// Only use Builder pattern when:
//   - Construction has complex validation logic or multi-step processes
//   - Objects need predefined configurations (Director pattern)
//   - Building immutable objects with many optional parameters (10+ properties)
// Modern C# reduces the need for Builder pattern in simple cases.
public static class Builder
{
    public static void Run()
    {
        Console.WriteLine("Builder Pattern: Constructing complex Computer objects step by step...");
        Console.WriteLine("Builder Pattern: Using fluent interface for step-by-step configuration.\n");

        // Build a gaming computer
        Console.WriteLine("Builder Pattern: --- Building Custom Gaming Computer ---");
        var gamingComputer = new ComputerBuilder()
            .WithProcessor("Intel i9-13900K")
            .WithRam(32)
            .WithStorage(2000)
            .WithGraphicsCard("NVIDIA RTX 4090")
            .WithOperatingSystem("Windows 11 Pro")
            .Build();

        Console.WriteLine("Builder Pattern: Gaming Computer:");
        Console.WriteLine(gamingComputer);
        Console.WriteLine();

        // Build an office computer
        Console.WriteLine("Builder Pattern: --- Building Custom Office Computer ---");
        var officeComputer = new ComputerBuilder()
            .WithProcessor("Intel i5-12400")
            .WithRam(16)
            .WithStorage(512)
            .WithOperatingSystem("Windows 11")
            .Build();

        Console.WriteLine("Builder Pattern: Office Computer:");
        Console.WriteLine(officeComputer);
        Console.WriteLine();

        // Director pattern - predefined configurations
        Console.WriteLine("Builder Pattern: --- Using Director for Predefined Configurations: Server ---");
        var serverComputer = ComputerDirector.BuildServer();
        Console.WriteLine("Builder Pattern: Server Computer:");
        Console.WriteLine(serverComputer);
        Console.WriteLine();

        Console.WriteLine("Builder Pattern: --- Using Director for Predefined Configurations: Workstation Computer ---");
        var workstationComputer = ComputerDirector.BuildWorkstation();
        Console.WriteLine("Builder Pattern: Workstation Computer:");
        Console.WriteLine(workstationComputer);
    }
}

public sealed record Computer
{
    public required string Processor { get; init; }
    public required int RamInGb { get; init; }
    public required int StorageInGb { get; init; }
    public string? GraphicsCard { get; init; }
    public required string OperatingSystem { get; init; }

    public override string ToString()
    {
        var specs =
            $"Builder Pattern: Processor: {Processor}\n" +
            $"Builder Pattern: RAM: {RamInGb}GB\n" +
            $"Builder Pattern: Storage: {StorageInGb}GB\n" +
            $"Builder Pattern: OS: {OperatingSystem}";

        if (!string.IsNullOrEmpty(GraphicsCard))
        {
            specs += $"\nBuilder Pattern: Graphics: {GraphicsCard}";
        }

        return specs;
    }
}

public sealed class ComputerBuilder
{
    private string? _processor;
    private int _ramInGb;
    private int _storageInGb;
    private string? _graphicsCard;
    private string? _operatingSystem;

    public ComputerBuilder WithProcessor(string processor)
    {
        _processor = processor;
        return this;
    }

    public ComputerBuilder WithRam(int ramInGb)
    {
        _ramInGb = ramInGb;
        return this;
    }

    public ComputerBuilder WithStorage(int storageInGb)
    {
        _storageInGb = storageInGb;
        return this;
    }

    public ComputerBuilder WithGraphicsCard(string graphicsCard)
    {
        _graphicsCard = graphicsCard;
        return this;
    }

    public ComputerBuilder WithOperatingSystem(string operatingSystem)
    {
        _operatingSystem = operatingSystem;
        return this;
    }

    public Computer Build()
    {
        if (string.IsNullOrEmpty(_processor))
        {
            throw new InvalidOperationException("Processor is required");
        }

        if (_ramInGb <= 0)
        {
            throw new InvalidOperationException("RAM must be greater than 0");
        }

        if (_storageInGb <= 0)
        {
            throw new InvalidOperationException("Storage must be greater than 0");
        }

        if (string.IsNullOrEmpty(_operatingSystem))
        {
            throw new InvalidOperationException("Operating System is required");
        }

        return new Computer
        {
            Processor = _processor,
            RamInGb = _ramInGb,
            StorageInGb = _storageInGb,
            GraphicsCard = _graphicsCard,
            OperatingSystem = _operatingSystem
        };
    }
}

public sealed class ComputerDirector
{
    public static Computer BuildServer()
    {
        return new ComputerBuilder()
            .WithProcessor("AMD EPYC 7763")
            .WithRam(128)
            .WithStorage(8000)
            .WithOperatingSystem("Ubuntu Server 22.04 LTS")
            .Build();
    }

    public static Computer BuildWorkstation()
    {
        return new ComputerBuilder()
            .WithProcessor("Intel Xeon W-3375")
            .WithRam(64)
            .WithStorage(4000)
            .WithGraphicsCard("NVIDIA RTX A6000")
            .WithOperatingSystem("Windows 11 Pro for Workstations")
            .Build();
    }

    public static Computer BuildBudget()
    {
        return new ComputerBuilder()
            .WithProcessor("AMD Ryzen 5 5600G")
            .WithRam(8)
            .WithStorage(256)
            .WithOperatingSystem("Windows 11 Home")
            .Build();
    }
}