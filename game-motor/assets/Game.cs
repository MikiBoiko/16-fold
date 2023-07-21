using Fold.Motor;

namespace Fold;

public enum CardColor
{
    red = 0,
    black = 1,
    both = 2,
    none = 3
};

public class Game
{
    #region Attributes and constructor
    public static readonly int PLAYER_COUNT = 2;

    // Players (red and black)
    public readonly Player[] players;

    // Board (contains the cards)
    private Board _board;

    // Current state
    private int _startingTurnPlayerIndex;
    public int TurnCount { private set; get; }
    private CardColor _turn;
    public int GameMove => TurnCount / 2;

    public delegate void OnInitializedGame();
    public OnInitializedGame OnGameStarted;

    private GameResolution? _resolution;
    public bool GameEnded => _resolution != null;
    public bool GameStarted => TurnCount == 0;
    public delegate void OnSetGameEnded(GameResolution resolution);
    private OnSetGameEnded OnGameEnded;

    // Constructor
    // Player 1 and 2 instances,
    // time and increment, both in milliseconds
    public Game(
        int idRed,
        int idBlack,
        double interval,
        double increment
    )
    {
        players = new Player[PLAYER_COUNT];

        players[0] = new Player(
            idRed,
            CardColor.red,
            interval,
            increment,
            () => SetGameResolution(new GameResolution(CardColor.black, GameResolution.Reason.TIME))
        );

        players[1] = new Player(
            idBlack,
            CardColor.black,
            interval,
            increment,
            () => SetGameResolution(new GameResolution(CardColor.red, GameResolution.Reason.TIME))
        );

        _board = new Board();
    }
    #endregion

    #region Initialization and finalization
    // After this method is done, the game is ready to begin.
    // 1. Uses the game seed to generate a pseudo-random class.
    // 2. Pick who starts.
    // 3. Initializes the board with random initial values and places them in the
    //    board player's starting position.
    public GameInitializationResolution InitializeGame()
    {
        // All random events only happens in the initialization 
        Random random = new Random();

        // Pick who starts
        TurnCount = 0;
        _turn = (CardColor)random.Next(PLAYER_COUNT);

        // Sets up the board with a deck. 
        List<int> cardValues = _board.InitializeBoard(players[0], players[1], random.Next(), new FrenchDeck(random.Next()));

        return new GameInitializationResolution(
            _turn,
            cardValues
        );
    }

    // Restarts an already finished game
    public void Restart(bool sameCards)
    {
        if (!GameEnded)
            throw new GameDidNotEndException();

        // unset resolution
        _resolution = null;

        // calculate starting turn
        TurnCount = 0;
        _turn = _startingTurnPlayerIndex;
        NextTurn(false);
        _startingTurnPlayerIndex = _turn;

        // Restart it
        Random random = new Random();
        _board.InitializeBoard(
            players[0],
            players[1],
            positionShuffleSeed: random.Next(),
            deck: sameCards ? null : new FrenchDeck(random.Next())
        );
    }
    #endregion

    #region Actions and decisions
    public ActionResolution DoAction(CardColor playerColor, Action action)
    {
        if (GameEnded)
            throw new GameEndedException();

        Player turnPlayer = players[_turn];

        // Check that it's the players turn
        if (turnPlayer.color != playerColor) // TODO : add premove
            throw new NotPlayersTurnException();

        ActionResolution actionResolution = action.DoAction(turnPlayer, _board);

        if (actionResolution.resolution.HasValue)
            SetGameResolution(actionResolution.resolution.Value);
        else NextTurn(actionResolution.actionColor != turnPlayer.color);

        return actionResolution;
    }

    public void DoDecision(CardColor playerColor, Decision decision)
    {
        if (GameEnded)
            throw new GameEndedException();

        Player player = GetPlayerById((int)playerColor);
        decision.DoDecision(player, this);
    }
    #endregion

    #region Class methods
    public void NextTurn(bool didAnIllegalMove)
    {
        players[(int)_turn].EndTurn(didAnIllegalMove);
        TurnCount++;
        
        _turn++;
        _turn %= 2;
        players[_turn].StartTurn();
    }

    // TODO : maybe simplifly
    public Player GetPlayerById(int playerId)
    {
        Player? player = null;
        for (int i = 0; i < PLAYER_COUNT && player == null; i++)
            if (players[i].id == playerId)
                player = players[i];

        if (player == null)
            throw new PlayerIdNotFoundException();

        return player;
    }

    public Player OtherPlayer(Player player) => players[((int)player.color + 1) % 2];

    public void SetGameResolution(GameResolution resolution)
    {
        if (GameEnded)
            throw new GameEndedException();

        _resolution = resolution;
        OnGameEnded.Invoke(resolution);
    }
    #endregion

    #region Dev
    public void PrintBoard()
    {
        string[,] boardFormatted = new string[4, 7];
        foreach (KeyValuePair<BoardPosition, CardStack?> stack in _board.GetPosition())
        {
            if (stack.Value != null)
            {
                string formatedValue = stack.Value.Card.value.ToString("00");
                boardFormatted[stack.Key.x, stack.Key.y] = formatedValue != "00" ? formatedValue : "JK";
            }
            else
                boardFormatted[stack.Key.x, stack.Key.y] = "XX";
        }

        Console.WriteLine("·----·----·----·----·");
        for (int y = 0; y < 7; y++)
        {
            string line = "| ";
            for (int x = 0; x < 4; x++)
            {
                line += (boardFormatted[x, y] ?? "  ") + " | ";
            }
            Console.WriteLine(line);
            Console.WriteLine("·----·----·----·----·");
        }
    }
    #endregion
}

#region Game exceptions
public class NotPlayersTurnException : Exception { }
public class PlayerIdNotFoundException : Exception { }
public class GameEndedException : Exception { }
public class GameDidNotEndException : Exception { }
#endregion

#region Board Exceptions
public class NotAdjecentException : Exception { }
public class NoCardFoundException : Exception { }
public class PositionOccupiedException : Exception { }
public class OutOfBoardPositionException : Exception { }
public class CardNotCloseToWinningException : Exception { }
#endregion

#region Game resolutions
public class GameInitializationResolution
{
    public CardColor Turn { set; get; }
    public List<int> PickedCards { set; get; }

    public GameInitializationResolution(CardColor turn, List<int> pickedCards)
    {
        Turn = turn;
        PickedCards = pickedCards;
    }
}

public struct GameResolution
{
    public enum Reason
    {
        AGREED = 0,
        RESIGN = 1,
        PASSING = 2,
        MATERIAL = 3,
        TIME = 4,
        REPORT = 5,
        ILLEGAL = 6
    }

    public CardColor result;
    public Reason reason;

    public GameResolution(CardColor result, Reason reason)
    {
        this.result = result;
        this.reason = reason;
    }
}
#endregion
