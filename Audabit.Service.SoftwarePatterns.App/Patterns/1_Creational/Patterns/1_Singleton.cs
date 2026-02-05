namespace Audabit.Service.SoftwarePatterns.App.Patterns._1_Creational.Patterns;

// Singleton Pattern: Ensures a class has only one instance and provides a global point of access to it.
// In this example:
// - We create a SingletonInstance class that can only have one instance throughout the application's lifetime.
// - The instance is created eagerly (when the class is first accessed) using static readonly initialization, which is thread-safe.
// - Multiple calls to Instance property return the same object, demonstrating the single instance guarantee.
// - The key here is that the constructor is private, preventing external instantiation.
//
// MODERN C# USAGE:
// In production .NET code, AVOID implementing Singleton manually:
// - Use Dependency Injection instead: services.AddSingleton<MyService>();
// - The DI container manages the lifetime and ensures a single instance.
// - This approach is testable (you can inject mocks), doesn't use global state, and follows modern best practices.
// - Only implement manual Singleton for framework-level code or when DI isn't available.
public static class Singleton
{
    public static void Run()
    {
        Console.WriteLine("Singleton Pattern: Demonstrating single instance creation...");
        Console.WriteLine("Singleton Pattern: Only one instance exists throughout application lifetime.\n");

        Console.WriteLine("Singleton Pattern: --- Accessing Instance for First Time ---");
        var singletonInstance = SingletonInstance.Instance;

        Console.WriteLine("\nSingleton Pattern: --- Proving Same Instance on Multiple Accesses ---");
        for (var i = 0; i < 3; i++)
        {
            Console.WriteLine($"Singleton Pattern: Access #{i + 1}");
            var message = singletonInstance.GetMessage();
            Console.WriteLine($"{message}");
        }
    }
}

public sealed class SingletonInstance
{
    public static SingletonInstance Instance => _instance;
    private static readonly SingletonInstance _instance = new();
    private readonly int _randomValue;

    private SingletonInstance()
    {
        // Private constructor to prevent instantiation
        Console.WriteLine($"Singleton Pattern: Creating instance...");
        _randomValue = new Random().Next();
        Console.WriteLine($"Singleton Pattern: Instance created with random value {_randomValue}");
    }

    public string GetMessage()
    {
        return $"Singleton Pattern: Accessing singleton instance with random value {_randomValue}.";
    }
}