namespace Fold.Motor.Resources.Response;

public class GameStartedResponse
{
    public required CardColor Turn { set; get; }
    public required List<int> PickedCards { set; get; }
}

public class GameEndedResponse
{
    public enum Reason
    {
        AGREED = 0,
        RESIGN = 1,
        PASSING = 2,
        MATERIAL = 3,
        TIME = 4,
        REPORT = 5,
        ILLEGAL = 6
    }

    public Reason Way { set; get; }
    public CardColor Result { set; get; }
}