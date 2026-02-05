namespace Audabit.Service.SoftwarePatterns.App.Patterns._5_Messaging.Patterns;

// Message Router Pattern: Routes incoming messages to different channels/destinations based on routing rules.
// In this example:
// - Messages arrive at a central router from various sources.
// - The router examines message properties (type, priority, region) and applies routing rules.
// - Messages are sent to appropriate destination channels based on the rules.
// - The key here is that routing logic is centralized and can be changed without modifying producers/consumers.
//
// MODERN C# USAGE:
// In production .NET integration:
// - Azure Service Bus provides built-in topic filters and subscription rules.
// - MassTransit supports routing through send topologies and message conventions.
// - Use pattern matching in C# (switch expressions) for clean routing logic.
// - RabbitMQ routing keys and exchanges provide declarative message routing.
// - Combine with Content-Based Router for more sophisticated routing based on message payload.
public static class MessageRouter
{
    public static void Run()
    {
        Console.WriteLine("Message Router Pattern: Routing messages to appropriate channels...");
        Console.WriteLine("Message Router Pattern: Centralized routing logic based on message properties.\n");

        Console.WriteLine("Message Router Pattern: --- Routing Different Message Types ---\n");

        Console.WriteLine("Message Router Pattern: --- High Priority Order ---");
        var message1 = new RoutableMessage
        {
            Id = "MSG-001",
            Type = "Order",
            Priority = "High",
            Region = "US",
            Payload = "Urgent laptop order"
        };
        CentralMessageRouter.Route(message1);

        Console.WriteLine("\nMessage Router Pattern: --- Normal Priority Customer Query ---");
        var message2 = new RoutableMessage
        {
            Id = "MSG-002",
            Type = "CustomerQuery",
            Priority = "Normal",
            Region = "EU",
            Payload = "Question about shipping"
        };
        CentralMessageRouter.Route(message2);

        Console.WriteLine("\nMessage Router Pattern: --- Low Priority Analytics Event ---");
        var message3 = new RoutableMessage
        {
            Id = "MSG-003",
            Type = "AnalyticsEvent",
            Priority = "Low",
            Region = "APAC",
            Payload = "Page view event"
        };
        CentralMessageRouter.Route(message3);

        Console.WriteLine("\nMessage Router Pattern: --- High Priority Payment ---");
        var message4 = new RoutableMessage
        {
            Id = "MSG-004",
            Type = "Payment",
            Priority = "High",
            Region = "US",
            Payload = "Payment confirmation required"
        };
        CentralMessageRouter.Route(message4);
    }
}

// Message with routing properties
public sealed class RoutableMessage
{
    public required string Id { get; init; }
    public required string Type { get; init; }
    public required string Priority { get; init; }
    public required string Region { get; init; }
    public required string Payload { get; init; }
}

// Destination channels
public sealed class MessageChannels
{
    public static void SendToOrderChannel(RoutableMessage message)
    {
        Console.WriteLine($"Message Router Pattern: [OrderChannel] Received message {message.Id}");
        Console.WriteLine($"Message Router Pattern: [OrderChannel] Processing: {message.Payload}");
    }

    public static void SendToCustomerServiceChannel(RoutableMessage message)
    {
        Console.WriteLine($"Message Router Pattern: [CustomerServiceChannel] Received message {message.Id}");
        Console.WriteLine($"Message Router Pattern: [CustomerServiceChannel] Processing: {message.Payload}");
    }

    public static void SendToAnalyticsChannel(RoutableMessage message)
    {
        Console.WriteLine($"Message Router Pattern: [AnalyticsChannel] Received message {message.Id}");
        Console.WriteLine($"Message Router Pattern: [AnalyticsChannel] Processing: {message.Payload}");
    }

    public static void SendToPaymentChannel(RoutableMessage message)
    {
        Console.WriteLine($"Message Router Pattern: [PaymentChannel] Received message {message.Id}");
        Console.WriteLine($"Message Router Pattern: [PaymentChannel] Processing: {message.Payload}");
    }

    public static void SendToPriorityChannel(RoutableMessage message)
    {
        Console.WriteLine($"Message Router Pattern: [PriorityChannel] Received HIGH PRIORITY message {message.Id}");
        Console.WriteLine($"Message Router Pattern: [PriorityChannel] Fast-track processing: {message.Payload}");
    }
}

// Central router with routing logic
public sealed class CentralMessageRouter
{
    public static void Route(RoutableMessage message)
    {
        Console.WriteLine($"Message Router Pattern: [Router] Examining message {message.Id}");
        Console.WriteLine($"Message Router Pattern: [Router] Type: {message.Type}, Priority: {message.Priority}, Region: {message.Region}");

        // Rule 1: High priority messages always go to priority channel first
        if (message.Priority == "High")
        {
            Console.WriteLine($"Message Router Pattern: [Router] Rule matched: High Priority → Priority Channel");
            MessageChannels.SendToPriorityChannel(message);
            return;
        }

        // Rule 2: Route by message type
        Console.WriteLine($"Message Router Pattern: [Router] Applying type-based routing rules...");

        switch (message.Type)
        {
            case "Order":
                Console.WriteLine($"Message Router Pattern: [Router] Rule matched: Order Type → Order Channel");
                MessageChannels.SendToOrderChannel(message);
                break;

            case "CustomerQuery":
                Console.WriteLine($"Message Router Pattern: [Router] Rule matched: CustomerQuery Type → Customer Service Channel");
                MessageChannels.SendToCustomerServiceChannel(message);
                break;

            case "Payment":
                Console.WriteLine($"Message Router Pattern: [Router] Rule matched: Payment Type → Payment Channel");
                MessageChannels.SendToPaymentChannel(message);
                break;

            case "AnalyticsEvent":
                Console.WriteLine($"Message Router Pattern: [Router] Rule matched: Analytics Type → Analytics Channel");
                MessageChannels.SendToAnalyticsChannel(message);
                break;

            default:
                Console.WriteLine($"Message Router Pattern: [Router] No routing rule matched - message sent to default channel");
                MessageChannels.SendToCustomerServiceChannel(message);
                break;
        }
    }
}