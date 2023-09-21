using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reddit.Services.Abstractions;
using System.Text.Json;

namespace Reddit.WebAPI.Controllers
{
    [AllowAnonymous]
    public class ApiController : Controller
    {
        private readonly IWorkerService _workerService;

        public ApiController(IWorkerService workerService)
        {
            _workerService = workerService;
        }

        public IActionResult Index()
        {
            return View();
        }
       
        public async Task GetNewPostsForSubrredditAsync(string id)
        {            
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            var post = await _workerService.GetRecentPostWithMostVotes(id, CancellationToken.None);

            await JsonSerializer.SerializeAsync(Response.Body, post.Data, cancellationToken: CancellationToken.None);
        }

        [HttpGet]
        public async Task GetUserWithMostRecentPostsAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            var data = await _workerService.GetUserWithMostRecentPosts(id, CancellationToken.None);

            await JsonSerializer.SerializeAsync(Response.Body, data, cancellationToken: CancellationToken.None);
        }
    }
}
