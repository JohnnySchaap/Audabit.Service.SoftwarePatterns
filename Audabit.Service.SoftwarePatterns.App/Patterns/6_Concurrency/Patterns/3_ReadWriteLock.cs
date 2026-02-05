namespace Audabit.Service.SoftwarePatterns.App.Patterns._6_Concurrency.Patterns;

// Read-Write Lock Pattern: Allow multiple concurrent readers or one exclusive writer.
// In this example:
// - ReaderWriterLockSlim allows many threads to read simultaneously.
// - Write operations get exclusive access (no readers or writers).
// - This is perfect for read-heavy scenarios like caches.
// - Much better than lock {} for read-heavy workloads.
//
// MODERN C# USAGE:
// - Use ReaderWriterLockSlim for thread-safe collections with many reads, few writes
// - Common in caching scenarios (in-memory cache, configuration cache)
// - ConcurrentDictionary is often simpler but RW lock gives finer control
// - Be careful of lock upgrades - they can deadlock
// - Dispose the lock when done (IDisposable)
public static class ReadWriteLock
{
    public static async Task RunAsync()
    {
        Console.WriteLine("Read-Write Lock Pattern: Demonstrating read-write lock with cache...");
        Console.WriteLine("Read-Write Lock Pattern: Multiple readers can access simultaneously.\n");

        var cache = new ThreadSafeCache();

        Console.WriteLine("Read-Write Lock Pattern: --- Initial Write ---");
        cache.Write("config", "initial-value");

        Console.WriteLine("\nRead-Write Lock Pattern: --- Concurrent Reads (10 threads) ---");
        var readTasks = new List<Task>();
        for (var i = 0; i < 10; i++)
        {
            var readerId = i + 1;
            var task = Task.Run(() =>
            {
                var threadId = Environment.CurrentManagedThreadId;
                var value = cache.Read("config");
                Console.WriteLine($"Read-Write Lock Pattern: Reader {readerId} (Thread {threadId}) read: {value}");
                Thread.Sleep(100); // Simulate read work
            });
            readTasks.Add(task);
        }

        await Task.WhenAll(readTasks);

        Console.WriteLine("\nRead-Write Lock Pattern: --- Write Update (Exclusive Access) ---");
        cache.Write("config", "updated-value");

        Console.WriteLine("\nRead-Write Lock Pattern: --- Read After Update ---");
        var finalValue = cache.Read("config");
        Console.WriteLine($"Read-Write Lock Pattern: Final value: {finalValue}");

        cache.Dispose();
        Console.WriteLine("\nRead-Write Lock Pattern: Cache disposed.");
    }
}

public sealed class ThreadSafeCache : IDisposable
{
    private readonly ReaderWriterLockSlim _lock = new();
    private readonly Dictionary<string, string> _data = [];

    public string? Read(string key)
    {
        _lock.EnterReadLock();
        try
        {
            var threadId = Environment.CurrentManagedThreadId;
            _data.TryGetValue(key, out var value);
            Console.WriteLine($"Read-Write Lock Pattern: Thread {threadId} acquired READ lock");
            return value;
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    public void Write(string key, string value)
    {
        _lock.EnterWriteLock();
        try
        {
            var threadId = Environment.CurrentManagedThreadId;
            Console.WriteLine($"Read-Write Lock Pattern: Thread {threadId} acquired WRITE lock (exclusive)");
            Thread.Sleep(50); // Simulate write work
            _data[key] = value;
            Console.WriteLine($"Read-Write Lock Pattern: Thread {threadId} wrote '{key}' = '{value}'");
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    public void Dispose()
    {
        _lock.Dispose();
    }
}