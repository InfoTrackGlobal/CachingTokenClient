using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace InfoTrack.OAuth.Caching.DotNetStandard
{
    public class CachingTokenClient : BaseCachingTokenClient
    {
        private readonly IMemoryCache _memoryCache;

        public CachingTokenClient(IMemoryCache memoryCache)
            : this(memoryCache, ClientOptions.Default) { }

        public CachingTokenClient(IMemoryCache memoryCache, ClientOptions clientOptions)
            : base(clientOptions)
        {
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        }

        protected override async Task<TItem> GetOrCreateAsync<TItem>(string key, Func<CacheItem, Task<TItem>> factory)
        {
            return await _memoryCache.GetOrCreateAsync(key, async cacheEntry =>
            {
                var cacheItem = new CacheItem();
                var item = await factory(cacheItem);

                cacheEntry.AbsoluteExpiration = cacheItem.AbsoluteExpiration;

                return item;
            });
        }

        protected override void InvalidateToken(string key)
        {
            if (_memoryCache.TryGetValue(key, out var cached) && cached != null)
            {
                _memoryCache.Remove(key);
            }
        }
    }
}