# Messaging Patterns

Enterprise integration patterns for designing reliable, scalable message-based communication between distributed systems and services.

## 📖 About Messaging Patterns

Messaging patterns solve common challenges in distributed system communication:
- **Decoupling**: Services communicate without knowing about each other
- **Reliability**: Messages are delivered even if systems are temporarily unavailable
- **Scalability**: Distribute load across multiple consumers
- **Resilience**: Graceful handling of failures and retries

---

## 🎯 Patterns

### 1. [Outbox Pattern](1_Outbox.cs)
**Problem**: Database changes succeed but message publishing fails (dual-write problem)
**Solution**: Store messages in database transaction, guarantee eventual publishing via background worker

**When to Use**:
- Need to guarantee message delivery after database changes
- Cannot afford lost events when message broker is unavailable
- Avoid partial failures (data saved but event not published)

**Modern .NET**:
```csharp
// Store message in same transaction as business data
await using var transaction = await db.Database.BeginTransactionAsync();
db.Orders.Add(order);
db.OutboxMessages.Add(new OutboxMessage
{
    EventType = "OrderCreated",
    Payload = JsonSerializer.Serialize(order)
});
await db.SaveChangesAsync();
await transaction.CommitAsync();

// Background service publishes from outbox
```

**Real-World Examples**: Order processing, payment systems, inventory management

**Personal Usage**:
> **Store Order Placement**
>
> I used this pattern when placing orders down to store. To ensure the order was really placed to the service bus, the PlaceOrder event was stored along with the Order itself in the same Cosmos DB transaction.
>
> - **Challenge**: Orders were being saved to Cosmos but Service Bus publishing occasionally failed, causing orders to never reach the store
> - **Solution**: Store PlaceOrder event in same Cosmos container, background worker publishes to Service Bus
> - **Result**: Zero lost orders, automatic cleanup of published events, simplified error recovery
> - **Technology**: Azure Cosmos DB (transactional batch), Azure Service Bus, Hosted Service
>
> ```csharp
> // Store order + outbox event in same Cosmos transaction
> var order = new Order
> {
>     id = orderId,
>     CustomerId = customerId,
>     Items = items,
>     StoreId = storeId
> };
>
> var outboxEvent = new OutboxEvent
> {
>     id = Guid.NewGuid().ToString(),
>     EventType = "PlaceOrder",
>     Payload = JsonSerializer.Serialize(order),
>     CreatedAt = DateTime.UtcNow,
> };
>
> // Atomic transaction - both succeed or both fail
> var batch = container.CreateTransactionalBatch(new PartitionKey(storeId))
>     .CreateItem(order)
>     .CreateItem(outboxEvent);
>
> await batch.ExecuteAsync();
>
> // Background worker publishes events and marks them as processed
> // Note: In our implementation, if publishing fails after 5 minutes,
> // we trigger an order refund and still mark the event as processed
> // rather than indefinitely retrying
> ```

---

### 2. [Inbox Pattern](2_Inbox.cs)
**Problem**: Messages can be delivered multiple times, causing duplicate processing (duplicate payments, emails, etc.)
**Solution**: Store processed message IDs in database, check before processing (exactly-once semantics)

**When to Use**:
- Message broker provides at-least-once delivery
- Need to prevent duplicate processing
- Want idempotent message handlers

**Modern .NET**:
```csharp
// Insert-first pattern with status tracking
var messageRecord = new ProcessedMessage
{
    MessageId = message.Id,
    Status = "Processing"
};

try {
    await db.ProcessedMessages.AddAsync(messageRecord);
    await db.SaveChangesAsync(); // Unique constraint prevents duplicates

    await ProcessMessage(message);

    messageRecord.Status = "Success";
    await db.SaveChangesAsync();
}
catch (DbUpdateException) {
    // Duplicate - already being processed
    return;
}
```

**Real-World Examples**: Payment processing, webhook handlers, event consumers

