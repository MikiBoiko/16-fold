// https://ably.com/topic/scaling-signalr
using Fold.Server.Hubs;
using Microsoft.AspNetCore.SignalR;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSignalR();

// https://learn.microsoft.com/es-es/aspnet/core/security/cors?view=aspnetcore-7.0
string WEB_POLICY_NAME = "WebPolicy";
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        name: WEB_POLICY_NAME,
        policy =>
        {
            policy.WithOrigins("http://localhost:3000/")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .SetIsOriginAllowed((host) => true)
                .AllowCredentials();
        }
    );
});

var app = builder.Build();

app.UseCors(WEB_POLICY_NAME);

app.MapHub<GameHub>("/game");

SetUpGame(app);

app.Run();

static void SetUpGame(WebApplication app)
{
    IHubContext<GameHub>? context = (IHubContext<GameHub>?)app.Services.GetService(typeof(IHubContext<GameHub>));
    GameHub.SetUp(context);
}