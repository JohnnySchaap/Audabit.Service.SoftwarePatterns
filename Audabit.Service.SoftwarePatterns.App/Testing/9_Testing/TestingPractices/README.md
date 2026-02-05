# Testing Practices

Practices and methodologies for writing effective tests that verify correctness, prevent regressions, and enable confident refactoring.

## 📖 About Testing Practices

Testing practices help build reliable, maintainable test suites:
- **Confidence**: Verify behavior and catch regressions early
- **Design Feedback**: Tests reveal design issues before production
- **Documentation**: Tests describe how code should behave
- **Refactoring Safety**: Change implementation with confidence

**Note**: These are testing **practices** and **methodologies**, not traditional design patterns. They represent approaches and techniques for effective software testing.

---

## 🎯 Practices

### 1. [Test Pyramid](1_TestPyramid.cs)
**Concept**: A way of thinking about test distribution (not a pattern)
**Problem**: All tests at one level (only E2E or only unit tests) is inefficient and brittle
**Solution**: Balance test types - many fast unit tests, fewer integration tests, minimal E2E tests

**When to Use**:
- Building test strategy for a project
- Balancing test speed vs. comprehensiveness
- Optimizing CI/CD pipeline speed
- Deciding where to invest testing effort

**Modern .NET**:
```csharp
// Unit Tests (70%) - Fast, isolated, many
[Fact]
public void CalculateTotal_WithValidItems_ReturnsSum()
{
    var calculator = new OrderCalculator();
    var items = new[] { 10m, 20m, 30m };
    
    var total = calculator.CalculateTotal(items);
    
    Assert.Equal(60m, total);
}

// Integration Tests (20%) - Medium speed, real dependencies
public class OrderServiceTests : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task CreateOrder_SavesToDatabase()
    {
        var client = _factory.CreateClient();
        
        var response = await client.PostAsync("/api/orders", ...);
        
        var order = await _dbContext.Orders.FindAsync(orderId);
        Assert.NotNull(order);
    }
}

// E2E Tests (10%) - Slow, full system, few (webdriver.io)
[Fact]
public async Task CompleteCheckout_CreatesOrderAndSendsEmail()
{
    await _browser.GotoAsync("https://localhost/products/123");
    await _browser.ClickAsync("#add-to-cart");
    await _browser.ClickAsync("#checkout");
    await _browser.FillAsync("#card-number", "4111111111111111");
    await _browser.ClickAsync("#submit-order");
    
    await _browser.WaitForSelectorAsync("#order-confirmation");
}
```

**Recommended Ratios**: 70% unit, 20% integration, 10% E2E

**Real-World Examples**: xUnit unit tests, WebApplicationFactory integration tests, webdriver.io E2E tests

**Personal Usage**:
> **ASP.NET Core Test Strategy**
>
> Test pyramid guides my testing strategy for all .NET applications.
>
> - **Unit tests (70%)**: All business logic, validators, utilities - run in < 5 seconds, 1000+ tests
> - **Integration tests (20%)**: API endpoints with TestServer, repository with in-memory DB - run in ~2 minutes, 200 tests
> - **E2E tests (10%)**: Critical user workflows with Playwright - run nightly, 20 tests
> - **Result**: Fast feedback (unit tests on save), comprehensive coverage, confident deployments
- **Technology**: xUnit, NSubstitute, WebApplicationFactory, webdriver.io

---

### 2. [Mock, Stub, Fake](2_MockStubFake.cs)
**Technique**: Test doubles for isolating units under test (not a pattern)
**Problem**: Testing with real dependencies is slow, unreliable, and sometimes impossible
**Solution**: Use test doubles - mocks (verify behavior), stubs (canned responses), fakes (simplified implementations)

**When to Use**:
- Testing code with external dependencies (databases, APIs, file system)
- Verifying method calls and interactions (mocks)
- Controlling return values for specific scenarios (stubs)
- Need working implementation without full complexity (fakes)

**Modern .NET**:
```csharp
// STUB: Returns predefined data
var stubRepo = Substitute.For<IOrderRepository>();
stubRepo.GetById("123").Returns(new Order { Id = "123", Total = 100m });

// MOCK: Verifies method was called
var mockEmailer = Substitute.For<IEmailSender>();
service.CompleteOrder("123");
mockEmailer.Received(1).SendEmail(Arg.Any<string>(), Arg.Any<string>());

// FAKE: Working in-memory implementation
public class FakeDatabase : IDatabase
{
    private readonly Dictionary<string, Order> _orders = new();
    
    public void Save(Order order) => _orders[order.Id] = order;
    public Order Get(string id) => _orders[id];
}

// Using EF Core in-memory database (common fake)
services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("TestDb"));
```

**When to Use Which**:
- **Stub**: "Return this value when called" - simple test data
- **Mock**: "Verify this method was called" - behavior verification
- **Fake**: "Working implementation" - complex dependencies (databases)

