# 0. Software Engineering Principles

**Principles** are fundamental truths or rules that guide software design decisions. Unlike patterns (which are proven solutions to recurring problems), principles are foundational guidelines that inform *how* and *why* we make architectural and design choices.

Principles are the **foundation** upon which patterns are built. Understanding these principles helps you recognize *when* to apply patterns and *why* they work.

---

## Overview

This category demonstrates **7 core software engineering principles** that every developer should understand:

| # | Principle | Description | Key Benefit |
|---|-----------|-------------|-------------|
| 1 | **SOLID** | Five object-oriented design principles | Maintainable, extensible classes |
| 2 | **DRY** | Don't Repeat Yourself | Single source of truth |
| 3 | **KISS** | Keep It Simple, Stupid | Understandable, maintainable code |
| 4 | **YAGNI** | You Aren't Gonna Need It | Avoid over-engineering |
| 5 | **Separation of Concerns** | Each component has one responsibility | Clear architecture, testability |
| 6 | **Encapsulation** | Hide internal state, expose only necessary | Data integrity, maintainability |
| 7 | **Composition Over Inheritance** | Favor object composition over class inheritance | Flexibility, loose coupling |

---

## 1. SOLID Principles

**Acronym**: Five fundamental object-oriented design principles
- **S**ingle Responsibility Principle
- **O**pen/Closed Principle
- **L**iskov Substitution Principle
- **I**nterface Segregation Principle
- **D**ependency Inversion Principle

### Real-World Usage
- **ASP.NET Core**: Controllers handle HTTP (S), middleware is extensible (O), services inject interfaces (D)
- **Entity Framework**: DbContext follows SRP, repositories follow ISP
- **Microservices**: Each service has single responsibility (S), services depend on contracts (D)
- **React/Angular**: Components follow SRP, props/interfaces follow ISP

### When to Apply
- ✅ Designing new classes or refactoring existing ones
- ✅ Reviewing code for maintainability issues
- ✅ Building systems that need to scale or evolve
- ✅ Creating libraries or frameworks (especially D and O)

### When to Avoid
- ❌ Prototypes or throwaway code
- ❌ Simple scripts or one-off utilities
- ❌ When following the principle adds significant complexity for no benefit

### Code Example
See `1_SOLID.cs` for comprehensive C# demonstrations of all five principles.

---

## 2. DRY (Don't Repeat Yourself)

**Principle**: Every piece of knowledge must have a single, unambiguous, authoritative representation within a system.

### Real-World Usage
- **Validation Logic**: FluentValidation centralizes validation rules
- **Configuration**: ASP.NET Core configuration system (appsettings.json)
- **Database Queries**: Repository pattern avoids SQL duplication
- **API Clients**: HttpClient wrappers centralize API communication
- **Logging**: Centralized logging infrastructure (Serilog, NLog)

### When to Apply
- ✅ Same logic appears in 2+ places
- ✅ Magic strings or numbers repeated throughout code
- ✅ Similar code blocks with minor variations
- ✅ Business rules scattered across layers

### When to Avoid
- ❌ Abstracting too early (wait for 3rd occurrence - "Rule of Three")
- ❌ When duplication is coincidental, not conceptual
- ❌ When abstraction makes code harder to understand

### Code Example
See `2_DRY.cs` for before/after examples of eliminating duplication.

---

## 3. KISS (Keep It Simple, Stupid)

**Principle**: Simplicity should be a key goal in design, and unnecessary complexity should be avoided.

### Real-World Usage
- **LINQ**: Use built-in methods instead of custom loops
- **Pattern Matching**: Modern C# `switch` expressions replace nested `if-else`
- **Framework Features**: ASP.NET Core middleware instead of custom pipelines
- **Standard Libraries**: `HttpClient` instead of custom HTTP wrappers
- **Minimal APIs**: .NET 6+ minimal APIs instead of full MVC for simple endpoints

### When to Apply
- ✅ Choosing between simple and complex solutions
- ✅ Deciding whether to use custom code vs. built-in framework features
- ✅ Code reviews (ask: "Is there a simpler way?")
- ✅ Refactoring legacy code

