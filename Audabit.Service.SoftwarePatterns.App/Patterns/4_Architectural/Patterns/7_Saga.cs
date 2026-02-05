namespace Audabit.Service.SoftwarePatterns.App.Patterns._4_Architectural.Patterns;

// Saga Pattern: Manages long-running distributed transactions across multiple services by coordinating a sequence of local transactions with compensating actions for rollback.
// LEVEL: Code-level architecture pattern (orchestration logic for distributed transactions, though spans multiple services)
// In this example:
// - We orchestrate a travel booking across multiple services (Flight, Hotel, Payment).
// - Each step is a local transaction that can succeed or fail.
// - If any step fails, compensating transactions undo previous successful steps.
// - The key here is that we achieve distributed transaction semantics without a distributed transaction coordinator (2PC).
//
// MODERN C# USAGE:
// Saga pattern is essential for microservices architectures in .NET:
// - Use MassTransit with state machines for saga orchestration
// - NServiceBus Sagas for long-running business processes
// - Azure Durable Functions for serverless saga orchestration
// - Implement either Orchestration (coordinator) or Choreography (events) style
// - Store saga state in durable storage (Azure Cosmos DB, SQL Server)
// - Handle idempotency and duplicate message detection
// - Common in e-commerce (order processing), travel booking, financial transactions
// - Alternative to distributed transactions which don't scale in microservices
public static class Saga
{
    public static void Run()
    {
        Console.WriteLine("Saga Pattern: Coordinating distributed transaction across services...");
        Console.WriteLine("Saga Pattern: Each step has a compensating transaction for rollback.\n");

        var saga = new TravelBookingSaga();

        Console.WriteLine("Saga Pattern: --- Scenario 1: Successful Booking ---");
        saga.BookTravel("user@example.com", hasPayment: true);

        Console.WriteLine("\n\nSaga Pattern: --- Scenario 2: Payment Fails - Rollback Required ---");
        saga.BookTravel("user@example.com", hasPayment: false);
    }
}

// ============================================
// SAGA ORCHESTRATOR
// ============================================
public class TravelBookingSaga
{
    private readonly FlightService _flightService = new();
    private readonly HotelService _hotelService = new();
    private readonly PaymentService _paymentService = new();

    public void BookTravel(string customerEmail, bool hasPayment)
    {
        string? flightReservation = null;
        string? hotelReservation = null;

#pragma warning disable CA1031 // Do not catch general exception types - intentional for demo
        try
        {
            Console.WriteLine($"Saga Pattern: [Saga Orchestrator] Starting travel booking for {customerEmail}");

            // Step 1: Book flight
            Console.WriteLine("\nSaga Pattern: [Saga Orchestrator] Step 1: Booking flight...");
            flightReservation = _flightService.BookFlight(customerEmail);
            Console.WriteLine($"Saga Pattern: [Saga Orchestrator] Flight booked: {flightReservation}");

            // Step 2: Book hotel
            Console.WriteLine("\nSaga Pattern: [Saga Orchestrator] Step 2: Booking hotel...");
            hotelReservation = _hotelService.BookHotel(customerEmail);
            Console.WriteLine($"Saga Pattern: [Saga Orchestrator] Hotel booked: {hotelReservation}");

            // Step 3: Process payment
            Console.WriteLine("\nSaga Pattern: [Saga Orchestrator] Step 3: Processing payment...");
            var paymentSuccess = _paymentService.ChargePayment(customerEmail, 1500m, hasPayment);

            if (!paymentSuccess)
            {
                throw new Exception("Payment failed");
            }

            Console.WriteLine("\nSaga Pattern: [Saga Orchestrator] ✓ All steps completed successfully!");
            Console.WriteLine($"Saga Pattern: [Saga Orchestrator] Booking confirmed for {customerEmail}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nSaga Pattern: [Saga Orchestrator] ✗ Error: {ex.Message}");
            Console.WriteLine("Saga Pattern: [Saga Orchestrator] Starting compensating transactions (rollback)...");

            // Compensate in reverse order
            if (hotelReservation != null)
            {
                Console.WriteLine("\nSaga Pattern: [Saga Orchestrator] Compensating: Cancelling hotel...");
                _hotelService.CancelHotel(hotelReservation);
            }

            if (flightReservation != null)
            {
                Console.WriteLine("Saga Pattern: [Saga Orchestrator] Compensating: Cancelling flight...");
                _flightService.CancelFlight(flightReservation);
            }

            Console.WriteLine("\nSaga Pattern: [Saga Orchestrator] Rollback completed. Booking cancelled.");
        }
#pragma warning restore CA1031
    }
}

// ============================================
// SERVICES (Each with local transactions)
// ============================================
#pragma warning disable CA1822 // Mark members as static - intentionally instance methods for demo
public class FlightService
{
    public string BookFlight(string customerEmail)
    {
        var reservationId = $"FLIGHT-{Guid.NewGuid().ToString()[..8].ToUpper()}";
        Console.WriteLine($"Saga Pattern: [Flight Service] Reserved flight for {customerEmail}");
        Console.WriteLine($"Saga Pattern: [Flight Service] Reservation ID: {reservationId}");
        return reservationId;
    }

    public void CancelFlight(string reservationId)
    {
        Console.WriteLine($"Saga Pattern: [Flight Service] Cancelled flight reservation: {reservationId}");
        Console.WriteLine($"Saga Pattern: [Flight Service] Seat released back to inventory");
    }
}

public class HotelService
{
    public string BookHotel(string customerEmail)
    {
        var reservationId = $"HOTEL-{Guid.NewGuid().ToString()[..8].ToUpper()}";
        Console.WriteLine($"Saga Pattern: [Hotel Service] Reserved hotel for {customerEmail}");
        Console.WriteLine($"Saga Pattern: [Hotel Service] Reservation ID: {reservationId}");
        return reservationId;
    }

    public void CancelHotel(string reservationId)
    {
        Console.WriteLine($"Saga Pattern: [Hotel Service] Cancelled hotel reservation: {reservationId}");
        Console.WriteLine($"Saga Pattern: [Hotel Service] Room released back to inventory");
    }
}

public class PaymentService
{
    public bool ChargePayment(string customerEmail, decimal amount, bool hasPayment)
    {
        Console.WriteLine($"Saga Pattern: [Payment Service] Attempting to charge ${amount} to {customerEmail}");

        if (!hasPayment)
        {
            Console.WriteLine($"Saga Pattern: [Payment Service] Payment failed - insufficient funds");
            return false;
        }

        Console.WriteLine($"Saga Pattern: [Payment Service] Payment successful");
        return true;
    }
}
#pragma warning restore CA1822