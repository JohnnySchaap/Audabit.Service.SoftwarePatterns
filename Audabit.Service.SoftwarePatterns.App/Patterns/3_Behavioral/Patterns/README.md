# Behavioral Patterns

Communication patterns that define how objects interact and distribute responsibility, focusing on algorithms and assignment of responsibilities between objects.

## 📖 About Behavioral Patterns

Behavioral patterns characterize complex control flow and object communication:
- **Communication**: Define how objects interact and communicate
- **Responsibility**: Distribute work across collaborating objects
- **Algorithms**: Encapsulate varying behaviors and algorithms
- **Loose Coupling**: Reduce dependencies between communicating objects

---

## 🎯 Patterns

### 1. [Observer](1_Observer.cs)
**Problem**: Object state changes need to notify multiple dependent objects
**Solution**: Define one-to-many dependency so observers are notified automatically

**When to Use**:
- One object's change affects unknown number of others
- Want loose coupling between subject and observers
- Need broadcast communication

**Modern .NET**:
```csharp
// Built-in .NET events
public class Order
{
    public event EventHandler<OrderPlacedEventArgs>? OrderPlaced;

    public void PlaceOrder()
    {
        // Place order logic
        OnOrderPlaced(new OrderPlacedEventArgs(this));
    }

    protected virtual void OnOrderPlaced(OrderPlacedEventArgs e)
    {
        OrderPlaced?.Invoke(this, e);
    }
}

// Observers
public class EmailNotificationService
{
    public void Subscribe(Order order)
    {
        order.OrderPlaced += (sender, e) => SendEmail(e.Order);
    }

    private void SendEmail(Order order) { /* Send email */ }
}

// Modern alternative: IObservable<T> / IObserver<T>
public class OrderStream : IObservable<Order>
{
    private readonly List<IObserver<Order>> _observers = [];

    public IDisposable Subscribe(IObserver<Order> observer)
    {
        _observers.Add(observer);
        return new Unsubscriber(_observers, observer);
    }

    public void PublishOrder(Order order)
    {
        foreach (var observer in _observers)
        {
            observer.OnNext(order);
        }
    }
}
```

**Real-World Examples**: Event handling, notifications, pub/sub systems

**Personal Usage**:
> **Replaced by Message Broker in Production**
>
> I rarely use in-process Observer pattern in production because message brokers provide better reliability and scalability for event-driven architecture.
>
> - **Why not Observer**: In-process events do not survive crashes, cannot scale across processes, and have memory leak risks from forgotten unsubscriptions
> - **Alternative approach**: Azure Service Bus topics/subscriptions for reliable event distribution
> - **When you might need it**: Simple in-memory event notifications within a single process where reliability is not critical
> - **Technology**: Azure Service Bus for distributed events, built-in C# events for simple scenarios

---

### 2. [Strategy](2_Strategy.cs)
**Problem**: Need to switch between different algorithms at runtime
**Solution**: Define family of algorithms, encapsulate each, make them interchangeable

**When to Use**:
- Many related classes differ only in behavior
- Need different variants of algorithm
- Algorithm uses data clients shouldn't know about

**Modern .NET**:
```csharp
// Strategy interface
public interface IPricingStrategy
{
    decimal CalculatePrice(Order order);
}

// Concrete strategies
public class RegularPricingStrategy : IPricingStrategy
{
    public decimal CalculatePrice(Order order) =>
        order.Items.Sum(item => item.Price * item.Quantity);
}

public class BulkDiscountStrategy : IPricingStrategy
{
    public decimal CalculatePrice(Order order)
    {
        var total = order.Items.Sum(item => item.Price * item.Quantity);
        return total > 100 ? total * 0.9m : total; // 10% discount
    }
}

public class VipPricingStrategy : IPricingStrategy
{
    public decimal CalculatePrice(Order order)
    {
        var total = order.Items.Sum(item => item.Price * item.Quantity);
        return total * 0.85m; // 15% discount
    }
}

// Context - select strategy explicitly
public class OrderPricingService
{
    public decimal CalculateFinalPrice(Order order, string customerType)
    {
        IPricingStrategy strategy = customerType switch
        {
            "VIP" => new VipPricingStrategy(),
            "Bulk" => new BulkDiscountStrategy(),
            _ => new RegularPricingStrategy()
        };

        return strategy.CalculatePrice(order);
    }
}

// Usage in controller
public class OrdersController(OrderPricingService pricingService) : ControllerBase
{
    [HttpPost("calculate")]
    public IActionResult CalculatePrice(Order order)
    {
        var customerType = User.FindFirst("CustomerType")?.Value ?? "Regular";
        var total = pricingService.CalculateFinalPrice(order, customerType);
        return Ok(new { Total = total });
    }
}
```

