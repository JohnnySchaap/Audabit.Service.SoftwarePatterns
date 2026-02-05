namespace Audabit.Service.SoftwarePatterns.App.Patterns._7_Reliability.Patterns;

// Fail Fast Pattern: Validate inputs immediately and fail fast rather than processing invalid data.
// In this example:
// - We validate all inputs at system boundaries (API endpoints, message handlers).
// - Invalid inputs are rejected immediately with clear error messages.
// - Prevents wasted processing, resource consumption, and potential data corruption.
// - Makes debugging easier by failing close to the source of the problem.
//
// MODERN C# USAGE:
// - Use FluentValidation for complex validation rules
// - Validate in middleware/filters before controller actions
// - Use guard clauses at method entry points
// - Fail during startup if configuration is invalid (don't wait for runtime)
public static class FailFast
{
    public static void Run()
    {
        Console.WriteLine("Fail Fast Pattern: Demonstrating immediate validation and failure...");
        Console.WriteLine("Fail Fast Pattern: Detecting errors early to prevent cascading failures.\n");

        Console.WriteLine("Fail Fast Pattern: --- Valid Request ---\n");
        ProcessOrder(new OrderRequest
        {
            OrderId = "ORD-12345",
            CustomerId = "CUST-789",
            Items = ["Item1", "Item2"],
            TotalAmount = 99.99m
        });

        Console.WriteLine("\nFail Fast Pattern: --- Invalid Requests (Fail Fast) ---\n");

        // Missing OrderId
        ProcessOrder(new OrderRequest
        {
            OrderId = "",
            CustomerId = "CUST-789",
            Items = ["Item1"],
            TotalAmount = 50.00m
        });

        // No items
        ProcessOrder(new OrderRequest
        {
            OrderId = "ORD-99999",
            CustomerId = "CUST-789",
            Items = [],
            TotalAmount = 0
        });

        // Negative amount
        ProcessOrder(new OrderRequest
        {
            OrderId = "ORD-88888",
            CustomerId = "CUST-789",
            Items = ["Item1"],
            TotalAmount = -10.00m
        });

        Console.WriteLine("\nFail Fast Pattern: All invalid requests rejected immediately.");
        Console.WriteLine("Fail Fast Pattern: No resources wasted on invalid data.");
    }

    private static void ProcessOrder(OrderRequest request)
    {
        try
        {
            // FAIL FAST: Validate immediately at entry point
            ValidateOrderRequest(request);

            Console.WriteLine($"Fail Fast Pattern: Order {request.OrderId} - Validation passed");
            Console.WriteLine($"Fail Fast Pattern: Order {request.OrderId} - Processing order...");

            // Simulate expensive processing
            Thread.Sleep(100);

            Console.WriteLine($"Fail Fast Pattern: Order {request.OrderId} - Successfully processed");
        }
        catch (ValidationException ex)
        {
            Console.WriteLine($"Fail Fast Pattern: REJECTED - {ex.Message}");
            Console.WriteLine($"Fail Fast Pattern: Failed fast without wasting resources");
        }
    }

    private static void ValidateOrderRequest(OrderRequest request)
    {
        // Guard clauses - fail fast on invalid input
        if (string.IsNullOrWhiteSpace(request.OrderId))
        {
            throw new ValidationException("OrderId is required");
        }

        if (string.IsNullOrWhiteSpace(request.CustomerId))
        {
            throw new ValidationException("CustomerId is required");
        }

        if (request.Items == null || request.Items.Count == 0)
        {
            throw new ValidationException("Order must contain at least one item");
        }

        if (request.TotalAmount <= 0)
        {
            throw new ValidationException("TotalAmount must be greater than zero");
        }

        // Could add more sophisticated validation:
        // - OrderId format validation
        // - Customer exists check
        // - Inventory availability
        // - Credit limit check
        // All checked BEFORE starting expensive processing
    }
}

public class OrderRequest
{
    public string OrderId { get; init; } = string.Empty;
    public string CustomerId { get; init; } = string.Empty;
    public List<string> Items { get; init; } = [];
    public decimal TotalAmount { get; init; }
}

public class ValidationException(string message) : Exception(message)
{
}