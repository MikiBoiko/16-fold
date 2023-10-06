using Fold.Motor;

namespace Fold.Server.Resources;

public class StateResource
{
    public CardColor PlayingColor { set; get; }
    public Game.State? State { set; get; }
}