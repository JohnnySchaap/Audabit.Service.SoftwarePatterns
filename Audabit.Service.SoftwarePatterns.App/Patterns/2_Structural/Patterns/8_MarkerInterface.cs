namespace Audabit.Service.SoftwarePatterns.App.Patterns._2_Structural.Patterns;

// Marker Interface Pattern: An empty interface used to associate metadata with a class.
// In this example:
// - We have marker interfaces (ISerializable, IAuditable, ICacheable) with no methods.
// - Classes implement these interfaces to "tag" themselves with capabilities.
// - Framework code can check for these markers using pattern matching or reflection.
// - The key here is that the interface carries no behavior, only metadata/intent.
//
// MODERN C# USAGE:
// In modern .NET, attributes are strongly preferred over marker interfaces:
//   [Serializable] public class User { }
//   [JsonIgnore] public string Password { get; set; }
//   [Obsolete("Use NewMethod instead")] public void OldMethod() { }
// Attributes provide:
// - More flexibility (can pass parameters, apply to properties/methods/classes)
// - Better discoverability (reflection APIs designed for attributes)
// - No interface pollution (doesn't affect class hierarchy)
// Only use marker interfaces when you need polymorphic behavior (if (obj is ISerializable)).
// For simple metadata tagging, always prefer attributes.
public static class MarkerInterface
{
    public static void Run()
    {
        Console.WriteLine("Marker Interface Pattern: Using empty interfaces to tag classes with metadata...");
        Console.WriteLine("Marker Interface Pattern: Framework code checks for markers to apply cross-cutting concerns.\n");

        Console.WriteLine("Marker Interface Pattern: --- Processing Different Entity Types ---");
        DataProcessor.Process(new User { Id = 1, Name = "John Doe" });
        Console.WriteLine();
        DataProcessor.Process(new Product { Id = 101, Name = "Laptop" });
        Console.WriteLine();
        DataProcessor.Process(new Config { Key = "AppName", Value = "MyApp" });
    }
}

// Marker interfaces - no methods, just metadata
// NOTE: In modern C#, attributes are preferred over marker interfaces for this purpose.
// Example modern approach:
//   [Serializable] public class User { }
//   [Obsolete] public void OldMethod() { }
// Attributes provide the same "tagging" functionality with more flexibility and metadata support.
public interface ISerializable { }

public interface IAuditable { }

public interface ICacheable { }

// Entities with different marker combinations
public sealed class User : ISerializable, IAuditable
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public sealed class Product : ISerializable, ICacheable
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public sealed class Config : ISerializable
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}

// Framework code that checks for markers
public sealed class DataProcessor
{
    public static void Process<T>(T entity)
    {
        Console.WriteLine($"Marker Interface Pattern: [Processor] Processing {typeof(T).Name}...");

        // Check for serialization marker
        if (entity is ISerializable)
        {
            Console.WriteLine($"Marker Interface Pattern: [Processor] - Entity is ISerializable: Converting to JSON");
        }

        // Check for audit marker
        if (entity is IAuditable)
        {
            Console.WriteLine($"Marker Interface Pattern: [Processor] - Entity is IAuditable: Creating audit log entry");
        }

        // Check for cache marker
        if (entity is ICacheable)
        {
            Console.WriteLine($"Marker Interface Pattern: [Processor] - Entity is ICacheable: Storing in cache");
        }
    }
}