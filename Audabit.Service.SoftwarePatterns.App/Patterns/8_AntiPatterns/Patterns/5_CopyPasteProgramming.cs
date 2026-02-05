namespace Audabit.Service.SoftwarePatterns.App.Patterns._8_AntiPatterns.Patterns;

// Copy-Paste Programming Anti-Pattern: Duplicating code instead of abstracting common logic.
// This demonstrates the problems of code duplication and how to properly extract reusable code.
//
// PROBLEMS:
// - Code duplication (DRY violation)
// - Bug fixes need multiple changes
// - Inconsistent behavior across copies
// - Maintenance nightmare
// - Increased code size
//
// SOLUTION:
// - Extract common logic to methods
// - Use inheritance or composition
// - Create reusable utilities
// - Follow DRY principle
public static class CopyPasteProgramming
{
    public static void Run()
    {
        Console.WriteLine("Copy Paste Programming Anti-Pattern: Demonstrating duplication vs abstraction...");
        Console.WriteLine("Copy Paste Programming Anti-Pattern: DRY principle in action.\n");

        Console.WriteLine("Copy Paste Programming Anti-Pattern: --- BAD EXAMPLE: Duplicated Validation ---\n");
        DuplicatedValidationBad();

        Console.WriteLine("\nCopy Paste Programming Anti-Pattern: --- GOOD EXAMPLE: Extracted Common Validation ---\n");
        ExtractedValidationGood();

        Console.WriteLine("\nCopy Paste Programming Anti-Pattern: --- BAD EXAMPLE: Duplicated Data Access ---\n");
        DuplicatedDataAccessBad();

        Console.WriteLine("\nCopy Paste Programming Anti-Pattern: --- GOOD EXAMPLE: Generic Repository ---\n");
        GenericRepositoryGood();

        Console.WriteLine("\nCopy Paste Programming Anti-Pattern: Key Benefits:");
        Console.WriteLine("Copy Paste Programming Anti-Pattern: ✓ Single source of truth (DRY)");
        Console.WriteLine("Copy Paste Programming Anti-Pattern: ✓ Fix bugs in one place");
        Console.WriteLine("Copy Paste Programming Anti-Pattern: ✓ Consistent behavior");
        Console.WriteLine("Copy Paste Programming Anti-Pattern: ✓ Easier to test and maintain");
    }

    // ❌ BAD: Duplicated validation logic (copy-pasted)
    private static void DuplicatedValidationBad()
    {
        Console.WriteLine("Copy Paste Programming Anti-Pattern: [BAD] Validation logic duplicated everywhere:\n");

        // User validation - copy-pasted
        var user = new { Email = "invalid-email", Age = 15 };
        if (string.IsNullOrEmpty(user.Email) || !user.Email.Contains('@'))
        {
            Console.WriteLine("Copy Paste Programming Anti-Pattern: User email invalid");
        }
        if (user.Age < 18)
        {
            Console.WriteLine("Copy Paste Programming Anti-Pattern: User age invalid");
        }

        // Customer validation - SAME LOGIC copy-pasted
        var customer = new { Email = "no-at-sign", Age = 16 };
        if (string.IsNullOrEmpty(customer.Email) || !customer.Email.Contains('@'))
        {
            Console.WriteLine("Copy Paste Programming Anti-Pattern: Customer email invalid");
        }
        if (customer.Age < 18)
        {
            Console.WriteLine("Copy Paste Programming Anti-Pattern: Customer age invalid");
        }

        // Contact validation - SAME LOGIC copy-pasted AGAIN
        var contact = new { Email = "also-invalid", Age = 17 };
        if (string.IsNullOrEmpty(contact.Email) || !contact.Email.Contains('@'))
        {
            Console.WriteLine("Copy Paste Programming Anti-Pattern: Contact email invalid");
        }
        if (contact.Age < 18)
        {
            Console.WriteLine("Copy Paste Programming Anti-Pattern: Contact age invalid");
        }

        Console.WriteLine("\nCopy Paste Programming Anti-Pattern: Problem: Bug fix or change requires updating 3+ places!");
    }

    // ✅ GOOD: Extracted validation into reusable methods
    private static void ExtractedValidationGood()
    {
        Console.WriteLine("Copy Paste Programming Anti-Pattern: [GOOD] Validation extracted to reusable methods:\n");

        var user = new { Email = "invalid-email", Age = 15 };
        ValidateEntity("User", user.Email, user.Age);

        var customer = new { Email = "no-at-sign", Age = 16 };
        ValidateEntity("Customer", customer.Email, customer.Age);

        var contact = new { Email = "also-invalid", Age = 17 };
        ValidateEntity("Contact", contact.Email, contact.Age);

        Console.WriteLine("\nCopy Paste Programming Anti-Pattern: Benefit: Validation logic in ONE place!");
    }