**Personal Usage**:
> **Store Order Processing**
>
> Continuing from the Outbox pattern example, the store service receives PlaceOrder events from Service Bus and needs to ensure each order is processed exactly once before sending to the POS system.
>
> - **Challenge**: Prevent duplicate order processing AND enforce business rule that placed orders are immutable (cannot be modified after initial placement)
> - **Solution**: Use OrderId (not MessageId) as the deduplication key with unique constraint. This prevents both duplicate message processing and any subsequent attempts to modify the same order
> - **Result**: Zero duplicate orders sent to POS, automatic enforcement of order immutability business rule, simplified idempotency
> - **Technology**: SQL Server (running in each store), Azure Service Bus, Entity Framework Core
>
> ```csharp
> // Store order with OrderId as primary key (deduplication)
> var order = new Order
> {
>     OrderId = message.OrderId,  // Using OrderId, not MessageId!
>     CustomerId = message.CustomerId,
>     Items = message.Items,
>     StoreId = storeId,
>     Status = "Processing"
> };
>
> try {
>     // Unique constraint on OrderId prevents duplicates
>     db.Orders.Add(order);
>     await db.SaveChangesAsync();
>
>     // Send to POS system
>     await posClient.SubmitOrderAsync(order);
>
>     // Update status
>     order.Status = "Submitted";
>     await db.SaveChangesAsync();
> }
> catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx && sqlEx.Number == 2627) {
>     // Duplicate OrderId - already processed (or new event for same order)
>     // Business rule: placed orders cannot be modified
>     return;
> }
> ```

---

### 3. [Request-Reply](3_RequestReply.cs)
**Problem**: Need request-response semantics but synchronous HTTP is not suitable (long-running operations)
**Solution**: Correlation ID + reply-to queue for matching requests/responses

**When to Use**:
- Need async request-response
- Long-running operations
- Avoid blocking HTTP calls

**Modern .NET**:
```csharp
// Request with correlation
var correlationId = Guid.NewGuid().ToString();
await requestQueue.SendAsync(new Message
{
    CorrelationId = correlationId,
    ReplyTo = "replies",
    Body = request
});

// Handle reply
await foreach (var reply in replyQueue.ReceiveAsync())
{
    if (reply.CorrelationId == correlationId)
        return ProcessReply(reply);
}
```

**Real-World Examples**: RPC over messaging, async API calls, distributed queries

