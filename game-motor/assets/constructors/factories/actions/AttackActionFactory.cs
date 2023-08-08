using Fold.Motor.Model.Actions;

namespace Fold.Motor.Constructors.Factories.Actions;

public class AttackActionFactory : IFactory<Model.Action>
{
    public Model.Action Instantiate(Dictionary<string, object?> data)
    {
        if (!data.ContainsKey("from"))
            throw new FactoryParseException("Action request doesn't contain key \"from\".");

        if (!data.ContainsKey("to"))
            throw new FactoryParseException("Action request doesn't contain key \"from\".");

        if (data["from"] == null)
            throw new ArgumentNullException();
            
        if (data["to"] == null)
            throw new ArgumentNullException();

        object[] fromPositions = (object[])(data["from"] ?? new());
        List<Model.BoardPosition> fromBoardPosition = new();

        foreach (string fromPosition in fromPositions)
        {
            fromBoardPosition.Add(new Model.BoardPosition(fromPosition.ToString()));
        }

        return new AttackAction(
            fromBoardPosition,
            new Model.BoardPosition((data["to"] ?? new()).ToString())
        );
    }
}