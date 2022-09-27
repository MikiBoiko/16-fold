using System.Collections.Generic;
using System;

namespace Fold.Motor {
    public class Board {
        // size count of the tiles in a board
        public static readonly int SIZE_X = 4, SIZE_Y = 7, PLAYER_STARTING_CARDS_COUNT = 8;

        // deck used to genereate initial values
        private Deck? _deck;

        // count for the cards
        private int[] _playerCardStackCount = new int[2];
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
            _cards.Clear();

            _deck = deck ?? _deck ?? new FrenchDeck(new Random().Next());
            
            // generate new initial cards if a new deck is sent
            List<int> initialValues = _deck.GenerateInitialValues(hasJokers, PLAYER_STARTING_CARDS_COUNT);

            // Generate random instance with the provided seed
            Random random = new Random(positionShuffleSeed);

            // set up players
            SetUpPlayer(playerRed, _playerRedStartingPositions, new List<int>(initialValues), ref random);
            SetUpPlayer(playerBlack, _playerBlackStartingPositions, new List<int>(initialValues), ref random);
        }

        private void SetUpPlayer(Player player, BoardPosition[] positions, List<int> initialValues, ref Random random) {
            // set player card stack count
            _playerCardStackCount[(int)player.color] = PLAYER_STARTING_CARDS_COUNT;
            
            // instantiate players
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
        public ActionResolution Move(BoardPosition from, BoardPosition to) {
            if(!IsInbound(to))
                throw new OutOfBoardPositionException();

            if(_cards.ContainsKey(to))
                throw new PositionOccupiedException();


            CardStack stack = FindCardStack(from);
            _cards.Remove(from);
            _cards.Add(to, stack);
            return new ActionResolution(stack.OwnerColor);
        }

        // checks if the card in a position winned
        public ActionResolution CheckForWin(CardColor playerColor, BoardPosition position) {
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

            CardStack winingStack = FindCardStack(position); 
            return new ActionResolution(
                winingStack.OwnerColor,
                winingStack.OwnerColor == playerColor ?
                    new GameResolution(
                        (GameResolution.Result) playerColor,
                        GameResolution.Reason.PASSING
                    )
                    :
                    new GameResolution(
                        (GameResolution.Result) (((int)playerColor + 1) % 2),
                        GameResolution.Reason.ILLEGAL
                    )
            );
        }

        public ActionResolution Attack(List<BoardPosition> from, BoardPosition to) {
            // Find all attacking card stacks
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

            // Defend attack
            CardStack defendingStack = FindCardStack(to);
            CardDefense defense = defendingStack.Defend(
                attackingCardStacks.ConvertAll<Card>(
                    new Converter<CardStack, Card>(CardStack.ToCard)
                )
            );

            //Console.WriteLine(String.Format("Attack: {0}, {1}", defense.topCard.value, defense.result));

            // Apply the defense result
            bool switchCard = defense.topCard != defendingStack.Card;

            if(defense.result == CardDefense.Result.draw)
                _cards.Remove(to);

            foreach (BoardPosition fromPosition in from) {
                if(defense.result != CardDefense.Result.draw) {
                    CardStack attackingStack = _cards[fromPosition];
                    if(switchCard && attackingStack.Card == defense.topCard) {
                        attackingStack.MergeUnder(defendingStack);
                        _cards.Remove(to);
                        _playerCardStackCount[(int)defendingStack.OwnerColor]--;
                        _cards.Add(to, attackingStack);
                        switchCard = false;

                    }
                    else {
                        defendingStack.MergeUnder(attackingStack);
                        _playerCardStackCount[(int)attackingStack.OwnerColor]--;
                    }
                }
                _cards.Remove(fromPosition);
            }

            return new ActionResolution(
                actionColor ?? CardColor.both,
                (_playerCardStackCount[0] > 0 && _playerCardStackCount[1] > 0) ?
                    null
                :
                (_playerCardStackCount[0] > 0) ?
                    new GameResolution(
                        GameResolution.Result.RED,
                        GameResolution.Reason.MATERIAL
                    )
                :
                (_playerCardStackCount[1] > 0) ?
                    new GameResolution(
                        GameResolution.Result.BLACK,
                        GameResolution.Reason.MATERIAL
                    )
                :
                new GameResolution(
                    GameResolution.Result.DRAW,
                    GameResolution.Reason.MATERIAL
                )
            );


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