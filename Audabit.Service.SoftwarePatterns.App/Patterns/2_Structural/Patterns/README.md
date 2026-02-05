# Structural Patterns

Object composition patterns for building flexible, maintainable structures that define relationships between objects to form larger, more complex systems.

## 📖 About Structural Patterns

Structural patterns ease the design by identifying simple ways to realize relationships between entities:
- **Composition**: Combine objects into tree structures
- **Adaptation**: Make incompatible interfaces work together
- **Simplification**: Provide simplified interfaces to complex systems
- **Efficiency**: Share resources to support large numbers of objects

---

## 🎯 Patterns

### 1. [Adapter](1_Adapter.cs)
**Problem**: Incompatible interfaces prevent classes from working together
**Solution**: Create adapter class that translates one interface to another

**When to Use**:
- Want to use existing class with incompatible interface
- Integrating third-party libraries
- Need to create reusable class with unrelated interfaces

**Modern .NET**:
```csharp
// Third-party service with incompatible interface
public class LegacyPaymentService
{
    public bool ProcessPayment(string cardNumber, decimal amount)
    {
        // Legacy implementation
        return true;
    }
}

// Target interface your system expects
public interface IPaymentProcessor
{
    Task<PaymentResult> ProcessAsync(PaymentRequest request);
}

// Adapter
public class LegacyPaymentAdapter(LegacyPaymentService legacyService) : IPaymentProcessor
{
    public Task<PaymentResult> ProcessAsync(PaymentRequest request)
    {
        var success = legacyService.ProcessPayment(
            request.CardNumber,
            request.Amount);

        return Task.FromResult(new PaymentResult { Success = success });
    }
}
```

**Real-World Examples**: Third-party API wrappers, legacy system integration, database providers

**Personal Usage**:
> **POS System Integration**
>
> I use adapter pattern extensively when integrating with legacy POS systems that have outdated interfaces incompatible with modern async/await patterns.
>
> - **Challenge**: Legacy POS APIs use synchronous calls, callbacks, and custom data formats incompatible with modern async services
> - **Solution**: Adapter wraps legacy API with modern async interface, translates data formats, handles callbacks
> - **Result**: Modern codebase remains async throughout, POS integration isolated in adapter, easy to swap POS providers
> - **Technology**: Legacy COM interop, async Task wrappers, data transformation

---

### 2. [Bridge](2_Bridge.cs)
**Problem**: Abstraction and implementation tightly coupled, explosion of subclasses
**Solution**: Separate abstraction from implementation so they can vary independently

**When to Use**:
- Want to avoid permanent binding between abstraction and implementation
- Both abstraction and implementation should be extensible
- Changes in implementation shouldn't affect clients

**Modern .NET**:
```csharp
// Implementation interface
public interface IMessageSender
{
    Task SendAsync(string message);
}

// Concrete implementations
public class EmailSender : IMessageSender
{
    public Task SendAsync(string message) =>
        Task.CompletedTask; // Send email
}

public class SmsSender : IMessageSender
{
    public Task SendAsync(string message) =>
        Task.CompletedTask; // Send SMS
}

// Abstraction
public abstract class Notification(IMessageSender sender)
{
    protected IMessageSender Sender { get; } = sender;

    public abstract Task NotifyAsync(string message);
}

// Refined abstractions
public class UrgentNotification(IMessageSender sender) : Notification(sender)
{
    public override async Task NotifyAsync(string message)
    {
        await Sender.SendAsync($"[URGENT] {message}");
    }
}

public class StandardNotification(IMessageSender sender) : Notification(sender)
{
    public override async Task NotifyAsync(string message)
    {
        await Sender.SendAsync(message);
    }
}
```

**Real-World Examples**: UI rendering across platforms, notification systems, database drivers

**Personal Usage**:
> **Not Used - Prefer Strategy Pattern**
>
> I rarely use Bridge pattern in production because Strategy pattern combined with dependency injection achieves the same goal more simply in most .NET scenarios.
>
> - **Why not Bridge**: Adds complexity without clear benefit in modern .NET with DI
> - **Alternative approach**: Strategy pattern with injected implementation
> - **When you might need it**: Building frameworks where abstraction hierarchy is truly independent from implementation hierarchy

---