### When to Avoid
- ❌ When simplicity sacrifices correctness or performance
- ❌ When domain complexity is genuinely high (don't oversimplify complex problems)
- ❌ When "simple" means cutting corners on error handling or validation

### Code Example
See `3_KISS.cs` for examples of simplifying complex code.

---

## 4. YAGNI (You Aren't Gonna Need It)

**Principle**: Don't implement functionality until it is actually needed. Avoid building for hypothetical future requirements.

### Real-World Usage
- **Agile Development**: Implement features in sprints based on current needs
- **API Versioning**: Don't add v2 until v1 has breaking changes
- **Database Schema**: Don't add columns for "potential future features"
- **Configuration**: Don't support 5 file formats when you only use JSON
- **Microservices**: Start with monolith, split when scale demands it

### When to Apply
- ✅ Feature planning (ask: "Do we need this NOW?")
- ✅ Reviewing pull requests (ask: "Is this for current requirements?")
- ✅ Designing APIs (don't add endpoints until needed)
- ✅ Choosing libraries (don't add dependencies for future "maybe" features)

### When to Avoid
- ❌ Infrastructure decisions with high switching costs (database choice, cloud provider)
- ❌ Security features (encryption, authentication—don't cut corners)
- ❌ Compliance requirements (GDPR, HIPAA—must be built in from the start)

### Code Example
See `4_YAGNI.cs` for examples of over-engineering vs. appropriate implementation.

---

## 5. Separation of Concerns

**Principle**: A system should be divided into distinct sections, each addressing a separate concern (functionality).

### Real-World Usage
- **MVC/MVVM**: Separates presentation, logic, and data
- **Layered Architecture**: Presentation → Business Logic → Data Access → Infrastructure
- **Middleware Pipeline**: Each middleware handles one concern (logging, auth, exception handling)
- **CQRS**: Separates read operations from write operations
- **Clean Architecture**: Dependencies point inward, outer layers can be replaced

### When to Apply
- ✅ Designing system architecture
- ✅ Refactoring monolithic code
- ✅ When components have multiple reasons to change
- ✅ Building testable systems (separate dependencies for mocking)

### When to Avoid
- ❌ Simple scripts or utilities (over-separation adds unnecessary complexity)
- ❌ When the system is genuinely simple (don't create layers for 3 lines of code)
- ❌ Premature separation without clear benefits

### Code Example
See `5_SeparationOfConcerns.cs` for layered architecture examples.

---

## 6. Encapsulation

**Principle**: Hide internal state and implementation details, expose only what is necessary through a well-defined interface.

### Real-World Usage
- **Domain Models**: Private setters, validation in constructors
- **API Design**: Internal implementation hidden behind public contracts
- **State Management**: Properties with validation, computed properties
- **Security**: Sensitive data hidden, exposed through controlled interfaces
- **Database Access**: Repositories hide SQL/ORM details

### When to Apply
- ✅ Designing domain models and entities
- ✅ Creating APIs or libraries (public vs internal members)
- ✅ Protecting invariants (e.g., balance can't be negative)
- ✅ Hiding complex implementation details

### When to Avoid
- ❌ Simple DTOs or data transfer objects (no business logic)
- ❌ Internal implementation classes (not exposed to consumers)
- ❌ When excessive encapsulation reduces testability

### Code Example
See `6_Encapsulation.cs` for examples of proper data hiding and state management.

---

## 7. Composition Over Inheritance

**Principle**: Favor object composition (has-a relationships) over class inheritance (is-a relationships) for code reuse and flexibility.

### Real-World Usage
- **Dependency Injection**: Compose services via constructor injection
- **Decorator Pattern**: Wrap objects to add behavior without inheritance
- **Strategy Pattern**: Inject different algorithms (composition)
- **Middleware Pipeline**: Compose middleware components
- **React Components**: Compose components rather than inherit

### When to Apply
- ✅ When you need flexible, runtime behavior changes
- ✅ When inheritance would create deep, fragile hierarchies
- ✅ When you want to avoid tight coupling
- ✅ When behavior combinations would explode inheritance tree

### When to Avoid
- ❌ True "is-a" relationships (Car is a Vehicle)
- ❌ Framework base classes you must inherit (Controller, DbContext)
- ❌ When polymorphism genuinely simplifies the design

### Code Example
See `7_CompositionOverInheritance.cs` for examples of composition vs inheritance trade-offs.

---

## Principles vs Patterns vs Practices

### Hierarchy of Software Engineering Concepts

```
┌─────────────────────────────────────────────┐
│ PRINCIPLES (Why)                            │
│ Fundamental truths that guide decisions     │
│ Examples: SOLID, DRY, KISS, YAGNI           │
└──────────────────┬──────────────────────────┘
                   │
                   ▼
┌─────────────────────────────────────────────┐
│ PATTERNS (What)                             │
│ Proven solutions to recurring problems      │
│ Examples: Singleton, Repository, Strategy   │
└──────────────────┬──────────────────────────┘
                   │
                   ▼
┌─────────────────────────────────────────────┐
│ PRACTICES (How)                             │
│ Techniques for applying principles/patterns │
│ Examples: TDD, Code Reviews, CI/CD          │
└─────────────────────────────────────────────┘
```

### Example Chain
1. **Principle**: Dependency Inversion Principle (depend on abstractions)
2. **Pattern**: Repository Pattern (abstraction over data access)
3. **Practice**: Inject `IRepository` via constructor (dependency injection)

---

## How to Use These Principles

### 1. Learn the Principles First
Before diving into design patterns, understand these foundational principles. Patterns are *implementations* of these principles.

### 2. Use as Decision-Making Tools
When faced with design decisions, ask:
- **SOLID**: Does this class have a single responsibility? Can it be extended without modification?
- **DRY**: Am I repeating logic that should be centralized?
- **KISS**: Is there a simpler way to achieve this?
- **YAGNI**: Do I need this feature right now, or am I building for hypothetical futures?
- **SoC**: Are concerns properly separated, or is this component doing too much?

### 3. Apply in Code Reviews
Use these principles as code review criteria:
- "This violates SRP—let's split validation from persistence."
- "We're repeating this logic in 3 places (DRY violation)."
- "This is over-engineered (KISS violation)."
- "We don't have a requirement for this yet (YAGNI violation)."
- "This controller is doing data access (SoC violation)."

### 4. Balance Pragmatism with Purism
Principles are **guidelines**, not laws. Sometimes violating a principle is the right choice:
- Small projects may not need strict separation of concerns
- Prototypes can violate SOLID for speed
- Duplication is better than the wrong abstraction (temporary DRY violation)

---

## Testing Considerations

### Unit Testing Benefits
- **SOLID**: Each class has a single responsibility → easy to test in isolation
- **DRY**: Centralized logic → one test suite instead of many
- **KISS**: Simple code → simple tests
- **YAGNI**: Fewer features → fewer tests to maintain
- **SoC**: Separated concerns → mock dependencies easily

### Integration Testing
- **Separation of Concerns**: Test each layer independently, then integration points
- **Dependency Inversion**: Swap real dependencies with test doubles

---

## Common Mistakes

### 1. Over-Applying Principles
**Mistake**: Creating 10 classes for a simple calculation (SOLID overkill)  
**Fix**: Use principles to guide design, not dictate it. Small code can be simple.

### 2. Premature Abstraction
**Mistake**: Abstracting after 1st duplication (violating Rule of Three)  
**Fix**: Wait for 3rd occurrence before extracting common logic (DRY balance)

### 3. Analysis Paralysis
**Mistake**: Spending weeks designing "perfect" architecture (YAGNI violation)  
**Fix**: Build for current needs, refactor when requirements emerge

### 4. Ignoring Principles Entirely
**Mistake**: "Principles are academic, I just ship code"  
**Fix**: Principles reduce bugs, improve maintainability, and speed up long-term development

---

## Relationship to Design Patterns

Principles are **why** patterns work. Examples:

| Principle | Related Pattern | Connection |
|-----------|-----------------|------------|
| **Single Responsibility** | Repository Pattern | Data access is a single responsibility |
| **Open/Closed** | Strategy Pattern | Add new strategies without modifying context |
| **Dependency Inversion** | Dependency Injection | Depend on interfaces, not implementations |
| **Separation of Concerns** | MVC Pattern | Separates Model, View, Controller |
| **DRY** | Template Method | Centralizes common algorithm, delegates details |

---

## Quick Reference

### When You See...
- **Code duplication** → Apply **DRY**
- **Giant classes** → Apply **SRP** (Single Responsibility)
- **Hard to test** → Apply **Dependency Inversion**, **SoC**
- **Complex conditionals** → Apply **KISS**, consider **Strategy Pattern**
- **Unused features** → Apply **YAGNI**
- **Tight coupling** → Apply **Dependency Inversion**, **SoC**

### Decision Tree
```
Is the code duplicated?
├─ Yes → DRY: Extract common logic
└─ No
   ├─ Is it overly complex?
   │  └─ Yes → KISS: Simplify
   └─ No
      ├─ Is it for future needs?
      │  └─ Yes → YAGNI: Remove it
      └─ No
         ├─ Does it mix concerns?
         │  └─ Yes → SoC: Separate layers
         └─ No → Check SOLID principles
```

---

## Summary

These **7 principles** form the foundation of good software design:

1. **SOLID**: Five OOP principles for maintainable classes
2. **DRY**: Single source of truth for every piece of knowledge
3. **KISS**: Prefer simplicity over complexity
4. **YAGNI**: Build for today's needs, not tomorrow's guesses
5. **Separation of Concerns**: Each component has one clear responsibility
6. **Encapsulation**: Hide internal state, expose only necessary interfaces
7. **Composition Over Inheritance**: Favor flexible composition over rigid inheritance

**Next Steps**:
1. Run the demonstration code in `0_Principles/Patterns/`
2. Review existing code against these principles
3. Apply principles in daily development
4. Move to Design Patterns (Categories 1-9) to see principles in action
