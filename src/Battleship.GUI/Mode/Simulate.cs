namespace Battleship.GUI
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Windows;

    using Battleship.Core;

    /// <summary>
    /// Simulate method.
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Simulates a number of games.
        /// </summary>
        /// <param name="numberOfGames">Number of games to simulate.</param>
        /// <param name="algorithm1">The ID of Player 1's algorithm.</param>
        /// <param name="algorithm2">The ID of Player 2's algorithm.</param>
        public void Simulate(int numberOfGames, int algorithm1, int algorithm2)
        {
            int sumMoves = 0;
            int player1 = 0;
            int player2 = 0;
            List<dynamic> gameMoves = new List<dynamic>();
            decimal totalTime = 0;

            for (int i = 0; i < numberOfGames; i++)
            {
                Stopwatch elapsedTime = new Stopwatch();
                elapsedTime.Start();

                Game game = new Game();
                game.CreateGame(algorithm1, algorithm2);

                elapsedTime.Stop();

                gameMoves.Add(game.Moves);

                sumMoves += game.Moves;

                if (game.Winner == true)
                {
                    player1++;
                }
                else
                {
                    player2++;
                }

                totalTime += decimal.Divide(elapsedTime.ElapsedMilliseconds, 1000);
            }

            TimeSpan avg = TimeSpan.FromSeconds((double)decimal.Divide(totalTime, numberOfGames));
            TimeSpan tot = TimeSpan.FromSeconds((double)totalTime);

            this.Simulate_Stats.Visibility = Visibility.Visible;

            this.Simulate_Stats.Text = $"Board Dimension: {Settings.GridHeight}x{Settings.GridWidth}\nWhite won: {player1}\nBlack won: {player2}\n\nStatistics\nMinimum: {gameMoves.Min()}\nMaximum: {gameMoves.Max()}\nAverage: {decimal.Divide(sumMoves, numberOfGames)}\nMedian: {Program.Median(gameMoves)}\nMode: {Program.Mode(gameMoves)}\n\nTime\nTotal time elapsed: {tot.Hours} hours {tot.Minutes} minutes {tot.Seconds} seconds {tot.Milliseconds} milliseconds\nAverage time elapsed: {avg.Hours} hours {avg.Minutes} minutes {avg.Seconds} seconds {avg.Milliseconds} milliseconds";
        }

        public void Simulate_SubmitButton_OnClick()
        {
            this.Simulate_SP.Visibility = Visibility.Hidden;

            string numS = this.Simulate_GamesText.Text;

            int algo1 = this.Simulate_Algorithm1.SelectedIndex;
            int algo2 = this.Simulate_Algorithm2.SelectedIndex;

            if (!int.TryParse(numS, out int num))
            {
                this.Simulate_Status.Content = "Number of games must be an integer!";
            }

            if (num > 0)
            {
                this.Simulate(num, algo1, algo2);
            }
            else
            {
                this.Simulate_Status.Content = "Number of games must be a positive integer!";
            }
        }
    }
}
