namespace Audabit.Service.SoftwarePatterns.App.Patterns._4_Architectural.Patterns;

// Strangler Fig Pattern: Incrementally migrates a legacy system to a new system by gradually replacing specific pieces of functionality until the old system is completely retired.
// LEVEL: Infrastructure-level architecture pattern (migration strategy with routing layer between legacy and modern systems)
// In this example:
// - We have a legacy monolith system that needs to be modernized.
// - A facade/proxy intercepts requests and routes to either legacy or new system.
// - New features are built in the new system, old features gradually migrated.
// - The key here is that the migration happens incrementally without a risky "big bang" rewrite, allowing continuous delivery during the transition.
//
// MODERN C# USAGE:
// Strangler Fig is the recommended pattern for modernizing legacy .NET applications:
// - Use reverse proxy (YARP, Ocelot) to route traffic between legacy and new systems
// - Azure Application Gateway or API Management for cloud-based routing
// - Implement feature flags to gradually switch users to new implementation
// - Common migration: Legacy .NET Framework → Modern .NET 9.0
// - Or: Monolith → Microservices migration
// - Use anti-corruption layer to translate between legacy and modern models
// - Monitor both systems during transition with Application Insights
// - Named after strangler fig trees that gradually replace their host tree
public static class StranglerFig
{
    public static void Run()
    {
        Console.WriteLine("Strangler Fig Pattern: Incrementally replacing legacy system with new implementation...");
        Console.WriteLine("Strangler Fig Pattern: Facade routes requests to legacy or new system based on feature migration status.\n");

        // Legacy system (being replaced)
        var legacySystem = new LegacyMonolith();

        // New microservices (replacing legacy incrementally)
        var newUserService = new ModernUserService();
        var newOrderService = new ModernOrderService();

        // Strangler Facade (routing layer)
        var facade = new StranglerFacade(legacySystem, newUserService, newOrderService);

        Console.WriteLine("Strangler Fig Pattern: --- Processing Various Requests ---\n");

        // Request 1: User management (MIGRATED to new system)
        Console.WriteLine("Strangler Fig Pattern: Request 1: Get User");
        facade.GetUser("user123");

        // Request 2: Order management (MIGRATED to new system)
        Console.WriteLine("\n\nStrangler Fig Pattern: Request 2: Create Order");
        facade.CreateOrder("user123", 99.99m);

        // Request 3: Inventory management (NOT YET MIGRATED - still in legacy)
        Console.WriteLine("\n\nStrangler Fig Pattern: Request 3: Check Inventory");
        facade.CheckInventory("PROD-456");

        Console.WriteLine("\n\nStrangler Fig Pattern: --- Migration Progress ---");
        Console.WriteLine("Strangler Fig Pattern: ✓ User Service: Migrated to new system");
        Console.WriteLine("Strangler Fig Pattern: ✓ Order Service: Migrated to new system");
        Console.WriteLine("Strangler Fig Pattern: ⧗ Inventory Service: Still in legacy (pending migration)");
    }
}

// ============================================
// STRANGLER FACADE (Routing Layer)
// ============================================
public class StranglerFacade(
    LegacyMonolith legacySystem,
    ModernUserService modernUserService,
    ModernOrderService modernOrderService)
{
    private readonly LegacyMonolith _legacySystem = legacySystem;
    private readonly ModernUserService _modernUserService = modernUserService;
    private readonly ModernOrderService _modernOrderService = modernOrderService;

    // Feature flags track what's been migrated
    private readonly Dictionary<string, bool> _migratedFeatures = new()
    {
        { "UserManagement", true },   // Migrated
        { "OrderManagement", true },  // Migrated
        { "Inventory", false }        // Not yet migrated
    };

    public void GetUser(string userId)
    {
        Console.WriteLine("Strangler Fig Pattern: [Facade] Routing GetUser request...");

        if (_migratedFeatures["UserManagement"])
        {
            Console.WriteLine("Strangler Fig Pattern: [Facade] → Routing to NEW system (Modern User Service)");
            ModernUserService.GetUser(userId);
        }
        else
        {
            Console.WriteLine("Strangler Fig Pattern: [Facade] → Routing to LEGACY system");
            LegacyMonolith.GetUser(userId);
        }
    }

    public void CreateOrder(string userId, decimal amount)
    {
        Console.WriteLine("Strangler Fig Pattern: [Facade] Routing CreateOrder request...");

        if (_migratedFeatures["OrderManagement"])
        {
            Console.WriteLine("Strangler Fig Pattern: [Facade] → Routing to NEW system (Modern Order Service)");
            ModernOrderService.CreateOrder(userId, amount);
        }
        else
        {
            Console.WriteLine("Strangler Fig Pattern: [Facade] → Routing to LEGACY system");
            LegacyMonolith.CreateOrder(userId, amount);
        }
    }

    public void CheckInventory(string productId)
    {
        Console.WriteLine("Strangler Fig Pattern: [Facade] Routing CheckInventory request...");

        if (_migratedFeatures["Inventory"])
        {
            Console.WriteLine("Strangler Fig Pattern: [Facade] → Routing to NEW system");
            // Modern service doesn't exist yet
        }
        else
        {
            Console.WriteLine("Strangler Fig Pattern: [Facade] → Routing to LEGACY system (not yet migrated)");
            LegacyMonolith.CheckInventory(productId);
        }
    }
}

// ============================================
// LEGACY SYSTEM (Being replaced)
// ============================================
public class LegacyMonolith
{
    public static void GetUser(string userId)
    {
        Console.WriteLine($"Strangler Fig Pattern: [Legacy Monolith] Getting user {userId}");
        Console.WriteLine($"Strangler Fig Pattern: [Legacy Monolith] Using old .NET Framework code");
    }

    public static void CreateOrder(string userId, decimal _)
    {
        Console.WriteLine($"Strangler Fig Pattern: [Legacy Monolith] Creating order for {userId}");
        Console.WriteLine($"Strangler Fig Pattern: [Legacy Monolith] Using old database schema");
    }

    public static void CheckInventory(string productId)
    {
        Console.WriteLine($"Strangler Fig Pattern: [Legacy Monolith] Checking inventory for {productId}");
        Console.WriteLine($"Strangler Fig Pattern: [Legacy Monolith] Using legacy inventory system");
        Console.WriteLine($"Strangler Fig Pattern: [Legacy Monolith] Product available: 45 units");
    }
}

// ============================================
// NEW MODERN SERVICES (Replacing legacy)
// ============================================
public class ModernUserService
{
    public static void GetUser(string userId)
    {
        Console.WriteLine($"Strangler Fig Pattern: [Modern User Service] Getting user {userId}");
        Console.WriteLine($"Strangler Fig Pattern: [Modern User Service] Using .NET 9.0 with async/await");
        Console.WriteLine($"Strangler Fig Pattern: [Modern User Service] Reading from modern database schema");
    }
}

public class ModernOrderService
{
    public static void CreateOrder(string userId, decimal _)
    {
        Console.WriteLine($"Strangler Fig Pattern: [Modern Order Service] Creating order for {userId}");
        Console.WriteLine($"Strangler Fig Pattern: [Modern Order Service] Using event-driven architecture");
        Console.WriteLine($"Strangler Fig Pattern: [Modern Order Service] Publishing OrderCreatedEvent");
    }
}