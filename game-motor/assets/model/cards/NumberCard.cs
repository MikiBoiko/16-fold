namespace Fold.Motor.Model.Cards;

public class NumberCard : Card {
    public NumberCard(CardColor color, int value) : base(color, value) { }

    public override CardDefense Defend(List<Card> cardsAttacking) {
        CardDefense result;
        result.topCard = this;
        result.result = CardDefense.Result.won;

        int attackValue = 0;
        foreach (Card cardAttacking in cardsAttacking) {
            attackValue += cardAttacking.CombatValue;
            if(attackValue == CombatValue)
                result.result = CardDefense.Result.draw;
            if(attackValue > CombatValue) {
                result.result = CardDefense.Result.lost;
                break;
            }
        }

        return result;
    }
}