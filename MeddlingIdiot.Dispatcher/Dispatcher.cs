using System.Collections.Concurrent;

namespace MeddlingIdiot.Dispatcher;

public interface IDispatcher : ISender { }

public class Dispatcher(IServiceProvider _serviceProvider) : IDispatcher
{
    private static readonly ConcurrentDictionary<Type, RequestHandlerBase> _requestHandlerCache = new();

    public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var handler = (RequestHandlerWrapper<TResponse>)_requestHandlerCache.GetOrAdd(request.GetType(), static t =>
        {
            var wrapperType = typeof(RequestHandlerWrapperImpl<,>).MakeGenericType(t, typeof(TResponse));
            var wrapper = (RequestHandlerBase)Activator.CreateInstance(wrapperType)! ?? throw new InvalidOperationException($"Could not create wrapper for {t}");
            return (RequestHandlerBase)wrapper;
        });

        return handler.Handle(request, _serviceProvider, cancellationToken);
    }

    public Task Send(IRequest request, CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var handler = (RequestHandlerWrapper)_requestHandlerCache.GetOrAdd(request.GetType(), static t =>
        {
            var wrapperType = typeof(RequestHandlerWrapperImpl<>).MakeGenericType(t);
            var wrapper = (RequestHandlerBase)Activator.CreateInstance(wrapperType)! ?? throw new InvalidOperationException($"Could not create wrapper for {t}");
            return (RequestHandlerBase)wrapper;
        });

        return handler.Handle(request, _serviceProvider, cancellationToken);
    }
}
