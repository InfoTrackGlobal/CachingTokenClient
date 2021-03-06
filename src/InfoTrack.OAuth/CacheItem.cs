﻿using System;

namespace InfoTrack.OAuth
{
    /// <summary>
    /// Represents a cache entry.
    /// </summary>
    public class CacheItem
    {
        /// <summary>
        /// Gets or sets an absolute expiration date for the cache entry.
        /// </summary>
        public DateTimeOffset AbsoluteExpiration { get; set; }
    }
}
