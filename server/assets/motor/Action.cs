using System;

namespace Fold {
    public abstract class Action {
        protected abstract CardColor OnAction(Player player, Board board);
        public void DoAction(Player player, Board board) => player.RegisterAction(
            didAnIllegalAction: player.color != OnAction(player, board)
        );
    }
    public class TestAction : Action {
        protected override CardColor OnAction(Player player, Board board) {
            Console.WriteLine("YES");
            return player.color;
        }
    }
    public class MoveAction : Action {
        private BoardPosition _from;
        private BoardPosition _to;

        public MoveAction(BoardPosition from, BoardPosition to) {
            if(!BoardPosition.AreAdjacent(from, to))
                throw new NotAdjecentException();

            _from = from;
            _to = to;
        }

        protected override CardColor OnAction(Player player, Board board) => board.Move(_from, _to);
    }
    
    public class MoveToWinAction : Action {
        private BoardPosition _from;
        
        public MoveToWinAction(BoardPosition from) {
            _from = from;
        }
        
        protected override CardColor OnAction(Player player, Board board) => board.CheckForWin(player.color, _from);
    }

// TODO : EMPTY INPUT
    public class AttackAction : Action {
        private List<BoardPosition> _from;
        private BoardPosition _to;

        public AttackAction(List<BoardPosition> from, BoardPosition to) {
            if(from.Count == 0)
                throw new NoEmptyPositionListException();

            _from = from;
            _to = to;
        }

        protected override CardColor OnAction(Player player, Board board) => board.Attack(_from, _to);
    }

    public class NoEmptyPositionListException : Exception { }
}