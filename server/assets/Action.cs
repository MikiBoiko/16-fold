namespace Fold {
    public abstract class Action {
        public Action() { }

        public abstract void DoAction(Player player, Board board);
    }
}