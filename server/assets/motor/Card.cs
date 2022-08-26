using System.Collections.Generic;

namespace Fold {
    public enum CardColor {
        red = 0,
        black = 1
    };

    public struct CardDefenseResult {
        public Card topCard;
        public bool didDefend;
    }

    public abstract class Card {
        public readonly CardColor color;
        public readonly int value;

        public Card(CardColor color, int value) {
            this.color = color;
            this.value = value;
        }

        public abstract CardDefenseResult Defend(List<Card> cardsAttacking);
    }

    public class NumberCard : Card {
        public NumberCard(CardColor color, int value) : base(color, value) { }

        public override CardDefenseResult Defend(List<Card> cardsAttacking) {
            CardDefenseResult result;
            result.topCard = this;
            result.didDefend = true;

            int attackValue = 0;
            foreach (Card cardAttacking in cardsAttacking) {
                attackValue += cardAttacking.value;
                if(attackValue >= value) {
                    result.didDefend = false;
                    break;
                }
            }

            return result;
        }
    }

    public class Joker : Card {
        public Joker(CardColor color) : base(color, 0) { }

        public override CardDefenseResult Defend(List<Card> cardsAttacking) {
            CardDefenseResult result;
            result.topCard = this;
            result.didDefend = true;

            Card cardWithHighestValue = cardsAttacking[0];
            foreach (Card cardAttacking in cardsAttacking) {
                if(cardAttacking.value == value) {
                    result.didDefend = false;
                    break;
                }

                if(cardAttacking.value > cardWithHighestValue.value) {
                    cardWithHighestValue = cardAttacking;
                }
            }

            if(result.didDefend)
                result.topCard = cardWithHighestValue;

            return result;
        }
    }

    public class CardStack {
        private Player _owner;

        public Card TopCard { private set; get; }
        private List<Card> _deadCards;

        public bool IsHidden { get => _deadCards.Count == 0; }

        public CardStack(Player owner, Card topCard, List<Card> deadCards) {
            _owner = owner;

            TopCard = topCard;
            _deadCards = deadCards;
        }

        public CardStack(Player owner, Card topCard) {
            _owner = owner;

            TopCard = topCard;
            _deadCards = new List<Card>();
        }

        public void Defend(Player playerAttacking, List<Card> cardsAttacking) {

        }
    }
}