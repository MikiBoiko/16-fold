namespace Fold.Motor.Model; 

public struct CardDefense {
    public Card topCard;
    public enum Result { won = 0, lost = 1, draw = 2 };
    public Result result;
}

public abstract class Card {
    public readonly CardColor color;
    public readonly int value;

    public int CombatValue => value / 2;

    public Card(CardColor color, int value) {
        this.color = color;
        this.value = value;
    }

    public abstract CardDefense Defend(List<Card> cardsAttacking);
}

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

    #region State
    public class State
    {
        public CardColor OwnerColor { set; get; }
        public int Value { set; get; }
    }

    public State? GetState()
    {
        if(IsHidden)
            return null;
            
        return new State {
            OwnerColor = OwnerColor,
            Value = Card.value
        };
    }
    #endregion
}
