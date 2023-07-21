using Microsoft.AspNetCore.SignalR;
using Fold;
using FoldServer.Resources;

namespace FoldServer.Hubs;

public class GameHub : Hub
{
    private static int s_connectedPlayers = 0; 
    public static Game? CurrentGame = null;

    protected GameHub() : base()
    {
        if(CurrentGame == null)
        {
            Console.WriteLine("Null game while starting GameHub.");
            return;
        }

        CurrentGame.OnGameStarted = (GameInitializationResolution gameStartedResource) => {

        };
    }

    private CardColor GetColorFromConnectionId()
    {
        string connectionId = Context.ConnectionId;
        return CardColor.none;
    }

    #region Connection
    public async Task Connect() 
    {
        await Clients.Caller.SendAsync("Connected", s_connectedPlayers > 1 ? CardColor.none : (CardColor) s_connectedPlayers);
        
        if(s_connectedPlayers >= 1)
        {
            dev
        }
        
        s_connectedPlayers++;
    }

    public override Task OnConnectedAsync()
    {
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        return base.OnDisconnectedAsync(exception);
    }
    #endregion

    #region Chat
    private static readonly int MAX_MESSAGE_LENGTH = 300;

    public async Task SendMessage(string username, string content)
    {
        bool isShort = content.Length == 0;
        if(isShort)
        {
            //throw new EmptyMessageContent();
            await Clients.Caller.SendAsync("Error", "Empty chat message.");
            return;
        }
        else if(content.Length > MAX_MESSAGE_LENGTH)
        {
            await Clients.Caller.SendAsync("Error", "Too long of a message.");
            return;
        }
        
        await Clients.All.SendAsync("RecieveMessage", new ChatMessageResource{ Username = username, Content = content });
    }
    #endregion

    #region Actions
    public async Task MoveAction(string from, string to)
    {
        CardColor connectionColor = GetColorFromConnectionId();

        if(CurrentGame == null)
        {
            //throw new GameNotInitializedException();
            await Clients.Caller.SendAsync("Error", "Game not instantiated.");
            return;
        }

        if((int)connectionColor > 1)
        {
            //throw new NotAPlayerException();
            await Clients.Caller.SendAsync("Error", "Not a player.");
            return;
        }

        try
        {
            ActionResolution actionResolution = CurrentGame.DoAction(
                connectionColor,
                new MoveAction(
                    new BoardPosition(from),
                    new BoardPosition(to)
                )
            );
            
            await Clients.All.SendAsync("RecieveMove", from, to);
        }
        catch (Exception e)
        {
            await Clients.Caller.SendAsync("Error", "Moving a card. " + e.Source + ": " + e.Message + ". " + e.StackTrace);
            return;
        }
    }
    #endregion

    #region Decisions

    #endregion
}

//public class EmptyMessageContent : Exception { }
//public class GameNotInitializedException : Exception { }
//public class NotAPlayerException : Exception { }