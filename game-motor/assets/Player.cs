using Fold.Motor.Timers;
using Fold.Motor;

namespace Fold;   
  
public class Player {
    public readonly int id;
    public readonly CardColor color;

    public PlayerTimer Timer { private set; get; }
    public int ActionCount { private set; get; }

    public bool DidAnIllegalAction { private set; get; }

    public Player(int id, CardColor cardColor, double interval, double increment, PlayerTimer.OnTimeLost onTimeLostGameResolution) {
        this.id = id;
        this.color = cardColor;
        this.Timer = new PlayerTimer(interval, increment, onTimeLostGameResolution);

        DidAnIllegalAction = false;
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
}