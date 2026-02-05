namespace Audabit.Service.SoftwarePatterns.App.Principles._0_Principles.Principles;

// Composition Over Inheritance: Favor object composition over class inheritance to achieve code reuse
// and flexibility. Compose objects to create complex behavior instead of inheriting from base classes.

// MODERN C# USAGE:
// - Dependency injection for composition
// - Interface-based design
// - Strategy pattern, Decorator pattern
// - Avoid deep inheritance hierarchies
// - Use composition for "has-a" relationships, inheritance for "is-a" only when appropriate
public static class CompositionOverInheritance
{
    public static void Run()
    {
        Console.WriteLine("Composition Over Inheritance: Demonstrating flexibility through composition...");
        Console.WriteLine("Composition Over Inheritance: Favor object composition over class inheritance for better flexibility.\n");

        // Example 1: Inheritance problem - rigid hierarchy
        Console.WriteLine("Composition Over Inheritance: --- Example 1: Inheritance Problem ---\n");

        Console.WriteLine("Composition Over Inheritance: ❌ BAD: Deep inheritance hierarchy:");
        Console.WriteLine("Composition Over Inheritance: ❌ BAD:   Animal → Bird → FlyingBird → Eagle");
        Console.WriteLine("Composition Over Inheritance: ❌ BAD:   Animal → Bird → FlightlessBird → Penguin");
        Console.WriteLine("Composition Over Inheritance: ❌ BAD: What about bats (mammals that fly)?");
        Console.WriteLine("Composition Over Inheritance: ❌ BAD: Hard to add new behaviors without restructuring\n");

        // ✅ GOOD: Composition-based approach
        Console.WriteLine("Composition Over Inheritance: ✅ GOOD: Composition approach:");
        var eagle = new AnimalComposition("Eagle", new CanFly(), new CanWalk());
        eagle.PerformMovement();

        var penguin = new AnimalComposition("Penguin", new CanSwim(), new CanWalk());
        penguin.PerformMovement();

        var bat = new AnimalComposition("Bat", new CanFly(), new CanEcholocate());
        bat.PerformMovement();
        Console.WriteLine();

        // Example 2: Multiple inheritance problem
        Console.WriteLine("Composition Over Inheritance: --- Example 2: Multiple Inheritance Problem ---\n");

        Console.WriteLine("Composition Over Inheritance: ❌ BAD: C# doesn't support multiple inheritance:");
        Console.WriteLine("Composition Over Inheritance: ❌ BAD:   class SmartPhone : Device, Camera, Phone, GPS { }");
        Console.WriteLine("Composition Over Inheritance: ❌ BAD: Cannot inherit from multiple classes\n");

        Console.WriteLine("Composition Over Inheritance: ✅ GOOD: Compose capabilities:");
        var smartphone = new SmartPhoneComposition(
            new CameraCapability(),
            new PhoneCapability(),
            new GpsCapability());
        smartphone.UseAllFeatures();
        Console.WriteLine();

        // Example 3: Fragile base class problem
        Console.WriteLine("Composition Over Inheritance: --- Example 3: Fragile Base Class ---\n");

        Console.WriteLine("Composition Over Inheritance: ❌ BAD: Inheritance couples child to parent:");
        var inheritedLogger = new InheritedFileLogger();
        inheritedLogger.Log("Test message");
        Console.WriteLine("Composition Over Inheritance: ❌ BAD: If base class changes, all children break\n");

        Console.WriteLine("Composition Over Inheritance: ✅ GOOD: Composition decouples components:");
        var composedLogger = new ComposedLogger(new FileWriter(), new ConsoleWriter());
        composedLogger.Log("Test message");
        Console.WriteLine("Composition Over Inheritance: ✅ GOOD: Writers can change independently\n");

        // Example 4: Strategy pattern (composition in action)
        Console.WriteLine("Composition Over Inheritance: --- Example 4: Strategy Pattern ---\n");

        Console.WriteLine("Composition Over Inheritance: ✅ GOOD: Payment processing with composition:");
        var processor = new PaymentProcessor();

        processor.SetStrategy(new CreditCardStrategy());
        processor.ProcessPayment(100m);

        processor.SetStrategy(new PayPalStrategy());
        processor.ProcessPayment(100m);

        processor.SetStrategy(new CryptoStrategy());
        processor.ProcessPayment(100m);
        Console.WriteLine();

        // Example 5: Decorator pattern (composition for extending behavior)
        Console.WriteLine("Composition Over Inheritance: --- Example 5: Decorator Pattern ---\n");

        Console.WriteLine("Composition Over Inheritance: ✅ GOOD: Add features through composition:");
        IMessageNotificationService notification = new EmailMessageNotification();
        notification.Send("Hello!");

        // Wrap with logging
        notification = new LoggingMessageNotificationDecorator(notification);
        notification.Send("Hello with logging!");

        // Wrap with retry
        notification = new RetryMessageNotificationDecorator(notification);
        notification.Send("Hello with logging and retry!");
        Console.WriteLine();

        // Example 6: When inheritance IS appropriate
        Console.WriteLine("Composition Over Inheritance: --- Example 6: When Inheritance Works ---\n");

        Console.WriteLine("Composition Over Inheritance: ✅ GOOD: Inheritance for true 'is-a' relationships:");
        Circle circle = new() { Radius = 5 };
        CompositionRectangle rect = new() { Width = 4, Height = 6 };

        Console.WriteLine($"Composition Over Inheritance: Circle area: {circle.CalculateArea()}");
        Console.WriteLine($"Composition Over Inheritance: Rectangle area: {rect.CalculateArea()}");
        Console.WriteLine("Composition Over Inheritance: ✅ GOOD: Shape hierarchy is appropriate (true is-a)\n");

        Console.WriteLine("Composition Over Inheritance: Key Takeaways:");
        Console.WriteLine("Composition Over Inheritance: ✓ Prefer composition for flexibility");
        Console.WriteLine("Composition Over Inheritance: ✓ Use interfaces to define contracts");
        Console.WriteLine("Composition Over Inheritance: ✓ Inject dependencies for loose coupling");
        Console.WriteLine("Composition Over Inheritance: ✓ Inheritance is OK for true 'is-a' relationships");
        Console.WriteLine("Composition Over Inheritance: ✓ Avoid deep inheritance hierarchies");
        Console.WriteLine("Composition Over Inheritance: ✓ Composition enables runtime behavior changes");
    }
}

