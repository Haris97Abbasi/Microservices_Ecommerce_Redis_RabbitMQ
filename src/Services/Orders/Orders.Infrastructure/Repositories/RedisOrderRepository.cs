using System.Text.Json;
using System.Text.Json.Serialization;
using Orders.Core.Repositories;
using StackExchange.Redis;
using Order = Orders.Core.Entities.Order;

namespace Orders.Infrastructure.Repositories;

public class RedisOrderRepository(IConnectionMultiplexer redis) : IOrderRepository
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    private static RedisKey Key(Guid id) => $"order:{id}";

    public async Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var json = await redis.GetDatabase().StringGetAsync(Key(id));
        return json.IsNullOrEmpty ? null : JsonSerializer.Deserialize<Order>(json.ToString(), JsonOptions);
    }

    public Task SaveAsync(Order order, CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(order, JsonOptions);
        return redis.GetDatabase().StringSetAsync(Key(order.Id), json);
    }
}
