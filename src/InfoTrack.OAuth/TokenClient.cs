using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace InfoTrack.OAuth
{
    /// <summary>
    /// OAuth2 Token Client
    /// </summary>
    /// <remarks>
    /// This class is fully thread safe.
    /// </remarks>
    public class TokenClient : ITokenClient
    {
        private static readonly HttpClient HttpClient = new HttpClient();

        /// <summary>
        /// This is the OAuth 2.0 grant that server processes use to access an API.
        /// Use this endpoint to directly request an Access Token by using the Client's credentials (a Client ID and a Client Secret).
        /// </summary>
        /// <param name="tokenEndpoint">The authentication server's access token endpoint</param>
        /// <param name="clientId">Your application's Client ID.</param>
        /// <param name="clientSecret">Your application's Client Secret.</param>
        /// <exception cref="ArgumentException">Thrown for invalid or missing arguments.</exception><exception cref="ArgumentNullException">Thrown for null arguments.</exception><exception cref="AuthenticationException">Thrown for unexpected errors.</exception>
        public Task<TokenResponse> ClientCredentialsGrantAsync(Uri tokenEndpoint, string clientId, string clientSecret)
        {
            if (tokenEndpoint == null) throw new ArgumentNullException(nameof(tokenEndpoint));
            if (string.IsNullOrWhiteSpace(clientId)) throw new ArgumentException("Null or blank argument", nameof(clientId));
            if (string.IsNullOrWhiteSpace(clientSecret)) throw new ArgumentException("Null or blank argument", nameof(clientSecret));

            return ClientCredentialsGrantAsync(tokenEndpoint, clientId, clientSecret, requiredScopes: null);
        }

        /// <summary>
        /// This is the OAuth 2.0 grant that server processes use to access an API.
        /// Use this endpoint to directly request an Access Token by using the Client's credentials (a Client ID and a Client Secret).
        /// </summary>
        /// <param name="tokenEndpoint">The authentication server's access token endpoint</param>
        /// <param name="clientId">Your application's Client ID.</param>
        /// <param name="clientSecret">Your application's Client Secret.</param>
        /// <param name="requiredScopes">Required scopes.</param>
        /// <exception cref="ArgumentException">Thrown for invalid or missing arguments.</exception><exception cref="ArgumentNullException">Thrown for null arguments.</exception><exception cref="AuthenticationException">Thrown for unexpected errors.</exception>
        public Task<TokenResponse> ClientCredentialsGrantAsync(Uri tokenEndpoint, string clientId, string clientSecret, IEnumerable<string> requiredScopes)
        {
            if (tokenEndpoint == null) throw new ArgumentNullException(nameof(tokenEndpoint));
            if (string.IsNullOrWhiteSpace(clientId)) throw new ArgumentException("Null or blank argument", nameof(clientId));
            if (string.IsNullOrWhiteSpace(clientSecret)) throw new ArgumentException("Null or blank argument", nameof(clientSecret));

            return ClientCredentialsGrantInternalAsync(tokenEndpoint, clientId, clientSecret, requiredScopes);
        }

        /// <summary>
        /// This is the OAuth 2.0 grant that highly-trusted apps use to access an API.
        /// In this flow, the end-user is asked to fill in credentials (username/password), typically using an interactive form in the user-agent (browser).
        /// </summary>
        /// <param name="username">Resource Owner's identifier.</param>
        /// <param name="password">Resource Owner's secret.</param>
        /// <exception cref="ArgumentException">Thrown for invalid or missing arguments.</exception><exception cref="ArgumentNullException">Thrown for null arguments.</exception><exception cref="AuthenticationException">Thrown for unexpected errors.</exception>
        public Task<TokenResponse> ResourceOwnerPasswordGrantAsync(
            Uri tokenEndpoint,
            string username,
            string password
            )
        {
            if (tokenEndpoint == null) throw new ArgumentNullException(nameof(tokenEndpoint));
            if (string.IsNullOrWhiteSpace(username)) throw new ArgumentException("Null or blank argument", nameof(username));
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Null or blank argument", nameof(password));

            return ResourceOwnerPasswordGrantInternalAsync(tokenEndpoint, username, password, clientId: null, clientSecret: null, requiredScopes: null, extraParameters: null);
        }

        /// <summary>
        /// This is the OAuth 2.0 grant that highly-trusted apps use to access an API.
        /// In this flow, the end-user is asked to fill in credentials (username/password), typically using an interactive form in the user-agent (browser).
        /// </summary>
        /// <param name="username">Resource Owner's identifier.</param>
        /// <param name="password">Resource Owner's secret.</param>
        /// <param name="clientId">Your application's Client ID.</param>
        /// <param name="clientSecret">Your application's Client Secret.</param>
        /// <exception cref="ArgumentException">Thrown for invalid or missing arguments.</exception><exception cref="ArgumentNullException">Thrown for null arguments.</exception><exception cref="AuthenticationException">Thrown for unexpected errors.</exception>
        public Task<TokenResponse> ResourceOwnerPasswordGrantAsync(
            Uri tokenEndpoint,
            string username,
            string password,
            string clientId,
            string clientSecret
            )
        {
            if (tokenEndpoint == null) throw new ArgumentNullException(nameof(tokenEndpoint));
            if (string.IsNullOrWhiteSpace(username)) throw new ArgumentException("Null or blank argument", nameof(username));
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Null or blank argument", nameof(password));

            return ResourceOwnerPasswordGrantInternalAsync(tokenEndpoint, username, password, clientId, clientSecret, requiredScopes: null, extraParameters: null);
        }

        /// <summary>
        /// This is the OAuth 2.0 grant that highly-trusted apps use to access an API.
        /// In this flow, the end-user is asked to fill in credentials (username/password), typically using an interactive form in the user-agent (browser).
        /// </summary>
        /// <param name="username">Resource Owner's identifier.</param>
        /// <param name="password">Resource Owner's secret.</param>
        /// <param name="clientId">Your application's Client ID.</param>
        /// <param name="clientSecret">Your application's Client Secret.</param>
        /// <param name="requiredScopes">The scopes the application is asking for.</param>
        /// <exception cref="ArgumentException">Thrown for invalid or missing arguments.</exception><exception cref="ArgumentNullException">Thrown for null arguments.</exception><exception cref="AuthenticationException">Thrown for unexpected errors.</exception>
        public Task<TokenResponse> ResourceOwnerPasswordGrantAsync(
            Uri tokenEndpoint,
            string username,
            string password,
            string clientId,
            string clientSecret,
            IEnumerable<string> requiredScopes
            )
        {
            if (tokenEndpoint == null) throw new ArgumentNullException(nameof(tokenEndpoint));
            if (string.IsNullOrWhiteSpace(username)) throw new ArgumentException("Null or blank argument", nameof(username));
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Null or blank argument", nameof(password));

            return ResourceOwnerPasswordGrantInternalAsync(tokenEndpoint, username, password, clientId, clientSecret, requiredScopes, extraParameters: null);
        }

        /// <summary>
        /// This is the OAuth 2.0 grant that highly-trusted apps use to access an API.
        /// In this flow, the end-user is asked to fill in credentials (username/password), typically using an interactive form in the user-agent (browser).
        /// </summary>
        /// <param name="username">Resource Owner's identifier.</param>
        /// <param name="password">Resource Owner's secret.</param>
        /// <param name="clientId">Your application's Client ID.</param>
        /// <param name="clientSecret">Your application's Client Secret.</param>
        /// <param name="requiredScopes">The scopes the application is asking for.</param>
        /// <param name="extraParameters">Extra parameters to send with the request.</param>
        /// <exception cref="ArgumentException">Thrown for invalid or missing arguments.</exception><exception cref="ArgumentNullException">Thrown for null arguments.</exception><exception cref="AuthenticationException">Thrown for unexpected errors.</exception>
        public Task<TokenResponse> ResourceOwnerPasswordGrantAsync(
            Uri tokenEndpoint,
            string username,
            string password,
            string clientId,
            string clientSecret,
            IEnumerable<string> requiredScopes,
            IDictionary<string, string> extraParameters
            )
        {
            if (tokenEndpoint == null) throw new ArgumentNullException(nameof(tokenEndpoint));
            if (string.IsNullOrWhiteSpace(username)) throw new ArgumentException("Null or blank argument", nameof(username));
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Null or blank argument", nameof(password));

            return ResourceOwnerPasswordGrantInternalAsync(tokenEndpoint, username, password, clientId, clientSecret, requiredScopes, extraParameters);
        }

        /// <summary>
        /// This is the OAuth2.0 grant type used to exchange a refresh token for an access token.
        /// This allows clients to continue to have a valid access token without further interaction with the user.
        /// </summary>
        /// <param name="tokenEndpoint"></param>
        /// <param name="refreshToken"></param>
        /// <param name="clientId"></param>
        /// <param name="clientSecret"></param>
        /// <returns></returns>
        public Task<TokenResponse> RefreshTokenGrantAsync(Uri tokenEndpoint, string refreshToken, string clientId, string clientSecret)
        {
            if (tokenEndpoint == null) throw new ArgumentNullException(nameof(tokenEndpoint));
            if (string.IsNullOrWhiteSpace(clientId)) throw new ArgumentException("Null or blank argument", nameof(clientId));
            if (string.IsNullOrWhiteSpace(clientSecret)) throw new ArgumentException("Null or blank argument", nameof(clientSecret));

            return RefreshTokenGrantInternalAsync(tokenEndpoint, refreshToken, clientId, clientSecret);
        }

        private async Task<TokenResponse> RefreshTokenGrantInternalAsync(Uri tokenEndpoint, string refreshToken, string clientId, string clientSecret)
        {
            var request = new Dictionary<string, string>
            {
                { "grant_type", Constants.GrantTypes.RefreshToken },
                { "refresh_token", refreshToken },
                { "client_id", clientId },
                { "client_secret", clientSecret }
            };

            return await Request<TokenResponse>(tokenEndpoint, request).ConfigureAwait(false);
        }

        private async Task<TokenResponse> ClientCredentialsGrantInternalAsync(Uri tokenEndpoint, string clientId, string clientSecret, IEnumerable<string> requiredScopes)
        {
            var request = new Dictionary<string, string>
            {
                { "grant_type", Constants.GrantTypes.ClientCredentials },
                { "client_id", clientId },
                { "client_secret", clientSecret }
            };

            if ((requiredScopes ?? Enumerable.Empty<string>()).Any())
            {
                request.Add("scope", string.Join(" ", requiredScopes));
            }

            return await Request<TokenResponse>(tokenEndpoint, request).ConfigureAwait(false);
        }

        private async Task<TokenResponse> ResourceOwnerPasswordGrantInternalAsync(
            Uri tokenEndpoint,
            string username,
            string password,
            string clientId,
            string clientSecret,
            IEnumerable<string> requiredScopes,
            IDictionary<string, string> extraParameters
            )
        {
            var request = new Dictionary<string, string>
            {
                { "grant_type", Constants.GrantTypes.Password },
                { "client_id", clientId },
                { "client_secret", clientSecret },
                { "username", username },
                { "password", password }
            }
            .Concat(extraParameters ?? new Dictionary<string, string>())
            .Where(kvp => kvp.Value != null)
            .ToDictionary(i => i.Key, i => i.Value);

            if ((requiredScopes ?? Enumerable.Empty<string>()).Any())
            {
                request.Add("scope", string.Join(" ", requiredScopes));
            }

            var response = await Request<TokenResponse>(tokenEndpoint, request).ConfigureAwait(false);

            return response;
        }

        private async Task<T> Request<T>(Uri endpoint, IDictionary<string, string> requestParameters, string accessToken = null)
        {
            var postContent = new FormUrlEncodedContent(requestParameters);

            var request = new HttpRequestMessage(HttpMethod.Post, endpoint)
            {
                Content = postContent,
            };

            if (!string.IsNullOrWhiteSpace(accessToken))
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            HttpResponseMessage tokenResponse = await HttpClient.SendAsync(request).ConfigureAwait(false);

            var responseBody = await tokenResponse.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (!tokenResponse.IsSuccessStatusCode && tokenResponse.StatusCode != HttpStatusCode.BadRequest)
            {
                throw new AuthenticationException($"Non-success status code: {tokenResponse.StatusCode}. Response body: {responseBody}");
            }

            return JsonConvert.DeserializeObject<T>(responseBody);
        }
    }
}