**Real-World Examples**: NSubstitute mocks, in-memory EF Core database, fake file system

**Personal Usage**:
> **HTTP Client Testing**
>
> I use different test doubles depending on what I'm verifying.
>
> - **Stubs**: HTTP message handlers returning canned JSON responses for API client tests
> - **Mocks**: Verify retry logic calls HttpClient with correct parameters (NSubstitute)
> - **Fakes**: In-memory EF Core database for repository tests (faster than real SQL)
> - **Result**: Fast, reliable tests without real external dependencies
> - **Technology**: NSubstitute for mocks/stubs, EF Core InMemory for fake database, HttpMessageHandler fakes

---

### 3. [TDD (Test-Driven Development)](3_TDD.cs)
**Methodology**: A software development approach (not a pattern)
**Problem**: Tests written after code often miss edge cases and don't drive good design
**Solution**: Write tests FIRST using Red-Green-Refactor cycle

**When to Use**:
- Starting new feature development
- Complex business logic with clear requirements
- Want to ensure high test coverage
- Need confidence to refactor

**Modern .NET**:
```csharp
// RED: Write failing test
[Fact]
public void CalculateDiscount_WithVIPCustomer_Returns20PercentOff()
{
    var calculator = new PriceCalculator(); // Doesn't exist yet!
    
    var price = calculator.CalculateDiscount(100m, CustomerType.VIP);
    
    Assert.Equal(80m, price); // Test fails - RED
}

// GREEN: Minimal implementation to pass
public class PriceCalculator
{
    public decimal CalculateDiscount(decimal price, CustomerType type)
    {
        if (type == CustomerType.VIP)
            return price * 0.8m; // Simplest code to pass test
        return price;
    }
}
// Test passes - GREEN

// REFACTOR: Improve code quality (tests stay green)
public class PriceCalculator
{
    private const decimal VipDiscountRate = 0.20m;
    
    public decimal CalculateDiscount(decimal price, CustomerType type)
    {
        var discountRate = GetDiscountRate(type);
        return price * (1 - discountRate);
    }
    
    private decimal GetDiscountRate(CustomerType type) =>
        type == CustomerType.VIP ? VipDiscountRate : 0m;
}
// Tests still pass - REFACTOR complete
```

**The Cycle**:
1. **Red**: Write test, see it fail
2. **Green**: Write minimal code to pass
3. **Refactor**: Improve code while keeping tests green
4. Repeat

**Real-World Examples**: Business logic, algorithms, parsers, validators

**Personal Usage**:
> **Complex Business Rules**
>
> TDD is my go-to approach for complex business logic with clear requirements.
>
> - **Example**: Pricing calculation with multiple discount rules, tax variations, promotions
> - **Process**: Write test for each rule, implement minimally, refactor once all tests pass
> - **Result**: 100% test coverage, YAGNI (only implement tested features), refactor fearlessly
> - **Technology**: xUnit, Theory/InlineData for parameterized tests, NSubstitute for dependencies

---

### 4. [BDD (Behavior-Driven Development)](4_BDD.cs)
**Methodology**: A software development approach combining TDD with domain-driven design (not a pattern)
**Problem**: Tests focus on implementation details instead of business behavior
**Solution**: Write tests as executable specifications using Given-When-Then scenarios

**When to Use**:
- Business requirements are clear and stable
- Need shared language between developers, QA, and business
- Want tests to serve as documentation
- Testing user-facing behavior

**Modern .NET**:
```csharp
// Using Given-When-Then structure in xUnit
[Fact]
public void User_can_login_with_valid_credentials()
{
    // GIVEN a registered user
    var authService = new AuthService();
    authService.Register("user@example.com", "SecurePass123");
    
    // WHEN the user attempts to login
    var result = authService.Login("user@example.com", "SecurePass123");
    
    // THEN login should succeed
    Assert.True(result.IsSuccess);
    Assert.NotNull(result.Token);
}

// Using SpecFlow (Gherkin syntax) for full BDD
Feature: User Authentication
  Scenario: Successful login with valid credentials
    Given a user account exists with email "user@example.com"
    And the password is "SecurePass123"
    When the user attempts to login
    Then login should succeed
    And a session token should be returned

// Step definitions (C# code)
[Given(@"a user account exists with email ""(.*)""")]
public void GivenUserExists(string email)
{
    _authService.Register(email, _password);
}
```

**Scenario Structure**:
- **Given**: Context and preconditions
- **When**: Action being tested
- **Then**: Expected outcomes
- **And**: Additional conditions/outcomes

**Real-World Examples**: User authentication, order processing, payment flows, approval workflows

**Personal Usage**:
> **API Endpoint Behavior Specifications**
>
> BDD scenarios make API behavior clear and testable.
>
> - **Challenge**: Complex API flows with multiple edge cases (authentication, validation, authorization, business rules)
> - **Solution**: Given-When-Then tests for each scenario, read like business requirements
> - **Result**: Tests double as documentation, shared understanding with product team, clear acceptance criteria
> - **Technology**: xUnit with Given-When-Then comments, SpecFlow for critical features, living documentation

