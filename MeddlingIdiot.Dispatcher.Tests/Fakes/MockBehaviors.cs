namespace MeddlingIdiot.Dispatcher.Tests.Fakes;
public class MockLoggingBehavior<TRequest>(MockDataStore dataStore) : IPipelineBehavior<TRequest> where TRequest : IRequest
{
    public readonly List<string> Logs = new();

    public Task<Unit> Handle(TRequest request, RequestHandlerDelegate<Unit> next, CancellationToken cancellationToken = default)
    {
        dataStore.Add("Log", $"Handling Request");
        var result = next(cancellationToken);
        return result;
    }
}

public class MockLoggingBehavior<TRequest, TResponse>(MockDataStore dataStore) : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    public readonly List<string> Logs = new();

    public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken = default)
    {
        dataStore.Add("Log", $"Handling Request");
        var result = next(cancellationToken);
        return result;
    }
}
