# Anti-Patterns

Common bad practices to avoid, with examples of the problems they cause and how to refactor them into good code.

## 📖 About Anti-Patterns

Anti-patterns are common solutions that appear reasonable but lead to problems:
- **Technical Debt**: Quick fixes that become long-term burdens
- **Maintenance Hell**: Code that's increasingly difficult to change
- **Hidden Bugs**: Design flaws that mask or create defects
- **Team Friction**: Code that frustrates developers and slows delivery

**Important**: These are patterns to **AVOID**. Each example shows both the problem and the proper solution.

---

## 🎯 Anti-Patterns

### 1. [Spaghetti Code](1_SpaghettiCode.cs)
**Problem**: Tangled, deeply nested code with no clear structure, making it impossible to understand or modify
**Impact**: High bug rate, long debugging sessions, fear of making changes, new developers overwhelmed

**Warning Signs**:
- 5+ levels of indentation
- Methods over 50 lines
- No clear flow or structure
- Can't understand what code does without debugger
- Change in one place breaks seemingly unrelated code

**Root Causes**:
- No separation of concerns
- Everything in one method
- Fear of extracting methods
- "Just get it working" mentality

**Refactoring Strategy**:
```csharp
// ❌ BAD: Spaghetti code
public void ProcessOrder(Order order)
{
    if (order != null)
    {
        if (order.Items.Count > 0)
        {
            if (order.Customer != null)
            {
                if (order.Customer.IsActive)
                {
                    // ... 10 more levels of nesting
                    // ... mixed validation, calculation, persistence
                    // ... impossible to follow
                }
            }
        }
    }
}

// ✅ GOOD: Clean, structured code
public void ProcessOrder(Order order)
{
    if (!ValidateOrder(order))
        return;

    var total = CalculateTotal(order);
    SaveOrder(order, total);
    NotifyCustomer(order.Customer);
}

private bool ValidateOrder(Order order)
{
    if (order == null) return false;
    if (order.Items.Count == 0) return false;
    if (order.Customer?.IsActive != true) return false;
    return true;
}
```

**Key Fixes**:
- Extract methods with single responsibilities
- Use early returns to reduce nesting
- Separate validation, business logic, and side effects
- Clear naming that reveals intent

**Real-World Examples**: Legacy codebases, rushed features, code without reviews

**Personal Experience**:
> **Legacy Payment Processing Refactoring**
>
> Inherited 500-line payment processing method with 8 levels of nesting, 30+ local variables, and mixed concerns.
>
> - **Problem**: Every bug fix broke something else, 2-hour debugging sessions, new features took weeks
> - **Refactoring**: Extracted 15 focused methods (validation, calculation, persistence, notification), added tests
> - **Result**: Bug fix time dropped from hours to minutes, new features from weeks to days, team velocity doubled
> - **Lesson**: Spaghetti code looks like it works but is a hidden productivity killer

---

### 2. [God Object](2_GodObject.cs)
**Problem**: A single class that knows too much and does too much, violating Single Responsibility Principle
**Impact**: Tight coupling, impossible to test, changes ripple everywhere, merge conflicts

**Warning Signs**:
- Class has 1000+ lines
- Dozens of methods and properties
- Name contains "Manager", "Handler", "Util", "Helper"
- Everyone touches this file (merge conflict hell)
- Can't describe class purpose in one sentence

**Root Causes**:
- "Just add it to the existing class"
- Fear of creating new classes
- Lack of domain modeling
- No clear architecture

