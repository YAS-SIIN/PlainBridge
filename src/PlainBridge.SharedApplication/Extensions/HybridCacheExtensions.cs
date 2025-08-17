using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Hybrid;

namespace PlainBridge.SharedApplication.Extensions;


public static class HybridCacheExtensions
{
    //private static readonly HybridCacheEntryOptions _options = new HybridCacheEntryOptions()
    //{
    //    Flags = HybridCacheEntryFlags.DisableUnderlyingData
    //};

    ///// <summary>
    ///// Returns true if the cache contains an item with a matching key.
    ///// </summary>
    ///// <param name="cache">An instance of <see cref="HybridCache"/></param>
    ///// <param name="key">The name (key) of the item to search for in the cache.</param>
    ///// <returns>True if the item exists already. False if it doesn't.</returns>
    ///// <remarks>Will never add or alter the state of any items in the cache.</remarks>
    //public async static Task<bool> ExistsAsync(this HybridCache cache, string key, CancellationToken cancellation = default)
    //{
    //    (var exists, _) = await TryGetValueAsync<object>(cache, key);
    //    return exists;
    //}

    /// <summary>
    /// Returns true if the cache contains an item with a matching key, along with the value of the matching cache entry.
    /// </summary>
    /// <typeparam name="T">The type of the value of the item in the cache.</typeparam>
    /// <param name="cache">An instance of <see cref="HybridCache"/></param>
    /// <param name="key">The name (key) of the item to search for in the cache.</param>
    /// <returns>A tuple of <see cref="bool"/> and the object (if found) retrieved from the cache.</returns>
    /// <remarks>Will never add or alter the state of any items in the cache.</remarks>
    public async static Task<T?> TryGetValueAsync<T>(this HybridCache cache, string key, CancellationToken cancellationToken = default)
    {
        var exists = true;

        var result = await cache.GetOrCreateAsync(
            key: key,
             static _ => ValueTask.FromResult<T?>(default),
            cancellationToken: cancellationToken);

        return result;
    }
}