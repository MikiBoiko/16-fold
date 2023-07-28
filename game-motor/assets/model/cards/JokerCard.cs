namespace Fold.Motor.Model.Cards;

public class JokerCard : Card {
    public JokerCard(CardColor color) : base(color, 0) { }

    public override CardDefense Defend(List<Card> cardsAttacking) {
        CardDefense result;
        result.topCard = this;
        result.result = CardDefense.Result.won;

        Card cardWithHighestValue = cardsAttacking[0];
        foreach (Card cardAttacking in cardsAttacking) {
            if(cardAttacking.CombatValue == CombatValue) {
                result.result = CardDefense.Result.lost;
                break;
            }

            if(cardAttacking.CombatValue > cardWithHighestValue.CombatValue) {
                cardWithHighestValue = cardAttacking;
            }
        }

        if(result.result == CardDefense.Result.won)
            result.topCard = cardWithHighestValue;

        return result;
    }
}