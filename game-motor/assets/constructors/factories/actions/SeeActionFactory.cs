using Fold.Motor.Model.Actions;

namespace Fold.Motor.Constructors.Factories.Actions;

public class SeeActionFactory : IFactory<Model.Action>
{
    public Model.Action Instantiate(Dictionary<string, object> data)
    {
        if (!data.ContainsKey("from"))
            throw new FactoryParseException("Action request doesn't contain key \"from\".");

        return new SeeAction(
            new Model.BoardPosition((string)data["from"])
        );
    }
}