### 3. [Composite](3_Composite.cs)
**Problem**: Need to treat individual objects and compositions uniformly
**Solution**: Compose objects into tree structures to represent part-whole hierarchies

**When to Use**:
- Representing part-whole hierarchies
- Want clients to treat individual and composite objects uniformly
- Tree structures (file systems, organization charts, UI controls)

**Modern .NET**:
```csharp
// Component interface
public interface IMenuComponent
{
    string Name { get; }
    decimal GetPrice();
    void Display(int depth = 0);
}

// Leaf
public class MenuItem(string name, decimal price) : IMenuComponent
{
    public string Name => name;

    public decimal GetPrice() => price;

    public void Display(int depth = 0)
    {
        Console.WriteLine($"{new string(' ', depth)}- {Name}: ${price}");
    }
}

// Composite
public class MenuCategory(string name) : IMenuComponent
{
    private readonly List<IMenuComponent> _items = [];

    public string Name => name;

    public void Add(IMenuComponent item) => _items.Add(item);

    public decimal GetPrice() => _items.Sum(item => item.GetPrice());

    public void Display(int depth = 0)
    {
        Console.WriteLine($"{new string(' ', depth)}{Name}");
        foreach (var item in _items)
        {
            item.Display(depth + 2);
        }
    }
}

// Usage
var menu = new MenuCategory("Main Menu");
menu.Add(new MenuItem("Burger", 9.99m));

var drinks = new MenuCategory("Drinks");
drinks.Add(new MenuItem("Coke", 2.99m));
drinks.Add(new MenuItem("Water", 1.99m));

menu.Add(drinks);
menu.Display(); // Shows hierarchy
var total = menu.GetPrice(); // $14.97
```

**Real-World Examples**: File systems, organization hierarchies, UI component trees

**Personal Usage**:
> **Menu and Category Hierarchies**
>
> I use composite pattern for representing hierarchical product catalogs and menu structures where categories can contain items or other categories.
>
> - **Challenge**: Product catalogs have unlimited nesting levels (Department → Category → Subcategory → Product)
> - **Solution**: Composite pattern treats items and containers uniformly for pricing, display, and searching
> - **Result**: Flexible hierarchy, uniform interface for operations, easy to add new nesting levels
> - **Technology**: EF Core navigation properties, recursive queries, JSON serialization

---

### 4. [Decorator](4_Decorator.cs)
**Problem**: Need to add responsibilities to objects dynamically without affecting others
**Solution**: Wrap objects with decorator classes that add behavior

**When to Use**:
- Add responsibilities to individual objects dynamically
- Responsibilities can be withdrawn
- Extension by subclassing is impractical

**Modern .NET**:
```csharp
// Component interface
public interface IDataRepository
{
    Task<string> GetDataAsync(string key);
    Task SaveDataAsync(string key, string value);
}

// Concrete component
public class DatabaseRepository : IDataRepository
{
    public async Task<string> GetDataAsync(string key)
    {
        // Database lookup
        await Task.CompletedTask;
        return "data from database";
    }

    public async Task SaveDataAsync(string key, string value)
    {
        // Save to database
        await Task.CompletedTask;
    }
}

// Decorator - adds caching
public class CachedRepository(IDataRepository repository, IMemoryCache cache)
    : IDataRepository
{
    public async Task<string> GetDataAsync(string key)
    {
        if (cache.TryGetValue(key, out string? cached))
        {
            return cached!;
        }

        var data = await repository.GetDataAsync(key);
        cache.Set(key, data, TimeSpan.FromMinutes(5));
        return data;
    }

    public async Task SaveDataAsync(string key, string value)
    {
        await repository.SaveDataAsync(key, value);
        cache.Remove(key); // Invalidate cache
    }
}

// Decorator - adds logging
public class LoggedRepository(IDataRepository repository, ILogger<LoggedRepository> logger)
    : IDataRepository
{
    public async Task<string> GetDataAsync(string key)
    {
        logger.LogInformation("Getting data for key: {Key}", key);
        return await repository.GetDataAsync(key);
    }

    public async Task SaveDataAsync(string key, string value)
    {
        logger.LogInformation("Saving data for key: {Key}", key);
        await repository.SaveDataAsync(key, value);
    }
}

// DI Registration - decorators compose
builder.Services.AddScoped<IDataRepository, DatabaseRepository>();
builder.Services.Decorate<IDataRepository, CachedRepository>();
builder.Services.Decorate<IDataRepository, LoggedRepository>();
```

