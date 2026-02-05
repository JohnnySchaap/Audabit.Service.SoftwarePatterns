namespace Audabit.Service.SoftwarePatterns.App.Patterns._1_Creational.Patterns;

// Lazy Initialization Pattern: Defers the creation of an object until the first time it is needed.
// In this example:
// - We use null-coalescing assignment (??=) to delay object creation until it's actually accessed.
// - The singleton instance and the random value are both lazily initialized on first access.
// - This pattern is useful for expensive-to-create objects that may not always be needed.
// - The key here is that initialization happens only when the Instance or GetMessage() property is first accessed, avoiding unnecessary work.
//
// MODERN C# USAGE:
// .NET provides Lazy<T> for thread-safe lazy initialization:
//   private readonly Lazy<ExpensiveObject> _lazy = new(() => new ExpensiveObject());
//   var obj = _lazy.Value; // Created on first access, thread-safe
// Combined with DI, you can use lazy dependencies:
//   public MyService(Lazy<IExpensiveService> lazyService) { }
// This is cleaner and safer than manual ??= patterns for complex scenarios.
public static class LazyInitialization
{
    public static void Run()
    {
        Console.WriteLine("Lazy Initialization Pattern: Demonstrating deferred initialization...");
        Console.WriteLine("Lazy Initialization Pattern: Object creation is delayed until first access.\n");

        Console.WriteLine("Lazy Initialization Pattern: --- Instance Not Yet Created ---");
        Console.WriteLine("Lazy Initialization Pattern: Instance will be created on first access.");

        Console.WriteLine("\nLazy Initialization Pattern: --- Accessing Instance for First Time ---");
        var singletonInstance = LazyInitializationInstance.Instance;

        Console.WriteLine("\nLazy Initialization Pattern: --- Accessing Methods (Lazy Value Generation) ---");
        for (var i = 0; i < 3; i++)
        {
            Console.WriteLine($"Lazy Initialization Pattern: Access #{i + 1}");
            var message = singletonInstance.GetMessage();
            Console.WriteLine($"{message}");
        }
    }
}

public sealed class LazyInitializationInstance
{
    // Lazy Initialization: We only initialize the singleton instance when it's first accessed.
    // Combined with Singleton implementation
    public static LazyInitializationInstance Instance => _instance ??= new LazyInitializationInstance();
    private static LazyInitializationInstance? _instance;
    private int? _randomValue;

    private LazyInitializationInstance()
    {
        // Private constructor to prevent instantiation
        Console.WriteLine("Lazy Initialization Pattern: Creating instance...");
        Console.WriteLine("Lazy Initialization Pattern: Instance created.");
    }

    public string GetMessage()
    {
        // Lazy initialization: We only initialize the random value when it's first accessed
        if (!_randomValue.HasValue)
        {
            Console.WriteLine("Lazy Initialization Pattern: Generating random value...");
            _randomValue ??= new Random().Next();
            Console.WriteLine($"Lazy Initialization Pattern: Random value generated: {_randomValue}");
        }
        return $"Lazy Initialization Pattern: Accessing lazy initialized instance with random value {_randomValue}.";
    }
}