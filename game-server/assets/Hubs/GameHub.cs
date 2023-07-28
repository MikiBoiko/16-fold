using Microsoft.AspNetCore.SignalR;
using Fold.Motor;
using Fold.Motor.Resources.Request;
using Fold.Motor.Resources.Response;
using Fold.Server.Resources;
using Fold.Motor.Resources.Resolution;

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
    private Dictionary<string, CardColor> connectionColor = new();

    private CardColor GetColorFromConnectionId()
    {
        if (connectionColor.ContainsKey(Context.ConnectionId))
            return CardColor.none;

        return connectionColor[Context.ConnectionId];
    }
    #endregion

    #region Game
    public static Game? CurrentGame { private set; get; }

    public static void GameStarted(GameStartedResponse startedResponse) { }
    public static void GameEnded(GameEndedResponse endedResponse) { }

    public static void SetUp(double interval = 120000, double increment = 3000)
    {
        CurrentGame = new Game(
            interval,
            increment,
            onGameStarted: GameStarted,
            onGameEnded: GameEnded
        );

        CurrentGame.Start();
    }

    #endregion


    #region Connection
    public async Task Connect()
    {
        if (CurrentGame == null)
        {
            //throw new EmptyMessageContent();
            await Clients.Caller.SendAsync("RecieveError", "Empty chat message.");
            return;
        }

        Console.WriteLine(s_connectedPlayers);
        await Clients.Caller.SendAsync("RecieveConnected", new ConnectedResource
        {
            PlayingColor =  (CardColor)(s_connectedPlayers % 2), // s_connectedPlayers > 1 ? CardColor.none : (CardColor)s_connectedPlayers,
            State = CurrentGame.GetState()
        });

        s_connectedPlayers++;
    }
    #endregion

    #region Chat
    private static readonly int MAX_MESSAGE_LENGTH = 300;

    public async Task SendMessage(string username, string content)
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

        await Clients.All.SendAsync("RecieveMessage", new ChatMessageResource { Username = username, Content = content });
    }
    #endregion

    #region Actions
    public async Task MoveAction(string from, string to)
    {
        CardColor connectionColor = GetColorFromConnectionId();

        if (connectionColor == CardColor.none)
        {
            //throw new NotAPlayerException();
            await Clients.Caller.SendAsync("RecieveError", "Not a player.");
            return;
        }

        try
        {
            if (CurrentGame == null)
                throw new GameNullException("Game is not set in GameHub.");

            ActionResolution actionResolution = CurrentGame.DoAction(
                connectionColor,
                new ActionRequest
                {
                    Type = "Move",
                    Data = new Dictionary<string, object>
                    {
                        { "from", from },
                        { "to", to }
                    }
                }
            );

            await Clients.Caller.SendAsync("RecieveMove", from, to);
        }
        catch (Exception e)
        {
            await Clients.Caller.SendAsync("RecieveError", "Moving a card. " + e.Source + ": " + e.Message + ". " + e.StackTrace);
            return;
        }
    }

    public async Task SeeAction(string from)
    {
        // TODO
    }

    public async Task PassAction(string from)
    {
        // TODO
    }
    #endregion

    #region Decisions
    public async Task ReportIllegalMoveDecision(string from)
    {
        // TODO
    }

    public async Task ResignDecision(string from)
    {
        // TODO
    }

    public async Task AddTimeDecision(string from)
    {
        // TODO
    }
    #endregion
}

public class GameNullException : Exception
{
    public GameNullException(string message) : base(message) {}
}
//public class EmptyMessageContent : Exception { }
//public class NotAPlayerException : Exception { }