**Real-World Examples**: Payment processing, sorting algorithms, compression strategies

**Personal Usage**:
> **Payment Provider Selection**
>
> I use strategy pattern to select payment providers (credit card, PayPal, Apple Pay) based on customer choice, keeping the selection logic explicit.
>
> - **Challenge**: Support multiple payment providers with different APIs and processing logic without complex DI resolution or hidden dependencies
> - **Solution**: Service methods accept payment method parameter, explicitly select and instantiate appropriate payment strategy based on customer's selected payment type
> - **Result**: Clear payment provider selection visible in code, easy to debug, straightforward testing with different payment methods, no DI magic
> - **Technology**: Simple switch expressions, explicit strategy instantiation, payment method parameter-based selection

---

### 3. [Command](3_Command.cs)
**Problem**: Need to parameterize objects with operations, queue requests, support undo
**Solution**: Encapsulate request as object with all necessary information

**When to Use**:
- Parameterize objects with operations
- Queue, log, or support undo of operations
- Support transactional behavior

**Modern .NET**:
```csharp
// Command interface
public interface ICommand
{
    Task ExecuteAsync();
    Task UndoAsync();
}

// Concrete command
public class PlaceOrderCommand(
    Order order,
    IOrderRepository repository,
    IEmailService emailService) : ICommand
{
    public async Task ExecuteAsync()
    {
        await repository.SaveAsync(order);
        await emailService.SendConfirmationAsync(order);
    }

    public async Task UndoAsync()
    {
        await repository.DeleteAsync(order.Id);
        await emailService.SendCancellationAsync(order);
    }
}

// Invoker
public class CommandInvoker
{
    private readonly Stack<ICommand> _executedCommands = new();

    public async Task ExecuteCommandAsync(ICommand command)
    {
        await command.ExecuteAsync();
        _executedCommands.Push(command);
    }

    public async Task UndoLastCommandAsync()
    {
        if (_executedCommands.TryPop(out var command))
        {
            await command.UndoAsync();
        }
    }
}
```

**Real-World Examples**: Task scheduling, undo/redo, macro recording

**Personal Usage**:
> **Not Used - Except for Saga Pattern**
>
> I do not use the Command pattern for typical operations because direct service methods are simpler and more explicit. The one exception is implementing the Saga pattern for distributed transactions where command objects with compensation logic are essential.
>
> - **Saga use case**: Coordinating multi-service workflows (order → payment → inventory → shipping) where each step needs compensating action if later steps fail
> - **Why Command works here**: Each saga step is a command with both Execute and Compensate methods, making rollback logic explicit and testable
> - **Technology**: NServiceBus sagas, MassTransit sagas, or custom saga orchestration with command objects

---

### 4. [Chain of Responsibility](4_ChainOfResponsibility.cs)
**Problem**: Multiple objects may handle request, avoid coupling sender to receivers
**Solution**: Chain handlers, pass request along until one handles it

**When to Use**:
- More than one object may handle request
- Handler set determined dynamically
- Want to issue request without specifying receiver

