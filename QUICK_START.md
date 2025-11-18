# Quick Start Guide

## Running the Demo

```bash
dotnet run --project src/RecruitmentStrategy.Demo/RecruitmentStrategy.Demo.csproj
```

## Running Tests

```bash
# Run all tests
dotnet test

# Run with detailed output
dotnet test --verbosity normal

# Run with code coverage
dotnet test --collect:"XPlat Code Coverage"
```

## Building the Solution

```bash
# Restore dependencies
dotnet restore

# Build
dotnet build

# Build in Release mode
dotnet build --configuration Release
```

## Running Infrastructure

```bash
# Start all services (SQL Server, Redis, RabbitMQ)
docker-compose up -d

# View logs
docker-compose logs -f

# Stop services
docker-compose down

# Stop and remove volumes
docker-compose down -v
```

## Project Structure

```
src/
├── RecruitmentStrategy.Core/              # Domain models
├── RecruitmentStrategy.Application/       # Business logic (CQRS + Strategy)
└── RecruitmentStrategy.Demo/              # Demo console app

tests/
└── RecruitmentStrategy.Tests/             # Unit & Integration tests
```

## Key Files

- `Directory.Build.props` - Centralized build configuration
- `Directory.Packages.props` - Central package version management
- `.editorconfig` - Code style rules
- `docker-compose.yml` - Local development infrastructure
- `.github/workflows/build.yml` - CI/CD pipeline

## Adding a New Recruitment Process

1. Create new command class inheriting from `RecruitmentBaseCommand`:
```csharp
public class RecruitmentProcessCCommand : RecruitmentBaseCommand
{
    // Add 10% unique fields here
}
```

2. Create strategy implementation:
```csharp
public class ProcessCStrategy : IRecruitmentSpecificRulesStrategy<RecruitmentProcessCCommand>
{
    public async Task<RecruitmentResult> ApplySpecificRules(
        RecruitmentProcessCCommand command, 
        RecruitmentResult baseResult)
    {
        // Add 10% unique logic here
        return await Task.FromResult(baseResult);
    }
}
```

3. Create command handler:
```csharp
public class RecruitmentProcessCCommandHandler 
    : BaseRecruitmentHandler<RecruitmentProcessCCommand>, 
      IRequestHandler<RecruitmentProcessCCommand, RecruitmentResult>
{
    private readonly IRecruitmentSpecificRulesStrategy<RecruitmentProcessCCommand> _strategy;

    public RecruitmentProcessCCommandHandler(
        IRecruitmentSpecificRulesStrategy<RecruitmentProcessCCommand> strategy,
        ILogger<RecruitmentProcessCCommandHandler> logger) 
        : base(logger)
    {
        _strategy = strategy;
    }

    public async Task<RecruitmentResult> Handle(
        RecruitmentProcessCCommand request, 
        CancellationToken cancellationToken)
    {
        var baseResult = await ApplyCommonRecruitmentLogic(request);
        var finalResult = await _strategy.ApplySpecificRules(request, baseResult);
        return finalResult;
    }
}
```

4. Register in DI (DependencyInjection.cs):
```csharp
services.AddScoped<IRecruitmentSpecificRulesStrategy<RecruitmentProcessCCommand>, ProcessCStrategy>();
```

Done! The 90% common logic is automatically inherited.

## Code Quality Tools

- **SonarAnalyzer** - Automatic code quality checks
- **TreatWarningsAsErrors** - All warnings treated as build errors
- **EditorConfig** - Consistent code formatting
- **Central Package Management** - Version consistency

## Useful Commands

```bash
# Clean build artifacts
dotnet clean

# List solution projects
dotnet sln list

# Add new project to solution
dotnet sln add path/to/project.csproj

# Run specific test
dotnet test --filter "FullyQualifiedName~ProcessAStrategyTests"

# Watch mode (auto-rebuild on changes)
dotnet watch run --project src/RecruitmentStrategy.Demo

# Format code
dotnet format
```

## Architecture Highlights

**90% Shared Logic:**
- Candidate validation
- Experience requirements
- Skills matching  
- Priority adjustments
- Channel validation

**10% Process-Specific:**
- Process A: Technical assessments, certifications
- Process B: Cultural fit, team preferences

## Getting Help

- Check `README.md` for detailed documentation
- Review `IMPLEMENTATION_SUMMARY.md` for architecture overview
- See `plan.md` for the original implementation plan
- Run tests to see examples: `dotnet test`
- Run demo for usage: `dotnet run --project src/RecruitmentStrategy.Demo`
