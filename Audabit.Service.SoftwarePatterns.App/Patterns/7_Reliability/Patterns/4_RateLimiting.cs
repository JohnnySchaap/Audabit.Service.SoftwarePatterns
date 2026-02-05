
namespace Audabit.Service.SoftwarePatterns.App.Patterns._7_Reliability.Patterns;

// Rate Limiting Pattern: Control the rate of incoming requests to prevent system overload.
// In this example:
// - We implement Token Bucket algorithm for rate limiting.
// - Tokens are replenished at fixed rate (e.g., 10 per second).
// - Each request consumes a token; requests are rejected when bucket is empty.
// - Prevents resource exhaustion and ensures fair usage.
//
// MODERN C# USAGE:
// - Use System.Threading.RateLimiting (.NET 7+) for production rate limiting
// - Apply at API Gateway level (Azure API Management, AWS API Gateway)
// - Use sliding window for more accurate rate limiting
// - Combine with quotas for per-user/per-tenant limits
public static class RateLimiting
{
    public static async Task RunAsync()
    {
        Console.WriteLine("Rate Limiting Pattern: Demonstrating token bucket rate limiter...");
        Console.WriteLine("Rate Limiting Pattern: Preventing system overload.\n");

        var rateLimiter = new TokenBucketRateLimiter(
            tokensPerSecond: 5,
            bucketCapacity: 10);

        Console.WriteLine("Rate Limiting Pattern: Rate limit: 5 requests/second, burst capacity: 10\n");

        Console.WriteLine("Rate Limiting Pattern: --- Burst of Requests ---\n");

        // Simulate burst of requests
        var tasks = new List<Task>();
        for (var i = 1; i <= 15; i++)
        {
            var requestId = i;
            tasks.Add(Task.Run(async () =>
            {
                if (rateLimiter.TryAcquire())
                {
                    Console.WriteLine($"Rate Limiting Pattern: Request #{requestId} - ALLOWED");
                    await Task.Delay(50); // Simulate processing
                }
                else
                {
                    Console.WriteLine($"Rate Limiting Pattern: Request #{requestId} - REJECTED (Rate limit exceeded)");
                }
            }));

            await Task.Delay(50); // Stagger requests slightly
        }

        await Task.WhenAll(tasks);

        Console.WriteLine("\nRate Limiting Pattern: Waiting for token replenishment...\n");
        await Task.Delay(1000);

        Console.WriteLine("Rate Limiting Pattern: --- After Token Replenishment ---\n");

        for (var i = 16; i <= 20; i++)
        {
            if (rateLimiter.TryAcquire())
            {
                Console.WriteLine($"Rate Limiting Pattern: Request #{i} - ALLOWED");
            }
            else
            {
                Console.WriteLine($"Rate Limiting Pattern: Request #{i} - REJECTED");
            }
        }

        Console.WriteLine("\nRate Limiting Pattern: Demonstration complete.");
    }
}

// Simple Token Bucket rate limiter implementation
public class TokenBucketRateLimiter(int tokensPerSecond, int bucketCapacity)
{
    private readonly double _tokensPerSecond = tokensPerSecond;
    private readonly int _bucketCapacity = bucketCapacity;
    private readonly Lock _lock = new();

    private double _availableTokens = bucketCapacity;
    private DateTime _lastRefillTime = DateTime.UtcNow;

    public bool TryAcquire()
    {
        lock (_lock)
        {
            RefillTokens();

            if (_availableTokens >= 1)
            {
                _availableTokens -= 1;
                return true;
            }

            return false;
        }
    }

    private void RefillTokens()
    {
        var now = DateTime.UtcNow;
        var timeSinceLastRefill = now - _lastRefillTime;
        var tokensToAdd = timeSinceLastRefill.TotalSeconds * _tokensPerSecond;

        _availableTokens = Math.Min(_availableTokens + tokensToAdd, _bucketCapacity);
        _lastRefillTime = now;
    }
}