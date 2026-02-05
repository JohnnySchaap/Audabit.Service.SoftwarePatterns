# Architectural Patterns

Large-scale structural patterns that define the overall organization and high-level structure of software systems.

## 📖 About Architectural Patterns

Architectural patterns shape system structure and organization:
- **System Structure**: Define how system components are organized
- **Scalability**: Enable systems to handle growth in users, data, and complexity
- **Maintainability**: Facilitate long-term evolution and modification
- **Distribution**: Support distributed and microservices architectures

---

## 🎯 Patterns

### 1. [Layered Architecture](1_Layered.cs)
**Problem**: Need to organize code into logical groupings with clear dependencies
**Solution**: Organize system into horizontal layers, each with specific responsibility

**When to Use**:
- Traditional enterprise applications
- Clear separation of concerns needed
- Team organization by technical specialty

**Modern .NET**:
```csharp
// Presentation Layer
[ApiController]
[Route("api/[controller]")]
public class OrdersController(IOrderService orderService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateOrder(CreateOrderRequest request)
    {
        var result = await orderService.CreateOrderAsync(request);
        return Ok(result);
    }
}

// Business Logic Layer
public class OrderService(
    IOrderRepository repository,
    IPaymentService paymentService,
    IInventoryService inventoryService) : IOrderService
{
    public async Task<OrderResult> CreateOrderAsync(CreateOrderRequest request)
    {
        // Business logic
        if (!await inventoryService.CheckAvailabilityAsync(request.Items))
        {
            return OrderResult.OutOfStock();
        }

        var order = new Order { /* map from request */ };
        await repository.SaveAsync(order);

        await paymentService.ProcessPaymentAsync(order.Total);

        return OrderResult.Success(order.Id);
    }
}

// Data Access Layer
public class OrderRepository(ApplicationDbContext context) : IOrderRepository
{
    public async Task<Order> SaveAsync(Order order)
    {
        context.Orders.Add(order);
        await context.SaveChangesAsync();
        return order;
    }

    public async Task<Order?> GetByIdAsync(string id)
    {
        return await context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id);
    }
}
```

**Real-World Examples**: ASP.NET MVC applications, traditional web apps, desktop applications

**Personal Usage**:
> **Foundation of Services**
>
> I use layered architecture for service implementations.
>
> - **Challenge**: Build maintainable services with clear boundaries for future extraction
> - **Solution**: Strict layering (API → Application → Domain → Infrastructure) with vertical slicing by feature
> - **Result**: Clear separation, easily testable, features can be extracted to microservices later
> - **Technology**: ASP.NET Core, EF Core, clean architecture layers

---

### 2. [Hexagonal Architecture (Ports & Adapters)](2_Hexagonal.cs)
**Problem**: Business logic becomes coupled to external systems and frameworks
**Solution**: Isolate business logic with ports (interfaces) and adapters (implementations)

**When to Use**:
- Business logic must be framework-independent
- Multiple UIs or entry points
- Need to swap infrastructure easily

**Modern .NET**:
```csharp
// Domain (Core) - No external dependencies
public class Order
{
    public string Id { get; private set; } = Guid.NewGuid().ToString();
    public List<OrderItem> Items { get; private set; } = [];
    public OrderStatus Status { get; private set; }

    public void AddItem(OrderItem item)
    {
        Items.Add(item);
    }

    public decimal CalculateTotal() => Items.Sum(i => i.Price * i.Quantity);
}

// Port (Interface) - Defined by domain
public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(string id);
    Task SaveAsync(Order order);
}

// Adapter - Infrastructure implementation
public class SqlOrderRepository(ApplicationDbContext context) : IOrderRepository
{
    public async Task<Order?> GetByIdAsync(string id)
    {
        return await context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task SaveAsync(Order order)
    {
        context.Orders.Add(order);
        await context.SaveChangesAsync();
    }
}

// Alternative adapter - Different infrastructure
public class CosmosOrderRepository(CosmosClient client) : IOrderRepository
{
    private readonly Container _container =
        client.GetContainer("orders-db", "orders");

    public async Task<Order?> GetByIdAsync(string id)
    {
        try
        {
            var response = await _container.ReadItemAsync<Order>(id, new PartitionKey(id));
            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task SaveAsync(Order order)
    {
        await _container.UpsertItemAsync(order, new PartitionKey(order.Id));
    }
}

// DI Registration - Choose adapter
builder.Services.AddScoped<IOrderRepository, SqlOrderRepository>();
// OR
builder.Services.AddScoped<IOrderRepository, CosmosOrderRepository>();
```

