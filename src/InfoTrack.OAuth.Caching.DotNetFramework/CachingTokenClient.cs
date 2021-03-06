﻿using System;
using System.Runtime.Caching;
using System.Threading;
using System.Threading.Tasks;

namespace InfoTrack.OAuth.Caching.DotNetFramework
{
    public class CachingTokenClient : BaseCachingTokenClient
    {
        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private readonly ObjectCache _cache;

        public CachingTokenClient()
            : this(MemoryCache.Default) { }

        public CachingTokenClient(ClientOptions clientOptions)
            : this(MemoryCache.Default, clientOptions) { }

        public CachingTokenClient(ObjectCache cache)
            : this(cache, ClientOptions.Default) { }

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

        protected override void InvalidateToken(string key)
        {
            if (_cache.Get(key) != null)
            {
                _cache.Remove(key);
            }
        }
    }
}
