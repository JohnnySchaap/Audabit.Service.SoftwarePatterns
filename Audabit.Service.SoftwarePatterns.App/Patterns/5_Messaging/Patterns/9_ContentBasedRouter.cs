namespace Audabit.Service.SoftwarePatterns.App.Patterns._5_Messaging.Patterns;

// Content-Based Router Pattern: Routes messages to different destinations by inspecting message content/payload.
// In this example:
// - Insurance claims arrive with different claim types and amounts.
// - The router examines claim details (amount, type, region) to determine routing.
// - Small claims go to automated processing, large claims to manual review.
// - The key here is that routing decisions are based on actual message data, not just metadata.
//

// MODERN C# USAGE:
// In production .NET integration:
// - Use System.Text.Json to deserialize and inspect message payloads.
// - Azure Service Bus SQL filters allow content-based routing: "Amount > 10000".
// - Pattern matching with switch expressions provides clean content inspection.
// - MassTransit polymorphic message routing based on message type and properties.
// - Combine with Message Translator when different channels expect different formats.
public static class ContentBasedRouter
{
    public static void Run()
    {
        Console.WriteLine("Content-Based Router Pattern: Routing based on message content...");
        Console.WriteLine("Content-Based Router Pattern: Inspects payload to determine destination.\n");

        Console.WriteLine("Content-Based Router Pattern: --- Processing Different Insurance Claims ---\n");

        Console.WriteLine("Content-Based Router Pattern: --- Small Auto Claim ---");
        var claim1 = new InsuranceClaim
        {
            ClaimId = "CLM-001",
            ClaimType = "Auto",
            Amount = 2500m,
            Region = "West",
            CustomerId = "CUST-123",
            Description = "Minor fender bender"
        };
        ClaimRouter.ProcessClaim(claim1);

        Console.WriteLine("\nContent-Based Router Pattern: --- Large Property Claim ---");
        var claim2 = new InsuranceClaim
        {
            ClaimId = "CLM-002",
            ClaimType = "Property",
            Amount = 45000m,
            Region = "East",
            CustomerId = "CUST-456",
            Description = "Fire damage to home"
        };
        ClaimRouter.ProcessClaim(claim2);

        Console.WriteLine("\nContent-Based Router Pattern: --- Fraud Risk Health Claim ---");
        var claim3 = new InsuranceClaim
        {
            ClaimId = "CLM-003",
            ClaimType = "Health",
            Amount = 8500m,
            Region = "South",
            CustomerId = "CUST-789",
            Description = "Medical procedure - duplicate submission detected"
        };
        ClaimRouter.ProcessClaim(claim3);

        Console.WriteLine("\nContent-Based Router Pattern: --- Standard Health Claim ---");
        var claim4 = new InsuranceClaim
        {
            ClaimId = "CLM-004",
            ClaimType = "Health",
            Amount = 1200m,
            Region = "North",
            CustomerId = "CUST-321",
            Description = "Routine medical checkup"
        };
        ClaimRouter.ProcessClaim(claim4);
    }
}

// Insurance claim message
public sealed class InsuranceClaim
{
    public required string ClaimId { get; init; }
    public required string ClaimType { get; init; }
    public decimal Amount { get; init; }
    public required string Region { get; init; }
    public required string CustomerId { get; init; }
    public required string Description { get; init; }
}

// Claim processing channels
public sealed class ClaimProcessor
{
    public static void ProcessAutomatic(InsuranceClaim claim)
    {
        Console.WriteLine($"Content-Based Router Pattern: [AutomaticProcessor] Processing claim {claim.ClaimId}");
        Console.WriteLine($"Content-Based Router Pattern: [AutomaticProcessor] Auto-approved for ${claim.Amount:N2}");
        Console.WriteLine($"Content-Based Router Pattern: [AutomaticProcessor] Payment will be issued within 24 hours");
    }

    public static void ProcessManualReview(InsuranceClaim claim)
    {
        Console.WriteLine($"Content-Based Router Pattern: [ManualReviewQueue] Claim {claim.ClaimId} requires review");
        Console.WriteLine($"Content-Based Router Pattern: [ManualReviewQueue] Reason: High amount (${claim.Amount:N2})");
        Console.WriteLine($"Content-Based Router Pattern: [ManualReviewQueue] Assigned to senior adjuster");
    }

    public static void ProcessFraudDetection(InsuranceClaim claim)
    {
        Console.WriteLine($"Content-Based Router Pattern: [FraudDetection] ALERT: Claim {claim.ClaimId} flagged");
        Console.WriteLine($"Content-Based Router Pattern: [FraudDetection] Description: {claim.Description}");
        Console.WriteLine($"Content-Based Router Pattern: [FraudDetection] Escalated to investigation team");
    }

    public static void ProcessRegionalOffice(InsuranceClaim claim)
    {
        Console.WriteLine($"Content-Based Router Pattern: [RegionalOffice-{claim.Region}] Processing claim {claim.ClaimId}");
        Console.WriteLine($"Content-Based Router Pattern: [RegionalOffice-{claim.Region}] Routed to local office for specialized handling");
    }
}

// Content-based router
public sealed class ClaimRouter
{
    private const decimal HighValueThreshold = 10000m;
    private const decimal AutoApprovalLimit = 5000m;

    public static void ProcessClaim(InsuranceClaim claim)
    {
        Console.WriteLine($"Content-Based Router Pattern: [Router] Analyzing claim {claim.ClaimId}");
        Console.WriteLine($"Content-Based Router Pattern: [Router] Type: {claim.ClaimType}, Amount: ${claim.Amount:N2}");
        Console.WriteLine($"Content-Based Router Pattern: [Router] Inspecting content for routing decision...");

        // Check for fraud indicators in description
        if (claim.Description.Contains("duplicate", StringComparison.OrdinalIgnoreCase) ||
            claim.Description.Contains("fraud", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine($"Content-Based Router Pattern: [Router] Content analysis: Fraud risk detected");
            Console.WriteLine($"Content-Based Router Pattern: [Router] Route decision: FRAUD DETECTION QUEUE");
            ClaimProcessor.ProcessFraudDetection(claim);
            return;
        }

        // Check claim amount for routing
        if (claim.Amount >= HighValueThreshold)
        {
            Console.WriteLine($"Content-Based Router Pattern: [Router] Content analysis: High-value claim (>= ${HighValueThreshold:N2})");
            Console.WriteLine($"Content-Based Router Pattern: [Router] Route decision: MANUAL REVIEW");
            ClaimProcessor.ProcessManualReview(claim);
            return;
        }

        // Check if claim can be auto-approved
        if (claim.Amount <= AutoApprovalLimit && claim.ClaimType != "Property")
        {
            Console.WriteLine($"Content-Based Router Pattern: [Router] Content analysis: Low-value, non-property claim");
            Console.WriteLine($"Content-Based Router Pattern: [Router] Route decision: AUTOMATIC PROCESSING");
            ClaimProcessor.ProcessAutomatic(claim);
            return;
        }

        // Default routing to regional office for specialized handling
        Console.WriteLine($"Content-Based Router Pattern: [Router] Content analysis: Standard claim requiring regional processing");
        Console.WriteLine($"Content-Based Router Pattern: [Router] Route decision: REGIONAL OFFICE ({claim.Region})");
        ClaimProcessor.ProcessRegionalOffice(claim);
    }
}