**Modern .NET**:
```csharp
// Handler interface
public interface IValidationHandler
{
    IValidationHandler? Next { get; set; }
    Task<ValidationResult> ValidateAsync(Order order);
}

// Base handler
public abstract class ValidationHandler : IValidationHandler
{
    public IValidationHandler? Next { get; set; }

    public virtual async Task<ValidationResult> ValidateAsync(Order order)
    {
        if (Next != null)
        {
            return await Next.ValidateAsync(order);
        }

        return ValidationResult.Success();
    }
}

// Concrete handlers
public class CustomerValidationHandler : ValidationHandler
{
    public override async Task<ValidationResult> ValidateAsync(Order order)
    {
        if (string.IsNullOrEmpty(order.CustomerId))
        {
            return ValidationResult.Failure("Customer ID required");
        }

        return await base.ValidateAsync(order);
    }
}

public class InventoryValidationHandler(IInventoryService inventory) : ValidationHandler
{
    public override async Task<ValidationResult> ValidateAsync(Order order)
    {
        foreach (var item in order.Items)
        {
            if (!await inventory.IsAvailableAsync(item.ProductId, item.Quantity))
            {
                return ValidationResult.Failure($"Product {item.ProductId} out of stock");
            }
        }

        return await base.ValidateAsync(order);
    }
}

// Setup chain
var customerValidator = new CustomerValidationHandler();
var inventoryValidator = new InventoryValidationHandler(inventoryService);

customerValidator.Next = inventoryValidator;

var result = await customerValidator.ValidateAsync(order);
```

**Real-World Examples**: Logging frameworks, request processing pipelines, validation chains

**Personal Usage**:
> **ASP.NET Core Middleware Pipeline**
>
> I use chain of responsibility daily via ASP.NET Core middleware pipeline—every HTTP request flows through a chain of middleware components in strict order.
>
> - **Challenge**: Process HTTP requests through multiple layers: correlation ID injection, security (API key validation), exception handling, request/response logging, validation—each step potentially short-circuits or enriches the request
> - **Solution**: Middleware pipeline is textbook chain of responsibility—each middleware decides to handle, enrich, or pass to next via `await next(context)`
> - **Typical pipeline order**:
>   1. Correlation ID middleware (adds X-Correlation-Id header for tracing)
>   2. Exception handling middleware (catches unhandled exceptions)
>   3. Security middleware (API key validation, authentication)
>   4. Logging middleware (logs request/response details)
>   5. Validation middleware (model state validation)
>   6. Endpoint execution
> - **Result**: Composable request processing, order-dependent behaviors (security before logging), reusable middleware components, clear separation of concerns
> - **Technology**: ASP.NET Core middleware, `IApplicationBuilder`, `RequestDelegate`, custom middleware classes

---

### 5. [Mediator](5_Mediator.cs)
**Problem**: Objects communicate in complex ways, creating tight coupling
**Solution**: Encapsulate object interactions in mediator object

**When to Use**:
- Objects communicate in complex but well-defined ways
- Reusing objects difficult due to many dependencies
- Behavior distributed between classes should be customizable

**Modern .NET**:
```csharp
// Using MediatR
public record PlaceOrderCommand(Order Order) : IRequest<OrderResult>;

public class PlaceOrderHandler(
    IOrderRepository orders,
    IInventoryService inventory,
    IPaymentService payment,
    IEmailService email) : IRequestHandler<PlaceOrderCommand, OrderResult>
{
    public async Task<OrderResult> Handle(
        PlaceOrderCommand command,
        CancellationToken cancellationToken)
    {
        // Mediator coordinates all interactions
        var order = command.Order;

        if (!await inventory.ReserveAsync(order.Items, cancellationToken))
        {
            return OrderResult.OutOfStock();
        }

        var paymentResult = await payment.ProcessAsync(order.Total, cancellationToken);
        if (!paymentResult.Success)
        {
            await inventory.ReleaseAsync(order.Items, cancellationToken);
            return OrderResult.PaymentFailed();
        }

        await orders.SaveAsync(order, cancellationToken);
        await email.SendConfirmationAsync(order.CustomerEmail, cancellationToken);

        return OrderResult.Success(order.Id);
    }
}

// Usage in controller
public class OrdersController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> PlaceOrder(Order order)
    {
        var result = await mediator.Send(new PlaceOrderCommand(order));
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
}
```

**Real-World Examples**: CQRS command/query handlers, UI dialog coordination, chat rooms

**Personal Usage**:
> **Service Orchestration via Facade**
>
> I use Facade pattern combined with dependency injection for service coordination rather than a mediator library, keeping dependencies explicit and call graphs clear.
>
> - **Challenge**: Coordinate complex workflows across multiple services without tight coupling or hidden dependencies
> - **Solution**: Facade services orchestrate workflows, dependency injection provides loose coupling, middleware handles cross-cutting concerns
> - **Result**: Explicit dependencies visible in constructors, clear call graph for debugging, no reflection overhead, easier to understand
> - **Technology**: ASP.NET Core DI, Facade pattern, middleware/filters for cross-cutting concerns

