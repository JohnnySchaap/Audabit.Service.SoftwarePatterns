namespace Audabit.Service.SoftwarePatterns.App.Patterns._1_Creational.Patterns;

// Simple Factory Pattern: Provides a static method to create and return instances of different document types based on input parameters.
// In this example:
// - We centralize object creation logic in a single static factory class (DocumentFactorySimpleFactory).
// - The factory uses a parameter (document type) to decide which concrete class to instantiate.
// - Client code doesn't need to know about concrete document classes, only the factory method.
// - The key difference from Factory Method is that this uses a simple static method with conditional logic, not inheritance.
// - This way the caller doesn't need to know about the concrete subclasses being instantiated.
//
// MODERN C# USAGE:
// In real-world .NET applications with Dependency Injection (DI), this pattern becomes even simpler:
// - Register all IDocument implementations in the DI container: services.AddScoped<IDocument, PdfDocument>();
// - Inject IEnumerable<IDocument> into a factory class.
// - Use LINQ to select: documents.FirstOrDefault(d => d.Type == documentType) ?? throw new Exception();
// - No switch statements needed! Just register new implementations in DI - the factory requires NO changes.
// - DI essentially acts as a more powerful factory itself, and you only need this simple selection logic.
public static class SimpleFactory
{
    public static void Run()
    {
        Console.WriteLine("Simple Factory Pattern: Creating documents using Simple Factory...");
        Console.WriteLine("Simple Factory Pattern: All creation logic is centralized in one static factory method.\n");

        Console.WriteLine("Simple Factory Pattern: --- Creating PDF Document ---");
        var pdfDoc = DocumentFactorySimpleFactory.CreateDocument("pdf");
        pdfDoc.Open();

        Console.WriteLine("\nSimple Factory Pattern: --- Creating Word Document ---");
        var wordDoc = DocumentFactorySimpleFactory.CreateDocument("word");
        wordDoc.Open();

        Console.WriteLine("\nSimple Factory Pattern: --- Creating Excel Document ---");
        var excelDoc = DocumentFactorySimpleFactory.CreateDocument("excel");
        excelDoc.Open();
    }
}

public static class DocumentFactorySimpleFactory
{
    public static IDocumentSimpleFactory CreateDocument(string type)
    {
        return type.ToLower() switch
        {
            "pdf" => new PdfDocumentSimpleFactory(),
            "word" => new WordDocumentSimpleFactory(),
            "excel" => new ExcelDocumentSimpleFactory(),
            _ => throw new ArgumentException($"Unknown document type: {type}")
        };
    }
}

public interface IDocumentSimpleFactory
{
    void Open();
}

public sealed class PdfDocumentSimpleFactory : IDocumentSimpleFactory
{
    public void Open()
    {
        Console.WriteLine("Simple Factory Pattern: [PDF] Opening PDF file");
    }
}

public sealed class WordDocumentSimpleFactory : IDocumentSimpleFactory
{
    public void Open()
    {
        Console.WriteLine("Simple Factory Pattern: [Word] Opening Word document");
    }
}

public sealed class ExcelDocumentSimpleFactory : IDocumentSimpleFactory
{
    public void Open()
    {
        Console.WriteLine("Simple Factory Pattern: [Excel] Opening Excel spreadsheet");
    }
}