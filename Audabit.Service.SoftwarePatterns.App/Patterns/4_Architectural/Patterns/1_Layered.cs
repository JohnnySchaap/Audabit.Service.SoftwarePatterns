namespace Audabit.Service.SoftwarePatterns.App.Patterns._4_Architectural.Patterns;

// Layered Pattern: Organizes the system into horizontal layers, where each layer provides services to the layer above it and uses services from the layer below it.
// LEVEL: Code-level architecture pattern (how you organize classes and projects within an application)
// In this example:
// - We have a typical 3-tier architecture: Presentation Layer, Business Layer, and Data Access Layer.
// - Each layer has a specific responsibility and communicates only with adjacent layers.
// - Presentation Layer handles UI/API concerns, Business Layer contains business logic, Data Layer manages persistence.
// - The key here is that each layer is isolated and can be modified independently as long as the interface contracts are maintained.
//
// MODERN C# USAGE:
// Layered architecture is still the foundation of most .NET applications:
// - ASP.NET Core naturally supports layers: Controllers (Presentation) → Services (Business) → Repositories (Data)
// - Use dependency injection to wire layers together: services.AddScoped<IUserService, UserService>();
// - Modern variations include Clean Architecture and Onion Architecture which invert dependencies
// - Entity Framework Core is commonly used for the Data Access Layer
// - Each layer should be in its own project for clear separation: MyApp.Api, MyApp.Core, MyApp.Infrastructure
public static class Layered
{
    public static void Run()
    {
        Console.WriteLine("Layered Pattern: Organizing application into horizontal layers...");
        Console.WriteLine("Layered Pattern: Each layer has distinct responsibilities and communicates only with adjacent layers.\n");

        // Simulate a user login flow through all layers
        Console.WriteLine("Layered Pattern: --- User Login Request Flow ---");

        // Presentation Layer receives request
        var presentationLayer = new PresentationLayer();
        presentationLayer.HandleLoginRequest("john.doe@example.com", "SecurePass123");
    }
}

// ============================================
// PRESENTATION LAYER (UI/API)
// ============================================
public class PresentationLayer
{
    private readonly BusinessLayer _businessLayer = new();

    public void HandleLoginRequest(string email, string password)
    {
        Console.WriteLine("Layered Pattern: [Presentation Layer] Received login request");
        Console.WriteLine($"Layered Pattern: [Presentation Layer] Email: {email}");

        // Validate input format
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            Console.WriteLine("Layered Pattern: [Presentation Layer] Validation failed - empty fields\n");
            return;
        }

        // Pass to business layer
        var result = _businessLayer.AuthenticateUser(email, password);

        if (result)
        {
            Console.WriteLine("Layered Pattern: [Presentation Layer] Login successful! Redirecting to dashboard...\n");
        }
        else
        {
            Console.WriteLine("Layered Pattern: [Presentation Layer] Login failed. Invalid credentials.\n");
        }
    }
}

// ============================================
// BUSINESS LAYER (Business Logic)
// ============================================
public class BusinessLayer
{
    private readonly DataAccessLayer _dataLayer = new();

    public bool AuthenticateUser(string email, string password)
    {
        Console.WriteLine("Layered Pattern: [Business Layer] Processing authentication request");

        // Get user from data layer
        var user = _dataLayer.GetUserByEmail(email);

        if (user == null)
        {
            Console.WriteLine("Layered Pattern: [Business Layer] User not found in database");
            return false;
        }

        // Business logic: Verify password (in real app, use hashing)
        if (user.Password == password)
        {
            Console.WriteLine($"Layered Pattern: [Business Layer] Authentication successful for user: {user.Name}");

            // Additional business logic
            DataAccessLayer.UpdateLastLoginTime(user.Id);

            return true;
        }

        Console.WriteLine("Layered Pattern: [Business Layer] Password verification failed");
        return false;
    }
}

// ============================================
// DATA ACCESS LAYER (Persistence)
// ============================================
public class DataAccessLayer
{
    // In-memory "database" for demo
    private readonly List<User> _database =
    [
        new User { Id = 1, Name = "John Doe", Email = "john.doe@example.com", Password = "SecurePass123" },
        new User { Id = 2, Name = "Jane Smith", Email = "jane.smith@example.com", Password = "Password456" }
    ];

    public User? GetUserByEmail(string email)
    {
        Console.WriteLine($"Layered Pattern: [Data Layer] Querying database for email: {email}");

        var user = _database.FirstOrDefault(u => u.Email == email);

        if (user != null)
        {
            Console.WriteLine($"Layered Pattern: [Data Layer] User found: {user.Name}");
        }
        else
        {
            Console.WriteLine("Layered Pattern: [Data Layer] User not found");
        }

        return user;
    }

    public static void UpdateLastLoginTime(int userId)
    {
        Console.WriteLine($"Layered Pattern: [Data Layer] Updating last login time for user ID: {userId}");
        // In real app: UPDATE Users SET LastLoginTime = @now WHERE Id = @userId
    }
}

// ============================================
// DOMAIN MODEL
// ============================================
public class User
{
    public int Id { get; init; }
    public required string Name { get; init; }
    public required string Email { get; init; }
    public required string Password { get; init; }
}