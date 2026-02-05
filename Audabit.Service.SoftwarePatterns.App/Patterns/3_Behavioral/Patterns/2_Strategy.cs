namespace Audabit.Service.SoftwarePatterns.App.Patterns._3_Behavioral.Patterns;

// Strategy Pattern: Defines a family of algorithms, encapsulates each one, and makes them interchangeable at runtime.
// In this example:
// - We have different payment strategies (CreditCard, PayPal, Bitcoin) that implement IPaymentStrategy.
// - ShoppingCart uses a strategy to process payment without knowing the specific implementation.
// - The payment strategy can be changed at runtime based on user preference.
// - The key here is that the algorithm (payment method) varies independently from the client (shopping cart).
//
// MODERN C# USAGE:
// Strategy pattern is commonly used with dependency injection in modern .NET:
//   services.AddScoped<IPaymentStrategy, CreditCardStrategy>(); // Register strategy
//   public class CheckoutService(IPaymentStrategy paymentStrategy) { } // Inject strategy
// For runtime strategy selection, use factory pattern with DI:
//   public class PaymentStrategyFactory(IEnumerable<IPaymentStrategy> strategies) {
//       public IPaymentStrategy GetStrategy(string type) => strategies.First(s => s.Type == type);
//   }
// Modern LINQ and delegates can replace simple strategies: list.OrderBy(x => x.Name) vs custom sorting strategies.
public static class Strategy
{
    public static void Run()
    {
        Console.WriteLine("Strategy Pattern: Selecting payment algorithm at runtime...");
        Console.WriteLine("Strategy Pattern: Different payment strategies can be swapped dynamically.\n");

        var cart = new ShoppingCart();
        cart.AddItem("Laptop", 1200);
        cart.AddItem("Mouse", 25);

        Console.WriteLine("Strategy Pattern: --- Paying with Credit Card ---");
        cart.SetPaymentStrategy(new CreditCardStrategy("1234-5678-9012-3456", "John Doe"));
        cart.Checkout();

        Console.WriteLine("\nStrategy Pattern: --- Paying with PayPal ---");
        cart.SetPaymentStrategy(new PayPalStrategy("john.doe@email.com"));
        cart.Checkout();

        Console.WriteLine("\nStrategy Pattern: --- Paying with Bitcoin ---");
        cart.SetPaymentStrategy(new BitcoinStrategy("1A2B3C4D5E6F7G8H9I"));
        cart.Checkout();
    }
}

// Strategy interface
public interface IPaymentStrategy
{
    void Pay(decimal amount);
}

// Concrete Strategies
public sealed class CreditCardStrategy(string cardNumber, string cardHolder) : IPaymentStrategy
{
    private readonly string _cardNumber = cardNumber;
    private readonly string _cardHolder = cardHolder;

    public void Pay(decimal amount)
    {
        Console.WriteLine($"Strategy Pattern: [Credit Card] Paying ${amount:F2} using card {_cardNumber} (holder: {_cardHolder})");
    }
}

public sealed class PayPalStrategy(string email) : IPaymentStrategy
{
    private readonly string _email = email;

    public void Pay(decimal amount)
    {
        Console.WriteLine($"Strategy Pattern: [PayPal] Paying ${amount:F2} using PayPal account {_email}");
    }
}

public sealed class BitcoinStrategy(string walletAddress) : IPaymentStrategy
{
    private readonly string _walletAddress = walletAddress;

    public void Pay(decimal amount)
    {
        Console.WriteLine($"Strategy Pattern: [Bitcoin] Paying ${amount:F2} to wallet {_walletAddress}");
    }
}

// Context
public sealed class ShoppingCart
{
    private readonly List<(string name, decimal price)> _items = [];
    private IPaymentStrategy? _paymentStrategy;

    public void AddItem(string name, decimal price)
    {
        _items.Add((name, price));
        Console.WriteLine($"Strategy Pattern: [Cart] Added {name} (${price:F2})");
    }

    public void SetPaymentStrategy(IPaymentStrategy strategy)
    {
        _paymentStrategy = strategy;
    }

    public void Checkout()
    {
        if (_paymentStrategy == null)
        {
            Console.WriteLine("Strategy Pattern: [Cart] No payment strategy set!");
            return;
        }

        var total = _items.Sum(item => item.price);
        Console.WriteLine($"Strategy Pattern: [Cart] Total: ${total:F2}");
        _paymentStrategy.Pay(total);
    }
}