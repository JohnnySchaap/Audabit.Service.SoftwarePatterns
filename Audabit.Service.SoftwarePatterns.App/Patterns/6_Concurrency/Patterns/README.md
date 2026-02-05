# Concurrency Patterns

Patterns for managing multiple threads of execution, ensuring thread safety, and coordinating concurrent operations in multi-threaded applications.

## 📖 About Concurrency Patterns

Concurrency patterns help manage parallel execution and shared resources:
- **Thread Safety**: Ensure correct behavior when multiple threads access shared data
- **Performance**: Maximize CPU utilization through parallelism
- **Coordination**: Synchronize tasks and manage dependencies
- **Scalability**: Handle increased workload through concurrent processing

---

## 🎯 Patterns

### 1. [Thread Pool](1_ThreadPool.cs)
**Problem**: Creating new threads for every task is expensive and can exhaust system resources
**Solution**: Reuse a pool of worker threads to execute queued tasks

**When to Use**:
- Many short-lived tasks need execution
- Want to limit number of concurrent threads
- Need to avoid thread creation overhead

**Modern .NET**:
```csharp
// Using built-in ThreadPool
ThreadPool.QueueUserWorkItem(_ =>
{
    // Task executes on thread pool thread
    ProcessData();
});

// Modern async approach with Task
await Task.Run(() => ProcessData());
```

**Real-World Examples**: Web servers, background job processing, parallel computations

**Personal Usage**:
> **ASP.NET Core Request Handling**
>
> Every ASP.NET Core application relies on ThreadPool for handling HTTP requests concurrently without creating new threads for each request.
>
> - **Challenge**: Handle thousands of concurrent HTTP requests efficiently
> - **Solution**: ASP.NET Core uses ThreadPool to process requests, avoiding thread creation overhead
> - **Result**: High throughput, efficient resource usage, automatic thread management
> - **Technology**: .NET ThreadPool, async/await, Kestrel web server

---

### 2. [Future/Promise (Task)](2_FuturePromise.cs)
**Problem**: Need to represent result of async operation that completes in the future
**Solution**: Return object representing eventual result that can be awaited

**When to Use**:
- Async operations should not block caller
- Want to compose async operations
- Need to handle async results uniformly

**Modern .NET**:
```csharp
// Task<T> is .NET's implementation of Future/Promise
public async Task<OrderResult> ProcessOrderAsync(Order order)
{
    // Returns immediately with Task (promise)
    var inventoryTask = CheckInventoryAsync(order);
    var pricingTask = CalculatePriceAsync(order);

    // Await both (futures are resolved)
    await Task.WhenAll(inventoryTask, pricingTask);

    return new OrderResult(inventoryTask.Result, pricingTask.Result);
}
```

**Real-World Examples**: HTTP requests, database queries, file I/O

**Personal Usage**:
> **Foundation of All Modern .NET**
>
> Task<T> is fundamental to every async operation in .NET—from HTTP calls to database queries to file I/O.
>
> - **Daily usage**: Every async method returns Task or Task<T>
> - **Composition**: Task.WhenAll, Task.WhenAny for parallel operations
> - **Cancellation**: CancellationToken support built-in
> - **Technology**: Task-based Asynchronous Pattern (TAP), async/await, ValueTask<T>

---

### 3. [Read-Write Lock](3_ReadWriteLock.cs)
**Problem**: Many readers contend with occasional writers, causing poor performance
**Solution**: Allow multiple concurrent readers but exclusive writer access

**When to Use**:
- Read operations vastly outnumber writes
- Reads are lengthy operations
- Want to maximize read concurrency

**Modern .NET**:
```csharp
// Using ReaderWriterLockSlim
private readonly ReaderWriterLockSlim _lock = new();
private Dictionary<string, int> _cache = new();

public int Read(string key)
{
    _lock.EnterReadLock();
    try
    {
        return _cache[key];
    }
    finally
    {
        _lock.ExitReadLock();
    }
}

public void Write(string key, int value)
{
    _lock.EnterWriteLock();
    try
    {
        _cache[key] = value;
    }
    finally
    {
        _lock.ExitWriteLock();
    }
}
```

**Real-World Examples**: Caching systems, configuration stores, read-heavy data structures