**Personal Usage**:
> **Multi-Store Customer Data Deletion (GDPR)**
>
> I used this pattern when implementing GDPR "right to be forgotten" requests that required deleting customer data from all store databases. The central service needs to send delete requests to all stores and wait for confirmation before reporting success to the customer.
>
> - **Challenge**: Need to delete customer data from hundreds of store databases, wait for all deletions to complete, and aggregate results to determine if the operation fully succeeded across all stores
> - **Solution**: Send DeleteCustomerData request to each store with correlation ID and reply-to queue. Stores process deletion and send reply. Aggregate responses to determine overall success/failure
> - **Result**: Reliable multi-store data deletion, clear audit trail of which stores succeeded/failed, ability to retry failed stores, complete visibility into deletion status
> - **Technology**: Azure Service Bus (request/reply queues), Azure Table Storage (response aggregation), correlation IDs
>
> ```csharp
> // Requester (central GDPR service) - sends requests to all stores
> public async Task<CustomerDeletionResult> DeleteCustomerDataAsync(string customerId)
> {
>     var correlationId = Guid.NewGuid().ToString();
>     var storeNumbers = await GetAllStoreNumbersAsync();
>
>     // Track expected responses in Table Storage
>     await tableClient.AddEntityAsync(new DeletionRequest
>     {
>         PartitionKey = correlationId,
>         RowKey = correlationId,
>         CustomerId = customerId,
>         ExpectedStores = storeNumbers.Count,
>         Status = "Pending"
>     });
>
>     // Publish request to topic - each store subscription receives it
>     var request = new ServiceBusMessage
>     {
>         CorrelationId = correlationId,
>         ReplyTo = "gdpr-deletion-replies",
>         Body = BinaryData.FromObjectAsJson(new DeleteCustomerDataRequest
>         {
>             CustomerId = customerId
>         })
>     };
>
>     await topicClient.SendMessageAsync(request);
>
>     // Wait for all replies (with timeout)
>     return await WaitForAllRepliesAsync(correlationId, TimeSpan.FromMinutes(5));
> }
>
> // Reply handler - aggregates responses as they arrive
> var replyProcessor = serviceBusClient.CreateProcessor("gdpr-deletion-replies");
>
> replyProcessor.ProcessMessageAsync += async args =>
> {
>     var reply = args.Message.Body.ToObjectFromJson<DeleteCustomerDataReply>();
>     var correlationId = args.Message.CorrelationId;
>
>     // Aggregate response
>     var request = await tableClient.GetEntityAsync<DeletionRequest>(
>         correlationId, correlationId);
>
>     request.Value.ReceivedReplies++;
>     if (reply.Success)
>         request.Value.SuccessfulStores++;
>     else
>         request.Value.FailedStores.Add(reply.StoreNumber);
>
>     // Check if all replies received
>     if (request.Value.ReceivedReplies == request.Value.ExpectedStores)
>     {
>         request.Value.Status = request.Value.FailedStores.Count == 0
>             ? "Completed"
>             : "PartialFailure";
>         request.Value.CompletedAt = DateTime.UtcNow;
>     }
>
>     await tableClient.UpdateEntityAsync(request.Value, ETag.All);
>     await args.CompleteMessageAsync(args.Message);
> };
>
> // Store service - processes deletion and sends reply
> // Each store has subscription with filter for its store number
> var storeProcessor = serviceBusClient.CreateProcessor(
>     topicName: "gdpr-deletion-requests",
>     subscriptionName: "store-1234");  // Each store has own subscription
>
> storeProcessor.ProcessMessageAsync += async args =>
> {
>     var request = args.Message.Body.ToObjectFromJson<DeleteCustomerDataRequest>();
>     var success = false;
>
>     try
>     {
>         await DeleteCustomerFromStoreDatabase(request.CustomerId);
>         success = true;
>     }
>     catch (Exception ex)
>     {
>         // Log error but send reply anyway
>     }
>
>     // Send reply to reply-to queue
>     var reply = new ServiceBusMessage
>     {
>         CorrelationId = args.Message.CorrelationId,
>         Body = BinaryData.FromObjectAsJson(new DeleteCustomerDataReply
>         {
>             CustomerId = request.CustomerId,
>             StoreNumber = "1234",  // This store's number
>             Success = success,
>             ProcessedAt = DateTime.UtcNow
>         })
>     };
>
>     var replyQueue = serviceBusClient.CreateSender(args.Message.ReplyTo);
>     await replyQueue.SendMessageAsync(reply);
>     await args.CompleteMessageAsync(args.Message);
> };
> ```

---

### 4. [Publish-Subscribe](4_PublishSubscribe.cs)
**Problem**: Multiple independent systems need to react to the same event
**Solution**: Publisher sends to topic, multiple subscribers receive copies

**When to Use**:
- One event, many interested parties
- Subscribers do not know about each other
- Dynamic subscription management

**Modern .NET** (Azure Service Bus):
```csharp
// Publisher
await topicClient.SendMessageAsync(new ServiceBusMessage
{
    Subject = "OrderCreated",
    Body = BinaryData.FromObjectAsJson(order)
});

// Multiple subscribers
await subscription1.ProcessMessageAsync(async msg => { /* Email notification */ });
await subscription2.ProcessMessageAsync(async msg => { /* Inventory update */ });
await subscription3.ProcessMessageAsync(async msg => { /* Analytics */ });
```

**Real-World Examples**: Event notifications, fan-out processing, event-driven architecture

