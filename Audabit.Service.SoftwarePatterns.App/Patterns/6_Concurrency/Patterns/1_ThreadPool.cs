namespace Audabit.Service.SoftwarePatterns.App.Patterns._6_Concurrency.Patterns;

// Thread Pool Pattern: Reuse a pool of worker threads to execute queued tasks instead of creating new threads.
// In this example:
// - We demonstrate both the low-level ThreadPool API and the modern Task-based approach.
// - Multiple work items are queued to the thread pool for concurrent execution.
// - The thread pool automatically manages thread creation, reuse, and destruction.
// - This avoids the overhead of creating new threads for every task.
//
// MODERN C# USAGE:
// - Prefer Task.Run() over ThreadPool.QueueUserWorkItem for better composability
// - Use async/await for I/O-bound operations (doesn't use thread pool threads while waiting)
// - Thread pool is used internally by ASP.NET Core, Task.Run, Parallel.For, etc.
// - Only explicitly use ThreadPool for specialized scenarios or performance tuning
public static class ThreadPool
{
    public static async Task RunAsync()
    {
        Console.WriteLine("Thread Pool Pattern: Demonstrating thread pool usage...");
        Console.WriteLine("Thread Pool Pattern: Reusing worker threads for multiple tasks.\n");

        Console.WriteLine("Thread Pool Pattern: --- Queuing Work Items to Thread Pool ---");
        var completedCount = 0;
        var workItemsCount = 5;
        using var countdown = new CountdownEvent(workItemsCount);

        for (var i = 0; i < workItemsCount; i++)
        {
            var workItemNumber = i + 1;
            System.Threading.ThreadPool.QueueUserWorkItem(_ =>
            {
                var threadId = Environment.CurrentManagedThreadId;
                Console.WriteLine($"Thread Pool Pattern: Work item {workItemNumber} executing on thread {threadId}");
                Thread.Sleep(100); // Simulate work
                Console.WriteLine($"Thread Pool Pattern: Work item {workItemNumber} completed on thread {threadId}");
                Interlocked.Increment(ref completedCount);
                countdown.Signal();
            });
        }

        countdown.Wait();
        Console.WriteLine($"\nThread Pool Pattern: All {completedCount} work items completed.");

        Console.WriteLine("\nThread Pool Pattern: --- Using Modern Task API ---");
        var tasks = new List<Task>();
        for (var i = 0; i < 3; i++)
        {
            var taskNumber = i + 1;
            var task = Task.Run(() =>
            {
                var threadId = Environment.CurrentManagedThreadId;
                Console.WriteLine($"Thread Pool Pattern: Task {taskNumber} executing on thread {threadId}");
                Thread.Sleep(50);
                Console.WriteLine($"Thread Pool Pattern: Task {taskNumber} completed");
            });
            tasks.Add(task);
        }

        await Task.WhenAll(tasks);
        Console.WriteLine("\nThread Pool Pattern: All tasks completed using Task API.");
    }
}