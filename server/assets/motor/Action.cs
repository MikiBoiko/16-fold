namespace Fold {
    public abstract class Action {
        public abstract void DoAction(Player player, Board board);
    }

    public class TestAction : Action {
        public override void DoAction(Player player, Board board) {
            Console.WriteLine("YES");
        }
    }

    public class MoveAction : Action {
        private BoardPosition _from;
        private BoardPosition _to;

        public MoveAction(BoardPosition from, BoardPosition to) {
            if(!BoardPosition.AreAdjacent(from, to)) {
                throw new NotAdjecentException();
            }

            _from = from;
            _to = to;
        }

        public override void DoAction(Player player, Board board) {
            throw new NotImplementedException();
        }
    }

    public class AttackAction : Action {
        public AttackAction(List<BoardPosition> attackingPositions, BoardPosition defendingPosition) {
            
        }

        public override void DoAction(Player player, Board board) {
            throw new NotImplementedException();
        }
    }
}