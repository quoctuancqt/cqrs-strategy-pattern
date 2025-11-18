# Recruitment Strategy - CQRS + Strategy Pattern Implementation

A production-ready .NET 8 solution demonstrating the **CQRS pattern** combined with the **Strategy Pattern** to achieve 90% code reusability while maintaining 10% process-specific customization.

## 🏗️ Architecture

This solution implements a recruitment processing system with two distinct processes (A and B) that share 90% of their business logic while having 10% unique requirements.

### Key Patterns & Principles

- **CQRS (Command Query Responsibility Segregation)**: Using MediatR for command handling
- **Strategy Pattern**: For process-specific business rules (10% unique logic)
- **DRY (Don't Repeat Yourself)**: 90% shared logic in base handler
- **OCP (Open/Closed Principle)**: Extensible for new recruitment processes
- **SRP (Single Responsibility Principle)**: Each strategy handles only its specific rules

## 📁 Project Structure

```
RecruitmentStrategy/
├── src/
│   ├── RecruitmentStrategy.Core/              # Domain models
│   │   └── Models/
│   │       ├── Candidate.cs
│   │       ├── Position.cs
│   │       └── RecruitmentResult.cs
│   │
│   └── RecruitmentStrategy.Application/        # Application layer
│       ├── Commands/                            # CQRS Commands
│       │   ├── RecruitmentBaseCommand.cs       # 90% shared data
│       │   ├── RecruitmentProcessACommand.cs   # 10% Process A data
│       │   └── RecruitmentProcessBCommand.cs   # 10% Process B data
│       │
│       ├── Handlers/                            # CQRS Handlers
│       │   ├── BaseRecruitmentHandler.cs       # 90% shared logic
│       │   ├── RecruitmentProcessACommandHandler.cs
│       │   └── RecruitmentProcessBCommandHandler.cs
│       │
│       ├── Strategies/                          # Strategy Pattern (10% unique)
│       │   ├── IRecruitmentSpecificRulesStrategy.cs
│       │   ├── ProcessAStrategy.cs             # Process A specific rules
│       │   └── ProcessBStrategy.cs             # Process B specific rules
│       │
│       └── DependencyInjection.cs              # Service registration
│
├── tests/
│   └── RecruitmentStrategy.Tests/
│       ├── Strategies/                          # Unit tests for strategies
│       │   ├── ProcessAStrategyTests.cs
│       │   └── ProcessBStrategyTests.cs
│       │
│       └── Integration/                         # Integration tests
│           └── RecruitmentProcessIntegrationTests.cs
│
├── .editorconfig                                # Code style enforcement
├── Directory.Build.props                        # Centralized build config
├── Directory.Packages.props                     # Central package management
├── docker-compose.yml                           # Local development environment
└── .github/workflows/build.yml                  # CI/CD pipeline
```

## 🚀 Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker](https://www.docker.com/get-started) (optional, for local infrastructure)

### Build and Test

```bash
# Restore dependencies
dotnet restore

# Build the solution
dotnet build

# Run tests
dotnet test

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"
```

### Running Infrastructure

```bash
# Start SQL Server, Redis, and RabbitMQ
docker-compose up -d

# Stop infrastructure
docker-compose down
```

## 📊 Code Reusability Breakdown

### 90% Shared Logic (BaseRecruitmentHandler)
- Candidate validation
- Experience requirement checking
- Skills matching
- Priority level adjustments
- Recruitment channel validation
- Scoring and approval calculation

### 10% Process-Specific Logic

**Process A (Technical Focus):**
- Technical assessment scheduling
- Certification requirements validation
- Interview time preferences

**Process B (Cultural & Team Fit):**
- Cultural fit interview scheduling
- Team size preference handling
- Soft skills emphasis

## 🧪 Testing

The solution includes comprehensive tests:

- **Unit Tests**: Testing individual strategies in isolation
- **Integration Tests**: End-to-end testing of the complete CQRS flow
- **9 test cases** covering various scenarios

All tests use:
- **xUnit** for test framework
- **FluentAssertions** for readable assertions
- **Moq** for mocking dependencies

## 🔧 Code Quality

### Static Analysis
- **SonarAnalyzer.CSharp** integrated for code quality
- **TreatWarningsAsErrors** enabled to enforce clean code
- **EditorConfig** for consistent code style

### Best Practices
- Central package management
- Nullable reference types enabled
- Latest C# language features
- Implicit usings for cleaner code

## 🔄 CI/CD

GitHub Actions workflow automatically:
1. Restores dependencies
2. Builds the solution in Release mode
3. Runs all tests
4. Collects code coverage
5. Uploads test results and coverage reports

## 📝 Usage Example

```csharp
// Register services
services.AddRecruitmentApplication();

// Create a command for Process A
var command = new RecruitmentProcessACommand
{
    Candidate = new Candidate { /* ... */ },
    Position = new Position { /* ... */ },
    RequiresTechnicalAssessment = true,
    CertificationRequirements = new List<string> { "Azure", "AWS" }
};

// Process through MediatR
var result = await mediator.Send(command);

// Check result
if (result.IsApproved)
{
    Console.WriteLine($"Candidate approved with score: {result.Score}");
}
```

## 🎯 Adding a New Process

To add a new recruitment process (e.g., Process C):

1. Create `RecruitmentProcessCCommand` inheriting from `RecruitmentBaseCommand`
2. Create `ProcessCStrategy` implementing `IRecruitmentSpecificRulesStrategy`
3. Create `RecruitmentProcessCCommandHandler` inheriting from `BaseRecruitmentHandler`
4. Register the strategy in `DependencyInjection.cs`
5. Write tests for the new strategy

The 90% shared logic is automatically inherited!

## 📚 Design Decisions

### Why CQRS?
- Clear separation of concerns
- Easy to test handlers independently
- Scalable architecture for complex business logic

### Why Strategy Pattern?
- Adheres to Open/Closed Principle
- Easy to add new processes without modifying existing code
- Testable in isolation

### Why MediatR?
- Industry-standard implementation of CQRS
- Built-in dependency injection
- Minimal boilerplate code

## 🤝 Contributing

1. Follow the EditorConfig settings
2. Ensure all tests pass
3. Maintain code coverage above 80%
4. Add tests for new features

## 📄 License

This is a proof of concept implementation for educational purposes.

---

**Built with ❤️ using .NET 8, CQRS, and Strategy Pattern**
