# Strategy Pattern Refactoring - Generic Implementation

## Overview

The Strategy Pattern has been refactored from a **generic, type-safe approach** to a **runtime-based, non-generic approach** similar to the implementation shown in the reference images. This provides more flexibility and follows the classic Strategy Pattern more closely.

## Key Changes

### Before (Generic Strategy)

```csharp
// Generic strategy interface
public interface IRecruitmentSpecificRulesStrategy<in TCommand> 
    where TCommand : RecruitmentBaseCommand
{
    Task<RecruitmentResult> ApplySpecificRules(TCommand command, RecruitmentResult baseResult);
}

// Multiple handlers - one per command type
public class RecruitmentProcessACommandHandler 
    : BaseRecruitmentHandler<RecruitmentProcessACommand>
{
    private readonly IRecruitmentSpecificRulesStrategy<RecruitmentProcessACommand> _strategy;
    // ...
}
```

### After (Non-Generic Strategy)

```csharp
// Non-generic strategy interface with runtime selection
public interface IRecruitmentSpecificRulesStrategy
{
    bool CanHandle(RecruitmentBaseCommand command);
    Task<RecruitmentResult> ApplySpecificRules(RecruitmentBaseCommand command, RecruitmentResult baseResult);
}

// Single generic handler - routes to appropriate strategy
public class RecruitmentCommandHandler : IRequestHandler<RecruitmentBaseCommand, RecruitmentResult>
{
    private readonly IEnumerable<IRecruitmentSpecificRulesStrategy> _strategies;
    
    public async Task<RecruitmentResult> Handle(RecruitmentBaseCommand request, CancellationToken ct)
    {
        var baseResult = await ApplyCommonRecruitmentLogic(request);
        var strategy = _strategies.FirstOrDefault(s => s.CanHandle(request));
        return await strategy.ApplySpecificRules(request, baseResult);
    }
}
```

## Architecture Benefits

### 1. Single Entry Point
- **One handler** (`RecruitmentCommandHandler`) processes all recruitment commands
- Eliminates code duplication across multiple handlers
- Centralizes the orchestration logic (90% common + 10% specific)

### 2. Runtime Strategy Selection
- Strategies are selected at runtime based on `ProcessType`
- More flexible than compile-time generic constraints
- Easy to add new strategies without modifying the handler

### 3. Simpler Dependency Injection
```csharp
// Register all strategies as a collection
services.AddScoped<IRecruitmentSpecificRulesStrategy, ProcessAStrategy>();
services.AddScoped<IRecruitmentSpecificRulesStrategy, ProcessBStrategy>();

// Single handler registration
services.AddScoped<IRequestHandler<RecruitmentBaseCommand, RecruitmentResult>, RecruitmentCommandHandler>();
```

### 4. Classic Strategy Pattern
- Follows the Gang of Four Strategy Pattern more closely
- Strategy interface defines behavior contract
- Concrete strategies implement `CanHandle()` for runtime selection

## Implementation Details

### Command Type Identification

Each command now has a `ProcessType` property:

```csharp
public abstract class RecruitmentBaseCommand : IRequest<RecruitmentResult>
{
    public abstract string ProcessType { get; }
    // ... other properties
}

public class RecruitmentProcessACommand : RecruitmentBaseCommand
{
    public override string ProcessType => "A";
}

public class RecruitmentProcessBCommand : RecruitmentBaseCommand
{
    public override string ProcessType => "B";
}
```

### Strategy Selection Logic

```csharp
public class ProcessAStrategy : IRecruitmentSpecificRulesStrategy
{
    public bool CanHandle(RecruitmentBaseCommand command)
    {
        return command.ProcessType == "A";
    }
    
    public async Task<RecruitmentResult> ApplySpecificRules(
        RecruitmentBaseCommand command, 
        RecruitmentResult baseResult)
    {
        // Cast to specific type
        if (command is not RecruitmentProcessACommand processACommand)
        {
            throw new InvalidOperationException($"Expected {nameof(RecruitmentProcessACommand)}");
        }
        
        // Apply Process A specific logic...
    }
}
```

## Flow Diagram

```
Client Request (ProcessACommand or ProcessBCommand)
          ↓
    MediatR Mediator
          ↓
RecruitmentCommandHandler (Single Entry Point)
          ↓
  Apply 90% Common Logic
          ↓
Find Strategy (CanHandle check)
          ↓
  Apply 10% Specific Logic (Strategy Pattern)
          ↓
    Return Result
```

## Trade-offs

### Advantages ✅
- **Single handler** instead of multiple handlers
- **Runtime flexibility** - easy to add new process types
- **Less boilerplate** - no need for multiple handler classes
- **Classic pattern** - follows traditional Strategy Pattern
- **Cleaner DI** - strategies registered as collection

### Considerations ⚠️
- **Runtime type checking** - uses pattern matching instead of compile-time safety
- **Slightly more runtime overhead** - strategy lookup via `CanHandle()`
- **Potential for missing strategy** - handled with exception if no match found

## Test Coverage

All 11 tests pass successfully:
- ✅ Process A Strategy - Technical assessment (3 tests)
- ✅ Process B Strategy - Cultural fit (3 tests)
- ✅ Integration - End-to-end flows (3 tests)
- ✅ Strategy Selection - CanHandle validation (2 tests)

## Adding New Process Types

To add a new process (e.g., Process C):

1. Create command with `ProcessType`:
```csharp
public class RecruitmentProcessCCommand : RecruitmentBaseCommand
{
    public override string ProcessType => "C";
    // Add specific fields...
}
```

2. Create strategy with `CanHandle`:
```csharp
public class ProcessCStrategy : IRecruitmentSpecificRulesStrategy
{
    public bool CanHandle(RecruitmentBaseCommand command) => command.ProcessType == "C";
    
    public async Task<RecruitmentResult> ApplySpecificRules(...)
    {
        // Apply Process C logic...
    }
}
```

3. Register strategy:
```csharp
services.AddScoped<IRecruitmentSpecificRulesStrategy, ProcessCStrategy>();
```

**No handler changes needed!** The existing `RecruitmentCommandHandler` will automatically route to the new strategy.

## Conclusion

This refactoring demonstrates a **more flexible and maintainable** approach to the Strategy Pattern while maintaining the **90/10 code reusability** goal. The single generic handler simplifies the codebase and makes it easier to extend with new recruitment processes.

---

**Status: ✅ Refactoring Complete**
- Build: ✅ Successful
- Tests: ✅ 11/11 Passing  
- Demo: ✅ Working
