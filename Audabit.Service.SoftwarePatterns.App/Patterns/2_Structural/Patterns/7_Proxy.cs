namespace Audabit.Service.SoftwarePatterns.App.Patterns._2_Structural.Patterns;

// Proxy Pattern: Provides a surrogate or placeholder object to control access to another object.
// In this example:
// - We have an ImageProxy that controls access to the real HighResolutionImage.
// - The proxy delays loading the expensive image until it's actually needed (lazy loading).
// - The proxy can also add access control, logging, or caching functionality.
// - The key here is that the proxy maintains the same interface as the real object, controlling or enhancing access to it.
//
// MODERN C# USAGE:
// For simple lazy loading, modern C# provides Lazy<T>:
//   private readonly Lazy<ExpensiveObject> _lazy = new(() => new ExpensiveObject());
//   var obj = _lazy.Value; // Created on first access
// Use full Proxy pattern when you need access control, logging, or more complex behavior beyond lazy initialization.
public static class Proxy
{
#pragma warning disable CA1859 // Use concrete types when possible - intentionally using interface to demonstrate pattern
    public static void Run()
    {
        Console.WriteLine("Proxy Pattern: Controlling access to objects through a proxy...");
        Console.WriteLine("Proxy Pattern: Proxy delays expensive operations until they are actually needed.\n");

        Console.WriteLine("Proxy Pattern: --- Creating Image Proxies ---");
        IImage image1 = new ImageProxy("photo1.jpg");
        IImage image2 = new ImageProxy("photo2.jpg");

        Console.WriteLine("\nProxy Pattern: --- Displaying Images (First Access) ---");
        image1.Display(); // Loads and displays
        image2.Display(); // Loads and displays

        Console.WriteLine("\nProxy Pattern: --- Displaying Images Again (Cached) ---");
        image1.Display(); // Already loaded, just displays
        image2.Display(); // Already loaded, just displays
    }
#pragma warning restore CA1859 // Use concrete types when possible
}

// Subject interface
public interface IImage
{
    void Display();
}

// Real subject - expensive to create
public sealed class HighResolutionImage(string filename) : IImage
{
    private readonly string _filename = filename;

    public void LoadFromDisk()
    {
        Console.WriteLine($"Proxy Pattern: [Real Image] Loading high-resolution image '{_filename}' from disk (expensive operation)...");
    }

    public void Display()
    {
        Console.WriteLine($"Proxy Pattern: [Real Image] Displaying '{_filename}'");
    }
}

// Proxy - controls access to real subject
public sealed class ImageProxy(string filename) : IImage
{
    private readonly string _filename = filename;
    private HighResolutionImage? _realImage;

    public void Display()
    {
        if (_realImage == null)
        {
            Console.WriteLine($"Proxy Pattern: [Proxy] First access to '{_filename}', loading real image...");
            _realImage = new HighResolutionImage(_filename);
            _realImage.LoadFromDisk();
        }
        else
        {
            Console.WriteLine($"Proxy Pattern: [Proxy] Image '{_filename}' already loaded, using cached version");
        }

        _realImage.Display();
    }
}