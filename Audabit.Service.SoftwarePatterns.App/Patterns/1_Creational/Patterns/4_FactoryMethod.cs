namespace Audabit.Service.SoftwarePatterns.App.Patterns._1_Creational.Patterns;

// Factory Method Pattern: Defines an interface for creating an object, but lets subclasses alter the type of objects that will be created.
// In this example:
// - We create an abstract ApplicationFactory class with an abstract CreateDocument() method.
// - Concrete factory subclasses (PdfApplicationFactory, WordApplicationFactory) override CreateDocument() to return specific document types.
// - The factory method pattern delegates the instantiation decision to subclasses through inheritance.
// - The key difference from Simple Factory is that this uses inheritance and abstract methods, allowing subclasses to determine which class to instantiate.
// - The downside is that it requires more classes and can be more complex than Simple Factory. The caller must also be aware of the specific factory subclass to use.
//
// MODERN C# USAGE:
// This pattern is rarely needed in modern .NET with Dependency Injection:
// - The complex inheritance hierarchy (abstract factory class + concrete factory subclasses) is overkill.
// - Instead, use a Simple Factory that receives IEnumerable<IDocument> via DI.
// - Select the right document using LINQ: documents.FirstOrDefault(d => d.Type == type) ?? throw new Exception();
// - Adding new document types only requires registering them in DI - no new factory subclasses needed.
// - This pattern is mainly useful for learning OOP principles and understanding legacy code.
// - In production .NET code, prefer the DI-based Simple Factory approach (see SimpleFactory.cs comment).
public static class FactoryMethod
{
    public static void Run()
    {
        Console.WriteLine("Factory Method Pattern: Creating documents using Factory Method...");
        Console.WriteLine("Factory Method Pattern: Each factory subclass decides which document type to create.\n");

        Console.WriteLine("Factory Method Pattern: --- Using PDF Application Factory ---");
        var pdfApp = new PdfApplicationFactory();
        var pdfDoc = pdfApp.CreateDocument();
        pdfDoc.Open();

        Console.WriteLine("\nFactory Method Pattern: --- Using Word Application Factory ---");
        var wordApp = new WordApplicationFactory();
        var wordDoc = wordApp.CreateDocument();
        wordDoc.Open();
    }
}

public abstract class ApplicationFactory
{
    public abstract IDocument CreateDocument();
}

public sealed class PdfApplicationFactory : ApplicationFactory
{
    public override IDocument CreateDocument()
    {
        return new PdfDocument();
    }
}

public sealed class WordApplicationFactory : ApplicationFactory
{
    public override IDocument CreateDocument()
    {
        return new WordDocument();
    }
}

public interface IDocument
{
    void Open();
}

public sealed class PdfDocument : IDocument
{
    public void Open()
    {
        Console.WriteLine("Factory Method Pattern: [PDF] Opening PDF file");
    }
}

public sealed class WordDocument : IDocument
{
    public void Open()
    {
        Console.WriteLine("Factory Method Pattern: [DOCX] Opening DOCX file");
    }
}