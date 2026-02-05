namespace Audabit.Service.SoftwarePatterns.App.Principles._0_Principles.Principles;

// Encapsulation: Bundling data and methods that operate on that data within a single unit (class),
// and controlling access to the internal state through public interfaces

// MODERN C# USAGE:
// - Properties with private setters or init-only setters
// - Private fields with public methods
// - Record types for immutable data
// - Internal/private access modifiers
// - Validation in constructors/setters
public static class Encapsulation
{
    public static void Run()
    {
        Console.WriteLine("Encapsulation: Demonstrating data hiding and controlled access...");
        Console.WriteLine("Encapsulation: Hide internal state and require interaction through well-defined interfaces.\n");

        // Example 1: Poor encapsulation vs proper encapsulation
        Console.WriteLine("Encapsulation: --- Example 1: Public Fields vs Properties ---\n");

        // ❌ BAD: Public fields expose internal state
        Console.WriteLine("Encapsulation: ❌ BAD: Public fields - no validation, no control:");
        var badAccount = new BankAccountBad
        {
            Balance = 1000m,
            AccountNumber = "12345"
        };
        badAccount.Balance = -500m; // Can set invalid state!
        Console.WriteLine($"Encapsulation: ❌ BAD: Balance can be negative: ${badAccount.Balance}");
        Console.WriteLine("Encapsulation: ❌ BAD: Anyone can modify internal state directly\n");

        // ✅ GOOD: Properties with validation
        Console.WriteLine("Encapsulation: ✅ GOOD: Encapsulated with validation:");
        var goodAccount = new BankAccountGood("12345", 1000m);
        Console.WriteLine($"Encapsulation: ✅ GOOD: Initial balance: ${goodAccount.Balance}");

        try
        {
            goodAccount.Withdraw(1500m); // Validation prevents invalid state
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Encapsulation: ✅ GOOD: Validation prevented: {ex.Message}");
        }

        goodAccount.Deposit(500m);
        Console.WriteLine($"Encapsulation: ✅ GOOD: After deposit: ${goodAccount.Balance}\n");

        // Example 2: Information hiding
        Console.WriteLine("Encapsulation: --- Example 2: Information Hiding ---\n");

        // ❌ BAD: Implementation details exposed
        Console.WriteLine("Encapsulation: ❌ BAD: Exposed implementation:");
        var badUser = new UserBad
        {
            PasswordHash = "plaintext_password" // Direct access to sensitive field
        };
        Console.WriteLine($"Encapsulation: ❌ BAD: Password exposed: {badUser.PasswordHash}\n");

        // ✅ GOOD: Implementation hidden, controlled access
        Console.WriteLine("Encapsulation: ✅ GOOD: Hidden implementation:");
        var goodUser = new UserGood("john@example.com");
        goodUser.SetPassword("SecurePassword123!");
        var isValid = goodUser.VerifyPassword("SecurePassword123!");
        Console.WriteLine($"Encapsulation: ✅ GOOD: Password verified: {isValid}");
        Console.WriteLine("Encapsulation: ✅ GOOD: Hash algorithm hidden from external code\n");

        // Example 3: Immutability with encapsulation
        Console.WriteLine("Encapsulation: --- Example 3: Immutability ---\n");

        // ❌ BAD: Mutable reference types leak
        Console.WriteLine("Encapsulation: ❌ BAD: Mutable collection exposed:");
        var badOrder = new OrderBad();
        badOrder.Items.Add("Hacked Item"); // External code can modify internal state!
        Console.WriteLine($"Encapsulation: ❌ BAD: Order items compromised: {string.Join(", ", badOrder.Items)}\n");

        // ✅ GOOD: Immutable or defensive copies
        Console.WriteLine("Encapsulation: ✅ GOOD: Defensive copying:");
        var goodOrder = new OrderGood(["Item1", "Item2", "Item3"]);
        var items = goodOrder.GetItems(); // Returns copy
        items.Add("Attempted Hack"); // Doesn't affect internal state
        Console.WriteLine($"Encapsulation: ✅ GOOD: Original items protected: {string.Join(", ", goodOrder.GetItems())}");
        Console.WriteLine($"Encapsulation: ✅ GOOD: External copy modified: {string.Join(", ", items)}\n");

        // Example 4: Encapsulation in modern C# (records, init)
        Console.WriteLine("Encapsulation: --- Example 4: Modern C# Encapsulation ---\n");

        Console.WriteLine("Encapsulation: ✅ GOOD: Record with init-only properties:");
        var person = new PersonRecord("John Doe", 30);
        Console.WriteLine($"Encapsulation: ✅ GOOD: Person: {person.Name}, Age: {person.Age}");
        // person.Age = 31; // Compile error - init-only!

        var olderPerson = person with { Age = 31 }; // Non-destructive mutation
        Console.WriteLine($"Encapsulation: ✅ GOOD: New instance: {olderPerson.Name}, Age: {olderPerson.Age}");
        Console.WriteLine($"Encapsulation: ✅ GOOD: Original unchanged: {person.Name}, Age: {person.Age}\n");

        // Example 5: Calculated properties
        Console.WriteLine("Encapsulation: --- Example 5: Calculated Properties ---\n");

        Console.WriteLine("Encapsulation: ✅ GOOD: Derived state from encapsulated data:");
        var rectangle = new Rectangle(5, 10);
        Console.WriteLine($"Encapsulation: ✅ GOOD: Width: {rectangle.Width}, Height: {rectangle.Height}");
        Console.WriteLine($"Encapsulation: ✅ GOOD: Calculated Area: {rectangle.Area}");
        Console.WriteLine($"Encapsulation: ✅ GOOD: Calculated Perimeter: {rectangle.Perimeter}");
        Console.WriteLine("Encapsulation: ✅ GOOD: Area/Perimeter computed on-demand, not stored\n");

        Console.WriteLine("Encapsulation: Key Takeaways:");
        Console.WriteLine("Encapsulation: ✓ Hide internal implementation details");
        Console.WriteLine("Encapsulation: ✓ Control access through public interfaces");
        Console.WriteLine("Encapsulation: ✓ Validate state changes");
        Console.WriteLine("Encapsulation: ✓ Use properties instead of public fields");
        Console.WriteLine("Encapsulation: ✓ Return defensive copies of mutable collections");
        Console.WriteLine("Encapsulation: ✓ Leverage modern C# features (records, init, readonly)");
    }
}

