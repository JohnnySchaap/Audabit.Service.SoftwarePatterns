using System.Text.Json;

namespace Audabit.Service.SoftwarePatterns.App.Testing._9_Testing.TestingPractices;

// Golden Master Technique: Regression testing by comparing output against known good "golden" baseline.
// - CA1869 suppression: Creating JsonSerializerOptions inline is intentional for demo clarity

#pragma warning disable CA1869 // Cache and reuse JsonSerializerOptions

// Golden Master Technique: Regression testing by comparing output against known good "golden" baseline.
// In this example:
// - Capture output from known-good version (golden master)
// - After refactoring, compare new output against golden master
// - Any differences indicate potential regression
// - Useful for legacy code without tests or complex output verification
//
// MODERN C# USAGE:
// - Serialize complex objects to JSON and compare
// - Use for characterization testing of legacy code
// - Snapshot testing (Jest snapshots, ApprovalTests.NET)
// - Great for refactoring code that lacks tests
public static class GoldenMaster
{
    public static void Run()
    {
        Console.WriteLine("Golden Master Technique: Demonstrating regression testing with baselines...");
        Console.WriteLine("Golden Master Technique: Capture → Refactor → Compare.\n");

        Console.WriteLine("Golden Master Technique: --- Step 1: Capture Golden Master ---\n");
        var goldenOutput = CaptureGoldenMaster();
        Console.WriteLine($"Golden Master Technique: Golden output captured ({goldenOutput.Length} characters)");
        Console.WriteLine($"Golden Master Technique: Preview: {goldenOutput[..Math.Min(100, goldenOutput.Length)]}...\n");

        Console.WriteLine("Golden Master Technique: --- Step 2: Refactor Implementation ---\n");
        Console.WriteLine("Golden Master Technique: Refactoring report generation logic...");
        Console.WriteLine("Golden Master Technique: (Improved performance, readability, maintainability)");
        Console.WriteLine("Golden Master Technique: Refactoring complete.\n");

        Console.WriteLine("Golden Master Technique: --- Step 3: Compare Against Golden Master ---\n");
        var newOutput = CaptureOutputAfterRefactoring();

        if (goldenOutput == newOutput)
        {
            Console.WriteLine("Golden Master Technique: ✓ PASS - Output matches golden master");
            Console.WriteLine("Golden Master Technique: ✓ Refactoring is safe, no behavior changes");
        }
        else
        {
            Console.WriteLine("Golden Master Technique: ✗ FAIL - Output differs from golden master");
            Console.WriteLine("Golden Master Technique: ✗ Potential regression detected");
            Console.WriteLine($"Golden Master Technique: Expected length: {goldenOutput.Length}");
            Console.WriteLine($"Golden Master Technique: Actual length: {newOutput.Length}");
        }

        Console.WriteLine("\nGolden Master Technique: --- Demonstrating Intentional Change ---\n");
        var modifiedOutput = CaptureOutputWithIntentionalChange();

        if (goldenOutput != modifiedOutput)
        {
            Console.WriteLine("Golden Master Technique: ✗ Output differs (expected - we added a feature)");
            Console.WriteLine("Golden Master Technique: → Review differences");
            Console.WriteLine("Golden Master Technique: → If changes are correct, update golden master");
            Console.WriteLine("Golden Master Technique: → New golden master becomes the baseline");
        }

        Console.WriteLine("\nGolden Master Technique: Use Cases:");
        Console.WriteLine("Golden Master Technique: ✓ Refactoring legacy code without tests");
        Console.WriteLine("Golden Master Technique: ✓ Testing complex output (reports, transformations)");
        Console.WriteLine("Golden Master Technique: ✓ Characterization testing (document current behavior)");
        Console.WriteLine("Golden Master Technique: ✓ Snapshot testing for UI components");
    }

    private static string CaptureGoldenMaster()
    {
        // Simulate capturing output from original implementation
        var report = GenerateReport();
        return JsonSerializer.Serialize(report, new JsonSerializerOptions { WriteIndented = true });
    }

    private static string CaptureOutputAfterRefactoring()
    {
        // Same logic after refactoring (should produce identical output)
        var report = GenerateReportRefactored();
        return JsonSerializer.Serialize(report, new JsonSerializerOptions { WriteIndented = true });
    }

    private static string CaptureOutputWithIntentionalChange()
    {
        // Output with intentional new feature
        var report = GenerateReportWithNewFeature();
        return JsonSerializer.Serialize(report, new JsonSerializerOptions { WriteIndented = true });
    }

    // Original implementation (golden master baseline)
    private static SalesReport GenerateReport()
    {
        return new SalesReport
        {
            ReportDate = new DateTime(2024, 1, 15),
            TotalSales = 15000.00m,
            TotalOrders = 42,
            TopProducts =
            [
                new() { ProductName = "Widget A", UnitsSold = 120, Revenue = 6000.00m },
                new() { ProductName = "Widget B", UnitsSold = 85, Revenue = 5100.00m },
                new() { ProductName = "Widget C", UnitsSold = 50, Revenue = 3900.00m }
            ]
        };
    }

    // Refactored implementation (should match golden master)
    private static SalesReport GenerateReportRefactored()
    {
        // Refactored for better performance/readability but same output
        var products = GetTopProductSales();
        var totalRevenue = products.Sum(p => p.Revenue);

        return new SalesReport
        {
            ReportDate = new DateTime(2024, 1, 15),
            TotalSales = totalRevenue,
            TotalOrders = 42,
            TopProducts = products
        };
    }

    private static List<ProductSales> GetTopProductSales()
    {
        return
        [
            new() { ProductName = "Widget A", UnitsSold = 120, Revenue = 6000.00m },
            new() { ProductName = "Widget B", UnitsSold = 85, Revenue = 5100.00m },
            new() { ProductName = "Widget C", UnitsSold = 50, Revenue = 3900.00m }
        ];
    }

    // Modified implementation (intentional change - add average order value)
    private static SalesReport GenerateReportWithNewFeature()
    {
        var products = GetTopProductSales();
        var totalRevenue = products.Sum(p => p.Revenue);

        return new SalesReport
        {
            ReportDate = new DateTime(2024, 1, 15),
            TotalSales = totalRevenue,
            TotalOrders = 42,
            AverageOrderValue = totalRevenue / 42, // NEW FEATURE
            TopProducts = products
        };
    }
}

public class SalesReport
{
    public DateTime ReportDate { get; init; }
    public decimal TotalSales { get; init; }
    public int TotalOrders { get; init; }
    public decimal AverageOrderValue { get; init; }
    public List<ProductSales> TopProducts { get; init; } = [];
}

public class ProductSales
{
    public string ProductName { get; init; } = string.Empty;
    public int UnitsSold { get; init; }
    public decimal Revenue { get; init; }
}