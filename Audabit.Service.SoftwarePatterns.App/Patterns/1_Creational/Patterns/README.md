# Creational Patterns

Object creation mechanisms that provide flexibility in how objects are instantiated, promoting code reusability and reducing coupling between components.

## 📖 About Creational Patterns

Creational patterns abstract the instantiation process, making systems independent of how objects are created, composed, and represented:
- **Flexibility**: Choose object creation mechanisms at runtime
- **Encapsulation**: Hide complex creation logic behind simple interfaces
- **Reusability**: Centralize object creation for consistency
- **Testability**: Easier to mock and test with dependency injection

---

## 🎯 Patterns

### 1. [Singleton](1_Singleton.cs)
**Problem**: Need exactly one instance of a class shared across the entire application
**Solution**: Private constructor with static instance property ensuring single object creation

**When to Use**:
- Global access point to shared resource (configuration, logging)
- Exactly one instance required (thread pool, cache)
- Lazy initialization beneficial for performance

**Modern .NET**:
```csharp
// Thread-safe singleton with Lazy<T>
public sealed class DatabaseConnection
{
    private static readonly Lazy<DatabaseConnection> _instance =
        new(() => new DatabaseConnection());

    public static DatabaseConnection Instance => _instance.Value;

    private DatabaseConnection()
    {
        // Expensive initialization
    }
}

// Usage
var db = DatabaseConnection.Instance;
```

**Real-World Examples**: Configuration managers, logging services, caching systems

**Personal Usage**:
> **Not Recommended in Modern .NET**
>
> I avoid using Singleton pattern in production code because dependency injection provides better testability and lifecycle management. Modern .NET DI container handles singleton lifetime automatically.
>
> - **Challenge**: Need single instance shared across application with global access
> - **Solution**: Use `services.AddSingleton<IMapper, Mapper>()` instead of manual Singleton pattern
> - **Result**: Testable (inject mocks), explicit dependencies, follows SOLID, container manages lifetime
> - **Technology**: Microsoft.Extensions.DependencyInjection
> - **Safe singleton via DI**: Stateless classes, immutable state (readonly fields), thread-safe state (concurrent collections), examples include IOptions<T>, AutoMapper, IMemoryCache
> - **Why avoid manual pattern**: Hard to test (global static state), hidden dependencies (no constructor injection), tight coupling (violates Dependency Inversion Principle)

---

### 2. [Lazy Initialization](2_LazyInitialization.cs)
**Problem**: Object creation is expensive but might not always be needed
**Solution**: Defer object creation until first access using `Lazy<T>`

**When to Use**:
- Object creation is resource-intensive
- Object may not be used in every execution path
- Want thread-safe initialization without locks

**Modern .NET**:
```csharp
// Lazy initialization with Lazy<T>
public class DataService
{
    private readonly Lazy<ExpensiveResource> _resource =
        new(() => new ExpensiveResource());

    public void ProcessData()
    {
        // Only created when first accessed
        _resource.Value.DoWork();
    }
}
```

**Real-World Examples**: Report generation, file parsing, database connections

**Personal Usage**:
> **Report Generation with Deferred Data Loading**
>
> I use this pattern extensively for expensive resources that are not always needed in every execution path, particularly in reporting and data processing services.
>
> - **Challenge**: Loading large reference data (tax tables, product catalogs) upfront caused slow startup times even when not needed
> - **Solution**: Use `Lazy<T>` to defer loading until actual use, reducing memory footprint and startup time
> - **Result**: Faster application startup, reduced memory usage, resources loaded only when needed
> - **Technology**: .NET Lazy<T>, async initialization patterns

---

### 3. [Simple Factory](3_SimpleFactory.cs)
**Problem**: Object creation logic scattered across codebase
**Solution**: Centralize creation logic in a factory class

**When to Use**:
- Creation logic is simple but repeated
- Want to centralize object creation
- Do not need abstract factories or complex hierarchies

