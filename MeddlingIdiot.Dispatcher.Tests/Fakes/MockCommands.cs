namespace MeddlingIdiot.Dispatcher.Tests.Fakes;

public record AddStoreValueCommand(string Key, string Value) : IRequest;

public class AddStoreValueCommandHandler(MockDataStore dataStore) : IRequestHandler<AddStoreValueCommand>
{
    public Task Handle(AddStoreValueCommand request, CancellationToken cancellationToken = default)
    {
        dataStore.Add(request.Key, request.Value);
        return Task.FromResult(Unit.Value);
    }
}