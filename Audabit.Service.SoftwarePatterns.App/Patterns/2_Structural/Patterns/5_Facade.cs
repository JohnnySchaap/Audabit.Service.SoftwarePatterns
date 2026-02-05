namespace Audabit.Service.SoftwarePatterns.App.Patterns._2_Structural.Patterns;

// Facade Pattern: Provides a simplified interface to a complex subsystem, making it easier to use.
// In this example:
// - We have a complex home theater system with multiple components (DVD player, projector, sound system, lights).
// - The HomeTheaterFacade provides simple methods (WatchMovie, EndMovie) that coordinate all components.
// - Clients don't need to know about individual subsystem components or their interactions.
// - The key here is that the facade hides complexity behind a simple, unified interface for common tasks.
//
// MODERN C# USAGE:
// In Clean Architecture, application services often act as facades:
//   public class OrderService  // Facade to domain/infrastructure
//   {
//       public async Task PlaceOrderAsync(Order order)
//       {
//           await _inventory.Reserve(order.Items);
//           await _payment.Process(order.Total);
//           await _repository.Save(order);
//           await _email.SendConfirmation(order);
//       }
//   }
// The service layer is essentially a facade coordinating multiple subsystems.
public static class Facade
{
    public static void Run()
    {
        Console.WriteLine("Facade Pattern: Simplifying complex subsystem interactions...");
        Console.WriteLine("Facade Pattern: Facade provides simple methods that coordinate multiple subsystem components.\n");

        var homeTheater = new HomeTheaterFacade();

        Console.WriteLine("Facade Pattern: --- Starting Movie ---");
        homeTheater.WatchMovie("The Matrix");

        Console.WriteLine("\nFacade Pattern: --- Ending Movie ---");
        homeTheater.EndMovie();
    }
}

// Subsystem classes
#pragma warning disable CA1822 // Mark members as static - intentionally instance methods to simulate real subsystem objects
public sealed class DvdPlayer
{
    public void On()
    {
        Console.WriteLine("Facade Pattern: [DVD Player] Turning on");
    }

    public void Play(string movie)
    {
        Console.WriteLine($"Facade Pattern: [DVD Player] Playing '{movie}'");
    }

    public void Stop()
    {
        Console.WriteLine("Facade Pattern: [DVD Player] Stopping");
    }

    public void Off()
    {
        Console.WriteLine("Facade Pattern: [DVD Player] Turning off");
    }
}

public sealed class Projector
{
    public void On()
    {
        Console.WriteLine("Facade Pattern: [Projector] Turning on");
    }

    public void WideScreenMode()
    {
        Console.WriteLine("Facade Pattern: [Projector] Setting widescreen mode");
    }

    public void Off()
    {
        Console.WriteLine("Facade Pattern: [Projector] Turning off");
    }
}

public sealed class SoundSystem
{
    public void On()
    {
        Console.WriteLine("Facade Pattern: [Sound System] Turning on");
    }

    public void SetVolume(int level)
    {
        Console.WriteLine($"Facade Pattern: [Sound System] Setting volume to {level}");
    }

    public void Off()
    {
        Console.WriteLine("Facade Pattern: [Sound System] Turning off");
    }
}

public sealed class Lights
{
    public void Dim(int level)
    {
        Console.WriteLine($"Facade Pattern: [Lights] Dimming to {level}%");
    }

    public void On()
    {
        Console.WriteLine("Facade Pattern: [Lights] Turning on");
    }
}

// Facade
public sealed class HomeTheaterFacade
{
    private readonly DvdPlayer _dvd = new();
    private readonly Projector _projector = new();
    private readonly SoundSystem _soundSystem = new();
    private readonly Lights _lights = new();

    public void WatchMovie(string movie)
    {
        Console.WriteLine("Facade Pattern: [Facade] Get ready to watch a movie...");
        _lights.Dim(10);
        _projector.On();
        _projector.WideScreenMode();
        _soundSystem.On();
        _soundSystem.SetVolume(5);
        _dvd.On();
        _dvd.Play(movie);
    }

    public void EndMovie()
    {
        Console.WriteLine("Facade Pattern: [Facade] Shutting down movie theater...");
        _dvd.Stop();
        _dvd.Off();
        _soundSystem.Off();
        _projector.Off();
        _lights.On();
    }
}
#pragma warning restore CA1822 // Mark members as static