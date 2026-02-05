namespace Audabit.Service.SoftwarePatterns.App.Patterns._8_AntiPatterns.Patterns;

// SYSLIB1045 suppression: Using Regex directly (not GeneratedRegex) for demo simplicity
#pragma warning disable SYSLIB1045 // Use GeneratedRegexAttribute

// Golden Hammer Anti-Pattern: Using the same solution for every problem ("When all you have is a hammer, everything looks like a nail").
// This demonstrates over-applying a favored technology/pattern regardless of fit.
//
// PROBLEMS:
// - Using one tool/pattern for all scenarios
// - Ignoring context and requirements
// - Complexity where simplicity is needed
// - Performance issues from wrong tool
//
// SOLUTION:
// - Choose the right tool for the job
// - Consider trade-offs and context
// - Use simple solutions for simple problems
// - Match technology to requirements
public static partial class GoldenHammer
{
    public static void Run()
    {
        Console.WriteLine("Golden Hammer Anti-Pattern: Demonstrating inappropriate tool selection...");
        Console.WriteLine("Golden Hammer Anti-Pattern: Right tool for the right job.\n");

        Console.WriteLine("Golden Hammer Anti-Pattern: --- BAD EXAMPLE: Using Regex for Everything ---\n");
        UseRegexForEverythingBad();

        Console.WriteLine("\nGolden Hammer Anti-Pattern: --- GOOD EXAMPLE: Right Tool for Each Task ---\n");
        UseAppropriateToolsGood();

        Console.WriteLine("\nGolden Hammer Anti-Pattern: --- BAD EXAMPLE: Microservices for Everything ---\n");
        UseMicroservicesForEverythingBad();

        Console.WriteLine("\nGolden Hammer Anti-Pattern: --- GOOD EXAMPLE: Match Architecture to Needs ---\n");
        UseAppropriateArchitectureGood();

        Console.WriteLine("\nGolden Hammer Anti-Pattern: Key Lessons:");
        Console.WriteLine("Golden Hammer Anti-Pattern: ✓ Evaluate context before choosing solution");
        Console.WriteLine("Golden Hammer Anti-Pattern: ✓ Simple problems deserve simple solutions");
        Console.WriteLine("Golden Hammer Anti-Pattern: ✓ Complex patterns are not always better");
        Console.WriteLine("Golden Hammer Anti-Pattern: ✓ Consider maintenance and performance trade-offs");
    }

    // ❌ BAD: Using regex for everything (the golden hammer)
    private static void UseRegexForEverythingBad()
    {
        Console.WriteLine("Golden Hammer Anti-Pattern: [BAD] Developer loves regex, uses it for everything:\n");

        // Simple string check - regex is overkill
        var email = "user@example.com";
        var containsAt = MyRegex().IsMatch(email);
        Console.WriteLine($"Golden Hammer Anti-Pattern: Email contains @: {containsAt} (used regex for simple string.Contains!)");

        // Simple number parsing - regex is wrong tool
        var ageStr = "25";
        var ageMatch = MyRegex1().Match(ageStr);
        Console.WriteLine($"Golden Hammer Anti-Pattern: Age: {ageMatch.Value} (used regex instead of int.TryParse!)");

        // String replacement - regex is excessive
        var text = "Hello World";
        var replaced = System.Text.RegularExpressions.Regex.Replace(text, "World", "Universe");
        Console.WriteLine($"Golden Hammer Anti-Pattern: Replaced: {replaced} (used regex instead of string.Replace!)");

        Console.WriteLine("\nGolden Hammer Anti-Pattern: Problems: Slower, harder to read, maintenance nightmare");
    }

