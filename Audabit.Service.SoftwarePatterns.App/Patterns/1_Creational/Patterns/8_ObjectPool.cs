namespace Audabit.Service.SoftwarePatterns.App.Patterns._1_Creational.Patterns;

// Object Pool Pattern: Reuses objects instead of creating and destroying them repeatedly, improving performance.
// In this example:
// - We create a ConnectionPool that manages a fixed pool of DatabaseConnection objects.
// - The pool provides AcquireConnection() and ReleaseConnection() methods to borrow and return objects.
// - When the pool is exhausted, AcquireConnection() returns null instead of creating new connections.
// - The key here is that expensive objects (like database connections) are reused rather than recreated, significantly improving performance.
//
// MODERN C# USAGE:
// .NET provides built-in object pooling:
// - ArrayPool<T>: ArrayPool<byte>.Shared.Rent(size) / Return(array) - for byte arrays
// - Microsoft.Extensions.ObjectPool: ObjectPool<T> with PooledObjectPolicy<T>
// - DbContext pooling: services.AddDbContextPool<MyContext>() - for EF Core
// - HttpClient pooling: IHttpClientFactory automatically pools HttpClient instances
// Use built-in pools instead of implementing your own. Custom pools are only needed for domain-specific expensive objects.
// Always use 'using' or try/finally to ensure resources are returned to the pool.
public static class ObjectPool
{
    public static void Run()
    {
        Console.WriteLine("Object Pool Pattern: Reusing expensive objects from a pool...");
        Console.WriteLine("Object Pool Pattern: Pool manages limited resources and reuses them efficiently.\n");

        var pool = new ConnectionPool(maxSize: 3);

        Console.WriteLine("Object Pool Pattern: --- Acquiring Connections ---");
        var conn1 = pool.AcquireConnection();
        var conn2 = pool.AcquireConnection();
        var conn3 = pool.AcquireConnection();

        conn1.Execute("Query 1");
        conn2.Execute("Query 2");
        conn3.Execute("Query 3");

        Console.WriteLine("\nObject Pool Pattern: --- Releasing Connections ---");
        pool.ReleaseConnection(conn1);
        pool.ReleaseConnection(conn2);

        Console.WriteLine("\nObject Pool Pattern: --- Reusing Pooled Connection ---");
        var conn4 = pool.AcquireConnection();
        conn4.Execute("Query 4");
    }
}

public sealed class ConnectionPool(int maxSize)
{
    private readonly Queue<DatabaseConnection> _availableConnections = new();
    private readonly int _maxSize = maxSize;
    private int _currentSize;

    public DatabaseConnection AcquireConnection()
    {
        if (_availableConnections.Count > 0)
        {
            var conn = _availableConnections.Dequeue();
            Console.WriteLine($"Object Pool Pattern: Reusing connection {conn.Id} from pool");
            return conn;
        }

        if (_currentSize < _maxSize)
        {
            _currentSize++;
            var conn = new DatabaseConnection(_currentSize);
            Console.WriteLine($"Object Pool Pattern: Creating new connection {conn.Id}");
            return conn;
        }

        throw new InvalidOperationException("Pool exhausted: No available connections");
    }

    public void ReleaseConnection(DatabaseConnection connection)
    {
        Console.WriteLine($"Object Pool Pattern: Returning connection {connection.Id} to pool");
        connection.Reset();
        _availableConnections.Enqueue(connection);
    }
}

public sealed class DatabaseConnection(int id)
{
    public int Id { get; } = id;

    public void Execute(string query)
    {
        Console.WriteLine($"Object Pool Pattern: Connection {Id}: Executing '{query}'");
    }

    public void Reset()
    {
        Console.WriteLine($"Object Pool Pattern: Connection {Id}: Resetting state");
    }
}