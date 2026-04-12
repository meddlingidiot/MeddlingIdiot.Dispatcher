using FluentAssertions;
using MeddlingIdiot.Dispatcher.Tests.Fakes;
using Microsoft.Extensions.DependencyInjection;

namespace MeddlingIdiot.Dispatcher.Tests;

public class DispatcherTests : IDisposable
{
    private readonly MockDataStore _mockDataStore = new();
    private readonly IServiceCollection _services;

    public DispatcherTests()
    {
        _services = new ServiceCollection();
        _services.AddSingleton(_mockDataStore);
        _services.AddDispatcher(typeof(DispatcherTests).Assembly);
    }

    public void Dispose()
    {
        _mockDataStore.Clear();
    }

    [Fact]
    public async Task Send_RequestWithResponse_ReturnsExpectedResponse()
    {
        // Arrange
        var provider = _services.BuildServiceProvider();
        var dispatcher = provider.GetRequiredService<IDispatcher>();
        var request = new AddStoreValueCommand("key1", "value1");
        // Act
        await dispatcher.Send(request);
        // Assert
        _mockDataStore.Get("key1").Should().Be("value1");
    }

    [Fact]
    public async Task Send_RequestWithoutResponse_CompletesSuccessfully()
    {
        // Arrange
        var provider = _services.BuildServiceProvider();
        var dispatcher = provider.GetRequiredService<IDispatcher>();
        var request = new GetStoreValueQuery("key1");
        // Act
        var response = await dispatcher.Send(request);
        // Assert
        response.Should().BeNull();
    }

    [Fact]
    public async Task Send_RequestWithResponseAndBehavior_AppliesBehavior()
    {
        // Arrange
        _services.AddOpenBehavior(typeof(MockLoggingBehavior<>));
        var provider = _services.BuildServiceProvider();
        var dispatcher = provider.GetRequiredService<IDispatcher>();
        var request = new AddStoreValueCommand("key2", "value2");
        // Act
        await dispatcher.Send(request);
        // Assert
        _mockDataStore.Get("Log").Should().Be("Handling Request");
        _mockDataStore.Get("key2").Should().Be("value2");
    }

    [Fact]
    public async Task Send_RequestWithoutResponseAndBehavior_AppliesBehavior()
    {
        // Arrange
        _services.AddOpenBehavior(typeof(MockLoggingBehavior<,>));
        var provider = _services.BuildServiceProvider();
        var dispatcher = provider.GetRequiredService<IDispatcher>();
        var request = new GetStoreValueQuery("key2");
        // Act
        var response = await dispatcher.Send(request);
        // Assert
        _mockDataStore.Get("Log").Should().Be("Handling Request");
        response.Should().BeNull();
    }
}