// ✅ GOOD: Composition-based animal system
internal interface IMovementBehavior
{
    void Move();
}

internal class CanFly : IMovementBehavior
{
    public void Move()
    {
        Console.WriteLine("Composition Over Inheritance: ✅ Flying through the air");
    }
}

internal class CanWalk : IMovementBehavior
{
    public void Move()
    {
        Console.WriteLine("Composition Over Inheritance: ✅ Walking on the ground");
    }
}

internal class CanSwim : IMovementBehavior
{
    public void Move()
    {
        Console.WriteLine("Composition Over Inheritance: ✅ Swimming in water");
    }
}

internal class CanEcholocate : IMovementBehavior
{
    public void Move()
    {
        Console.WriteLine("Composition Over Inheritance: ✅ Navigating with echolocation");
    }
}

internal class AnimalComposition(string name, params IMovementBehavior[] behaviors)
{
    public void PerformMovement()
    {
        Console.WriteLine($"Composition Over Inheritance: {name} can:");
        foreach (var behavior in behaviors)
        {
            behavior.Move();
        }
    }
}

// ✅ GOOD: Smartphone with composition
internal interface ICapability
{
    void Use();
}

internal class CameraCapability : ICapability
{
    public void Use()
    {
        Console.WriteLine("Composition Over Inheritance: ✅ Taking photos");
    }
}

internal class PhoneCapability : ICapability
{
    public void Use()
    {
        Console.WriteLine("Composition Over Inheritance: ✅ Making calls");
    }
}