**Modern .NET**:
```csharp
// Simple factory
public class PaymentProcessorFactory
{
    public static IPaymentProcessor Create(string paymentType)
    {
        return paymentType switch
        {
            "CreditCard" => new CreditCardProcessor(),
            "PayPal" => new PayPalProcessor(),
            "BankTransfer" => new BankTransferProcessor(),
            _ => throw new ArgumentException($"Unknown payment type: {paymentType}")
        };
    }
}
```

**Real-World Examples**: Payment processors, notification senders, file parsers

**Personal Usage**:
> **Not Used**
>
> I do not use simple factories in production because they often become complex and violate Open/Closed Principle. Instead, I use Factory Method pattern with dependency injection.
>
> - **Why not Simple Factory**: Becomes God Object, violates Open/Closed, hard to test
> - **Alternative approach**: Strategy pattern with DI-based factory method (see Factory Method pattern example)
> - **When you might need this**: Very simple creation logic that will not grow, prototyping, scripts where SOLID principles are less critical

---

### 4. [Factory Method](4_FactoryMethod.cs)
**Problem**: Class cannot anticipate type of objects it needs to create
**Solution**: Delegate object creation to subclasses via abstract factory method

**When to Use**:
- Framework defines interface, subclasses choose implementation
- Class delegates creation to subclasses
- Want parallel class hierarchies

**Modern .NET**:
```csharp
// Abstract creator
public abstract class DocumentProcessor
{
    protected abstract IDocument CreateDocument();

    public void ProcessDocument()
    {
        var document = CreateDocument();
        document.Open();
        document.Process();
        document.Close();
    }
}

// Concrete creators
public class PdfProcessor : DocumentProcessor
{
    protected override IDocument CreateDocument() => new PdfDocument();
}

public class WordProcessor : DocumentProcessor
{
    protected override IDocument CreateDocument() => new WordDocument();
}
```

**Real-World Examples**: UI framework controls, document processors, data exporters

**Personal Usage**:
> **Payment Provider Factory with Strategy + DI**
>
> I use this pattern in combination with Strategy pattern and dependency injection, not with traditional abstract base classes and inheritance.
>
> - **Challenge**: Need to select payment provider (Adyen, PayPal, AsiaPay, GMO, EdenRed) at runtime based on customer region, payment method, or business rules
> - **Solution**: Register all providers in DI, inject `IEnumerable<IPaymentProvider>` and select at runtime, or use factory class with logic
> - **Result**: No inheritance hierarchy, each provider is a strategy, easy to add new providers without modifying existing code
> - **Technology**: DI enumerable injection, Strategy pattern, configuration-based selection
> - **Example**:
>   ```csharp
>   // Register all providers
>   services.AddTransient<AdyenProvider>();
>   services.AddTransient<PayPalProvider>();
>   services.AddTransient<AsiaPayProvider>();
>
>   // Factory selects provider
>   public class PaymentProviderFactory(IEnumerable<IPaymentProvider> providers)
>   {
>       public IPaymentProvider GetProvider(string region, string method) =>
>           providers.First(p => p.Supports(region, method));
>   }
>   ```

---

### 5. [Abstract Factory](5_AbstractFactory.cs)
**Problem**: Need to create families of related objects without specifying concrete classes
**Solution**: Interface for creating families of related objects

**When to Use**:
- System needs to be independent of object creation
- Families of related objects must be used together
- Want to enforce constraints on object combinations

**Modern .NET**:
```csharp
// Abstract factory
public interface IUIFactory
{
    IButton CreateButton();
    ITextBox CreateTextBox();
    ICheckBox CreateCheckBox();
}

// Concrete factories
public class DarkThemeFactory : IUIFactory
{
    public IButton CreateButton() => new DarkButton();
    public ITextBox CreateTextBox() => new DarkTextBox();
    public ICheckBox CreateCheckBox() => new DarkCheckBox();
}

public class LightThemeFactory : IUIFactory
{
    public IButton CreateButton() => new LightButton();
    public ITextBox CreateTextBox() => new LightTextBox();
    public ICheckBox CreateCheckBox() => new LightCheckBox();
}
```