---

### 5. [Golden Master (Snapshot Testing)](5_GoldenMaster.cs)
**Technique**: A testing approach for legacy code and complex outputs (not a pattern)
**Problem**: Legacy code lacks tests; refactoring is risky without knowing current behavior
**Solution**: Capture current output as "golden master" baseline, compare after refactoring

**When to Use**:
- Refactoring legacy code without tests (characterization testing)
- Testing complex transformations (reports, data migrations)
- Verifying refactoring doesn't change behavior
- Snapshot testing for UI components

**Modern .NET**:
```csharp
// Capture golden master baseline
[Fact]
public void GenerateReport_ProducesExpectedOutput()
{
    var report = ReportGenerator.Generate(sampleData);
    var actualJson = JsonSerializer.Serialize(report);
    
    // First run: captures golden master to file
    // Subsequent runs: compares against golden master
    VerifyJson(actualJson).ToFile("report-golden-master.json");
}

// Using ApprovalTests.NET
[Fact]
public void GenerateInvoice_MatchesApprovedOutput()
{
    var invoice = InvoiceGenerator.Generate(order);
    var html = invoice.ToHtml();
    
    // Compares against InvoiceGenerator.approved.html
    // Differences launch diff tool for review
    Approvals.Verify(html);
}

// Custom golden master comparison
public void CompareAgainstGoldenMaster(string actual, string goldenMasterFile)
{
    var golden = File.ReadAllText(goldenMasterFile);
    
    if (actual != golden)
    {
        File.WriteAllText(goldenMasterFile + ".new", actual);
        throw new Exception($"Output differs from golden master. Review {goldenMasterFile}.new");
    }
}
```

**Workflow**:
1. **Capture**: Run code, save output as golden master
2. **Refactor**: Change implementation
3. **Compare**: Run again, compare against golden master
4. **Review**: If different, either fix bug or update golden master

**Real-World Examples**: Report generation, data transformations, API response formats, UI snapshots

**Personal Usage**:
> **Legacy Code Refactoring**
>
> Golden master testing gives confidence when refactoring code without tests.
>
> - **Challenge**: Legacy reporting system needs performance optimization but has no tests
> - **Solution**: Capture JSON output from 100 report variations as golden masters, refactor, compare
> - **Result**: Caught 3 subtle bugs during refactoring, confidence to optimize aggressively, tests for future changes
> - **Technology**: ApprovalTests.NET, xUnit, JSON serialization for output comparison

---

## 🏆 Pattern Combinations

### Test Pyramid + TDD = Comprehensive Coverage
```
TDD drives unit tests (70%) → Integration tests verify integration (20%) → E2E tests verify workflows (10%)
```

### BDD + Test Pyramid = Business-Driven Testing
```
BDD scenarios define behavior → Unit tests implement details → Integration/E2E verify end-to-end
```

### Golden Master → TDD = Legacy to Modern
```
Golden Master (characterization) → TDD (add features) → Full test coverage achieved
```

### Mocks/Stubs + TDD = Isolated Unit Tests
```
TDD test first → Stub dependencies for state → Mock dependencies to verify interactions
```

---

## ⚠️ Common Pitfalls

1. **Inverted pyramid** - Too many E2E tests, too few unit tests (slow, brittle)
2. **Testing implementation details** - Tests break when refactoring (couple to behavior, not implementation)
3. **Overusing mocks** - Excessive mocking makes tests fragile (prefer real objects when practical)
4. **No test organization** - Hard to find failing tests (use Given-When-Then, clear naming)
5. **Flaky tests** - Intermittent failures destroy trust (fix or delete flaky tests immediately)
6. **Golden master without review** - Blindly accepting changes (always review diffs)
7. **Not updating golden masters** - Intentional changes should update baseline

---

## 📊 Real-World Testing Strategy

### ASP.NET Core Web API
```
Unit Tests (70%):
- Business logic services
- Validators (FluentValidation)
- Utilities and helpers
- Domain models

Integration Tests (20%):
- API endpoints (WebApplicationFactory)
- Database repositories (in-memory EF Core)
- Message handlers (in-memory Service Bus)

E2E Tests (10%):
- Critical user flows (webdriver.io)
- Payment processing
- Authentication flows
```

### Test Execution
```
On Save:        Unit tests (5 seconds)
On Commit:      Unit + Integration (2 minutes)
On PR:          All tests (10 minutes)
Nightly:        E2E tests (30 minutes)
```

### TDD for Business Logic
```
1. Write unit test (Red)
2. Implement minimally (Green)
3. Refactor (tests stay Green)
4. Add integration test for full flow
5. Verify with BDD scenario
```

---

[← Back to Main README](../../../README.md)
