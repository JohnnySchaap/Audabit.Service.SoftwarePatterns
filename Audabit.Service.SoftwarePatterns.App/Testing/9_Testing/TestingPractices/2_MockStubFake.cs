namespace Audabit.Service.SoftwarePatterns.App.Testing._9_Testing.TestingPractices;

// Mock, Stub, Fake Pattern: Different types of test doubles for different testing needs.
// In this example:
// - Mock: Verify behavior (interactions, method calls) - "Did you call Send()?"
// - Stub: Provide canned responses - "When GetUser() is called, return this user"
// - Fake: Working implementation for testing - "In-memory database instead of real SQL"
//
// MODERN C# USAGE:
// - Use NSubstitute or Moq for mocks and stubs in C#
// - Use in-memory databases (EF Core InMemory) for fakes
// - Prefer fakes for complex dependencies, mocks for verification
// - Stubs for simple return values
public static class MockStubFake
{
    public static void Run()
    {
        Console.WriteLine("Mock Stub Fake Technique: Demonstrating test double types...");
        Console.WriteLine("Mock Stub Fake Technique: Different tools for different testing needs.\n");

        Console.WriteLine("Mock Stub Fake Technique: --- 1. Stub (Canned Responses) ---\n");
        DemonstrateStub();

        Console.WriteLine("\nMock Stub Fake Technique: --- 2. Mock (Behavior Verification) ---\n");
        DemonstrateMock();

        Console.WriteLine("\nMock Stub Fake Technique: --- 3. Fake (Working Implementation) ---\n");
        DemonstrateFake();

        Console.WriteLine("\nMock Stub Fake Technique: Summary:");
        Console.WriteLine("Mock Stub Fake Technique: - Stub: Returns predefined values");
        Console.WriteLine("Mock Stub Fake Technique: - Mock: Verifies method calls and interactions");
        Console.WriteLine("Mock Stub Fake Technique: - Fake: Simplified working implementation");
    }

    private static void DemonstrateStub()
    {
        Console.WriteLine("Mock Stub Fake Technique: Testing OrderService with stubbed repository\n");

        // Stub: Just returns predefined data
        var stubRepository = new StubOrderRepository();
        var orderService = new OrderService(stubRepository, null!);

        var order = orderService.GetOrder("123");
        Console.WriteLine($"Mock Stub Fake Technique: Retrieved order: {order.Id} - {order.Status}");
        Console.WriteLine("Mock Stub Fake Technique: Stub provides canned response - no verification");
    }

    private static void DemonstrateMock()
    {
        Console.WriteLine("Mock Stub Fake Technique: Testing OrderService with mock email sender\n");

        // Mock: Tracks calls and verifies behavior
        var mockEmailSender = new MockEmailSender();
        var stubRepository = new StubOrderRepository();
        var orderService = new OrderService(stubRepository, mockEmailSender);

        orderService.CompleteOrder("123");

        Console.WriteLine("Mock Stub Fake Technique: Verifying interactions:");
        Console.WriteLine($"Mock Stub Fake Technique: ✓ SendEmail was called: {mockEmailSender.SendEmailWasCalled}");
        Console.WriteLine($"Mock Stub Fake Technique: ✓ Email sent to: {mockEmailSender.LastRecipient}");
        Console.WriteLine($"Mock Stub Fake Technique: ✓ Email subject: {mockEmailSender.LastSubject}");
        Console.WriteLine("Mock Stub Fake Technique: Mock verifies behavior, not just return values");
    }

    private static void DemonstrateFake()
    {
        Console.WriteLine("Mock Stub Fake Technique: Testing OrderService with in-memory fake database\n");

        // Fake: Simplified but working implementation
        var fakeDatabase = new FakeOrderDatabase();
        var orderService = new OrderService(fakeDatabase, null!);

        // Fake database actually stores and retrieves data
        fakeDatabase.Save(new Order { Id = "456", Status = "Pending", Total = 99.99m });

        var retrieved = orderService.GetOrder("456");
        Console.WriteLine($"Mock Stub Fake Technique: Retrieved from fake DB: {retrieved.Id} - ${retrieved.Total}");
        Console.WriteLine("Mock Stub Fake Technique: Fake provides real behavior, simplified implementation");
        Console.WriteLine("Mock Stub Fake Technique: Example: In-memory DB instead of SQL Server");
    }
}

// Example service being tested
public class OrderService(IOrderRepository repository, IEmailSender emailSender)
{
    private readonly IOrderRepository _repository = repository;
    private readonly IEmailSender _emailSender = emailSender;

    public Order GetOrder(string id)
    {
        return _repository.GetById(id);
    }

    public void CompleteOrder(string id)
    {
        var order = _repository.GetById(id);
        order.Status = "Completed";
        _repository.Update(order);
        _emailSender.SendEmail("customer@example.com", "Order Completed", "Your order is ready!");
    }
}

// Stub: Returns predefined data
public class StubOrderRepository : IOrderRepository
{
    public Order GetById(string id)
    {
        return new Order { Id = id, Status = "Pending", Total = 50.00m };
    }

    public void Update(Order order) { /* No-op */ }
}

// Mock: Tracks calls for verification
public class MockEmailSender : IEmailSender
{
    public bool SendEmailWasCalled { get; private set; }
    public string? LastRecipient { get; private set; }
    public string? LastSubject { get; private set; }

    public void SendEmail(string to, string subject, string body)
    {
        SendEmailWasCalled = true;
        LastRecipient = to;
        LastSubject = subject;
    }
}

// Fake: Working in-memory implementation
public class FakeOrderDatabase : IOrderRepository
{
    private readonly Dictionary<string, Order> _orders = [];

    public Order GetById(string id)
    {
        return _orders.TryGetValue(id, out var order) ? order : new Order();
    }

    public void Update(Order order)
    {
        _orders[order.Id] = order;
    }

    public void Save(Order order)
    {
        _orders[order.Id] = order;
    }
}

public interface IOrderRepository
{
    Order GetById(string id);
    void Update(Order order);
}

public interface IEmailSender
{
    void SendEmail(string to, string subject, string body);
}

public class Order
{
    public string Id { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal Total { get; set; }
}