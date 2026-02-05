namespace Audabit.Service.SoftwarePatterns.App.Patterns._3_Behavioral.Patterns;

// Iterator Pattern: Provides a way to access elements of a collection sequentially without exposing its underlying representation.
// In this example:
// - BookCollection stores books in an internal array.
// - BookIterator provides sequential access to the books.
// - Clients can iterate through books without knowing the collection's internal structure.
// - The key here is that iteration logic is separate from the collection itself.
//
// MODERN C# USAGE:
// C# has built-in iterator support with IEnumerable<T> and yield return:
//   public IEnumerable<Book> GetBooks() {
//       foreach (var book in _books) {
//           yield return book;
//       }
//   }
// Use LINQ methods (Where, Select, OrderBy) instead of manual iteration.
// IAsyncEnumerable<T> provides async iteration:
//   await foreach (var item in GetItemsAsync()) { }
// Collections automatically support foreach with GetEnumerator().
public static class Iterator
{
    public static void Run()
    {
        Console.WriteLine("Iterator Pattern: Accessing collection elements sequentially...");
        Console.WriteLine("Iterator Pattern: Iterator provides access without exposing internal structure.\n");

        var collection = new BookCollection();
        collection.AddBook(new Book("Design Patterns", "Gang of Four"));
        collection.AddBook(new Book("Clean Code", "Robert Martin"));
        collection.AddBook(new Book("Refactoring", "Martin Fowler"));

        Console.WriteLine("Iterator Pattern: --- Iterating Through Books ---");
        var iterator = collection.CreateIterator();
        while (iterator.HasNext())
        {
            var book = iterator.Next();
            Console.WriteLine($"Iterator Pattern: 📖 {book.Title} by {book.Author}");
        }

        Console.WriteLine("\nIterator Pattern: --- Using Modern C# Foreach ---");
        foreach (var book in collection)
        {
            Console.WriteLine($"Iterator Pattern: 📚 {book.Title} by {book.Author}");
        }
    }
}

// Element
public sealed record Book(string Title, string Author);

// Iterator interface
public interface IIterator<T>
{
    bool HasNext();
    T Next();
}

// Aggregate interface
public interface IBookAggregate
{
    IIterator<Book> CreateIterator();
}

// Concrete Iterator
public sealed class BookIterator(Book[] books) : IIterator<Book>
{
    private readonly Book[] _books = books;
    private int _position = 0;

    public bool HasNext()
    {
        return _position < _books.Length;
    }

    public Book Next()
    {
        if (!HasNext())
        {
            throw new InvalidOperationException("No more books in collection");
        }

        var book = _books[_position];
        _position++;
        return book;
    }
}

// Concrete Aggregate - also implements IEnumerable<T> for modern C# support
public sealed class BookCollection : IBookAggregate, IEnumerable<Book>
{
    private readonly List<Book> _books = [];

    public void AddBook(Book book)
    {
        _books.Add(book);
        Console.WriteLine($"Iterator Pattern: [BookCollection] Added: {book.Title}");
    }

    public IIterator<Book> CreateIterator()
    {
        return new BookIterator([.. _books]);
    }

    // Modern C# IEnumerable implementation
    public IEnumerator<Book> GetEnumerator()
    {
        return _books.GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}