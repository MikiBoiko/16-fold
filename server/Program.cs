using Fold.Motor;
using Fold.Socket;

namespace Fold
{
    class Program
    {
        static async Task Main(string[] args)
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
                1,
                2,
                1000,
                3 * 100,
                (GameResolution resolution) => {
                    Console.WriteLine(
                        String.Format(
                            "RESULT : {0}, {1}", 
                            resolution.reason,
                            resolution.result
                        )
                    );
                }
            );

            game.InitializeGame();

            game.DoAction(1, new MoveAction(new BoardPosition("a3"), new BoardPosition("a4")));
            game.DoAction(
                2,
                new AttackAction(
                    new List<BoardPosition> {
                        new BoardPosition("a5"),
                        new BoardPosition("b5")
                    }, 
                    new BoardPosition("a4")
                )
            );


            game.PrintBoard();
            game.DoAction(1, new MoveAction(new BoardPosition("a4"), new BoardPosition("a5")));
            game.DoAction(2, new MoveAction(new BoardPosition("a7"), new BoardPosition("b6")));
            game.PrintBoard();
            await Task.Delay(3000);
            game.DoAction(1, new MoveAction(new BoardPosition("a5"), new BoardPosition("a6")));
            game.DoAction(2, new SeeAction(new BoardPosition("b6")));
            game.PrintBoard();
            game.DoAction(1, new MoveAction(new BoardPosition("a6"), new BoardPosition("a7")));
            game.DoAction(
                2,
                new AttackAction(
                    new List<BoardPosition> {
                        new BoardPosition("b7"),
                    }, 
                    new BoardPosition("a7")
                )
            );
            game.PrintBoard();
            game.DoAction(1, new MoveToWinAction(new BoardPosition("a7")));
            //game.DoAction(2, new SeeAction(new BoardPosition("b6")));
            game.PrintBoard();
            game.Restart(true);
            game.PrintBoard();
        }
    }
}