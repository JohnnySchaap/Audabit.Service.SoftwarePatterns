# Reliability Patterns

Patterns for building resilient systems that gracefully handle failures, prevent cascading issues, and maintain availability under adverse conditions.

## 📖 About Reliability Patterns

Reliability patterns help systems survive failures and degraded conditions:
- **Resilience**: Gracefully handle transient failures and recover automatically
- **Availability**: Keep services running even when dependencies fail
- **Fault Isolation**: Prevent failures from cascading across system boundaries
- **Graceful Degradation**: Maintain partial functionality when components fail

---

## 🎯 Patterns

### 1. [Circuit Breaker](1_CircuitBreaker.cs)
**Problem**: Repeatedly calling a failing service wastes resources and causes cascading failures
**Solution**: Automatically stop calling a failing service for a timeout period, then test if it recovered

**When to Use**:
- Making calls to external services (APIs, databases, message queues)
- Service can fail temporarily (network issues, deployment, overload)
- Want to prevent cascading failures in microservices
- Need to fail fast when downstream service is down

**Modern .NET**:
```csharp
// Using Polly library (production-ready)
var circuitBreaker = Policy
    .Handle<HttpRequestException>()
    .CircuitBreakerAsync(
        exceptionsAllowedBeforeBreaking: 3,
        durationOfBreak: TimeSpan.FromSeconds(30));

await circuitBreaker.ExecuteAsync(async () =>
{
    return await httpClient.GetAsync("https://api.example.com/data");
});
```

**Real-World Examples**: Microservice API calls, database connections, message queue operations, external API integrations

**Personal Usage**:
> **Microservices Communication**
>
> Circuit breakers are **essential in microservices architectures** to prevent cascading failures when one service degrades.
>
> - **Challenge**: Payment service calls inventory service; when inventory is down, payment requests pile up and exhaust resources
> - **Solution**: Polly circuit breaker with 5 failures in 10 seconds trips circuit for 30 seconds, then half-open test
> - **Result**: Payment service fails fast when inventory is down, maintains own health, automatically recovers when inventory returns
> - **Technology**: Polly, HttpClientFactory with circuit breaker policies, Application Insights for monitoring state changes

---

### 2. [Bulkhead](2_Bulkhead.cs)
**Problem**: A failing component consumes all resources, bringing down the entire system
**Solution**: Partition resources into isolated pools so failure in one area doesn't affect others

**When to Use**:
- Multiple services/features share the same resource pool
- Critical operations must stay available even if non-critical ones fail
- Want to limit blast radius of failures
- Need to guarantee resources for high-priority requests

**Modern .NET**:
```csharp
// Using Polly Bulkhead for isolation
var bulkhead = Policy.BulkheadAsync(
    maxParallelization: 10,
    maxQueuingActions: 20);

await bulkhead.ExecuteAsync(async () =>
{
    return await ProcessRequestAsync();
});

// Or using SemaphoreSlim directly
private static readonly SemaphoreSlim _semaphore = new(10, 10);

public async Task<T> ProcessAsync<T>(Func<Task<T>> action)
{
    await _semaphore.WaitAsync();
    try
    {
        return await action();
    }
    finally
    {
        _semaphore.Release();
    }
}
```

**Real-World Examples**: 
- Separate thread pools for critical vs non-critical work
- Dedicated connection pools per tenant/customer
- Kubernetes resource limits (CPU/memory per pod)
- Database connection pooling per service

**Personal Usage**:
> **Multi-Tenant SaaS Resource Isolation**
>
> Bulkheads prevent one tenant from consuming all resources in multi-tenant applications.
>
> - **Challenge**: Large tenant runs expensive queries that exhaust database connections, impacting all other tenants
> - **Solution**: Separate connection pools and SemaphoreSlim limits per tenant tier (Enterprise: 50, Standard: 20, Free: 5)
> - **Result**: Noisy neighbor isolation, guaranteed capacity for premium tiers, fair resource distribution
> - **Technology**: SemaphoreSlim, separate HttpClient instances per tenant, Azure SQL elastic pools

---

### 3. [Retry with Backoff](3_RetryBackoff.cs)
**Problem**: Transient failures (network blips, temporary overload) cause operations to fail unnecessarily
**Solution**: Retry failed operations with exponentially increasing delays between attempts

