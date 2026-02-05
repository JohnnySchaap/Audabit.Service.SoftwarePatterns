namespace Audabit.Service.SoftwarePatterns.App.Patterns._5_Messaging.Patterns;

// Outbox Pattern: Ensures that database changes and message publishing happen atomically, preventing message loss or inconsistency.
// In this example:
// - When an order is created, both the database change and the outgoing message are stored in the same transaction.
// - A background worker reads pending messages from the outbox table and publishes them to the message broker.
// - If publishing fails, the message remains in the outbox for retry, ensuring guaranteed delivery.
// - The key here is that the outbox table is in the same database as business data, so they're committed together.
//
// MODERN C# USAGE:
// In production .NET microservices:
// - Use Entity Framework Core with a dedicated OutboxMessage entity in your DbContext.
// - Implement a background service (IHostedService) to poll and publish messages.
// - Libraries like MassTransit provide built-in outbox pattern support with EF Core.
// - Azure Service Bus/RabbitMQ handle message delivery, while outbox ensures at-least-once semantics.
// - Combine with idempotency on the receiver side (Inbox pattern) for exactly-once processing.
//
// ATOMICITY EXAMPLE (Real EF Core code):
// using (var transaction = await dbContext.Database.BeginTransactionAsync())
// {
//     dbContext.Orders.Add(order);                    // Step 1: Insert order
//     dbContext.OutboxMessages.Add(outboxMessage);   // Step 2: Insert outbox message
//     await dbContext.SaveChangesAsync();             // COMMIT - both succeed or both fail
//     await transaction.CommitAsync();
// }
// If SaveChanges fails (constraint violation, deadlock, etc.), BOTH operations roll back.
// This prevents scenarios like: "Order saved but message lost" or "Message saved but order lost".
public static class Outbox
{
    public static void Run()
    {
        Console.WriteLine("Outbox Pattern: Ensuring atomic database and message publishing...");
        Console.WriteLine("Outbox Pattern: Prevents message loss by storing messages in database first.\n");

        var database = new OrderDatabase();
        var orderService = new OrderService(database);

        Console.WriteLine("Outbox Pattern: --- Creating Order (Transactional Write) ---");
        orderService.CreateOrder("ORD-001", "Laptop", 1500m);

        Console.WriteLine("\nOutbox Pattern: --- Background Worker Publishing Pending Messages ---");
        var outboxWorker = new OutboxWorker(database);
        outboxWorker.ProcessPendingMessages();

        Console.WriteLine("\nOutbox Pattern: --- Creating Another Order ---");
        orderService.CreateOrder("ORD-002", "Mouse", 25m);
        outboxWorker.ProcessPendingMessages();
    }
}

// Outbox message stored in database
public sealed class OutboxMessage
{
    public required string Id { get; init; }
    public required string EventType { get; init; }
    public required string Payload { get; init; }
    public DateTime CreatedAt { get; init; }
    public bool Published { get; set; }
}

// Business entity
public sealed class Order
{
    public required string OrderId { get; init; }
    public required string Product { get; init; }
    public decimal Amount { get; init; }
}

// Simulated database with outbox table
public sealed class OrderDatabase
{
    private readonly List<Order> _orders = [];
    private readonly List<OutboxMessage> _outboxMessages = [];

    public void SaveOrderWithOutboxMessage(Order order, OutboxMessage message)
    {
        Console.WriteLine($"Outbox Pattern: [Database] BEGIN TRANSACTION");
        Console.WriteLine($"Outbox Pattern: [Database] Saving order {order.OrderId} to Orders table");
        _orders.Add(order);
        Console.WriteLine($"Outbox Pattern: [Database] Saving message {message.Id} to Outbox table");
        _outboxMessages.Add(message);
        Console.WriteLine($"Outbox Pattern: [Database] COMMIT TRANSACTION");
        Console.WriteLine($"Outbox Pattern: [Database] Both order and outbox message saved atomically!");
        Console.WriteLine($"Outbox Pattern: [Database] CRITICAL: If either operation fails, BOTH get rolled back!");
        Console.WriteLine($"Outbox Pattern: [Database] This prevents: Order saved but message lost, OR message saved but order lost");
    }

    public List<OutboxMessage> GetPendingMessages()
    {
        return [.. _outboxMessages.Where(m => !m.Published)];
    }

    public void MarkMessageAsPublished(string messageId)
    {
        var message = _outboxMessages.FirstOrDefault(m => m.Id == messageId);
        if (message != null)
        {
            message.Published = true;
            Console.WriteLine($"Outbox Pattern: [Database] Marked message {messageId} as published");
        }
    }
}

// Message publisher (simulates message broker)
public sealed class MessagePublisher
{
    public static void Publish(OutboxMessage message)
    {
        Console.WriteLine($"Outbox Pattern: [MessageBroker] Publishing event '{message.EventType}'");
        Console.WriteLine($"Outbox Pattern: [MessageBroker] Payload: {message.Payload}");
        Console.WriteLine($"Outbox Pattern: [MessageBroker] Message successfully sent to broker!");
    }
}

// Business service using outbox pattern
// Note: publisher is injected but not used directly - OutboxWorker handles publishing
public sealed class OrderService(OrderDatabase database)
{
    public void CreateOrder(string orderId, string product, decimal amount)
    {
        Console.WriteLine($"Outbox Pattern: [OrderService] Creating order {orderId}");

        var order = new Order
        {
            OrderId = orderId,
            Product = product,
            Amount = amount
        };

        var outboxMessage = new OutboxMessage
        {
            Id = Guid.NewGuid().ToString(),
            EventType = "OrderCreated",
            Payload = $"{{\"orderId\":\"{orderId}\",\"product\":\"{product}\",\"amount\":{amount}}}",
            CreatedAt = DateTime.UtcNow,
            Published = false
        };

        // Both operations happen in same transaction - either both succeed or both fail
        database.SaveOrderWithOutboxMessage(order, outboxMessage);
    }
}

// Background worker that publishes pending messages
public sealed class OutboxWorker(OrderDatabase database)
{
    public void ProcessPendingMessages()
    {
        Console.WriteLine($"Outbox Pattern: [OutboxWorker] Checking for pending messages...");
        var pendingMessages = database.GetPendingMessages();

        if (pendingMessages.Count == 0)
        {
            Console.WriteLine($"Outbox Pattern: [OutboxWorker] No pending messages to publish");
            return;
        }

        Console.WriteLine($"Outbox Pattern: [OutboxWorker] Found {pendingMessages.Count} pending message(s)");

        foreach (var message in pendingMessages)
        {
            try
            {
                MessagePublisher.Publish(message);
                database.MarkMessageAsPublished(message.Id);
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Outbox Pattern: [OutboxWorker] Failed to publish message {message.Id}: {ex.Message}");
                Console.WriteLine($"Outbox Pattern: [OutboxWorker] Message will be retried later");
            }
        }
    }
}