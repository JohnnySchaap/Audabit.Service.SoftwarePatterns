namespace Audabit.Service.SoftwarePatterns.App.Patterns._5_Messaging.Patterns;

// Splitter Pattern: Breaks a composite message containing multiple items into individual messages for separate processing.
// In this example:
// - A bulk order message containing multiple products is received.
// - The splitter breaks it into individual product order messages.
// - Each individual message can then be routed to the appropriate handler/service.
// - The key here is that downstream systems don't need to handle complex composite messages.
//
// MODERN C# USAGE:
// In production .NET integration:
// - Azure Functions/Logic Apps provide built-in message splitting capabilities.
// - MassTransit consumers can split messages and publish individual ones using context.Publish().
// - Use LINQ to iterate and transform: messages.SelectMany(m => m.Items).ToList().
// - NServiceBus provides message routing and splitting through sagas and handlers.
// - Combine with message correlation ID to track which messages came from same original message.
public static class Splitter
{
    public static void Run()
    {
        Console.WriteLine("Splitter Pattern: Breaking composite message into individual messages...");
        Console.WriteLine("Splitter Pattern: Enables parallel processing of message parts.\n");

        var messageBus = new MessageBus();
        var splitter = new OrderSplitter(messageBus);

        Console.WriteLine("Splitter Pattern: --- Receiving Bulk Order Message ---");
        var bulkOrder = new BulkOrderMessage
        {
            OrderId = "BULK-001",
            CustomerId = "CUST-123",
            Items =
            [
                new OrderItem { ProductId = "PROD-A", Quantity = 5, Price = 10.00m },
                new OrderItem { ProductId = "PROD-B", Quantity = 2, Price = 25.00m },
                new OrderItem { ProductId = "PROD-C", Quantity = 10, Price = 5.00m }
            ]
        };

        Console.WriteLine($"Splitter Pattern: Bulk order contains {bulkOrder.Items.Count} items");
        splitter.ProcessBulkOrder(bulkOrder);

        Console.WriteLine("\nSplitter Pattern: --- Processing Individual Messages ---");
        messageBus.ProcessAllMessages();
    }
}

// Composite message containing multiple items
public sealed class BulkOrderMessage
{
    public required string OrderId { get; init; }
    public required string CustomerId { get; init; }
    public required List<OrderItem> Items { get; init; }
}

public sealed class OrderItem
{
    public required string ProductId { get; init; }
    public int Quantity { get; init; }
    public decimal Price { get; init; }
}

// Individual message after splitting
public sealed class IndividualOrderMessage
{
    public required string OrderId { get; init; }
    public required string CustomerId { get; init; }
    public required string ProductId { get; init; }
    public int Quantity { get; init; }
    public decimal Price { get; init; }
    public required string CorrelationId { get; init; } // Links back to original bulk order
}

// Message bus that handles individual messages
public sealed class MessageBus
{
    private readonly List<IndividualOrderMessage> _pendingMessages = [];

    public void Send(IndividualOrderMessage message)
    {
        Console.WriteLine($"Splitter Pattern: [MessageBus] Received individual message for product {message.ProductId}");
        _pendingMessages.Add(message);
    }

    public void ProcessAllMessages()
    {
        Console.WriteLine($"Splitter Pattern: [MessageBus] Processing {_pendingMessages.Count} individual messages");
        foreach (var message in _pendingMessages)
        {
            ProcessMessage(message);
        }
    }

    private static void ProcessMessage(IndividualOrderMessage message)
    {
        Console.WriteLine($"Splitter Pattern: [Handler] Processing order for product {message.ProductId}");
        Console.WriteLine($"Splitter Pattern: [Handler] Customer: {message.CustomerId}, Quantity: {message.Quantity}, Price: {message.Price:C}");
        Console.WriteLine($"Splitter Pattern: [Handler] Total: {message.Quantity * message.Price:C}");
        Console.WriteLine($"Splitter Pattern: [Handler] CorrelationId: {message.CorrelationId}");
    }
}

// Splitter component
public sealed class OrderSplitter(MessageBus messageBus)
{
    public void ProcessBulkOrder(BulkOrderMessage bulkOrder)
    {
        Console.WriteLine($"Splitter Pattern: [Splitter] Splitting bulk order {bulkOrder.OrderId} into individual messages");
        Console.WriteLine($"Splitter Pattern: [Splitter] Original message contains {bulkOrder.Items.Count} items");

        var correlationId = Guid.NewGuid().ToString();
        Console.WriteLine($"Splitter Pattern: [Splitter] Assigning correlation ID: {correlationId}");

        foreach (var item in bulkOrder.Items)
        {
            var individualMessage = new IndividualOrderMessage
            {
                OrderId = bulkOrder.OrderId,
                CustomerId = bulkOrder.CustomerId,
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                Price = item.Price,
                CorrelationId = correlationId
            };

            Console.WriteLine($"Splitter Pattern: [Splitter] Created message for product {item.ProductId}");
            messageBus.Send(individualMessage);
        }

        Console.WriteLine($"Splitter Pattern: [Splitter] Successfully split into {bulkOrder.Items.Count} individual messages");
    }
}