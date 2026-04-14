namespace MeddlingIdiot.Dispatcher.UnitTests.Fakes;

public record GetStoreValueQuery(string Key) : IRequest<string?>;

public class GetStoreValueQueryHandler(MockDataStore dataStore) : IRequestHandler<GetStoreValueQuery, string?>
{
    public Task<string?> Handle(GetStoreValueQuery request, CancellationToken cancellationToken = default)
    {
        var value = dataStore.Get(request.Key);
        return Task.FromResult(value);
    }
}