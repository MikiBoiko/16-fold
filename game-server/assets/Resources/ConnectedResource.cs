using Fold.Motor;

namespace Fold.Server.Resources;

public class ConnectedResource
{
    public CardColor PlayingColor { set; get; }
    public Game.State? State { set; get; }
}