// ❌ BAD: Public fields
internal class BankAccountBad
{
    public string AccountNumber = string.Empty;
    public decimal Balance;
}

// ✅ GOOD: Encapsulated with validation
internal class BankAccountGood
{
    private decimal _balance;

    public string AccountNumber { get; }
    public decimal Balance => _balance;

    public BankAccountGood(string accountNumber, decimal initialBalance)
    {
        if (string.IsNullOrWhiteSpace(accountNumber))
            throw new ArgumentException("Account number is required");
        if (initialBalance < 0)
            throw new ArgumentException("Initial balance cannot be negative");

        AccountNumber = accountNumber;
        _balance = initialBalance;
    }

    public void Deposit(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Deposit amount must be positive");

        _balance += amount;
    }

    public void Withdraw(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Withdrawal amount must be positive");
        if (amount > _balance)
            throw new InvalidOperationException("Insufficient funds");

        _balance -= amount;
    }
}

// ❌ BAD: Sensitive data exposed
internal class UserBad
{
    public string Email = string.Empty;
    public string PasswordHash = string.Empty; // Exposed!
}

// ✅ GOOD: Implementation hidden
internal class UserGood
{
    private string _passwordHash = string.Empty;

    public string Email { get; }

    public UserGood(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required");

        Email = email;
    }

    public void SetPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
            throw new ArgumentException("Password must be at least 8 characters");

        // Simplified hash - in reality use BCrypt, Argon2, etc.
        _passwordHash = Convert.ToBase64String(
            System.Security.Cryptography.SHA256.HashData(
                System.Text.Encoding.UTF8.GetBytes(password)));
    }

    public bool VerifyPassword(string password)
    {
        var hash = Convert.ToBase64String(
            System.Security.Cryptography.SHA256.HashData(
                System.Text.Encoding.UTF8.GetBytes(password)));
        return hash == _passwordHash;
    }
}

// ❌ BAD: Mutable collection exposed
internal class OrderBad
{
    public List<string> Items { get; } = [];
}

// ✅ GOOD: Defensive copying
internal class OrderGood(IEnumerable<string> items)
{
    private readonly List<string> _items = [.. items];

    public List<string> GetItems()
    {
        return [.. _items]; // Defensive copy on output
    }

    public void AddItem(string item)
    {
        if (string.IsNullOrWhiteSpace(item))
            throw new ArgumentException("Item cannot be empty");
        _items.Add(item);
    }
}

// ✅ GOOD: Modern C# record with init-only properties
internal record PersonRecord(string Name, int Age);

// ✅ GOOD: Calculated properties
internal class Rectangle
{
    public double Width { get; }
    public double Height { get; }

    // Calculated properties - no backing field
    public double Area => Width * Height;
    public double Perimeter => 2 * (Width + Height);

    public Rectangle(double width, double height)
    {
        if (width <= 0 || height <= 0)
            throw new ArgumentException("Dimensions must be positive");

        Width = width;
        Height = height;
    }
}