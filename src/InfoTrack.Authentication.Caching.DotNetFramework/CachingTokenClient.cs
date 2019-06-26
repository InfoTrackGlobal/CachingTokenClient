using System;
using System.Runtime.Caching;
using System.Threading;
using System.Threading.Tasks;

namespace InfoTrack.Authentication.Caching.DotNetFramework
{
    public class CachingTokenClient : BaseCachingTokenClient
    {
        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private readonly ObjectCache _cache;

        public CachingTokenClient()
            : this(MemoryCache.Default) { }

        public CachingTokenClient(ObjectCache cache)
            : base(ClientOptions.Default)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        public CachingTokenClient(ObjectCache cache, ClientOptions clientOptions)
            : base(clientOptions)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        protected override async Task<TItem> GetOrCreateAsync<TItem>(string key, Func<CacheItem, Task<TItem>> factory)
        {
            var item = _cache.Get(key);

            if (item != null)
            {
                return (TItem)item;
            }

            await _semaphore.WaitAsync();

            try
            {
                item = _cache.Get(key);

                if (item != null)
                {
                    return (TItem)item;
                }

                var cacheItem = new CacheItem();
                item = await factory(cacheItem);

                _cache.Set(key, item, new CacheItemPolicy() { AbsoluteExpiration = cacheItem.AbsoluteExpiration });

                return (TItem)item;
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}
