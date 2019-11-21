using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InfoTrack.OAuth
{
    public interface ITokenClient
    {
        Task<TokenResponse> ClientCredentialsGrantAsync(Uri tokenEndpoint, string clientId, string clientSecret);
        Task<TokenResponse> ClientCredentialsGrantAsync(Uri tokenEndpoint, string clientId, string clientSecret, IEnumerable<string> requiredScopes);
        Task<TokenResponse> ResourceOwnerPasswordGrantAsync(Uri tokenEndpoint, string username, string password);
        Task<TokenResponse> ResourceOwnerPasswordGrantAsync(Uri tokenEndpoint, string username, string password, string clientId, string clientSecret);
        Task<TokenResponse> ResourceOwnerPasswordGrantAsync(Uri tokenEndpoint, string username, string password, string clientId, string clientSecret, IEnumerable<string> requiredScopes);
        Task<TokenResponse> ResourceOwnerPasswordGrantAsync(Uri tokenEndpoint, string username, string password, string clientId, string clientSecret, IEnumerable<string> requiredScopes, IDictionary<string, string> extraParameters);
        Task<TokenResponse> RefreshTokenGrantAsync(Uri tokenEndpoint, string refreshToken, string clientId, string clientSecret);
    }
}