**Refactoring Strategy**:
```csharp
// ❌ BAD: God object doing everything
public class OrderManager
{
    public void ProcessOrder() { } // Business logic
    public void SaveToDatabase() { } // Persistence
    public void SendEmail() { } // Notifications
    public void ValidateInput() { } // Validation
    public void CalculateTax() { } // Calculations
    public void LogActivity() { } // Logging
    // ... 50 more methods
}

// ✅ GOOD: Focused classes with single responsibilities
public class OrderValidator
{
    public bool Validate(Order order) { }
}

public class OrderCalculator
{
    public decimal CalculateTotal(Order order) { }
}

public class OrderRepository
{
    public void Save(Order order) { }
}

public class OrderNotifier
{
    public void NotifyCustomer(Order order) { }
}

public class OrderProcessor
{
    private readonly OrderValidator _validator;
    private readonly OrderCalculator _calculator;
    private readonly OrderRepository _repository;
    private readonly OrderNotifier _notifier;

    // Orchestrates focused components
    public void Process(Order order) { }
}
```

**Key Fixes**:
- Identify distinct responsibilities
- Extract each responsibility to focused class
- Use dependency injection for composition
- Follow Single Responsibility Principle

**Real-World Examples**: "Manager" classes, utility classes, service classes that grow over time

**Personal Experience**:
> **ApplicationService God Object**
>
> 3000-line ApplicationService class that handled everything: validation, business logic, database access, email, logging, caching.
>
> - **Problem**: 4 developers constantly had merge conflicts, couldn't test anything in isolation, changes broke unrelated features
> - **Refactoring**: Extracted 8 focused services (Validator, Calculator, Repository, Notifier, etc.), added interfaces
> - **Result**: Merge conflicts disappeared, unit test coverage went from 0% to 80%, parallel development possible
> - **Lesson**: God objects are magnets for technical debt and team bottlenecks

---

### 3. [Golden Hammer](3_GoldenHammer.cs)
**Problem**: Using the same solution for every problem ("When all you have is a hammer, everything looks like a nail")
**Impact**: Overcomplicated simple problems, poor performance, maintenance burden, wrong tool for the job

**Warning Signs**:
- Team insists on using X for everything
- Simple problems have complex solutions
- "We always use X" without considering alternatives
- Technology/pattern is the solution searching for a problem
- Resume-driven development

**Root Causes**:
- Comfort zone (only know one tool)
- Cargo cult ("Big companies use X")
- Resume building
- Not evaluating trade-offs

**Common Examples**:
```csharp
// ❌ BAD: Using regex for everything
var containsAt = Regex.IsMatch(email, "@"); // Overkill!
// ✅ GOOD: Use simple string method
var containsAt = email.Contains('@');

// ❌ BAD: Microservices for simple CRUD app (15 services)
// ✅ GOOD: Modular monolith (can extract later if needed)

// ❌ BAD: Design patterns everywhere (AbstractFactoryFactory)
// ✅ GOOD: YAGNI - simple solution for simple problem

// ❌ BAD: Blockchain for internal audit log
// ✅ GOOD: Database with immutable append-only table

// ❌ BAD: Machine learning for if/else logic
// ✅ GOOD: Business rules engine or simple conditionals
```

**Key Principles**:
- Evaluate context before choosing solution
- Simple problems deserve simple solutions
- Consider trade-offs (complexity, performance, maintenance)
- Use the right tool for the job

**Real-World Examples**: Microservices for everything, regex abuse, unnecessary design patterns, blockchain for everything

**Personal Experience**:
> **The Microservices Obsession**
>
> Team read about microservices, immediately split simple CRUD app into 12 microservices.
>
> - **Problem**: Simple page load required 8 service calls, distributed debugging nightmare, deployment complexity 10x
> - **Reality check**: App had 1000 users, monolith could handle 100,000+
> - **Refactoring**: Merged back to modular monolith with clear bounded contexts
> - **Result**: Deployment time from 2 hours to 5 minutes, development velocity tripled, infrastructure costs dropped 60%
> - **Lesson**: Start simple, evolve architecture when needed (YAGNI applies to architecture too)

---

### 4. [Reinventing the Wheel](4_ReinventingTheWheel.cs)
**Problem**: Building custom solutions for problems already solved by mature, battle-tested libraries
**Impact**: Wasted time, buggy implementations, maintenance burden, missing edge cases

