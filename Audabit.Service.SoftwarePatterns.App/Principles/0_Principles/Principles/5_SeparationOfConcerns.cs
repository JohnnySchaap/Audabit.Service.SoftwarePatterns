namespace Audabit.Service.SoftwarePatterns.App.Principles._0_Principles.Principles;

#pragma warning disable IDE0059 // Unnecessary assignment - intentional for demonstration
#pragma warning disable CA1822 // Mark members as static - intentional for demonstration
#pragma warning disable CS9113 // Parameter is unread - intentional for demonstration

// Separation of Concerns: A design principle for separating a program into distinct sections,
// each addressing a separate concern (piece of functionality)

// MODERN C# USAGE:
// - MVC/MVVM patterns separate UI, logic, data
// - Layered architecture (Presentation, Business, Data)
// - Middleware pipeline separates cross-cutting concerns
// - Dependency injection separates object creation from usage
// - CQRS separates reads from writes
public static class SeparationOfConcerns
{
    public static void Run()
    {
        Console.WriteLine("Separation of Concerns: Demonstrating concern separation in layered architecture...");
        Console.WriteLine("Separation of Concerns: Different aspects of functionality should be handled by separate components.\n");

        // Example 1: Monolithic mess vs Layered architecture
        Console.WriteLine("Separation of Concerns: --- Example 1: Web Application Layers ---\n");

        // ❌ BAD: All concerns mixed together
        Console.WriteLine("Separation of Concerns: ❌ BAD: MonolithicController:");
        Console.WriteLine("Separation of Concerns: ❌ BAD:   - HTTP request parsing (Presentation)");
        Console.WriteLine("Separation of Concerns: ❌ BAD:   - Business validation (Business Logic)");
        Console.WriteLine("Separation of Concerns: ❌ BAD:   - Database queries (Data Access)");
        Console.WriteLine("Separation of Concerns: ❌ BAD:   - Email sending (Infrastructure)");
        Console.WriteLine("Separation of Concerns: ❌ BAD: Result: 500-line controller, impossible to test\n");

        // ✅ GOOD: Separated concerns
        Console.WriteLine("Separation of Concerns: ✅ GOOD: Layered Architecture:");

        // Presentation Layer
        var controller = new SocOrderController(new SocOrderService(new SocOrderRepository(), new EmailService()));
        controller.CreateOrder(new SocOrderRequest { ProductId = 123, Quantity = 2 });

        Console.WriteLine();

        // Example 2: Data access separation
        Console.WriteLine("Separation of Concerns: --- Example 2: Data Access Layer ---\n");

        // ❌ BAD: SQL queries in business logic
        Console.WriteLine("Separation of Concerns: ❌ BAD: Business logic with embedded SQL:");
        Console.WriteLine("Separation of Concerns: ❌ BAD:   var sql = \"SELECT * FROM Users WHERE Id = @id\";");
        Console.WriteLine("Separation of Concerns: ❌ BAD:   var SocUser = connection.Query<SocUser>(sql, new { id });");
        Console.WriteLine("Separation of Concerns: ❌ BAD: Result: Cannot switch databases, hard to test\n");

        // ✅ GOOD: Repository pattern separates data access
        Console.WriteLine("Separation of Concerns: ✅ GOOD: Repository pattern:");
        var SocUserRepository = new SocUserRepository();
        var SocUser = SocUserRepository.GetById(1);
        Console.WriteLine($"Separation of Concerns: ✅ GOOD: Retrieved SocUser: {SocUser.Name}");
        Console.WriteLine("Separation of Concerns: ✅ GOOD: Business logic doesn't know about SQL\n");

        // Example 3: Cross-cutting concerns
        Console.WriteLine("Separation of Concerns: --- Example 3: Cross-Cutting Concerns ---\n");

        // ❌ BAD: Logging mixed with business logic
        Console.WriteLine("Separation of Concerns: ❌ BAD: Logging scattered everywhere:");
        Console.WriteLine("Separation of Concerns: ❌ BAD:   logger.Log(\"Starting validation\");");
        Console.WriteLine("Separation of Concerns: ❌ BAD:   // validation logic");
        Console.WriteLine("Separation of Concerns: ❌ BAD:   logger.Log(\"Validation complete\");");
        Console.WriteLine("Separation of Concerns: ❌ BAD: Result: Business logic cluttered with infrastructure\n");

        // ✅ GOOD: Middleware/Decorator handles cross-cutting concerns
        Console.WriteLine("Separation of Concerns: ✅ GOOD: Middleware pattern:");
        var service = new PaymentService();
        var loggingDecorator = new LoggingDecorator(service);
        loggingDecorator.ProcessPayment(100m);
        Console.WriteLine("Separation of Concerns: ✅ GOOD: Business logic stays clean\n");

        // Example 4: UI and business logic separation
        Console.WriteLine("Separation of Concerns: --- Example 4: UI Separation ---\n");

        // ❌ BAD: Business logic in UI layer
        Console.WriteLine("Separation of Concerns: ❌ BAD: UI component with business logic:");
        Console.WriteLine("Separation of Concerns: ❌ BAD:   void OnButtonClick() {");
        Console.WriteLine("Separation of Concerns: ❌ BAD:     var total = price * quantity;");
        Console.WriteLine("Separation of Concerns: ❌ BAD:     var tax = total * 0.08m;");
        Console.WriteLine("Separation of Concerns: ❌ BAD:     UpdateDisplay(total + tax);");
        Console.WriteLine("Separation of Concerns: ❌ BAD:   }");
        Console.WriteLine("Separation of Concerns: ❌ BAD: Result: Cannot reuse logic, hard to test\n");

        // ✅ GOOD: Business logic in separate service
        Console.WriteLine("Separation of Concerns: ✅ GOOD: Business logic separated:");
        var SocPriceCalculator = new SocPriceCalculator();
        var finalPrice = SocPriceCalculator.CalculateFinalPrice(100m, 2);
        Console.WriteLine($"Separation of Concerns: ✅ GOOD: Final price: ${finalPrice}");
        Console.WriteLine("Separation of Concerns: ✅ GOOD: UI only handles display, logic is reusable\n");

        // Example 5: Configuration vs Logic
        Console.WriteLine("Separation of Concerns: --- Example 5: Configuration Separation ---\n");

        // ❌ BAD: Configuration hardcoded in logic
        Console.WriteLine("Separation of Concerns: ❌ BAD: Hardcoded values:");
        Console.WriteLine("Separation of Concerns: ❌ BAD:   var timeout = 30;");
        Console.WriteLine("Separation of Concerns: ❌ BAD:   var apiUrl = \"https://api.example.com\";");
        Console.WriteLine("Separation of Concerns: ❌ BAD: Result: Cannot change without recompiling\n");

        // ✅ GOOD: Configuration externalized
        Console.WriteLine("Separation of Concerns: ✅ GOOD: External configuration:");
        var config = new AppConfiguration();
        Console.WriteLine($"Separation of Concerns: ✅ GOOD: Timeout: {AppConfiguration.Timeout}s");
        Console.WriteLine($"Separation of Concerns: ✅ GOOD: API URL: {AppConfiguration.ApiUrl}");
        Console.WriteLine("Separation of Concerns: ✅ GOOD: Configuration separated from logic\n");

        Console.WriteLine("Separation of Concerns: Key Takeaway: Each layer/component has ONE clear responsibility!");
    }
}

