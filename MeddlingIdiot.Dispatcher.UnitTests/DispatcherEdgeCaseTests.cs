using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace MeddlingIdiot.Dispatcher.UnitTests;

public class DispatcherEdgeCaseTests
{
    private readonly IServiceCollection _services = new ServiceCollection();

    public DispatcherEdgeCaseTests()
    {
        _services.AddScoped<IDispatcher, Dispatcher>();
    }

    [Test]
    public async Task Send_NullVoidRequest_ThrowsArgumentNullException()
    {
        var provider = _services.BuildServiceProvider();
        var dispatcher = provider.GetRequiredService<IDispatcher>();

        IRequest? nullRequest = null;
        var act = () => dispatcher.Send(nullRequest!);
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Test]
    public async Task Send_NullRequestWithResponse_ThrowsArgumentNullException()
    {
        var provider = _services.BuildServiceProvider();
        var dispatcher = provider.GetRequiredService<IDispatcher>();

        IRequest<string>? nullRequest = null;
        var act = () => dispatcher.Send(nullRequest!);
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Test]
    public async Task Send_RequestWithResponse_PassesCancellationTokenToHandler()
    {
        using var cts = new CancellationTokenSource();
        var handler = new TokenCapturingHandler();

        _services.AddTransient<IRequestHandler<TokenCapturingQuery, string>>(_ => handler);

        var provider = _services.BuildServiceProvider();
        var dispatcher = provider.GetRequiredService<IDispatcher>();

        await dispatcher.Send(new TokenCapturingQuery(), cts.Token);

        handler.CapturedToken.Should().Be(cts.Token);
    }

    [Test]
    public async Task Send_VoidRequest_PassesCancellationTokenToHandler()
    {
        using var cts = new CancellationTokenSource();
        var handler = new TokenCapturingCommandHandler();

        _services.AddTransient<IRequestHandler<TokenCapturingCommand>>(_ => handler);

        var provider = _services.BuildServiceProvider();
        var dispatcher = provider.GetRequiredService<IDispatcher>();

        await dispatcher.Send(new TokenCapturingCommand(), cts.Token);

        handler.CapturedToken.Should().Be(cts.Token);
    }

    [Test]
    public async Task Send_RequestWithResponse_MultipleBehaviorsExecuteInOrder()
    {
        var executionOrder = new List<string>();
        var request = new OrderedQuery();

        _services.AddTransient<IRequestHandler<OrderedQuery, string>>(_ =>
            new OrderedQueryHandler(executionOrder));
        _services.AddTransient<IPipelineBehavior<OrderedQuery, string>>(_ =>
            new OrderedBehavior<OrderedQuery, string>(executionOrder, "Behavior1"));
        _services.AddTransient<IPipelineBehavior<OrderedQuery, string>>(_ =>
            new OrderedBehavior<OrderedQuery, string>(executionOrder, "Behavior2"));

        var provider = _services.BuildServiceProvider();
        var dispatcher = provider.GetRequiredService<IDispatcher>();

        await dispatcher.Send(request);

        executionOrder.Should().Equal("Behavior1", "Behavior2", "Handler");
    }

    [Test]
    public async Task Send_VoidRequest_MultipleBehaviorsExecuteInOrder()
    {
        var executionOrder = new List<string>();
        var request = new OrderedCommand();

        _services.AddTransient<IRequestHandler<OrderedCommand>>(_ =>
            new OrderedCommandHandler(executionOrder));
        _services.AddTransient<IPipelineBehavior<OrderedCommand>>(_ =>
            new OrderedVoidBehavior<OrderedCommand>(executionOrder, "Behavior1"));
        _services.AddTransient<IPipelineBehavior<OrderedCommand>>(_ =>
            new OrderedVoidBehavior<OrderedCommand>(executionOrder, "Behavior2"));

        var provider = _services.BuildServiceProvider();
        var dispatcher = provider.GetRequiredService<IDispatcher>();

        await dispatcher.Send(request);

        executionOrder.Should().Equal("Behavior1", "Behavior2", "Handler");
    }
}

public sealed record TokenCapturingQuery : IRequest<string>;
public sealed record TokenCapturingCommand : IRequest;
public sealed record OrderedQuery : IRequest<string>;
public sealed record OrderedCommand : IRequest;

public class TokenCapturingHandler : IRequestHandler<TokenCapturingQuery, string>
{
    public CancellationToken CapturedToken { get; private set; }

    public Task<string> Handle(TokenCapturingQuery request, CancellationToken cancellationToken = default)
    {
        CapturedToken = cancellationToken;
        return System.Threading.Tasks.Task.FromResult("captured");
    }
}

public class TokenCapturingCommandHandler : IRequestHandler<TokenCapturingCommand>
{
    public CancellationToken CapturedToken { get; private set; }

    public Task Handle(TokenCapturingCommand request, CancellationToken cancellationToken = default)
    {
        CapturedToken = cancellationToken;
        return System.Threading.Tasks.Task.CompletedTask;
    }
}

public class OrderedQueryHandler(List<string> log) : IRequestHandler<OrderedQuery, string>
{
    public Task<string> Handle(OrderedQuery request, CancellationToken cancellationToken = default)
    {
        log.Add("Handler");
        return System.Threading.Tasks.Task.FromResult("done");
    }
}

public class OrderedCommandHandler(List<string> log) : IRequestHandler<OrderedCommand>
{
    public Task Handle(OrderedCommand request, CancellationToken cancellationToken = default)
    {
        log.Add("Handler");
        return System.Threading.Tasks.Task.CompletedTask;
    }
}

public class OrderedBehavior<TRequest, TResponse>(List<string> log, string name) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken = default)
    {
        log.Add(name);
        return await next(cancellationToken);
    }
}

public class OrderedVoidBehavior<TRequest>(List<string> log, string name) : IPipelineBehavior<TRequest>
    where TRequest : IRequest
{
    public async Task<Unit> Handle(TRequest request, RequestHandlerDelegate<Unit> next, CancellationToken cancellationToken = default)
    {
        log.Add(name);
        return await next(cancellationToken);
    }
}
