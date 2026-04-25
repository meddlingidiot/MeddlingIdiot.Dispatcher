using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace MeddlingIdiot.Dispatcher.UnitTests;

public class DependencyInjectionTests
{
    [Test]
    public void AddOpenBehavior_NonGenericType_ThrowsArgumentException()
    {
        var services = new ServiceCollection();
        var act = () => services.AddOpenBehavior(typeof(NonGenericBehaviorType));
        act.Should().Throw<ArgumentException>();
    }

    [Test]
    public void AddOpenBehavior_ThreeGenericParameters_ThrowsArgumentException()
    {
        var services = new ServiceCollection();
        var act = () => services.AddOpenBehavior(typeof(ThreeParamBehaviorType<,,>));
        act.Should().Throw<ArgumentException>();
    }

    [Test]
    public void AddOpenBehavior_TwoParameterBehavior_RegistersAsIPipelineBehavior()
    {
        var services = new ServiceCollection();
        services.AddOpenBehavior(typeof(OpenTwoParamBehavior<,>));
        services.Should().Contain(sd => sd.ServiceType == typeof(IPipelineBehavior<,>));
    }

    [Test]
    public void AddOpenBehavior_OneParameterBehavior_RegistersAsIPipelineBehavior()
    {
        var services = new ServiceCollection();
        services.AddOpenBehavior(typeof(OpenOneParamBehavior<>));
        services.Should().Contain(sd => sd.ServiceType == typeof(IPipelineBehavior<>));
    }

    [Test]
    public void AddDispatcher_RegistersIDispatcher()
    {
        var services = new ServiceCollection();
        services.AddDispatcher();
        var provider = services.BuildServiceProvider();
        provider.GetService<IDispatcher>().Should().NotBeNull();
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