**Personal Usage**:
> **Store Order Distribution**
>
> I used this pattern to distribute PlaceOrder events from the central ordering service to individual store subscriptions. When a customer places an order for a specific store, all stores are listening on the same topic, but each store only processes orders meant for them through subscription filters.
>
> - **Challenge**: Need to send orders to hundreds of stores efficiently, with each store only receiving orders intended for them, without maintaining individual queues per store
> - **Solution**: Publish PlaceOrder events to Azure Service Bus topic with StoreNumber property. Each store has its own subscription with SQL filter (`StoreNumber = '1234'`)
> - **Result**: Single publish point, dynamic store scaling (add subscription for new store), automatic filtering by broker, no routing logic in application code
> - **Technology**: Azure Service Bus (topics + subscription filters), continuing from Outbox pattern example
>
> ```csharp
> // Publisher (customer-facing service) - publishes to topic
> var message = new ServiceBusMessage
> {
>     Subject = "PlaceOrder",
>     Body = BinaryData.FromObjectAsJson(order),
>     ApplicationProperties =
>     {
>         ["StoreNumber"] = order.StoreId,
>         ["OrderId"] = order.OrderId
>     }
> };
>
> await topicClient.SendMessageAsync(message);
>
> // Store subscriptions - each store has subscription with filter
> // Subscription: "store-1234" with SQL filter: "StoreNumber = '1234'"
> // Subscription: "store-5678" with SQL filter: "StoreNumber = '5678'"
>
> // Store subscriber (runs at each store location)
> var processor = serviceBusClient.CreateProcessor(
>     topicName: "orders",
>     subscriptionName: "store-1234"); // Only receives StoreNumber = '1234'
>
> processor.ProcessMessageAsync += async args =>
> {
>     var order = args.Message.Body.ToObjectFromJson<Order>();
>     // This store only receives its own orders
>     await ProcessOrderForThisStore(order);
>     await args.CompleteMessageAsync(args.Message);
> };
> ```
>
> **Order Confirmation Fan-Out**
>
> After the store confirms it received and accepted the order, multiple independent systems need to react: payment capture, customer notification email, analytics tracking, and inventory updates.
>
> - **Challenge**: Store confirms order receipt, triggering multiple independent downstream actions without coupling the store service to payment, email, analytics, and inventory services
> - **Solution**: Store publishes "OrderConfirmed" event to topic. Independent subscriptions handle payment capture, email notifications, analytics, and inventory updates—each at their own pace
> - **Result**: Store service does not know about downstream systems, new subscribers can be added without changing store code, failures in one subscriber do not affect others, parallel execution
> - **Technology**: Azure Service Bus (topics with multiple independent subscriptions)
>
> ```csharp
> // Publisher (store service) - publishes order confirmation
> var confirmationMessage = new ServiceBusMessage
> {
>     Subject = "OrderConfirmed",
>     Body = BinaryData.FromObjectAsJson(new OrderConfirmedEvent
>     {
>         OrderId = order.OrderId,
>         StoreId = order.StoreId,
>         CustomerId = order.CustomerId,
>         TotalAmount = order.TotalAmount,
>         ConfirmedAt = DateTime.UtcNow
>     })
> };
>
> await topicClient.SendMessageAsync(confirmationMessage);
>
> // Subscriber 1: Payment capture service
> var paymentProcessor = serviceBusClient.CreateProcessor(
>     topicName: "order-events",
>     subscriptionName: "payment-capture");
>
> paymentProcessor.ProcessMessageAsync += async args =>
> {
>     var evt = args.Message.Body.ToObjectFromJson<OrderConfirmedEvent>();
>     await paymentService.CapturePaymentAsync(evt.OrderId, evt.TotalAmount);
>     await args.CompleteMessageAsync(args.Message);
> };
>
> // Subscriber 2: Email notification service
> var emailProcessor = serviceBusClient.CreateProcessor(
>     topicName: "order-events",
>     subscriptionName: "email-notifications");
>
> emailProcessor.ProcessMessageAsync += async args =>
> {
>     var evt = args.Message.Body.ToObjectFromJson<OrderConfirmedEvent>();
>     await emailService.SendOrderConfirmationAsync(evt.CustomerId, evt.OrderId);
>     await args.CompleteMessageAsync(args.Message);
> };
>
> // Subscriber 3: Analytics service
> // Subscriber 4: Inventory service
> // Each operates independently
> ```

---

