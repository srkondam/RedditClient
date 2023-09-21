using Microsoft.Extensions.Configuration;
using Reddit.Repositories;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Moq;

namespace Reddit.Repository.Tests
{

    public class MockLibrary
    {
        public static IConfiguration CreateMockConfiguration()
        {
            var configOptions = new Dictionary<string, string>
            {
                {"reddit_clientID", "RRW7mOBk21U4myZZVkvBbg"},
                {"reddit_clientSecret", "jaC07cs73jGaKLrK9BIo96BpUW_uoA"},
                {"reddit_username", "sak_3656"},
                {"reddit_password","Friend11!!"},
                {"reddit_token_endpoint", "https://www.reddit.com/api/v1/access_token"},
                {"reddit_subreddit_endpoint", "https://www.reddit.com/r/{SUBREDDIT_NAME}/"}
            };
            return new ConfigurationBuilder()
                .AddInMemoryCollection(configOptions)
                .Build();
        }


    }

    [TestClass]
    public class UnitTest1
    {
        

        [TestMethod]
        public void UnauthorizedTokenRequest_MissingBasicAuthDetails()
        {
            throw new NotImplementedException();
        }


        [TestMethod]
        public void UnknownSubreddit_ReturnEmptyListing()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void GetRecentPosts_ForTechnologSubreddit()
        {            
            var mockConfiguration = MockLibrary.CreateMockConfiguration();
            throw new NotImplementedException();
        }
    }
}