internal class GpsCapability : ICapability
{
    public void Use()
    {
        Console.WriteLine("Composition Over Inheritance: ✅ Getting directions");
    }
}

internal class SmartPhoneComposition(params ICapability[] capabilities)
{
    public void UseAllFeatures()
    {
        Console.WriteLine("Composition Over Inheritance: SmartPhone features:");
        foreach (var capability in capabilities)
        {
            capability.Use();
        }
    }
}

// ❌ BAD: Inheritance-based logger
internal class BaseLogger
{
    public virtual void Log(string message)
    {
        Console.WriteLine($"Composition Over Inheritance: ❌ [Base] {message}");
    }
}

internal class InheritedFileLogger : BaseLogger
{
    public override void Log(string message)
    {
        base.Log(message);
        Console.WriteLine("Composition Over Inheritance: ❌ Writing to file...");
    }
}

// ✅ GOOD: Composition-based logger
internal interface IWriter
{
    void Write(string message);
}

internal class FileWriter : IWriter
{
    public void Write(string message)
    {
        Console.WriteLine($"Composition Over Inheritance: ✅ [File] {message}");
    }
}

internal class ConsoleWriter : IWriter
{
    public void Write(string message)
    {
        Console.WriteLine($"Composition Over Inheritance: ✅ [Console] {message}");
    }
}

internal class ComposedLogger(params IWriter[] writers)
{
    public void Log(string message)
    {
        foreach (var writer in writers)
        {
            writer.Write(message);
        }
    }
}

// ✅ GOOD: Strategy pattern
internal interface IPaymentStrategy
{
    void Pay(decimal amount);
}

internal class CreditCardStrategy : IPaymentStrategy
{
    public void Pay(decimal amount)
    {
        Console.WriteLine($"Composition Over Inheritance: ✅ Paid ${amount} with Credit Card");
    }
}

internal class PayPalStrategy : IPaymentStrategy
{
    public void Pay(decimal amount)
    {
        Console.WriteLine($"Composition Over Inheritance: ✅ Paid ${amount} with PayPal");
    }
}

internal class CryptoStrategy : IPaymentStrategy
{
    public void Pay(decimal amount)
    {
        Console.WriteLine($"Composition Over Inheritance: ✅ Paid ${amount} with Cryptocurrency");
    }
}

internal class PaymentProcessor
{
    private IPaymentStrategy? _strategy;

    public void SetStrategy(IPaymentStrategy strategy)
    {
        _strategy = strategy;
    }

    public void ProcessPayment(decimal amount)
    {
        _strategy?.Pay(amount);
    }
}

// ✅ GOOD: Decorator pattern
internal interface IMessageNotificationService
{
    void Send(string message);
}

internal class EmailMessageNotification : IMessageNotificationService
{
    public void Send(string message)
    {
        Console.WriteLine($"Composition Over Inheritance: ✅ Email sent: {message}");
    }
}

internal class LoggingMessageNotificationDecorator(IMessageNotificationService inner) : IMessageNotificationService
{
    public void Send(string message)
    {
        Console.WriteLine("Composition Over Inheritance: ✅ [Logging] Before send");
        inner.Send(message);
        Console.WriteLine("Composition Over Inheritance: ✅ [Logging] After send");
    }
}

internal class RetryMessageNotificationDecorator(IMessageNotificationService inner) : IMessageNotificationService
{
    public void Send(string message)
    {
        Console.WriteLine("Composition Over Inheritance: ✅ [Retry] Attempting send...");
        inner.Send(message);
    }
}

// ✅ GOOD: When inheritance IS appropriate
internal abstract class Shape
{
    public abstract double CalculateArea();
}

internal class Circle : Shape
{
    public double Radius { get; init; }

    public override double CalculateArea()
    {
        return Math.PI * Radius * Radius;
    }
}

internal class CompositionRectangle : Shape
{
    public double Width { get; init; }
    public double Height { get; init; }

    public override double CalculateArea()
    {
        return Width * Height;
    }
}