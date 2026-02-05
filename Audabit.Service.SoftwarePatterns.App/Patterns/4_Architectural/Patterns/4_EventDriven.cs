namespace Audabit.Service.SoftwarePatterns.App.Patterns._4_Architectural.Patterns;

// Event-Driven Pattern: Uses events to trigger and communicate between decoupled services, where producers emit events and consumers react to them asynchronously.
// LEVEL: Hybrid pattern (code-level with in-process events, infrastructure-level with message buses like RabbitMQ/Azure Service Bus)
// In this example:
// - We have an event bus that coordinates communication between components.
// - Publishers emit events without knowing who will consume them.
// - Subscribers listen for specific event types and react accordingly.
// - The key here is loose coupling - publishers and subscribers don't have direct dependencies on each other.
//
// MODERN C# USAGE:
// Event-Driven Architecture is fundamental in modern distributed .NET applications:
// - Use Azure Service Bus, Azure Event Grid, or RabbitMQ for event messaging
// - .NET built-in events: public event EventHandler<OrderPlacedEventArgs>? OrderPlaced;
// - MediatR library for in-process event handling: await _mediator.Publish(new OrderPlacedEvent());
// - Mass Transit or NServiceBus for distributed event-driven systems
// - Event Sourcing often combined with event-driven architecture
// - Microservices frequently use event-driven patterns for inter-service communication
// - Use CloudEvents standard for interoperability across cloud providers
public static class EventDriven
{
    public static void Run()
    {
        Console.WriteLine("Event-Driven Pattern: Decoupling components through events...");
        Console.WriteLine("Event-Driven Pattern: Publishers emit events, subscribers react asynchronously.\n");

        // Event Bus coordinates all event communication
        var eventBus = new EventBus();

        // Register event subscribers
        _ = new InventoryService(eventBus);
        _ = new EmailNotificationService(eventBus);
        _ = new AnalyticsService(eventBus);

        // Publisher emits event
        var orderService = new OrderProcessingService(eventBus);

        Console.WriteLine("Event-Driven Pattern: --- Customer Places Order ---");
        orderService.PlaceOrder("ORD-789", "john.doe@example.com", 3);
    }
}

// ============================================
// EVENT BUS (Message Broker)
// ============================================
public class EventBus
{
    private readonly Dictionary<Type, List<Action<object>>> _subscribers = [];

    public void Subscribe<TEvent>(Action<TEvent> handler) where TEvent : class
    {
        var eventType = typeof(TEvent);

        if (!_subscribers.TryGetValue(eventType, out var value))
        {
            value = [];
            _subscribers[eventType] = value;
        }

        value.Add(e => handler((TEvent)e));
    }

    public void Publish<TEvent>(TEvent @event) where TEvent : class
    {
        var eventType = typeof(TEvent);
        var eventName = eventType.Name;

        Console.WriteLine($"\nEvent-Driven Pattern: [Event Bus] Publishing event: {eventName}");

        if (_subscribers.TryGetValue(eventType, out var handlers))
        {
            foreach (var handler in handlers)
            {
                handler(@event);
            }
        }
    }
}

// ============================================
// EVENTS (Domain Events)
// ============================================
public class OrderPlacedEvent
{
    public required string OrderId { get; init; }
    public required string CustomerEmail { get; init; }
    public required int Quantity { get; init; }
}

// ============================================
// PUBLISHER (Event Producer)
// ============================================
public class OrderProcessingService(EventBus eventBus)
{
    private readonly EventBus _eventBus = eventBus;

    public void PlaceOrder(string orderId, string customerEmail, int quantity)
    {
        Console.WriteLine($"Event-Driven Pattern: [Order Service] Processing order {orderId}");
        Console.WriteLine($"Event-Driven Pattern: [Order Service] Customer: {customerEmail}, Quantity: {quantity}");

        // Business logic here...

        // Publish event (fire and forget - doesn't know who's listening)
        var orderPlacedEvent = new OrderPlacedEvent
        {
            OrderId = orderId,
            CustomerEmail = customerEmail,
            Quantity = quantity
        };

        _eventBus.Publish(orderPlacedEvent);
    }
}

// ============================================
// SUBSCRIBERS (Event Consumers)
// ============================================
public class InventoryService
{
    public InventoryService(EventBus eventBus)
    {
        // Subscribe to OrderPlaced events
        eventBus.Subscribe<OrderPlacedEvent>(OnOrderPlaced);
    }

    private void OnOrderPlaced(OrderPlacedEvent @event)
    {
        Console.WriteLine($"Event-Driven Pattern: [Inventory Service] Received OrderPlacedEvent");
        Console.WriteLine($"Event-Driven Pattern: [Inventory Service] Reserving {@event.Quantity} items for order {@event.OrderId}");
    }
}

public class EmailNotificationService
{
    public EmailNotificationService(EventBus eventBus)
    {
        eventBus.Subscribe<OrderPlacedEvent>(OnOrderPlaced);
    }

    private void OnOrderPlaced(OrderPlacedEvent @event)
    {
        Console.WriteLine($"Event-Driven Pattern: [Email Service] Received OrderPlacedEvent");
        Console.WriteLine($"Event-Driven Pattern: [Email Service] Sending confirmation to {@event.CustomerEmail}");
    }
}

public class AnalyticsService
{
    public AnalyticsService(EventBus eventBus)
    {
        eventBus.Subscribe<OrderPlacedEvent>(OnOrderPlaced);
    }

    private void OnOrderPlaced(OrderPlacedEvent @event)
    {
        Console.WriteLine($"Event-Driven Pattern: [Analytics Service] Received OrderPlacedEvent");
        Console.WriteLine($"Event-Driven Pattern: [Analytics Service] Tracking order metrics for {@event.OrderId}");
    }
}