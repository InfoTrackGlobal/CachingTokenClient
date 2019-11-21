using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InfoTrack.OAuth
{
    public abstract class BaseCachingTokenClient : ITokenClient
    {
        private readonly TokenClient _tokenClient = new TokenClient();
        private readonly ClientOptions _clientOptions;

        protected BaseCachingTokenClient(ClientOptions clientOptions)
        {
            _clientOptions = clientOptions ?? throw new ArgumentNullException(nameof(clientOptions));
        }

        protected abstract Task<TItem> GetOrCreateAsync<TItem>(string key, Func<CacheItem, Task<TItem>> factory);

        public Task<TokenResponse> ClientCredentialsGrantAsync(Uri tokenEndpoint, string clientId, string clientSecret)
        {
            if (tokenEndpoint == null) throw new ArgumentNullException(nameof(tokenEndpoint));
            if (string.IsNullOrWhiteSpace(clientId)) throw new ArgumentException("Null or blank argument", nameof(clientId));
            if (string.IsNullOrWhiteSpace(clientSecret)) throw new ArgumentException("Null or blank argument", nameof(clientSecret));

            return ClientCredentialsGrantInternalAsync(tokenEndpoint, clientId, clientSecret, requiredScopes: null);
        }

        public Task<TokenResponse> ClientCredentialsGrantAsync(Uri tokenEndpoint, string clientId, string clientSecret, IEnumerable<string> requiredScopes)
        {
            if (tokenEndpoint == null) throw new ArgumentNullException(nameof(tokenEndpoint));
            if (string.IsNullOrWhiteSpace(clientId)) throw new ArgumentException("Null or blank argument", nameof(clientId));
            if (string.IsNullOrWhiteSpace(clientSecret)) throw new ArgumentException("Null or blank argument", nameof(clientSecret));
            if (requiredScopes == null) throw new ArgumentNullException(nameof(requiredScopes));

            return ClientCredentialsGrantInternalAsync(tokenEndpoint, clientId, clientSecret, requiredScopes);
        }

        public Task<TokenResponse> ResourceOwnerPasswordGrantAsync(Uri tokenEndpoint, string username, string password)
        {
            if (tokenEndpoint == null) throw new ArgumentNullException(nameof(tokenEndpoint));
            if (string.IsNullOrWhiteSpace(username)) throw new ArgumentException("Null or blank argument", nameof(username));
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Null or blank argument", nameof(password));

            return ResourceOwnerPasswordGrantInternalAsync(tokenEndpoint, username, password, clientId: null, clientSecret: null, requiredScopes: null, extraParameters: null);
        }

        public Task<TokenResponse> ResourceOwnerPasswordGrantAsync(Uri tokenEndpoint, string username, string password, string clientId, string clientSecret)
        {
            if (tokenEndpoint == null) throw new ArgumentNullException(nameof(tokenEndpoint));
            if (string.IsNullOrWhiteSpace(username)) throw new ArgumentException("Null or blank argument", nameof(username));
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Null or blank argument", nameof(password));
            if (string.IsNullOrWhiteSpace(clientId)) throw new ArgumentException("Null or blank argument", nameof(clientId));
            if (string.IsNullOrWhiteSpace(clientSecret)) throw new ArgumentException("Null or blank argument", nameof(clientSecret));

            return ResourceOwnerPasswordGrantInternalAsync(tokenEndpoint, username, password, clientId, clientSecret, requiredScopes: null, extraParameters: null);
        }

        public Task<TokenResponse> ResourceOwnerPasswordGrantAsync(Uri tokenEndpoint, string username, string password, string clientId, string clientSecret, IEnumerable<string> requiredScopes)
        {
            if (tokenEndpoint == null) throw new ArgumentNullException(nameof(tokenEndpoint));
            if (string.IsNullOrWhiteSpace(username)) throw new ArgumentException("Null or blank argument", nameof(username));
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Null or blank argument", nameof(password));
            if (string.IsNullOrWhiteSpace(clientId)) throw new ArgumentException("Null or blank argument", nameof(clientId));
            if (string.IsNullOrWhiteSpace(clientSecret)) throw new ArgumentException("Null or blank argument", nameof(clientSecret));
            if (requiredScopes == null) throw new ArgumentNullException(nameof(requiredScopes));

            return ResourceOwnerPasswordGrantInternalAsync(tokenEndpoint, username, password, clientId, clientSecret, requiredScopes, extraParameters: null);
        }

        public Task<TokenResponse> ResourceOwnerPasswordGrantAsync(Uri tokenEndpoint, string username, string password, string clientId, string clientSecret, IEnumerable<string> requiredScopes, IDictionary<string, string> extraParameters)
        {
            if (tokenEndpoint == null) throw new ArgumentNullException(nameof(tokenEndpoint));
            if (string.IsNullOrWhiteSpace(username)) throw new ArgumentException("Null or blank argument", nameof(username));
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Null or blank argument", nameof(password));
            if (string.IsNullOrWhiteSpace(clientId)) throw new ArgumentException("Null or blank argument", nameof(clientId));
            if (string.IsNullOrWhiteSpace(clientSecret)) throw new ArgumentException("Null or blank argument", nameof(clientSecret));
            if (requiredScopes == null) throw new ArgumentNullException(nameof(requiredScopes));
            if (extraParameters == null) throw new ArgumentNullException(nameof(extraParameters));

            return ResourceOwnerPasswordGrantInternalAsync(tokenEndpoint, username, password, clientId, clientSecret, requiredScopes, extraParameters);
        }

        public async Task<TokenResponse> RefreshTokenGrantAsync(Uri tokenEndpoint, string refreshToken, string clientId, string clientSecret)
        {
            return await GetOrCreateAsync(GenerateCacheKey(Constants.GrantTypes.RefreshToken, null, null, clientId, clientSecret), async cacheEntry =>
            {
                var tokenResponse = await _tokenClient.RefreshTokenGrantAsync(tokenEndpoint, refreshToken, clientId, clientSecret);

                if (tokenResponse.Error != null)
                {
                    throw new AuthenticationException($"Error: {tokenResponse.Error}. Error description: {tokenResponse.ErrorDescription}");
                }

                cacheEntry.AbsoluteExpiration = DateTime.Now.AddSeconds(tokenResponse.ExpiresIn ?? _clientOptions.DefaultCacheExpiry);

                return tokenResponse;
            });
        }

        private async Task<TokenResponse> ClientCredentialsGrantInternalAsync(Uri tokenEndpoint, string clientId, string clientSecret, IEnumerable<string> requiredScopes)
        {
            return await GetOrCreateAsync(GenerateCacheKey(Constants.GrantTypes.ClientCredentials, null, null, clientId, clientSecret), async cacheEntry =>
            {
                var tokenResponse = await _tokenClient.ClientCredentialsGrantAsync(tokenEndpoint, clientId, clientSecret, requiredScopes);

                if (tokenResponse.Error != null)
                {
                    throw new AuthenticationException($"Error: {tokenResponse.Error}. Error description: {tokenResponse.ErrorDescription}");
                }

                cacheEntry.AbsoluteExpiration = DateTime.Now.AddSeconds(tokenResponse.ExpiresIn ?? _clientOptions.DefaultCacheExpiry);

                return tokenResponse;
            });
        }

        private async Task<TokenResponse> ResourceOwnerPasswordGrantInternalAsync(Uri tokenEndpoint, string username, string password, string clientId, string clientSecret, IEnumerable<string> requiredScopes, IDictionary<string, string> extraParameters)
        {
            return await GetOrCreateAsync(GenerateCacheKey(Constants.GrantTypes.Password, username, password, clientId, clientSecret), async cacheEntry =>
            {
                var tokenResponse = await _tokenClient.ResourceOwnerPasswordGrantAsync(tokenEndpoint, username, password, clientId, clientSecret, requiredScopes, extraParameters);

                if (tokenResponse.Error != null)
                {
                    throw new AuthenticationException($"Error: {tokenResponse.Error}. Error description: {tokenResponse.ErrorDescription}");
                }

                cacheEntry.AbsoluteExpiration = DateTime.Now.AddSeconds(tokenResponse.ExpiresIn ?? _clientOptions.DefaultCacheExpiry);

                return tokenResponse;
            });
        }

        private string GenerateCacheKey(string primaryKey, string username, string password, string clientId, string clientSecret)
        {
            return $"_CachingTokenClient_{primaryKey}_{username}_{password}_{clientId}_{clientSecret}";
        }
    }
}
