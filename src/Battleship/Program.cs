namespace Battleship
{
    using System;
    using System.Diagnostics;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Timers;
    using System.Text.RegularExpressions;
    using System.Text.Json;

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
        public static void Run()
        {
            decimal totalTime = 0;

            string fileContent = string.Empty;

            for (int i = 0; i < 100; i++)
            {
                string winner = string.Empty;

                Stopwatch elapsedTime = new Stopwatch();
                elapsedTime.Start();

                Game game = new Game();
                game.CreateGame();

                elapsedTime.Stop();

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

                fileContent += $"{Settings.gridHeight},{Settings.gridWidth},Random,{winner},{game.Move}\n";

                totalTime += decimal.Divide(elapsedTime.ElapsedMilliseconds, 1000);

                TimeSpan avg = TimeSpan.FromSeconds((double)decimal.Divide(totalTime, i + 1));
                TimeSpan tot = TimeSpan.FromSeconds((double)totalTime);

                Console.Clear();
                Console.WriteLine($"Percent complete: {decimal.Divide(i + 1, 100) * 100}%\nCurrent Dimension: {Settings.gridHeight}x{Settings.gridWidth}\nWhite won: {player1}\nBlack won: {player2}\nAverage moves: {decimal.Divide(sumMoves, i + 1)}\nTotal time elapsed: {tot.Hours} hours {tot.Minutes} minutes {tot.Seconds} seconds {tot.Milliseconds} milliseconds\nAverage time elapsed: {avg.Hours} hours {avg.Minutes} minutes {avg.Seconds} seconds {avg.Milliseconds} milliseconds");
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

            delayTimer = new Timer();
            delayTimer.Interval = time;
            delayTimer.AutoReset = false;
            delayTimer.Elapsed += (s, args) => i = 1;
            delayTimer.Start();

            while (i == 0)
            { };
        }

        public static void Main()
        {
            Run();
        }

        public static string SpliceText(string text, int lineLength)
        {
            return Regex.Replace(text, "(.{" + lineLength + "})", "$1" + Environment.NewLine);
        }
    }
}
