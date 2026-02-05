namespace Audabit.Service.SoftwarePatterns.App.Principles._0_Principles.Principles;

// KISS Principle: Keep It Simple, Stupid
// Simplicity should be a key goal in design, and unnecessary complexity avoided

// MODERN C# USAGE:
// - Use built-in framework methods instead of custom implementations
// - LINQ for simple data transformations
// - Pattern matching for simple conditional logic
// - Avoid premature optimization
// - Favor readability over cleverness
public static class KISS
{
    public static void Run()
    {
        Console.WriteLine("KISS Principle: Keep It Simple, Stupid demonstration...");
        Console.WriteLine("KISS Principle: Simplicity should be a key goal in design, avoiding unnecessary complexity.\n");

        // Example 1: Simple vs Complex string checking
        Console.WriteLine("KISS Principle: --- Example 1: String Validation ---");

        var input = "Hello World";

        // ❌ COMPLEX: Over-engineered validation
        Console.WriteLine("KISS Principle: ❌ COMPLEX: Custom regex validator with 5 validation rules");
        Console.WriteLine("KISS Principle: ❌ COMPLEX: 20+ lines of code for simple check\n");

        // ✅ SIMPLE: Direct, clear validation
        var isValid = !string.IsNullOrWhiteSpace(input) && input.Length <= 100;
        Console.WriteLine($"KISS Principle: ✅ SIMPLE: Is valid: {isValid}");
        Console.WriteLine("KISS Principle: ✅ SIMPLE: One line, easy to understand\n");

        // Example 2: Finding maximum value
        Console.WriteLine("KISS Principle: --- Example 2: Finding Maximum ---");

        var numbers = new[] { 5, 12, 3, 9, 15, 7 };

        // ❌ COMPLEX: Manual implementation
        Console.WriteLine("KISS Principle: ❌ COMPLEX: Custom loop with temporary variables");
        var max = numbers[0];
        for (var i = 1; i < numbers.Length; i++)
        {
            if (numbers[i] > max)
            {
                max = numbers[i];
            }
        }
        Console.WriteLine($"KISS Principle: ❌ COMPLEX: Max value: {max}\n");

        // ✅ SIMPLE: Built-in method
        var simpleMax = numbers.Max();
        Console.WriteLine($"KISS Principle: ✅ SIMPLE: Max value: {simpleMax}");
        Console.WriteLine("KISS Principle: ✅ SIMPLE: One line using LINQ\n");

        // Example 3: Filtering data
        Console.WriteLine("KISS Principle: --- Example 3: Filtering Collections ---");

        var users = new[]
        {
            new { Name = "Alice", Age = 25, IsActive = true },
            new { Name = "Bob", Age = 17, IsActive = false },
            new { Name = "Charlie", Age = 30, IsActive = true }
        };

        // ❌ COMPLEX: Manual filtering with loops
        Console.WriteLine("KISS Principle: ❌ COMPLEX: Nested loops, temporary lists, manual filtering");
        var filteredUsersComplex = new List<string>();
        for (var i = 0; i < users.Length; i++)
        {
            if (users[i].IsActive)
            {
                if (users[i].Age >= 18)
                {
                    filteredUsersComplex.Add(users[i].Name);
                }
            }
        }
        Console.WriteLine($"KISS Principle: ❌ COMPLEX: Found {filteredUsersComplex.Count} users\n");

        // ✅ SIMPLE: LINQ query
        var filteredUsersSimple = users
            .Where(u => u.IsActive && u.Age >= 18)
            .Select(u => u.Name)
            .ToList();

        Console.WriteLine($"KISS Principle: ✅ SIMPLE: Found {filteredUsersSimple.Count} users: {string.Join(", ", filteredUsersSimple)}");
        Console.WriteLine("KISS Principle: ✅ SIMPLE: Clear, declarative LINQ query\n");

        // Example 4: Conditional logic
        Console.WriteLine("KISS Principle: --- Example 4: Conditional Logic ---");

        var status = "active";

        // ❌ COMPLEX: Nested if-else chains
        Console.WriteLine("KISS Principle: ❌ COMPLEX: Deeply nested if-else statements");
        string messageComplex;
        if (status == "active")
        {
            messageComplex = "User is active";
        }
        else if (status == "inactive")
        {
            messageComplex = "User is inactive";
        }
        else if (status == "pending")
        {
            messageComplex = "User is pending";
        }
        else
        {
            messageComplex = "Unknown status";
        }
        Console.WriteLine($"KISS Principle: ❌ COMPLEX: {messageComplex}\n");

        // ✅ SIMPLE: Pattern matching or dictionary
        var messageSimple = status switch
        {
            "active" => "User is active",
            "inactive" => "User is inactive",
            "pending" => "User is pending",
            _ => "Unknown status"
        };

        Console.WriteLine($"KISS Principle: ✅ SIMPLE: {messageSimple}");
        Console.WriteLine("KISS Principle: ✅ SIMPLE: Clean switch expression\n");

        // Example 5: Calculation simplification
        Console.WriteLine("KISS Principle: --- Example 5: Calculations ---");

        var price = 100m;
        var quantity = 3;

        // ❌ COMPLEX: Over-abstracted calculation engine
        Console.WriteLine("KISS Principle: ❌ COMPLEX: Custom CalculationEngine with Strategy pattern");
        Console.WriteLine("KISS Principle: ❌ COMPLEX: Multiple classes for simple multiplication\n");

        // ✅ SIMPLE: Direct calculation
        var total = price * quantity;
        Console.WriteLine($"KISS Principle: ✅ SIMPLE: Total: ${total}");
        Console.WriteLine("KISS Principle: ✅ SIMPLE: Straightforward arithmetic\n");

        Console.WriteLine("KISS Principle: Key Takeaway: Use built-in features, avoid over-engineering!");
    }
}