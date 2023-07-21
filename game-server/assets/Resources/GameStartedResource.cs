using Fold;
using Fold.Motor;

namespace FoldServer.Resources;

public class GameStartedResource
{
    public CardColor Turn { set; get; }
    public List<int> PickedCards { set; get; }
}