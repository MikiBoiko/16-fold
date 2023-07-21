// https://ably.com/topic/scaling-signalr
using FoldServer.Hubs;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSignalR();

// https://learn.microsoft.com/es-es/aspnet/core/security/cors?view=aspnetcore-7.0
string WEB_POLICY_NAME = "WebPolicy";
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        name: WEB_POLICY_NAME,
        policy  =>
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

// Map a new game between two players
// TODO : extend this into full matches with rematches
Fold.Game game = new Fold.Game(
    1, 
    2,
    20000,
    1000
);
GameHub.CurrentGame = game;
app.MapHub<GameHub>("/game");

app.Run();
