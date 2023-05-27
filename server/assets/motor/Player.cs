using System.Timers;

namespace Fold.Motor {    
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

        public void EndTurn(bool didAnIllegalAction, long increment) {
            DidAnIllegalAction = didAnIllegalAction;
            
            ActionCount++;
            
            if(ActionCount == 0)
                return;

            _timer.Disable();
        }

        public double GetTimeLeft(Game game) {
            return _timer.TimeLeft;
        }
    }
}