**Personal Usage**:
> **In-Memory Cache Implementation**
>
> I use ReaderWriterLockSlim for custom in-memory caches where reads are frequent but writes are rare.
>
> - **Challenge**: Application configuration cache read thousands of times per second but updated only on configuration changes
> - **Solution**: ReaderWriterLockSlim allows concurrent reads with exclusive writes
> - **Result**: High read throughput, safe updates, minimal contention
> - **Technology**: ReaderWriterLockSlim, IMemoryCache, concurrent collections

---

### 4. [Producer-Consumer](4_ProducerConsumer.cs)
**Problem**: Need to decouple data production from consumption with different processing rates
**Solution**: Use queue to buffer items between producer and consumer threads

**When to Use**:
- **Web API needs to queue background work** (most common in services!)
- Producers and consumers operate at different speeds
- Want to decouple generation from processing
- Need buffering between pipeline stages
- Need backpressure when queue is full

**Modern .NET**:
```csharp
// Using BlockingCollection
var queue = new BlockingCollection<WorkItem>();

// Producer
Task.Run(() =>
{
    foreach (var item in items)
    {
        queue.Add(item);
    }
    queue.CompleteAdding();
});

// Consumer
await foreach (var item in queue.GetConsumingEnumerable())
{
    await ProcessAsync(item);
}
```

**Real-World Examples**: 
- **Background work queues in Web APIs** (queue work items, process in background)
- Message processing systems (RabbitMQ, Azure Service Bus)
- Data pipelines (ETL processes)
- Event handling systems

**Personal Usage**:
> **Background Task Processing in ASP.NET Core**
>
> Most commonly used for **background work queues in Web APIs** - API receives requests and queues work, background service processes the queue.
>
> - **Challenge**: Web API endpoints need to return quickly but trigger long-running work (notifications, data processing, external API calls)
> - **Solution**: Bounded BlockingCollection/Channel where API queues work items and IHostedService processes them
> - **Result**: Fast API responses, backpressure when queue full (better than Task.Run which has no limits), controlled resource usage
> - **Technology**: System.Threading.Channels, IHostedService, ASP.NET Core
>
> Also used with Azure Service Bus where message arrival rate differs from processing capacity.

---

### 5. [Active Object](5_ActiveObject.cs)
**Problem**: Object methods called from multiple threads need synchronization
**Solution**: Decouple method invocation from execution by running on dedicated thread

**When to Use**:
- Object has mutable state accessed by multiple threads
- Want to serialize access without explicit locking
- Need async method execution

**Modern .NET**:
```csharp
// Active object with dedicated task scheduler
public class ActiveCounter
{
    private readonly Channel<Func<Task>> _channel = Channel.CreateUnbounded<Func<Task>>();
    private int _count = 0;

    public ActiveCounter()
    {
        // Dedicated thread processes all operations
        Task.Run(async () =>
        {
            await foreach (var operation in _channel.Reader.ReadAllAsync())
            {
                await operation();
            }
        });
    }

    public Task IncrementAsync()
    {
        var tcs = new TaskCompletionSource();
        _channel.Writer.TryWrite(async () =>
        {
            _count++;
            tcs.SetResult();
        });
        return tcs.Task;
    }
}
```

**Real-World Examples**: Actors (Akka.NET), single-threaded event loops, serial queues

**Personal Usage**:
> **Actor-Based Systems**
>
> I use active object pattern principles in actor-based systems where each actor processes messages sequentially on its own mailbox.
>
> - **Challenge**: Manage complex state machines with concurrent access without locks
> - **Solution**: Each actor is an active object processing messages serially
> - **Result**: No race conditions, simpler reasoning, natural async boundaries
> - **Technology**: Akka.NET actors, Orleans grains, actor model

---

## 🏆 Pattern Combinations

### Thread Pool + Producer-Consumer = Background Processing
```
ThreadPool workers → Process items from → BlockingCollection queue
```

### Future/Promise + Active Object = Async Actors
```
Async method calls → Queued to mailbox → Processed on dedicated thread
```

### Read-Write Lock + Lazy = Thread-Safe Lazy Cache
```
Lazy initialization → Protected by → Read-Write Lock for updates
```

---

## ⚠️ Common Pitfalls

1. **Thread pool exhaustion** - Blocking thread pool threads prevents new work from executing
2. **Deadlocks with locks** - Always acquire locks in consistent order
3. **Forgetting to dispose** - ReaderWriterLockSlim and SemaphoreSlim must be disposed
4. **Using locks for async** - Use SemaphoreSlim.WaitAsync instead of lock
5. **Producer-consumer queue unbounded** - Can cause memory exhaustion under load

---

[← Back to Main README](../../../README.md)
