using System;
using Fold.Motor;

var ended = false;
var turn = 0;

void NextTurn() {
    turn++;
    turn %= 2;
}

void OnTimeLost() {
    ended = true;
    Console.WriteLine("---------------------------");
    Console.WriteLine(String.Format("Player {0} lost on time", turn + 1));
}

double interval = 120000, increment = 3000;

List<PlayerTimer> playerTimers = new() {
    new PlayerTimer(interval, increment, OnTimeLost),
    new PlayerTimer(interval, increment, OnTimeLost)
};

Console.WriteLine("Starting test...");

while (!ended)
{
    var playerTimer = playerTimers[turn];
    playerTimer.Enable();

    await Task.Delay(new Random().Next(25000) + 2000);

    playerTimer.Disable();

    Console.WriteLine("---------------------------");
    Console.WriteLine(String.Format("End of player {0}'s turn.", turn + 1));
    Console.WriteLine(String.Format("Due time: {0}, time left: {1}", playerTimer.DueTime.ToUniversalTime(), playerTimer.TimeLeft));

    NextTurn();
}