---

### 6. [Memento](6_Memento.cs)
**Problem**: Need to restore object to previous state without violating encapsulation
**Solution**: Capture and externalize object's internal state for later restoration

**When to Use**:
- Need to save/restore object state
- Direct interface would expose implementation
- Want undo/redo functionality

**Modern .NET**:
```csharp
// Memento
public record OrderMemento(
    string OrderId,
    List<OrderItem> Items,
    OrderStatus Status,
    DateTime Timestamp);

// Originator
public class Order
{
    public string Id { get; private set; } = string.Empty;
    public List<OrderItem> Items { get; private set; } = [];
    public OrderStatus Status { get; private set; }

    public OrderMemento SaveState()
    {
        return new OrderMemento(
            Id,
            new List<OrderItem>(Items), // Deep copy
            Status,
            DateTime.UtcNow);
    }

    public void RestoreState(OrderMemento memento)
    {
        Id = memento.OrderId;
        Items = new List<OrderItem>(memento.Items);
        Status = memento.Status;
    }
}

// Caretaker
public class OrderHistory
{
    private readonly Stack<OrderMemento> _history = new();

    public void Save(Order order)
    {
        _history.Push(order.SaveState());
    }

    public void Undo(Order order)
    {
        if (_history.TryPop(out var memento))
        {
            order.RestoreState(memento);
        }
    }
}
```

**Real-World Examples**: Undo/redo, transaction rollback, state snapshots

**Personal Usage**:
> **Event Sourcing for State Reconstruction**
>
> I use memento-like patterns in event sourcing where events represent state changes and can reconstruct object state.
>
> - **Challenge**: Need complete audit trail and ability to reconstruct order state at any point in time
> - **Solution**: Event sourcing stores state changes as events, reconstruct by replaying events
> - **Result**: Complete history, temporal queries, state at any point in time
> - **Technology**: Event Store, Marten, event replay

---

### 7. [State](7_State.cs)
**Problem**: Object behavior changes based on internal state
**Solution**: Encapsulate state-specific behavior in separate state classes

**When to Use**:
- Object behavior depends on state
- Large conditional statements based on state
- State transitions are complex

**Modern .NET**:
```csharp
// State interface
public interface IOrderState
{
    Task<IOrderState> ProcessPaymentAsync(Order order);
    Task<IOrderState> ShipAsync(Order order);
    Task<IOrderState> CancelAsync(Order order);
}

// Concrete states
public class PendingState : IOrderState
{
    public async Task<IOrderState> ProcessPaymentAsync(Order order)
    {
        await order.PaymentService.ProcessAsync(order.Total);
        order.Status = OrderStatus.Paid;
        return new PaidState();
    }

    public Task<IOrderState> ShipAsync(Order order)
    {
        throw new InvalidOperationException("Cannot ship unpaid order");
    }

    public async Task<IOrderState> CancelAsync(Order order)
    {
        order.Status = OrderStatus.Cancelled;
        return new CancelledState();
    }
}

public class PaidState : IOrderState
{
    public Task<IOrderState> ProcessPaymentAsync(Order order)
    {
        throw new InvalidOperationException("Already paid");
    }

    public async Task<IOrderState> ShipAsync(Order order)
    {
        await order.ShippingService.CreateShipmentAsync(order);
        order.Status = OrderStatus.Shipped;
        return new ShippedState();
    }

    public async Task<IOrderState> CancelAsync(Order order)
    {
        await order.PaymentService.RefundAsync(order.Total);
        order.Status = OrderStatus.Cancelled;
        return new CancelledState();
    }
}

// Context
public class Order
{
    private IOrderState _state = new PendingState();

    public async Task ProcessPaymentAsync()
    {
        _state = await _state.ProcessPaymentAsync(this);
    }

    public async Task ShipAsync()
    {
        _state = await _state.ShipAsync(this);
    }
}
```

**Real-World Examples**: Order workflows, connection states, document approval

