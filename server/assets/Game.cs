namespace Fold {
    public class Game {
        #region Attributes and constructor
        public static readonly int PLAYER_COUNT = 2;

        // Players (red and black)
        public readonly Player[] players;

        // Board (contains the cards)
        private Board _board;

        // Time format
        public readonly long time, increment;

        // Current state
        private int _startingTurnPlayerIndex;
        private int _turnCount;
        private int _turnPlayerIndex;
        private GameResolution? _resolution;

        // Constructor
        // Player 1 and 2 instances,
        // time and increment, both in milliseconds
        public Game(Player playerRed, Player playerBlack, long time, long increment) {
            players = new Player[PLAYER_COUNT];
            players[0] = playerRed;
            players[1] = playerBlack;

            _board = new Board();

            this.time = time;
            this.increment = increment;
        }
        #endregion

        #region Initialization and finalization
        // After this method is done, the game is ready to begin.
        // 1. Uses the game seed to generate a pseudo-random class.
        // 2. Pick who starts.
        // 3. Initializes the board with random initial values and places them in the
        //    board player's starting position.
        public void InitializeGame() {
            // All random events only happens in the initialization 
            Random random = new Random();

            // Pick who starts
            _turnPlayerIndex = random.Next(PLAYER_COUNT);

            // Sets up the board with a deck. 
            _board.InitializeBoard(players[0], players[1], random.Next(), new FrenchDeck(random.Next()));
        }

        // Restarts an already finished game
        public void Restart(bool sameCards) {
            // calculate starting turn
            _turnPlayerIndex = _startingTurnPlayerIndex;
            NextTurn();
            _startingTurnPlayerIndex = _turnPlayerIndex;

            // Restart it
            Random random = new Random();
            if(sameCards)
                _board.InitializeBoard(players[0], players[1], positionShuffleSeed: random.Next());
            else 
                _board.InitializeBoard(players[0], players[1], positionShuffleSeed: random.Next(), deck: new FrenchDeck(random.Next()));
        }
        #endregion

        #region Actions and decisions
        public void NextTurn() {
            _turnCount++;
            _turnPlayerIndex = (_turnPlayerIndex + 1 == PLAYER_COUNT) ? 0 : _turnPlayerIndex + 1;
        } 

        public void DoAction(int playerId, Action action) {
            Player turnPlayer = players[(int)_turnPlayerIndex];
            // Check that it's the players turn
            if(turnPlayer.id != playerId) 
                throw new NotPlayersTurnException();

            action.DoAction(turnPlayer, _board);
        }
        
        public void DoDecision(int playerId, Decision decision) {
            Player player = GetPlayerById(playerId);
            decision.DoDecision(player, this);
        }
        #endregion

        #region Private methods
        private Player GetPlayerById(int playerId) {
            Player? player = null;
            for (int i = 0; i < PLAYER_COUNT && player == null; i++)
                if(players[i].id == playerId)
                    player = players[i];

            if(player == null)
                throw new PlayerIdNotFoundException();

            return player;
        }
        #endregion
    }

    #region Game exceptions
    public class NotPlayersTurnException : Exception { }
    public class PlayerIdNotFoundException : Exception { }
    #endregion

    #region Game resolutions
    public struct GameResolution {
        public enum Result { 
            RED = 0, 
            BLACK = 1, 
            DRAW = 2, 
            CANCELED = 3 
        };

        public enum Reason { 
            AGREED = 0, 
            SURRENDER = 1, 
            PASSING = 2,
            MATERIAL = 3, 
            TIME = 4
        }

        public Result result;
        public Reason reason;
    }
    #endregion
}