**Warning Signs**:
- "Let's build our own JSON parser"
- "I wrote a better logging framework"
- "Our custom ORM is simpler than Entity Framework"
- Custom HTTP client, date library, encryption
- Not searching for existing solutions first

**Root Causes**:
- NIH (Not Invented Here) syndrome
- Underestimating complexity
- Overconfidence
- Ignorance of existing solutions
- "How hard can it be?"

**Common Reinventions**:
```csharp
// ❌ BAD: Custom JSON parser (months of work, still buggy)
// ✅ GOOD: System.Text.Json (1 line, production-ready)
var person = JsonSerializer.Deserialize<Person>(json);

// ❌ BAD: Custom HTTP client (sockets, connection pooling, timeouts)
// ✅ GOOD: HttpClient with HttpClientFactory
services.AddHttpClient<IApiClient, ApiClient>();

// ❌ BAD: Custom dependency injection container
// ✅ GOOD: Microsoft.Extensions.DependencyInjection

// ❌ BAD: Custom validation framework
// ✅ GOOD: FluentValidation

// ❌ BAD: Custom logging framework
// ✅ GOOD: Microsoft.Extensions.Logging / Serilog

// ❌ BAD: Custom ORM (6 months of development)
// ✅ GOOD: Entity Framework Core / Dapper
```

**When to Build Custom**:
- Existing solutions don't fit requirements
- License/cost prohibitive
- Performance critical and can prove custom is faster
- Very simple, truly unique to domain

**Key Principles**:
- Research before building
- Leverage mature, maintained libraries
- Focus on unique business value
- Custom code is a liability (maintenance cost)

**Real-World Examples**: Custom ORMs, JSON parsers, HTTP clients, DI containers, logging frameworks

**Personal Experience**:
> **The Custom Validation Framework**
>
> Team decided FluentValidation was "too complicated", built custom validation framework with attributes and reflection.
>
> - **Reality**: Custom validators had inconsistent error messages, no support for async validation, difficult to test, missing conditional rules
> - **Complexity**: Nested validation required custom visitor pattern, dependency injection integration was buggy
> - **Maintenance**: Every new validation scenario required framework changes, 3 months building what FluentValidation does out-of-the-box
> - **Outcome**: Migrated to FluentValidation in 2 weeks, deleted 5,000 lines of custom validation code
> - **Lesson**: Mature libraries handle edge cases you haven't thought of. Focus on business logic, not infrastructure.

---

### 5. [Copy-Paste Programming](5_CopyPasteProgramming.cs)
**Problem**: Duplicating code instead of extracting common logic into reusable functions
**Impact**: Bugs need fixing in multiple places, inconsistent behavior, code bloat, maintenance nightmare

**Warning Signs**:
- Same code appears 3+ times
- Bug fixes require "finding all copies"
- Slight variations of same logic everywhere
- Comments like "same as method X but for Y"
- Search finds 10 instances of similar code

**Root Causes**:
- Deadline pressure ("just copy it")
- Fear of breaking existing code
- Not recognizing patterns
- Lack of refactoring skills
- "It's only a few lines"

**Refactoring Strategy**:
```csharp
// ❌ BAD: Duplicated validation (copy-pasted)
public void ValidateUser(User user)
{
    if (string.IsNullOrEmpty(user.Email) || !user.Email.Contains("@"))
        throw new Exception("Invalid email");
    if (user.Age < 18)
        throw new Exception("Must be 18+");
}

public void ValidateCustomer(Customer customer)
{
    if (string.IsNullOrEmpty(customer.Email) || !customer.Email.Contains("@"))
        throw new Exception("Invalid email");
    if (customer.Age < 18)
        throw new Exception("Must be 18+");
}

public void ValidateContact(Contact contact)
{
    if (string.IsNullOrEmpty(contact.Email) || !contact.Email.Contains("@"))
        throw new Exception("Invalid email");
    if (contact.Age < 18)
        throw new Exception("Must be 18+");
}

// ✅ GOOD: Extracted common validation
public void ValidateEntity(string email, int age, string entityType)
{
    if (!IsValidEmail(email))
        throw new Exception($"{entityType} email invalid");
    if (!IsValidAge(age))
        throw new Exception($"{entityType} must be 18+");
}

private bool IsValidEmail(string email) =>
    !string.IsNullOrEmpty(email) && email.Contains("@");

private bool IsValidAge(int age) => age >= 18;
```

