using Reddit.Repositories;
using Reddit.Repositories.Abstractions;
using Reddit.Services;
using Reddit.Services.Abstractions;
using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient("Reddit", httpClient =>
{
    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    httpClient.DefaultRequestHeaders.ConnectionClose = true;
    httpClient.DefaultRequestHeaders.UserAgent.Add(new System.Net.Http.Headers.ProductInfoHeaderValue("src", "1.0"));
});

builder.Services.AddSingleton<IRedditService, RedditService>();
builder.Services.AddSingleton<IWorkerService, WorkerService>();
builder.Services.AddSingleton<IRedditAccessTokenProvider, RedditAccessTokenProvider>();
builder.Services.AddTransient<IRedditRepository, HttpClientRedditRepository>();
builder.Services.AddSingleton<ILogger>((configure) => { 
    var loggerFactory = LoggerFactory.Create(builder =>
    {
        builder.ClearProviders()
            .AddConsole();
    });

    return loggerFactory.CreateLogger("reddit");
});

// Add builder.services to the container.
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapDefaultControllerRoute();

app.Run();
