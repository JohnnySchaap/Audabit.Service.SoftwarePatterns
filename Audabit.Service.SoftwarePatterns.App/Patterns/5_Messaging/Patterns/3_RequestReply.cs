namespace Audabit.Service.SoftwarePatterns.App.Patterns._5_Messaging.Patterns;

// Request-Reply Pattern: Enables asynchronous two-way communication where sender expects a response.
// In this example:
// - A client sends a credit check request and waits for a response.
// - The request includes a reply-to address (callback queue/correlation ID).
// - The service processes the request and sends back a reply to the specified address.
// - The key here is that communication is asynchronous - sender doesn't block waiting.
//
// MODERN C# USAGE:
// In production .NET applications:
// - Use Azure Service Bus with correlation ID for request-reply.
// - MassTransit provides request/response pattern: await client.GetResponse<Response>(request).
// - Use async/await with Task<T> for cleaner asynchronous code.
// - Implement timeout handling: CancellationTokenSource with timeouts.
// - gRPC and SignalR provide built-in request-reply semantics.
public static class RequestReply
{
    public static void Run()
    {
        Console.WriteLine("Request-Reply Pattern: Two-way asynchronous message communication...");
        Console.WriteLine("Request-Reply Pattern: Sender waits for reply using correlation ID.\n");

        var messageBus = new RequestReplyMessageBus();
        var creditCheckService = new CreditCheckService(messageBus);
        var client = new LoanApplicationClient(messageBus);

        Console.WriteLine("Request-Reply Pattern: --- Client Sending Credit Check Request ---\n");

        Console.WriteLine("Request-Reply Pattern: --- Request #1: Good Credit ---");
        client.CheckCredit("CUST-001", "Alice Johnson", 750);

        Console.WriteLine("\nRequest-Reply Pattern: --- Processing Requests ---");
        messageBus.ProcessPendingRequests(creditCheckService);

        Console.WriteLine("\nRequest-Reply Pattern: --- Client Processing Replies ---");
        messageBus.DeliverPendingReplies();

        Console.WriteLine("\n\nRequest-Reply Pattern: --- Request #2: Poor Credit ---");
        client.CheckCredit("CUST-002", "Bob Smith", 580);

        Console.WriteLine("\nRequest-Reply Pattern: --- Processing Requests ---");
        messageBus.ProcessPendingRequests(creditCheckService);

        Console.WriteLine("\nRequest-Reply Pattern: --- Client Processing Replies ---");
        messageBus.DeliverPendingReplies();
    }
}

// Request message
public sealed class CreditCheckRequest
{
    public required string RequestId { get; init; }
    public required string CustomerId { get; init; }
    public required string CustomerName { get; init; }
    public int CreditScore { get; init; }
    public required string ReplyTo { get; init; } // Where to send the reply
}

// Reply message
public sealed class CreditCheckReply
{
    public required string RequestId { get; init; } // Correlation ID
    public required string CustomerId { get; init; }
    public required bool Approved { get; init; }
    public required string Reason { get; init; }
    public decimal ApprovedAmount { get; init; }
}

// Message bus handling request-reply pattern
public sealed class RequestReplyMessageBus
{
    private readonly Queue<CreditCheckRequest> _requestQueue = new();
    private readonly Dictionary<string, CreditCheckReply> _replyQueue = [];

    public void SendRequest(CreditCheckRequest request)
    {
        Console.WriteLine($"Request-Reply Pattern: [MessageBus] Request {request.RequestId} queued");
        _requestQueue.Enqueue(request);
    }

    public void SendReply(CreditCheckReply reply)
    {
        Console.WriteLine($"Request-Reply Pattern: [MessageBus] Reply for request {reply.RequestId} queued");
        _replyQueue[reply.RequestId] = reply;
    }

    public void ProcessPendingRequests(CreditCheckService service)
    {
        Console.WriteLine($"Request-Reply Pattern: [MessageBus] Processing {_requestQueue.Count} pending request(s)");

        while (_requestQueue.Count > 0)
        {
            var request = _requestQueue.Dequeue();
            service.ProcessCreditCheck(request);
        }
    }

