namespace Audabit.Service.SoftwarePatterns.App.Patterns._8_AntiPatterns.Patterns;

// IDE0059/IDE0060 suppression: Intentional demo code showing anti-pattern structure
#pragma warning disable IDE0059 // Unnecessary assignment of a value
#pragma warning disable IDE0060 // Remove unused parameter

// God Object Anti-Pattern: A single class that knows/does too much, violating Single Responsibility Principle.
// This demonstrates the problem and the refactored solution using proper separation of concerns.
//
// PROBLEMS:
// - Single class with too many responsibilities
// - Tight coupling
// - Hard to test
// - Changes ripple through entire class
// - Violates SRP, OCP
//
// SOLUTION:
// - Separate responsibilities into focused classes
// - Dependency injection for flexibility
// - Each class has single, well-defined purpose
// - Easy to test, maintain, extend
public static class GodObject
{
    public static void Run()
    {
        Console.WriteLine("God Object Anti-Pattern: Demonstrating bloated vs focused classes...");
        Console.WriteLine("God Object Anti-Pattern: Single Responsibility Principle refactoring.\n");

        Console.WriteLine("God Object Anti-Pattern: --- BAD EXAMPLE: God Object ---\n");
        var godObject = new OrderManagerGodObject();
        OrderManagerGodObject.ProcessOrder("ORD-999", "CUST-123", 200.00m);

        Console.WriteLine("\nGod Object Anti-Pattern: --- GOOD EXAMPLE: Separated Responsibilities ---\n");
        var validator = new OrderValidator();
        var calculator = new OrderCalculator();
        var repository = new OrderRepository();
        var notifier = new OrderNotifier();
        var processor = new OrderProcessor(validator, calculator, repository, notifier);

        OrderProcessor.ProcessOrder("ORD-999", "CUST-123", 200.00m);

        Console.WriteLine("\nGod Object Anti-Pattern: Key Improvements:");
        Console.WriteLine("God Object Anti-Pattern: ✓ Single Responsibility: Each class has one job");
        Console.WriteLine("God Object Anti-Pattern: ✓ Testability: Can test validation, calculation, persistence separately");
        Console.WriteLine("God Object Anti-Pattern: ✓ Flexibility: Easy to swap implementations via interfaces");
        Console.WriteLine("God Object Anti-Pattern: ✓ Maintainability: Changes isolated to specific classes");
    }
}

// ❌ BAD: God Object that does everything
public class OrderManagerGodObject
{
    public static void ProcessOrder(string orderId, string customerId, decimal amount)
    {
        Console.WriteLine("God Object Anti-Pattern: [GOD OBJECT] Processing order...");

        // Responsibility 1: Validation
        if (string.IsNullOrEmpty(orderId) || string.IsNullOrEmpty(customerId) || amount <= 0)
        {
            Console.WriteLine("God Object Anti-Pattern: [GOD OBJECT] Validation failed");
            return;
        }

        // Responsibility 2: Tax calculation
        var tax = amount * 0.1m;
        var total = amount + tax;
        Console.WriteLine($"God Object Anti-Pattern: [GOD OBJECT] Calculated total: ${total}");

        // Responsibility 3: Database access
        Console.WriteLine($"God Object Anti-Pattern: [GOD OBJECT] Saving order {orderId} to database");

        // Responsibility 4: Email notification
        Console.WriteLine($"God Object Anti-Pattern: [GOD OBJECT] Sending email to customer {customerId}");

        // Responsibility 5: Inventory management
        Console.WriteLine("God Object Anti-Pattern: [GOD OBJECT] Updating inventory");

        // Responsibility 6: Logging
        Console.WriteLine("God Object Anti-Pattern: [GOD OBJECT] Logging order processed");

        // This class does EVERYTHING - hard to test, maintain, extend!
    }
}

// ✅ GOOD: Focused classes with single responsibilities

// Responsibility 1: Validation
public class OrderValidator
{
    public static bool Validate(string orderId, string customerId, decimal amount)
    {
        if (string.IsNullOrEmpty(orderId) || string.IsNullOrEmpty(customerId) || amount <= 0)
        {
            Console.WriteLine("God Object Anti-Pattern: [VALIDATOR] Validation failed");
            return false;
        }
        Console.WriteLine("God Object Anti-Pattern: [VALIDATOR] Validation passed");
        return true;
    }
}

// Responsibility 2: Calculation
public class OrderCalculator
{
    private const decimal TaxRate = 0.1m;

    public static decimal CalculateTotal(decimal amount)
    {
        var total = amount + (amount * TaxRate);
        Console.WriteLine($"God Object Anti-Pattern: [CALCULATOR] Calculated total: ${total}");
        return total;
    }
}

// Responsibility 3: Persistence
public class OrderRepository
{
    public static void Save(string orderId, decimal total)
    {
        Console.WriteLine($"God Object Anti-Pattern: [REPOSITORY] Saving order {orderId} to database");
    }
}

// Responsibility 4: Notification
public class OrderNotifier
{
    public static void NotifyCustomer(string customerId)
    {
        Console.WriteLine($"God Object Anti-Pattern: [NOTIFIER] Sending email to customer {customerId}");
    }
}

// Orchestrator with clear dependencies
public class OrderProcessor(
    OrderValidator validator,
    OrderCalculator calculator,
    OrderRepository repository,
    OrderNotifier notifier)
{
    private readonly OrderValidator _validator = validator;
    private readonly OrderCalculator _calculator = calculator;
    private readonly OrderRepository _repository = repository;
    private readonly OrderNotifier _notifier = notifier;

    public static void ProcessOrder(string orderId, string customerId, decimal amount)
    {
        Console.WriteLine("God Object Anti-Pattern: [PROCESSOR] Processing order...");

        if (!OrderValidator.Validate(orderId, customerId, amount))
            return;

        var total = OrderCalculator.CalculateTotal(amount);
        OrderRepository.Save(orderId, total);
        OrderNotifier.NotifyCustomer(customerId);

        Console.WriteLine("God Object Anti-Pattern: [PROCESSOR] Order processed successfully");
    }
}