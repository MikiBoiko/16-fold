namespace Fold
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
            Console.WriteLine("Time (in minutes): ");
            float time = float.Parse(Console.ReadLine() ?? "5");

            Console.WriteLine("Increment (in seconds): ");
            float increment = float.Parse(Console.ReadLine() ?? "3");
            
            Game game = new Game(
                new Player(1, CardColor.red), 
                new Player(2, CardColor.black), 
                time, 
                increment
            );
            */

            Game game = new Game(
                new Player(1, CardColor.red), 
                new Player(2, CardColor.black), 
                5 * 6000, 
                3 * 100
            );

            game.InitializeGame();

            game.DoAction(2, new TestAction());

            game.DoAction(1, new MoveAction(new BoardPosition("a3"), new BoardPosition("a4")));

            game.DoAction(
                2,
                new AttackAction(
                    new List<BoardPosition>{
                        new BoardPosition("a5"),
                        new BoardPosition("b5")
                    }, 
                    new BoardPosition("a4")
                )
            );

            game.PrintBoard();
            
            //game.Restart(true);
        }
    }
}