    public void DeliverPendingReplies()
    {
        Console.WriteLine($"Request-Reply Pattern: [MessageBus] Delivering {_replyQueue.Count} pending repl(ies)");

        foreach (var reply in _replyQueue.Values)
        {
            LoanApplicationClient.HandleReply(reply);
        }

        _replyQueue.Clear();
    }
}

// Service that processes requests and sends replies
public sealed class CreditCheckService(RequestReplyMessageBus messageBus)
{
    private const int MinimumCreditScore = 650;

    public void ProcessCreditCheck(CreditCheckRequest request)
    {
        Console.WriteLine($"Request-Reply Pattern: [CreditCheckService] Processing request {request.RequestId}");
        Console.WriteLine($"Request-Reply Pattern: [CreditCheckService] Customer: {request.CustomerName}");
        Console.WriteLine($"Request-Reply Pattern: [CreditCheckService] Credit Score: {request.CreditScore}");

        bool approved;
        string reason;
        decimal approvedAmount;

        if (request.CreditScore >= MinimumCreditScore)
        {
            approved = true;
            reason = "Credit score meets minimum requirements";
            approvedAmount = request.CreditScore * 100; // Simple approval amount calculation
            Console.WriteLine($"Request-Reply Pattern: [CreditCheckService] APPROVED - Amount: ${approvedAmount:N2}");
        }
        else
        {
            approved = false;
            reason = $"Credit score {request.CreditScore} below minimum {MinimumCreditScore}";
            approvedAmount = 0;
            Console.WriteLine($"Request-Reply Pattern: [CreditCheckService] DENIED - {reason}");
        }

        var reply = new CreditCheckReply
        {
            RequestId = request.RequestId, // Correlation ID links request to reply
            CustomerId = request.CustomerId,
            Approved = approved,
            Reason = reason,
            ApprovedAmount = approvedAmount
        };

        Console.WriteLine($"Request-Reply Pattern: [CreditCheckService] Sending reply to: {request.ReplyTo}");
        messageBus.SendReply(reply);
    }
}

// Client that sends requests and handles replies
public sealed class LoanApplicationClient(RequestReplyMessageBus messageBus)
{
    private const string ReplyAddress = "LoanApplicationClient.Replies";

    public void CheckCredit(string customerId, string customerName, int creditScore)
    {
        var requestId = Guid.NewGuid().ToString();

        Console.WriteLine($"Request-Reply Pattern: [Client] Creating credit check request for {customerName}");

        var request = new CreditCheckRequest
        {
            RequestId = requestId,
            CustomerId = customerId,
            CustomerName = customerName,
            CreditScore = creditScore,
            ReplyTo = ReplyAddress // Tell service where to send reply
        };

        Console.WriteLine($"Request-Reply Pattern: [Client] Request ID: {requestId}");
        Console.WriteLine($"Request-Reply Pattern: [Client] Reply-To: {ReplyAddress}");
        Console.WriteLine($"Request-Reply Pattern: [Client] Sending request to credit check service...");

        messageBus.SendRequest(request);
    }

    public static void HandleReply(CreditCheckReply reply)
    {
        Console.WriteLine($"Request-Reply Pattern: [Client] Received reply for request {reply.RequestId}");
        Console.WriteLine($"Request-Reply Pattern: [Client] Customer: {reply.CustomerId}");
        Console.WriteLine($"Request-Reply Pattern: [Client] Approved: {reply.Approved}");
        Console.WriteLine($"Request-Reply Pattern: [Client] Reason: {reply.Reason}");

        if (reply.Approved)
        {
            Console.WriteLine($"Request-Reply Pattern: [Client] Approved Amount: ${reply.ApprovedAmount:N2}");
            Console.WriteLine($"Request-Reply Pattern: [Client] Proceeding with loan application...");
        }
        else
        {
            Console.WriteLine($"Request-Reply Pattern: [Client] Loan application rejected");
        }
    }
}