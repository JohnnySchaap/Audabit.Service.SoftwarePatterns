namespace Audabit.Service.SoftwarePatterns.App.Patterns._7_Reliability;

/// <summary>
/// Service implementation for demonstrating Reliability / Cloud Patterns.
/// </summary>
public sealed class ReliabilityPatternsService : IReliabilityPatternsService
{
    public async Task CircuitBreakerAsync()
    {
        Console.WriteLine("\n=== Circuit Breaker Pattern Demo ===\n");
        await Patterns.CircuitBreaker.RunAsync();
        Console.WriteLine("\n=== Circuit Breaker Pattern: Demonstration complete.===\n");
    }

    public async Task BulkheadAsync()
    {
        Console.WriteLine("\n=== Bulkhead Pattern Demo ===\n");
        await Patterns.Bulkhead.RunAsync();
        Console.WriteLine("\n=== Bulkhead Pattern: Demonstration complete.===\n");
    }

    public async Task RetryBackoffAsync()
    {
        Console.WriteLine("\n=== Retry Backoff Pattern Demo ===\n");
        await Patterns.RetryBackoff.RunAsync();
        Console.WriteLine("\n=== Retry Backoff Pattern: Demonstration complete.===\n");
    }

    public async Task RateLimitingAsync()
    {
        Console.WriteLine("\n=== Rate Limiting Pattern Demo ===\n");
        await Patterns.RateLimiting.RunAsync();
        Console.WriteLine("\n=== Rate Limiting Pattern: Demonstration complete.===\n");
    }

    public void FailFast()
    {
        Console.WriteLine("\n=== Fail Fast Pattern Demo ===\n");
        Patterns.FailFast.Run();
        Console.WriteLine("\n=== Fail Fast Pattern: Demonstration complete.===\n");
    }
}