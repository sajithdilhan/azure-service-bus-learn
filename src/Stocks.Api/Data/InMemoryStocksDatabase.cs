using System.Collections.Concurrent;
using Shared.Entities;

namespace Stocks.Api.Data;

public sealed class InMemoryStocksDatabase
{
    public ConcurrentDictionary<string, Stock> Stocks { get; } = new(StringComparer.OrdinalIgnoreCase);
    public object SyncRoot { get; } = new();
}
