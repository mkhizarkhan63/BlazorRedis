using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace BlazorRedis.Helpers
{
    public static class DistributedCacheExtension
    {

        public static async Task SetRecordAsync<T>(this IDistributedCache cache, string recordId, T data,
            TimeSpan? absoluteExpiredTime = null,
            TimeSpan? unusedExpiredTime = null)
        {
            var options = new DistributedCacheEntryOptions();

            options.AbsoluteExpirationRelativeToNow = absoluteExpiredTime ?? TimeSpan.FromSeconds(60);
            options.SlidingExpiration = unusedExpiredTime;

            var jsonData = JsonSerializer.Serialize(data);
            await cache.SetStringAsync(recordId, jsonData, options);
        }

        public static async Task<T> GetRecordAsync<T>(this IDistributedCache cache, string recordId)
        {
            var jsonData = await cache.GetStringAsync(recordId);

            if (jsonData == null)
            {
                // If T is a reference type, return null; if T is a value type, return the default value for T
                return default;
            }

            return JsonSerializer.Deserialize<T>(jsonData);
        }

    }
}
