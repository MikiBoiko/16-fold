using System.Collections.Generic;

namespace Fold.Motor {
    public enum CardColor {
        red = 0,
        black = 1,
        both = 2
    };

    public struct CardDefense {
        public Card topCard;
        public enum Result { won, lost, draw };
        public Result result;
    }

    public abstract class Card {
        public readonly CardColor color;
        public readonly int value;

        public Card(CardColor color, int value) {
            this.color = color;
            this.value = value;
        }

        public abstract CardDefense Defend(List<Card> cardsAttacking);
    }

    public class NumberCard : Card {
        public NumberCard(CardColor color, int value) : base(color, value) { }

        public override CardDefense Defend(List<Card> cardsAttacking) {
            CardDefense result;
            result.topCard = this;
            result.result = CardDefense.Result.won;

            int attackValue = 0;
            foreach (Card cardAttacking in cardsAttacking) {
                attackValue += cardAttacking.value;
                if(attackValue == value)
                    result.result = CardDefense.Result.draw;
                if(attackValue > value) {
                    result.result = CardDefense.Result.lost;
                    break;
                }
            }

            return result;
        }
    }

    public class Joker : Card {
        public Joker(CardColor color) : base(color, 0) { }

        public override CardDefense Defend(List<Card> cardsAttacking) {
            CardDefense result;
            result.topCard = this;
            result.result = CardDefense.Result.won;

            Card cardWithHighestValue = cardsAttacking[0];
            foreach (Card cardAttacking in cardsAttacking) {
                if(cardAttacking.value == value) {
                    result.result = CardDefense.Result.lost;
                    break;
                }

                if(cardAttacking.value > cardWithHighestValue.value) {
                    cardWithHighestValue = cardAttacking;
                }
            }

            if(result.result == CardDefense.Result.won)
                result.topCard = cardWithHighestValue;

            return result;
        }
    }

// TODO : merging with nodes and shit
    public class CardStack {
        public static Card ToCard(CardStack stack) => stack.Card;
        
        public CardColor OwnerColor { private set; get; }
        public Card Card { private set; get; }
        public List<Card> DeadCards { private set; get; } = new List<Card>();
        public bool IsHidden { get => DeadCards.Count == 0; }


        public CardStack(CardColor color, Card card, List<Card> deadCards) {
            OwnerColor = color;
            Card = card;
            DeadCards = deadCards;
        }

        public CardDefense Defend(List<Card> cardsAttacking) => Card.Defend(cardsAttacking);

        public void MergeUnder(CardStack stack) {
            DeadCards.Add(stack.Card);
            DeadCards.AddRange(stack.DeadCards);
        }
    }
}