### 5. [Dead Letter Channel](5_DeadLetterChannel.cs)
**Problem**: Poison messages that repeatedly fail block the queue and prevent other messages from processing
**Solution**: Move failed messages to separate queue after max retries

**When to Use**:
- Messages fail repeatedly
- Need visibility into failures
- Manual intervention required

**Modern .NET** (Azure Service Bus):
```csharp
// Automatic DLQ with max delivery count
var options = new ServiceBusProcessorOptions
{
    MaxConcurrentCalls = 1,
    AutoCompleteMessages = false,
    MaxAutoLockRenewalDuration = TimeSpan.FromMinutes(5)
};

processor.ProcessMessageAsync += async args =>
{
    try
    {
        await ProcessMessage(args.Message);
        await args.CompleteMessageAsync(args.Message);
    }
    catch
    {
        await args.AbandonMessageAsync(args.Message);
        // After max retries (10), automatically moved to DLQ
    }
};
```

**Real-World Examples**: Failed webhook deliveries, corrupted messages, business rule violations

**Personal Usage**:
> **Order Processing with Automatic Refund on Delivery Failure**
>
> I used this pattern for store order processing where orders must be acknowledged within 5 minutes. If the store is down or the message cannot be processed, it moves to the dead letter queue and triggers an automatic refund.
>
> - **Challenge**: Orders sent to stores must be processed within SLA. If store is down or database issues prevent processing, need to fail gracefully and refund customer rather than retry indefinitely
> - **Solution**: Configure Service Bus with 5-minute max lock renewal duration. Failed messages automatically move to DLQ. DLQ processor issues refunds and notifies customers
> - **Result**: Customer receives immediate refund notification instead of order being stuck in limbo. Store downtime triggers alerts via DLQ monitoring. Clear separation between technical retries (automatic) and business compensation (refund)
> - **Technology**: Azure Service Bus (automatic DLQ), max delivery count, lock duration, DLQ processor
>
> ```csharp
> // Configure queue with max delivery count and lock duration
> var options = new ServiceBusProcessorOptions
> {
>     MaxConcurrentCalls = 10,
>     AutoCompleteMessages = false,
>     MaxAutoLockRenewalDuration = TimeSpan.FromMinutes(5)
> };
>
> var processor = serviceBusClient.CreateProcessor("store-orders", options);
>
> processor.ProcessMessageAsync += async args =>
> {
>     var order = args.Message.Body.ToObjectFromJson<Order>();
>
>     try
>     {
>         await ProcessOrder(order);
>         await args.CompleteMessageAsync(args.Message);
>     }
>     catch (Exception ex)
>     {
>         // Message will be retried automatically
>         // After max delivery count (default 10) or 5-minute lock expiration,
>         // Service Bus automatically moves message to DLQ
>         await args.AbandonMessageAsync(args.Message);
>     }
> };
>
> // Dead letter queue processor - triggers refund
> var dlqProcessor = serviceBusClient.CreateProcessor(
>     "store-orders",
>     new ServiceBusProcessorOptions
>     {
>         SubQueue = SubQueue.DeadLetter
>     });
>
> dlqProcessor.ProcessMessageAsync += async args =>
> {
>     var order = args.Message.Body.ToObjectFromJson<Order>();
>     var reason = args.Message.DeadLetterReason;
>     var errorDescription = args.Message.DeadLetterErrorDescription;
>
>     // Business rule: Dead lettered orders trigger automatic refund
>     await IssueRefund(order.CustomerId, order.Total,
>         reason: "Order could not be processed by store within 5 minutes");
>
>     await NotifyCustomer(order.CustomerId,
>         "Your order has been cancelled and refunded");
>
>     await args.CompleteMessageAsync(args.Message);
> };
> ```
>
> **Why This Approach**:
> - **Automatic DLQ**: Service Bus handles message poisoning automatically (no custom logic needed)
> - **Business Rule Enforcement**: 5-minute SLA enforced via MaxAutoLockRenewalDuration
> - **Customer Experience**: Immediate refund better than indefinite retry or silent failure
> - **Monitoring**: DLQ messages trigger alerts for infrastructure issues (store down, database problems)
> - **Separates Concerns**: Technical retry (automatic abandonment) vs business compensation (refund logic)
>
> **Alternative Considered**: Infinite retry was rejected because it delays customer notification and refund. The 5-minute timeout balances technical retries with business requirements.