**Personal Usage**:
> **Not Used - State Pattern**
>
> I do not use the State pattern in production because simple status enums and conditional logic are sufficient for my needs.

---

### 8. [Template Method](8_TemplateMethod.cs)
**Problem**: Multiple algorithms have same structure but different steps
**Solution**: Define algorithm skeleton in base class, let subclasses override specific steps

**When to Use**:
- Algorithms have invariant parts and variant parts
- Common behavior should be factored into common class
- Control subclass extensions

**Modern .NET**:
```csharp
// Template method
public abstract class DocumentProcessor
{
    // Template method defines skeleton
    public async Task ProcessDocumentAsync(Stream document)
    {
        var content = await ReadDocumentAsync(document);

        ValidateContent(content);

        var processedContent = await ProcessContentAsync(content);

        await SaveResultAsync(processedContent);

        await NotifyCompletionAsync();
    }

    // Steps to be implemented by subclasses
    protected abstract Task<string> ReadDocumentAsync(Stream document);
    protected abstract Task<string> ProcessContentAsync(string content);

    // Hook methods with default implementation
    protected virtual void ValidateContent(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            throw new ValidationException("Content cannot be empty");
        }
    }

    protected virtual async Task SaveResultAsync(string result)
    {
        await File.WriteAllTextAsync("result.txt", result);
    }

    protected virtual Task NotifyCompletionAsync()
    {
        Console.WriteLine("Processing complete");
        return Task.CompletedTask;
    }
}

// Concrete implementations
public class PdfDocumentProcessor : DocumentProcessor
{
    protected override async Task<string> ReadDocumentAsync(Stream document)
    {
        // PDF-specific reading logic
        return await Task.FromResult("PDF content");
    }

    protected override async Task<string> ProcessContentAsync(string content)
    {
        // PDF-specific processing
        return await Task.FromResult(content.ToUpper());
    }
}

public class WordDocumentProcessor : DocumentProcessor
{
    protected override async Task<string> ReadDocumentAsync(Stream document)
    {
        // Word-specific reading logic
        return await Task.FromResult("Word content");
    }

    protected override async Task<string> ProcessContentAsync(string content)
    {
        // Word-specific processing
        return await Task.FromResult(content.ToLower());
    }
}
```

**Real-World Examples**: Data import/export, document processing, test frameworks

**Personal Usage**:
> **Payment Provider Workflow**
>
> I use template method for payment processing where all providers follow the same workflow (init → auth → capture → refund) but each provider implements steps differently.
>
> - **Challenge**: Support multiple payment providers (credit card, PayPal, Apple Pay) with identical workflow structure but provider-specific implementation details for each step
> - **Solution**: Abstract base class defines template method with workflow steps, each payment provider inherits and implements provider-specific logic for init, auth, capture, and refund
> - **Result**: Consistent payment workflow across all providers, provider-specific logic isolated in subclasses, easy to add new payment providers by inheriting and implementing the four required steps
> - **Technology**: Abstract base classes, override pattern, async template methods for each workflow step

---

### 9. [Visitor](9_Visitor.cs)
**Problem**: Need to add operations to class hierarchy without modifying classes
**Solution**: Move operations into visitor class hierarchy

**When to Use**:
- Object structure contains many classes with different interfaces
- Many distinct operations need to be performed
- Classes rarely change, but operations change often