**Real-World Examples**: Logging, caching, compression, encryption

**Personal Usage**:
> **Repository Caching and Logging**
>
> I use decorator pattern extensively for adding cross-cutting concerns (caching, logging, retry) to repository and service classes without modifying core logic.
>
> - **Challenge**: Add caching, logging, and retry logic to repositories without polluting business logic
> - **Solution**: Decorator stack wraps core repository with layered concerns
> - **Result**: Clean separation of concerns, composable behaviors, testable in isolation
> - **Technology**: Scrutor (DI decoration), IMemoryCache, ILogger, Polly (retry)

---

### 5. [Facade](5_Facade.cs)
**Problem**: Complex subsystem with many interdependent classes
**Solution**: Provide simplified unified interface to complex subsystem

**When to Use**:
- Want simple interface to complex subsystem
- Decouple subsystem from clients and other subsystems
- Layering subsystems

**Modern .NET**:
```csharp
// Complex subsystems
public class InventoryService
{
    public bool CheckStock(string productId) => true;
    public void ReserveStock(string productId, int quantity) { }
}

public class PaymentService
{
    public Task<string> AuthorizeAsync(decimal amount) =>
        Task.FromResult("auth-123");
    public Task<bool> CaptureAsync(string authId) => Task.FromResult(true);
}

public class ShippingService
{
    public Task<string> CreateShipmentAsync(Address address) =>
        Task.FromResult("ship-456");
}

public class NotificationService
{
    public Task SendConfirmationAsync(string email, string orderId) =>
        Task.CompletedTask;
}

// Facade - simplified interface
public class OrderFacade(
    InventoryService inventory,
    PaymentService payment,
    ShippingService shipping,
    NotificationService notifications)
{
    public async Task<OrderResult> PlaceOrderAsync(OrderRequest request)
    {
        // Simplified workflow coordinating multiple subsystems
        if (!inventory.CheckStock(request.ProductId))
        {
            return OrderResult.OutOfStock();
        }

        inventory.ReserveStock(request.ProductId, request.Quantity);

        var authId = await payment.AuthorizeAsync(request.Total);
        var paymentSuccess = await payment.CaptureAsync(authId);

        if (!paymentSuccess)
        {
            return OrderResult.PaymentFailed();
        }

        var shipmentId = await shipping.CreateShipmentAsync(request.Address);
        await notifications.SendConfirmationAsync(request.Email, shipmentId);

        return OrderResult.Success(shipmentId);
    }
}

// Client code is simple
public class OrderController(OrderFacade orderFacade)
{
    public async Task<IActionResult> PlaceOrder(OrderRequest request)
    {
        var result = await orderFacade.PlaceOrderAsync(request);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
}
```

**Real-World Examples**: API aggregation, workflow coordination, subsystem simplification

**Personal Usage**:
> **Order Processing Orchestration**
>
> I use facade pattern to simplify complex multi-step order processing that coordinates inventory, payment, shipping, and notifications.
>
> - **Challenge**: Order placement involves 10+ steps across multiple services (validation, inventory, payment, shipping, email)
> - **Solution**: OrderFacade provides simple `PlaceOrderAsync` method coordinating all subsystems
> - **Result**: Controllers remain simple, complex orchestration centralized, easy to test workflow
> - **Technology**: DI-injected services, async coordination, error handling

---

### 6. [Flyweight](6_Flyweight.cs)
**Problem**: Large number of similar objects consume excessive memory
**Solution**: Share common state between multiple objects instead of storing in each

**When to Use**:
- Application uses large number of objects
- Storage costs are high due to quantity
- Most object state can be extrinsic (shared)

