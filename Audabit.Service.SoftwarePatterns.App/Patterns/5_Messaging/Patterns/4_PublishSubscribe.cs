namespace Audabit.Service.SoftwarePatterns.App.Patterns._5_Messaging.Patterns;

// Publish-Subscribe Pattern: Allows one publisher to send messages to multiple subscribers without knowing about them.
// In this example:
// - A price update publisher broadcasts product price changes.
// - Multiple subscribers (inventory, marketing, analytics) receive and process the same message independently.
// - Subscribers can join or leave without affecting the publisher or other subscribers.
// - The key here is decoupling - publisher doesn't know who receives the messages.
//
// MODERN C# USAGE:
// In production .NET applications:
// - Use Azure Service Bus Topics/Subscriptions for cloud pub/sub.
// - .NET IObservable<T>/IObserver<T> for in-process pub/sub.
// - System.Reactive (Rx.NET) provides powerful reactive programming: subject.Subscribe(x => Process(x)).
// - MassTransit provides message bus abstraction with pub/sub semantics.
// - Event-driven architectures heavily rely on pub/sub for loose coupling.
public static class PublishSubscribe
{
    public static void Run()
    {
        Console.WriteLine("Publish-Subscribe Pattern: Broadcasting messages to multiple subscribers...");
        Console.WriteLine("Publish-Subscribe Pattern: Decoupled communication - publishers don't know subscribers.\n");

        var messageBus = new PublishSubscribeMessageBus();

        Console.WriteLine("Publish-Subscribe Pattern: --- Registering Subscribers ---");
        var inventorySystem = new InventorySubscriber();
        var marketingSystem = new MarketingSubscriber();
        var analyticsSystem = new AnalyticsSubscriber();

        messageBus.Subscribe(inventorySystem);
        messageBus.Subscribe(marketingSystem);
        messageBus.Subscribe(analyticsSystem);

        var publisher = new PriceUpdatePublisher(messageBus);

        Console.WriteLine("\nPublish-Subscribe Pattern: --- Publisher Broadcasting Price Update ---");
        publisher.PublishPriceUpdate("LAPTOP-X1", 999.99m, 899.99m);

        Console.WriteLine("\nPublish-Subscribe Pattern: --- New Subscriber Joins ---");
        var emailNotificationSystem = new EmailNotificationSubscriber();
        messageBus.Subscribe(emailNotificationSystem);

        Console.WriteLine("\nPublish-Subscribe Pattern: --- Publisher Broadcasting Another Update ---");
        publisher.PublishPriceUpdate("MOUSE-A2", 29.99m, 24.99m);

        Console.WriteLine("\nPublish-Subscribe Pattern: --- Subscriber Leaves ---");
        messageBus.Unsubscribe(analyticsSystem);

        Console.WriteLine("\nPublish-Subscribe Pattern: --- Publisher Broadcasting Final Update ---");
        publisher.PublishPriceUpdate("KEYBOARD-Z", 79.99m, 69.99m);
    }
}

// Price update message
public sealed class PriceUpdateMessage
{
    public required string ProductId { get; init; }
    public decimal OldPrice { get; init; }
    public decimal NewPrice { get; init; }
    public DateTime UpdatedAt { get; init; }
}

// Subscriber interface
public interface IMessageSubscriber
{
    void OnMessageReceived(PriceUpdateMessage message);
}

// Concrete subscribers
public sealed class InventorySubscriber : IMessageSubscriber
{
    public void OnMessageReceived(PriceUpdateMessage message)
    {
        Console.WriteLine($"Publish-Subscribe Pattern: [InventorySystem] Received price update for {message.ProductId}");
        Console.WriteLine($"Publish-Subscribe Pattern: [InventorySystem] Updating inventory records: ${message.OldPrice} → ${message.NewPrice}");
        Console.WriteLine($"Publish-Subscribe Pattern: [InventorySystem] Recalculating stock value...");
    }
}

public sealed class MarketingSubscriber : IMessageSubscriber
{
    public void OnMessageReceived(PriceUpdateMessage message)
    {
        Console.WriteLine($"Publish-Subscribe Pattern: [MarketingSystem] Received price update for {message.ProductId}");
        var discount = ((message.OldPrice - message.NewPrice) / message.OldPrice) * 100;
        Console.WriteLine($"Publish-Subscribe Pattern: [MarketingSystem] Creating promotion: {discount:F0}% OFF!");
        Console.WriteLine($"Publish-Subscribe Pattern: [MarketingSystem] Updating website banners...");
    }
}

public sealed class AnalyticsSubscriber : IMessageSubscriber
{
    public void OnMessageReceived(PriceUpdateMessage message)
    {
        Console.WriteLine($"Publish-Subscribe Pattern: [AnalyticsSystem] Received price update for {message.ProductId}");
        Console.WriteLine($"Publish-Subscribe Pattern: [AnalyticsSystem] Logging price change event for analysis");
        Console.WriteLine($"Publish-Subscribe Pattern: [AnalyticsSystem] Updating pricing trend dashboard...");
    }
}

public sealed class EmailNotificationSubscriber : IMessageSubscriber
{
    public void OnMessageReceived(PriceUpdateMessage message)
    {
        Console.WriteLine($"Publish-Subscribe Pattern: [EmailNotification] Received price update for {message.ProductId}");
        Console.WriteLine($"Publish-Subscribe Pattern: [EmailNotification] Sending price drop alert to customers");
        Console.WriteLine($"Publish-Subscribe Pattern: [EmailNotification] New price: ${message.NewPrice} (was ${message.OldPrice})");
    }
}

// Message bus (topic/channel)
public sealed class PublishSubscribeMessageBus
{
    private readonly List<IMessageSubscriber> _subscribers = [];

    public void Subscribe(IMessageSubscriber subscriber)
    {
        _subscribers.Add(subscriber);
        var subscriberName = subscriber.GetType().Name.Replace("Subscriber", "");
        Console.WriteLine($"Publish-Subscribe Pattern: [MessageBus] {subscriberName} subscribed. Total subscribers: {_subscribers.Count}");
    }

    public void Unsubscribe(IMessageSubscriber subscriber)
    {
        _subscribers.Remove(subscriber);
        var subscriberName = subscriber.GetType().Name.Replace("Subscriber", "");
        Console.WriteLine($"Publish-Subscribe Pattern: [MessageBus] {subscriberName} unsubscribed. Total subscribers: {_subscribers.Count}");
    }

    public void Publish(PriceUpdateMessage message)
    {
        Console.WriteLine($"Publish-Subscribe Pattern: [MessageBus] Broadcasting message to {_subscribers.Count} subscriber(s)...");

        foreach (var subscriber in _subscribers)
        {
            subscriber.OnMessageReceived(message);
        }

        Console.WriteLine($"Publish-Subscribe Pattern: [MessageBus] Broadcast complete");
    }
}

// Publisher
public sealed class PriceUpdatePublisher(PublishSubscribeMessageBus messageBus)
{
    public void PublishPriceUpdate(string productId, decimal oldPrice, decimal newPrice)
    {
        Console.WriteLine($"Publish-Subscribe Pattern: [Publisher] Product {productId} price changed: ${oldPrice} → ${newPrice}");
        Console.WriteLine($"Publish-Subscribe Pattern: [Publisher] Publishing update to message bus...");

        var message = new PriceUpdateMessage
        {
            ProductId = productId,
            OldPrice = oldPrice,
            NewPrice = newPrice,
            UpdatedAt = DateTime.UtcNow
        };

        messageBus.Publish(message);
    }
}