**Modern .NET**:
```csharp
// Element interface
public interface IOrderElement
{
    void Accept(IOrderVisitor visitor);
}

// Concrete elements
public class OrderItem : IOrderElement
{
    public string ProductId { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }

    public void Accept(IOrderVisitor visitor) => visitor.Visit(this);
}

public class ShippingInfo : IOrderElement
{
    public string Address { get; set; } = string.Empty;
    public decimal ShippingCost { get; set; }

    public void Accept(IOrderVisitor visitor) => visitor.Visit(this);
}

// Visitor interface
public interface IOrderVisitor
{
    void Visit(OrderItem item);
    void Visit(ShippingInfo shipping);
}

// Concrete visitors
public class PriceCalculatorVisitor : IOrderVisitor
{
    public decimal TotalPrice { get; private set; }

    public void Visit(OrderItem item)
    {
        TotalPrice += item.Price * item.Quantity;
    }

    public void Visit(ShippingInfo shipping)
    {
        TotalPrice += shipping.ShippingCost;
    }
}

public class TaxCalculatorVisitor : IOrderVisitor
{
    public decimal TotalTax { get; private set; }

    public void Visit(OrderItem item)
    {
        TotalTax += item.Price * item.Quantity * 0.08m; // 8% tax
    }

    public void Visit(ShippingInfo shipping)
    {
        // No tax on shipping
    }
}

// Usage
var order = new List<IOrderElement>
{
    new OrderItem { Price = 10, Quantity = 2 },
    new OrderItem { Price = 20, Quantity = 1 },
    new ShippingInfo { ShippingCost = 5 }
};

var priceCalculator = new PriceCalculatorVisitor();
var taxCalculator = new TaxCalculatorVisitor();

foreach (var element in order)
{
    element.Accept(priceCalculator);
    element.Accept(taxCalculator);
}

var total = priceCalculator.TotalPrice; // $45
var tax = taxCalculator.TotalTax; // $3.20
```

**Real-World Examples**: Compiler AST traversal, tax calculation, report generation

**Personal Usage**:
> **Rarely Used - Pattern Matching Preferred**
>
> I rarely use visitor pattern in production because C# pattern matching achieves similar goals more simply.
>
> - **Why not Visitor**: Pattern matching with switch expressions is more idiomatic in modern C#
> - **Alternative approach**: `switch` expression with type patterns
> - **When you might need it**: Truly complex operations on stable hierarchies (compiler design)

---

### 10. [Iterator](10_Iterator.cs)
**Problem**: Access elements of aggregate without exposing internal structure
**Solution**: Provide standard way to traverse collection

**When to Use**:
- Need to access contents without exposing internal structure
- Support multiple traversal methods
- Provide uniform interface for different structures

**Modern .NET**:
```csharp
// Built into .NET with IEnumerable<T> and yield
public class OrderRepository
{
    private readonly List<Order> _orders = [];

    // Iterator using yield return
    public IEnumerable<Order> GetPendingOrders()
    {
        foreach (var order in _orders)
        {
            if (order.Status == OrderStatus.Pending)
            {
                yield return order;
            }
        }
    }

    // Custom iterator for pagination
    public IEnumerable<List<Order>> GetOrderBatches(int batchSize)
    {
        for (int i = 0; i < _orders.Count; i += batchSize)
        {
            yield return _orders.Skip(i).Take(batchSize).ToList();
        }
    }

    // Async iterator (IAsyncEnumerable<T>)
    public async IAsyncEnumerable<Order> GetOrdersAsync()
    {
        foreach (var order in _orders)
        {
            // Simulate async loading
            await Task.Delay(10);
            yield return order;
        }
    }
}

// Usage
await foreach (var order in repository.GetOrdersAsync())
{
    Console.WriteLine(order.Id);
}
```

**Real-World Examples**: Collection traversal, streaming data, pagination

**Personal Usage**:
> **Built Into .NET**
>
> Iterator pattern is fundamental to .NET via IEnumerable<T>, yield return, and IAsyncEnumerable<T>.
>
> - **Daily usage**: LINQ, foreach loops, async streams
> - **Custom iterators**: Pagination, batching, filtered sequences
> - **Async iterators**: Streaming large datasets, real-time data processing
> - **Technology**: IEnumerable<T>, IAsyncEnumerable<T>, yield return, LINQ

---

## 🏆 Pattern Combinations

### Mediator + Command = CQRS
```
Controller → Mediator → Command Handler → Repository
```
MediatR combines both patterns for clean CQRS architecture.

### Strategy + Template Method = Flexible Algorithms
```
Template Method (workflow) uses Strategy (variant steps)
```

### Chain of Responsibility + Command = Pipeline
```
Commands flow through chain of handlers (ASP.NET middleware)
```

---

## ⚠️ Common Pitfalls

1. **Observer memory leaks** - Forget to unsubscribe from events
2. **Too many strategies** - Consider configuration over code
3. **Command bloat** - Do not create command for every operation
4. **State explosion** - Too many state classes, consider state machine library
5. **Visitor rigidity** - Adding new element types requires changing all visitors

---

[← Back to Main README](../../../README.md)
