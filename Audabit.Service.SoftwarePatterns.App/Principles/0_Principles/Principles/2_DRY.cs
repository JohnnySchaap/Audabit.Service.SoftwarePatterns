namespace Audabit.Service.SoftwarePatterns.App.Principles._0_Principles.Principles;

#pragma warning disable IDE0059 // Unnecessary assignment - intentional for demonstration

// DRY Principle: Don't Repeat Yourself
// Every piece of knowledge must have a single, unambiguous, authoritative representation

// MODERN C# USAGE:
// - Extract methods, classes, or helper utilities
// - Use generics to avoid type-specific duplication
// - Use LINQ to avoid loop duplication
// - Use extension methods for reusable functionality
// - Use base classes or composition for shared behavior
public static class DRY
{
    public static void Run()
    {
        Console.WriteLine("DRY Principle: Don't Repeat Yourself demonstration...");
        Console.WriteLine("DRY Principle: Every piece of knowledge should have a single, authoritative representation.\n");

        // ❌ BAD: Repeated validation logic
        Console.WriteLine("DRY Principle: --- Before DRY (Code Duplication) ---");
        var user1 = new { Email = "user@example.com", Age = 25 };
        var user2 = new { Email = "invalid-email", Age = 15 };

        // Validation logic repeated in multiple places
        if (string.IsNullOrWhiteSpace(user1.Email) || !user1.Email.Contains('@'))
        {
            Console.WriteLine("DRY Principle: ❌ BAD: User1 email invalid");
        }
        if (user1.Age < 18)
        {
            Console.WriteLine("DRY Principle: ❌ BAD: User1 age invalid");
        }

        if (string.IsNullOrWhiteSpace(user2.Email) || !user2.Email.Contains('@'))
        {
            Console.WriteLine("DRY Principle: ❌ BAD: User2 email invalid");
        }
        if (user2.Age < 18)
        {
            Console.WriteLine("DRY Principle: ❌ BAD: User2 age invalid");
        }

        Console.WriteLine();

        // ✅ GOOD: Extracted validation logic
        Console.WriteLine("DRY Principle: --- After DRY (Single Source of Truth) ---");
        var validator = new UserValidator();

        var validationResult1 = UserValidator.Validate(user1.Email, user1.Age);
        Console.WriteLine($"DRY Principle: ✅ GOOD: User1 validation: {validationResult1}");

        var validationResult2 = UserValidator.Validate(user2.Email, user2.Age);
        Console.WriteLine($"DRY Principle: ✅ GOOD: User2 validation: {validationResult2}\n");

        // Another example: Repeated database queries
        Console.WriteLine("DRY Principle: --- Database Query Duplication ---");

        // ❌ BAD: Query logic repeated
        Console.WriteLine("DRY Principle: ❌ BAD: GetActiveUsers query duplicated 3 times");
        Console.WriteLine("DRY Principle: ❌ BAD: GetInactiveUsers query duplicated 3 times");
        Console.WriteLine("DRY Principle: ❌ BAD: Hard to maintain, easy to introduce bugs\n");

        // ✅ GOOD: Query logic extracted
        var userRepository = new UserRepository();
        var activeUsers = userRepository.GetActiveUsers();
        var inactiveUsers = userRepository.GetInactiveUsers();

        Console.WriteLine($"DRY Principle: ✅ GOOD: Found {activeUsers.Count} active users (single query method)");
        Console.WriteLine($"DRY Principle: ✅ GOOD: Found {inactiveUsers.Count} inactive users (single query method)");
        Console.WriteLine("DRY Principle: ✅ GOOD: Changes to query logic only happen in ONE place\n");

        // Example: Configuration values
        Console.WriteLine("DRY Principle: --- Configuration Duplication ---");

        // ❌ BAD: Magic strings repeated
        Console.WriteLine("DRY Principle: ❌ BAD: API URL hardcoded in 10 different places");
        Console.WriteLine("DRY Principle: ❌ BAD: Timeout value duplicated across services\n");

        // ✅ GOOD: Centralized configuration
        Console.WriteLine($"DRY Principle: ✅ GOOD: API URL: {AppConfig.ApiUrl}");
        Console.WriteLine($"DRY Principle: ✅ GOOD: Timeout: {AppConfig.TimeoutSeconds}s");
        Console.WriteLine("DRY Principle: ✅ GOOD: Configuration in ONE place, used everywhere\n");

        // Example: Business logic duplication
        Console.WriteLine("DRY Principle: --- Business Logic Duplication ---");

        var product1 = new DryProduct { Price = 100m, Category = "Electronics" };
        var product2 = new DryProduct { Price = 50m, Category = "Books" };

        // ✅ GOOD: Price calculation extracted to helper
        var calculator = new PriceCalculator();
        var price1 = PriceCalculator.CalculateWithTax(product1);
        var price2 = PriceCalculator.CalculateWithTax(product2);

        Console.WriteLine($"DRY Principle: ✅ GOOD: Product1 final price: ${price1:F2}");
        Console.WriteLine($"DRY Principle: ✅ GOOD: Product2 final price: ${price2:F2}");
        Console.WriteLine("DRY Principle: ✅ GOOD: Tax calculation logic in ONE method, not repeated\n");
    }
}

// ✅ GOOD: Extracted validation logic
public class UserValidator
{
    public static string Validate(string email, int age)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
        {
            errors.Add("Invalid email");
        }

        if (age < 18)
        {
            errors.Add("Must be 18 or older");
        }

        return errors.Count == 0 ? "Valid" : string.Join(", ", errors);
    }
}

// ✅ GOOD: Centralized repository
public class UserRepository
{
    private readonly List<string> _users =
    [
        "user1@example.com",
        "user2@example.com",
        "inactive@example.com"
    ];

    public List<string> GetActiveUsers()
    {
        return [.. _users.Where(u => !u.Contains("inactive"))];
    }

    public List<string> GetInactiveUsers()
    {
        return [.. _users.Where(u => u.Contains("inactive"))];
    }
}

// ✅ GOOD: Centralized configuration
public static class AppConfig
{
    public const string ApiUrl = "https://api.example.com";
    public const int TimeoutSeconds = 30;
    public const string Environment = "Production";
}

// ✅ GOOD: Business logic extracted
public class DryProduct
{
    public decimal Price { get; set; }
    public string Category { get; set; } = string.Empty;
}

public class PriceCalculator
{
    private const decimal TaxRate = 0.08m; // 8% tax

    public static decimal CalculateWithTax(DryProduct DryProduct)
    {
        return DryProduct.Price * (1 + TaxRate);
    }
}