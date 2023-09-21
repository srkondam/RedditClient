// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Reddit.Repositories.Abstractions;
using Reddit.Repositories;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using Reddit.Services;
using Reddit.Services.Abstractions;
using Reddit.Data;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureServices((hostContext, services) =>
    {        
        services.AddHttpClient("Reddit", httpClient =>
        {
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.ConnectionClose = true;
            httpClient.DefaultRequestHeaders.UserAgent.Add(new System.Net.Http.Headers.ProductInfoHeaderValue("src", "1.0"));
        });

        services.AddSingleton<IRedditService, RedditService>();
        services.AddSingleton<IWorkerService, WorkerService>();
        services.AddSingleton<IRedditAccessTokenProvider, RedditAccessTokenProvider>();

        services.AddTransient<IRedditRepository, HttpClientRedditRepository>();
    });

using var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.ClearProviders()
        .AddConsole();
});

var host = builder.Build();

var config = host.Services.GetRequiredService<IConfiguration>();
var redditService = host.Services.GetRequiredService<IRedditService>();
var logger = loggerFactory.CreateLogger<Program>();
var worker = new WorkerService(config, redditService, logger);

var popularPost = await worker.GetRecentPostWithMostVotes("artificial", CancellationToken.None);
Console.WriteLine(@"### Popular Post ###");
Console.WriteLine(popularPost.ToString());


var authprWithMostPosts = await worker.GetUserWithMostRecentPosts("technology", CancellationToken.None);
Console.WriteLine(@"### Auther With Most Recent Posts ###");
Console.WriteLine(authprWithMostPosts);

await host.RunAsync();
