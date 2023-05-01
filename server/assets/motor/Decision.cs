namespace Fold.Motor {
    public abstract class Decision {
        public Decision() { }

        public abstract void DoDecision(Player player, Game game);
    }

    public class AddTimeDecision : Decision {
        public override void DoDecision(Player player, Game game) {
            throw new NotImplementedException();
        }
    }


    #region Resolutions
    public interface DecisionResolution { }

    public class AddTimeResolution : DecisionResolution {
        private int _id;
        private long _time; 

        public AddTimeResolution(int id, long time) {
            if(time <= 0)
                throw new NegativeTimeException();
            
            _id = id;
            _time = time;
        }

        public override string ToString() {
            return String.Format("+t.{0}.{1}", _id, _time);
        }
    }

    public class ResignResolution : DecisionResolution {
        private int _id;

        public ResignResolution(int id) {            
            _id = id;
        }

        public override string ToString() {
            return String.Format("r.{0}", _id);
        }
    }

    public class NegativeTimeException : Exception { }
    #endregion
}