
namespace Audabit.Service.SoftwarePatterns.App.Patterns._7_Reliability.Patterns;

// CA1031 suppression: Generic catch intentional for circuit breaker demo to handle all failure types
#pragma warning disable CA1031 // Do not catch general exception types

// Circuit Breaker Pattern: Prevent cascading failures by breaking the circuit when failure threshold is exceeded.
// In this example:
// - We track consecutive failures and transition between Closed → Open → Half-Open states.
// - When circuit is Closed, requests pass through normally.
// - After threshold failures, circuit Opens (all requests fail fast for timeout period).
// - After timeout, circuit becomes Half-Open (single test request allowed).
// - If test succeeds, circuit Closes; if it fails, circuit reopens.
//
// MODERN C# USAGE:
// - Use Polly library for production circuit breakers (don't write your own!)
// - Combine with retry, timeout, and fallback policies
// - Monitor circuit state changes for observability
// - Consider per-endpoint circuit breakers in microservices
public static class CircuitBreaker
{
    public static async Task RunAsync()
    {
        Console.WriteLine("Circuit Breaker Pattern: Demonstrating circuit breaker with failure threshold...");
        Console.WriteLine("Circuit Breaker Pattern: Protecting against cascading failures.\n");

        var breaker = new SimpleCircuitBreaker(
            failureThreshold: 3,
            openDurationSeconds: 2);

        Console.WriteLine("Circuit Breaker Pattern: --- Simulating Remote Service Calls ---\n");

        // Simulate successful calls
        for (var i = 1; i <= 2; i++)
        {
            await CallServiceWithCircuitBreaker(breaker, shouldSucceed: true, callNumber: i);
        }

        Console.WriteLine("\nCircuit Breaker Pattern: --- Triggering Failures ---\n");

        // Trigger failures to open circuit
        for (var i = 3; i <= 5; i++)
        {
            await CallServiceWithCircuitBreaker(breaker, shouldSucceed: false, callNumber: i);
        }

        Console.WriteLine("\nCircuit Breaker Pattern: --- Circuit is OPEN - Fast Failing ---\n");

        // These will fail fast without calling service
        for (var i = 6; i <= 7; i++)
        {
            await CallServiceWithCircuitBreaker(breaker, shouldSucceed: true, callNumber: i);
        }

        Console.WriteLine("\nCircuit Breaker Pattern: Waiting for circuit to attempt recovery (Half-Open)...\n");
        await Task.Delay(2100);

        Console.WriteLine("Circuit Breaker Pattern: --- Circuit is HALF-OPEN - Testing Service ---\n");

        // This will attempt the call (half-open state)
        await CallServiceWithCircuitBreaker(breaker, shouldSucceed: true, callNumber: 8);

        Console.WriteLine("\nCircuit Breaker Pattern: --- Circuit is CLOSED - Normal Operation Resumed ---\n");

        // Circuit is closed again
        for (var i = 9; i <= 10; i++)
        {
            await CallServiceWithCircuitBreaker(breaker, shouldSucceed: true, callNumber: i);
        }

        Console.WriteLine("\nCircuit Breaker Pattern: Demonstration complete.");
    }

    private static async Task CallServiceWithCircuitBreaker(SimpleCircuitBreaker breaker, bool shouldSucceed, int callNumber)
    {
        try
        {
            await breaker.ExecuteAsync(async () =>
            {
                Console.WriteLine($"Circuit Breaker Pattern: Call #{callNumber} - Calling remote service...");
                await Task.Delay(50); // Simulate network call

                if (!shouldSucceed)
                {
                    throw new HttpRequestException("Service unavailable");
                }

                Console.WriteLine($"Circuit Breaker Pattern: Call #{callNumber} - Success!");
            });
        }
        catch (CircuitBreakerOpenException)
        {
            Console.WriteLine($"Circuit Breaker Pattern: Call #{callNumber} - REJECTED (Circuit is OPEN)");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Circuit Breaker Pattern: Call #{callNumber} - Failed: {ex.Message}");
        }
    }
}

// Simple circuit breaker implementation for demonstration
public class SimpleCircuitBreaker(int failureThreshold, int openDurationSeconds)
{
    private readonly int _failureThreshold = failureThreshold;
    private readonly TimeSpan _openDuration = TimeSpan.FromSeconds(openDurationSeconds);
    private readonly Lock _lock = new();

    private int _consecutiveFailures;
    private DateTime _lastFailureTime;
    private CircuitState _state = CircuitState.Closed;

    public async Task ExecuteAsync(Func<Task> action)
    {
        lock (_lock)
        {
            // Check if circuit should transition from Open to Half-Open
            if (_state == CircuitState.Open && DateTime.UtcNow - _lastFailureTime >= _openDuration)
            {
                _state = CircuitState.HalfOpen;
                Console.WriteLine("Circuit Breaker Pattern: State changed: OPEN → HALF-OPEN");
            }

            // Fail fast if circuit is open
            if (_state == CircuitState.Open)
            {
                throw new CircuitBreakerOpenException("Circuit breaker is OPEN");
            }
        }

        try
        {
            await action();
            OnSuccess();
        }
        catch (Exception)
        {
            OnFailure();
            throw;
        }
    }

    private void OnSuccess()
    {
        lock (_lock)
        {
            if (_state == CircuitState.HalfOpen)
            {
                Console.WriteLine("Circuit Breaker Pattern: Test call succeeded. State changed: HALF-OPEN → CLOSED");
            }

            _consecutiveFailures = 0;
            _state = CircuitState.Closed;
        }
    }

    private void OnFailure()
    {
        lock (_lock)
        {
            _consecutiveFailures++;
            _lastFailureTime = DateTime.UtcNow;

            if (_consecutiveFailures >= _failureThreshold)
            {
                _state = CircuitState.Open;
                Console.WriteLine($"Circuit Breaker Pattern: Failure threshold reached ({_failureThreshold}). State changed: CLOSED → OPEN");
            }
        }
    }
}

public enum CircuitState
{
    Closed,    // Normal operation
    Open,      // Failing fast
    HalfOpen   // Testing if service recovered
}

public class CircuitBreakerOpenException(string message) : Exception(message)
{
}