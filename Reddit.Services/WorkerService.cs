using Microsoft.Extensions.Configuration;
using Polly.RateLimit;
using Polly;
using Reddit.Data;
using Reddit.Repositories.Abstractions;
using Reddit.Services.Abstractions;
using Microsoft.Extensions.Logging;

namespace Reddit.Services
{
    public class WorkerService : IWorkerService
    {
        private readonly IConfiguration _configuration;
        private readonly IRedditService _redditService;
        private readonly ILogger _logger;

        // TODO: simple cache for throttling.
        private Post? _LastKnownPopularPost { get; set; }
        private string? _LastKnownAuthorWithMostPosts { get; set; }

        public WorkerService(IConfiguration configuration, IRedditService redditService, ILogger logger)
        {
            _configuration = configuration;
            _redditService = redditService;
            _logger = logger;
        }        

        public async Task<Post> GetRecentPostWithMostVotes(string topicKey, CancellationToken cancellationToken)
        {            
            Policy.RateLimitAsync(_configuration.GetValue<int>("reddit_rateLimitPerMin"), TimeSpan.FromSeconds(60), 5);

            try
            {
                var listing = await _redditService.GetNewPostsForSubrreddit(topicKey, cancellationToken);
                _LastKnownPopularPost = listing.Data.Posts.OrderByDescending(P => P.Data.Ups).First();
            }
            catch (RateLimitRejectedException ex)
            {
                _logger.LogWarning("RateLimitRejectedException: too many requests for reddit. Return cached information.");
            }

            if (_LastKnownPopularPost == null)
            {
                _logger.LogError("Application cache couldn't complete current request.");
                throw new ApplicationException("Something went wrong with application. Please reach out to administrator.");
            }
            
            return _LastKnownPopularPost;
        }

        /// <summary>
        /// Asynchronously get details of author with most recent posts.
        /// </summary>
        /// <remarks>Return is for following format; "AuthorName": "John" | "PostCount": "10" | ...</remarks>
        public async Task<string> GetUserWithMostRecentPosts(string topicKey, CancellationToken cancellationToken)
        {
            Policy.RateLimitAsync(_configuration.GetValue<int>("reddit_rateLimitPerMin"), TimeSpan.FromSeconds(60), 5);

            try
            {
                var listing = await _redditService.GetNewPostsForSubrreddit(topicKey, cancellationToken);

                _LastKnownAuthorWithMostPosts = listing.Data.Posts.GroupBy(P => P.Data.Author)
                .Select(r => @$"Author: {r.Key} | PostCount: {r.Count()} | TotalUpCountOnPosts: {r.Sum(p => p.Data.Ups)} | TotalDownCountOnPosts: {r.Sum(p => p.Data.Downs)}")
                .First();
            }
            catch (RateLimitRejectedException ex)
            {
                _logger.LogWarning("RateLimitRejectedException: too many requests for reddit. Return cached information.");
            }

            if (_LastKnownAuthorWithMostPosts == null)
            {
                _logger.LogError("Application cache couldn't complete current request.");
                throw new ApplicationException("Something went wrong with application. Please reach out to administrator.");
            }
            return _LastKnownAuthorWithMostPosts;
        }
    }
}