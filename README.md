# Audabit Software Patterns

A comprehensive reference implementation of software design patterns in modern .NET 10.0, demonstrating production-grade solutions and deep understanding of enterprise architecture.

> **💡 For Recruiters & Technical Evaluators**: This repository showcases practical mastery of software patterns through real-world implementations, complete with production examples from enterprise systems I've worked on. Each pattern includes detailed explanations, modern .NET code, and documentation of how I've applied these patterns in production distributed systems (order & payment processing, Service Bus messaging, POS integration, GDPR tasks (e.g. customer data deletion) etc.).

## 📚 Explore the Categories

**Each category contains a comprehensive README** with detailed explanations, real-world problems they solve, modern .NET implementations, production usage examples, and best practices. Click through to see in-depth technical analysis and code examples.

### [0. Principles](Audabit.Service.SoftwarePatterns.App/Principles/0_Principles/Principles) ⭐ **Foundation**
Fundamental truths that guide all software design decisions.
- SOLID (Single Responsibility, Open/Closed, Liskov Substitution, Interface Segregation, Dependency Inversion)
- DRY (Don't Repeat Yourself), KISS (Keep It Simple), YAGNI (You Aren't Gonna Need It)
- Separation of Concerns, Encapsulation, Composition Over Inheritance

### [1. Creational Patterns](Audabit.Service.SoftwarePatterns.App/Patterns/1_Creational/Patterns)
Object creation mechanisms providing flexibility in instantiation.
- Singleton, Lazy Initialization, Simple Factory, Factory Method, Abstract Factory
- Builder, Prototype, Object Pool, Dependency Injection

### [2. Structural Patterns](Audabit.Service.SoftwarePatterns.App/Patterns/2_Structural/Patterns)
Object composition patterns for building flexible, maintainable structures.
- Adapter, Bridge, Composite, Decorator, Facade
- Flyweight, Proxy, Marker Interface, Pipes and Filters

### [3. Behavioral Patterns](Audabit.Service.SoftwarePatterns.App/Patterns/3_Behavioral/Patterns)
Communication patterns defining how objects interact and distribute responsibility.
- Observer, Strategy, Command, Chain of Responsibility, Mediator
- Memento, State, Template Method, Visitor, Iterator

### [4. Architectural Patterns](Audabit.Service.SoftwarePatterns.App/Patterns/4_Architectural/Patterns)
High-level system organization and structure patterns.
- Layered, Hexagonal, Microservices, Event-Driven, CQRS
- Event Sourcing, Saga, API Gateway, Strangler Fig

### [5. Messaging Patterns](Audabit.Service.SoftwarePatterns.App/Patterns/5_Messaging/Patterns)
Enterprise integration patterns for reliable, scalable distributed communication.
- Outbox, Inbox, Request-Reply, Publish-Subscribe, Dead Letter Channel
- Splitter, Aggregator, Message Router, Content-Based Router, Message Translator

### [6. Concurrency Patterns](Audabit.Service.SoftwarePatterns.App/Patterns/6_Concurrency/Patterns)
Patterns for thread-safe, parallel processing in multi-threaded applications.
- Async/Await, Producer-Consumer, Parallel Pipeline, Semaphore Throttling, Actor Model

### [7. Reliability Patterns](Audabit.Service.SoftwarePatterns.App/Patterns/7_Reliability/Patterns)
Building resilient systems that handle failures gracefully.
- Retry, Circuit Breaker, Bulkhead, Timeout, Rate Limiter

### [8. Anti-Patterns](Audabit.Service.SoftwarePatterns.App/Patterns/8_AntiPatterns/Patterns)
Common mistakes to avoid with better alternatives.
- God Object, Spaghetti Code, Copy-Paste Programming, Premature Optimization, Reinventing the Wheel

### [9. Testing Practices](Audabit.Service.SoftwarePatterns.App/Testing/9_Testing/TestingPractices)
Practices and methodologies for maintainable, comprehensive testing.
- Test Pyramid, Test Doubles (Mocks, Stubs, Fakes), TDD, BDD, Golden Master

## 🎯 Why Software Patterns Matter

Software design patterns are proven, reusable solutions to commonly occurring problems in software design. They represent best practices evolved over decades by experienced software architects.

- **Common Vocabulary**: Shared language for communicating design decisions across teams
- **Proven Solutions**: Battle-tested approaches that have solved real-world problems at scale
- **Maintainability**: Patterns make code easier to understand, modify, and extend
- **Scalability**: Proper patterns enable systems to handle growth and change
- **Quality**: Following established patterns reduces bugs and technical debt

## 📖 What You'll Find in Each Pattern README

Every pattern category contains a comprehensive README with:

✅ **Problem Statements**: Real problems these patterns solve (not just definitions)
✅ **Solution Approaches**: How the pattern addresses the problem
✅ **When to Use**: Clear guidance on appropriate use cases
✅ **Modern .NET Code**: C# 13 implementations with latest framework features
✅ **Real-World Examples**: Production scenarios where these patterns excel
✅ **Personal Usage**: Actual implementations from enterprise systems with detailed context
✅ **Code Examples**: Complete, runnable code showing the pattern in action
✅ **Best Practices**: Production-proven approaches and alternatives considered
✅ **Pattern Combinations**: How patterns work together to solve complex problems
✅ **Common Pitfalls**: Mistakes to avoid when implementing these patterns

## 🚀 Modern .NET Implementation

All patterns in this repository are implemented using:

- **.NET 10.0**: Latest features and performance improvements
- **C# 13**: Modern language features (primary constructors, collection expressions, file-scoped namespaces)
- **ASP.NET Core**: Web API patterns and middleware

## 📖 How to Use This Repository

1. **Learning**: Each pattern includes detailed explanations, use cases, and modern .NET implementations
2. **Reference**: Use as a quick reference when designing solutions
3. **Templates**: Copy and adapt pattern implementations for your projects
4. **Teaching**: Great resource for teaching design patterns with real .NET examples

## 🏃 Running the Project

```bash
# Clone the repository
git clone https://github.com/JohnnySchaap/Audabit.Service.SoftwarePatterns.git

# Navigate to the project
cd Audabit.Service.SoftwarePatterns

# Build the solution
dotnet build

# Run the API
dotnet run --project Audabit.Service.SoftwarePatterns.WebApi

# Run tests
dotnet test
```

The API will be available at `https://localhost:5001` with Swagger documentation at `https://localhost:5001/swagger`.

## 📋 API Endpoints

Each category is exposed through versioned REST API endpoints:

- **Principles**: `/api/v1/principles`
- **Creational**: `/api/v1/creational-patterns`
- **Structural**: `/api/v1/structural-patterns`
- **Behavioral**: `/api/v1/behavioral-patterns`
- **Architectural**: `/api/v1/architectural-patterns`
- **Messaging**: `/api/v1/messaging-patterns`
- **Concurrency**: `/api/v1/concurrency-patterns`
- **Reliability**: `/api/v1/reliability-patterns`
- **Testing**: `/api/v1/testing-patterns`
- **Anti-Patterns**: `/api/v1/anti-patterns`

--- 

## 📄 License

This project is licensed under the Apache License 2.0 - see the [LICENSE](LICENSE) file for details.

## 👤 Author

**John Schaap**

- GitHub: [@JohnnySchaap](https://github.com/JohnnySchaap)

> **Note**: This is a living repository. Patterns are continuously updated to reflect modern .NET best practices and real-world usage.