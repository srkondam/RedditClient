using Reddit.Data;
using Reddit.Repositories.Abstractions;
using Reddit.Services.Abstractions;

namespace Reddit.Services
{
    public class RedditService : IRedditService
    {
        private readonly IRedditRepository _redditRepository;
        private readonly IRedditAccessTokenProvider _accessTokenProvider;        


        public RedditService(IRedditRepository redditRepository, IRedditAccessTokenProvider accessTokenProvider)
        {
            _redditRepository = redditRepository;
            _accessTokenProvider = accessTokenProvider;
        }

        public async Task<Listing> GetNewPostsForSubrreddit(string subredditName, CancellationToken cancellationToken)
        {
            var accessToken = await _accessTokenProvider.GetAccessTokenAsync(cancellationToken);
            return await _redditRepository.GetNewPostsForSubrredditAsync(subredditName, accessToken, cancellationToken);
        }

        public async Task<Listing> GetPopularPostsForSubrreddit(string subredditName, CancellationToken cancellationToken)
        {
            var accessToken = await _accessTokenProvider.GetAccessTokenAsync(cancellationToken);
            return await _redditRepository.GetPopularPostsForSubrredditAsync(subredditName, accessToken, cancellationToken);
        }

    }
}
