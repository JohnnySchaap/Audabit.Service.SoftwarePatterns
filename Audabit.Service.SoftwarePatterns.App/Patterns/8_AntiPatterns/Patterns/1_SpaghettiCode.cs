namespace Audabit.Service.SoftwarePatterns.App.Patterns._8_AntiPatterns.Patterns;

// Spaghetti Code Anti-Pattern: Tangled control flow with no clear structure, making code impossible to understand.
// This demonstrates the problem and the refactored solution.
//
// PROBLEMS:
// - No clear separation of concerns
// - Deeply nested conditions
// - Mixed responsibilities
// - Hard to test
// - Impossible to maintain
//
// SOLUTION:
// - Extract methods with single responsibilities
// - Use early returns to reduce nesting
// - Separate validation, business logic, and side effects
// - Clear, linear flow
public static class SpaghettiCode
{
    public static void Run()
    {
        Console.WriteLine("Spaghetti Code Anti-Pattern: Demonstrating tangled vs clean code...");
        Console.WriteLine("Spaghetti Code Anti-Pattern: Before and after refactoring.\n");

        Console.WriteLine("Spaghetti Code Anti-Pattern: --- BAD EXAMPLE: Spaghetti Code ---\n");
        ProcessOrderBad("ORD-123", "CUST-456", 150.00m);

        Console.WriteLine("\nSpaghetti Code Anti-Pattern: --- GOOD EXAMPLE: Refactored Code ---\n");
        ProcessOrderGood("ORD-123", "CUST-456", 150.00m);

        Console.WriteLine("\nSpaghetti Code Anti-Pattern: Key Improvements:");
        Console.WriteLine("Spaghetti Code Anti-Pattern: ✓ Single Responsibility: Each method does one thing");
        Console.WriteLine("Spaghetti Code Anti-Pattern: ✓ Early Returns: Reduced nesting from 5 levels to 1");
        Console.WriteLine("Spaghetti Code Anti-Pattern: ✓ Testability: Can test validation, calculation, notification separately");
        Console.WriteLine("Spaghetti Code Anti-Pattern: ✓ Readability: Clear linear flow, easy to understand");
    }

    // ❌ BAD: Spaghetti code with deep nesting and mixed concerns
    private static void ProcessOrderBad(string orderId, string customerId, decimal amount)
    {
        Console.WriteLine("Spaghetti Code Anti-Pattern: Processing order (BAD version)...");

        if (!string.IsNullOrEmpty(orderId))
        {
            if (!string.IsNullOrEmpty(customerId))
            {
                if (amount > 0)
                {
                    // Validation mixed with business logic
                    var customer = GetCustomer(customerId);
                    if (customer != null)
                    {
                        if (customer.IsActive)
                        {
                            // Calculation mixed with side effects
                            var tax = amount * 0.1m;
                            var total = amount + tax;
                            if (total <= customer.CreditLimit)
                            {
                                // Save order
                                Console.WriteLine($"Spaghetti Code Anti-Pattern: Order saved: {orderId}, Total: ${total}");
                                // Send email mixed in
                                Console.WriteLine($"Spaghetti Code Anti-Pattern: Email sent to customer {customerId}");
                                // Update inventory mixed in
                                Console.WriteLine("Spaghetti Code Anti-Pattern: Inventory updated");
                            }
                            else
                            {
                                Console.WriteLine("Spaghetti Code Anti-Pattern: ERROR: Credit limit exceeded");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Spaghetti Code Anti-Pattern: ERROR: Customer not active");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Spaghetti Code Anti-Pattern: ERROR: Customer not found");
                    }
                }
                else
                {
                    Console.WriteLine("Spaghetti Code Anti-Pattern: ERROR: Invalid amount");
                }
            }
            else
            {
                Console.WriteLine("Spaghetti Code Anti-Pattern: ERROR: Customer ID required");
            }
        }
        else
        {
            Console.WriteLine("Spaghetti Code Anti-Pattern: ERROR: Order ID required");
        }
    }

    // ✅ GOOD: Clean, structured code with clear responsibilities
    private static void ProcessOrderGood(string orderId, string customerId, decimal amount)
    {
        Console.WriteLine("Spaghetti Code Anti-Pattern: Processing order (GOOD version)...");

        // Step 1: Validate inputs (early returns)
        if (!ValidateOrderInput(orderId, customerId, amount))
            return;

        // Step 2: Get customer and validate
        var customer = GetCustomer(customerId);
        if (!ValidateCustomer(customer))
            return;

        // Step 3: Calculate totals
        var total = CalculateOrderTotal(amount);

        // Step 4: Validate credit limit
        if (!ValidateCreditLimit(customer!, total))
            return;

        // Step 5: Process order (single responsibility)
        SaveOrder(orderId, total);
        NotifyCustomer(customerId);
        UpdateInventory();

        Console.WriteLine("Spaghetti Code Anti-Pattern: Order processed successfully");
    }

    private static bool ValidateOrderInput(string orderId, string customerId, decimal amount)
    {
        if (string.IsNullOrEmpty(orderId))
        {
            Console.WriteLine("Spaghetti Code Anti-Pattern: ERROR: Order ID required");
            return false;
        }

        if (string.IsNullOrEmpty(customerId))
        {
            Console.WriteLine("Spaghetti Code Anti-Pattern: ERROR: Customer ID required");
            return false;
        }

        if (amount <= 0)
        {
            Console.WriteLine("Spaghetti Code Anti-Pattern: ERROR: Invalid amount");
            return false;
        }

        return true;
    }

    private static bool ValidateCustomer(Customer? customer)
    {
        if (customer == null)
        {
            Console.WriteLine("Spaghetti Code Anti-Pattern: ERROR: Customer not found");
            return false;
        }

        if (!customer.IsActive)
        {
            Console.WriteLine("Spaghetti Code Anti-Pattern: ERROR: Customer not active");
            return false;
        }

        return true;
    }

    private static decimal CalculateOrderTotal(decimal amount)
    {
        const decimal taxRate = 0.1m;
        return amount + (amount * taxRate);
    }

    private static bool ValidateCreditLimit(Customer customer, decimal total)
    {
        if (total > customer.CreditLimit)
        {
            Console.WriteLine("Spaghetti Code Anti-Pattern: ERROR: Credit limit exceeded");
            return false;
        }
        return true;
    }

    private static void SaveOrder(string orderId, decimal total)
    {
        Console.WriteLine($"Spaghetti Code Anti-Pattern: Order saved: {orderId}, Total: ${total}");
    }

    private static void NotifyCustomer(string customerId)
    {
        Console.WriteLine($"Spaghetti Code Anti-Pattern: Email sent to customer {customerId}");
    }

    private static void UpdateInventory()
    {
        Console.WriteLine("Spaghetti Code Anti-Pattern: Inventory updated");
    }

    private static Customer? GetCustomer(string customerId)
    {
        return new Customer { Id = customerId, IsActive = true, CreditLimit = 1000m };
    }

    private class Customer
    {
        public string Id { get; init; } = string.Empty;
        public bool IsActive { get; init; }
        public decimal CreditLimit { get; init; }
    }
}