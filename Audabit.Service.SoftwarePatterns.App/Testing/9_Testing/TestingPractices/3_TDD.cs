namespace Audabit.Service.SoftwarePatterns.App.Testing._9_Testing.TestingPractices;

// IDE0059 suppression: Intentionally showing TDD evolution with "unused" intermediate assignments
#pragma warning disable IDE0059 // Unnecessary assignment of a value

// TDD (Test-Driven Development) Pattern: Write tests before implementation using Red-Green-Refactor cycle.
// In this example:
// - RED: Write a failing test for desired functionality
// - GREEN: Write minimal code to make test pass
// - REFACTOR: Improve code while keeping tests green
// - Repeat cycle for each feature
//
// MODERN C# USAGE:
// - Start with test describing desired behavior
// - Run test (it fails - red)
// - Implement simplest solution to pass test (green)
// - Refactor for quality (tests stay green)
// - Provides regression safety and design feedback
public static class TDD
{
    public static void Run()
    {
        Console.WriteLine("TDD Methodology: Demonstrating Test-Driven Development cycle...");
        Console.WriteLine("TDD Methodology: Red → Green → Refactor.\n");

        Console.WriteLine("TDD Methodology: === Feature: Calculate Order Total with Tax ===\n");

        Console.WriteLine("TDD Methodology: --- Cycle 1: Basic Total Calculation ---\n");
        Console.WriteLine("TDD Methodology: [RED] Write test: CalculateTotal should sum item prices");
        Console.WriteLine("TDD Methodology: [RED] Run test → FAIL (method doesn't exist yet)");
        Console.WriteLine("TDD Methodology: [GREEN] Implement: return items.Sum(i => i.Price)");
        Console.WriteLine("TDD Methodology: [GREEN] Run test → PASS ✓");
        Console.WriteLine();

        var calculator = new OrderCalculatorV1();
        var testItems = new List<OrderItem>
        {
            new() { Name = "Item1", Price = 10.00m },
            new() { Name = "Item2", Price = 20.00m }
        };
        var total = OrderCalculatorV1.CalculateTotal(testItems);
        Console.WriteLine($"TDD Methodology: Test assertion: total == 30.00 → {total == 30.00m} ✓\n");

        Console.WriteLine("TDD Methodology: --- Cycle 2: Add Tax Calculation ---\n");
        Console.WriteLine("TDD Methodology: [RED] Write test: CalculateTotal should include 10% tax");
        Console.WriteLine("TDD Methodology: [RED] Run test → FAIL (tax not implemented)");
        Console.WriteLine("TDD Methodology: [GREEN] Implement: return subtotal * 1.10m");
        Console.WriteLine("TDD Methodology: [GREEN] Run test → PASS ✓");
        Console.WriteLine();

        var calculatorV2 = new OrderCalculatorV2();
        var totalWithTax = OrderCalculatorV2.CalculateTotal(testItems);
        Console.WriteLine($"TDD Methodology: Test assertion: total == 33.00 → {totalWithTax == 33.00m} ✓\n");

        Console.WriteLine("TDD Methodology: --- Cycle 3: Refactor for Readability ---\n");
        Console.WriteLine("TDD Methodology: [REFACTOR] Extract tax rate to constant");
        Console.WriteLine("TDD Methodology: [REFACTOR] Extract subtotal calculation to method");
        Console.WriteLine("TDD Methodology: [REFACTOR] Run all tests → PASS ✓ (no behavior change)");
        Console.WriteLine();

        var calculatorV3 = new OrderCalculatorV3();
        var totalRefactored = OrderCalculatorV3.CalculateTotal(testItems);
        Console.WriteLine($"TDD Methodology: Test still passes: total == 33.00 → {totalRefactored == 33.00m} ✓\n");

        Console.WriteLine("TDD Methodology: Benefits:");
        Console.WriteLine("TDD Methodology: ✓ Tests drive design (YAGNI - only implement what's tested)");
        Console.WriteLine("TDD Methodology: ✓ Full test coverage (every line written to pass a test)");
        Console.WriteLine("TDD Methodology: ✓ Regression safety (refactor with confidence)");
        Console.WriteLine("TDD Methodology: ✓ Better API design (test-first reveals usability issues)");
    }
}

// Version 1: Minimal implementation to pass first test
public class OrderCalculatorV1
{
    public static decimal CalculateTotal(List<OrderItem> items)
    {
        return items.Sum(i => i.Price);
    }
}

// Version 2: Add tax to pass second test
public class OrderCalculatorV2
{
    public static decimal CalculateTotal(List<OrderItem> items)
    {
        var subtotal = items.Sum(i => i.Price);
        return subtotal * 1.10m; // 10% tax
    }
}

// Version 3: Refactored for readability (tests still pass)
public class OrderCalculatorV3
{
    private const decimal TaxRate = 0.10m;

    public static decimal CalculateTotal(List<OrderItem> items)
    {
        var subtotal = CalculateSubtotal(items);
        return subtotal + (subtotal * TaxRate);
    }

    private static decimal CalculateSubtotal(List<OrderItem> items)
    {
        return items.Sum(i => i.Price);
    }
}

public class OrderItem
{
    public string Name { get; init; } = string.Empty;
    public decimal Price { get; init; }
}