using System.Collections.Generic;
using System;

namespace Fold {
    public class Board {
        // size count of the tiles in a board
        public static readonly int SIZE_X = 4, SIZE_Y = 7, PLAYER_STARTING_CARDS_COUNT = 8;

        // deck used to genereate initial values
        private Deck? _deck;

        // map of the cards in the game where the key is letter(x) + y, BoardPosition format
        private int _redCards = PLAYER_STARTING_CARDS_COUNT, _blackCards = PLAYER_STARTING_CARDS_COUNT;
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
            _cards.Clear();

            _deck = deck ?? _deck ?? new FrenchDeck(new Random().Next());
            
            // generate new initial cards if a new deck is sent
            List<int> initialValues = _deck.GenerateInitialValues(hasJokers, PLAYER_STARTING_CARDS_COUNT);

            // Generate random instance with the provided seed
            Random random = new Random(positionShuffleSeed);

            // set up players
            _redCards = _blackCards = PLAYER_STARTING_CARDS_COUNT;
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
                playerCards.Add(new CardStack(player.color, playerCard, new List<Card>()));
            }

            // same with the joker
            Joker playerJokerCard = new Joker(player.color);
            playerCards.Add(new CardStack(player.color, playerJokerCard, new List<Card>()));


            // randomly initialize player cards into the starting positions...
            for (int i = 0; i < positions.Length; i++)
            {
                int randomIndex = random.Next(playerCards.Count);
                Console.WriteLine(String.Format("{0} : {1}", positions[i], playerCards[randomIndex].Card.value));
                _cards.Add(positions[i], playerCards[randomIndex]);
                playerCards.RemoveAt(randomIndex);
            }
        }

        // Moves a card stack from one tile to another
        public CardColor Move(BoardPosition from, BoardPosition to) {
            if(!IsInbound(to))
                throw new OutOfBoardPositionException();

            if(_cards.ContainsKey(to))
                throw new PositionOccupiedException();


            CardStack stack = FindCardStack(from);
            _cards.Remove(from);
            _cards.Add(to, stack);
            return stack.OwnerColor;
        }

        // checks if the card in a position winned
        public CardColor CheckForWin(CardColor playerColor, BoardPosition position) {
            if(!IsInbound(position))
                throw new OutOfBoardPositionException();

            if(playerColor == CardColor.red) {
                if(position.y != SIZE_Y - 1)
                    throw new CardNotCloseToWinningException();
            }
            else if(playerColor == CardColor.black) {
                if(position.y != 0)
                    throw new CardNotCloseToWinningException();
            }

            return FindCardStack(position).OwnerColor;
        }

        public CardColor Attack(List<BoardPosition> from, BoardPosition to) {
            CardColor? actionColor = null;
            List<CardStack> attackingCardStacks = new List<CardStack>();
            foreach (BoardPosition fromPosition in from)
            {
                CardStack attackingStack = FindCardStack(fromPosition);
                actionColor = actionColor == null ? 
                    (attackingStack.OwnerColor) 
                    : 
                    (actionColor != attackingStack.OwnerColor ? CardColor.both : actionColor);
                attackingCardStacks.Add(attackingStack);
            }

            CardStack defensingStack = FindCardStack(to);
            CardDefense defense = defensingStack.Defend(
                attackingCardStacks.ConvertAll<Card>(
                    new Converter<CardStack, Card>(CardStack.ToCard)
                )
            );

            Console.WriteLine(String.Format("Attack: {0}, {1}", defense.topCard.value, defense.result));

            bool switchCard = defense.topCard != defensingStack.Card;

            if(defense.result == CardDefense.Result.draw)
                _cards.Remove(to);

            foreach (BoardPosition fromPosition in from) {
                if(defense.result != CardDefense.Result.draw) {
                    CardStack attackingStack = _cards[fromPosition];
                    if(switchCard && attackingStack.Card == defense.topCard) {
                        attackingStack.MergeUnder(defensingStack);
                        _cards.Remove(to);
                        _cards.Add(to, attackingStack);
                        switchCard = false;
                    }
                    else {
                        defensingStack.MergeUnder(attackingStack);
                    }
                }
                _cards.Remove(fromPosition);
            }

            return actionColor ?? CardColor.both;
        }

        private bool IsInbound(BoardPosition position) => position.x >= 0 && position.x < SIZE_X && position.y >= 0 && position.y < SIZE_Y;

        private CardStack FindCardStack(BoardPosition at) {
            if(!_cards.ContainsKey(at))
                throw new NoCardFoundException();
            
            return _cards[at];
        }

        // returns a dictionary of the board positions, and returns null if the stack is hidden
        public Dictionary<BoardPosition, CardStack?> GetPosition() {
            Dictionary<BoardPosition, CardStack?> result = new Dictionary<BoardPosition, CardStack?>();
            foreach (KeyValuePair<BoardPosition, CardStack> cardStack in _cards)
            {
                result.Add(cardStack.Key, cardStack.Value.IsHidden ? null : cardStack.Value);
            }
            return result;
        }
    }

    #region Board Exceptions
        public class NotAdjecentException : Exception { }
        public class NoCardFoundException : Exception { }
        public class PositionOccupiedException : Exception { }
        public class OutOfBoardPositionException : Exception { }
        public class CardNotCloseToWinningException : Exception {  }
    #endregion
}