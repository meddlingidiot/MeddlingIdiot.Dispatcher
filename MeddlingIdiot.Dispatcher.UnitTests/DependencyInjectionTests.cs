using Microsoft.Extensions.DependencyInjection;

namespace MeddlingIdiot.Dispatcher.UnitTests;

public class DependencyInjectionTests
{
    [Test]
    public async Task AddOpenBehavior_NonGenericType_ThrowsArgumentException()
    {
        var services = new ServiceCollection();
        await Assert.That(() => services.AddOpenBehavior(typeof(NonGenericBehaviorType))).Throws<ArgumentException>();
    }

    [Test]
    public async Task AddOpenBehavior_ThreeGenericParameters_ThrowsArgumentException()
    {
        var services = new ServiceCollection();
        await Assert.That(() => services.AddOpenBehavior(typeof(ThreeParamBehaviorType<,,>))).Throws<ArgumentException>();
    }

    [Test]
    public async Task AddOpenBehavior_TwoParameterBehavior_RegistersAsIPipelineBehavior()
    {
        var services = new ServiceCollection();
        services.AddOpenBehavior(typeof(OpenTwoParamBehavior<,>));
        await Assert.That(services.Any(sd => sd.ServiceType == typeof(IPipelineBehavior<,>))).IsTrue();
    }

    [Test]
    public async Task AddOpenBehavior_OneParameterBehavior_RegistersAsIPipelineBehavior()
    {
        var services = new ServiceCollection();
        services.AddOpenBehavior(typeof(OpenOneParamBehavior<>));
        await Assert.That(services.Any(sd => sd.ServiceType == typeof(IPipelineBehavior<>))).IsTrue();
    }

    [Test]
    public async Task AddDispatcher_RegistersIDispatcher()
    {
        var services = new ServiceCollection();
        services.AddDispatcher();
        var provider = services.BuildServiceProvider();
        await Assert.That(provider.GetService<IDispatcher>()).IsNotNull();
    }
}

public class NonGenericBehaviorType { }

public class ThreeParamBehaviorType<T1, T2, T3> { }

public class OpenTwoParamBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken = default)
        => next(cancellationToken);
}

public class OpenOneParamBehavior<TRequest> : IPipelineBehavior<TRequest>
    where TRequest : IRequest
{
    public Task<Unit> Handle(TRequest request, RequestHandlerDelegate<Unit> next, CancellationToken cancellationToken = default)
        => next(cancellationToken);
}
