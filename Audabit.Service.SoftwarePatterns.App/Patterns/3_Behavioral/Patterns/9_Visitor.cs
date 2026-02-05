namespace Audabit.Service.SoftwarePatterns.App.Patterns._3_Behavioral.Patterns;

// Visitor Pattern: Separates an algorithm from the object structure on which it operates, allowing new operations to be added without modifying the objects.
// In this example:
// - Shopping items (Book, Food, Electronics) accept visitors.
// - Visitors (TaxCalculator, ShippingCost, Discount) implement different operations on these items.
// - New operations can be added by creating new visitors without changing the item classes.
// - The key here is that operations are defined externally, not within the element classes.
//
// MODERN C# USAGE:
// Pattern matching with switch expressions often replaces the Visitor pattern:
//   decimal CalculateTax(IShoppingItem item) => item switch {
//       Book b => 0m,  // Tax exempt
//       Food f => f.Price * 0.05m,
//       Electronics e => e.Price * 0.15m,
//       _ => throw new ArgumentException()
//   };
// For complex hierarchies with many operations, Visitor is still useful.
// C# 11 list patterns and type patterns make traversal simpler.
public static class Visitor
{
    public static void Run()
    {
        Console.WriteLine("Visitor Pattern: Separating operations from object structure...");
        Console.WriteLine("Visitor Pattern: Different visitors perform different operations on the same items.\n");

        var cart = new List<IShoppingItem>
        {
            new ShoppingBook("Design Patterns", 50.00m),
            new ShoppingFood("Organic Milk", 5.00m, isPerishable: true),
            new ShoppingElectronics("Laptop", 1000.00m, warrantyYears: 2),
            new ShoppingBook("Clean Code", 40.00m),
            new ShoppingFood("Rice", 15.00m, isPerishable: false)
        };

        Console.WriteLine("Visitor Pattern: --- Calculating Tax ---");
        var taxVisitor = new TaxCalculatorVisitor();
        foreach (var item in cart)
        {
            item.Accept(taxVisitor);
        }
        Console.WriteLine($"Visitor Pattern: Total Tax: ${taxVisitor.TotalTax:F2}\n");

        Console.WriteLine("Visitor Pattern: --- Calculating Shipping ---");
        var shippingVisitor = new ShippingCostVisitor();
        foreach (var item in cart)
        {
            item.Accept(shippingVisitor);
        }
        Console.WriteLine($"Visitor Pattern: Total Shipping: ${shippingVisitor.TotalShipping:F2}\n");

        Console.WriteLine("Visitor Pattern: --- Applying Discounts ---");
        var discountVisitor = new DiscountVisitor();
        foreach (var item in cart)
        {
            item.Accept(discountVisitor);
        }
        Console.WriteLine($"Visitor Pattern: Total Discount: ${discountVisitor.TotalDiscount:F2}");
    }
}

// Element interface
public interface IShoppingItem
{
    void Accept(IShoppingItemVisitor visitor);
}

// Visitor interface
public interface IShoppingItemVisitor
{
    void VisitBook(ShoppingBook book);
    void VisitFood(ShoppingFood food);
    void VisitElectronics(ShoppingElectronics electronics);
}

// Concrete Elements
public sealed class ShoppingBook(string title, decimal price) : IShoppingItem
{
    public string Title { get; } = title;
    public decimal Price { get; } = price;

    public void Accept(IShoppingItemVisitor visitor)
    {
        visitor.VisitBook(this);
    }
}

public sealed class ShoppingFood(string name, decimal price, bool isPerishable) : IShoppingItem
{
    public string Name { get; } = name;
    public decimal Price { get; } = price;
    public bool IsPerishable { get; } = isPerishable;

    public void Accept(IShoppingItemVisitor visitor)
    {
        visitor.VisitFood(this);
    }
}

public sealed class ShoppingElectronics(string name, decimal price, int warrantyYears) : IShoppingItem
{
    public string Name { get; } = name;
    public decimal Price { get; } = price;
    public int WarrantyYears { get; } = warrantyYears;

    public void Accept(IShoppingItemVisitor visitor)
    {
        visitor.VisitElectronics(this);
    }
}

// Concrete Visitors
public sealed class TaxCalculatorVisitor : IShoppingItemVisitor
{
    public decimal TotalTax { get; private set; } = 0;

    public void VisitBook(ShoppingBook book)
    {
        var tax = 0m;
        Console.WriteLine($"Visitor Pattern: [TaxCalculator] '{book.Title}': ${tax:F2} (0% - tax exempt)");
        TotalTax += tax;
    }

    public void VisitFood(ShoppingFood food)
    {
        var tax = food.Price * 0.05m;
        Console.WriteLine($"Visitor Pattern: [TaxCalculator] '{food.Name}': ${tax:F2} (5%)");
        TotalTax += tax;
    }

    public void VisitElectronics(ShoppingElectronics electronics)
    {
        var tax = electronics.Price * 0.15m;
        Console.WriteLine($"Visitor Pattern: [TaxCalculator] '{electronics.Name}': ${tax:F2} (15%)");
        TotalTax += tax;
    }
}

public sealed class ShippingCostVisitor : IShoppingItemVisitor
{
    public decimal TotalShipping { get; private set; } = 0;

    public void VisitBook(ShoppingBook book)
    {
        var shipping = 3.00m;
        Console.WriteLine($"Visitor Pattern: [ShippingCost] '{book.Title}': ${shipping:F2} (flat rate)");
        TotalShipping += shipping;
    }

    public void VisitFood(ShoppingFood food)
    {
        var shipping = food.IsPerishable ? 15.00m : 5.00m;
        var type = food.IsPerishable ? "expedited" : "standard";
        Console.WriteLine($"Visitor Pattern: [ShippingCost] '{food.Name}': ${shipping:F2} ({type})");
        TotalShipping += shipping;
    }

    public void VisitElectronics(ShoppingElectronics electronics)
    {
        var shipping = electronics.Price * 0.10m;
        Console.WriteLine($"Visitor Pattern: [ShippingCost] '{electronics.Name}': ${shipping:F2} (with insurance)");
        TotalShipping += shipping;
    }
}

public sealed class DiscountVisitor : IShoppingItemVisitor
{
    public decimal TotalDiscount { get; private set; } = 0;

    public void VisitBook(ShoppingBook book)
    {
        var discount = book.Price * 0.20m;
        Console.WriteLine($"Visitor Pattern: [Discount] '{book.Title}': -${discount:F2} (20% off)");
        TotalDiscount += discount;
    }

    public void VisitFood(ShoppingFood food)
    {
        Console.WriteLine($"Visitor Pattern: [Discount] '{food.Name}': $0.00 (no discount)");
    }

    public void VisitElectronics(ShoppingElectronics electronics)
    {
        var discount = electronics.Price * 0.10m;
        Console.WriteLine($"Visitor Pattern: [Discount] '{electronics.Name}': -${discount:F2} (10% off)");
        TotalDiscount += discount;
    }
}