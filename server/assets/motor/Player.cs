using System.Timers;

namespace Fold.Motor {    
    public class Player {
        public readonly int id;
        public readonly CardColor color;

        private System.Timers.Timer _timer;
        private int _actionCount;

        public bool DidAnIllegalAction { private set; get; }

        public Player(int id, CardColor cardColor, long time) {
            this.id = id;
            this.color = cardColor;

            _timer = new System.Timers.Timer();
            _timer.Interval = time;
            _actionCount = 0;

            DidAnIllegalAction = false;
        }

        public void StartTurn() {
            if(_actionCount > 0)
                _timer.Start();
        }

        public void EndTurn(bool didAnIllegalAction, long increment) {
            DidAnIllegalAction = didAnIllegalAction;
            
            _timer.Stop();
            _timer.Interval += increment;
            
            _actionCount++;
        }

        public long GetTimeLeft(Game game) {
            return (long)_timer.Interval;
        }
    }
}