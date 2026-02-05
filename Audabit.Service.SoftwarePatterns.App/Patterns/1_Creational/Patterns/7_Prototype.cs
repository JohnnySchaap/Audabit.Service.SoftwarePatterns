namespace Audabit.Service.SoftwarePatterns.App.Patterns._1_Creational.Patterns;

// Prototype Pattern: Creates new objects by copying an existing object (prototype) rather than creating new instances from scratch.
// In this example:
// - We implement an IPrototype<T> interface with a Clone() method that creates a copy of the object.
// - The Document class implements cloning by creating a new instance with the same property values.
// - Cloned objects are independent - modifying the clone doesn't affect the original.
// - The key here is that new objects are created by copying existing ones, which is useful when object creation is expensive or complex.
// - Keep in mind that this is a shallow copy; for deep copies, nested objects would also need to be cloned.
//
// MODERN C# USAGE:
// C# records provide built-in non-destructive mutation with the 'with' expression:
//   public record Document(string Title, string Content);
//   var original = new Document("Title", "Content");
//   var clone = original with { Title = "New Title" }; // Creates a copy with modified property
// Records use value-based equality and are perfect for prototype scenarios.
// For deep cloning complex objects, consider serialization: JsonSerializer.Deserialize(JsonSerializer.Serialize(obj)).
// Manual Clone() methods are only needed for complex cloning logic or non-record classes.
public static class Prototype
{
    public static void Run()
    {
        Console.WriteLine("Prototype Pattern: Cloning objects instead of creating new instances...");
        Console.WriteLine("Prototype Pattern: Objects are copied rather than created from scratch.\n");

        Console.WriteLine("Prototype Pattern: --- Creating Original Document ---");
        var originalDocument = new Document("Original Document", "This is the original content.");
        Console.WriteLine($"Prototype Pattern: Original: {originalDocument}");

        Console.WriteLine("\nPrototype Pattern: --- Cloning the Original ---");
        var clonedDocument = originalDocument.Clone();
        Console.WriteLine($"Prototype Pattern: Cloned: {clonedDocument}");

        Console.WriteLine("\nPrototype Pattern: --- Modifying Clone (Original Unchanged) ---");
        clonedDocument.Title = "Cloned Document";
        clonedDocument.Content = "This is the cloned content.";
        Console.WriteLine($"Prototype Pattern: Original: {originalDocument}");
        Console.WriteLine($"Prototype Pattern: Cloned: {clonedDocument}");

        Console.WriteLine("\nPrototype Pattern: Showing Prototyping using Record Types...");
        Console.WriteLine("Prototype Pattern: --- Creating Original Document ---");
        var recordOriginal = new RecordDocument("Record Original", "This is the original record content.");
        Console.WriteLine($"Prototype Pattern: Record Original: {recordOriginal}");
        Console.WriteLine("\nPrototype Pattern: --- Cloning the Original using 'with' Expression ---");
        var recordCloned = recordOriginal with { Title = "Record Cloned" };
        Console.WriteLine($"Prototype Pattern: Record Cloned: {recordCloned}");
    }
}

public sealed record RecordDocument(string Title, string Content);

public interface IPrototype<T>
{
    T Clone();
}

public sealed class Document(string title, string content) : IPrototype<Document>
{
    public string Title { get; set; } = title;
    public string Content { get; set; } = content;

    public Document Clone()
    {
        Console.WriteLine($"Prototype Pattern: Cloning document '{Title}'");
        return new Document(Title, Content);
    }

    public override string ToString()
    {
        return $"Document: '{Title}' - {Content}";
    }
}