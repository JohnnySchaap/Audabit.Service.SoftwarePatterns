namespace Audabit.Service.SoftwarePatterns.App.Patterns._6_Concurrency;

/// <summary>
/// Service implementation for demonstrating Concurrency / Multithreading Patterns.
/// </summary>
public sealed class ConcurrencyPatternsService : IConcurrencyPatternsService
{
    public async Task ThreadPoolAsync()
    {
        Console.WriteLine("\n=== Thread Pool Pattern Demo ===\n");
        await Patterns.ThreadPool.RunAsync();
        Console.WriteLine("\n=== Thread Pool Pattern: Demonstration complete.===\n");
    }

    public async Task FuturePromiseAsync()
    {
        Console.WriteLine("\n=== Future-Promise Pattern Demo ===\n");
        await Patterns.FuturePromise.RunAsync();
        Console.WriteLine("\n=== Future-Promise Pattern: Demonstration complete.===\n");
    }

    public async Task ReadWriteLockAsync()
    {
        Console.WriteLine("\n=== Read-Write Lock Pattern Demo ===\n");
        await Patterns.ReadWriteLock.RunAsync();
        Console.WriteLine("\n=== Read-Write Lock Pattern: Demonstration complete.===\n");
    }

    public async Task ProducerConsumerAsync()
    {
        Console.WriteLine("\n=== Producer-Consumer Pattern Demo ===\n");
        await Patterns.ProducerConsumer.RunAsync();
        Console.WriteLine("\n=== Producer-Consumer Pattern: Demonstration complete.===\n");
    }

    public async Task ActiveObjectAsync()
    {
        Console.WriteLine("\n=== Active Object Pattern Demo ===\n");
        await Patterns.ActiveObject.RunAsync();
        Console.WriteLine("\n=== Active Object Pattern: Demonstration complete.===\n");
    }
}