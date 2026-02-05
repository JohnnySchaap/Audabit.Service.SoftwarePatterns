using System.Threading.Channels;

namespace Audabit.Service.SoftwarePatterns.App.Patterns._6_Concurrency.Patterns;

// Active Object Pattern: Decouple method invocation from execution by running on dedicated thread.
// In this example:
// - The ActiveCounter class processes all operations on a dedicated thread.
// - Operations are queued to a channel and executed serially.
// - This ensures thread safety without locks - operations never execute concurrently.
// - Callers get a Task to await completion of their operation.
//
// MODERN C# USAGE:
// - Use for objects with complex state that need thread safety without locks
// - Actor frameworks (Akka.NET, Orleans) implement this pattern
// - Channels provide the async queue for message passing
// - Great for event loops, state machines, or serializing access to resources
public static class ActiveObject
{
    public static async Task RunAsync()
    {
        Console.WriteLine("Active Object Pattern: Demonstrating active object with dedicated thread...");
        Console.WriteLine("Active Object Pattern: All operations execute serially on single thread.\n");

        var counter = new ActiveCounter();

        Console.WriteLine("Active Object Pattern: --- Incrementing Counter from Multiple Threads ---");

        // Multiple threads try to increment concurrently
        var tasks = new List<Task>();
        for (var i = 0; i < 5; i++)
        {
            var threadNumber = i + 1;
            var task = Task.Run(async () =>
            {
                var callerThreadId = Environment.CurrentManagedThreadId;
                Console.WriteLine($"Active Object Pattern: Thread {callerThreadId} requesting increment {threadNumber}");

                await counter.IncrementAsync();

                Console.WriteLine($"Active Object Pattern: Thread {callerThreadId} increment {threadNumber} completed");
            });
            tasks.Add(task);
        }

        await Task.WhenAll(tasks);

        Console.WriteLine("\nActive Object Pattern: --- Getting Final Count ---");
        var finalCount = counter.GetCountAsync().Result;
        Console.WriteLine($"Active Object Pattern: Final count = {finalCount}");

        counter.Dispose();
        Console.WriteLine("\nActive Object Pattern: Active object disposed.");
    }
}

public sealed class ActiveCounter : IDisposable
{
    private readonly Channel<Func<Task>> _channel = Channel.CreateUnbounded<Func<Task>>();
    private readonly Task _processingTask;
    private int _count = 0;

    public ActiveCounter()
    {
        Console.WriteLine("Active Object Pattern: Creating active object with dedicated processing thread...");

        // Dedicated task processes all operations serially
        _processingTask = Task.Run(async () =>
        {
            var threadId = Environment.CurrentManagedThreadId;
            Console.WriteLine($"Active Object Pattern: Processing thread {threadId} started\n");

            await foreach (var operation in _channel.Reader.ReadAllAsync())
            {
                await operation();
            }

            Console.WriteLine($"\nActive Object Pattern: Processing thread {threadId} shutting down");
        });
    }

    public Task IncrementAsync()
    {
        var tcs = new TaskCompletionSource();
        _channel.Writer.TryWrite(async () =>
        {
            var threadId = Environment.CurrentManagedThreadId;
            await Task.Delay(50); // Simulate some work
            _count++;
            Console.WriteLine($"Active Object Pattern: Executed increment on thread {threadId}, count = {_count}");
            tcs.SetResult();
        });
        return tcs.Task;
    }

    public Task<int> GetCountAsync()
    {
        var tcs = new TaskCompletionSource<int>();
        _channel.Writer.TryWrite(async () =>
        {
            await Task.CompletedTask;
            tcs.SetResult(_count);
        });
        return tcs.Task;
    }

    public void Dispose()
    {
        _channel.Writer.Complete();
        _processingTask.Wait();
    }
}