// ✅ GOOD: Presentation Layer (HTTP concerns)
internal class SocOrderController(SocOrderService SocOrderService)
{
    public string CreateOrder(SocOrderRequest request)
    {
        Console.WriteLine("Separation of Concerns: ✅ [Presentation] Received HTTP request");

        var orderId = SocOrderService.CreateOrder(request.ProductId, request.Quantity);

        Console.WriteLine("Separation of Concerns: ✅ [Presentation] Returning HTTP response");
        return $"SocOrder created: {orderId}";
    }
}

internal class SocOrderRequest
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}

// ✅ GOOD: Business Logic Layer (validation, orchestration)
internal class SocOrderService(SocOrderRepository repository, EmailService emailService)
{
    public int CreateOrder(int productId, int quantity)
    {
        Console.WriteLine("Separation of Concerns: ✅ [Business Logic] Validating SocOrder");

        if (quantity <= 0)
        {
            throw new ArgumentException("Quantity must be positive");
        }

        Console.WriteLine("Separation of Concerns: ✅ [Business Logic] Processing SocOrder");
        var orderId = SocOrderRepository.SaveOrder(productId, quantity);

        EmailService.SendOrderConfirmation(orderId);

        return orderId;
    }
}

// ✅ GOOD: Data Access Layer (database concerns)
internal class SocOrderRepository
{
    private static int _orderIdCounter = 1000;

    public static int SaveOrder(int productId, int quantity)
    {
        Console.WriteLine($"Separation of Concerns: ✅ [Data Access] Saving to database: Product {productId}, Qty {quantity}");
        return _orderIdCounter++;
    }

    public static SocOrder GetById(int id)
    {
        return new SocOrder { Id = id, ProductId = 123, Quantity = 2 };
    }
}

internal class SocOrder
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}

// ✅ GOOD: Infrastructure Layer (external services)
internal class EmailService
{
    public static void SendOrderConfirmation(int orderId)
    {
        Console.WriteLine($"Separation of Concerns: ✅ [Infrastructure] Sending email for SocOrder {orderId}");
    }
}

// ✅ GOOD: SocUser repository
internal class SocUser
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

internal class SocUserRepository
{
    public static SocUser GetById(int id)
    {
        return new SocUser { Id = id, Name = "John Doe" };
    }
}

// ✅ GOOD: Decorator for cross-cutting concerns
public interface IPaymentService
{
    void ProcessPayment(decimal amount);
}

internal class PaymentService : IPaymentService
{
    public void ProcessPayment(decimal amount)
    {
        Console.WriteLine($"Separation of Concerns: ✅ [Business Logic] Processing ${amount} payment");
    }
}

internal class LoggingDecorator(IPaymentService inner) : IPaymentService
{
    public void ProcessPayment(decimal amount)
    {
        Console.WriteLine("Separation of Concerns: ✅ [Cross-Cutting] Logging: Payment started");
        inner.ProcessPayment(amount);
        Console.WriteLine("Separation of Concerns: ✅ [Cross-Cutting] Logging: Payment completed");
    }
}

// ✅ GOOD: Price calculator (business logic)
internal class SocPriceCalculator
{
    private const decimal TaxRate = 0.08m;

    public static decimal CalculateFinalPrice(decimal price, int quantity)
    {
        var subtotal = price * quantity;
        var tax = subtotal * TaxRate;
        return subtotal + tax;
    }
}

// ✅ GOOD: External configuration
internal class AppConfiguration
{
    public static int Timeout => 30;
    public static string ApiUrl => "https://api.example.com";
}