namespace Audabit.Service.SoftwarePatterns.App.Patterns._5_Messaging;

/// <summary>
/// Service implementation for demonstrating Enterprise Integration / Messaging Patterns.
/// </summary>
public sealed class MessagingPatternsService : IMessagingPatternsService
{
    public void Outbox()
    {
        Console.WriteLine("\n=== Outbox Pattern Demo ===\n");
        Patterns.Outbox.Run();
        Console.WriteLine("\n=== Outbox Pattern: Demonstration complete.===\n");
    }

    public void Inbox()
    {
        Console.WriteLine("\n=== Inbox Pattern Demo ===\n");
        Patterns.Inbox.Run();
        Console.WriteLine("\n=== Inbox Pattern: Demonstration complete.===\n");
    }

    public void Splitter()
    {
        Console.WriteLine("\n=== Splitter Pattern Demo ===\n");
        Patterns.Splitter.Run();
        Console.WriteLine("\n=== Splitter Pattern: Demonstration complete.===\n");
    }

    public void Aggregator()
    {
        Console.WriteLine("\n=== Aggregator Pattern Demo ===\n");
        Patterns.Aggregator.Run();
        Console.WriteLine("\n=== Aggregator Pattern: Demonstration complete.===\n");
    }

    public void MessageRouter()
    {
        Console.WriteLine("\n=== Message Router Pattern Demo ===\n");
        Patterns.MessageRouter.Run();
        Console.WriteLine("\n=== Message Router Pattern: Demonstration complete.===\n");
    }

    public void ContentBasedRouter()
    {
        Console.WriteLine("\n=== Content-Based Router Pattern Demo ===\n");
        Patterns.ContentBasedRouter.Run();
        Console.WriteLine("\n=== Content-Based Router Pattern: Demonstration complete.===\n");
    }

    public void MessageTranslator()
    {
        Console.WriteLine("\n=== Message Translator Pattern Demo ===\n");
        Patterns.MessageTranslator.Run();
        Console.WriteLine("\n=== Message Translator Pattern: Demonstration complete.===\n");
    }

    public void PublishSubscribe()
    {
        Console.WriteLine("\n=== Publish-Subscribe Pattern Demo ===\n");
        Patterns.PublishSubscribe.Run();
        Console.WriteLine("\n=== Publish-Subscribe Pattern: Demonstration complete.===\n");
    }

    public void RequestReply()
    {
        Console.WriteLine("\n=== Request-Reply Pattern Demo ===\n");
        Patterns.RequestReply.Run();
        Console.WriteLine("\n=== Request-Reply Pattern: Demonstration complete.===\n");
    }

    public void DeadLetterChannel()
    {
        Console.WriteLine("\n=== Dead Letter Channel Pattern Demo ===\n");
        Patterns.DeadLetterChannel.Run();
        Console.WriteLine("\n=== Dead Letter Channel Pattern: Demonstration complete.===\n");
    }
}