**Key Fixes**:
- Extract common logic to methods/classes
- Use generic types/methods when appropriate
- Create utility classes for shared functionality
- Follow DRY (Don't Repeat Yourself)

**Real-World Examples**: Repeated validation, data access code, formatting logic, API clients

**Personal Experience**:
> **Copy-Paste Multiplies Codebase Size**
>
> Regularly encounter codebases that are 3-4x longer than necessary due to copy-paste programming.
>
> - **Pattern**: Similar logic repeated with slight variations (validation, formatting, data access, API calls)
> - **Approach**: As soon as I see something similar twice, I abstract immediately before it spreads
> - **Techniques**: Split classes and use composition, extract to utility methods, pass `Func<>` parameters for varying behavior
> - **Result**: Codebase shrinks 50-75%, bugs fixed once instead of hunting copies, new features faster
> - **Lesson**: Copy-paste isn't an easy fix later - it multiplies exponentially. Abstract early, prevent the explosion.

---

## 🏆 How Anti-Patterns Happen

### Pressure + Shortcuts = Technical Debt
```
Deadline pressure → "Just copy this code" → Anti-pattern established → 
Others copy the anti-pattern → Becomes "how we do things" → Technical debt compounds
```

### Knowledge Gaps
```
Don't know pattern exists → Reinvent badly → Becomes anti-pattern → 
Team copies it → Bad practice spreads
```

### Analysis Paralysis → Overcomplexity
```
Fear of future change → "What if we need to scale to 1 billion users?" →
Premature optimization → Golden Hammer → Overengineered mess
```

---

## ⚠️ Breaking Anti-Pattern Cycles

### 1. Code Reviews
- Catch anti-patterns early
- Share knowledge
- Enforce standards

### 2. Refactoring Time
- Budget time for cleanup
- Don't just add features
- Boy Scout Rule: leave code better than you found it

### 3. Continuous Learning
- Learn patterns and anti-patterns
- Study good codebases
- Read books (Clean Code, Refactoring)

### 4. Automated Checks
- Static analysis (SonarQube, Roslyn analyzers)
- Code complexity metrics
- Duplication detection

### 5. Team Culture
- Safe to refactor
- Praise clean code
- Technical excellence matters

---

## 📊 Real-World Impact

### Spaghetti Code
- **Symptom**: "No one understands this module"
- **Cost**: 10x longer debugging time
- **Fix**: Extract methods, add tests, refactor incrementally

### God Object  
- **Symptom**: Merge conflicts on every PR
- **Cost**: Blocked development, long PR review times
- **Fix**: Split responsibilities, use interfaces, dependency injection

### Golden Hammer
- **Symptom**: "Why is this simple feature so complex?"
- **Cost**: Over-engineering, unnecessary dependencies
- **Fix**: Simplify, use appropriate tools, YAGNI

### Reinventing the Wheel
- **Symptom**: "Our custom X has bugs again"
- **Cost**: Wasted development time, maintenance burden
- **Fix**: Replace with proven library, focus on business value

### Copy-Paste Programming
- **Symptom**: "I fixed this bug in 5 places, is that all?"
- **Cost**: Bugs, inconsistency, code bloat
- **Fix**: Extract common code, create utilities, enforce DRY

---

[← Back to Main README](../../../README.md)
