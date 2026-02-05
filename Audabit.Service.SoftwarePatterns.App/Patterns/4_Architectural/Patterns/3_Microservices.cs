namespace Audabit.Service.SoftwarePatterns.App.Patterns._4_Architectural.Patterns;

// Microservices Pattern: Structures an application as a collection of small, autonomous services that are independently deployable and communicate via lightweight protocols.
// LEVEL: Infrastructure-level architecture pattern (distributed systems, network boundaries, separate deployments)
// In this example:
// - We have three independent microservices: UserService, OrderService, and NotificationService.
// - Each service has its own data store and business logic (single responsibility).
// - Services communicate via HTTP APIs (could also use message queues in production).
// - The key here is that each service can be developed, deployed, and scaled independently.
//
// MODERN C# USAGE:
// Microservices are the foundation of modern cloud-native .NET applications:
// - Use ASP.NET Core Minimal APIs or Web API for each microservice
// - Deploy each service as a Docker container: FROM mcr.microsoft.com/dotnet/aspnet:9.0
// - Orchestrate with Kubernetes or Azure Container Apps
// - Use service mesh (Istio, Dapr) for cross-cutting concerns (retries, circuit breakers, observability)
// - Implement API Gateway pattern with Azure API Management or Ocelot
// - Use message brokers (Azure Service Bus, RabbitMQ, Kafka) for asynchronous communication
// - Each service has its own database (database per service pattern)
public static class Microservices
{
    public static void Run()
    {
        Console.WriteLine("Microservices Pattern: Decomposing application into independent services...");
        Console.WriteLine("Microservices Pattern: Each service owns its data and communicates via APIs.\n");

        // Simulate independent microservices
        var userService = new UserMicroservice();
        var orderService = new OrderMicroservice();
        _ = new NotificationMicroservice();

        Console.WriteLine("Microservices Pattern: --- User Registration Flow ---");

        // User Service: Register new user
        var userId = userService.RegisterUser("john.doe@example.com", "John Doe");

        // Order Service: Create order for user
        var orderId = orderService.CreateOrder(userId, 149.99m);

        // Notification Service: Send confirmation
        NotificationMicroservice.SendOrderConfirmation(userId, orderId);
    }
}

// ============================================
// USER MICROSERVICE
// ============================================
public class UserMicroservice
{
    private readonly List<UserEntity> _userDatabase = [];
    private int _nextId = 1;

    public int RegisterUser(string email, string name)
    {
        Console.WriteLine($"\nMicroservices Pattern: [User Service] POST /api/users");
        Console.WriteLine($"Microservices Pattern: [User Service] Registering user: {name}");

        var user = new UserEntity
        {
            Id = _nextId++,
            Email = email,
            Name = name
        };

        _userDatabase.Add(user);
        Console.WriteLine($"Microservices Pattern: [User Service] User created with ID: {user.Id}");
        Console.WriteLine($"Microservices Pattern: [User Service] Stored in UserService database");

        return user.Id;
    }

    public UserEntity? GetUser(int userId)
    {
        return _userDatabase.FirstOrDefault(u => u.Id == userId);
    }
}

// ============================================
// ORDER MICROSERVICE
// ============================================
public class OrderMicroservice
{
    private readonly List<OrderEntity> _orderDatabase = [];
    private int _nextOrderId = 1000;

    public int CreateOrder(int userId, decimal amount)
    {
        Console.WriteLine($"\nMicroservices Pattern: [Order Service] POST /api/orders");
        Console.WriteLine($"Microservices Pattern: [Order Service] Creating order for user ID: {userId}");

        var order = new OrderEntity
        {
            OrderId = _nextOrderId++,
            UserId = userId,
            Amount = amount,
            Status = "Pending"
        };

        _orderDatabase.Add(order);
        Console.WriteLine($"Microservices Pattern: [Order Service] Order created: #{order.OrderId}");
        Console.WriteLine($"Microservices Pattern: [Order Service] Amount: ${amount}");
        Console.WriteLine($"Microservices Pattern: [Order Service] Stored in OrderService database");

        return order.OrderId;
    }
}

// ============================================
// NOTIFICATION MICROSERVICE
// ============================================
public class NotificationMicroservice
{
    public static void SendOrderConfirmation(int userId, int orderId)
    {
        Console.WriteLine($"\nMicroservices Pattern: [Notification Service] POST /api/notifications");
        Console.WriteLine($"Microservices Pattern: [Notification Service] Sending confirmation for order #{orderId}");
        Console.WriteLine($"Microservices Pattern: [Notification Service] To user ID: {userId}");
        Console.WriteLine($"Microservices Pattern: [Notification Service] Email sent successfully");
        Console.WriteLine($"Microservices Pattern: [Notification Service] Notification logged in NotificationService database");
    }
}

// ============================================
// DATA MODELS (Each service has its own)
// ============================================
public class UserEntity
{
    public int Id { get; init; }
    public required string Email { get; init; }
    public required string Name { get; init; }
}

public class OrderEntity
{
    public int OrderId { get; init; }
    public int UserId { get; init; }
    public decimal Amount { get; init; }
    public required string Status { get; init; }
}