---

### 6. [Splitter Pattern](6_Splitter.cs)
**Problem**: Large composite messages are slow to process sequentially and failures block the entire batch
**Solution**: Split into individual messages for parallel processing

**When to Use**:
- Bulk operations need parallel processing
- Partial failures shouldn't block entire batch
- Independent retry logic per item

**Modern .NET**:
```csharp
// Split bulk order into individual items
foreach (var item in bulkOrder.Items)
{
    await messageQueue.SendAsync(new ItemMessage
    {
        CorrelationId = bulkOrder.OrderId,
        Item = item
    });
}
```

**Real-World Examples**: Batch file processing, bulk email, CSV imports

**Personal Usage**:
> **GDPR Data Deletion Processing**
>
> I used this pattern when implementing GDPR data retention compliance. The data warehouse identifies dormant customers requiring deletion (after email notification) and drops a CSV file containing customer IDs into blob storage.
>
> - **Challenge**: Processing thousands of customer deletions sequentially would be slow, and a single failure would block the entire batch from completing
> - **Solution**: Azure Function with blob trigger reads CSV file, splits into individual customer deletion messages, publishes each to Azure Storage queue for parallel processing
> - **Result**: Parallel processing across multiple workers, individual retry logic per customer, partial failures do not block batch, complete audit trail
> - **Technology**: Azure Blob Storage, Azure Functions (Blob Trigger), Azure Storage Queue, CSV parsing
>
> ```csharp
> [FunctionName("GdprDeletionSplitter")]
> public async Task Run(
>     [BlobTrigger("gdpr-deletions/{name}", Connection = "BlobStorage")] Stream blobStream,
>     string name)
> {
>     using var reader = new StreamReader(blobStream);
>     var batchId = Guid.NewGuid().ToString();
>     var customerIds = new List<string>();
>
>     // Read CSV file
>     while (!reader.EndOfStream)
>     {
>         var line = await reader.ReadLineAsync();
>         if (!string.IsNullOrWhiteSpace(line))
>             customerIds.Add(line.Trim());
>     }
>
>     // Persist batch tracking record for aggregation
>     var tableClient = new TableClient(connectionString, "gdprbatches");
>     await tableClient.CreateIfNotExistsAsync();
>     await tableClient.AddEntityAsync(new BatchTracking
>     {
>         PartitionKey = batchId,
>         RowKey = batchId,
>         ExpectedCount = customerIds.Count,
>         SourceFile = name,
>         Status = "Processing"
>     });
>
>     // Split into individual messages for parallel processing
>     var queueClient = new QueueClient(connectionString, "gdpr-deletion-queue");
>     await queueClient.CreateIfNotExistsAsync();
>
>     foreach (var customerId in customerIds)
>     {
>         var message = new CustomerDeletionMessage
>         {
>             CustomerId = customerId,
>             BatchId = batchId,
>             SourceFile = name
>         };
>
>         await queueClient.SendMessageAsync(
>             BinaryData.FromObjectAsJson(message));
>     }
>
> }
> ```

---

### 7. [Aggregator Pattern](7_Aggregator.cs)
⚠️ **DANGEROUS PATTERN** - Use with caution!

**Problem**: Need to make decisions based on results from multiple related messages
**Solution**: Collect messages until complete, then process aggregate

**When to Use** (Limited scenarios):
- Non-critical aggregation (analytics, reporting)
- Can tolerate partial/incomplete results
- Have completion strategy (timeout + count)

