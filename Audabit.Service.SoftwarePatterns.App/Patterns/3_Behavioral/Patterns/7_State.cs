namespace Audabit.Service.SoftwarePatterns.App.Patterns._3_Behavioral.Patterns;

// State Pattern: Allows an object to alter its behavior when its internal state changes, appearing to change its class.
// In this example:
// - Order has different states (Pending, Processing, Shipped, Delivered).
// - Each state defines its own behavior for Next() and Cancel() operations.
// - State transitions are encapsulated within the state objects themselves.
// - The key here is that behavior changes based on internal state without complex conditionals.
//
// MODERN C# USAGE:
// For simple state machines, use discriminated unions with pattern matching:
//   OrderState state = new Pending();
//   var result = state switch {
//       Pending => ProcessPending(),
//       Processing => ProcessProcessing(),
//       _ => throw new InvalidOperationException()
//   };
// For complex workflows, use dedicated state machine libraries like Stateless or Automatonymous.
// EF Core can track entity state changes (Added, Modified, Deleted, Unchanged).
public static class State
{
    public static void Run()
    {
        Console.WriteLine("State Pattern: Object behavior changes based on internal state...");
        Console.WriteLine("State Pattern: Order transitions through different states with different behaviors.\n");

        var order = new Order();

        Console.WriteLine("State Pattern: --- Processing Order Through States ---");
        order.Next();  // Pending → Processing
        order.Next();  // Processing → Shipped
        order.Next();  // Shipped → Delivered
        order.Next();  // Already delivered

        Console.WriteLine("\nState Pattern: --- Cancelling Order in Different States ---");
        var order2 = new Order();
        order2.Next();  // Pending → Processing
        order2.Cancel();  // Can cancel in Processing
    }
}

// State interface
public interface IOrderState
{
    void Next(Order order);
    void Cancel(Order order);
    string GetStatus();
}

// Concrete States
public sealed class PendingState : IOrderState
{
    public void Next(Order order)
    {
        Console.WriteLine("State Pattern: [PendingState] ➡️ Moving to Processing");
        order.SetState(new ProcessingState());
    }

    public void Cancel(Order order)
    {
        Console.WriteLine("State Pattern: [PendingState] ❌ Order cancelled");
        order.SetState(new CancelledState());
    }

    public string GetStatus()
    {
        return "Pending";
    }
}

public sealed class ProcessingState : IOrderState
{
    public void Next(Order order)
    {
        Console.WriteLine("State Pattern: [ProcessingState] ➡️ Moving to Shipped");
        order.SetState(new ShippedState());
    }

    public void Cancel(Order order)
    {
        Console.WriteLine("State Pattern: [ProcessingState] ❌ Order cancelled (may incur fees)");
        order.SetState(new CancelledState());
    }

    public string GetStatus()
    {
        return "Processing";
    }
}

public sealed class ShippedState : IOrderState
{
    public void Next(Order order)
    {
        Console.WriteLine("State Pattern: [ShippedState] ➡️ Moving to Delivered");
        order.SetState(new DeliveredState());
    }

    public void Cancel(Order order)
    {
        Console.WriteLine("State Pattern: [ShippedState] ⚠️ Cannot cancel - already shipped. Request return instead.");
    }

    public string GetStatus()
    {
        return "Shipped";
    }
}

public sealed class DeliveredState : IOrderState
{
    public void Next(Order order)
    {
        Console.WriteLine("State Pattern: [DeliveredState] ✅ Order already delivered");
    }

    public void Cancel(Order order)
    {
        Console.WriteLine("State Pattern: [DeliveredState] ⚠️ Cannot cancel - already delivered. Request return instead.");
    }

    public string GetStatus()
    {
        return "Delivered";
    }
}

public sealed class CancelledState : IOrderState
{
    public void Next(Order order)
    {
        Console.WriteLine("State Pattern: [CancelledState] ⚠️ Cannot proceed - order is cancelled");
    }

    public void Cancel(Order order)
    {
        Console.WriteLine("State Pattern: [CancelledState] ℹ️ Order already cancelled");
    }

    public string GetStatus()
    {
        return "Cancelled";
    }
}

// Context
public sealed class Order
{
    private IOrderState _state = new PendingState();

    public void SetState(IOrderState state)
    {
        _state = state;
        Console.WriteLine($"State Pattern: [Order] State changed to: {_state.GetStatus()}");
    }

    public void Next()
    {
        _state.Next(this);
    }

    public void Cancel()
    {
        _state.Cancel(this);
    }

    public string GetStatus()
    {
        return _state.GetStatus();
    }
}