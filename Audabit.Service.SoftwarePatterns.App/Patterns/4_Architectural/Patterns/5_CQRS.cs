namespace Audabit.Service.SoftwarePatterns.App.Patterns._4_Architectural.Patterns;

// CQRS Pattern (Command Query Responsibility Segregation): Separates read operations (queries) from write operations (commands), often using different models and even databases for each.
// LEVEL: Code-level architecture pattern (how you separate read/write logic; can become infrastructure-level with separate physical databases)
// In this example:
// - Commands (CreateProduct, UpdatePrice) modify state and don't return data.
// - Queries (GetProductById, GetAllProducts) read data and don't modify state.
// - Write model is optimized for consistency and validation, read model is optimized for performance.
// - The key here is that reads and writes have different requirements and are handled separately.
//
// MODERN C# USAGE:
// CQRS is widely used in modern .NET applications, especially with DDD and Event Sourcing:
// - Use MediatR library to implement CQRS: IRequest<TResponse> for queries, IRequest for commands
// - Commands: public record CreateProductCommand(string Name, decimal Price) : IRequest;
// - Queries: public record GetProductQuery(int Id) : IRequest<ProductDto>;
// - Separate read/write databases: SQL Server for writes, Redis/Elasticsearch for reads
// - Azure Cosmos DB supports CQRS with change feed for read model synchronization
// - Often combined with Event Sourcing for ultimate scalability
// - Use separate DTOs for commands/queries to avoid coupling
public static class CQRS
{
    public static void Run()
    {
        Console.WriteLine("CQRS Pattern: Separating read and write responsibilities...");
        Console.WriteLine("CQRS Pattern: Commands modify state, queries retrieve data without side effects.\n");

        // Separate databases - Write DB and Read DB
        var writeDatabase = new ProductWriteDatabase();
        var readDatabase = new ProductReadDatabase();

        // Command handlers only write to write database
        var commandHandler = new ProductCommandHandler(writeDatabase);

        // Query handlers only read from read database
        var queryHandler = new ProductQueryHandler(readDatabase);

        Console.WriteLine("CQRS Pattern: --- Executing Commands (Writes) ---");

        // Command 1: Create product
        commandHandler.Handle(new CreateProductCommand(1, "Laptop", 999.99m));

        // Command 2: Create another product
        commandHandler.Handle(new CreateProductCommand(2, "Mouse", 29.99m));

        // Command 3: Update price
        commandHandler.Handle(new UpdateProductPriceCommand(1, 899.99m));

        // NOTE: Write DB → Read DB sync happens automatically in background (CDC, events, etc.)
        // Simulating that sync for this demo:
        SyncDatabases(writeDatabase, readDatabase);

        Console.WriteLine("\nCQRS Pattern: --- Executing Queries (Reads) ---");

        // Query 1: Get specific product
        var product = queryHandler.Handle(new GetProductByIdQuery(1));
        if (product != null)
        {
            Console.WriteLine($"CQRS Pattern: [Query Result] {product.Name} - ${product.Price}");
        }

        // Query 2: Get all products
        var allProducts = queryHandler.Handle(new GetAllProductsQuery());
        Console.WriteLine($"CQRS Pattern: [Query Result] Found {allProducts.Count} products in read database");
    }

    // Helper method to simulate background sync (in production: CDC, events, message bus, etc.)
    private static void SyncDatabases(ProductWriteDatabase writeDb, ProductReadDatabase readDb)
    {
        Console.WriteLine("\nCQRS Pattern: [Background] Syncing databases...");

        foreach (var writeModel in writeDb.GetAll())
        {
            var readModel = new ProductReadModel
            {
                Id = writeModel.Id,
                Name = writeModel.Name,
                Price = writeModel.Price,
                DisplayName = $"{writeModel.Name} - ${writeModel.Price:F2}"
            };
            readDb.Save(readModel);
        }

        Console.WriteLine("CQRS Pattern: [Background] Sync complete\n");
    }
}

// ============================================
// COMMANDS (Write Operations)
// ============================================
public record CreateProductCommand(int Id, string Name, decimal Price);
public record UpdateProductPriceCommand(int Id, decimal NewPrice);

// ============================================
// QUERIES (Read Operations)
// ============================================
public record GetProductByIdQuery(int Id);
public record GetAllProductsQuery;