**Modern .NET**:
```csharp
// Simple in-memory aggregator (requires careful state management)
public class OrderAggregator
{
    private readonly ConcurrentDictionary<string, AggregateState> _states = new();

    public async Task HandleMessage(ItemProcessed message)
    {
        var state = _states.GetOrAdd(message.OrderId, _ => new AggregateState
        {
            ExpectedCount = message.TotalItems,
            Results = new ConcurrentBag<string>()
        });

        state.Results.Add(message.Result);

        if (state.Results.Count == state.ExpectedCount)
        {
            await ProcessCompleteOrder(message.OrderId, state.Results);
            _states.TryRemove(message.OrderId, out _);
        }
    }
}
```

**Real-World Examples**: Payment reconciliation (use Saga!), distributed queries, scatter-gather

**Personal Usage**:
> **GDPR Batch Completion Tracking**
>
> Continuing from the Splitter pattern example, after splitting the GDPR deletion CSV into individual messages, we need to know when ALL deletions in a batch have completed to mark the batch as done and send a completion notification.
>
> - **Challenge**: Need to know when all individual deletion messages from a batch have been processed (success or failure) to trigger batch completion actions (notifications, cleanup, auditing)
> - **Solution**: Splitter persists batch metadata in Azure Table Storage with expected count. Each deletion processor increments a processed count. When processed count equals expected count, batch is complete
> - **Result**: Accurate batch completion tracking, notification when entire batch finishes, clear audit trail of batch progress, ability to detect stuck batches
> - **Technology**: Azure Table Storage (batch tracking), Azure Functions (queue triggers), atomic increment operations
>
> ```csharp
> [FunctionName("GdprDeletionProcessor")]
> public async Task Run(
>     [QueueTrigger("gdpr-deletion-queue")] CustomerDeletionMessage message)
> {
>     try
>     {
>         // Process individual customer deletion
>         await customerService.DeleteCustomerDataAsync(message.CustomerId);
>     }
>     catch (Exception ex)
>     {
>         // Record failure but continue aggregation
>     }
>
>     // Aggregate: increment processed count for batch
>     var tableClient = new TableClient(connectionString, "gdprbatches");
>     var batch = await tableClient.GetEntityAsync<BatchTracking>(
>         message.BatchId, message.BatchId);
>
>     // Atomic increment
>     batch.Value.ProcessedCount++;
>     await tableClient.UpdateEntityAsync(batch.Value, ETag.All);
>
>     // Check if batch is complete
>     if (batch.Value.ProcessedCount == batch.Value.ExpectedCount)
>     {
>         // Trigger completion actions
>         batch.Value.Status = "Completed";
>         batch.Value.CompletedAt = DateTime.UtcNow;
>         await tableClient.UpdateEntityAsync(batch.Value, ETag.All);
>
>         await notificationService.SendBatchCompletionNotificationAsync(message.BatchId);
>     }
> }
> ```

---

### 8. [Message Router](8_MessageRouter.cs)
**Problem**: Different message types arrive on same queue but need different processing
**Solution**: Inspect message headers/properties and route accordingly

**When to Use**:
- Route by priority, type, or source
- Technical routing (not business logic)
- Need high performance (no deserialization)

**Modern .NET**:
```csharp
// Route by message type (metadata)
if (message.Properties["MessageType"] == "OrderCreated")
    await orderQueue.SendAsync(message);
else if (message.Properties["Priority"] == "High")
    await highPriorityQueue.SendAsync(message);
```

**Real-World Examples**: Message broker routing, priority queues, multi-tenant systems

**Personal Usage**:
> **Not Used - Service Bus Topics Preferred**
>
> I have not used the Message Router pattern in production. Instead, I use Azure Service Bus topics with subscription filters for routing messages based on metadata (message properties). Service Bus handles the routing infrastructure, eliminating the need for custom router implementations.
>
> - **Why not Message Router**: Azure Service Bus topics provide built-in filtering on message properties without writing custom routing code
> - **Alternative approach**: Publish to topic, subscriptions filter by properties (`Priority = 'High'`, `MessageType = 'OrderCreated'`, `StoreNumber = '1234'`)
> - **Real usage example**: Route PlaceOrder events to specific store subscriptions based on store number property, ensuring each store only receives orders intended for them
> - **Benefits**: Broker-native feature, no custom code, dynamic subscription management, better scalability
> - **When you might need this**: Using Azure Storage Queue or simpler message brokers (chosen for cost/scope reasons) that contain multiple message types and need metadata-based routing
> - **Technology**: Azure Service Bus (topics + subscription filters)

