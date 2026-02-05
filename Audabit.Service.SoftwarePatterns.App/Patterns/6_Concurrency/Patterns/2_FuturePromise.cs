namespace Audabit.Service.SoftwarePatterns.App.Patterns._6_Concurrency.Patterns;

// Future/Promise Pattern: Represent the result of an async operation that completes in the future.
// In this example:
// - Task<T> is .NET's implementation of the Future/Promise pattern.
// - We demonstrate async operations that return Tasks (promises).
// - The caller can await the Task to get the result (future).
// - Multiple async operations can be composed and awaited together.
//
// MODERN C# USAGE:
// - Task<T> and async/await are the standard for all async programming in .NET
// - Use Task.WhenAll for parallel execution, Task.WhenAny for racing
// - ValueTask<T> for high-performance scenarios with sync completion path
// - Always use async/await instead of Task.Wait() or .Result to avoid deadlocks
public static class FuturePromise
{
    public static async Task RunAsync()
    {
        Console.WriteLine("Future Promise Pattern: Demonstrating async operations with Task<T>...");
        Console.WriteLine("Future Promise Pattern: Task represents future result of async operation.\n");

        Console.WriteLine("Future Promise Pattern: --- Starting Multiple Async Operations ---");

        // Start async operations (they return Tasks immediately - promises)
        var task1 = FetchDataAsync("Operation-1", delayMs: 200);
        var task2 = FetchDataAsync("Operation-2", delayMs: 150);
        var task3 = FetchDataAsync("Operation-3", delayMs: 100);

        Console.WriteLine("Future Promise Pattern: All operations started. Waiting for completion...\n");

        // Await all to complete (futures are resolved)
        await Task.WhenAll(task1, task2, task3);

        Console.WriteLine("\nFuture Promise Pattern: --- Retrieving Results ---");
        Console.WriteLine($"Future Promise Pattern: {await task1}");
        Console.WriteLine($"Future Promise Pattern: {await task2}");
        Console.WriteLine($"Future Promise Pattern: {await task3}");

        Console.WriteLine("\nFuture Promise Pattern: --- Demonstrating Parallel Execution ---");

        Console.WriteLine("Future Promise Pattern: Starting parallel fetch operations...");

        // Start all operations in parallel
        var tasks = new[]
        {
            FetchDataAsync("Parallel-A", 80),
            FetchDataAsync("Parallel-B", 90),
            FetchDataAsync("Parallel-C", 70)
        };

        // Await all to complete
        var results = await Task.WhenAll(tasks);

        Console.WriteLine($"\nFuture Promise Pattern: Parallel operations result: {string.Join(", ", results)}");
    }

    private static async Task<string> FetchDataAsync(string operationName, int delayMs)
    {
        var threadId = Environment.CurrentManagedThreadId;
        Console.WriteLine($"Future Promise Pattern: {operationName} started on thread {threadId}");

        await Task.Delay(delayMs); // Simulate async I/O

        var result = $"{operationName} completed";
        Console.WriteLine($"Future Promise Pattern: {result} on thread {Environment.CurrentManagedThreadId}");

        return result;
    }
}