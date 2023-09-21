using Microsoft.Extensions.Configuration;
using Reddit.Data;
using Reddit.Repositories.Abstractions;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Reddit.Repositories
{
    public class HttpClientRedditRepository : IRedditRepository
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public HttpClientRedditRepository(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }      
        
        private string ResolveSubredditEndpoint(string subredditName)
        {
            var endpoint = _configuration.GetValue<string>("reddit_subreddit_endpoint") ?? string.Empty;
            return endpoint.Replace("{SUBREDDIT_NAME}", subredditName);
        }

        /// <summary>
        /// Asynchronously reads subreddit posts.
        /// </summary>
        /// <param name="uri">Formatted endpoint for reading subreddit posts.</param>
        /// <param name="token">Reddit api access token</param>
        /// <param name="cancellationToken"></param>
        /// <returns>A collection of Reddit.Data.Post.</returns>
        private async Task<Listing> GetPosts(Uri uri, string token, CancellationToken cancellationToken = default)
        {
            var httpClient = _httpClientFactory.CreateClient("Reddit");

            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            using var responseMessage = await httpClient.SendAsync(request, cancellationToken);
            var rs = await responseMessage.Content.ReadAsStreamAsync(cancellationToken);
            try
            {
                responseMessage.EnsureSuccessStatusCode();
                var listing = await JsonSerializer.DeserializeAsync<Listing>(rs, cancellationToken: cancellationToken);
                return listing ?? new Listing();
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

        /// <summary>
        /// Asynchronously get a listing of posts for the subreddit.
        /// </summary>
        /// <remarks>API doesn't throw error if you haven't subscribed for the particular subreddit.</remarks>
        public async Task<Listing> GetNewPostsForSubrredditAsync(string subredditName, string accessToken, CancellationToken cancellationToken)
        {
            var uri = new Uri(@$"{ResolveSubredditEndpoint(subredditName)}new");
            return await GetPosts(uri, accessToken, cancellationToken);
        }

        /// <summary>
        /// Asynchronously get a listing of popular posts for the subreddit.
        /// </summary>
        public async Task<Listing> GetPopularPostsForSubrredditAsync(string subredditName, string accessToken, CancellationToken cancellationToken)
        {
            var uri = new Uri(@$"{ResolveSubredditEndpoint(subredditName)}hot");
            return await GetPosts(uri, accessToken, cancellationToken);
        }
    }
    
   

}