    // ✅ GOOD: Use appropriate tools for each task
    private static void UseAppropriateToolsGood()
    {
        Console.WriteLine("Golden Hammer Anti-Pattern: [GOOD] Choosing the right tool for each task:\n");

        // Simple string check - use built-in method
        var email = "user@example.com";
        var containsAt = email.Contains('@');
        Console.WriteLine($"Golden Hammer Anti-Pattern: Email contains @: {containsAt} (string.Contains - simple & clear)");

        // Number parsing - use proper parser
        var ageStr = "25";
        var age = int.TryParse(ageStr, out var ageValue) ? ageValue : 0;
        Console.WriteLine($"Golden Hammer Anti-Pattern: Age: {age} (int.TryParse - type-safe)");

        // String replacement - use string method
        var text = "Hello World";
        var replaced = text.Replace("World", "Universe");
        Console.WriteLine($"Golden Hammer Anti-Pattern: Replaced: {replaced} (string.Replace - fast & readable)");

        // Complex pattern - NOW use regex
        var phonePattern = @"^\d{3}-\d{3}-\d{4}$";
        var phone = "555-123-4567";
        var isValidPhone = System.Text.RegularExpressions.Regex.IsMatch(phone, phonePattern);
        Console.WriteLine($"Golden Hammer Anti-Pattern: Valid phone: {isValidPhone} (regex - RIGHT tool for pattern matching)");

        Console.WriteLine("\nGolden Hammer Anti-Pattern: Benefits: Faster, clearer intent, easier maintenance");
    }

    // ❌ BAD: Microservices for everything (golden hammer)
    private static void UseMicroservicesForEverythingBad()
    {
        Console.WriteLine("Golden Hammer Anti-Pattern: [BAD] Team heard microservices are great, uses them everywhere:\n");

        Console.WriteLine("Golden Hammer Anti-Pattern: Simple CRUD app split into 15 microservices:");
        Console.WriteLine("Golden Hammer Anti-Pattern: - User Service");
        Console.WriteLine("Golden Hammer Anti-Pattern: - Authentication Service");
        Console.WriteLine("Golden Hammer Anti-Pattern: - Validation Service");
        Console.WriteLine("Golden Hammer Anti-Pattern: - Email Service");
        Console.WriteLine("Golden Hammer Anti-Pattern: - Logger Service");
        Console.WriteLine("Golden Hammer Anti-Pattern: - ... (10 more services)");
        Console.WriteLine();
        Console.WriteLine("Golden Hammer Anti-Pattern: Result: Distributed monolith, 10x complexity, harder to debug");
        Console.WriteLine("Golden Hammer Anti-Pattern: Simple page load requires 8 service calls");
    }

    // ✅ GOOD: Match architecture to actual needs
    private static void UseAppropriateArchitectureGood()
    {
        Console.WriteLine("Golden Hammer Anti-Pattern: [GOOD] Architecture matches requirements:\n");

        Console.WriteLine("Golden Hammer Anti-Pattern: Simple CRUD app → Modular Monolith:");
        Console.WriteLine("Golden Hammer Anti-Pattern: - Single deployment");
        Console.WriteLine("Golden Hammer Anti-Pattern: - Clear module boundaries");
        Console.WriteLine("Golden Hammer Anti-Pattern: - Easy to develop and debug");
        Console.WriteLine("Golden Hammer Anti-Pattern: - Can extract microservices LATER if needed");
        Console.WriteLine();

        Console.WriteLine("Golden Hammer Anti-Pattern: Large e-commerce platform → Microservices:");
        Console.WriteLine("Golden Hammer Anti-Pattern: - Catalog Service (independent scaling)");
        Console.WriteLine("Golden Hammer Anti-Pattern: - Order Service (high availability)");
        Console.WriteLine("Golden Hammer Anti-Pattern: - Payment Service (isolated security)");
        Console.WriteLine("Golden Hammer Anti-Pattern: - Inventory Service (real-time updates)");
        Console.WriteLine();
        Console.WriteLine("Golden Hammer Anti-Pattern: Result: Right complexity for the right scale");
    }

    [System.Text.RegularExpressions.GeneratedRegex("@")]
    private static partial System.Text.RegularExpressions.Regex MyRegex();
    [System.Text.RegularExpressions.GeneratedRegex(@"\d+")]
    private static partial System.Text.RegularExpressions.Regex MyRegex1();
}