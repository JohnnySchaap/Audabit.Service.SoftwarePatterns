namespace Audabit.Service.SoftwarePatterns.App.Patterns._8_AntiPatterns.Patterns;

// Reinventing the Wheel Anti-Pattern: Building custom solutions for problems already solved by standard libraries.
// This demonstrates wasting time reimplementing existing functionality.
//
// PROBLEMS:
// - Time wasted on solved problems
// - Bugs in custom implementations
// - Maintenance burden
// - Missing edge cases
// - Not leveraging battle-tested code
//
// SOLUTION:
// - Research existing libraries first
// - Use proven, maintained solutions
// - Focus on unique business value
// - Only build custom when truly needed
public static class ReinventingTheWheel
{
    public static void Run()
    {
        Console.WriteLine("Reinventing the Wheel Anti-Pattern: Demonstrating unnecessary custom implementations...");
        Console.WriteLine("Reinventing the Wheel Anti-Pattern: Use libraries, focus on business value.\n");

        Console.WriteLine("Reinventing the Wheel Anti-Pattern: --- BAD EXAMPLE: Custom JSON Parser ---\n");
        UseCustomJsonParserBad();

        Console.WriteLine("\nReinventing the Wheel Anti-Pattern: --- GOOD EXAMPLE: Use System.Text.Json ---\n");
        UseSystemTextJsonGood();

        Console.WriteLine("\nReinventing the Wheel Anti-Pattern: --- BAD EXAMPLE: Custom HTTP Client ---\n");
        UseCustomHttpClientBad();

        Console.WriteLine("\nReinventing the Wheel Anti-Pattern: --- GOOD EXAMPLE: Use HttpClient ---\n");
        UseHttpClientGood();

        Console.WriteLine("\nReinventing the Wheel Anti-Pattern: Key Lessons:");
        Console.WriteLine("Reinventing the Wheel Anti-Pattern: ✓ Research before building");
        Console.WriteLine("Reinventing the Wheel Anti-Pattern: ✓ Leverage mature, tested libraries");
        Console.WriteLine("Reinventing the Wheel Anti-Pattern: ✓ Focus time on unique business problems");
        Console.WriteLine("Reinventing the Wheel Anti-Pattern: ✓ Custom code is a liability (maintenance cost)");
    }

    // ❌ BAD: Custom JSON parser (hundreds of hours, still buggy)
    private static void UseCustomJsonParserBad()
    {
        Console.WriteLine("Reinventing the Wheel Anti-Pattern: [BAD] Team spent 3 months building custom JSON parser:");
        Console.WriteLine("Reinventing the Wheel Anti-Pattern: - 2000 lines of parsing code");
        Console.WriteLine("Reinventing the Wheel Anti-Pattern: - Handles basic cases");
        Console.WriteLine("Reinventing the Wheel Anti-Pattern: - Bugs with escaped characters");
        Console.WriteLine("Reinventing the Wheel Anti-Pattern: - Bugs with unicode");
        Console.WriteLine("Reinventing the Wheel Anti-Pattern: - Bugs with nested arrays");
        Console.WriteLine("Reinventing the Wheel Anti-Pattern: - Slow performance");
        Console.WriteLine("Reinventing the Wheel Anti-Pattern: - No one wants to maintain it");
        Console.WriteLine("Reinventing the Wheel Anti-Pattern: Custom parser result: Probably broken for edge cases");
    }

    // ✅ GOOD: Use battle-tested library
    private static void UseSystemTextJsonGood()
    {
        Console.WriteLine("Reinventing the Wheel Anti-Pattern: [GOOD] Using System.Text.Json:");
        Console.WriteLine("Reinventing the Wheel Anti-Pattern: - 1 line of code");
        Console.WriteLine("Reinventing the Wheel Anti-Pattern: - Handles all edge cases");
        Console.WriteLine("Reinventing the Wheel Anti-Pattern: - Optimized performance");
        Console.WriteLine("Reinventing the Wheel Anti-Pattern: - Microsoft maintains it");
        Console.WriteLine("Reinventing the Wheel Anti-Pattern: - Team can focus on business logic");

        var json = "{\"name\":\"John\",\"age\":30}";
        var person = System.Text.Json.JsonSerializer.Deserialize<Person>(json);
        Console.WriteLine($"Reinventing the Wheel Anti-Pattern: Parsed: Name={person?.Name}, Age={person?.Age}");
        Console.WriteLine("Reinventing the Wheel Anti-Pattern: Works perfectly, no maintenance burden!");
    }

    // ❌ BAD: Custom HTTP client implementation
    private static void UseCustomHttpClientBad()
    {
        Console.WriteLine("Reinventing the Wheel Anti-Pattern: [BAD] Building custom HTTP client from scratch:");
        Console.WriteLine("Reinventing the Wheel Anti-Pattern: - Socket programming (error-prone)");
        Console.WriteLine("Reinventing the Wheel Anti-Pattern: - Custom connection pooling (buggy)");
        Console.WriteLine("Reinventing the Wheel Anti-Pattern: - Manual timeout handling (incomplete)");
        Console.WriteLine("Reinventing the Wheel Anti-Pattern: - No built-in retry logic");
        Console.WriteLine("Reinventing the Wheel Anti-Pattern: - Missing compression support");
        Console.WriteLine("Reinventing the Wheel Anti-Pattern: - Security vulnerabilities");
        Console.WriteLine("Reinventing the Wheel Anti-Pattern: Result: Months of work, still worse than HttpClient");
    }

    // ✅ GOOD: Use HttpClient with modern patterns
    private static void UseHttpClientGood()
    {
        Console.WriteLine("Reinventing the Wheel Anti-Pattern: [GOOD] Using HttpClient with HttpClientFactory:");
        Console.WriteLine("Reinventing the Wheel Anti-Pattern: - Production-ready out of the box");
        Console.WriteLine("Reinventing the Wheel Anti-Pattern: - Connection pooling built-in");
        Console.WriteLine("Reinventing the Wheel Anti-Pattern: - Timeout handling");
        Console.WriteLine("Reinventing the Wheel Anti-Pattern: - Easy to add retry policies (Polly)");
        Console.WriteLine("Reinventing the Wheel Anti-Pattern: - Compression support");
        Console.WriteLine("Reinventing the Wheel Anti-Pattern: - Security best practices");
        Console.WriteLine("Reinventing the Wheel Anti-Pattern: Result: Focus on API integration, not HTTP internals");

        // In real code: services.AddHttpClient()
        Console.WriteLine("Reinventing the Wheel Anti-Pattern: Example: var response = await httpClient.GetAsync(url);");
    }

    private class Person
    {
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
    }
}