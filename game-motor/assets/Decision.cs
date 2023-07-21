namespace Fold;

public abstract class Decision
{
    public Decision() { }

    public abstract IDecisionResolution DoDecision(Player player, Game game);
}

public class AddTimeDecision : Decision
{
    public override IDecisionResolution DoDecision(Player player, Game game)
    {
        // TODO : for now hardcoded time added on decision
        double increment = 15000;
        Player other = game.OtherPlayer(player);
        other.Timer.AddTime(increment);
        return new AddTimeResolution(other.id, other.GetTimeLeft());
    }
}

public class ResignDecision : Decision
{
    public override IDecisionResolution DoDecision(Player player, Game game)
    {
        Player other = game.OtherPlayer(player);
        GameResolution resolution = new GameResolution(
            other.color,
            GameResolution.Reason.RESIGN
        );
        game.SetGameResolution(resolution);
        return new ResignResolution(player.id);
    }
}

public class ReportIllegalMoveDecision : Decision
{
    public override IDecisionResolution DoDecision(Player player, Game game)
    {
        Player other = game.OtherPlayer(player);
        GameResolution resolution = other.DidAnIllegalAction ?
            new GameResolution(
                player.color,
                GameResolution.Reason.ILLEGAL
            )
            :
            new GameResolution(
                other.color,
                GameResolution.Reason.REPORT
            );

        game.SetGameResolution(resolution);
        return new ReportIllegalMoveResolution(player.id);
    }
}


#region Resolutions
public interface IDecisionResolution { }

public class AddTimeResolution : IDecisionResolution
{
    private int _id;
    private double _time;

    public AddTimeResolution(int id, double time)
    {
        if (time <= 0)
            throw new NegativeTimeException();

        _id = id;
        _time = time;
    }

    public override string ToString()
    {
        return String.Format("+t.{0}.{1}", _id, _time);
    }
}

public class ResignResolution : IDecisionResolution
{
    private int _id;

    public ResignResolution(int id)
    {
        _id = id;
    }

    public override string ToString()
    {
        return String.Format("r.{0}", _id);
    }
}

public class ReportIllegalMoveResolution : IDecisionResolution
{
    private int _id;

    public ReportIllegalMoveResolution(int id)
    {
        _id = id;
    }

    public override string ToString()
    {
        return String.Format("rim.{0}", _id);
    }
}

public class NegativeTimeException : Exception { }
#endregion