**Real-World Examples**: Domain-driven design, testable systems, framework independence

**Personal Usage**:
> **Domain-Driven Design Foundation**
>
> I use hexagonal architecture for complex domains where business logic must be protected from infrastructure concerns.
>
> - **Challenge**: Business rules polluted with EF Core, Azure SDK, external API concerns
> - **Solution**: Domain defines ports (interfaces), infrastructure provides adapters
> - **Result**: Pure domain logic, easily testable with in-memory adapters, swap infrastructure without touching domain
> - **Technology**: Clean architecture, DDD, interface segregation
> - **Template**: [Audabit.Service.Template](https://github.com/JohnnySchaap/Audabit.Service.Template) - Production-ready service template with clean architecture implementation

---

### 3. [Microservices](3_Microservices.cs)
**Problem**: Monolithic applications difficult to scale, deploy, and maintain independently
**Solution**: Decompose application into small, independently deployable services

**When to Use**:
- Large teams needing independent deployment
- Different parts need different scaling
- Technology diversity required

**Modern .NET**:
```csharp
// Order Service - Independent microservice
[ApiController]
[Route("api/orders")]
public class OrdersController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateOrder(CreateOrderCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrder(string id)
    {
        var result = await mediator.Send(new GetOrderQuery(id));
        return result != null ? Ok(result) : NotFound();
    }
}

// Inventory Service - Separate microservice
[ApiController]
[Route("api/inventory")]
public class InventoryController(IInventoryRepository repository) : ControllerBase
{
    [HttpPost("reserve")]
    public async Task<IActionResult> ReserveItems(ReserveItemsRequest request)
    {
        var success = await repository.ReserveAsync(request.Items);
        return success ? Ok() : Conflict("Insufficient inventory");
    }
}

// Service-to-service communication via HTTP
public class InventoryHttpClient(HttpClient httpClient)
{
    public async Task<bool> ReserveItemsAsync(List<OrderItem> items)
    {
        var request = new ReserveItemsRequest { Items = items };
        var response = await httpClient.PostAsJsonAsync("api/inventory/reserve", request);
        return response.IsSuccessStatusCode;
    }
}

// Or via message broker
public class OrderCreatedEventHandler(IServiceBusSender sender)
    : INotificationHandler<OrderCreatedEvent>
{
    public async Task Handle(OrderCreatedEvent notification, CancellationToken ct)
    {
        var message = new ServiceBusMessage(JsonSerializer.Serialize(notification));
        await sender.SendMessageAsync(message, ct);
    }
}
```

**Real-World Examples**: Netflix, Amazon, Uber - any large-scale distributed system

**Personal Usage**:
> **Domino's Pizza Digital Platform**
>
> I built microservices architecture for separating order processing, inventory, payment, and delivery domains.
>
> - **Challenge**: Monolithic system couldn't scale independently, deployments risky, team bottlenecks
> - **Solution**: Extracted bounded contexts into microservices communicating via Service Bus
> - **Result**: Independent deployment, team autonomy, scale inventory separate from orders
> - **Technology**: ASP.NET Core, Azure Service Bus, Azure Functions, Cosmos DB

---

### 4. [Event-Driven Architecture](4_EventDriven.cs)
**Problem**: Services tightly coupled through synchronous calls
**Solution**: Services communicate via asynchronous events

**When to Use**:
- Loose coupling between services
- Eventual consistency acceptable
- Need to react to state changes

**Modern .NET**:
```csharp
// Event definition
public record OrderPlacedEvent(
    string OrderId,
    string CustomerId,
    List<OrderItem> Items,
    decimal Total,
    DateTime PlacedAt);

// Publisher
public class OrderService(IServiceBusSender sender)
{
    public async Task PlaceOrderAsync(Order order)
    {
        // Save order
        // ...

        // Publish event
        var orderPlaced = new OrderPlacedEvent(
            order.Id,
            order.CustomerId,
            order.Items,
            order.Total,
            DateTime.UtcNow);

        var message = new ServiceBusMessage(JsonSerializer.Serialize(orderPlaced))
        {
            Subject = nameof(OrderPlacedEvent)
        };

        await sender.SendMessageAsync(message);
    }
}

// Subscriber 1 - Email Service
public class EmailService(IServiceBusReceiver receiver)
{
    public async Task ProcessMessagesAsync()
    {
        await foreach (var message in receiver.ReceiveMessagesAsync())
        {
            var orderPlaced = JsonSerializer.Deserialize<OrderPlacedEvent>(message.Body);
            await SendOrderConfirmationEmailAsync(orderPlaced);
            await receiver.CompleteMessageAsync(message);
        }
    }
}

// Subscriber 2 - Inventory Service
public class InventoryService(IServiceBusReceiver receiver)
{
    public async Task ProcessMessagesAsync()
    {
        await foreach (var message in receiver.ReceiveMessagesAsync())
        {
            var orderPlaced = JsonSerializer.Deserialize<OrderPlacedEvent>(message.Body);
            await ReserveInventoryAsync(orderPlaced.Items);
            await receiver.CompleteMessageAsync(message);
        }
    }
}

// Azure Function subscriber
[Function("OrderPlacedHandler")]
public async Task Run(
    [ServiceBusTrigger("orders", "order-placed", Connection = "ServiceBus")]
    ServiceBusReceivedMessage message)
{
    var orderPlaced = JsonSerializer.Deserialize<OrderPlacedEvent>(message.Body);
    // Process event
}
```

**Real-World Examples**: Order processing, notifications, analytics, audit logs

**Personal Usage**:
> **Fire-and-Forget Communication**
>
> I use event-driven architecture when services do not need immediate response—for notifications, analytics, side effects where the publisher does not care about the outcome.
>
> - **Challenge**: Synchronous REST calls created cascading failures, tight coupling, deployment dependencies - even when caller didn't need the response
> - **Solution**: Services publish domain events to Service Bus for fire-and-forget scenarios (order confirmation emails, inventory updates, analytics); use HTTP when response is needed
> - **Result**: Loose coupling for async workflows, independent deployment, automatic retry, better resilience; synchronous HTTP for request-response patterns
> - **Technology**: Azure Service Bus topics/subscriptions for events, Azure Functions for subscribers, HTTP clients for request-response, MediatR domain events

---

### 5. [CQRS (Command Query Responsibility Segregation)](5_CQRS.cs)
**Problem**: Read and write operations have different requirements and complexity
**Solution**: Separate read models from write models

**When to Use**:
- Read and write workloads differ significantly
- Need different optimization strategies
- Complex domain logic on writes, simple queries on reads

**Modern .NET**:
```csharp
// Write Model - Commands
public record CreateOrderCommand(
    string CustomerId,
    List<OrderItem> Items) : IRequest<string>;

public class CreateOrderHandler(ApplicationDbContext context)
    : IRequestHandler<CreateOrderCommand, string>
{
    public async Task<string> Handle(CreateOrderCommand command, CancellationToken ct)
    {
        var order = new Order
        {
            Id = Guid.NewGuid().ToString(),
            CustomerId = command.CustomerId,
            Items = command.Items,
            Status = OrderStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        context.Orders.Add(order);
        await context.SaveChangesAsync(ct);

        return order.Id;
    }
}

// Read Model - Queries (denormalized for performance)
public record OrderSummary(
    string OrderId,
    string CustomerName,
    decimal Total,
    OrderStatus Status,
    DateTime CreatedAt);

public record GetOrderSummaryQuery(string OrderId) : IRequest<OrderSummary?>;

public class GetOrderSummaryHandler(IOrderReadRepository readRepo)
    : IRequestHandler<GetOrderSummaryQuery, OrderSummary?>
{
    public async Task<OrderSummary?> Handle(GetOrderSummaryQuery query, CancellationToken ct)
    {
        // Read from denormalized view optimized for queries
        return await readRepo.GetOrderSummaryAsync(query.OrderId);
    }
}

// Separate read repository (could be different database)
public interface IOrderReadRepository
{
    Task<OrderSummary?> GetOrderSummaryAsync(string orderId);
    Task<List<OrderSummary>> GetCustomerOrdersAsync(string customerId);
}

// Read model updated via event handler
public class OrderCreatedEventHandler(IOrderReadRepository readRepo)
    : INotificationHandler<OrderCreatedEvent>
{
    public async Task Handle(OrderCreatedEvent notification, CancellationToken ct)
    {
        // Update denormalized read model
        await readRepo.UpdateOrderSummaryAsync(notification.ToOrderSummary());
    }
}
```

**Real-World Examples**: High-traffic systems, reporting, audit trails, analytics

**Personal Usage**:
> **Domino's Pizza Reporting Dashboard**
>
> I use CQRS for complex reporting where write model is normalized but reads need denormalized aggregates.
>
> - **Challenge**: Real-time dashboard queries joining 10+ tables causing write-side performance issues
> - **Solution**: CQRS with separate read database (Cosmos DB) updated via change feed from SQL
> - **Result**: Fast denormalized reads, normalized writes, independent scaling
> - **Technology**: MediatR (commands/queries), SQL Server (write), Cosmos DB (read), Change Feed

---

### 6. [Event Sourcing](6_EventSourcing.cs)
**Problem**: Need complete audit trail and ability to reconstruct state
**Solution**: Store state changes as sequence of events instead of current state

**When to Use**:
- Complete audit trail required
- Need to reconstruct past states
- Temporal queries important

**Modern .NET**:
```csharp
// Domain events
public abstract record OrderEvent(string OrderId, DateTime OccurredAt);

public record OrderCreatedEvent(
    string OrderId,
    string CustomerId,
    List<OrderItem> Items,
    DateTime OccurredAt) : OrderEvent(OrderId, OccurredAt);

public record OrderPaidEvent(
    string OrderId,
    decimal Amount,
    string PaymentMethod,
    DateTime OccurredAt) : OrderEvent(OrderId, OccurredAt);

public record OrderShippedEvent(
    string OrderId,
    string TrackingNumber,
    DateTime OccurredAt) : OrderEvent(OrderId, OccurredAt);

// Aggregate that applies events
public class Order
{
    public string Id { get; private set; } = string.Empty;
    public string CustomerId { get; private set; } = string.Empty;
    public List<OrderItem> Items { get; private set; } = [];
    public OrderStatus Status { get; private set; }

    private readonly List<OrderEvent> _uncommittedEvents = [];

    // Reconstruct from events
    public static Order FromEvents(IEnumerable<OrderEvent> events)
    {
        var order = new Order();
        foreach (var @event in events)
        {
            order.Apply(@event);
        }
        return order;
    }

    // Apply event to update state
    private void Apply(OrderEvent @event)
    {
        switch (@event)
        {
            case OrderCreatedEvent created:
                Id = created.OrderId;
                CustomerId = created.CustomerId;
                Items = created.Items;
                Status = OrderStatus.Pending;
                break;

            case OrderPaidEvent paid:
                Status = OrderStatus.Paid;
                break;

            case OrderShippedEvent shipped:
                Status = OrderStatus.Shipped;
                break;
        }
    }

    // Command creates events
    public void MarkAsPaid(decimal amount, string paymentMethod)
    {
        var @event = new OrderPaidEvent(Id, amount, paymentMethod, DateTime.UtcNow);
        Apply(@event);
        _uncommittedEvents.Add(@event);
    }

    public IReadOnlyList<OrderEvent> GetUncommittedEvents() => _uncommittedEvents;
}

// Event store
public interface IEventStore
{
    Task AppendEventsAsync(string streamId, IEnumerable<OrderEvent> events);
    Task<List<OrderEvent>> GetEventsAsync(string streamId);
}

// Repository
public class OrderEventRepository(IEventStore eventStore)
{
    public async Task<Order> GetByIdAsync(string orderId)
    {
        var events = await eventStore.GetEventsAsync(orderId);
        return Order.FromEvents(events);
    }

    public async Task SaveAsync(Order order)
    {
        var events = order.GetUncommittedEvents();
        await eventStore.AppendEventsAsync(order.Id, events);
    }
}
```

**Real-World Examples**: Financial systems, healthcare records, regulatory compliance

**Personal Usage**:
> **Order History and Audit Trail**
>
> I use event sourcing for orders where complete audit trail and state reconstruction are regulatory requirements.
>
> - **Challenge**: Need to answer "what was order state at time X?" for customer disputes and audits
> - **Solution**: Store all order events, reconstruct state by replaying events
> - **Result**: Complete audit trail, temporal queries, source of truth for analytics
> - **Technology**: Marten (PostgreSQL event store), event-carried state transfer

---

### 7. [Saga Pattern](7_Saga.cs)
**Problem**: Distributed transactions across microservices are unreliable
**Solution**: Coordinate long-running transactions via choreography or orchestration

**When to Use**:
- Transaction spans multiple microservices
- Need eventual consistency with compensation
- Two-phase commit not viable

**Modern .NET**:
```csharp
// Orchestration-based Saga
public class OrderSaga(
    IOrderRepository orders,
    IInventoryService inventory,
    IPaymentService payment,
    IShippingService shipping,
    IServiceBusSender sender)
{
    public async Task<SagaResult> ProcessOrderAsync(Order order)
    {
        var sagaState = new OrderSagaState { OrderId = order.Id };

        try
        {
            // Step 1: Reserve inventory
            sagaState.InventoryReserved = await inventory.ReserveAsync(order.Items);
            if (!sagaState.InventoryReserved)
            {
                return SagaResult.Failed("Insufficient inventory");
            }

            // Step 2: Process payment
            sagaState.PaymentId = await payment.ProcessAsync(order.Total);
            if (sagaState.PaymentId == null)
            {
                await CompensateInventoryAsync(order.Items);
                return SagaResult.Failed("Payment failed");
            }

            // Step 3: Create shipment
            sagaState.ShipmentId = await shipping.CreateShipmentAsync(order);
            if (sagaState.ShipmentId == null)
            {
                await CompensatePaymentAsync(sagaState.PaymentId);
                await CompensateInventoryAsync(order.Items);
                return SagaResult.Failed("Shipping failed");
            }

            // All steps succeeded
            await orders.SaveAsync(order);
            return SagaResult.Success(order.Id);
        }
        catch (Exception ex)
        {
            // Compensate completed steps
            await CompensateAsync(sagaState);
            return SagaResult.Failed($"Saga failed: {ex.Message}");
        }
    }

    private async Task CompensateAsync(OrderSagaState state)
    {
        if (state.ShipmentId != null)
        {
            await shipping.CancelShipmentAsync(state.ShipmentId);
        }

        if (state.PaymentId != null)
        {
            await CompensatePaymentAsync(state.PaymentId);
        }

        if (state.InventoryReserved)
        {
            // Inventory compensation
        }
    }

    private async Task CompensatePaymentAsync(string paymentId)
    {
        await payment.RefundAsync(paymentId);
    }

    private async Task CompensateInventoryAsync(List<OrderItem> items)
    {
        await inventory.ReleaseAsync(items);
    }
}

// Choreography-based Saga (event-driven)
public class OrderCreatedEventHandler(
    IInventoryService inventory,
    IServiceBusSender sender) : INotificationHandler<OrderCreatedEvent>
{
    public async Task Handle(OrderCreatedEvent notification, CancellationToken ct)
    {
        var reserved = await inventory.ReserveAsync(notification.Items);

        var nextEvent = reserved
            ? new InventoryReservedEvent(notification.OrderId, notification.Items)
            : new InventoryReservationFailedEvent(notification.OrderId);

        await sender.PublishEventAsync(nextEvent, ct);
    }
}
```

**Real-World Examples**: E-commerce checkout, travel booking, order fulfillment

**Personal Usage**:
> **Order Fulfillment Workflow**
>
> I use saga pattern for order processing across inventory, payment, and shipping microservices.
>
> - **Challenge**: Order processing requires coordinating 3 services, partial failures must be compensated
> - **Solution**: Orchestration-based saga with compensation logic for each step
> - **Result**: Reliable distributed transactions, automatic rollback on failures
> - **Technology**: MassTransit state machine sagas, Azure Service Bus, compensation patterns

---

### 8. [API Gateway](8_ApiGateway.cs)
**Problem**: Clients need to call multiple microservices
**Solution**: Single entry point that routes requests to appropriate services

**When to Use**:
- Multiple microservices behind single endpoint
- Need authentication, rate limiting, logging centrally
- Backend-for-frontend pattern

**Modern .NET**:
```csharp
// Using YARP (Yet Another Reverse Proxy)
// appsettings.json
{
  "ReverseProxy": {
    "Routes": {
      "orders-route": {
        "ClusterId": "orders-cluster",
        "Match": {
          "Path": "/api/orders/{**catch-all}"
        },
        "Transforms": [
          { "PathPattern": "/api/orders/{**catch-all}" }
        ]
      },
      "inventory-route": {
        "ClusterId": "inventory-cluster",
        "Match": {
          "Path": "/api/inventory/{**catch-all}"
        }
      }
    },
    "Clusters": {
      "orders-cluster": {
        "Destinations": {
          "destination1": {
            "Address": "https://orders-service:5001"
          }
        }
      },
      "inventory-cluster": {
        "Destinations": {
          "destination1": {
            "Address": "https://inventory-service:5002"
          }
        }
      }
    }
  }
}

// Program.cs
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

// Add authentication, rate limiting, etc.
app.UseAuthentication();
app.UseRateLimiter();

app.MapReverseProxy();
app.Run();

// Custom transformation
public class AddCorrelationIdTransform : ITransformProvider
{
    public void Apply(TransformBuilderContext context)
    {
        context.AddRequestTransform(async transformContext =>
        {
            var correlationId = Guid.NewGuid().ToString();
            transformContext.ProxyRequest.Headers.Add("X-Correlation-Id", correlationId);
        });
    }
}
```

**Real-World Examples**: Mobile backends, aggregated APIs, microservices facades

**Personal Usage**:
> **GraphQL Layer for Customer-Facing APIs**
>
> I use GraphQL as an API gateway layer between customers and backend microservices to abstract internal architecture and provide flexible data fetching.
>
> - **Challenge**: Mobile app making 20+ REST calls to different microservices, overfetching data, exposing internal service boundaries to customers, authentication/logging duplicated
> - **Solution**: GraphQL layer as single entry point - customers query what they need, resolvers aggregate data from backend microservices, internal services stay hidden
> - **Result**: Single GraphQL endpoint for customers, clients fetch exactly what they need (no overfetching), backend microservices abstracted and protected from direct customer access, centralized authentication and rate limiting
> - **Technology**: GraphQL server (HotChocolate), resolver-based aggregation, JWT authentication, internal service-to-service calls hidden behind GraphQL schema

---

### 9. [Strangler Fig](9_StranglerFig.cs)
**Problem**: Need to migrate legacy system to new architecture without big-bang rewrite
**Solution**: Gradually replace functionality by routing to new system

**When to Use**:
- Migrating from legacy to new system
- Big-bang rewrite too risky
- Need incremental migration

**Modern .NET**:
```csharp
// API Gateway routing old vs new
public class StranglerFigMiddleware(RequestDelegate next, IConfiguration config)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value;

        // Check if feature migrated to new system
        if (IsFeatureMigrated(path))
        {
            // Route to new .NET service
            await ProxyToNewSystemAsync(context, "https://new-api:5000");
        }
        else
        {
            // Route to legacy system
            await ProxyToLegacySystemAsync(context, "http://legacy-app:8080");
        }
    }

    private bool IsFeatureMigrated(string? path)
    {
        // Configuration-driven feature flags
        var migratedFeatures = config.GetSection("MigratedFeatures").Get<List<string>>() ?? [];
        return migratedFeatures.Any(feature => path?.StartsWith(feature) ?? false);
    }

    private async Task ProxyToNewSystemAsync(HttpContext context, string baseUrl)
    {
        using var httpClient = new HttpClient();
        var requestMessage = CreateProxyRequest(context, baseUrl);
        var response = await httpClient.SendAsync(requestMessage);
        await CopyResponseAsync(response, context);
    }

    private async Task ProxyToLegacySystemAsync(HttpContext context, string baseUrl)
    {
        using var httpClient = new HttpClient();
        var requestMessage = CreateProxyRequest(context, baseUrl);
        var response = await httpClient.SendAsync(requestMessage);
        await CopyResponseAsync(response, context);
    }
}

// Configuration-based migration
// appsettings.json
{
  "MigratedFeatures": [
    "/api/orders",      // Migrated to new system
    "/api/customers"    // Migrated to new system
  ]
}

// Feature flag approach
public class FeatureMigrationService(IFeatureManager featureManager)
{
    public async Task<bool> IsFeatureMigratedAsync(string featureName)
    {
        return await featureManager.IsEnabledAsync($"Migrate_{featureName}");
    }
}
```

**Real-World Examples**: Legacy system modernization, platform migrations, framework upgrades

**Personal Usage**:
> **Legacy Backend Modernization to Kubernetes**
>
> I use strangler fig pattern to migrate legacy backend to modern .NET microservices running on Kubernetes.
>
> - **Challenge**: Old backend, complete rewrite would take 2 years and halt feature development
> - **Solution**: API Gateway routes by feature flag - migrated features go to new Kubernetes services, rest to legacy backend
> - **Result**: Gradual migration over 18 months, continuous delivery, reduced risk, modern microservices architecture on Kubernetes
> - **Technology**: YARP gateway, Azure App Configuration feature flags, Kubernetes, incremental migration, feature-by-feature extraction

---

## 🏆 Pattern Combinations

### CQRS + Event Sourcing + Event-Driven
```
Commands → Event Store → Events → Read Model Updates
```
Complete event-driven architecture with audit trail.

### Microservices + API Gateway + Saga
```
Client → Gateway → Microservices (coordinated via Saga)
```
Distributed system with orchestrated workflows.

### Hexagonal + DDD + CQRS
```
Ports (interfaces) → Commands/Queries → Domain → Adapters (infrastructure)
```
Clean architecture with domain focus.

---

## ⚠️ Common Pitfalls

1. **Distributed monolith** - Microservices with tight coupling defeat the purpose
2. **Event storm** - Too many events create complexity
3. **Saga complexity** - Long-running sagas become state machines nightmare
4. **Premature microservices** - Start with modular monolith, extract microservices when needed
5. **Eventual consistency surprise** - Users expect immediate consistency, requires UX consideration

---

[← Back to Main README](../../../README.md)
