# Audabit.Service.SoftwarePatterns - Copilot Instructions

This file provides comprehensive guidance for the Audabit.Service.SoftwarePatterns reference implementation.

---

## Project Overview

Audabit.Service.SoftwarePatterns demonstrates enterprise-grade best practices for:
- Multi-layer architecture with clean separation of concerns
- External API integration with proper abstraction
- Comprehensive telemetry and observability
- Production-ready error handling and validation
- Automated build, test, and deployment pipelines

---

## Architecture & Design Patterns

### Pattern 1: Multi-Layer HTTP Client Pattern (StarWars - REFERENCE IMPLEMENTATION)

**Use Case**: External API integration requiring proper abstraction and testability

**Layer Flow**:
```
External API → Client Layer → Domain Models → Service Layer → API Layer → Consumer
              (HTTP Client)   (Mapping)      (Business Logic) (DTO Mapping)
```

**Implementation Layers**:

1. **Client Models** (`App/Clients/StarWars/Models/`) - DTOs matching external API
2. **Client Interface & Implementation** (`App/Clients/StarWars/`) - HTTP client with resilience (v1, v2)
3. **Client Factory** (`App/Clients/StarWars/Factories/`) - Factory for selecting client version (IStarWarsClientFactory)
4. **Client Mapper** (`App/Clients/StarWars/Mappers/`) - External DTO → Domain mapping (Riok.Mapperly)
5. **Domain Models** (`App.Models/`) - Business entities independent of external API
6. **Telemetry Events** (`App/Telemetry/`) - Retrieving/Retrieved/RetrievalFailed events
7. **Service Layer** (`App/Services/`) - Business logic with telemetry integration, uses factory
8. **API DTOs** (`Api/v1/StarWars/`) - Public API contract models
9. **API Mapper** (`WebApi/Mappers/`) - Domain → API DTO mapping
10. **Controller** (`WebApi/Controllers/v1/`) - HTTP endpoints

**When to Use**: Any feature requiring external HTTP API integration

---

### Pattern 2: Client Factory Pattern (StarWars)

**Use Case**: Multiple client versions requiring dynamic selection based on business rules

**Implementation**:
```csharp
// Interface (App/Clients/StarWars/Factories/IStarWarsClientFactory.cs)
public interface IStarWarsClientFactory
{
    IStarWarsClient GetClient(int id);
}

// Factory (App/Clients/StarWars/Factories/StarWarsClientFactory.cs)
public sealed class StarWarsClientFactory(IEnumerable<IStarWarsClient> starWarsClients) 
    : IStarWarsClientFactory
{
    public IStarWarsClient GetClient(int id)
    {
        // NOTE: This is just an example of business logic to choose the client version.
        // NOTE2: You could also use starWarsClients.OfType<StarWarsClient>().First() & starWarsClients.OfType<StarWarsClientV2>().First()
        var version = (id % 10 == 2) ? "v2" : "v1";
        return starWarsClients.FirstOrDefault(client => client.Version == version)
            ?? throw new InvalidOperationException($"Client version '{version}' not registered.");
    }
}

// Service usage
public sealed class StarWarsService(IStarWarsClientFactory factory) : IStarWarsService
{
    public async Task<StarWarsPerson?> GetPersonAsync(int id, CancellationToken cancellationToken)
    {
        var client = factory.GetClient(id);
        return await client.GetPersonAsync(id, cancellationToken);
    }
}
```

**Registration** (`Program.cs`):
```csharp
builder.Services.AddScoped<IStarWarsClient, StarWarsClient>();
builder.Services.AddScoped<IStarWarsClient, StarWarsClientV2>();
builder.Services.AddScoped<IStarWarsClientFactory, StarWarsClientFactory>();
```

**Architectural Note**: 
- Factory lives in App layer (not WebApi) because selection logic is a business rule
- Same rationale as keeping HTTP clients in App layer - cohesion and business logic placement
- In pure Clean Architecture, factories typically live in infrastructure (WebApi), but this is a pragmatic exception

**When to Use**:
- A/B testing between client versions
- Canary deployments with gradual rollout
- Feature flags controlling which version to use
- Multiple backend versions requiring different client implementations

