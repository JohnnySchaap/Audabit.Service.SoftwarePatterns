namespace Audabit.Service.SoftwarePatterns.App.Patterns._3_Behavioral.Patterns;

// Command Pattern: Encapsulates a request as an object, allowing parameterization of clients with different requests, queuing of requests, and support for undoable operations.
// In this example:
// - Text editor commands (TypeCommand, DeleteCommand) are encapsulated as objects.
// - Each command knows how to execute itself and undo itself.
// - The TextEditor stores command history, enabling undo functionality.
// - The key here is that requests are reified (turned into objects), enabling undo/redo, logging, and queuing.
//
// MODERN C# USAGE:
// For simple undo/redo, use the Memento pattern with a stack of states.
// For complex applications, consider CQRS (Command Query Responsibility Segregation):
//   public record CreateOrderCommand(string CustomerId, List<OrderItem> Items) : IRequest<OrderResult>;
//   public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, OrderResult> { }
// MediatR library implements command pattern for CQRS: mediator.Send(new CreateOrderCommand(...))
// Modern .NET uses command objects extensively in CQRS, messaging systems, and event sourcing.
public static class Command
{
    public static void Run()
    {
        Console.WriteLine("Command Pattern: Encapsulating requests as objects with undo support...");
        Console.WriteLine("Command Pattern: Commands can be executed, undone, and stored in history.\n");

        var editor = new TextEditor();

        Console.WriteLine("Command Pattern: --- Typing Text ---");
        editor.ExecuteCommand(new TypeCommand(editor, "Hello "));
        editor.ExecuteCommand(new TypeCommand(editor, "World!"));
        editor.PrintDocument();

        Console.WriteLine("\nCommand Pattern: --- Deleting Text ---");
        editor.ExecuteCommand(new DeleteCommand(editor, 6));
        editor.PrintDocument();

        Console.WriteLine("\nCommand Pattern: --- Undoing Last Command ---");
        editor.Undo();
        editor.PrintDocument();

        Console.WriteLine("\nCommand Pattern: --- Undoing Again ---");
        editor.Undo();
        editor.PrintDocument();
    }
}

// Command interface
public interface ICommand
{
    void Execute();
    void Undo();
}

// Receiver
public sealed class TextDocument
{
    private string _content = string.Empty;

    public void Append(string text)
    {
        _content += text;
        Console.WriteLine($"Command Pattern: [Document] Appended '{text}' → '{_content}'");
    }

    public void Delete(int length)
    {
        if (length <= _content.Length)
        {
            var deleted = _content.Substring(_content.Length - length, length);
            _content = _content[..^length];
            Console.WriteLine($"Command Pattern: [Document] Deleted '{deleted}' → '{_content}'");
        }
    }

    public string GetContent()
    {
        return _content;
    }
}

// Concrete Commands
public sealed class TypeCommand(TextEditor editor, string text) : ICommand
{
    private readonly TextEditor _editor = editor;
    private readonly string _text = text;

    public void Execute()
    {
        Console.WriteLine($"Command Pattern: [TypeCommand] Executing: Type '{_text}'");
        _editor.Document.Append(_text);
    }

    public void Undo()
    {
        Console.WriteLine($"Command Pattern: [TypeCommand] Undoing: Delete {_text.Length} characters");
        _editor.Document.Delete(_text.Length);
    }
}

public sealed class DeleteCommand(TextEditor editor, int length) : ICommand
{
    private readonly TextEditor _editor = editor;
    private readonly int _length = length;
    private string _deletedText = string.Empty;

    public void Execute()
    {
        Console.WriteLine($"Command Pattern: [DeleteCommand] Executing: Delete {_length} characters");
        var content = _editor.Document.GetContent();
        _deletedText = content.Substring(content.Length - _length, _length);
        _editor.Document.Delete(_length);
    }

    public void Undo()
    {
        Console.WriteLine($"Command Pattern: [DeleteCommand] Undoing: Restore '{_deletedText}'");
        _editor.Document.Append(_deletedText);
    }
}

// Invoker
public sealed class TextEditor
{
    public TextDocument Document { get; } = new();
    private readonly Stack<ICommand> _commandHistory = new();

    public void ExecuteCommand(ICommand command)
    {
        command.Execute();
        _commandHistory.Push(command);
    }

    public void Undo()
    {
        if (_commandHistory.Count > 0)
        {
            var command = _commandHistory.Pop();
            command.Undo();
        }
        else
        {
            Console.WriteLine("Command Pattern: [Editor] No commands to undo");
        }
    }

    public void PrintDocument()
    {
        Console.WriteLine($"Command Pattern: [Editor] Current document: '{Document.GetContent()}'");
    }
}