using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace PageBoostAI.Infrastructure.Caching;

public interface ICacheService
{
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);
    Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken cancellationToken = default);
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default);
}

public class RedisCacheService : ICacheService, IDisposable
{
    private readonly ConnectionMultiplexer _connection;
    private readonly IDatabase _database;
    private readonly ILogger<RedisCacheService> _logger;
    private static readonly TimeSpan DefaultExpiry = TimeSpan.FromMinutes(30);

    public RedisCacheService(IConfiguration configuration, ILogger<RedisCacheService> logger)
    {
        _logger = logger;
        var connectionString = configuration["REDIS_URL"] ?? "localhost:6379";

        // Convert redis:// URL format to StackExchange.Redis format
        if (connectionString.StartsWith("redis://"))
        {
            var uri = new Uri(connectionString);
            connectionString = $"{uri.Host}:{uri.Port}";
        }

        _connection = ConnectionMultiplexer.Connect(connectionString);
        _database = _connection.GetDatabase();
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var value = await _database.StringGetAsync(key);
        if (value.IsNullOrEmpty)
            return default;

        _logger.LogDebug("Cache hit for key: {Key}", key);
        return JsonSerializer.Deserialize<T>(value!);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken cancellationToken = default)
    {
        var serialized = JsonSerializer.Serialize(value);
        await _database.StringSetAsync(key, serialized, expiry ?? DefaultExpiry);
        _logger.LogDebug("Cached key: {Key}, expiry: {Expiry}", key, expiry ?? DefaultExpiry);
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        await _database.KeyDeleteAsync(key);
        _logger.LogDebug("Removed cache key: {Key}", key);
    }

    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        return await _database.KeyExistsAsync(key);
    }

    public void Dispose()
    {
        _connection.Dispose();
    }
}