**Modern .NET**:
```csharp
// Flyweight - shared immutable state
public record ProductInfo(string Name, string Description, string ImageUrl);

// Flyweight factory
public class ProductInfoFactory
{
    private readonly ConcurrentDictionary<string, ProductInfo> _cache = new();

    public ProductInfo GetProductInfo(string productId)
    {
        return _cache.GetOrAdd(productId, id =>
        {
            // Load from database once, then cache
            return new ProductInfo(
                Name: "Product " + id,
                Description: "Description for " + id,
                ImageUrl: $"/images/{id}.jpg"
            );
        });
    }
}

// Context object - unique per instance
public class OrderItem
{
    public string ProductId { get; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }

    // Flyweight - shared across all orders for this product
    private readonly ProductInfo _productInfo;

    public OrderItem(string productId, ProductInfoFactory factory)
    {
        ProductId = productId;
        _productInfo = factory.GetProductInfo(productId);
    }

    public string ProductName => _productInfo.Name;
    public string ProductDescription => _productInfo.Description;
}
```

**Real-World Examples**: Text editors (character objects), game engines (sprite sharing), caching

**Personal Usage**:
> **Already Handled by Caching**
>
> I rarely implement explicit flyweight pattern because modern caching (IMemoryCache, Redis) achieves the same goal more flexibly.
>
> - **Why not Flyweight**: IMemoryCache provides the same sharing with better eviction policies
> - **When I use caching instead**: Product catalogs, configuration data, reference data
> - **Technology**: IMemoryCache, distributed caching (Redis), EF Core query caching

---

### 7. [Proxy](7_Proxy.cs)
**Problem**: Need to control access to object or add functionality without changing it
**Solution**: Provide surrogate object that controls access to another object

**When to Use**:
- Lazy loading (virtual proxy)
- Access control (protection proxy)
- Remote object access (remote proxy)
- Logging, caching (smart proxy)

**Modern .NET**:
```csharp
// Subject interface
public interface IDocumentService
{
    Task<Document> GetDocumentAsync(string id);
}

// Real subject
public class DocumentService : IDocumentService
{
    public async Task<Document> GetDocumentAsync(string id)
    {
        // Expensive database/API call
        await Task.Delay(100);
        return new Document { Id = id, Content = "Document content" };
    }
}

// Proxy with caching
public class CachedDocumentProxy(
    IDocumentService documentService,
    IMemoryCache cache) : IDocumentService
{
    public async Task<Document> GetDocumentAsync(string id)
    {
        var cacheKey = $"doc_{id}";

        if (cache.TryGetValue(cacheKey, out Document? cached))
        {
            return cached!;
        }

        var document = await documentService.GetDocumentAsync(id);
        cache.Set(cacheKey, document, TimeSpan.FromMinutes(10));

        return document;
    }
}
```

**Real-World Examples**: Lazy loading, access control, logging, caching

**Personal Usage**:
> **Not Used - Prefer Explicit Loading**
>
> I rarely use proxy pattern in production because explicit loading strategies are more predictable and avoid common performance pitfalls.
>
> - **Why not Proxy**: Lazy loading proxies can cause N+1 query problems and unexpected database calls
> - **Alternative approach**: Explicit eager loading with `.Include()` or projection with `.Select()`
> - **When you might need it**: Legacy codebases with complex navigation graphs where refactoring is impractical

---

### 8. [Marker Interface](8_MarkerInterface.cs)
**Problem**: Need to mark classes for special treatment without adding behavior
**Solution**: Empty interface that acts as metadata for runtime checks

**When to Use**:
- Mark classes for special processing
- Runtime type checking for specific behaviors
- Serialization markers, security markers

**Modern .NET**:
```csharp
// Marker interface
public interface ISoftDeletable
{
    bool IsDeleted { get; set; }
    DateTime? DeletedAt { get; set; }
}

// Entity implementing marker
public class Order : ISoftDeletable
{
    public int Id { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
}

// Generic repository that handles soft deletes
public class Repository<T>(DbContext context) where T : class
{
    public void Delete(T entity)
    {
        if (entity is ISoftDeletable softDeletable)
        {
            softDeletable.IsDeleted = true;
            softDeletable.DeletedAt = DateTime.UtcNow;
            context.Update(entity);
        }
        else
        {
            context.Remove(entity);
        }
    }

    public IQueryable<T> GetAll()
    {
        var query = context.Set<T>().AsQueryable();

        if (typeof(ISoftDeletable).IsAssignableFrom(typeof(T)))
        {
            query = query.Where(e => !((ISoftDeletable)e).IsDeleted);
        }

        return query;
    }
}
```

