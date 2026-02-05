namespace Audabit.Service.SoftwarePatterns.App.Patterns._5_Messaging.Patterns;

// Message Translator Pattern: Converts messages from one format to another to enable communication between incompatible systems.
// In this example:
// - Legacy system sends messages in XML format.
// - Modern system expects messages in JSON format.
// - The translator sits between them and performs format conversion.
// - The key here is that systems don't need to change - the translator handles incompatibility.
//
// MODERN C# USAGE:
// In production .NET integration:
// - Use System.Text.Json for JSON serialization/deserialization.
// - Use System.Xml.Linq for XML parsing and generation.
// - Azure Logic Apps provide visual message transformation workflows.
// - AutoMapper can handle complex object-to-object mapping.
// - Consider using canonical data model pattern for multi-system integration.
public static class MessageTranslator
{
    public static void Run()
    {
        Console.WriteLine("Message Translator Pattern: Converting between different message formats...");
        Console.WriteLine("Message Translator Pattern: Enables communication between incompatible systems.\n");

        Console.WriteLine("Message Translator Pattern: --- Legacy System Sending XML Messages ---\n");

        Console.WriteLine("Message Translator Pattern: --- Sending Customer Order (XML) ---");
        LegacyXmlSystem.SendCustomerOrder("12345", "John Doe", 150.75m);

        Console.WriteLine("\nMessage Translator Pattern: --- Sending Product Update (XML) ---");
        LegacyXmlSystem.SendProductUpdate("PROD-999", "Wireless Mouse", 29.99m);
    }
}

// Legacy XML message format
public sealed class LegacyXmlMessage
{
    public required string MessageType { get; init; }
    public required string XmlPayload { get; init; }
}

// Modern JSON message format
public sealed class ModernJsonMessage
{
    public required string MessageType { get; init; }
    public required string JsonPayload { get; init; }
}

// Legacy system that produces XML
public sealed class LegacyXmlSystem
{
    public static void SendCustomerOrder(string orderId, string customerName, decimal amount)
    {
        var xmlPayload = $@"<?xml version=""1.0""?>
<Order>
    <OrderId>{orderId}</OrderId>
    <CustomerName>{customerName}</CustomerName>
    <Amount>{amount}</Amount>
</Order>";

        var message = new LegacyXmlMessage
        {
            MessageType = "CustomerOrder",
            XmlPayload = xmlPayload
        };

        Console.WriteLine($"Message Translator Pattern: [LegacySystem] Created XML message:");
        Console.WriteLine($"Message Translator Pattern: [LegacySystem] {xmlPayload.Replace(Environment.NewLine, " ").Trim()}");
        Console.WriteLine($"Message Translator Pattern: [LegacySystem] Sending to translator...");

        XmlToJsonTranslator.TranslateAndForward(message);
    }

    public static void SendProductUpdate(string productId, string productName, decimal price)
    {
        var xmlPayload = $@"<?xml version=""1.0""?>
<Product>
    <ProductId>{productId}</ProductId>
    <ProductName>{productName}</ProductName>
    <Price>{price}</Price>
</Product>";

        var message = new LegacyXmlMessage
        {
            MessageType = "ProductUpdate",
            XmlPayload = xmlPayload
        };

        Console.WriteLine($"Message Translator Pattern: [LegacySystem] Created XML message:");
        Console.WriteLine($"Message Translator Pattern: [LegacySystem] {xmlPayload.Replace(Environment.NewLine, " ").Trim()}");
        Console.WriteLine($"Message Translator Pattern: [LegacySystem] Sending to translator...");

        XmlToJsonTranslator.TranslateAndForward(message);
    }
}

// Translator component
public sealed class XmlToJsonTranslator
{
    public static void TranslateAndForward(LegacyXmlMessage xmlMessage)
    {
        Console.WriteLine($"Message Translator Pattern: [Translator] Received XML message of type '{xmlMessage.MessageType}'");
        Console.WriteLine($"Message Translator Pattern: [Translator] Beginning translation XML → JSON...");

        string jsonPayload;

        if (xmlMessage.MessageType == "CustomerOrder")
        {
            jsonPayload = TranslateOrderXmlToJson();
        }
        else if (xmlMessage.MessageType == "ProductUpdate")
        {
            jsonPayload = TranslateProductXmlToJson();
        }
        else
        {
            Console.WriteLine($"Message Translator Pattern: [Translator] Unknown message type - using default translation");
            jsonPayload = "{\"error\":\"Unknown message type\"}";
        }

        var jsonMessage = new ModernJsonMessage
        {
            MessageType = xmlMessage.MessageType,
            JsonPayload = jsonPayload
        };

        Console.WriteLine($"Message Translator Pattern: [Translator] Translation complete!");
        Console.WriteLine($"Message Translator Pattern: [Translator] Forwarding to modern system...");

        ModernJsonSystem.ReceiveMessage(jsonMessage);
    }

    private static string TranslateOrderXmlToJson()
    {
        // Simplified XML to JSON translation (in production, use proper XML parsing)
        Console.WriteLine($"Message Translator Pattern: [Translator] Parsing XML order data...");

        // Simulated translation
        var jsonPayload = @"{
  ""type"": ""order"",
  ""orderId"": ""12345"",
  ""customer"": ""John Doe"",
  ""total"": 150.75
}";

        Console.WriteLine($"Message Translator Pattern: [Translator] Created JSON structure");
        return jsonPayload;
    }

    private static string TranslateProductXmlToJson()
    {
        Console.WriteLine($"Message Translator Pattern: [Translator] Parsing XML product data...");

        // Simulated translation
        var jsonPayload = @"{
  ""type"": ""product"",
  ""productId"": ""PROD-999"",
  ""name"": ""Wireless Mouse"",
  ""price"": 29.99
}";

        Console.WriteLine($"Message Translator Pattern: [Translator] Created JSON structure");
        return jsonPayload;
    }
}

// Modern system that consumes JSON
public sealed class ModernJsonSystem
{
    public static void ReceiveMessage(ModernJsonMessage message)
    {
        Console.WriteLine($"Message Translator Pattern: [ModernSystem] Received JSON message:");
        Console.WriteLine($"Message Translator Pattern: [ModernSystem] Type: {message.MessageType}");
        Console.WriteLine($"Message Translator Pattern: [ModernSystem] Payload: {message.JsonPayload.Replace(Environment.NewLine, " ").Trim()}");
        Console.WriteLine($"Message Translator Pattern: [ModernSystem] Successfully processed!");
    }
}