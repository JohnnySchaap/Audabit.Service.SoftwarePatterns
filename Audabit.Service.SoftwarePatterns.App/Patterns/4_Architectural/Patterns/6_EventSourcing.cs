namespace Audabit.Service.SoftwarePatterns.App.Patterns._4_Architectural.Patterns;

// Event Sourcing Pattern: Stores all changes to application state as a sequence of events, rather than just the current state, allowing full audit trail and time travel.
// LEVEL: Code-level architecture pattern (how you persist and rebuild state from events)
// In this example:
// - Instead of storing current account balance, we store all deposit/withdrawal events.
// - Current state is derived by replaying all events from the beginning.
// - Events are immutable - they represent facts that happened in the past.
// - The key here is that the event stream becomes the source of truth, not the current state.
//
// MODERN C# USAGE:
// Event Sourcing is increasingly popular in .NET for audit-critical and complex domains:
// - Use Marten library for .NET with PostgreSQL as event store
// - EventStore DB (formerly Event Store) is a dedicated event sourcing database
// - Azure Cosmos DB change feed can implement event sourcing patterns
// - Combine with CQRS: events feed read models for queries
// - Use snapshots to optimize replay performance for long event streams
// - MassTransit and NServiceBus support event sourcing patterns
// - Perfect for financial systems, healthcare, and compliance-heavy domains
// - Enables temporal queries: "What was the balance on January 15th?"
public static class EventSourcing
{
    public static void Run()
    {
        Console.WriteLine("Event Sourcing Pattern: Storing state changes as immutable events...");
        Console.WriteLine("Event Sourcing Pattern: Current state derived by replaying event history.\n");

        var eventStore = new EventStore();
        var accountId = "ACC-12345";

        Console.WriteLine("Event Sourcing Pattern: --- Recording Events ---");

        // Event 1: Account opened
        eventStore.AppendEvent(accountId, new AccountOpenedEvent(accountId, "John Doe", 0m));

        // Event 2: Money deposited
        eventStore.AppendEvent(accountId, new MoneyDepositedEvent(accountId, 1000m));

        // Event 3: Money withdrawn
        eventStore.AppendEvent(accountId, new MoneyWithdrawnEvent(accountId, 250m));

        // Event 4: Money deposited again
        eventStore.AppendEvent(accountId, new MoneyDepositedEvent(accountId, 500m));

        Console.WriteLine("\nEvent Sourcing Pattern: --- Reconstructing Current State ---");

        // Rebuild current state by replaying events
        var account = new BankAccount();
        var events = eventStore.GetEvents(accountId);

        foreach (var @event in events)
        {
            account.Apply(@event);
        }

        Console.WriteLine($"\nEvent Sourcing Pattern: Final Balance: ${account.Balance}");
        Console.WriteLine($"Event Sourcing Pattern: Account Holder: {account.OwnerName}");
        Console.WriteLine($"Event Sourcing Pattern: Total Events: {events.Count}");
    }
}

// ============================================
// EVENTS (Immutable facts)
// ============================================
public abstract record BankAccountEvent(string AccountId)
{
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}

public record AccountOpenedEvent(string AccountId, string OwnerName, decimal InitialBalance)
    : BankAccountEvent(AccountId);

public record MoneyDepositedEvent(string AccountId, decimal Amount)
    : BankAccountEvent(AccountId);

public record MoneyWithdrawnEvent(string AccountId, decimal Amount)
    : BankAccountEvent(AccountId);

// ============================================
// EVENT STORE (Append-only log)
// ============================================
public class EventStore
{
    private readonly Dictionary<string, List<BankAccountEvent>> _eventStreams = [];

    public void AppendEvent(string streamId, BankAccountEvent @event)
    {
        Console.WriteLine($"Event Sourcing Pattern: [Event Store] Appending event: {@event.GetType().Name}");

        if (!_eventStreams.TryGetValue(streamId, out var value))
        {
            value = [];
            _eventStreams[streamId] = value;
        }

        value.Add(@event);
        Console.WriteLine($"Event Sourcing Pattern: [Event Store] Event persisted to stream: {streamId}");
    }

    public List<BankAccountEvent> GetEvents(string streamId)
    {
        Console.WriteLine($"Event Sourcing Pattern: [Event Store] Loading event stream: {streamId}");
        return _eventStreams.GetValueOrDefault(streamId) ?? [];
    }
}

// ============================================
// AGGREGATE (Derived from events)
// ============================================
public class BankAccount
{
    public string AccountId { get; private set; } = string.Empty;
    public string OwnerName { get; private set; } = string.Empty;
    public decimal Balance { get; private set; }

    // Apply events to rebuild state
    public void Apply(BankAccountEvent @event)
    {
        switch (@event)
        {
            case AccountOpenedEvent opened:
                Console.WriteLine($"Event Sourcing Pattern: [Replay] Applying AccountOpenedEvent");
                AccountId = opened.AccountId;
                OwnerName = opened.OwnerName;
                Balance = opened.InitialBalance;
                Console.WriteLine($"Event Sourcing Pattern: [Replay] Account opened for {OwnerName}");
                break;

            case MoneyDepositedEvent deposited:
                Console.WriteLine($"Event Sourcing Pattern: [Replay] Applying MoneyDepositedEvent");
                Balance += deposited.Amount;
                Console.WriteLine($"Event Sourcing Pattern: [Replay] Deposited ${deposited.Amount}, Balance: ${Balance}");
                break;

            case MoneyWithdrawnEvent withdrawn:
                Console.WriteLine($"Event Sourcing Pattern: [Replay] Applying MoneyWithdrawnEvent");
                Balance -= withdrawn.Amount;
                Console.WriteLine($"Event Sourcing Pattern: [Replay] Withdrew ${withdrawn.Amount}, Balance: ${Balance}");
                break;
        }
    }
}