using System.Diagnostics;

namespace Fold {    
    public class Player {
        public readonly int id;
        public readonly CardColor color;

        private Stopwatch _stopwatch;
        private int _actionCount;

        public bool DidAnIllegalAction { private set; get; }

        public Player(int id, CardColor cardColor) {
            this.id = id;
            this.color = cardColor;

            _stopwatch = new Stopwatch();
            _actionCount = 0;

            DidAnIllegalAction = false;
        }

        public void RegisterAction(bool didAnIllegalAction) {
            _actionCount++;
            DidAnIllegalAction = didAnIllegalAction;
        }

        public long GetTimeLeftInMilliseconds(Game game) {
            return _actionCount * game.increment + game.time - _stopwatch.ElapsedMilliseconds;
        }
    }
}