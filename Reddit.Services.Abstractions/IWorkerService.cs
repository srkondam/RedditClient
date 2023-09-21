using Reddit.Data;

namespace Reddit.Services.Abstractions
{
    public interface IWorkerService
    {
        Task<Post> GetRecentPostWithMostVotes(string topicKey, CancellationToken cancellationToken);

        Task<string> GetUserWithMostRecentPosts(string topicKey, CancellationToken cancellationToken);
    }
}