namespace Audabit.Service.SoftwarePatterns.App.Principles._0_Principles.Principles;

// Pragma suppressions: Intentional for demonstration purposes
#pragma warning disable IDE0060 // Remove unused parameter
#pragma warning disable IDE0059 // Unnecessary assignment
#pragma warning disable CA1859 // Use concrete types - intentional for ISP demonstration

// SOLID Principles: Five fundamental object-oriented design principles
// S - Single Responsibility Principle
// O - Open/Closed Principle
// L - Liskov Substitution Principle
// I - Interface Segregation Principle
// D - Dependency Inversion Principle

// MODERN C# USAGE:
// - SOLID guides class design and architecture decisions
// - Use interfaces and dependency injection for D
// - Primary constructors support D
// - Records support S (immutable, focused data)
// - Pattern matching supports O (polymorphism)
public static class SOLID
{
    public static void Run()
    {
        Console.WriteLine("SOLID Principles: Demonstrating five fundamental OOP design principles...");
        Console.WriteLine("SOLID Principles: Five core principles for maintainable object-oriented design.\n");

        // Single Responsibility Principle
        Console.WriteLine("SOLID Principles: --- 1. Single Responsibility Principle (SRP) ---");
        Console.WriteLine("SOLID Principles: A class should have only ONE reason to change.\n");

        var order = new Order { Id = "ORD-123", Total = 100m, CustomerEmail = "customer@example.com" };

        // ❌ BAD: OrderProcessor has multiple responsibilities
        // var badProcessor = new OrderProcessorBad();
        // badProcessor.Process(order); // Handles validation, calculation, persistence, notification!

        // ✅ GOOD: Each class has a single responsibility
        var validator = new OrderValidator();
        var calculator = new OrderCalculator();
        var repository = new OrderRepository();
        var notifier = new OrderNotifier();

        if (OrderValidator.Validate(order))
        {
            OrderCalculator.CalculateTotal(order);
            OrderRepository.Save(order);
            OrderNotifier.NotifyCustomer(order.CustomerEmail);
        }

        Console.WriteLine("SOLID Principles: SRP: Validation, calculation, persistence, notification separated.\n");

        // Open/Closed Principle
        Console.WriteLine("SOLID Principles: --- 2. Open/Closed Principle (OCP) ---");
        Console.WriteLine("SOLID Principles: Open for extension, closed for modification.\n");

        var standardCustomer = new StandardCustomer();
        var premiumCustomer = new PremiumCustomer();
        var vipCustomer = new VipCustomer();

        var discountCalculator = new DiscountCalculator();
        Console.WriteLine($"SOLID Principles: OCP: Standard discount: {DiscountCalculator.Calculate(standardCustomer, 100m):C}");
        Console.WriteLine($"SOLID Principles: OCP: Premium discount: {DiscountCalculator.Calculate(premiumCustomer, 100m):C}");
        Console.WriteLine($"SOLID Principles: OCP: VIP discount: {DiscountCalculator.Calculate(vipCustomer, 100m):C}");
        Console.WriteLine("SOLID Principles: OCP: New customer types can be added without modifying DiscountCalculator.\n");

        // Liskov Substitution Principle
        Console.WriteLine("SOLID Principles: --- 3. Liskov Substitution Principle (LSP) ---");
        Console.WriteLine("SOLID Principles: Subtypes must be substitutable for their base types.\n");

        ProcessPayment(new CreditCardPayment());
        ProcessPayment(new PayPalPayment());
        ProcessPayment(new BankTransferPayment());
        Console.WriteLine("SOLID Principles: LSP: All payment types work interchangeably.\n");

        // Interface Segregation Principle
        Console.WriteLine("SOLID Principles: --- 4. Interface Segregation Principle (ISP) ---");
        Console.WriteLine("SOLID Principles: Clients shouldn't depend on interfaces they don't use.\n");

        IReadOnlyRepository readRepo = new Repository();
        Console.WriteLine($"SOLID Principles: ISP: Read-only access: {readRepo.GetById("123")?.Id}");

        IWriteOnlyRepository writeRepo = new Repository();
        writeRepo.Save(new Order { Id = "ORD-456", Total = 200m });
        Console.WriteLine("SOLID Principles: ISP: Clients only see methods they need.\n");

        // Dependency Inversion Principle
        Console.WriteLine("SOLID Principles: --- 5. Dependency Inversion Principle (DIP) ---");
        Console.WriteLine("SOLID Principles: Depend on abstractions, not concretions.\n");

        INotificationService emailService = new EmailNotificationService();
        INotificationService smsService = new SmsNotificationService();

        var orderService1 = new OrderService(emailService);
        orderService1.ProcessOrder();

        var orderService2 = new OrderService(smsService);
        orderService2.ProcessOrder();

        Console.WriteLine("SOLID Principles: DIP: OrderService depends on INotificationService abstraction, not concrete implementations.\n");
    }