**Real-World Examples**: UI themes, database providers, cloud platform abstractions

**Personal Usage**:
> **Multi-Cloud Infrastructure Abstraction**
>
> I've used this pattern to abstract infrastructure components across different cloud providers (Azure, AWS) ensuring consistent component families.
>
> - **Challenge**: Support multiple cloud providers without tight coupling to specific implementations
> - **Solution**: Abstract factory creates provider-specific component families (storage, messaging, databases)
> - **Result**: Switch providers with configuration change, test with mock factories, consistent component usage
> - **Technology**: Azure SDK, AWS SDK, interface abstractions
> - **Example**:
>   ```csharp
>   // Abstract factory interface
>   public interface ICloudInfrastructureFactory
>   {
>       ICloudStorage CreateStorage();
>       ICloudMessaging CreateMessaging();
>       ICloudDatabase CreateDatabase();
>   }
>
>   // Azure factory
>   public class AzureInfrastructureFactory : ICloudInfrastructureFactory
>   {
>       public ICloudStorage CreateStorage() => new AzureBlobStorage();
>       public ICloudMessaging CreateMessaging() => new AzureServiceBus();
>       public ICloudDatabase CreateDatabase() => new AzureCosmosDb();
>   }
>
>   // AWS factory
>   public class AwsInfrastructureFactory : ICloudInfrastructureFactory
>   {
>       public ICloudStorage CreateStorage() => new AwsS3Storage();
>       public ICloudMessaging CreateMessaging() => new AwsSqs();
>       public ICloudDatabase CreateDatabase() => new AwsDynamoDb();
>   }
>
>   // Register both factories
>   services.AddTransient<AzureInfrastructureFactory>();
>   services.AddTransient<AwsInfrastructureFactory>();
>
>   // Factory selector chooses at runtime
>   public class CloudInfrastructureSelector(
>       AzureInfrastructureFactory azureFactory,
>       AwsInfrastructureFactory awsFactory)
>   {
>       public ICloudInfrastructureFactory GetFactory(string region)
>       {
>           // Select based on runtime request data
>           return region.StartsWith("ap-") ? awsFactory : azureFactory;
>       }
>   }
>   ```

---

### 6. [Builder](6_Builder.cs)
**Problem**: Complex object construction with many optional parameters
**Solution**: Separate construction from representation using fluent builder interface

**When to Use**:
- Object has many optional parameters (telescoping constructor problem)
- Want readable, self-documenting construction code
- Need to construct object in multiple steps

**Modern .NET**:
```csharp
// Fluent builder
public class EmailMessageBuilder
{
    private string _to = string.Empty;
    private string _subject = string.Empty;
    private string _body = string.Empty;
    private List<string> _attachments = [];

    public EmailMessageBuilder To(string email)
    {
        _to = email;
        return this;
    }

    public EmailMessageBuilder WithSubject(string subject)
    {
        _subject = subject;
        return this;
    }

    public EmailMessageBuilder WithBody(string body)
    {
        _body = body;
        return this;
    }

    public EmailMessageBuilder AddAttachment(string filePath)
    {
        _attachments.Add(filePath);
        return this;
    }

    public EmailMessage Build()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(_to);
        ArgumentException.ThrowIfNullOrWhiteSpace(_subject);

        return new EmailMessage(_to, _subject, _body, _attachments);
    }
}

// Usage
var email = new EmailMessageBuilder()
    .To("customer@example.com")
    .WithSubject("Order Confirmation")
    .WithBody("Your order has been confirmed")
    .AddAttachment("invoice.pdf")
    .Build();
```

**Real-World Examples**: Email construction, HTTP requests, query builders

