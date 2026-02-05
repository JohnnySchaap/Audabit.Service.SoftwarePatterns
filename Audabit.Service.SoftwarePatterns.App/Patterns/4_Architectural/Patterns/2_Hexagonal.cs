namespace Audabit.Service.SoftwarePatterns.App.Patterns._4_Architectural.Patterns;

// Hexagonal Pattern (Ports and Adapters): Isolates the core business logic from external concerns by defining clear boundaries through ports and adapters.
// LEVEL: Code-level architecture pattern (how you structure dependencies and abstractions using interfaces)
// In this example:
// - The core domain (OrderService) is isolated from infrastructure concerns (database, email, payment).
// - Ports are interfaces that define how to interact with the core (IOrderRepository, IEmailService, IPaymentGateway).
// - Adapters are concrete implementations that connect external systems to the core.
// - The key here is that the core doesn't depend on infrastructure - infrastructure depends on the core through interfaces.
//
// MODERN C# USAGE:
// Hexagonal Architecture (also called Ports & Adapters or Clean Architecture) is very popular in modern .NET:
// - Use dependency injection to inject adapters: services.AddScoped<IOrderRepository, SqlOrderRepository>();
// - Core domain in MyApp.Domain project with only business logic and port interfaces
// - Adapters in MyApp.Infrastructure project (SqlOrderRepository, SmtpEmailService, StripePaymentGateway)
// - ASP.NET Core controllers are also adapters (HTTP adapter) that call the core through ports
// - This architecture makes testing easy - use fake/mock adapters for unit tests
// - Easily swap implementations (SQL → MongoDB, SMTP → SendGrid) without touching core logic
public static class Hexagonal
{
    public static void Run()
    {
        Console.WriteLine("Hexagonal Pattern: Isolating core business logic from infrastructure...");
        Console.WriteLine("Hexagonal Pattern: Core depends on ports (interfaces), adapters implement ports for external systems.\n");

        // Setup: Wire adapters to ports (in real app, done via DI container)
        IOrderRepository orderRepo = new InMemoryOrderRepository();
        IEmailService emailService = new ConsoleEmailService();
        IPaymentGateway paymentGateway = new FakePaymentGateway();

        // Core: Business logic uses ports, doesn't know about adapters
        var orderService = new OrderService(orderRepo, emailService, paymentGateway);

        Console.WriteLine("Hexagonal Pattern: --- Processing Customer Order ---");
        orderService.PlaceOrder("ORD-12345", "john.doe@example.com", 99.99m);
    }
}

// ============================================
// CORE DOMAIN (Business Logic)
// ============================================
public class OrderService(
    IOrderRepository orderRepository,
    IEmailService emailService,
    IPaymentGateway paymentGateway)
{
    // Ports: Dependencies defined as interfaces
    private readonly IOrderRepository _orderRepository = orderRepository;
    private readonly IEmailService _emailService = emailService;
    private readonly IPaymentGateway _paymentGateway = paymentGateway;

    public void PlaceOrder(string orderId, string customerEmail, decimal amount)
    {
        Console.WriteLine($"Hexagonal Pattern: [CORE] Processing order {orderId} for ${amount}");

        // Business logic: Charge payment
        var paymentSuccess = _paymentGateway.ChargePayment(customerEmail, amount);

        if (!paymentSuccess)
        {
            Console.WriteLine("Hexagonal Pattern: [CORE] Payment failed. Order cancelled.\n");
            return;
        }

        // Business logic: Save order
        var order = new Order
        {
            OrderId = orderId,
            CustomerEmail = customerEmail,
            Amount = amount,
            Status = "Confirmed"
        };

        _orderRepository.Save(order);

        // Business logic: Send confirmation
        _emailService.SendEmail(customerEmail, "Order Confirmed", $"Your order {orderId} has been placed successfully!");

        Console.WriteLine($"Hexagonal Pattern: [CORE] Order {orderId} completed successfully\n");
    }
}

// ============================================
// PORTS (Interfaces - Define boundaries)
// ============================================
public interface IOrderRepository
{
    void Save(Order order);
    Order? GetById(string orderId);
}

public interface IEmailService
{
    void SendEmail(string to, string subject, string body);
}

public interface IPaymentGateway
{
    bool ChargePayment(string customerEmail, decimal amount);
}

// ============================================
// ADAPTERS (Implementations - Connect to external systems)
// ============================================
public class InMemoryOrderRepository : IOrderRepository
{
    private readonly List<Order> _orders = [];

    public void Save(Order order)
    {
        Console.WriteLine($"Hexagonal Pattern: [ADAPTER:Repository] Saving order {order.OrderId} to database");
        _orders.Add(order);
    }

    public Order? GetById(string orderId)
    {
        return _orders.FirstOrDefault(o => o.OrderId == orderId);
    }
}

public class ConsoleEmailService : IEmailService
{
    public void SendEmail(string to, string subject, string body)
    {
        Console.WriteLine($"Hexagonal Pattern: [ADAPTER:Email] Sending email to {to}");
        Console.WriteLine($"Hexagonal Pattern: [ADAPTER:Email] Subject: {subject}");
        Console.WriteLine($"Hexagonal Pattern: [ADAPTER:Email] Body: {body}");
    }
}

public class FakePaymentGateway : IPaymentGateway
{
    public bool ChargePayment(string customerEmail, decimal amount)
    {
        Console.WriteLine($"Hexagonal Pattern: [ADAPTER:Payment] Charging ${amount} to {customerEmail}");
        Console.WriteLine("Hexagonal Pattern: [ADAPTER:Payment] Payment processed successfully");
        return true;
    }
}

// ============================================
// DOMAIN MODEL
// ============================================
public class Order
{
    public required string OrderId { get; init; }
    public required string CustomerEmail { get; init; }
    public required decimal Amount { get; init; }
    public required string Status { get; init; }
}