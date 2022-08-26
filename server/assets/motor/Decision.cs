namespace Fold {
    public abstract class Decision {
        public Decision() { }

        public abstract void DoDecision(Player player, Game game);
    }
}