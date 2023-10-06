using Microsoft.AspNetCore.SignalR;
using Fold.Motor;
using Fold.Motor.Resources.Request;
using Fold.Motor.Resources.Response;
using Fold.Server.Resources;
using Fold.Motor.Resources.Resolution;
using System.Collections.Concurrent;

namespace Fold.Server.Hubs;

public class GameHub : Hub
{
    public GameHub() : base()
    {
        if (CurrentGame == null)
            throw new GameNotStartedException();
    }

    #region Player colors
    private static int s_connectedPlayers = 0;
    private static ConcurrentDictionary<string, CardColor> s_connectionColors = new();

    private CardColor GetColorFromConnectionId()
    {
        if (!s_connectionColors.ContainsKey(Context.ConnectionId))
            return CardColor.none;

        return s_connectionColors[Context.ConnectionId];
    }
    #endregion

    #region Game
    public static Game? CurrentGame { private set; get; }

    public static void SetUp(IHubContext<GameHub>? context, double interval = 1200000, double increment = 3000)
    {
        if(context == null)
        {
            throw new NoContextContent();
        }

        CurrentGame = new Game(
            interval,
            increment,
            onGameStarted: (GameStartedResponse response) => {
                context.Clients.All.SendAsync("RecieveGameStarted", response);
            },
            onGameEnded: (GameEndedResponse response) => {
                context.Clients.All.SendAsync("RecieveGameEnded", response);
            }
        );
    }
    #endregion

    #region Connection
    public async Task State()
    {
        if (CurrentGame == null)
        {
            await Clients.Caller.SendAsync("RecieveError", "Empty chat message.");
            return;
        }

        CardColor connectionColor = (CardColor)(s_connectedPlayers % 2);
        s_connectionColors.TryAdd(Context.ConnectionId, connectionColor);
        await Clients.Caller.SendAsync("RecieveState", new StateResource
        {
            PlayingColor = connectionColor,
            State = CurrentGame.GetState()
        });

        if (s_connectedPlayers == 1)
            CurrentGame.Start();
        
        s_connectedPlayers++;
    }
    #endregion

    #region Chat
    private static readonly int MAX_MESSAGE_LENGTH = 300;

    public async Task SendMessage(string content)
    {
        bool isShort = content.Length == 0;
        if (isShort)
        {
            //throw new EmptyMessageContent();
            await Clients.Caller.SendAsync("RecieveError", "Empty chat message.");
            return;
        }
        else if (content.Length > MAX_MESSAGE_LENGTH)
        {
            await Clients.Caller.SendAsync("RecieveError", "Too long of a message.");
            return;
        }

        await Clients.All.SendAsync("RecieveMessage", new ChatMessageResource { Username = GetColorFromConnectionId() == CardColor.red ? "RED" : "BLACK", Content = content });
    }
    #endregion

    #region Actions
    public async Task DoAction(ActionRequest actionRequest)
    {
        Console.WriteLine("Doing action: " + actionRequest.Type);
        CardColor connectionColor = GetColorFromConnectionId();

        if (connectionColor == CardColor.none)
        {
            await Clients.Caller.SendAsync("RecieveError", "Not a player.");
            return;
        }

        if (CurrentGame == null)
        {
            await Clients.Caller.SendAsync("RecieveError", "Game is not loaded in server.");
            return;
        }

        if (!CurrentGame.GameStarted || CurrentGame.GameEnded)
        {
            await Clients.Caller.SendAsync("RecieveError", "Game is not in a playing state.");
            return;
        }

        try
        {
            ActionResolution actionResolution = CurrentGame.DoAction(
                connectionColor,
                actionRequest
            );

            await Clients.All.SendAsync("Recieve" + actionRequest.Type, actionResolution.AllResponse);
            if (actionResolution.OwnerResponse != null)
                await Clients.Caller.SendAsync("RecieveOwner" + actionRequest.Type, actionResolution.OwnerResponse);
        }
        catch (Exception e)
        {
            await Clients.Caller.SendAsync("RecieveError", "Error doing an action. " + e.Source + ": " + e.Message + ". " + e.StackTrace);
            return;
        }
    }
    #endregion

    #region Decisions
    public async Task DoDecision(DecisionRequest decisionRequest)
    {
        Console.WriteLine("Doing decision: " + decisionRequest.Type);
        CardColor connectionColor = GetColorFromConnectionId();

        if (connectionColor == CardColor.none)
        {
            //throw new NotAPlayerException();
            await Clients.Caller.SendAsync("RecieveError", "Not a player.");
            return;
        }

        if (CurrentGame == null)
        {
            await Clients.Caller.SendAsync("RecieveError", "Game is not loaded in server.");
            return;
        }

        if (!CurrentGame.GameStarted || CurrentGame.GameEnded)
        {
            await Clients.Caller.SendAsync("RecieveError", "Game is not in a playing state.");
            return;
        }

        try
        {
            DecisionResolution decisionResolution = CurrentGame.DoDecision(
                connectionColor,
                decisionRequest
            );

            await Clients.Caller.SendAsync("Recieve" + decisionRequest.Type, decisionResolution.Response);
        }
        catch (Exception e)
        {
            await Clients.Caller.SendAsync("RecieveError", "Error while doing a decision. " + e.Source + ": " + e.Message + ". " + e.StackTrace);
        }
    }
    #endregion
}

public class GameNullException : Exception
{
    public GameNullException(string message) : base(message) { }
}

public class GameStateException : Exception
{
    public GameStateException(string message) : base(message) { }
}

public class NoContextContent : Exception { }
//public class EmptyMessageContent : Exception { }
//public class NotAPlayerException : Exception { }