# MeddlingIdiot.Dispatcher

A lightweight mediator-style dispatching library for .NET. Provides a simple way to send commands and queries to their respective handlers, with support for pipeline behaviors (logging, validation, etc.).

---

## Features

- **Request/Response pattern** via `IRequest<TResponse>` and `IRequest` (void commands)
- **Request handlers** via `IRequestHandler<TRequest, TResponse>` or `IRequestHandler<TRequest>`
- **Pipeline behaviors** (`IPipelineBehavior`) for cross-cutting concerns
- **Dependency injection** via `IServiceCollection` extensions with assembly scanning
- **Multi-framework support**: .NET 8, 9, 10, .NET Standard 2.0, .NET Framework 4.7.2+

---

## Getting Started

### 1. Configure Services

Register the dispatcher and scan assemblies for handlers:

```csharp
services.AddDispatcher(typeof(Program).Assembly);
```

Optionally register open generic pipeline behaviors:

```csharp
services.AddOpenBehavior(typeof(LoggingBehavior<>));
services.AddOpenBehavior(typeof(LoggingBehavior<,>));
```

### 2. Define Requests and Handlers

**Command (no return value):**

```csharp
public record AddItemCommand(string Key, string Value) : IRequest;

public class AddItemCommandHandler : IRequestHandler<AddItemCommand>
{
    public Task Handle(AddItemCommand request, CancellationToken cancellationToken = default)
    {
        // handle logic
        return Task.FromResult(Unit.Value);
    }
}
```

**Query (with return value):**

```csharp
public record GetItemQuery(string Key) : IRequest<string?>;

public class GetItemQueryHandler : IRequestHandler<GetItemQuery, string?>
{
    public Task<string?> Handle(GetItemQuery request, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(store.Get(request.Key));
    }
}
```

### 3. Send Requests

Inject `IDispatcher` or `ISender`:

```csharp
await dispatcher.Send(new AddItemCommand("key", "value"));

string? result = await dispatcher.Send(new GetItemQuery("key"));
```

---

## Pipeline Behaviors

Behaviors wrap handler execution and run in registration order. Each behavior receives a `next` delegate to pass control down the chain.

**Void command behavior:**

```csharp
public class LoggingBehavior<TRequest> : IPipelineBehavior<TRequest>
    where TRequest : IRequest
{
    public async Task<Unit> Handle(TRequest request, RequestHandlerDelegate<Unit> next, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"Handling {typeof(TRequest).Name}");
        return await next(cancellationToken);
    }
}
```

**Query behavior:**

```csharp
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"Handling {typeof(TRequest).Name}");
        return await next(cancellationToken);
    }
}
```

Register with:

```csharp
services.AddOpenBehavior(typeof(LoggingBehavior<>));
services.AddOpenBehavior(typeof(LoggingBehavior<,>));
```

---

## Build & Test

```bash
dotnet build
dotnet test
```

---

