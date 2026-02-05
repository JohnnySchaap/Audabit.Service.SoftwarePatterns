namespace Audabit.Service.SoftwarePatterns.App.Patterns._5_Messaging.Patterns;

// Inbox Pattern: Ensures messages are processed exactly once by storing message IDs in database and checking for duplicates.
// In this example:
// - When a message arrives, we first check if its ID already exists in the inbox table.
// - If the ID exists, we skip processing (idempotent behavior - already processed).
// - If the ID is new, we process the message and store the ID in the inbox table, all in one transaction.
// - The key here is that duplicate messages (retries, network issues) don't cause duplicate processing.
//
// MODERN C# USAGE:
// In production .NET microservices:
// - Use Entity Framework Core with a ProcessedMessage/InboxMessage entity in your DbContext.
// - Store message ID + timestamp + processed date in the inbox table.
// - Implement cleanup job to remove old processed messages (after retention period).
// - Azure Service Bus provides message deduplication, but inbox pattern works across any broker.
// - Combine with Outbox pattern for end-to-end exactly-once semantics.
public static class Inbox
{
    public static void Run()
    {
        Console.WriteLine("Inbox Pattern: Ensuring messages are processed exactly once...");
        Console.WriteLine("Inbox Pattern: Deduplicates messages using database-backed inbox.\n");

        var database = new InboxDatabase();
        var messageHandler = new PaymentMessageHandler(database);

        Console.WriteLine("Inbox Pattern: --- Receiving New Message ---");
        var message1 = new IncomingMessage
        {
            MessageId = "MSG-001",
            EventType = "PaymentReceived",
            Payload = "{\"orderId\":\"ORD-123\",\"amount\":100.00}"
        };
        messageHandler.HandleMessage(message1);

        Console.WriteLine("\nInbox Pattern: --- Receiving Duplicate Message (Retry) ---");
        var message2 = new IncomingMessage
        {
            MessageId = "MSG-001", // Same ID as above
            EventType = "PaymentReceived",
            Payload = "{\"orderId\":\"ORD-123\",\"amount\":100.00}"
        };
        messageHandler.HandleMessage(message2);

        Console.WriteLine("\nInbox Pattern: --- Receiving Different Message ---");
        var message3 = new IncomingMessage
        {
            MessageId = "MSG-002", // Different ID
            EventType = "PaymentReceived",
            Payload = "{\"orderId\":\"ORD-456\",\"amount\":250.00}"
        };
        messageHandler.HandleMessage(message3);
    }
}

// Incoming message from message broker
public sealed class IncomingMessage
{
    public required string MessageId { get; init; }
    public required string EventType { get; init; }
    public required string Payload { get; init; }
}

// Inbox record stored in database
public sealed class InboxRecord
{
    public required string MessageId { get; init; }
    public required string EventType { get; init; }
    public DateTime ProcessedAt { get; init; }
    public string Status { get; init; } = "Processing";  // Processing, Success, Failed
    public string? ErrorMessage { get; init; }  // Capture failure details for visibility
}

// Business entity
public sealed class Payment
{
    public required string OrderId { get; init; }
    public decimal Amount { get; init; }
}

// Simulated database with inbox table
public sealed class InboxDatabase
{
    private readonly List<InboxRecord> _inboxRecords = [];
    private readonly List<Payment> _payments = [];

    /// <summary>
    /// INSERT-FIRST: Try to insert inbox record immediately (with unique constraint simulation).
    /// Returns true if inserted (new message), false if duplicate key (already being processed).
    /// </summary>
    public bool TryInsertInboxRecord(InboxRecord inboxRecord)
    {
        // Simulate unique constraint on MessageId
        if (_inboxRecords.Any(r => r.MessageId == inboxRecord.MessageId))
        {
            Console.WriteLine($"Inbox Pattern: [Database] DUPLICATE KEY: Message {inboxRecord.MessageId} already exists - another consumer got it first");
            return false;  // Another consumer already inserted this message
        }

        Console.WriteLine($"Inbox Pattern: [Database] INSERT inbox record {inboxRecord.MessageId} with status 'Processing'");
        _inboxRecords.Add(inboxRecord);
        return true;  // Successfully claimed this message
    }

    public void UpdateInboxRecordStatus(string messageId, string status, string? errorMessage = null)
    {
        var record = _inboxRecords.First(r => r.MessageId == messageId);
        Console.WriteLine($"Inbox Pattern: [Database] UPDATE inbox record {messageId} status: {record.Status} → {status}");

        // In reality, this would update the record in-place
        // For demo, we'll just show the status change
        _inboxRecords.Remove(record);
        _inboxRecords.Add(new InboxRecord
        {
            MessageId = record.MessageId,
            EventType = record.EventType,
            ProcessedAt = record.ProcessedAt,
            Status = status,
            ErrorMessage = errorMessage
        });
    }

    public void SavePayment(Payment payment)
    {
        Console.WriteLine($"Inbox Pattern: [Database] Saving payment for order {payment.OrderId}");
        _payments.Add(payment);
    }

    public int GetTotalPaymentsProcessed()
    {
        return _payments.Count;
    }
}

// Message handler using inbox pattern with INSERT-FIRST strategy
public sealed class PaymentMessageHandler(InboxDatabase database)
{
    public void HandleMessage(IncomingMessage message)
    {
        Console.WriteLine($"Inbox Pattern: [Handler] Received message {message.MessageId}");

        // INSERT-FIRST: Try to insert inbox record immediately
        // If insert fails (duplicate key), another consumer is already processing this message
        var inboxRecord = new InboxRecord
        {
            MessageId = message.MessageId,
            EventType = message.EventType,
            ProcessedAt = DateTime.UtcNow,
            Status = "Processing"  // Start in Processing state
        };

        if (!database.TryInsertInboxRecord(inboxRecord))
        {
            Console.WriteLine($"Inbox Pattern: [Handler] Message {message.MessageId} already being processed by another consumer - SKIPPING");
            Console.WriteLine($"Inbox Pattern: [Handler] This prevents race conditions with multiple consumers!");
            return;
        }

        Console.WriteLine($"Inbox Pattern: [Handler] Successfully claimed message {message.MessageId} - processing...");

        try
        {
            // Simulate parsing payment data
            var payment = new Payment
            {
                OrderId = "ORD-123",
                Amount = 100.00m
            };

            // Save payment
            database.SavePayment(payment);

            // Update inbox record to Success
            database.UpdateInboxRecordStatus(message.MessageId, "Success");

            Console.WriteLine($"Inbox Pattern: [Handler] Message {message.MessageId} processed successfully!");
            Console.WriteLine($"Inbox Pattern: [Handler] Total payments processed: {database.GetTotalPaymentsProcessed()}");
        }
        catch (Exception ex)
        {
            // DON'T REMOVE THE INBOX RECORD! Mark as Failed instead.
            // This prevents duplicate processing if message broker redelivers.
            database.UpdateInboxRecordStatus(message.MessageId, "Failed", ex.Message);

            Console.WriteLine($"Inbox Pattern: [Handler] ❌ Message {message.MessageId} processing FAILED: {ex.Message}");
            Console.WriteLine($"Inbox Pattern: [Handler] Record kept with 'Failed' status to prevent duplicate processing on retry");
            Console.WriteLine($"Inbox Pattern: [Handler] Message will go to Dead Letter Queue after max retries");

            throw;  // Let message broker handle retries/DLQ
        }
    }
}