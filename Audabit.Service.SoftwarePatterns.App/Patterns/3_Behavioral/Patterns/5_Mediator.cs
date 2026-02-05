namespace Audabit.Service.SoftwarePatterns.App.Patterns._3_Behavioral.Patterns;

// Mediator Pattern: Defines an object that encapsulates how a set of objects interact, promoting loose coupling by preventing objects from referring to each other explicitly.
// In this example:
// - ChatRoom is the mediator that coordinates communication between users.
// - Users send messages through the mediator instead of directly to each other.
// - The mediator handles the routing and broadcasting logic.
// - The key here is that components communicate through a central mediator rather than directly.
//
// MODERN C# USAGE:
// MediatR library is the standard implementation of Mediator pattern in .NET:
//   public record SendMessageCommand(string UserId, string Message) : IRequest;
//   public class SendMessageHandler : IRequestHandler<SendMessageCommand> { }
//   await mediator.Send(new SendMessageCommand("user1", "Hello"));
// Used extensively in CQRS architectures to decouple command/query handlers from controllers.
// SignalR hubs act as mediators for real-time communication between clients.
public static class Mediator
{
    public static void Run()
    {
        Console.WriteLine("Mediator Pattern: Coordinating communication through a central mediator...");
        Console.WriteLine("Mediator Pattern: Users communicate through the chat room instead of directly.\n");

        var chatRoom = new ChatRoom();

        var user1 = new User("Alice", chatRoom);
        var user2 = new User("Bob", chatRoom);
        var user3 = new User("Charlie", chatRoom);

        chatRoom.Register(user1);
        chatRoom.Register(user2);
        chatRoom.Register(user3);

        Console.WriteLine("Mediator Pattern: --- Sending Messages ---");
        user1.Send("Hello everyone!");
        user2.Send("Hi Alice!");
        user3.Send("Hey folks!");
    }
}

// Mediator interface
public interface IChatMediator
{
    void Register(User user);
    void SendMessage(string message, User sender);
}

// Concrete Mediator
public sealed class ChatRoom : IChatMediator
{
    private readonly List<User> _users = [];

    public void Register(User user)
    {
        Console.WriteLine($"Mediator Pattern: [ChatRoom] {user.Name} joined the chat");
        _users.Add(user);
    }

    public void SendMessage(string message, User sender)
    {
        Console.WriteLine($"Mediator Pattern: [ChatRoom] Broadcasting message from {sender.Name}");
        foreach (var user in _users)
        {
            if (user != sender)
            {
                user.Receive(message, sender.Name);
            }
        }
    }
}

// Colleague
public sealed class User(string name, IChatMediator mediator)
{
    public string Name { get; } = name;
    private readonly IChatMediator _mediator = mediator;

    public void Send(string message)
    {
        Console.WriteLine($"Mediator Pattern: [{Name}] Sending: '{message}'");
        _mediator.SendMessage(message, this);
    }

    public void Receive(string message, string senderName)
    {
        Console.WriteLine($"Mediator Pattern: [{Name}] Received from {senderName}: '{message}'");
    }
}