namespace Audabit.Service.SoftwarePatterns.App.Patterns._7_Reliability.Patterns;

// CA1031 suppression: Generic catch intentional for retry pattern demo to handle all failure types
#pragma warning disable CA1031 // Do not catch general exception types

// Retry with Exponential Backoff Pattern: Retry failed operations with increasing delays between attempts.
// In this example:
// - We retry transient failures (network issues, temporary service unavailability).
// - Delay between retries grows exponentially: 100ms, 200ms, 400ms, 800ms, etc.
// - Adds jitter to prevent thundering herd problem.
// - Gives downstream services time to recover.
//
// MODERN C# USAGE:
// - Use Polly for production retry policies (don't roll your own!)
// - Combine with circuit breaker to avoid retrying when service is down
// - Use CancellationToken to allow cancellation during retries
// - Only retry transient errors (5xx, timeout), not client errors (4xx)
public static class RetryBackoff
{
    public static async Task RunAsync()
    {
        Console.WriteLine("Retry Backoff Pattern: Demonstrating exponential backoff retry...");
        Console.WriteLine("Retry Backoff Pattern: Retrying with increasing delays.\n");

        Console.WriteLine("Retry Backoff Pattern: --- Transient Failure (Succeeds on Retry) ---\n");
        var attemptCount = 0;
        await RetryWithBackoff(
            () =>
            {
                attemptCount++;
                Console.WriteLine($"Retry Backoff Pattern: Attempt #{attemptCount} - Calling flaky service...");

                if (attemptCount < 3)
                {
                    throw new HttpRequestException("Transient network error");
                }

                Console.WriteLine($"Retry Backoff Pattern: Attempt #{attemptCount} - Success!");
                return Task.CompletedTask;
            },
            maxRetries: 5);

        Console.WriteLine("\nRetry Backoff Pattern: --- Permanent Failure (Exhausts Retries) ---\n");
        attemptCount = 0;
        try
        {
            await RetryWithBackoff(
                () =>
                {
                    attemptCount++;
                    Console.WriteLine($"Retry Backoff Pattern: Attempt #{attemptCount} - Calling failing service...");
                    throw new HttpRequestException("Service permanently down");
                },
                maxRetries: 3);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nRetry Backoff Pattern: All retries exhausted. Final error: {ex.Message}");
        }

        Console.WriteLine("\nRetry Backoff Pattern: Demonstration complete.");
    }

    private static async Task RetryWithBackoff(Func<Task> action, int maxRetries)
    {
        var baseDelayMs = 100;
        var maxDelayMs = 5000;

        for (var attempt = 0; attempt <= maxRetries; attempt++)
        {
            try
            {
                await action();
                return; // Success!
            }
            catch (Exception ex)
            {
                if (attempt >= maxRetries)
                {
                    throw; // Final attempt failed
                }

                // Calculate exponential backoff with jitter
                var exponentialDelay = Math.Min(baseDelayMs * Math.Pow(2, attempt), maxDelayMs);
                var jitter = Random.Shared.Next(0, (int)(exponentialDelay * 0.3));
                var delayMs = (int)exponentialDelay + jitter;

                Console.WriteLine($"Retry Backoff Pattern: Failed: {ex.Message}");
                Console.WriteLine($"Retry Backoff Pattern: Waiting {delayMs}ms before retry (attempt {attempt + 1}/{maxRetries})...\n");

                await Task.Delay(delayMs);
            }
        }
    }
}