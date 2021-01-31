﻿namespace Battleships
{
    using System;
    using System.Timers;

    /// <summary>
    /// Main class of the project.
    /// </summary>
    public class Program
    {
        private static Timer delayTimer;

        private static int sumMoves = 0;

        private static int player1 = 0;

        private static int player2 = 0;

        /// <summary>
        /// Main method of the project.
        /// </summary>
        public static void Main()
        {
            for (int i = 1; i < 10000; i++)
            {
                Game game = new Game();

                game.CreateGame();
                sumMoves += game.Move;
                if (game.Winner == true)
                {
                    player1++;
                }
                else
                {
                    player2++;
                }

                Console.Clear();
                Console.WriteLine($"Number of games: {i}\nWhite won: {player1}\nBlack won: {player2}\nAverage number of moves: {sumMoves / i}");
            }
        }

        /// <summary>
        /// Delays a task by a certain amount of seconds.
        /// </summary>
        /// <param name="time">The number of seconds to delay.</param>
        public static void Delay(double time)
        {
            int i = 0;

            delayTimer = new System.Timers.Timer();
            delayTimer.Interval = time;
            delayTimer.AutoReset = false;
            delayTimer.Elapsed += (s, args) => i = 1;
            delayTimer.Start();

            while (i == 0)
            { };
        }
    }
}