---

### Pattern 3: Simple Service Pattern (Weather)

**Use Case**: Internal data generation without external dependencies

**Layers**: Service → Mapper → Controller

---

### Pattern 4: Telemetry Event Pattern

**Event Lifecycle**:
1. `{Operation}RetrievingEvent` - Before operation
2. `{Operation}RetrievedEvent` - Success (includes result)
3. `{Operation}RetrievalFailedEvent` - Failure (includes error)

**Usage**: Structured observability for monitoring, debugging, analytics

---

## Adding New Features Checklist (StarWars Pattern)

When adding features with external API integration:

1. [ ] Define client models (`App/Clients/{Feature}/Models/`)
2. [ ] Create client interface (`I{Feature}Client.cs`) with Version property
3. [ ] Implement HTTP client(s) (`{Feature}Client.cs`, `{Feature}ClientV2.cs` if multiple versions)
4. [ ] Add client factory if multiple versions (`Factories/I{Feature}ClientFactory.cs`)
5. [ ] Add client mapper with `[Mapper]` attribute
6. [ ] Define domain models (`App.Models/`)
7. [ ] Create telemetry events (Retrieving/Retrieved/RetrievalFailed)
8. [ ] Implement service interface and class (use factory if multiple client versions)
9. [ ] Define API DTOs (`Api/v1/{Feature}/`)
10. [ ] Add API mapper
11. [ ] Implement controller with API versioning
12. [ ] Register services in `Program.cs` (including factory if applicable)
13. [ ] Write tests for all layers (client, factory, mapper, service, controller)
14. [ ] Update README.md

---

## CI/CD Pipeline

**Version Format**: `yyyy.MM.dd.counter` (e.g., 2026.01.20.1)  
**Pipeline Stages**: Format check → Restore → Build → Test → Pack (Api) → Publish (WebApi)  
**Artifacts**: NuGet package to Azure Artifacts, WebApi for deployment

---

## Best Practices Summary

**DO** ✅:
- Follow StarWars pattern for external APIs
- Use Riok.Mapperly for mappings
- Add telemetry events for operations
- Write tests for every layer
- Use primary constructors for DI
- Keep controllers thin

**DON'T** ❌:
- Expose domain models in API responses (use DTOs)
- Use reflection-based mappers
- Skip telemetry for important operations
- Mix business logic in controllers

---

## Documentation Standards

### XML Documentation

**Service Projects**: XML documentation is **OPTIONAL** for service project code.
- Focus on clean, self-documenting code over verbose documentation
- Use XML docs sparingly, only where truly needed for clarity
- Prefer clear naming and simple implementations

**Common Libraries**: XML documentation is **REQUIRED** for all public APIs in Audabit.Common.* libraries.

### Code Comments

**Use inline comments** (`//`) for:
- Business logic explanations
- Non-obvious implementation details
- Examples of alternative approaches

**Example** (StarWarsClientFactory):
```csharp
public IStarWarsClient GetClient(int id)
{
    // NOTE: This is just an example of business logic to choose the client version.
    // NOTE2: You could also use starWarsClients.OfType<StarWarsClient>().First() & starWarsClients.OfType<StarWarsClientV2>().First()
    var version = (id % 10 == 2) ? "v2" : "v1";
    return starWarsClients.FirstOrDefault(client => client.Version == version)
        ?? throw new InvalidOperationException($"Client version '{version}' not registered.");
}
```

---

## Modern C# Features

### .NET 9 Interface Members

**Abstract Properties** (.NET 9+):
```csharp
// ✅ Valid in .NET 9 - explicitly marks property as abstract
public interface IStarWarsClient
{
    abstract string Version { get; }
    Task<StarWarsPerson?> GetPersonAsync(int id, CancellationToken ct = default);
}
```

**Alternative** (Implicit abstract - works in all .NET versions):
```csharp
public interface IStarWarsClient  
{
    string Version { get; } // Implicitly abstract
    Task<StarWarsPerson?> GetPersonAsync(int id, CancellationToken ct = default);
}
```

### ConfigureAwait(false) - Application Code

