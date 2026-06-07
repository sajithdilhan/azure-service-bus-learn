using System.Collections.Concurrent;
using Shared.Entities;

namespace Orders.Api.Data;

public sealed class InMemoryOrdersDatabase
{
    public ConcurrentDictionary<Guid, Order> Orders { get; } = [];
}
