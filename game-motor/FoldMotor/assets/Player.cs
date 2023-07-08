using Fold.Motor.Timers;
using Fold.Motor;

namespace Fold {    
    public class Player {
        public readonly int id;
        public readonly CardColor color;

        private PlayerTimer _timer;
        public int ActionCount { private set; get; }

        public bool DidAnIllegalAction { private set; get; }

        public Player(int id, CardColor cardColor, double interval, double increment, PlayerTimer.OnTimeLost onTimeLost) {
            this.id = id;
            this.color = cardColor;
            this._timer = new PlayerTimer(interval, increment, onTimeLost);

            DidAnIllegalAction = false;
        }

        public void StartTurn() {
            if(ActionCount == 0)
                return;

            _timer.Enable();
        }

        public void EndTurn(bool didAnIllegalAction) {
            DidAnIllegalAction = didAnIllegalAction;
            
            ActionCount++;
            
            if(ActionCount == 0)
                return;

            _timer.Disable();
        }

        public void AddTime(double increment) => _timer.AddTime(increment);

        public double GetTimeLeft() {
            return _timer.TimeLeft;
        }
    }
}