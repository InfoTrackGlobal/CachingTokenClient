using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;

namespace InfoTrack.Authentication.Caching.DotNetCore
{
    public class CachingTokenClient : BaseCachingTokenClient
    {
        private readonly IMemoryCache _memoryCache;
        
        public CachingTokenClient(IMemoryCache memoryCache)
            : base(ClientOptions.Default)
        {
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        }

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
    }
}
