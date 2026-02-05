namespace Audabit.Service.SoftwarePatterns.App.Patterns._4_Architectural.Patterns;

// API Gateway Pattern: Provides a single entry point for client applications, routing requests to appropriate microservices and aggregating responses.
// LEVEL: Infrastructure-level architecture pattern (network component, reverse proxy, deployed as separate service)
// In this example:
// - Clients call a single API Gateway instead of calling multiple microservices directly.
// - Gateway handles routing, authentication, rate limiting, and response aggregation.
// - Backend services remain isolated and can evolve independently.
// - The key here is that the gateway acts as a facade for the microservices architecture, simplifying client integration.
//
// MODERN C# USAGE:
// API Gateway is fundamental in modern .NET microservices architectures:
// - Azure API Management for enterprise-grade API gateway with policies, throttling, and developer portal
// - Ocelot library for .NET: lightweight API gateway built on ASP.NET Core
// - YARP (Yet Another Reverse Proxy) from Microsoft for high-performance reverse proxy
// - Kong, Tyk, or AWS API Gateway for cloud-native solutions
// - Implement cross-cutting concerns: authentication (JWT), rate limiting, logging, caching
// - Backend for Frontend (BFF) pattern: separate gateway per client type (web, mobile, IoT)
// - Use response aggregation to reduce client-server round trips
public static class ApiGateway
{
    public static void Run()
    {
        Console.WriteLine("API Gateway Pattern: Single entry point for microservices architecture...");
        Console.WriteLine("API Gateway Pattern: Gateway handles routing, authentication, and aggregation.\n");

        // Backend microservices (clients don't call these directly)
        var userService = new UserServiceBackend();
        var orderService = new OrderServiceBackend();
        var productService = new ProductServiceBackend();

        // API Gateway (single entry point)
        var gateway = new ApiGatewayService(userService, orderService, productService);

        Console.WriteLine("API Gateway Pattern: --- Client Request: Get User Dashboard ---");
        gateway.GetUserDashboard("user123");
    }
}

// ============================================
// API GATEWAY (Single Entry Point)
// ============================================
public class ApiGatewayService(
    UserServiceBackend userService,
    OrderServiceBackend orderService,
    ProductServiceBackend productService)
{
    private readonly UserServiceBackend _userService = userService;
    private readonly OrderServiceBackend _orderService = orderService;
    private readonly ProductServiceBackend _productService = productService;

    public void GetUserDashboard(string userId)
    {
        Console.WriteLine($"API Gateway Pattern: [Gateway] Received request: GET /api/dashboard/{userId}");

        // Cross-cutting concern: Authentication
        Console.WriteLine("API Gateway Pattern: [Gateway] Validating JWT token...");
        Console.WriteLine("API Gateway Pattern: [Gateway] Authentication successful");

        // Cross-cutting concern: Rate limiting
        Console.WriteLine("API Gateway Pattern: [Gateway] Checking rate limit... OK");

        // Request aggregation: Call multiple backend services
        Console.WriteLine("\nAPI Gateway Pattern: [Gateway] Aggregating data from microservices...");

        var user = _userService.GetUser(userId);
        var orders = _orderService.GetRecentOrders(userId);
        var recommendations = _productService.GetRecommendations(userId);

        // Aggregate response
        Console.WriteLine("\nAPI Gateway Pattern: [Gateway] Combining responses...");
        Console.WriteLine($"API Gateway Pattern: [Gateway] Dashboard for {user}");
        Console.WriteLine($"API Gateway Pattern: [Gateway] - Recent Orders: {orders}");
        Console.WriteLine($"API Gateway Pattern: [Gateway] - Recommendations: {recommendations}");
        Console.WriteLine("\nAPI Gateway Pattern: [Gateway] Response sent to client (single round-trip)");
    }
}

// ============================================
// BACKEND MICROSERVICES
// ============================================
#pragma warning disable CA1822 // Mark members as static - intentionally instance methods for demo
public class UserServiceBackend
{
    public string GetUser(string userId)
    {
        Console.WriteLine($"API Gateway Pattern: [User Service] GET /users/{userId}");
        Console.WriteLine($"API Gateway Pattern: [User Service] Retrieved user: John Doe");
        return "John Doe";
    }
}

public class OrderServiceBackend
{
    public string GetRecentOrders(string userId)
    {
        Console.WriteLine($"API Gateway Pattern: [Order Service] GET /orders?userId={userId}");
        Console.WriteLine($"API Gateway Pattern: [Order Service] Found 3 recent orders");
        return "3 orders";
    }
}

public class ProductServiceBackend
{
    public string GetRecommendations(string userId)
    {
        Console.WriteLine($"API Gateway Pattern: [Product Service] GET /recommendations/{userId}");
        Console.WriteLine($"API Gateway Pattern: [Product Service] Generated 5 recommendations");
        return "5 products";
    }
}
#pragma warning restore CA1822