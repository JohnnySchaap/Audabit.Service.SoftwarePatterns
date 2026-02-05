namespace Audabit.Service.SoftwarePatterns.App.Patterns._2_Structural.Patterns;

// Bridge Pattern: Decouples an abstraction from its implementation so that the two can vary independently.
// In this example:
// - We have different devices (TV, Radio) and different ways to control them (BasicRemote, AdvancedRemote).
// - The abstraction (RemoteControl) is separated from the implementation (IDevice).
// - Instead of tightly coupling each device with each control type, they're connected loosely through a bridge.
// - RemoteControl (abstraction) has COMPOSITION with IDevice (implementation interface).
// - Each remote contains an IDevice field - this is the "bridge" connection.
// - All devices implement IDevice with the same functions (TurnOn, TurnOff, SetVolume, etc.).
// - This avoids explosion: Without Bridge, you'd need TVBasicRemote, TVAdvancedRemote, RadioBasicRemote, RadioAdvancedRemote, etc.
// - The key here is that devices and remotes can evolve independently - new devices or new remotes can be added without modifying existing code.
//
// MODERN C# USAGE:
// The Bridge pattern is still relevant in modern .NET when you have two orthogonal hierarchies that need to vary independently.
// Use dependency injection to inject the implementation interface:
//   public class RemoteControl(IDevice device) { } // Primary constructor
// Register both abstraction and implementation in DI:
//   services.AddScoped<IDevice, Television>();
//   services.AddScoped<RemoteControl, AdvancedRemote>();
// Modern .NET's composition-based approach naturally supports the Bridge pattern through DI.
public static class Bridge
{
    public static void Run()
    {
        Console.WriteLine("Bridge Pattern: Decoupling abstraction from implementation...");
        Console.WriteLine("Bridge Pattern: Devices and remotes can vary independently without tight coupling.\n");

        Console.WriteLine("Bridge Pattern: --- Basic Remote with TV ---");
        IDevice tv = new Television();
        RemoteControl basicRemote = new BasicRemote(tv);
        basicRemote.TogglePower();
        basicRemote.VolumeUp();
        basicRemote.VolumeUp();

        Console.WriteLine("\nBridge Pattern: --- Advanced Remote with TV ---");
        RemoteControl advancedRemote = new AdvancedRemote(tv);
        advancedRemote.TogglePower();
        advancedRemote.VolumeUp();
        ((AdvancedRemote)advancedRemote).Mute();

        Console.WriteLine("\nBridge Pattern: --- Basic Remote with Radio ---");
        IDevice radio = new Radio();
        RemoteControl radioRemote = new BasicRemote(radio);
        radioRemote.TogglePower();
        radioRemote.VolumeUp();
        radioRemote.VolumeDown();
    }
}

// Implementation interface - the device being controlled
public interface IDevice
{
    void TurnOn();
    void TurnOff();
    void SetVolume(int volume);
    int GetVolume();
    bool IsEnabled();
}

// Concrete implementations - actual devices
public sealed class Television : IDevice
{
    private bool _isOn;
    private int _volume = 30;

    public void TurnOn()
    {
        _isOn = true;
        Console.WriteLine("Bridge Pattern: [TV] Turning on");
    }

    public void TurnOff()
    {
        _isOn = false;
        Console.WriteLine("Bridge Pattern: [TV] Turning off");
    }

    public void SetVolume(int volume)
    {
        _volume = volume;
        Console.WriteLine($"Bridge Pattern: [TV] Setting volume to {_volume}%");
    }

    public int GetVolume() { return _volume; }

    public bool IsEnabled() { return _isOn; }
}

public sealed class Radio : IDevice
{
    private bool _isOn;
    private int _volume = 50;

    public void TurnOn()
    {
        _isOn = true;
        Console.WriteLine("Bridge Pattern: [Radio] Turning on");
    }

    public void TurnOff()
    {
        _isOn = false;
        Console.WriteLine("Bridge Pattern: [Radio] Turning off");
    }

    public void SetVolume(int volume)
    {
        _volume = volume;
        Console.WriteLine($"Bridge Pattern: [Radio] Setting volume to {_volume}%");
    }

    public int GetVolume() { return _volume; }

    public bool IsEnabled() { return _isOn; }
}

// Abstraction - the remote control
// NOTE: RemoteControl must be an abstract CLASS (not interface) to hold a reference to the IDevice object.
// Interfaces cannot store fields/state, but composition requires storing the device reference.
public abstract class RemoteControl(IDevice device)
{
    protected IDevice Device { get; } = device;

    public void TogglePower()
    {
        if (Device.IsEnabled())
        {
            Device.TurnOff();
        }
        else
        {
            Device.TurnOn();
        }
    }

    public void VolumeUp()
    {
        var currentVolume = Device.GetVolume();
        Device.SetVolume(currentVolume + 10);
    }

    public void VolumeDown()
    {
        var currentVolume = Device.GetVolume();
        Device.SetVolume(currentVolume - 10);
    }
}

// Refined abstractions - different types of remotes
public sealed class BasicRemote(IDevice device) : RemoteControl(device)
{
    // Basic remote only uses the base functionality
}

public sealed class AdvancedRemote(IDevice device) : RemoteControl(device)
{
    public void Mute()
    {
        Console.WriteLine("Bridge Pattern: [Advanced Remote] Muting device");
        Device.SetVolume(0);
    }
}