**Service Projects**: `.ConfigureAwait(false)` is **NOT REQUIRED**.
- ASP.NET Core doesn't have a synchronization context to capture
- Omitting ConfigureAwait improves code readability
- Performance difference is negligible in ASP.NET Core applications

**Common Libraries**: `.ConfigureAwait(false)` is **REQUIRED** on all awaits.

```csharp
// ✅ Service Projects - ConfigureAwait NOT required
var person = await client.GetPersonAsync(id, cancellationToken);

// ✅ Common Libraries - ConfigureAwait REQUIRED
var response = await httpClient.GetAsync(endpoint, cancellationToken).ConfigureAwait(false);
```

---

## Sealed Modifier Pattern - Web API Projects

### When to Use `sealed`

**Always Sealed**:
- ✅ Settings records: `public sealed record {Feature}Settings`
- ✅ Validators: `public sealed class {Feature}Validator : AbstractValidator<T>`
- ✅ Services: `public sealed class {Feature}Service : I{Feature}Service`
- ✅ Clients: `public sealed class {Feature}Client : I{Feature}Client`
- ✅ Factories: `public sealed class {Feature}ClientFactory : I{Feature}ClientFactory`
- ✅ DTOs/Transport models (optional but recommended): `public sealed record {Feature}Request`

**Never Sealed**:
- ❌ Controllers (ASP.NET framework compatibility)
- ❌ Interfaces
- ❌ Mappers (Riok.Mapperly requires partial classes)

**Examples**:
```csharp
// ✅ Settings - Always sealed
public sealed record WeatherSettings
{
    public int MinTemperature { get; init; } = -20;
    public int MaxTemperature { get; init; } = 55;
}

// ✅ Validator - Always sealed
public sealed class WeatherSettingsValidator : AbstractValidator<WeatherSettings>
{
    public WeatherSettingsValidator()
    {
        RuleFor(x => x.MinTemperature)
            .LessThan(x => x.MaxTemperature);
    }
}

// ✅ Service - Always sealed (performance + final implementation)
public sealed class WeatherService : IWeatherService
{
    // Implementation (testing done via interface)
}

// ✅ Client - Always sealed (performance + final implementation)
public sealed class StarWarsClient : IStarWarsClient
{
    public string Version => "v1";
    // Implementation (testing done via interface)
}

// ✅ Factory - Always sealed (performance + final implementation)
public sealed class StarWarsClientFactory : IStarWarsClientFactory
{
    // Implementation (testing done via interface)
}

// ✅ Request/Response DTOs - Recommended sealed
public sealed record PutWeatherForecastRequest(string SomeRandomParameter);
public sealed record GetWeatherForecastResponse(IEnumerable<WeatherForecastDto> Forecasts);

// ❌ Controller - Never sealed
public class WeatherController : ControllerBase { }

// ❌ Mapper - Never sealed (Riok.Mapperly uses partial)
[Mapper]
public partial class WeatherMapper { }
```

---

## XML Documentation Requirements - Web API Projects

### Required Documentation

**All Public Interfaces** (class + methods):
```csharp
/// <summary>
/// Contract for weather forecast operations.
/// </summary>
public interface IWeatherService
{
    /// <summary>
    /// Generates random weather forecasts for the specified country.
    /// </summary>
    /// <param name="countryCode">The 2-letter country code (e.g., "US").</param>
    /// <returns>An array of weather forecasts.</returns>
    Task<WeatherForecast[]> GetForecastAsync(string countryCode);
}
```

**All Validators** (class + constructor):
```csharp
/// <summary>
/// Validator for <see cref="WeatherSettings"/> configuration.
/// </summary>
public sealed class WeatherSettingsValidator : AbstractValidator<WeatherSettings>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WeatherSettingsValidator"/> class
    /// and configures validation rules for weather forecast settings.
    /// </summary>
    public WeatherSettingsValidator()
    {
        // Rules...
    }
}
```

