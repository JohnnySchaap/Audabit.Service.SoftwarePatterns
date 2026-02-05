namespace Audabit.Service.SoftwarePatterns.App.Principles._0_Principles.Principles;

#pragma warning disable IDE0059 // Unnecessary assignment - intentional for demonstration

// YAGNI Principle: You Aren't Gonna Need It
// Don't implement functionality until it is actually needed

// MODERN C# USAGE:
// - Start with minimal implementation
// - Add features when requirements are clear
// - Avoid "future-proofing" that never gets used
// - Refactor when needs change, don't predict them
// - Focus on current requirements, not hypothetical ones
public static class YAGNI
{
    public static void Run()
    {
        Console.WriteLine("YAGNI Principle: You Aren't Gonna Need It demonstration...");
        Console.WriteLine("YAGNI Principle: Don't implement features until they are actually required.\n");

        // Example 1: Over-engineered user system
        Console.WriteLine("YAGNI Principle: --- Example 1: User Management ---");

        // ❌ YAGNI VIOLATION: Building for hypothetical future needs
        Console.WriteLine("YAGNI Principle: ❌ VIOLATION: Built UserManagementSystem with:");
        Console.WriteLine("YAGNI Principle: ❌ VIOLATION:   - Multi-tenant support (not needed yet)");
        Console.WriteLine("YAGNI Principle: ❌ VIOLATION:   - Role-based permissions (only 1 role exists)");
        Console.WriteLine("YAGNI Principle: ❌ VIOLATION:   - Audit logging (no compliance requirement)");
        Console.WriteLine("YAGNI Principle: ❌ VIOLATION:   - Custom caching layer (premature optimization)");
        Console.WriteLine("YAGNI Principle: ❌ VIOLATION: Result: 2000+ lines of unused code\n");

        // ✅ YAGNI COMPLIANT: Start with what's needed
        Console.WriteLine("YAGNI Principle: ✅ COMPLIANT: Built simple user CRUD:");
        var simpleUserService = new SimpleUserService();
        SimpleUserService.CreateUser("Alice");
        Console.WriteLine("YAGNI Principle: ✅ COMPLIANT:   - Create, Read, Update, Delete users");
        Console.WriteLine("YAGNI Principle: ✅ COMPLIANT:   - Single database table");
        Console.WriteLine("YAGNI Principle: ✅ COMPLIANT: Result: 200 lines, meets current requirements\n");

        // Example 2: Configuration system
        Console.WriteLine("YAGNI Principle: --- Example 2: Configuration ---");

        // ❌ YAGNI VIOLATION: Over-engineered config
        Console.WriteLine("YAGNI Principle: ❌ VIOLATION: Built ConfigurationFramework with:");
        Console.WriteLine("YAGNI Principle: ❌ VIOLATION:   - Support for JSON, XML, YAML, INI files");
        Console.WriteLine("YAGNI Principle: ❌ VIOLATION:   - Environment-specific overrides");
        Console.WriteLine("YAGNI Principle: ❌ VIOLATION:   - Hot-reloading without restart");
        Console.WriteLine("YAGNI Principle: ❌ VIOLATION:   - Encryption for sensitive values");
        Console.WriteLine("YAGNI Principle: ❌ VIOLATION: Actual need: Read 3 values from appsettings.json\n");

        // ✅ YAGNI COMPLIANT: Use built-in framework
        Console.WriteLine("YAGNI Principle: ✅ COMPLIANT: Used ASP.NET Core Configuration:");
        var simpleConfig = new SimpleConfig();
        Console.WriteLine($"YAGNI Principle: ✅ COMPLIANT: ApiUrl: {SimpleConfig.ApiUrl}");
        Console.WriteLine($"YAGNI Principle: ✅ COMPLIANT: Timeout: {SimpleConfig.Timeout}");
        Console.WriteLine("YAGNI Principle: ✅ COMPLIANT: Built-in, simple, works perfectly\n");

        // Example 3: Data access layer
        Console.WriteLine("YAGNI Principle: --- Example 3: Data Access ---");

        // ❌ YAGNI VIOLATION: Generic repository pattern
        Console.WriteLine("YAGNI Principle: ❌ VIOLATION: Built GenericRepository<T> with:");
        Console.WriteLine("YAGNI Principle: ❌ VIOLATION:   - Support for 5 different ORMs");
        Console.WriteLine("YAGNI Principle: ❌ VIOLATION:   - Automatic migration generation");
        Console.WriteLine("YAGNI Principle: ❌ VIOLATION:   - Read replicas and sharding");
        Console.WriteLine("YAGNI Principle: ❌ VIOLATION:   - Custom query builder DSL");
        Console.WriteLine("YAGNI Principle: ❌ VIOLATION: Actual need: Save and retrieve one entity type\n");

        // ✅ YAGNI COMPLIANT: Simple repository
        Console.WriteLine("YAGNI Principle: ✅ COMPLIANT: Created YagniProductRepository:");
        var productRepo = new YagniProductRepository();
        productRepo.Save(new YagniProduct { Id = 1, Name = "Widget" });
        var YagniProduct = productRepo.GetById(1);
        Console.WriteLine($"YAGNI Principle: ✅ COMPLIANT: Retrieved: {YagniProduct?.Name}");
        Console.WriteLine("YAGNI Principle: ✅ COMPLIANT: Entity Framework, straightforward queries\n");

        // Example 4: API design
        Console.WriteLine("YAGNI Principle: --- Example 4: API Design ---");

        // ❌ YAGNI VIOLATION: Over-designed API
        Console.WriteLine("YAGNI Principle: ❌ VIOLATION: Built RESTful API with:");
        Console.WriteLine("YAGNI Principle: ❌ VIOLATION:   - GraphQL support (no complex queries needed)");
        Console.WriteLine("YAGNI Principle: ❌ VIOLATION:   - Versioning v1, v2, v3 (no existing clients)");
        Console.WriteLine("YAGNI Principle: ❌ VIOLATION:   - Rate limiting per endpoint");
        Console.WriteLine("YAGNI Principle: ❌ VIOLATION:   - 20+ endpoints for future features");
        Console.WriteLine("YAGNI Principle: ❌ VIOLATION: Actual use: 3 endpoints by internal team\n");

        // ✅ YAGNI COMPLIANT: Simple API
        Console.WriteLine("YAGNI Principle: ✅ COMPLIANT: Created minimal API:");
        Console.WriteLine("YAGNI Principle: ✅ COMPLIANT:   - GET /products");
        Console.WriteLine("YAGNI Principle: ✅ COMPLIANT:   - POST /products");
        Console.WriteLine("YAGNI Principle: ✅ COMPLIANT:   - PUT /products/{id}");
        Console.WriteLine("YAGNI Principle: ✅ COMPLIANT: Covers current requirements, easy to extend\n");

        // Example 5: Testing infrastructure
        Console.WriteLine("YAGNI Principle: --- Example 5: Testing ---");

        // ❌ YAGNI VIOLATION: Over-built test framework
        Console.WriteLine("YAGNI Principle: ❌ VIOLATION: Built custom test framework:");
        Console.WriteLine("YAGNI Principle: ❌ VIOLATION:   - Support for 10+ assertion libraries");
        Console.WriteLine("YAGNI Principle: ❌ VIOLATION:   - Parallel test execution engine");
        Console.WriteLine("YAGNI Principle: ❌ VIOLATION:   - AI-powered test generation");
        Console.WriteLine("YAGNI Principle: ❌ VIOLATION: Actual need: Run 20 unit tests\n");

        // ✅ YAGNI COMPLIANT: Use standard tools
        Console.WriteLine("YAGNI Principle: ✅ COMPLIANT: Used xUnit + NSubstitute:");
        Console.WriteLine("YAGNI Principle: ✅ COMPLIANT:   - Standard testing framework");
        Console.WriteLine("YAGNI Principle: ✅ COMPLIANT:   - Works out of the box");
        Console.WriteLine("YAGNI Principle: ✅ COMPLIANT:   - 100% test coverage achieved\n");

        Console.WriteLine("YAGNI Principle: Key Takeaway: Build for TODAY's needs, not TOMORROW's guesses!");
    }
}

// ✅ YAGNI COMPLIANT: Simple implementations
public class SimpleUserService
{
    public static void CreateUser(string name)
    {
        Console.WriteLine($"YAGNI Principle: ✅ COMPLIANT: Created user: {name}");
    }
}

public class SimpleConfig
{
    public static string ApiUrl => "https://api.example.com";
    public static int Timeout => 30;
}

public class YagniProduct
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

internal class YagniProductRepository
{
    private readonly Dictionary<int, YagniProduct> _products = [];

    public void Save(YagniProduct YagniProduct)
    {
        _products[YagniProduct.Id] = YagniProduct;
        Console.WriteLine($"YAGNI Principle: ✅ COMPLIANT: Saved YagniProduct: {YagniProduct.Name}");
    }

    public YagniProduct? GetById(int id)
    {
        return _products.GetValueOrDefault(id);
    }
}