    private static void ProcessPayment(IPaymentMethod payment)
    {
        payment.Process(100m);
    }
}

// --- Single Responsibility Principle Examples ---

internal class Order
{
    public string Id { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public string CustomerEmail { get; set; } = string.Empty;
}

// ✅ GOOD: Each class has ONE responsibility
internal class OrderValidator
{
    public static bool Validate(Order order)
    {
        return !string.IsNullOrEmpty(order.Id) && order.Total > 0;
    }
}

internal class OrderCalculator
{
    public static void CalculateTotal(Order order)
    {
        // Calculate taxes, discounts, etc.
    }
}

internal class OrderRepository
{
    public static void Save(Order order)
    {
        Console.WriteLine($"SOLID Principles: SRP: Saved order {order.Id}");
    }
}

internal class OrderNotifier
{
    public static void NotifyCustomer(string email)
    {
        Console.WriteLine($"SOLID Principles: SRP: Sent notification to {email}");
    }
}

// --- Open/Closed Principle Examples ---

// ✅ GOOD: Open for extension (add new customer types)
// ✅ GOOD: Closed for modification (DiscountCalculator doesn't change)
public interface ICustomer
{
    decimal GetDiscount(decimal amount);
}

internal class StandardCustomer : ICustomer
{
    public decimal GetDiscount(decimal amount)
    {
        return 0; // No discount
    }
}

internal class PremiumCustomer : ICustomer
{
    public decimal GetDiscount(decimal amount)
    {
        return amount * 0.10m; // 10% discount
    }
}

internal class VipCustomer : ICustomer
{
    public decimal GetDiscount(decimal amount)
    {
        return amount * 0.20m; // 20% discount
    }
}

internal class DiscountCalculator
{
    public static decimal Calculate(ICustomer customer, decimal amount)
    {
        return customer.GetDiscount(amount);
    }
}

// --- Liskov Substitution Principle Examples ---

// ✅ GOOD: All implementations can substitute the base interface
public interface IPaymentMethod
{
    void Process(decimal amount);
}

internal class CreditCardPayment : IPaymentMethod
{
    public void Process(decimal amount)
    {
        Console.WriteLine($"SOLID Principles: LSP: Processed ${amount} via Credit Card");
    }
}

internal class PayPalPayment : IPaymentMethod
{
    public void Process(decimal amount)
    {
        Console.WriteLine($"SOLID Principles: LSP: Processed ${amount} via PayPal");
    }
}

internal class BankTransferPayment : IPaymentMethod
{
    public void Process(decimal amount)
    {
        Console.WriteLine($"SOLID Principles: LSP: Processed ${amount} via Bank Transfer");
    }
}

// --- Interface Segregation Principle Examples ---

// ✅ GOOD: Segregated interfaces - clients only see what they need
internal interface IReadOnlyRepository
{
    Order? GetById(string id);
}

internal interface IWriteOnlyRepository
{
    void Save(Order order);
}

internal class Repository : IReadOnlyRepository, IWriteOnlyRepository
{
    public Order? GetById(string id)
    {
        return new Order { Id = id };
    }

    public void Save(Order order)
    {
        Console.WriteLine($"SOLID Principles: ISP: Saved {order.Id}");
    }
}

// --- Dependency Inversion Principle Examples ---

// ✅ GOOD: High-level module depends on abstraction
public interface INotificationService
{
    void Send(string message);
}

internal class EmailNotificationService : INotificationService
{
    public void Send(string message)
    {
        Console.WriteLine($"SOLID Principles: DIP: Email: {message}");
    }
}

internal class SmsNotificationService : INotificationService
{
    public void Send(string message)
    {
        Console.WriteLine($"SOLID Principles: DIP: SMS: {message}");
    }
}

internal class OrderService(INotificationService notificationService)
{
    public void ProcessOrder()
    {
        // Process order logic
        notificationService.Send("Order processed successfully");
    }
}