namespace Audabit.Service.SoftwarePatterns.App.Patterns._5_Messaging.Patterns;

// Dead Letter Channel Pattern: Routes messages that cannot be processed successfully to a separate channel for analysis.
// In this example:
// - Messages that fail processing after max retries are sent to dead letter queue.
// - Invalid messages (malformed, missing data) are immediately dead-lettered.
// - Dead letter queue allows investigation without blocking main processing.
// - The key here is that failed messages don't get lost - they're preserved for debugging.
//
// MODERN C# USAGE:
// In production .NET messaging:
// - Azure Service Bus has built-in dead letter queues for failed messages.
// - RabbitMQ provides dead letter exchanges (DLX) for automatic routing.
// - MassTransit configures dead letter routing: cfg.ReceiveEndpoint("queue", e => e.UseDeadLetter()).
// - Store dead letter metadata: exception details, retry count, timestamps.
// - Implement monitoring/alerts on dead letter queue growth.
public static class DeadLetterChannel
{
    public static void Run()
    {
        Console.WriteLine("Dead Letter Channel Pattern: Handling failed messages gracefully...");
        Console.WriteLine("Dead Letter Channel Pattern: Failed messages go to dead letter queue for investigation.\n");

        var deadLetterQueue = new DeadLetterQueue();
        var processor = new OrderProcessor(deadLetterQueue, maxRetries: 3);

        Console.WriteLine("Dead Letter Channel Pattern: --- Processing Different Order Messages ---\n");

        Console.WriteLine("Dead Letter Channel Pattern: --- Valid Order (Will Succeed) ---");
        var message1 = new OrderMessage
        {
            MessageId = "MSG-001",
            OrderId = "ORD-123",
            CustomerId = "CUST-001",
            Amount = 150.00m,
            IsValid = true
        };
        processor.ProcessMessage(message1);

        Console.WriteLine("\nDead Letter Channel Pattern: --- Invalid Order (Missing Data - Immediate Dead Letter) ---");
        var message2 = new OrderMessage
        {
            MessageId = "MSG-002",
            OrderId = "",
            CustomerId = "CUST-002",
            Amount = 0m,
            IsValid = false
        };
        processor.ProcessMessage(message2);

        Console.WriteLine("\nDead Letter Channel Pattern: --- Order That Will Fail Processing (Retry Until Dead Letter) ---");
        var message3 = new OrderMessage
        {
            MessageId = "MSG-003",
            OrderId = "ORD-456",
            CustomerId = "CUST-003",
            Amount = -50.00m, // Invalid amount causes processing failure
            IsValid = true
        };
        processor.ProcessMessage(message3);

        Console.WriteLine("\nDead Letter Channel Pattern: --- Checking Dead Letter Queue ---");
        deadLetterQueue.ShowDeadLetters();
    }
}

// Order message
public sealed class OrderMessage
{
    public required string MessageId { get; init; }
    public required string OrderId { get; init; }
    public required string CustomerId { get; init; }
    public decimal Amount { get; init; }
    public bool IsValid { get; init; }
    public int RetryCount { get; set; }
}

// Dead letter record
public sealed class DeadLetterRecord
{
    public required string MessageId { get; init; }
    public required string OriginalQueue { get; init; }
    public required string FailureReason { get; init; }
    public int RetryCount { get; init; }
    public DateTime DeadLetteredAt { get; init; }
    public required string MessageData { get; init; }
}

// Dead letter queue
public sealed class DeadLetterQueue
{
    private readonly List<DeadLetterRecord> _deadLetters = [];

    public void AddDeadLetter(OrderMessage message, string failureReason, string originalQueue)
    {
        Console.WriteLine($"Dead Letter Channel Pattern: [DeadLetterQueue] Message {message.MessageId} moved to dead letter queue");
        Console.WriteLine($"Dead Letter Channel Pattern: [DeadLetterQueue] Reason: {failureReason}");

        var deadLetter = new DeadLetterRecord
        {
            MessageId = message.MessageId,
            OriginalQueue = originalQueue,
            FailureReason = failureReason,
            RetryCount = message.RetryCount,
            DeadLetteredAt = DateTime.UtcNow,
            MessageData = $"OrderId: {message.OrderId}, CustomerId: {message.CustomerId}, Amount: {message.Amount}"
        };

        _deadLetters.Add(deadLetter);
        Console.WriteLine($"Dead Letter Channel Pattern: [DeadLetterQueue] Message preserved for investigation");
    }

