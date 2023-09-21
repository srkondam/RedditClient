using Microsoft.Extensions.Configuration;
using Reddit.Data;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Reddit.Services
{

    public interface IRedditAccessTokenProvider
    {
        Task<string> GetAccessTokenAsync(CancellationToken cancellationToken);
    }

    public class RedditAccessTokenProvider : IRedditAccessTokenProvider
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        private AccessToken? _accessToken;

        public RedditAccessTokenProvider(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken)
        {
            // TODO: check expiration and refresh token
            var accessToken = _accessToken;
            if (accessToken == null)
            {
                accessToken = await GenerateTokenAsync(cancellationToken).ConfigureAwait(false);
                _accessToken = accessToken;
            }

            return accessToken.Token;
        }

        /// <summary>
        /// Get access token for reddit api.
        /// 
        /// </summary>
        /// <remarks>Tokens are good for a day. RefreshToken is phase 2. skip for now. May be we can move below snippet to an extension so we can reuse it for refresh token.</remarks>
        private async Task<AccessToken> GenerateTokenAsync(CancellationToken cancellationToken = default)
        {
            var httpClient = _httpClientFactory.CreateClient("Reddit");

            var uri = new Uri(_configuration.GetValue<string>("reddit_token_endpoint") ?? string.Empty);
            var body = new Dictionary<string, string>
            {
                { "grant_type", "password"},
                { "username",_configuration.GetValue<string>("reddit_username") ?? String.Empty},
                { "password", _configuration.GetValue<string>("reddit_password") ?? String.Empty}
            };
            var authenticationString = $"{_configuration.GetValue<string>("reddit_clientID")}:{_configuration.GetValue<string>("reddit_clientSecret")}";
            var base64String = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(authenticationString));

            var request = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                Content = new FormUrlEncodedContent(body)
            };

            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", base64String);

            using var responseMessage = await httpClient.SendAsync(request, cancellationToken);
            using var rs = await responseMessage.Content.ReadAsStreamAsync();
            try
            {
                responseMessage.EnsureSuccessStatusCode();
                var token = await JsonSerializer.DeserializeAsync<AccessToken>(rs);
                if (token == null)
                {
                    throw new Exception("Something went wrong with token request.");
                }

                return token;

            }
            catch (HttpRequestException hre)
            {
                var tokenError = await JsonSerializer.DeserializeAsync<RedditApiError>(rs);
                throw new Exception(@$"Failed to get access token. Details: {tokenError?.ErrorMessage}");
            }
            catch (Exception e)
            {
                throw;
            }
        }

    }
}
