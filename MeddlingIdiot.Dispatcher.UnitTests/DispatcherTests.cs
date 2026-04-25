using Microsoft.Extensions.DependencyInjection;
using TUnit.Mocks;
using TUnit.Mocks.Arguments;
using TUnit.Mocks.Generated;

[assembly: GenerateMock(typeof(MeddlingIdiot.Dispatcher.IRequestHandler<MeddlingIdiot.Dispatcher.UnitTests.AddStoreValueCommand>))]
[assembly: GenerateMock(typeof(MeddlingIdiot.Dispatcher.IRequestHandler<MeddlingIdiot.Dispatcher.UnitTests.GetStoreValueQuery, string?>))]
[assembly: GenerateMock(typeof(MeddlingIdiot.Dispatcher.IPipelineBehavior<MeddlingIdiot.Dispatcher.UnitTests.AddStoreValueCommand>))]
[assembly: GenerateMock(typeof(MeddlingIdiot.Dispatcher.IPipelineBehavior<MeddlingIdiot.Dispatcher.UnitTests.GetStoreValueQuery, string?>))]

namespace MeddlingIdiot.Dispatcher.UnitTests;

public class DispatcherTests
{
    private readonly IServiceCollection _services = new ServiceCollection();

    public DispatcherTests()
    {
        _services.AddScoped<IDispatcher, Dispatcher>();
    }

    [Test]
    public async Task Send_RequestWithoutResponse_CallsHandler()
    {
        var request = new AddStoreValueCommand("key1", "value1");
        var handlerMock = Mock.Of<IRequestHandler<AddStoreValueCommand>>();
        handlerMock.Handle(request, Arg.Any<CancellationToken>()).Returns();

        _services.AddTransient(_ => handlerMock.Object);

        var provider = _services.BuildServiceProvider();
        var dispatcher = provider.GetRequiredService<IDispatcher>();

        await dispatcher.Send(request);

        handlerMock.Handle(request, Arg.Any<CancellationToken>()).WasCalled(Times.Once);
    }

    [Test]
    public async Task Send_RequestWithResponse_ReturnsExpectedResponse()
    {
        var request = new GetStoreValueQuery("key1");
        var handlerMock = Mock.Of<IRequestHandler<GetStoreValueQuery, string?>>();

        handlerMock.Handle(request, Arg.Any<CancellationToken>()).Returns("value1");

        _services.AddTransient(_ => handlerMock.Object);

        var provider = _services.BuildServiceProvider();
        var dispatcher = provider.GetRequiredService<IDispatcher>();

        var response = await dispatcher.Send(request);

        await Assert.That(response).IsEqualTo("value1");
        handlerMock.Handle(request, Arg.Any<CancellationToken>()).WasCalled(Times.Once);
    }

    [Test]
    public async Task Send_RequestWithoutResponse_AppliesPipelineBehavior()
    {
        var request = new AddStoreValueCommand("key2", "value2");
        var handlerMock = Mock.Of<IRequestHandler<AddStoreValueCommand>>();
        var behaviorMock = Mock.Of<IPipelineBehavior<AddStoreValueCommand>>();

        handlerMock.Handle(request, Arg.Any<CancellationToken>()).Returns();
        behaviorMock
            .Handle(Arg.Any<AddStoreValueCommand>(), Arg.Any<RequestHandlerDelegate<Unit>>(), Arg.Any<CancellationToken>())
            .ReturnsAsync((_, next, cancellationToken) => next(cancellationToken));

        _services.AddTransient(_ => handlerMock.Object);
        _services.AddTransient(_ => behaviorMock.Object);

        var provider = _services.BuildServiceProvider();
        var dispatcher = provider.GetRequiredService<IDispatcher>();

        await dispatcher.Send(request);

        behaviorMock
            .Handle(request, Arg.Any<RequestHandlerDelegate<Unit>>(), Arg.Any<CancellationToken>())
            .WasCalled(Times.Once);
        handlerMock.Handle(request, Arg.Any<CancellationToken>()).WasCalled(Times.Once);
    }

    [Test]
    public async Task Send_RequestWithResponse_AppliesPipelineBehavior()
    {
        var request = new GetStoreValueQuery("key2");
        var handlerMock = Mock.Of<IRequestHandler<GetStoreValueQuery, string?>>();
        var behaviorMock = Mock.Of<IPipelineBehavior<GetStoreValueQuery, string?>>();

        handlerMock.Handle(request, Arg.Any<CancellationToken>()).Returns("value2");
        behaviorMock
            .Handle(Arg.Any<GetStoreValueQuery>(), Arg.Any<RequestHandlerDelegate<string?>>(), Arg.Any<CancellationToken>())
            .ReturnsAsync((_, next, cancellationToken) => next(cancellationToken));

        _services.AddTransient(_ => handlerMock.Object);
        _services.AddTransient(_ => behaviorMock.Object);

        var provider = _services.BuildServiceProvider();
        var dispatcher = provider.GetRequiredService<IDispatcher>();

        var response = await dispatcher.Send(request);

        await Assert.That(response).IsEqualTo("value2");
        behaviorMock
            .Handle(request, Arg.Any<RequestHandlerDelegate<string?>>(), Arg.Any<CancellationToken>())
            .WasCalled(Times.Once);
        handlerMock.Handle(request, Arg.Any<CancellationToken>()).WasCalled(Times.Once);
    }
}

public sealed record AddStoreValueCommand(string Key, string Value) : IRequest;

public sealed record GetStoreValueQuery(string Key) : IRequest<string?>;
