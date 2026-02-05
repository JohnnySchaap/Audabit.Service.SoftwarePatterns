namespace Audabit.Service.SoftwarePatterns.App.Patterns._3_Behavioral.Patterns;

// Template Method Pattern: Defines the skeleton of an algorithm in a base class, letting subclasses override specific steps without changing the algorithm's structure.
// In this example:
// - DataProcessor defines the template method ProcessData() with a fixed algorithm structure.
// - Subclasses (CSV, JSON processors) override specific steps like ReadData, ParseData, TransformData.
// - The overall flow (Read → Parse → Transform → Save) remains consistent across all processors.
// - The key here is that the algorithm structure is defined once, but steps can be customized.
//
// MODERN C# USAGE:
// Strategy pattern with dependency injection is often preferred over Template Method:
//   public class DataProcessor(IDataReader reader, IDataParser parser, IDataTransformer transformer)
//   Instead of inheritance, inject different implementations of each step.
// For simple cases, use delegates/lambda expressions:
//   public void Process(Func<string> read, Func<string, Data> parse, Func<Data, Data> transform)
// Minimal APIs use this pattern: app.MapGet("/", (context) => { /* custom logic */ });
public static class TemplateMethod
{
    public static void Run()
    {
        Console.WriteLine("Template Method Pattern: Defining algorithm skeleton with customizable steps...");
        Console.WriteLine("Template Method Pattern: Different processors follow the same template but override specific steps.\n");

        Console.WriteLine("Template Method Pattern: --- Processing CSV Data ---");
        DataProcessor csvProcessor = new CsvDataProcessor();
        csvProcessor.ProcessData();

        Console.WriteLine("\nTemplate Method Pattern: --- Processing JSON Data ---");
        DataProcessor jsonProcessor = new JsonDataProcessor();
        jsonProcessor.ProcessData();
    }
}

// Abstract class with template method
public abstract class DataProcessor
{
    // Template Method: Defines the skeleton of the algorithm
    public void ProcessData()
    {
        Console.WriteLine($"Template Method Pattern: [{GetType().Name}] Starting data processing...");

        var rawData = ReadData();
        var parsedData = ParseData(rawData);
        var transformedData = TransformData(parsedData);
        SaveData(transformedData);

        Console.WriteLine($"Template Method Pattern: [{GetType().Name}] ✅ Data processing complete");
    }

    // Abstract steps - must be implemented by subclasses
    protected abstract string ReadData();
    protected abstract string ParseData(string rawData);
    protected abstract string TransformData(string parsedData);

    // Concrete step - shared by all subclasses
    protected void SaveData(string data)
    {
        Console.WriteLine($"Template Method Pattern: [{GetType().Name}] 💾 Saving data: {data}");
    }
}

// Concrete implementation for CSV
public sealed class CsvDataProcessor : DataProcessor
{
    protected override string ReadData()
    {
        var data = "name,age,city\nAlice,30,NYC";
        Console.WriteLine($"Template Method Pattern: [CsvDataProcessor] 📖 Reading CSV: {data.Replace("\n", " | ")}");
        return data;
    }

    protected override string ParseData(string rawData)
    {
        Console.WriteLine("Template Method Pattern: [CsvDataProcessor] 🔍 Parsing CSV format");
        return rawData.Replace(",", ";").Replace("\n", " || ");
    }

    protected override string TransformData(string parsedData)
    {
        Console.WriteLine("Template Method Pattern: [CsvDataProcessor] ✨ Transforming to uppercase");
        return parsedData.ToUpperInvariant();
    }
}

// Concrete implementation for JSON
public sealed class JsonDataProcessor : DataProcessor
{
    protected override string ReadData()
    {
        var data = "{\"name\":\"Bob\",\"age\":25,\"city\":\"LA\"}";
        Console.WriteLine($"Template Method Pattern: [JsonDataProcessor] 📖 Reading JSON: {data}");
        return data;
    }

    protected override string ParseData(string rawData)
    {
        Console.WriteLine("Template Method Pattern: [JsonDataProcessor] 🔍 Parsing JSON format");
        return rawData.Replace("{", "[").Replace("}", "]");
    }

    protected override string TransformData(string parsedData)
    {
        Console.WriteLine("Template Method Pattern: [JsonDataProcessor] ✨ Transforming to key-value pairs");
        return parsedData.Replace(":", "=").Replace(",", " & ");
    }
}