# Automation.Dispatcher

## Latest Release

**Version:** 2.0.1
**Released:** 2025-11-24
**Coverage:** % line / % branch
**Build:** [2.0.1](https://dev.azure.com/AFTR/Automation/_build/results?buildId=699)

A lightweight **mediator-style dispatching library** for .NET 8 applications.  
It provides a simple way to send commands and queries to their respective handlers with support for pipeline behaviors (cross-cutting concerns such as logging, validation, etc.).

---

## ✨ Features

## Latest Release

**Version:** 2.0.1
**Released:** 2025-11-24
**Coverage:** % line / % branch
**Build:** [2.0.1](https://dev.azure.com/AFTR/Automation/_build/results?buildId=699)
- **Request/Response pattern** with `IRequest<TResponse>` and `IRequest` (for commands without return values).
- **Request handlers** via `IRequestHandler<TRequest, TResponse>` or `IRequestHandler<TRequest>`.
- **Pipeline behaviors** (`IPipelineBehavior`) for cross-cutting logic such as logging or validation.
- **Dependency injection support** through `IServiceCollection` extensions.

---

## 🚀 Getting Started

## Latest Release

**Version:** 2.0.1
**Released:** 2025-11-24
**Coverage:** % line / % branch
**Build:** [2.0.1](https://dev.azure.com/AFTR/Automation/_build/results?buildId=699)

### 1️⃣ Configure Services

## Latest Release

**Version:** 2.0.1
**Released:** 2025-11-24
**Coverage:** % line / % branch
**Build:** [2.0.1](https://dev.azure.com/AFTR/Automation/_build/results?buildId=699)
Register the dispatcher and scan assemblies for request handlers:

```csharp
services.AddDispatcher(typeof(Program).Assembly);
```

Optional: Add open generic pipeline behaviors:
```csharp
services.AddOpenBehavior(typeof(MyLoggingBehavior<>));
services.AddOpenBehavior(typeof(MyLoggingBehavior<,>));
```

### 2️⃣ Define Requests and Handlers

## Latest Release

**Version:** 2.0.1
**Released:** 2025-11-24
**Coverage:** % line / % branch
**Build:** [2.0.1](https://dev.azure.com/AFTR/Automation/_build/results?buildId=699)
Create a command or query:
```csharp
public record AddStoreValueCommand(string Key, string Value) : IRequest;
```

Create a handler:
```csharp
public class AddStoreValueCommandHandler : IRequestHandler<AddStoreValueCommand>
{
    public Task Handle(AddStoreValueCommand request, CancellationToken cancellationToken = default)
    {
        // Handle logic here
        return Task.FromResult(Unit.Value);
    }
}
```

### 3️⃣ Send Requests

## Latest Release

**Version:** 2.0.1
**Released:** 2025-11-24
**Coverage:** % line / % branch
**Build:** [2.0.1](https://dev.azure.com/AFTR/Automation/_build/results?buildId=699)
Inject `IDispatcher` or `ISender` and call:
```csharp
await dispatcher.Send(new AddStoreValueCommand("key", "value"));
```

For queries:
```csharp
string? result = await dispatcher.Send(new GetStoreValueQuery("key"));
```

---

## 🛠 Pipeline Behaviors

## Latest Release

**Version:** 2.0.1
**Released:** 2025-11-24
**Coverage:** % line / % branch
**Build:** [2.0.1](https://dev.azure.com/AFTR/Automation/_build/results?buildId=699)
Pipeline behaviors allow you to run cross-cutting logic:

```csharp
public class LoggingBehavior<TRequest> : IPipelineBehavior<TRequest>
    where TRequest : IRequest
{
    public async Task<Unit> Handle(TRequest request, RequestHandlerDelegate<Unit> next, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Handling {typeof(TRequest).Name}");
        return await next(cancellationToken);
    }
}
```

Register with:
```csharp
services.AddOpenBehavior(typeof(LoggingBehavior<>));
```

---

## 🧪 Build & Test

## Latest Release

**Version:** 2.0.1
**Released:** 2025-11-24
**Coverage:** % line / % branch
**Build:** [2.0.1](https://dev.azure.com/AFTR/Automation/_build/results?buildId=699)
Build:
```bash
dotnet build
```

Run tests:
```bash
dotnet test
```

Tests show how to use requests, handlers, and pipeline behaviors.

---

## Changelog

### [2.0.1] - 2025-11-24

**Package:** Automation.Dispatcher
**Coverage:** % line / % branch

#### Other Changes
- refactor(project): update nuspec handling and dependencies
- Add .nuspec file and update project for NuGet packaging
