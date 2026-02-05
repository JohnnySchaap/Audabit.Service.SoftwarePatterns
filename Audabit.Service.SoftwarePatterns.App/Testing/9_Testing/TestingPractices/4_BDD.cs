namespace Audabit.Service.SoftwarePatterns.App.Testing._9_Testing.TestingPractices;

// BDD (Behavior-Driven Development) Pattern: Write tests as executable specifications using Given-When-Then.
// In this example:
// - GIVEN: Set up initial context/state
// - WHEN: Perform the action being tested
// - THEN: Assert expected outcomes
// - Uses natural language to describe behavior from user perspective
//
// MODERN C# USAGE:
// - SpecFlow for full BDD with Gherkin syntax
// - Structure unit tests using Given-When-Then comments
// - Focus on business behavior, not implementation
// - Makes tests readable by non-technical stakeholders
public static class BDD
{
    public static void Run()
    {
        Console.WriteLine("BDD Methodology: Demonstrating Behavior-Driven Development...");
        Console.WriteLine("BDD Methodology: Given-When-Then scenarios.\n");

        Console.WriteLine("BDD Methodology: === Feature: User Authentication ===\n");

        Console.WriteLine("BDD Methodology: Scenario 1: Successful login with valid credentials");
        Console.WriteLine("BDD Methodology: ---------------------------------------------------");
        LoginWithValidCredentials();
        Console.WriteLine();

        Console.WriteLine("BDD Methodology: Scenario 2: Login fails with invalid password");
        Console.WriteLine("BDD Methodology: ------------------------------------------------");
        LoginWithInvalidPassword();
        Console.WriteLine();

        Console.WriteLine("BDD Methodology: Scenario 3: Account locks after multiple failures");
        Console.WriteLine("BDD Methodology: ---------------------------------------------------");
        AccountLocksAfterFailedAttempts();
        Console.WriteLine();

        Console.WriteLine("BDD Methodology: Benefits:");
        Console.WriteLine("BDD Methodology: ✓ Tests read like specifications");
        Console.WriteLine("BDD Methodology: ✓ Shared language between devs, QA, and business");
        Console.WriteLine("BDD Methodology: ✓ Focus on behavior, not implementation details");
        Console.WriteLine("BDD Methodology: ✓ Living documentation of system behavior");
    }

    private static void LoginWithValidCredentials()
    {
        // GIVEN
        Console.WriteLine("BDD Methodology: GIVEN a user account exists with username 'john@example.com'");
        Console.WriteLine("BDD Methodology:   AND the password is 'SecurePass123'");
        var authService = new AuthenticationService();
        authService.CreateUser("john@example.com", "SecurePass123");

        // WHEN
        Console.WriteLine("BDD Methodology: WHEN the user attempts to login with correct credentials");
        var result = authService.Login("john@example.com", "SecurePass123");

        // THEN
        Console.WriteLine("BDD Methodology: THEN login should succeed");
        Console.WriteLine($"BDD Methodology:   AND user should be authenticated → {result.IsAuthenticated} ✓");
        Console.WriteLine($"BDD Methodology:   AND session token should be returned → {!string.IsNullOrEmpty(result.Token)} ✓");
    }

    private static void LoginWithInvalidPassword()
    {
        // GIVEN
        Console.WriteLine("BDD Methodology: GIVEN a user account exists");
        var authService = new AuthenticationService();
        authService.CreateUser("jane@example.com", "CorrectPassword");

        // WHEN
        Console.WriteLine("BDD Methodology: WHEN the user attempts to login with wrong password");
        var result = authService.Login("jane@example.com", "WrongPassword");

        // THEN
        Console.WriteLine("BDD Methodology: THEN login should fail");
        Console.WriteLine($"BDD Methodology:   AND user should not be authenticated → {!result.IsAuthenticated} ✓");
        Console.WriteLine($"BDD Methodology:   AND error message should be shown → {!string.IsNullOrEmpty(result.ErrorMessage)} ✓");
        Console.WriteLine($"BDD Methodology:   AND error is '{result.ErrorMessage}'");
    }

    private static void AccountLocksAfterFailedAttempts()
    {
        // GIVEN
        Console.WriteLine("BDD Methodology: GIVEN a user account exists");
        Console.WriteLine("BDD Methodology:   AND account lockout is enabled after 3 failures");
        var authService = new AuthenticationService(maxFailedAttempts: 3);
        authService.CreateUser("bob@example.com", "Password123");

        // WHEN
        Console.WriteLine("BDD Methodology: WHEN the user fails to login 3 times");
        authService.Login("bob@example.com", "Wrong1");
        authService.Login("bob@example.com", "Wrong2");
        var thirdAttempt = authService.Login("bob@example.com", "Wrong3");

        // THEN
        Console.WriteLine("BDD Methodology: THEN the account should be locked");
        Console.WriteLine($"BDD Methodology:   AND error indicates account is locked → {thirdAttempt.IsLocked} ✓");

        // AND WHEN
        Console.WriteLine("BDD Methodology:   AND WHEN user tries to login with correct password");
        var lockedLoginAttempt = authService.Login("bob@example.com", "Password123");

        // THEN
        Console.WriteLine("BDD Methodology: THEN login should still fail due to lock");
        Console.WriteLine($"BDD Methodology:   AND user cannot authenticate → {!lockedLoginAttempt.IsAuthenticated} ✓");
    }
}

// Simple authentication service for demonstration
public class AuthenticationService(int maxFailedAttempts = 3)
{
    private readonly Dictionary<string, UserAccount> _users = [];
    private readonly int _maxFailedAttempts = maxFailedAttempts;

    public void CreateUser(string username, string password)
    {
        _users[username] = new UserAccount
        {
            Username = username,
            PasswordHash = password, // In real code: hash this!
            FailedAttempts = 0,
            IsLocked = false
        };
    }

    public LoginResult Login(string username, string password)
    {
        if (!_users.TryGetValue(username, out var user))
        {
            return new LoginResult { ErrorMessage = "User not found" };
        }

        if (user.IsLocked)
        {
            return new LoginResult { IsLocked = true, ErrorMessage = "Account is locked" };
        }

        if (user.PasswordHash != password)
        {
            user.FailedAttempts++;
            if (user.FailedAttempts >= _maxFailedAttempts)
            {
                user.IsLocked = true;
                return new LoginResult { IsLocked = true, ErrorMessage = "Account locked due to multiple failed attempts" };
            }
            return new LoginResult { ErrorMessage = "Invalid password" };
        }

        user.FailedAttempts = 0;
        return new LoginResult
        {
            IsAuthenticated = true,
            Token = Guid.NewGuid().ToString()
        };
    }
}

public class UserAccount
{
    public string Username { get; init; } = string.Empty;
    public string PasswordHash { get; init; } = string.Empty;
    public int FailedAttempts { get; set; }
    public bool IsLocked { get; set; }
}

public class LoginResult
{
    public bool IsAuthenticated { get; init; }
    public bool IsLocked { get; init; }
    public string Token { get; init; } = string.Empty;
    public string ErrorMessage { get; init; } = string.Empty;
}