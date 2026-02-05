namespace Audabit.Service.SoftwarePatterns.App.Patterns._3_Behavioral.Patterns;

// Observer Pattern: Defines a one-to-many dependency between objects so that when one object changes state, all its dependents are notified automatically.
// In this example:
// - WeatherStation is the subject that holds the state (temperature).
// - Display devices (PhoneDisplay, WebsiteDisplay) are observers that get notified when temperature changes.
// - Observers subscribe/unsubscribe to the subject to receive updates.
// - The key here is that the subject doesn't need to know about specific observer types, only that they implement IObserver.
//
// MODERN C# USAGE:
// .NET provides built-in event system that implements Observer pattern:
//   public event EventHandler<TemperatureChangedEventArgs>? TemperatureChanged;
//   TemperatureChanged?.Invoke(this, new TemperatureChangedEventArgs(temperature));
// For reactive programming, use System.Reactive (Rx.NET):
//   IObservable<int> source = Observable.Interval(TimeSpan.FromSeconds(1));
//   source.Subscribe(x => Console.WriteLine(x));
// Modern .NET heavily uses Observer pattern through events and IObservable<T>/IObserver<T>.
public static class Observer
{
    public static void Run()
    {
        Console.WriteLine("Observer Pattern: Subject notifies observers of state changes...");
        Console.WriteLine("Observer Pattern: Observers automatically receive updates when subject changes.\n");

        var weatherStation = new WeatherStation();

        Console.WriteLine("Observer Pattern: --- Registering Observers ---");
        var phoneDisplay = new PhoneDisplay("iPhone");
        var websiteDisplay = new WebsiteDisplay("WeatherApp.com");

        weatherStation.Attach(phoneDisplay);
        weatherStation.Attach(websiteDisplay);

        Console.WriteLine("\nObserver Pattern: --- Changing Temperature ---");
        weatherStation.Temperature = 25.5f;

        Console.WriteLine("\nObserver Pattern: --- Removing Phone Observer ---");
        weatherStation.Detach(phoneDisplay);

        Console.WriteLine("\nObserver Pattern: --- Changing Temperature Again ---");
        weatherStation.Temperature = 30.0f;
    }
}

// Observer interface
public interface IObserver
{
    void Update(float temperature);
}

// Subject interface
public interface ISubject
{
    void Attach(IObserver observer);
    void Detach(IObserver observer);
    void Notify();
}

// Concrete Subject
public sealed class WeatherStation : ISubject
{
    private readonly List<IObserver> _observers = [];
    private float _temperature;

    public float Temperature
    {
        get => _temperature;
        set
        {
            Console.WriteLine($"Observer Pattern: [WeatherStation] Temperature changed to {value}°C");
            _temperature = value;
            Notify();
        }
    }

    public void Attach(IObserver observer)
    {
        Console.WriteLine($"Observer Pattern: [WeatherStation] Observer attached");
        _observers.Add(observer);
    }

    public void Detach(IObserver observer)
    {
        Console.WriteLine($"Observer Pattern: [WeatherStation] Observer detached");
        _observers.Remove(observer);
    }

    public void Notify()
    {
        Console.WriteLine($"Observer Pattern: [WeatherStation] Notifying {_observers.Count} observer(s)...");
        foreach (var observer in _observers)
        {
            observer.Update(_temperature);
        }
    }
}

// Concrete Observers
public sealed class PhoneDisplay(string deviceName) : IObserver
{
    private readonly string _deviceName = deviceName;

    public void Update(float temperature)
    {
        Console.WriteLine($"Observer Pattern: [PhoneDisplay - {_deviceName}] Received update: {temperature}°C");
    }
}

public sealed class WebsiteDisplay(string siteName) : IObserver
{
    private readonly string _siteName = siteName;

    public void Update(float temperature)
    {
        Console.WriteLine($"Observer Pattern: [WebsiteDisplay - {_siteName}] Received update: {temperature}°C");
    }
}