**When to Use**:
- Dealing with transient failures (network timeouts, temporary service unavailability)
- Remote service might recover after brief delay
- Want to avoid thundering herd when many clients retry simultaneously
- Cost of retrying is lower than cost of failure

**Modern .NET**:
```csharp
// Using Polly for exponential backoff
var retryPolicy = Policy
    .Handle<HttpRequestException>()
    .WaitAndRetryAsync(
        retryCount: 5,
        sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
        onRetry: (exception, timeSpan, retryCount, context) =>
        {
            Console.WriteLine($"Retry {retryCount} after {timeSpan}");
        });

await retryPolicy.ExecuteAsync(async () =>
{
    return await httpClient.GetAsync("https://api.example.com/data");
});
```

**Real-World Examples**: HTTP API calls, database operations, message publishing, file uploads to cloud storage

**Personal Usage**:
> **Azure Service Bus Message Processing**
>
> Retry with backoff handles transient failures when processing messages from Azure Service Bus.
>
> - **Challenge**: Message processing occasionally fails due to transient database deadlocks or rate limiting
> - **Solution**: Polly retry policy with exponential backoff (100ms, 200ms, 400ms, 800ms) + jitter to prevent thundering herd
> - **Result**: Automatic recovery from transient failures, reduced failed messages, no manual intervention needed
> - **Technology**: Polly, Azure Service Bus, retry policies combined with circuit breaker

---

### 4. [Rate Limiting](4_RateLimiting.cs)
**Problem**: Unlimited request rate can overwhelm system resources and cause outages
**Solution**: Control request rate using token bucket or sliding window algorithms

**When to Use**:
- **API endpoints** need protection from abuse or overload
- Want to ensure fair usage across clients/tenants
- Need to prevent resource exhaustion (CPU, memory, connections)
- Upstream service has rate limits you must respect

**Modern .NET**:
```csharp
// Using System.Threading.RateLimiting (.NET 7+)
var rateLimiter = new TokenBucketRateLimiter(new TokenBucketRateLimiterOptions
{
    TokenLimit = 100,
    TokensPerPeriod = 10,
    ReplenishmentPeriod = TimeSpan.FromSeconds(1)
});

using var lease = await rateLimiter.AcquireAsync();
if (lease.IsAcquired)
{
    await ProcessRequestAsync();
}
else
{
    return Results.StatusCode(429); // Too Many Requests
}

// Or in middleware (ASP.NET Core 7+)
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("api", limiterOptions =>
    {
        limiterOptions.PermitLimit = 100;
        limiterOptions.Window = TimeSpan.FromMinutes(1);
    });
});
```

**Real-World Examples**: 
- API Gateway rate limiting (requests per minute per API key)
- Per-user quotas in SaaS applications
- Database connection rate limiting
- External API calls respecting provider limits

**Personal Usage**:
> **Public API Rate Limiting**
>
> Rate limiting is **mandatory for any public API** to prevent abuse and ensure availability.
>
> - **Challenge**: Public REST API needs to prevent abuse while allowing legitimate high-volume clients
> - **Solution**: ASP.NET Core rate limiting middleware with token bucket: 1000 requests/minute per API key, 10,000/minute for premium tier
> - **Result**: Protection against DDoS/abuse, fair resource distribution, premium tier monetization
> - **Technology**: System.Threading.RateLimiting, ASP.NET Core middleware, Azure API Management for advanced policies

---

### 5. [Fail Fast](5_FailFast.cs)
**Problem**: Processing invalid input wastes resources and makes debugging harder
**Solution**: Validate inputs immediately at system boundaries and reject invalid data early

**When to Use**:
- **API endpoints** receiving external input
- Message handlers processing queue messages
- Background jobs with configuration dependencies
- Any operation where early validation prevents wasted work

