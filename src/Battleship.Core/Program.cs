namespace Battleship.Core
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Collections.Generic;
    using System.IO;
    using System.Text.RegularExpressions;
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
        /// Simulates a number of games.
        /// </summary>
        /// <param name="simulations">Number of games to simulate.</param>
        public static void Run(int simulations)
        {
            decimal totalTime = 0;

            string fileContent = string.Empty;

            List<dynamic> gameMoves = new List<dynamic>();

            for (int i = 0; i < simulations; i++)
            {
                string winner = string.Empty;

                Stopwatch elapsedTime = new Stopwatch();
                elapsedTime.Start();

                Game game = new Game();
                game.CreateGame();

                elapsedTime.Stop();

                gameMoves.Add(game.Move);

                sumMoves += game.Move;
                if (game.Winner == true)
                {
                    winner = "Player 1";
                    player1++;
                }
                else
                {
                    winner = "Player 2";
                    player2++;
                }

                fileContent += $"{Settings.GridHeight},{Settings.GridWidth},Random,{winner},{game.Move}\n";

                totalTime += decimal.Divide(elapsedTime.ElapsedMilliseconds, 1000);

                TimeSpan avg = TimeSpan.FromSeconds((double)decimal.Divide(totalTime, i + 1));
                TimeSpan tot = TimeSpan.FromSeconds((double)totalTime);

                Console.Clear();
                Console.WriteLine($"Percent complete: {decimal.Divide(i + 1, simulations) * 100}\nCurrent Dimension: {Settings.GridHeight}x{Settings.GridWidth}\nWhite won: {player1}\nBlack won: {player2}\n\nStatistics\nMinimum: {gameMoves.Min()}\nMaximum: {gameMoves.Max()}\nAverage: {decimal.Divide(sumMoves, i + 1)}\nMedian: {Median(gameMoves)}\nMode: {Mode(gameMoves)}\n\nTime\nTotal time elapsed: {tot.Hours} hours {tot.Minutes} minutes {tot.Seconds} seconds {tot.Milliseconds} milliseconds\nAverage time elapsed: {avg.Hours} hours {avg.Minutes} minutes {avg.Seconds} seconds {avg.Milliseconds} milliseconds");
            }

            //using (StreamWriter outputFile = File.AppendText("HTTest1.csv"))
            //{
            //    outputFile.WriteLine(fileContent);
            //}
        }

        /// <summary>
        /// Delays a task by a certain amount of seconds.
        /// </summary>
        /// <param name="time">The number of seconds to delay.</param>
        public static void Delay(double time)
        {
            int i = 0;

            delayTimer = new Timer
            {
                Interval = time,
                AutoReset = false,
            };
            delayTimer.Elapsed += (s, args) => i = 1;
            delayTimer.Start();

            while (i == 0)
            { };
        }

        /// <summary>
        /// Main method of the project.
        /// </summary>
        public static void Main()
        {
            Run(100);
        }

        public static dynamic? Mode(List<dynamic> list)
        {
            return list
                .GroupBy(x => x)
                .OrderByDescending(x => x.Count()).ThenBy(x => x.Key)
                .Select(x => (int?)x.Key)
                .FirstOrDefault();
        }

        public static decimal Median(List<dynamic> xs)
        {
            var ys = xs.OrderBy(x => x).ToList();
            double mid = (ys.Count - 1) / 2.0;
            return (ys[(int)(mid)] + ys[(int)(mid + 0.5)]) / 2;
        }
    }
}
