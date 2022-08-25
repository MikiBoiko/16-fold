namespace Fold {
    public class Board {
        // size count of the tiles in a board
        public static readonly int SIZE_X = 4, SIZE_Y = 7, PLAYER_STARTING_CARDS_COUNT = 8;

        // deck used to genereate initial values
        private Deck? _deck;

        // map of the cards in the game where the key is letter(x) + y, BoardPosition format
        private Dictionary<BoardPosition, CardStack> _cards = new Dictionary<BoardPosition, CardStack>();


        // starting positions of both players
        private static readonly BoardPosition[] _playerRedStartingPositions = BoardPosition.FormatedPositionStringArrayToBoardPositionArray(
            new string[] {
                "a3", "b3", "c3", "d3",
                "a1", "b1", "c1", "d1"
            },
            PLAYER_STARTING_CARDS_COUNT
        );

        private static readonly BoardPosition[] _playerBlackStartingPositions = BoardPosition.FormatedPositionStringArrayToBoardPositionArray(
            new string[] {
                "a7", "b7", "c7", "d7",
                "a5", "b5", "c5", "d5"
            },
            PLAYER_STARTING_CARDS_COUNT
        );
        
        public void InitializeBoard(Player playerRed, Player playerBlack, int positionShuffleSeed, Deck? deck = null, bool hasJokers = true) {
            if(_deck == null) {
                _deck = new FrenchDeck(new Random().Next());
            }
            
            // generate new initial cards if a new deck is sent
            List<int> initialValues = (deck != null ? deck : _deck).GenerateInitialValues(hasJokers, PLAYER_STARTING_CARDS_COUNT);

            // Generate random instance with the provided seed
            Random random = new Random(positionShuffleSeed);

            // set up players
            SetUpPlayer(playerRed, _playerRedStartingPositions, new List<int>(initialValues), ref random);
            SetUpPlayer(playerBlack, _playerBlackStartingPositions, new List<int>(initialValues), ref random);
        }

        private void SetUpPlayer(Player player, BoardPosition[] positions, List<int> initialValues, ref Random random) {
            // instantiate players cards
            List<CardStack> playerCards = new List<CardStack>();

            // for each initial value...
            foreach (int initialValue in initialValues)
            {
                // create a card for the player
                NumberCard playerCard = new NumberCard(player.color, initialValue);
                // add it to the cards
                playerCards.Add(new CardStack(player, new List<Card>{ playerCard }));
            }

            // same with the joker
            Joker playerJokerCard = new Joker(player.color);
            playerCards.Add(new CardStack(player, new List<Card>{ playerJokerCard }));


            // randomly initialize player cards into the starting positions...
            for (int i = 0; i < positions.Length; i++)
            {
                int randomIndex = random.Next(playerCards.Count);
                _cards.Add(positions[i], playerCards[randomIndex]);
                playerCards.RemoveAt(randomIndex);
            }
        }
    }
}