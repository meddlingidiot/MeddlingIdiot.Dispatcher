using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automation.Dispatcher.Tests.Fakes;
public class MockDataStore
{
    private readonly Dictionary<string, string> _store = new();

    public Dictionary<string, string> Store => _store;

    public void Clear() => _store.Clear();

    public void Add(string key, string value) => _store[key] = value;

    public string? Get(string key) => _store.TryGetValue(key, out var value) ? value : null;
}