**Settings with Comprehensive Examples**:
```csharp
/// <summary>
/// Configuration settings for weather forecast generation.
/// </summary>
/// <remarks>
/// <para>
/// Controls the parameters for generating random weather forecasts.
/// All settings can be configured via appsettings.json or environment variables.
/// </para>
/// </remarks>
/// <example>
/// Configuration in appsettings.json:
/// <code>
/// {
///   "WeatherSettings": {
///     "MinTemperature": -20,
///     "MaxTemperature": 55
///   }
/// }
/// </code>
/// 
/// Or using environment variables:
/// <code>
/// WeatherSettings__MinTemperature=-20
/// WeatherSettings__MaxTemperature=55
/// </code>
/// </example>
public sealed record WeatherSettings
{
    /// <summary>
    /// Gets or initializes the minimum temperature in Celsius.
    /// </summary>
    /// <value>
    /// Valid range: -100 to 100. Default is -20°C.
    /// Used as the lower bound for random temperature generation.
    /// </value>
    public int MinTemperature { get; init; } = -20;
}
```

---

## Common Libraries Integration

### Key Differences: Service Projects vs Common Libraries

| Aspect | Audabit.Service.SoftwarePatterns (This Project) | Audabit.Common.* Libraries |
|--------|-------------------------------|---------------------------|
| **XML Documentation** | Optional (focus on clean code) | Required for all public APIs |
| **ConfigureAwait(false)** | Not required (ASP.NET Core) | Required on all awaits |
| **Target** | Web API application | Reusable NuGet packages |
| **Sealed Classes** | Services, Clients, Factories, Validators, Settings | Background services, Health checks, Validators, Settings |
| **Testing** | Unit + Integration tests | Unit tests only |
| **Package Distribution** | Api project packaged, WebApi deployed | All projects packaged to NuGet |
| **Dependencies** | Can depend on specific versions | Minimize dependencies |

### Audabit.Service.SoftwarePatterns Usage of Common Libraries

**Infrastructure Libraries Used**:
- `Audabit.Common.ApiVersioning.AspNet` - API versioning
- `Audabit.Common.CorrelationId.AspNet` - Request correlation tracking
- `Audabit.Common.ExceptionHandling.AspNet` - Global exception handling
- `Audabit.Common.HealthChecks.AspNet` - Health check endpoints (startup, readiness, liveness)
- `Audabit.Common.HttpClient.AspNet` - Resilient HTTP clients with Polly
- `Audabit.Common.Observability` - Structured logging event system
- `Audabit.Common.Observability.AspNet` - ASP.NET Core observability extensions
- `Audabit.Common.Security.AspNet` - API key authentication
- `Audabit.Common.Serialization` - JSON serialization with sensitive data masking
- `Audabit.Common.ServiceInfo.AspNet` - Service metadata endpoint
- `Audabit.Common.Swagger.AspNet` - OpenAPI documentation
- `Audabit.Common.Validation.AspNet` - FluentValidation integration

**Registration Example** (see `Program.cs` for complete setup):
```csharp
// Infrastructure services
builder.Services.AddServiceInfo(serviceSettingsSection);
builder.Services.AddApiKeySecurity(apiKeySettingsSection);
builder.Services.AddObservability(serviceName).UseJsonConsoleLogging();
builder.Services.AddApiVersioningConfiguration();
builder.Services.AddFluentValidationOnApis(typeof(Program));
builder.Services.AddSwaggerDocumentation(serviceName, apiKeyHeaderName, typeof(Program));
builder.Services.AddHealthChecks(healthChecksSettingsSection);

// HTTP clients with resilience
builder.Services
    .AddResilientHttpClients(httpClientSettingsSection)
    .AddResilientHttpClient<StarWarsClient>()
    .AddResilientHttpClient<StarWarsClientV2>();

// Middleware order (critical!)
app.UseExceptionMiddleware(serviceName);
app.UseCorrelationIdMiddleware();
app.UseApiKeyMiddleware(app.Environment.IsDevelopment());
app.UseHealthChecks();
```

---

**For detailed Common library development standards, see**:
- Root: `C:\\Users\\John.Schaap\\Desktop\\Audabit\\.github\\copilot-instructions.md`
- Each library's `.github/copilot-instructions.md` in their respective repositories

---

**Last Updated**: January 25, 2026  
**Maintainer**: John Schaap  
**License**: Apache 2.0
