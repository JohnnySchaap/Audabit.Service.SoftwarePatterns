namespace Audabit.Service.SoftwarePatterns.App.Patterns._1_Creational.Patterns;

// Dependency Injection Pattern: Objects receive dependencies from external sources rather than creating them internally.
// In this example:
// - We contrast tight coupling (OrderServiceBad creates its own dependencies with 'new') vs loose coupling (OrderServiceGood receives dependencies via constructor).
// - With DI, we can easily swap implementations (e.g., EmailNotifier for SmsNotifier) without changing the OrderService code.
// - Dependencies are injected through the constructor (constructor injection), making them explicit and testable.
// - The key here is that objects declare what they need, and an external source (DI container or manual injection) provides those dependencies.
//
// MODERN C# USAGE:
// .NET has built-in DI container (Microsoft.Extensions.DependencyInjection):
//   services.AddScoped<IRepository, SqlRepository>();
//   services.AddSingleton<ILogger, ConsoleLogger>();
// Constructor injection is the standard:
//   public class OrderService(IRepository repo, ILogger logger) { } // Primary constructor (C# 12+)
// Lifetimes: Transient (new each time), Scoped (per request), Singleton (one instance)
// DI is the foundation of modern .NET - ALL production code should use it.
// Manual 'new' for dependencies is an anti-pattern except for DTOs, value objects, or primitives.
public static class DependencyInjection
{
    public static void Run()
    {
        Console.WriteLine("Dependency Injection Pattern: Injecting dependencies instead of creating them...");
        Console.WriteLine("Dependency Injection Pattern: Comparing tight coupling vs loose coupling.\n");

        // WITHOUT Dependency Injection (Tight Coupling)
        Console.WriteLine("\nDependency Injection Pattern: --- WITHOUT DI (Tight Coupling) ---");
        var badService = new OrderServiceBad();
        badService.ProcessOrder(101);

        // WITH Dependency Injection (Loose Coupling)
        Console.WriteLine("\nDependency Injection Pattern: --- WITH DI (Loose Coupling) ---");

        // 1. Constructor Injection
        IRepository repository = new SqlRepository();
        ILogger logger = new ConsoleLogger();
        var goodService = new OrderServiceGood(repository, logger);
        goodService.ProcessOrder(102);

        // 2. Easy to swap implementations
        Console.WriteLine("\nDependency Injection Pattern: --- Swapping to Mock Implementation ---");
        IRepository mockRepository = new MockRepository();
        var testService = new OrderServiceGood(mockRepository, logger);
        testService.ProcessOrder(103);
    }
}

// WITHOUT DI - Tightly coupled to concrete implementations
public sealed class OrderServiceBad
{
    private readonly SqlRepository _repository = new(); // Hard-coded dependency!
    private readonly ConsoleLogger _logger = new();     // Can't change or test!

    public void ProcessOrder(int orderId)
    {
        _logger.Log($"Processing order {orderId}");
        _repository.Save($"Order {orderId}");
    }
}

// WITH DI - Loosely coupled to abstractions
public sealed class OrderServiceGood(IRepository repository, ILogger logger)
{
    private readonly IRepository _repository = repository;
    private readonly ILogger _logger = logger;

    public void ProcessOrder(int orderId)
    {
        _logger.Log($"Processing order {orderId}");
        _repository.Save($"Order {orderId}");
    }
}

// Abstractions (Interfaces)
public interface IRepository
{
    void Save(string data);
}

public interface ILogger
{
    void Log(string message);
}

// Concrete Implementations
public sealed class SqlRepository : IRepository
{
    public void Save(string data)
    {
        Console.WriteLine($"Dependency Injection Pattern: SqlRepository: Saving '{data}' to SQL database");
    }
}

public sealed class MockRepository : IRepository
{
    public void Save(string data)
    {
        Console.WriteLine($"Dependency Injection Pattern: MockRepository: Pretending to save '{data}' (for testing)");
    }
}

public sealed class ConsoleLogger : ILogger
{
    public void Log(string message)
    {
        Console.WriteLine($"Dependency Injection Pattern: ConsoleLogger: {message}");
    }
}