namespace Fold.Motor.Model;   
  
public class Player {
    public readonly CardColor color;
    public readonly string username;
    public PlayerTimer Timer { private set; get; }
    public int ActionCount { private set; get; }
    public bool DidAnIllegalAction { private set; get; }

    public Player(CardColor cardColor, string username, double interval, double increment, PlayerTimer.OnTimeLost onTimeLostGameResolution) {
        this.color = cardColor;
        this.username = username;
        this.Timer = new PlayerTimer(interval, increment, onTimeLostGameResolution);

        Restart();
    }

    public void Restart()
    {
        DidAnIllegalAction = false;
        ActionCount = 0;
        Timer.Restart();
    }

    public void StartTurn() {
        if(ActionCount == 0)
            return;

        Timer.Enable();
    }

    public void EndTurn(bool didAnIllegalAction) {
        DidAnIllegalAction = didAnIllegalAction;
        
        ActionCount++;
        
        if(ActionCount == 0)
            return;

        Timer.Disable();
    }

    public double GetTimeLeft() {
        return Timer.TimeLeft;
    }

    #region State
    public class State
    {
        public string? Username { get; set; }
        public PlayerTimer.State? PlayerTimerState { get; set; }
    }

    public State GetState()
    {
        return new State {
            Username = username,
            PlayerTimerState = Timer.GetState(ActionCount > 0)
        };
    }
    #endregion
}