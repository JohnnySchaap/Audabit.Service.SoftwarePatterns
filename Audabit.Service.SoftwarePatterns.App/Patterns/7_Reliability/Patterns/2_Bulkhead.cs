namespace Audabit.Service.SoftwarePatterns.App.Patterns._7_Reliability.Patterns;

// Bulkhead Pattern: Isolate resources to prevent total system failure when one component fails.
// In this example:
// - We partition resources (thread pools, connections, etc.) into isolated bulkheads.
// - Each bulkhead has a maximum capacity (semaphore).
// - If one bulkhead is saturated, others continue operating normally.
// - Prevents cascading failures across system boundaries.
//
// MODERN C# USAGE:
// - Use SemaphoreSlim to limit concurrent operations per bulkhead
// - Isolate HTTP clients with separate HttpClientHandler pools
// - Use dedicated thread pools for critical vs non-critical work
// - In Kubernetes: resource limits per pod (CPU/memory bulkheads)
public static class Bulkhead
{
    public static async Task RunAsync()
    {
        Console.WriteLine("Bulkhead Pattern: Demonstrating resource isolation with bulkheads...");
        Console.WriteLine("Bulkhead Pattern: Preventing cascading failures across services.\n");

        // Create separate bulkheads for different services
        var criticalServiceBulkhead = new ServiceBulkhead("CriticalService", maxConcurrency: 3);
        var nonCriticalServiceBulkhead = new ServiceBulkhead("NonCriticalService", maxConcurrency: 2);

        Console.WriteLine("Bulkhead Pattern: --- Simulating Mixed Load ---\n");

        var tasks = new List<Task>();

        // Saturate non-critical service
        for (var i = 1; i <= 5; i++)
        {
            var requestId = i;
            tasks.Add(Task.Run(async () => await CallServiceThroughBulkhead(nonCriticalServiceBulkhead, requestId, delayMs: 1000)));
        }

        // Critical service should still be available
        await Task.Delay(100); // Let non-critical requests start

        Console.WriteLine("Bulkhead Pattern: Non-critical service is saturated. Testing critical service...\n");

        for (var i = 10; i <= 12; i++)
        {
            var requestId = i;
            tasks.Add(Task.Run(async () => await CallServiceThroughBulkhead(criticalServiceBulkhead, requestId, delayMs: 500)));
        }

        await Task.WhenAll(tasks);

        Console.WriteLine("\nBulkhead Pattern: --- Results Summary ---");
        Console.WriteLine($"Bulkhead Pattern: Critical service processed all requests (isolated from non-critical)");
        Console.WriteLine($"Bulkhead Pattern: Non-critical service rejected some requests (bulkhead full)");
        Console.WriteLine($"Bulkhead Pattern: Total system remained operational despite partial failure");
    }

    private static async Task CallServiceThroughBulkhead(ServiceBulkhead bulkhead, int requestId, int delayMs)
    {
        try
        {
            await bulkhead.ExecuteAsync(async () =>
            {
                Console.WriteLine($"Bulkhead Pattern: [{bulkhead.Name}] Request #{requestId} - Processing...");
                await Task.Delay(delayMs);
                Console.WriteLine($"Bulkhead Pattern: [{bulkhead.Name}] Request #{requestId} - Completed");
            });
        }
        catch (BulkheadRejectedException ex)
        {
            Console.WriteLine($"Bulkhead Pattern: [{bulkhead.Name}] Request #{requestId} - REJECTED: {ex.Message}");
        }
    }
}

// Service bulkhead using SemaphoreSlim for resource isolation
public class ServiceBulkhead(string name, int maxConcurrency)
{
    private readonly SemaphoreSlim _semaphore = new(maxConcurrency, maxConcurrency);
    public string Name { get; } = name;

    public async Task ExecuteAsync(Func<Task> action)
    {
        // Try to acquire slot (fail fast if bulkhead is full)
        if (!await _semaphore.WaitAsync(TimeSpan.FromMilliseconds(100)))
        {
            throw new BulkheadRejectedException($"{Name} bulkhead is at capacity");
        }

        try
        {
            await action();
        }
        finally
        {
            _semaphore.Release();
        }
    }
}

public class BulkheadRejectedException(string message) : Exception(message)
{
}