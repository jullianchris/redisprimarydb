using System.Text.Json;
using RedisAPI.Models;
using StackExchange.Redis;

namespace RedisAPI.Data;

public class RedisPlatformRepo : IPlatformRepo
{
    private readonly IConnectionMultiplexer _redis;

    public RedisPlatformRepo(IConnectionMultiplexer redis)
    {
        _redis = redis;
    }

    public void CreatePlatform(Platform plat)
    {
        if (plat == null)
            throw new ArgumentOutOfRangeException(nameof(plat));

        var db = _redis.GetDatabase();
        var serialPlat = JsonSerializer.Serialize(plat);
        // db.StringSet(plat.Id, serialPlat);
        // db.SetAdd("PlatformSet", serialPlat);

        db.HashSet("hashplatform", new HashEntry[]{
            new HashEntry(plat.Id,serialPlat)
        });
    }

    public IEnumerable<Platform?>? GetAllPlatforms()
    {
        var db = _redis.GetDatabase();
        // var completeSet = db.SetMembers("PlatformSet");
        var compelteHash = db.HashGetAll("hashplatform");

        // if (completeSet.Length > 0)
        //     return Array.ConvertAll(completeSet, val => JsonSerializer.Deserialize<Platform>(val)).ToList();
        if (compelteHash.Length > 0)
            return Array.ConvertAll(compelteHash, val => JsonSerializer.Deserialize<Platform>(val.Value)).ToList();

        return null;
    }

    public Platform? GetPlatformById(string id)
    {
        var db = _redis.GetDatabase();
        //var plat = db.StringGet(id);
        var plat = db.HashGet("hashplatform", id);

        if (!string.IsNullOrEmpty(plat))
            return JsonSerializer.Deserialize<Platform>(plat);

        return null;
    }
}