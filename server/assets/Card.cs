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
        private List<Card> _cards;

        public bool IsHidden { get => _cards.Count == 1; }

        public CardStack(Player owner, List<Card> cards) {
            _owner = owner;
            _cards = cards;
        }

        public void Defend(Player playerAttacking, List<Card> cardsAttacking) {

        }
    }
}