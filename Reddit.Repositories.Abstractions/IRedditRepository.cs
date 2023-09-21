using Reddit.Data;

namespace Reddit.Repositories.Abstractions
{
    public interface IRedditRepository
    {       

        Task<Listing> GetNewPostsForSubrredditAsync(string subredditName, string accessToken, CancellationToken cancellationToken);

        Task<Listing> GetPopularPostsForSubrredditAsync(string subredditName, string accessToken, CancellationToken cancellationToken);

    }
}