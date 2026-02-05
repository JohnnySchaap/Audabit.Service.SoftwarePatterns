namespace Audabit.Service.SoftwarePatterns.App.Patterns._3_Behavioral.Patterns;

// Memento Pattern: Captures and externalizes an object's internal state without violating encapsulation, allowing the object to be restored to this state later.
// In this example:
// - TextEditor creates mementos (snapshots) of its content state.
// - EditorHistory stores these mementos for undo functionality.
// - The editor can be restored to any previous state without exposing its internals.
// - The key here is that state is saved and restored without breaking encapsulation.
//
// MODERN C# USAGE:
// C# records provide built-in immutable state snapshots:
//   public record EditorState(string Content, int CursorPosition);
//   var state = editor.CreateSnapshot(); // Returns EditorState record
//   editor.Restore(state); // Restores from record
// For complex state management, consider event sourcing: store events instead of state snapshots.
// IMemoryCache can store state snapshots for session management.
public static class Memento
{
    public static void Run()
    {
        Console.WriteLine("Memento Pattern: Saving and restoring object state...");
        Console.WriteLine("Memento Pattern: Editor state can be captured and restored without breaking encapsulation.\n");

        var editor = new MementoTextEditor();
        var history = new EditorHistory();

        Console.WriteLine("Memento Pattern: --- Typing and Saving States ---");
        editor.Type("Hello ");
        history.Save(editor.CreateMemento());

        editor.Type("World");
        history.Save(editor.CreateMemento());

        editor.Type("!");
        Console.WriteLine($"Memento Pattern: Current content: '{editor.GetContent()}'");

        Console.WriteLine("\nMemento Pattern: --- Undoing to Previous State ---");
        editor.Restore(history.Undo());
        Console.WriteLine($"Memento Pattern: After undo: '{editor.GetContent()}'");

        Console.WriteLine("\nMemento Pattern: --- Undoing Again ---");
        editor.Restore(history.Undo());
        Console.WriteLine($"Memento Pattern: After undo: '{editor.GetContent()}'");
    }
}

// Memento
public sealed record EditorMemento(string Content);

// Originator
public sealed class MementoTextEditor
{
    private string _content = string.Empty;

    public void Type(string text)
    {
        _content += text;
        Console.WriteLine($"Memento Pattern: [Editor] Typed '{text}' → '{_content}'");
    }

    public string GetContent()
    {
        return _content;
    }

    public EditorMemento CreateMemento()
    {
        Console.WriteLine($"Memento Pattern: [Editor] Creating memento of '{_content}'");
        return new EditorMemento(_content);
    }

    public void Restore(EditorMemento memento)
    {
        _content = memento.Content;
        Console.WriteLine($"Memento Pattern: [Editor] Restored to '{_content}'");
    }
}

// Caretaker
public sealed class EditorHistory
{
    private readonly Stack<EditorMemento> _mementos = new();

    public void Save(EditorMemento memento)
    {
        _mementos.Push(memento);
        Console.WriteLine($"Memento Pattern: [History] Saved state (history size: {_mementos.Count})");
    }

    public EditorMemento Undo()
    {
        if (_mementos.Count > 0)
        {
            var memento = _mementos.Pop();
            Console.WriteLine($"Memento Pattern: [History] Restoring state (history size: {_mementos.Count})");
            return memento;
        }

        Console.WriteLine("Memento Pattern: [History] No more states to undo");
        return new EditorMemento(string.Empty);
    }
}