namespace Audabit.Service.SoftwarePatterns.App.Patterns._7_Reliability;

public interface IReliabilityPatternsService
{
    Task CircuitBreakerAsync();

    Task BulkheadAsync();

    Task RetryBackoffAsync();

    Task RateLimitingAsync();

    void FailFast();
}