**Personal Usage**:
> **AutoFixture Test Data Construction**
>
> I use builder pattern extensively in unit tests with AutoFixture to create test objects with specific overrides.
>
> - **Challenge**: Need to create test objects with specific properties while leaving others randomized, avoid verbose test setup
> - **Solution**: AutoFixture's built-in builder pattern provides fluent API for test data construction
> - **Result**: Readable test setup, reduce test maintenance, only specify what matters for each test case
> - **Technology**: AutoFixture, xUnit, fluent test data builders
> - **Example**:
>   ```csharp
>   // AutoFixture builder pattern in tests
>   var fixture = new Fixture();
>
>   var order = fixture.Build<Order>()
>       .With(o => o.CustomerId, "CUST123")
>       .With(o => o.Status, OrderStatus.Pending)
>       .Create();
>   ```

---

### 7. [Prototype](7_Prototype.cs)
**Problem**: Object creation is expensive or complex, need to create copies
**Solution**: Clone existing objects instead of creating new instances

**When to Use**:
- Object creation is expensive (database load, network call)
- Want to avoid subclass explosion
- Objects differ only in state, not behavior

**Modern .NET**:
```csharp
// Prototype with ICloneable
public class OrderTemplate : ICloneable
{
    public string CustomerId { get; set; } = string.Empty;
    public List<OrderItem> Items { get; set; } = [];
    public ShippingAddress ShippingAddress { get; set; } = new();

    public object Clone()
    {
        // Deep copy
        return new OrderTemplate
        {
            CustomerId = CustomerId,
            Items = Items.Select(item => (OrderItem)item.Clone()).ToList(),
            ShippingAddress = (ShippingAddress)ShippingAddress.Clone()
        };
    }
}

// Usage
var template = LoadOrderTemplate();
var newOrder = (OrderTemplate)template.Clone();
newOrder.CustomerId = "CUST123";
```

**Real-World Examples**: Configuration templates, test data generation, document templates

**Personal Usage**:
> **Records with `with` Keyword**
>
> I use this pattern extensively when working with records and the `with` keyword. Records are immutable, so the `with` expression is the only way to create a cloned object with different values.
>
> - **Challenge**: Need to create modified copies of immutable records without breaking immutability guarantees
> - **Solution**: Use `with` keyword to create shallow copies with specific property changes
> - **Result**: Immutability preserved, concise syntax, compiler-verified property names, thread-safe by design
> - **Technology**: C# records, `with` expressions, immutable domain models
> - **Example**:
>   ```csharp
>   // Immutable record
>   public record Order(string CustomerId, OrderStatus Status, decimal Total);
>
>   // Create copy with modified properties using 'with'
>   var originalOrder = new Order("CUST123", OrderStatus.Pending, 100m);
>   var completedOrder = originalOrder with { Status = OrderStatus.Completed };
>
>   // Nested records
>   public record Address(string Street, string City, string PostalCode);
>   public record Customer(string Id, string Name, Address ShippingAddress);
>
>   var customer = new Customer("CUST123", "John", new Address("123 Main", "NYC", "10001"));
>   var updated = customer with
>   {
>       ShippingAddress = customer.ShippingAddress with { City = "Boston" }
>   };
>   ```

---

### 8. [Object Pool](8_ObjectPool.cs)
**Problem**: Object creation is expensive, need to reuse objects
**Solution**: Maintain pool of reusable objects instead of creating/destroying

**When to Use**:
- Object creation is expensive (database connections, threads)
- High frequency of object creation and destruction
- Objects can be reset and reused