**Modern .NET**:
```csharp
// Using FluentValidation
public class CreateOrderValidator : AbstractValidator<CreateOrderRequest>
{
    public CreateOrderValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty();
        RuleFor(x => x.CustomerId).NotEmpty();
        RuleFor(x => x.Items).NotEmpty();
        RuleFor(x => x.TotalAmount).GreaterThan(0);
    }
}

// In API endpoint
[HttpPost]
public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
{
    // Validation happens in filter - fails fast before controller action
    var validator = new CreateOrderValidator();
    var result = await validator.ValidateAsync(request);
    
    if (!result.IsValid)
    {
        return BadRequest(result.Errors); // Fail fast!
    }

    // Expensive processing only happens for valid input
    await _orderService.ProcessAsync(request);
    return Ok();
}

// Or validate configuration at startup
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        var config = Configuration.GetSection("MySettings").Get<MySettings>();
        
        // Fail fast if configuration is invalid - don't start the app!
        if (string.IsNullOrEmpty(config.ConnectionString))
        {
            throw new InvalidOperationException("ConnectionString is required");
        }
    }
}
```

**Real-World Examples**: 
- API request validation
- Configuration validation at startup
- Database constraint checks
- Message schema validation

**Personal Usage**:
> **API Gateway Request Validation**
>
> Fail fast validation prevents invalid requests from reaching downstream services.
>
> - **Challenge**: Invalid API requests waste downstream service resources and complicate debugging
> - **Solution**: FluentValidation in API middleware validates all requests before routing; startup validation ensures configuration is correct
> - **Result**: 40% reduction in downstream errors, faster debugging (errors close to source), reduced costs from wasted processing
> - **Technology**: FluentValidation, ASP.NET Core filters, startup health checks

---

## 🏆 Pattern Combinations

### Circuit Breaker + Retry = Smart Resilience
```
Retry (transient failures) → Circuit Breaker (persistent failures) → Fail fast when service is down
```
Don't retry when circuit is open - respect the circuit breaker state.

### Bulkhead + Rate Limiting = Resource Protection
```
Rate Limiting (per client) → Bulkhead (per service) → Isolated resource pools with controlled access
```

### Fail Fast + Circuit Breaker = Defense in Depth
```
Fail Fast (invalid input) → Circuit Breaker (failing service) → Multiple layers of failure detection
```

### All Together = Polly Resilience Pipeline
```csharp
// Modern resilience strategy combining all patterns
var pipeline = new ResiliencePipelineBuilder()
    .AddRateLimiter(new TokenBucketRateLimiter(...))     // Rate limiting
    .AddTimeout(TimeSpan.FromSeconds(10))                // Fail fast on timeout
    .AddRetry(new RetryStrategyOptions                   // Retry with backoff
    {
        MaxRetryAttempts = 3,
        Delay = TimeSpan.FromSeconds(1),
        BackoffType = DelayBackoffType.Exponential
    })
    .AddCircuitBreaker(new CircuitBreakerStrategyOptions // Circuit breaker
    {
        FailureRatio = 0.5,
        MinimumThroughput = 10,
        BreakDuration = TimeSpan.FromSeconds(30)
    })
    .AddBulkheadIsolation(maxConcurrency: 10)           // Bulkhead
    .Build();

await pipeline.ExecuteAsync(async ct =>
{
    return await httpClient.GetAsync(url, ct);
});
```

---

## ⚠️ Common Pitfalls

1. **Not combining retry with circuit breaker** - Retrying when circuit is open wastes resources
2. **Retrying non-transient errors** - Don't retry 4xx client errors, only 5xx server errors
3. **No backoff or jitter** - Fixed retry delays cause thundering herd problem
4. **Circuit breaker timeout too short** - Service needs time to recover
5. **Bulkhead too small** - Can't handle normal load, too restrictive
6. **Rate limiting too aggressive** - Legitimate clients get blocked
7. **Fail fast but no fallback** - User sees error instead of degraded functionality

---

## 📊 Real-World Usage Examples

### E-Commerce Platform Resilience
```
User checkout → Rate limiting (prevent abuse) → 
Fail fast validation → Bulkhead (payment isolated from search) →
Circuit breaker (payment gateway) → Retry with backoff (transient failures)
```

### Microservices Communication
```
Service A → Circuit breaker → Retry with backoff → Service B
              ↓ Open                                    ↓ Down
         Fail fast with fallback              Return cached data
```

### Public API Gateway
```
Client request → Rate limiting (per API key) →
Fail fast validation → Bulkhead (per tenant) →
Circuit breaker (per downstream service) → Backend services
```

---

[← Back to Main README](../../../README.md)
