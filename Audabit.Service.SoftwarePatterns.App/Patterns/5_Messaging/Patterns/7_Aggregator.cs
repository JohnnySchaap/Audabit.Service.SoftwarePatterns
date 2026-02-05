namespace Audabit.Service.SoftwarePatterns.App.Patterns._5_Messaging.Patterns;

// Aggregator Pattern: Combines multiple related messages into a single composite message for batch processing.
// In this example:
// - Individual payment confirmation messages arrive from different payment processors.
// - The aggregator waits until all expected confirmations arrive (or timeout occurs).
// - Once all parts are received, it combines them into a single reconciliation report.
// - The key here is that the aggregator knows how many messages to expect and when to release the aggregate.
//
// MODERN C# USAGE:
// In production .NET integration:
// - Use NServiceBus Sagas or MassTransit Sagas to implement stateful aggregation.
// - Store partial messages in Redis or database with TTL for timeout handling.
// - Azure Durable Functions provide orchestration for aggregating async results.
// - Correlation ID links related messages together (all parts from same transaction).
// - Implement completion strategies: count-based, timeout-based, or content-based completion.
public static class Aggregator
{
    public static void Run()
    {
        Console.WriteLine("Aggregator Pattern: Combining multiple related messages into one...");
        Console.WriteLine("Aggregator Pattern: Waits for all parts before creating aggregate.\n");

        var aggregator = new PaymentAggregator(expectedMessageCount: 3);

        Console.WriteLine("Aggregator Pattern: --- Simulating Distributed Payment Processing ---");
        Console.WriteLine("Aggregator Pattern: Order requires confirmation from 3 payment processors\n");

        Console.WriteLine("Aggregator Pattern: --- Payment Processor 1 Confirms ---");
        var payment1 = new PaymentConfirmation
        {
            CorrelationId = "TXN-001",
            ProcessorId = "PayPal",
            Amount = 100.00m,
            Status = "Success",
            Timestamp = DateTime.UtcNow
        };
        aggregator.ReceivePaymentConfirmation(payment1);

        Console.WriteLine("\nAggregator Pattern: --- Payment Processor 2 Confirms ---");
        var payment2 = new PaymentConfirmation
        {
            CorrelationId = "TXN-001",
            ProcessorId = "Stripe",
            Amount = 100.00m,
            Status = "Success",
            Timestamp = DateTime.UtcNow.AddSeconds(1)
        };
        aggregator.ReceivePaymentConfirmation(payment2);

        Console.WriteLine("\nAggregator Pattern: --- Payment Processor 3 Confirms ---");
        var payment3 = new PaymentConfirmation
        {
            CorrelationId = "TXN-001",
            ProcessorId = "Square",
            Amount = 100.00m,
            Status = "Success",
            Timestamp = DateTime.UtcNow.AddSeconds(2)
        };
        aggregator.ReceivePaymentConfirmation(payment3);
    }
}

// Individual payment confirmation message
public sealed class PaymentConfirmation
{
    public required string CorrelationId { get; init; }
    public required string ProcessorId { get; init; }
    public decimal Amount { get; init; }
    public required string Status { get; init; }
    public DateTime Timestamp { get; init; }
}

// Aggregated report combining all confirmations
public sealed class ReconciliationReport
{
    public required string CorrelationId { get; init; }
    public required List<PaymentConfirmation> Confirmations { get; init; }
    public decimal TotalAmount { get; init; }
    public required string OverallStatus { get; init; }
    public DateTime CreatedAt { get; init; }
}

// Aggregator that collects and combines messages
public sealed class PaymentAggregator(int expectedMessageCount)
{
    private readonly Dictionary<string, List<PaymentConfirmation>> _pendingConfirmations = [];

    public void ReceivePaymentConfirmation(PaymentConfirmation confirmation)
    {
        Console.WriteLine($"Aggregator Pattern: [Aggregator] Received confirmation from {confirmation.ProcessorId}");
        Console.WriteLine($"Aggregator Pattern: [Aggregator] Status: {confirmation.Status}, Amount: {confirmation.Amount:C}");

        // Add to pending collection
        if (!_pendingConfirmations.TryGetValue(confirmation.CorrelationId, out var confirmations))
        {
            confirmations = [];
            _pendingConfirmations[confirmation.CorrelationId] = confirmations;
        }

        confirmations.Add(confirmation);

        var receivedCount = confirmations.Count;
        Console.WriteLine($"Aggregator Pattern: [Aggregator] Received {receivedCount} of {expectedMessageCount} confirmations");

        // Check if we have all expected messages
        if (receivedCount == expectedMessageCount)
        {
            Console.WriteLine($"Aggregator Pattern: [Aggregator] All confirmations received! Creating aggregate report...");
            CreateReconciliationReport(confirmation.CorrelationId);
        }
        else
        {
            Console.WriteLine($"Aggregator Pattern: [Aggregator] Waiting for {expectedMessageCount - receivedCount} more confirmation(s)...");
        }
    }

    private void CreateReconciliationReport(string correlationId)
    {
        var confirmations = _pendingConfirmations[correlationId];

        var report = new ReconciliationReport
        {
            CorrelationId = correlationId,
            Confirmations = confirmations,
            TotalAmount = confirmations.Sum(c => c.Amount),
            OverallStatus = confirmations.All(c => c.Status == "Success") ? "AllSucceeded" : "PartialFailure",
            CreatedAt = DateTime.UtcNow
        };

        Console.WriteLine($"\nAggregator Pattern: [Aggregator] === Reconciliation Report Created ===");
        Console.WriteLine($"Aggregator Pattern: [Report] Correlation ID: {report.CorrelationId}");
        Console.WriteLine($"Aggregator Pattern: [Report] Total Amount: {report.TotalAmount:C}");
        Console.WriteLine($"Aggregator Pattern: [Report] Overall Status: {report.OverallStatus}");
        Console.WriteLine($"Aggregator Pattern: [Report] Processors involved:");
        foreach (var conf in report.Confirmations)
        {
            Console.WriteLine($"Aggregator Pattern: [Report]   - {conf.ProcessorId}: {conf.Status} ({conf.Amount:C})");
        }

        // Clean up
        _pendingConfirmations.Remove(correlationId);
        Console.WriteLine($"Aggregator Pattern: [Aggregator] Aggregate sent to downstream system");
    }
}