    public void ShowDeadLetters()
    {
        Console.WriteLine($"Dead Letter Channel Pattern: [DeadLetterQueue] === Dead Letter Queue Status ===");
        Console.WriteLine($"Dead Letter Channel Pattern: [DeadLetterQueue] Total messages: {_deadLetters.Count}");

        if (_deadLetters.Count == 0)
        {
            Console.WriteLine($"Dead Letter Channel Pattern: [DeadLetterQueue] Queue is empty");
            return;
        }

        foreach (var deadLetter in _deadLetters)
        {
            Console.WriteLine($"Dead Letter Channel Pattern: [DeadLetterQueue] ---");
            Console.WriteLine($"Dead Letter Channel Pattern: [DeadLetterQueue] Message ID: {deadLetter.MessageId}");
            Console.WriteLine($"Dead Letter Channel Pattern: [DeadLetterQueue] From Queue: {deadLetter.OriginalQueue}");
            Console.WriteLine($"Dead Letter Channel Pattern: [DeadLetterQueue] Failure: {deadLetter.FailureReason}");
            Console.WriteLine($"Dead Letter Channel Pattern: [DeadLetterQueue] Retries: {deadLetter.RetryCount}");
            Console.WriteLine($"Dead Letter Channel Pattern: [DeadLetterQueue] Dead Lettered: {deadLetter.DeadLetteredAt:yyyy-MM-dd HH:mm:ss}");
            Console.WriteLine($"Dead Letter Channel Pattern: [DeadLetterQueue] Data: {deadLetter.MessageData}");
        }
    }
}

// Message processor with dead letter handling
public sealed class OrderProcessor(DeadLetterQueue deadLetterQueue, int maxRetries)
{
    private const string QueueName = "OrderProcessingQueue";

    public void ProcessMessage(OrderMessage message)
    {
        Console.WriteLine($"Dead Letter Channel Pattern: [Processor] Received message {message.MessageId}");

        // Check if message is valid
        if (!message.IsValid)
        {
            Console.WriteLine($"Dead Letter Channel Pattern: [Processor] Message validation failed");
            deadLetterQueue.AddDeadLetter(message, "Invalid message format - missing required fields", QueueName);
            return;
        }

        // Attempt to process the message
        var processedSuccessfully = AttemptProcessing(message);

        if (processedSuccessfully)
        {
            Console.WriteLine($"Dead Letter Channel Pattern: [Processor] Message {message.MessageId} processed successfully");
            return;
        }

        // Processing failed - check retry count
        message.RetryCount++;
        Console.WriteLine($"Dead Letter Channel Pattern: [Processor] Processing failed. Retry count: {message.RetryCount}/{maxRetries}");

        if (message.RetryCount < maxRetries)
        {
            Console.WriteLine($"Dead Letter Channel Pattern: [Processor] Retrying message processing...");

            // Simulate retry
            for (var attempt = message.RetryCount; attempt < maxRetries; attempt++)
            {
                Console.WriteLine($"Dead Letter Channel Pattern: [Processor] Retry attempt {attempt + 1}/{maxRetries}");
                message.RetryCount++;

                if (AttemptProcessing(message))
                {
                    Console.WriteLine($"Dead Letter Channel Pattern: [Processor] Message {message.MessageId} processed successfully on retry");
                    return;
                }
            }
        }

        // Max retries exceeded - send to dead letter queue
        Console.WriteLine($"Dead Letter Channel Pattern: [Processor] Max retries ({maxRetries}) exceeded");
        deadLetterQueue.AddDeadLetter(
            message,
            $"Processing failed after {message.RetryCount} attempts - Amount validation error",
            QueueName);
    }

    private static bool AttemptProcessing(OrderMessage message)
    {
        try
        {
            // Simulate processing logic
            if (message.Amount <= 0)
            {
                Console.WriteLine($"Dead Letter Channel Pattern: [Processor] Validation error: Amount must be greater than 0");
                return false;
            }

            Console.WriteLine($"Dead Letter Channel Pattern: [Processor] Processing order {message.OrderId} for ${message.Amount:N2}");
            return true;
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Dead Letter Channel Pattern: [Processor] Exception during processing: {ex.Message}");
            return false;
        }
    }
}