// ============================================
// COMMAND HANDLER (Processes writes)
// ============================================
public class ProductCommandHandler(ProductWriteDatabase writeDatabase)
{
    private readonly ProductWriteDatabase _writeDatabase = writeDatabase;

    public void Handle(CreateProductCommand command)
    {
        Console.WriteLine($"CQRS Pattern: [Command Handler] Processing CreateProductCommand");
        Console.WriteLine($"CQRS Pattern: [Command Handler] Validating product: {command.Name}");

        // Write ONLY to the write database (normalized, ACID compliant)
        var product = new ProductWriteModel
        {
            Id = command.Id,
            Name = command.Name,
            Price = command.Price,
            CreatedAt = DateTime.UtcNow
        };

        _writeDatabase.Save(product);
        Console.WriteLine($"CQRS Pattern: [Command Handler] Product saved to write database");
        Console.WriteLine($"CQRS Pattern: [Command Handler] Command completed (read DB syncs separately)\n");
    }

    public void Handle(UpdateProductPriceCommand command)
    {
        Console.WriteLine($"CQRS Pattern: [Command Handler] Processing UpdateProductPriceCommand");

        var product = _writeDatabase.GetById(command.Id);
        if (product != null)
        {
            product.Price = command.NewPrice;
            _writeDatabase.Save(product);
            Console.WriteLine($"CQRS Pattern: [Command Handler] Price updated to ${command.NewPrice} in write database");
            Console.WriteLine($"CQRS Pattern: [Command Handler] Command completed (read DB syncs separately)\n");
        }
    }
}

// ============================================
// QUERY HANDLER (Processes reads)
// ============================================
public class ProductQueryHandler(ProductReadDatabase readDatabase)
{
    private readonly ProductReadDatabase _readDatabase = readDatabase;

    public ProductReadModel? Handle(GetProductByIdQuery query)
    {
        Console.WriteLine($"CQRS Pattern: [Query Handler] Processing GetProductByIdQuery for ID: {query.Id}");
        Console.WriteLine($"CQRS Pattern: [Query Handler] Querying optimized read database");

        return _readDatabase.GetById(query.Id);
    }

    public List<ProductReadModel> Handle(GetAllProductsQuery _)
    {
        Console.WriteLine($"CQRS Pattern: [Query Handler] Processing GetAllProductsQuery");
        Console.WriteLine($"CQRS Pattern: [Query Handler] Retrieving from denormalized read database");

        return _readDatabase.GetAll();
    }
}

// ============================================
// WRITE DATABASE (Optimized for consistency)
// In production: SQL Server/PostgreSQL - normalized schema, ACID transactions
// ============================================
public class ProductWriteModel
{
    public int Id { get; init; }
    public required string Name { get; init; }
    public decimal Price { get; set; }
    public DateTime CreatedAt { get; init; }
}

public class ProductWriteDatabase
{
    // In production: SQL Server with normalized tables, foreign keys, transactions
    private readonly Dictionary<int, ProductWriteModel> _products = [];

    public void Save(ProductWriteModel product)
    {
        Console.WriteLine($"CQRS Pattern: [Write Database] Saving to SQL Server (normalized schema)");
        _products[product.Id] = product;
    }

    public ProductWriteModel? GetById(int id)
    {
        return _products.GetValueOrDefault(id);
    }

    public IEnumerable<ProductWriteModel> GetAll()
    {
        return _products.Values;
    }
}

// ============================================
// READ DATABASE (Optimized for queries)
// In production: Redis/Elasticsearch/MongoDB - denormalized, fast reads
// ============================================
public class ProductReadModel
{
    public int Id { get; init; }
    public required string Name { get; init; }
    public decimal Price { get; init; }
    public required string DisplayName { get; init; } // Denormalized field - pre-formatted for display
}

public class ProductReadDatabase
{
    // In production: Redis cache, Elasticsearch index, or denormalized MongoDB collection
    private readonly Dictionary<int, ProductReadModel> _products = [];

    public void Save(ProductReadModel product)
    {
        Console.WriteLine($"CQRS Pattern: [Read Database] Saving to Redis/Elasticsearch (denormalized, optimized for reads)");
        _products[product.Id] = product;
    }

    public ProductReadModel? GetById(int id)
    {
        return _products.GetValueOrDefault(id);
    }

    public List<ProductReadModel> GetAll()
    {
        return [.. _products.Values];
    }
}