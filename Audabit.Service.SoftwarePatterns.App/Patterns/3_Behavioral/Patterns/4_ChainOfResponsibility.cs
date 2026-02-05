namespace Audabit.Service.SoftwarePatterns.App.Patterns._3_Behavioral.Patterns;

// Chain of Responsibility Pattern: Passes a request along a chain of handlers, where each handler decides either to process the request or pass it to the next handler.
// In this example:
// - HTTP request logging handlers form a chain (AuthenticationHandler → LoggingHandler → ValidationHandler).
// - Each handler processes part of the request and decides whether to pass it forward.
// - If any handler can't process the request, it can stop the chain.
// - The key here is that the client doesn't know which handler will ultimately process the request.
//
// MODERN C# USAGE:
// ASP.NET Core middleware pipeline IS the Chain of Responsibility pattern:
//   app.UseAuthentication();  // Handler 1
//   app.UseAuthorization();   // Handler 2
//   app.UseEndpoints(...);    // Handler 3
// Each middleware calls next() to pass the request to the next handler.
// For custom chains, use Pipes and Filters pattern with LINQ or modern pipeline libraries.
public static class ChainOfResponsibility
{
    public static void Run()
    {
        Console.WriteLine("Chain of Responsibility Pattern: Passing requests through a chain of handlers...");
        Console.WriteLine("Chain of Responsibility Pattern: Each handler processes or passes the request.\n");

        var validation = new ValidationHandler();
        var logging = new LoggingHandler();
        var authentication = new AuthenticationHandler();

        authentication.SetNext(logging).SetNext(validation);

        Console.WriteLine("Chain of Responsibility Pattern: --- Processing Valid Request ---");
        var request1 = new HttpRequest("user123", "GET /api/data", true);
        authentication.Handle(request1);

        Console.WriteLine("\nChain of Responsibility Pattern: --- Processing Invalid Request ---");
        var request2 = new HttpRequest("", "POST /api/data", false);
        authentication.Handle(request2);
    }
}

// Request
public sealed record HttpRequest(string UserId, string Path, bool IsValid);

// Handler interface
public abstract class RequestHandler
{
    private RequestHandler? _nextHandler;

    public RequestHandler SetNext(RequestHandler handler)
    {
        _nextHandler = handler;
        return handler;
    }

    public virtual void Handle(HttpRequest request)
    {
        _nextHandler?.Handle(request);
    }
}

// Concrete Handlers
public sealed class AuthenticationHandler : RequestHandler
{
    public override void Handle(HttpRequest request)
    {
        if (string.IsNullOrEmpty(request.UserId))
        {
            Console.WriteLine("Chain of Responsibility Pattern: [AuthenticationHandler] ❌ Authentication failed - No user ID");
            return;
        }

        Console.WriteLine($"Chain of Responsibility Pattern: [AuthenticationHandler] ✅ User '{request.UserId}' authenticated");
        base.Handle(request);
    }
}

public sealed class LoggingHandler : RequestHandler
{
    public override void Handle(HttpRequest request)
    {
        Console.WriteLine($"Chain of Responsibility Pattern: [LoggingHandler] 📝 Logging request: {request.Path}");
        base.Handle(request);
    }
}

public sealed class ValidationHandler : RequestHandler
{
    public override void Handle(HttpRequest request)
    {
        if (!request.IsValid)
        {
            Console.WriteLine("Chain of Responsibility Pattern: [ValidationHandler] ❌ Request validation failed");
            return;
        }

        Console.WriteLine("Chain of Responsibility Pattern: [ValidationHandler] ✅ Request validated successfully");
        base.Handle(request);
    }
}