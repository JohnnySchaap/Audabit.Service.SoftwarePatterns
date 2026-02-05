namespace Audabit.Service.SoftwarePatterns.App.Patterns._2_Structural.Patterns;

// Pipes and Filters Pattern: A chain of processes where the output of each process is the input of the next.
// In this example:
// - We have filters that transform text data (trim, uppercase, remove spaces, etc.).
// - A pipeline chains these filters together, passing output from one to the next.
// - Each filter is independent and reusable.
// - The key here is that data flows through a series of transformations in a defined sequence.
//
// MODERN C# USAGE:
// LINQ is a built-in pipes and filters implementation:
//   var result = data
//       .Where(x => x.IsValid)      // Filter 1
//       .Select(x => x.Transform()) // Filter 2
//       .OrderBy(x => x.Name)       // Filter 3
//       .ToList();                  // Terminal operation
// For high-performance I/O pipelines, use System.IO.Pipelines (async, memory-efficient streaming).
// Middleware in ASP.NET Core is also pipes and filters: app.UseAuthentication().UseAuthorization()...
public static class PipesAndFilters
{
    public static void Run()
    {
        Console.WriteLine("Pipes and Filters Pattern: Processing data through a chain of transformations...");
        Console.WriteLine("Pipes and Filters Pattern: Each filter transforms the data and passes it to the next filter.\n");

        var pipeline = new TextProcessingPipeline()
            .AddFilter(new TrimFilter())
            .AddFilter(new UpperCaseFilter())
            .AddFilter(new RemoveSpacesFilter())
            .AddFilter(new AddPrefixFilter("[PROCESSED]"));

        Console.WriteLine("Pipes and Filters Pattern: --- Processing Text Through Pipeline ---");
        var input = "  hello world from pipes and filters  ";
        Console.WriteLine($"Pipes and Filters Pattern: Input: '{input}'");
        Console.WriteLine();

        var output = pipeline.Execute(input);

        Console.WriteLine();
        Console.WriteLine($"Pipes and Filters Pattern: Final Output: '{output}'");
    }
}

// Filter interface
public interface ITextFilter
{
    string Process(string input);
}

// Concrete filters
public sealed class TrimFilter : ITextFilter
{
    public string Process(string input)
    {
        var output = input.Trim();
        Console.WriteLine($"Pipes and Filters Pattern: [TrimFilter] '{input}' → '{output}'");
        return output;
    }
}

public sealed class UpperCaseFilter : ITextFilter
{
    public string Process(string input)
    {
        var output = input.ToUpperInvariant();
        Console.WriteLine($"Pipes and Filters Pattern: [UpperCaseFilter] '{input}' → '{output}'");
        return output;
    }
}

public sealed class RemoveSpacesFilter : ITextFilter
{
    public string Process(string input)
    {
        var output = input.Replace(" ", "");
        Console.WriteLine($"Pipes and Filters Pattern: [RemoveSpacesFilter] '{input}' → '{output}'");
        return output;
    }
}

public sealed class AddPrefixFilter(string prefix) : ITextFilter
{
    private readonly string _prefix = prefix;

    public string Process(string input)
    {
        var output = $"{_prefix} {input}";
        Console.WriteLine($"Pipes and Filters Pattern: [AddPrefixFilter] '{input}' → '{output}'");
        return output;
    }
}

// Pipeline - chains filters together
public sealed class TextProcessingPipeline
{
    private readonly List<ITextFilter> _filters = [];

    public TextProcessingPipeline AddFilter(ITextFilter filter)
    {
        _filters.Add(filter);
        return this;
    }

    public string Execute(string input)
    {
        var output = input;
        foreach (var filter in _filters)
        {
            output = filter.Process(output);
        }
        return output;
    }
}