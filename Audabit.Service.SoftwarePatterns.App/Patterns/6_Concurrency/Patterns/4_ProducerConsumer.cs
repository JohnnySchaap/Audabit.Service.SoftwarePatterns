using System.Collections.Concurrent;

namespace Audabit.Service.SoftwarePatterns.App.Patterns._6_Concurrency.Patterns;

// Producer-Consumer Pattern: Decouple data production from consumption using a queue.
// In this example:
// - Producer threads generate work items and add them to a shared queue.
// - Consumer threads take items from the queue and process them.
// - BlockingCollection provides thread-safe queue with blocking semantics.
// - The queue buffers items when production rate differs from consumption rate.
//
// MODERN C# USAGE:
// - Use System.Threading.Channels for high-performance producer-consumer scenarios
// - BlockingCollection is simpler but Channels provide better async support
// - For message processing, consider Azure Service Bus, RabbitMQ, or Kafka
// - Use bounded queues to apply backpressure and prevent memory exhaustion
public static class ProducerConsumer
{
    public static Task RunAsync()
    {
        Console.WriteLine("Producer Consumer Pattern: Demonstrating producer-consumer queue...");
        Console.WriteLine("Producer Consumer Pattern: Producers add items, consumers process them.\n");

        BasicExample();
        Console.WriteLine("\n" + new string('-', 80) + "\n");
        BackgroundQueueExample();

        return Task.CompletedTask;
    }

    private static void BasicExample()
    {
        Console.WriteLine("Producer Consumer Pattern: --- Basic Example: Multiple Producers/Consumers ---");

        var queue = new BlockingCollection<WorkItem>(boundedCapacity: 10);
        var producedCount = 0;
        var consumedCount = 0;

        // Start consumer threads
        var consumerTasks = new List<Task>();
        for (var i = 0; i < 2; i++)
        {
            var consumerId = i + 1;
            var task = Task.Run(() =>
            {
                foreach (var item in queue.GetConsumingEnumerable())
                {
                    var threadId = Environment.CurrentManagedThreadId;
                    Console.WriteLine($"Producer Consumer Pattern: Consumer {consumerId} (thread {threadId}) processing item {item.Id}");
                    Thread.Sleep(150); // Simulate processing
                    Interlocked.Increment(ref consumedCount);
                    Console.WriteLine($"Producer Consumer Pattern: Consumer {consumerId} completed item {item.Id}");
                }
            });
            consumerTasks.Add(task);
        }

        // Start producer threads
        var producerTasks = new List<Task>();
        for (var i = 0; i < 2; i++)
        {
            var producerId = i + 1;
            var task = Task.Run(() =>
            {
                for (var j = 0; j < 3; j++)
                {
                    var itemId = Interlocked.Increment(ref producedCount);
                    var item = new WorkItem(itemId, $"Data from producer {producerId}");
                    Console.WriteLine($"Producer Consumer Pattern: Producer {producerId} adding item {itemId}");
                    queue.Add(item);
                    Thread.Sleep(100); // Simulate work generation
                }
            });
            producerTasks.Add(task);
        }

        // Wait for producers to finish
        Task.WaitAll([.. producerTasks]);
        queue.CompleteAdding();
        Console.WriteLine("\nProducer Consumer Pattern: All producers finished. Queue marked as complete.");

        // Wait for consumers to finish
        Task.WaitAll([.. consumerTasks]);
        Console.WriteLine($"\nProducer Consumer Pattern: Produced {producedCount} items, consumed {consumedCount} items.");
    }

    private static void BackgroundQueueExample()
    {
        Console.WriteLine("Producer Consumer Pattern: --- Real-World Example: Background Queue Processing ---");
        Console.WriteLine("Producer Consumer Pattern: Simulating Web API queuing work for background processing\n");

        var backgroundQueue = new BackgroundTaskQueue(maxQueueSize: 5);
        var cts = new CancellationTokenSource();

        // Simulate background worker (like a BackgroundService)
        var workerTask = Task.Run(async () =>
        {
            Console.WriteLine("Producer Consumer Pattern: Background worker started\n");
            while (!cts.Token.IsCancellationRequested)
            {
                var workItem = await backgroundQueue.DequeueAsync(cts.Token);
                if (workItem != null)
                {
                    try
                    {
                        Console.WriteLine($"Producer Consumer Pattern: Background worker processing '{workItem.Name}'...");
                        await Task.Delay(200, cts.Token); // Simulate processing
                        Console.WriteLine($"Producer Consumer Pattern: Background worker completed '{workItem.Name}'");
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                }
            }
            Console.WriteLine("\nProducer Consumer Pattern: Background worker stopped");
        });

        // Simulate Web API receiving requests and queuing work
        Console.WriteLine("Producer Consumer Pattern: Simulating API requests queuing background work...\n");
        for (var i = 1; i <= 8; i++)
        {
            var requestId = i;
            var queued = backgroundQueue.TryEnqueue(new BackgroundTask($"Request-{requestId}", async ct => await Task.Delay(100, ct)));

            if (queued)
            {
                Console.WriteLine($"Producer Consumer Pattern: API request {requestId} queued for background processing");
            }
            else
            {
                Console.WriteLine($"Producer Consumer Pattern: API request {requestId} REJECTED - queue full (backpressure!)");
            }

            Thread.Sleep(50); // Simulate request arrival rate
        }

        Console.WriteLine("\nProducer Consumer Pattern: All API requests processed. Waiting for background worker to finish...\n");

        // Wait a bit for queue to drain
        Thread.Sleep(2000);

        // Stop the worker
        cts.Cancel();
        workerTask.Wait();

        Console.WriteLine($"\nProducer Consumer Pattern: Queue count at shutdown: {backgroundQueue.Count}");
    }

    private sealed record WorkItem(int Id, string Data);
}

/// <summary>
/// Background task queue for processing work items asynchronously.
/// This is a realistic pattern for Web APIs that need to queue background work.
/// </summary>
public sealed class BackgroundTaskQueue(int maxQueueSize)
{
    private readonly BlockingCollection<BackgroundTask> _queue = new(maxQueueSize);

    public bool TryEnqueue(BackgroundTask task)
    {
        // TryAdd returns false if queue is full (backpressure)
        return _queue.TryAdd(task);
    }

    public async Task<BackgroundTask?> DequeueAsync(CancellationToken cancellationToken)
    {
        // Blocking wait for next item
        try
        {
            return await Task.Run(() =>
            {
                if (_queue.TryTake(out var task, 100))
                {
                    return task;
                }
                return null;
            }, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            return null;
        }
    }

    public int Count => _queue.Count;
}

public sealed record BackgroundTask(string Name, Func<CancellationToken, Task> WorkItem);