using Reddit.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reddit.Services.Abstractions
{
    public interface IRedditService
    {
        Task<Listing> GetNewPostsForSubrreddit(string subredditName, CancellationToken cancellationToken);

        Task<Listing> GetPopularPostsForSubrreddit(string subredditName, CancellationToken cancellationToken);
    }
}