**Modern .NET**:
```csharp
// Object pool
public class DatabaseConnectionPool
{
    private readonly ConcurrentBag<DbConnection> _pool = [];
    private readonly SemaphoreSlim _semaphore;
    private readonly int _maxSize;

    public DatabaseConnectionPool(int maxSize)
    {
        _maxSize = maxSize;
        _semaphore = new SemaphoreSlim(maxSize, maxSize);
    }

    public async Task<DbConnection> GetConnectionAsync()
    {
        await _semaphore.WaitAsync();

        if (_pool.TryTake(out var connection) && connection.State == ConnectionState.Open)
        {
            return connection;
        }

        return CreateNewConnection();
    }

    public void ReturnConnection(DbConnection connection)
    {
        if (connection.State == ConnectionState.Open && _pool.Count < _maxSize)
        {
            _pool.Add(connection);
        }
        else
        {
            connection.Dispose();
        }

        _semaphore.Release();
    }
}
```

**Real-World Examples**: Database connection pools, thread pools, memory buffers

**Personal Usage**:
> **Already Built Into .NET**
>
> I rarely implement custom object pools because .NET provides excellent built-in pooling for common scenarios.
>
> - **Why rarely used**: Built-in pools handle most scenarios (ObjectPool<T>, ArrayPool<T>, MemoryPool<T>, ADO.NET connection pooling)
> - **When I use custom pools**: High-frequency object creation in performance-critical paths (game servers, real-time systems)
> - **Technology**: Microsoft.Extensions.ObjectPool, System.Buffers.ArrayPool

---

### 9. [Dependency Injection](9_DependencyInjection.cs)
**Problem**: Classes tightly coupled to concrete implementations
**Solution**: Inject dependencies via constructor/property instead of creating them

**When to Use**:
- Want loose coupling between components
- Need testability (inject mocks)
- Want to swap implementations without changing code

**Modern .NET**:
```csharp
// Service interface
public interface IEmailService
{
    Task SendAsync(string to, string subject, string body);
}

// Implementation
public class EmailService(ILogger<EmailService> logger, IOptions<EmailSettings> settings)
    : IEmailService
{
    public async Task SendAsync(string to, string subject, string body)
    {
        logger.LogInformation("Sending email to {To}", to);
        // Send email logic
    }
}

// Registration (Program.cs)
builder.Services.AddSingleton<IEmailService, EmailService>();

// Usage in controller
public class OrderController(IEmailService emailService)
{
    public async Task<IActionResult> PlaceOrder(Order order)
    {
        await emailService.SendAsync(
            order.CustomerEmail,
            "Order Confirmation",
            $"Order {order.Id} confirmed");

        return Ok();
    }
}
```

**Real-World Examples**: Service registration, middleware, repository pattern

**Personal Usage**:
> **Foundation of All Modern .NET Applications**
>
> Dependency injection is the cornerstone of every production application I build. It's not optional—it's fundamental architecture.
>
> - **Challenge**: Maintainable, testable, loosely coupled services across large distributed systems
> - **Solution**: Constructor injection for required dependencies, configure services in DI container
> - **Result**: 100% unit-testable services, easy to swap implementations, clear dependency graph
> - **Technology**: Microsoft.Extensions.DependencyInjection, Autofac (legacy projects)
> - **Patterns used**: Constructor injection (required deps), options pattern (configuration), factory pattern (runtime resolution)

---

## 🏆 Pattern Combinations

### Factory Method + Dependency Injection
```
DI Container → Factory Method → Concrete Implementation
```
Factory methods registered in DI container for runtime type resolution.

### Builder + Dependency Injection
```
Builder (injected) → Fluent API → Validated Object
```
Builders as transient services constructing complex objects with DI-provided dependencies.

### Abstract Factory + Dependency Injection
```
DI resolves factory → Factory creates product family → Products share lifetime
```

---

## ⚠️ Common Pitfalls

1. **Singleton abuse** - Use DI container singleton lifetime instead of pattern
2. **Factory without DI** - Factories become service locators (anti-pattern)
3. **Builder without validation** - Allow invalid objects to be constructed
4. **Prototype shallow copy** - Forget to deep copy nested objects
5. **Custom pools when built-in exists** - Reinventing the wheel (use ArrayPool<T>)

---

[← Back to Main README](../../../README.md)