**Real-World Examples**: Serialization markers, audit markers, soft delete markers

**Personal Usage**:
> **API Security and Data Protection**
>
> I use marker interfaces extensively for API security and sensitive data protection across the application.
>
> - **Challenge**: Identify endpoints that do not require API key authentication and properties containing sensitive data that should be masked in logs/serialization
> - **Solution**: Marker interfaces ([NotApiKeyProtected] attribute for endpoints, [SensitiveData] for properties) enable middleware and serialization logic to automatically apply security policies
> - **Result**: Consistent security enforcement across endpoints, automatic masking of secrets and PII in logs, centralized security policy
> - **Technology**: ASP.NET Core middleware (API key validation), custom JSON converters (property masking), reflection-based attribute scanning

---

### 9. [Pipes and Filters](9_PipesAndFilters.cs)
**Problem**: Complex processing requires multiple independent steps
**Solution**: Chain processing components (filters) connected by data conduits (pipes)

**When to Use**:
- Complex data processing in stages
- Each stage is independent
- Want reusable, composable processing steps

**Modern .NET**:
```csharp
// Filter interface
public interface IDataFilter<T>
{
    Task<T> ProcessAsync(T data);
}

// Concrete filters
public class ValidationFilter : IDataFilter<OrderData>
{
    public Task<OrderData> ProcessAsync(OrderData data)
    {
        if (string.IsNullOrEmpty(data.CustomerId))
            throw new ValidationException("CustomerId required");

        return Task.FromResult(data);
    }
}

public class EnrichmentFilter(ICustomerService customerService) : IDataFilter<OrderData>
{
    public async Task<OrderData> ProcessAsync(OrderData data)
    {
        var customer = await customerService.GetCustomerAsync(data.CustomerId);
        data.CustomerName = customer.Name;
        data.CustomerEmail = customer.Email;
        return data;
    }
}

public class PricingFilter(IPricingService pricing) : IDataFilter<OrderData>
{
    public async Task<OrderData> ProcessAsync(OrderData data)
    {
        data.Total = await pricing.CalculateTotalAsync(data.Items);
        return data;
    }
}

// Pipeline
public class OrderProcessingPipeline(IEnumerable<IDataFilter<OrderData>> filters)
{
    public async Task<OrderData> ProcessAsync(OrderData data)
    {
        var current = data;

        foreach (var filter in filters)
        {
            current = await filter.ProcessAsync(current);
        }

        return current;
    }
}

// DI Registration - order matters
builder.Services.AddScoped<IDataFilter<OrderData>, ValidationFilter>();
builder.Services.AddScoped<IDataFilter<OrderData>, EnrichmentFilter>();
builder.Services.AddScoped<IDataFilter<OrderData>, PricingFilter>();
builder.Services.AddScoped<OrderProcessingPipeline>();
```

**Real-World Examples**: Data processing pipelines, ASP.NET Core middleware, stream processing

**Personal Usage**:
> **Order Validation and Enrichment Pipeline**
>
> I use pipes and filters extensively for multi-stage data processing where each stage is independent and reusable.
>
> - **Challenge**: Order processing requires validation, enrichment, pricing calculation, tax calculation in sequence
> - **Solution**: Pipeline of filters, each handling one concern, composed via DI
> - **Result**: Testable stages in isolation, reusable filters, easy to add/remove stages
> - **Technology**: DI-based pipelines, async filters, ASP.NET Core middleware pattern

---

## 🏆 Pattern Combinations

### Decorator + Proxy = Layered Behavior
```
Client → Proxy (caching) → Decorator (logging) → Real Object
```

### Composite + Decorator = Hierarchical Enhancement
```
Composite tree where decorators can wrap any node
```

### Facade + Adapter = Simplified Integration
```
Facade coordinates multiple Adapters to external systems
```

---

## ⚠️ Common Pitfalls

1. **Adapter as translator** - Do not put business logic in adapters, only translation
2. **Decorator order matters** - Logging before caching gives different results than caching before logging
3. **Facade becomes God Object** - Keep facades focused on single workflow
4. **Flyweight premature optimization** - Use caching first, flyweight only for extreme cases
5. **Too many proxy layers** - Each layer adds overhead, use sparingly

---

[← Back to Main README](../../../README.md)