---

### 9. [Content-Based Router](9_ContentBasedRouter.cs)
**Problem**: Messages need different handling based on business rules in their content
**Solution**: Deserialize message, inspect content, apply business logic

**When to Use**:
- Routing requires business logic
- Decision based on payload values
- Complex routing rules

**Modern .NET**:
```csharp
// Route by business rules (content)
var claim = JsonSerializer.Deserialize<InsuranceClaim>(message.Body);

if (claim.Amount > 50000)
    await highValueQueue.SendAsync(message);
else if (claim.IsFraudulent)
    await fraudQueue.SendAsync(message);
else
    await standardQueue.SendAsync(message);
```

**Real-World Examples**: Insurance claims routing, order fulfillment, dynamic workflows

**Personal Usage**:
> **Not Used - Service Bus SQL Filters Preferred**
>
> I have not used the Content-Based Router pattern in production. Instead, I use Azure Service Bus topics with SQL-based subscription filters for routing based on message content. The Service Bus SQL filter syntax allows evaluating message properties and body fields directly, eliminating the need for custom routing code.
>
> - **Why not Content-Based Router**: Service Bus SQL filters handle content-based routing declaratively without deserializing or routing code
> - **Alternative approach**: Publish enriched messages with key properties as metadata, use SQL filters (`amount > 50000 AND region = 'EU'`)
> - **When you might need this**: Using Azure Storage Queue or simpler message brokers (chosen for cost/scope reasons) that lack advanced filtering and need routing based on message content
> - **Technology**: Azure Service Bus (topics + SQL subscription filters)

---

### 10. [Message Translator](10_MessageTranslator.cs)
**Problem**: Systems using incompatible message formats cannot communicate
**Solution**: Middleware translates between formats (XML ↔ JSON, etc.)

**When to Use**:
- Integrating legacy and modern systems
- Cannot change either system
- Need canonical data model

**Modern .NET**:
```csharp
// Translator middleware
public class XmlToJsonTranslator
{
    public async Task TranslateAsync(Message xmlMessage)
    {
        var xmlDoc = XDocument.Parse(xmlMessage.Body);
        var jsonPayload = JsonSerializer.Serialize(ConvertToObject(xmlDoc));

        await jsonQueue.SendAsync(new Message { Body = jsonPayload });
    }
}
```

**Real-World Examples**: Legacy system integration, API gateways, ETL pipelines

**Personal Usage**:
> **Not Used**
>
> I have not used the Message Translator pattern in production. In my experience, modern systems typically communicate using standardized formats (JSON over HTTP/messaging), eliminating the need for format translation middleware.
>
> - **Why not needed**: All integrated systems use JSON as the standard message format
> - **When you might need this**: Integrating with legacy systems that require XML, fixed-width files, or proprietary formats that cannot be modernized
> - **Alternative approaches**: API gateways with built-in transformation capabilities, Azure Logic Apps for low-code transformations

---

## 🏆 Pattern Combinations

### Outbox + Broker + Inbox = Exactly-Once Delivery
```
[Service A] → Outbox → [Broker] → Inbox → [Service B]
   (atomic)    (durable)   (redelivery)  (dedup)  (process)
```

### Splitter + Aggregator = Scatter-Gather
```
Order → [Splitter] → Item Messages → [Parallel Processing] → [Aggregator] → Decision
```

### Message Router + Content Router = Hybrid Routing
```
[Fast metadata filter] → [Business logic routing] → [Appropriate handler]
```

---

## ⚠️ Common Pitfalls

1. **Aggregator without timeout** - Memory leak from incomplete aggregations
2. **Inbox without status tracking** - Cannot distinguish retry from failure
3. **Outbox without background worker** - Messages never published
4. **No DLQ** - Poison messages block queue forever
5. **Synchronous reply waiting** - Defeats async benefits

---

[← Back to Main README](../../../README.md)
