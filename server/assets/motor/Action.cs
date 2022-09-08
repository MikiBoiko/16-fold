using System;

namespace Fold {
    public abstract class Action {
        public abstract ActionResolution DoAction(Player player, Board board);
    }
    public class SeeAction : Action {
        private BoardPosition _from;

        public SeeAction(BoardPosition from) {
            _from = from;
        }

        public override ActionResolution DoAction(Player player, Board board) {
            // TODO : implement
            return new ActionResolution(player.color);
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

        public override ActionResolution DoAction(Player player, Board board) => board.Move(_from, _to);
    }
    
    public class MoveToWinAction : Action {
        private BoardPosition _from;
        
        public MoveToWinAction(BoardPosition from) {
            _from = from;
        }
        
        public override ActionResolution DoAction(Player player, Board board) => board.CheckForWin(player.color, _from);
    }

    public class AttackAction : Action {
        private List<BoardPosition> _from;
        private BoardPosition _to;

        public AttackAction(List<BoardPosition> from, BoardPosition to) {
            if(from.Count == 0)
                throw new EmptyPositionListException();

            _from = from;
            _to = to;
        }

        public override ActionResolution DoAction(Player player, Board board) => board.Attack(_from, _to);
    }

    public class EmptyPositionListException : Exception { }
}