    private static void ValidateEntity(string entityType, string email, int age)
    {
        if (!IsValidEmail(email))
        {
            Console.WriteLine($"Copy Paste Programming Anti-Pattern: {entityType} email invalid");
        }

        if (!IsValidAge(age))
        {
            Console.WriteLine($"Copy Paste Programming Anti-Pattern: {entityType} age invalid");
        }
    }

    private static bool IsValidEmail(string email)
    {
        return !string.IsNullOrEmpty(email) && email.Contains('@');
    }

    private static bool IsValidAge(int age)
    {
        return age >= 18;
    }

    // ❌ BAD: Duplicated data access code
    private static void DuplicatedDataAccessBad()
    {
        Console.WriteLine("Copy Paste Programming Anti-Pattern: [BAD] Data access code duplicated for each entity:\n");

        // Get user - copy-pasted logic
        Console.WriteLine("Copy Paste Programming Anti-Pattern: UserRepository.GetById:");
        Console.WriteLine("Copy Paste Programming Anti-Pattern:   - Open connection");
        Console.WriteLine("Copy Paste Programming Anti-Pattern:   - Execute SELECT * FROM Users WHERE Id = @id");
        Console.WriteLine("Copy Paste Programming Anti-Pattern:   - Map to User object");
        Console.WriteLine("Copy Paste Programming Anti-Pattern:   - Close connection");

        // Get customer - SAME LOGIC copy-pasted
        Console.WriteLine("Copy Paste Programming Anti-Pattern: CustomerRepository.GetById:");
        Console.WriteLine("Copy Paste Programming Anti-Pattern:   - Open connection");
        Console.WriteLine("Copy Paste Programming Anti-Pattern:   - Execute SELECT * FROM Customers WHERE Id = @id");
        Console.WriteLine("Copy Paste Programming Anti-Pattern:   - Map to Customer object");
        Console.WriteLine("Copy Paste Programming Anti-Pattern:   - Close connection");

        // Get product - SAME LOGIC copy-pasted AGAIN
        Console.WriteLine("Copy Paste Programming Anti-Pattern: ProductRepository.GetById:");
        Console.WriteLine("Copy Paste Programming Anti-Pattern:   - Open connection");
        Console.WriteLine("Copy Paste Programming Anti-Pattern:   - Execute SELECT * FROM Products WHERE Id = @id");
        Console.WriteLine("Copy Paste Programming Anti-Pattern:   - Map to Product object");
        Console.WriteLine("Copy Paste Programming Anti-Pattern:   - Close connection");

        Console.WriteLine("\nCopy Paste Programming Anti-Pattern: Problem: 3 repositories, all with identical CRUD logic!");
    }

    // ✅ GOOD: Generic repository pattern
    private static void GenericRepositoryGood()
    {
        Console.WriteLine("Copy Paste Programming Anti-Pattern: [GOOD] Generic repository eliminates duplication:\n");
        _ = new Repository<User>("Users");
        _ = new Repository<Customer>("Customers");
        _ = new Repository<Product>("Products");

        Console.WriteLine("Copy Paste Programming Anti-Pattern: Repository<User>.GetById(1):");
        Console.WriteLine("Copy Paste Programming Anti-Pattern:   - Uses shared GetById<T> implementation");

        Console.WriteLine("Copy Paste Programming Anti-Pattern: Repository<Customer>.GetById(2):");
        Console.WriteLine("Copy Paste Programming Anti-Pattern:   - Uses SAME GetById<T> implementation");

        Console.WriteLine("Copy Paste Programming Anti-Pattern: Repository<Product>.GetById(3):");
        Console.WriteLine("Copy Paste Programming Anti-Pattern:   - Uses SAME GetById<T> implementation");

        Console.WriteLine("\nCopy Paste Programming Anti-Pattern: Benefit: CRUD logic written ONCE, used everywhere!");
    }

    private class Repository<T>(string tableName) where T : class, new()
    {
        private readonly string _tableName = tableName;

        public T GetById(int id)
        {
            // Single implementation used by all repositories
            Console.WriteLine($"Copy Paste Programming Anti-Pattern:   - Executing: SELECT * FROM {_tableName} WHERE Id = {id}");
            return new T();
        }

        // Add, Update, Delete methods also shared
    }

    private class User { }
    private class Customer { }
    private class Product { }
}