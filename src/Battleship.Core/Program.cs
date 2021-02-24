namespace Battleship.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
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
        /// <param name="numberOfGames">Number of games to simulate.</param>
        public static void Simulate(int numberOfGames)
        {
            decimal totalTime = 0;

            string fileContent = string.Empty;

            List<dynamic> gameMoves = new List<dynamic>();

            for (int i = 0; i < numberOfGames; i++)
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
                Console.WriteLine($"Percent complete: {decimal.Divide(i + 1, numberOfGames) * 100}\nCurrent Dimension: {Settings.GridHeight}x{Settings.GridWidth}\nWhite won: {player1}\nBlack won: {player2}\n\nStatistics\nMinimum: {gameMoves.Min()}\nMaximum: {gameMoves.Max()}\nAverage: {decimal.Divide(sumMoves, i + 1)}\nMedian: {Median(gameMoves)}\nMode: {Mode(gameMoves)}\n\nTime\nTotal time elapsed: {tot.Hours} hours {tot.Minutes} minutes {tot.Seconds} seconds {tot.Milliseconds} milliseconds\nAverage time elapsed: {avg.Hours} hours {avg.Minutes} minutes {avg.Seconds} seconds {avg.Milliseconds} milliseconds");
            }

            //using (StreamWriter outputFile = File.AppendText("HTTest1.csv"))
            //{
            //    outputFile.WriteLine(fileContent);
            //}
        }

        /// <summary>
        /// Finds the best square to search.
        /// </summary>
        /// <param name="player">The grid to analyse.</param>
        /// <returns>The square to search and the probability of it having a ship on it.</returns>
        public static (Square, Dictionary<int, int>) FindBestSquare(Grid player)
        {
            Dictionary<int, int> prob = new Dictionary<int, int>();

            int sunkSquares = 0;
            int sunkShips = 0;

            foreach (Square sq in player.Squares)
            {
                if (sq.IsSunk == true)
                {
                    sunkSquares++;
                }
            }

            foreach (Ship ship in player.OriginalShips)
            {
                if (ship.IsSunk == true)
                {
                    sunkShips += ship.Length;
                }
            }

            if (sunkShips != sunkSquares)
            {
                throw new Exception("Sunk squares do not match");
            }

            Square attackedSq;

            /*
             * DEFINITE
             * 1. If a 'hit' square is surrounded by misses in 3 directions, there must be a ship pointing in the fourth direction.
             * 2. If a certain spot on the board could only hold one certain ship, no other ships can cross any of those squares.

             * DISQUALIFY
             * 3. Misses and sunk ships are obstructions
             * 4. Ships that are not sunk can't be located entirely on 'hit' squares.
             */

            // 1.
            foreach (Square sq in player.Squares)
            {
                if (sq.BeenSearched && sq.HadShip == true && sq.IsSunk != true)
                {
                    List<Square> potentialSqs = new List<Square>();

                    foreach (Square square in sq.GetAdjacentSquares())
                    {
                        // is a valid square if it has not been searched
                        if (!sq.BeenSearched)
                        {
                            potentialSqs.Add(sq);
                        }
                    }

                    if (potentialSqs.Count == 1)
                    {
                        player.ToAttack.Add(potentialSqs.First());
                    }
                }
            }

            // 2.
            foreach (Ship ship in player.Ships)
            {
                List<List<int>> arrL = ship.GetArrangements();

                if (arrL.Count == 1)
                {
                    foreach (int sqID in arrL.First())
                    {
                        Square sq = player.Squares[sqID];

                        if (!sq.BeenSearched && !player.ToAttack.Contains(sq))
                        {
                            player.ToAttack.Add(sq);
                        }
                    }
                }
            }

            for (int i = 0; i < (Settings.GridHeight * Settings.GridWidth); i++)
            {
                prob.Add(i, 0);
            }

            if (player.ToAttack.Any())
            {
                foreach (Square square in player.ToAttack)
                {
                    prob[square.ID] = 100;
                }

                attackedSq = HighestHCS(player.ToAttack.ToList()).First();
            }
            else
            {
                Dictionary<int, double> probability = new Dictionary<int, double>();

                // domain: unsearched squares
                if (!player.ToSearch.Any())
                {
                    List<Square> l = HighestHCS(player.UnsearchedSquares);

                    foreach (Square sq in l)
                    {
                        probability.Add(sq.ID, 0);
                    }

                    foreach (Ship ship in player.Ships)
                    {
                        List<List<int>> arrL = ship.GetArrangements();
                        foreach (List<int> arr in arrL)
                        {
                            foreach (int sqID in arr)
                            {
                                if (l.Contains(player.Squares[sqID]))
                                {
                                    probability[sqID] += (double)decimal.Divide(1, arrL.Count);
                                    prob[sqID]++;
                                }
                            }
                        }
                    }
                }

                // domain: squares adjacent to hit squares
                else
                {
                    List<Square> l = HighestHCS(player.ToSearch.ToList());

                    foreach (Square sq in l)
                    {
                        probability.Add(sq.ID, 0);
                    }

                    foreach (Ship ship in player.Ships)
                    {
                        List<List<int>> arrL = ship.GetArrangements();
                        foreach (List<int> arr in arrL)
                        {
                            foreach (int sqID in arr)
                            {
                                if (l.Contains(player.Squares[sqID]))
                                {
                                    probability[sqID]++;
                                    prob[sqID]++;
                                }
                            }
                        }
                    }
                }

                double[] source = probability.Values.ToArray();
                int i = 0;
                double[][] result = source.GroupBy(s => i++ / 10).Select(g => g.ToArray()).ToArray();

                attackedSq = player.Squares[(int)probability.Aggregate((l, r) => l.Value > r.Value ? l : r).Key];
            }

            player.SearchedSquares.Add(attackedSq);
            player.ToAttack.Remove(attackedSq);
            player.ToSearch.Remove(attackedSq);
            attackedSq.BeenSearched = true;

            int sum = prob.Values.Sum();
            Dictionary<int, int> res = new Dictionary<int, int>();

            for (int i = 0; i < (Settings.GridWidth * Settings.GridHeight); i++)
            {
                res.Add(i, 0);
            }

            foreach (int key in prob.Keys)
            {
                res[key] = prob[key];
            }

            return (attackedSq, res);
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
        /// Finds best square.
        /// </summary>
        public static void Find()
        {
            List<Ship> shipList = new List<Ship>
            {
                new Ship(0, 5, 1),
                new Ship(1, 4, 1),
                new Ship(2, 3, 1),
                new Ship(3, 3, 1),
                new Ship(4, 2, 1),
            };

            Grid player = new Grid();
            foreach (Ship ship in shipList)
            {
                Ship sp = new Ship(player, ship.Length, ship.Breadth);

                player.OriginalShips.Add(sp);
                player.Ships.Add(sp);
            }

            while (player.Ships.Count > 0)
            {
                Console.Clear();
                (Square sq, Dictionary<int, int> e) = FindBestSquare(player);
                player.UnsearchedSquares.Remove(sq);
                Console.WriteLine($"Square Coordinates: {sq.ToCoor()}");
                Console.WriteLine($"Probability: {e.Values.Max()}%");
                Console.WriteLine("M / H / S");

                player.ToSearch.Clear();
                foreach (Square sq1 in player.Squares)
                {
                    if (!sq1.BeenSearched)
                    {
                        foreach (Square adjSq in sq1.GetAdjacentSquares())
                        {
                            if (adjSq.IsHit == true)
                            {
                                player.ToSearch.Add(sq1);
                                break;
                            }
                        }
                    }
                }

                string ans = Console.ReadLine();
                switch (ans)
                {
                    case "M":
                        player.Squares[sq.ID].IsMiss = true;
                        break;
                    case "H":
                        player.Squares[sq.ID].IsHit = true;
                        player.Squares[sq.ID].HadShip = true;
                        break;
                    case "S":
                        Console.Write("ID of ship: ");
                        int id = int.Parse(Console.ReadLine());
                        Console.WriteLine("Squares sunk: ");
                        string raw = Console.ReadLine();
                        string[] xy = raw.Replace(")", string.Empty).Split(",(");
                        foreach (Ship ship in player.Ships.ToList())
                        {
                            if (ship.ID == id)
                            {
                                foreach (string xys in xy)
                                {
                                    string s = xys.Replace("(", string.Empty);
                                    int x = int.Parse(s.Split(",")[0]);
                                    int y = int.Parse(s.Split(",")[1]);

                                    int sqID1 = ((y - 1) * Settings.GridWidth) + x - 1;

                                    ship.OriginalOccupiedSquares.Add(player.Squares[sqID1]);
                                }

                                foreach (Square square in ship.OriginalOccupiedSquares)
                                {
                                    square.IsSunk = true;
                                    square.HadShip = true;
                                    square.IsHit = false;
                                }

                                ship.IsSunk = true;
                                player.Ships.Remove(ship);
                            }
                        }

                        break;
                }
            }
        }

        /// <summary>
        /// The entry point of the program.
        /// </summary>
        public static void Main()
        {
            Simulate(1000);
        }

        /// <summary>
        /// Gets the mode of a list.
        /// </summary>
        /// <param name="list">A list.</param>
        /// <returns>The mode of the list.</returns>
        public static dynamic? Mode(List<dynamic> list)
        {
            return list
                .GroupBy(x => x)
                .OrderByDescending(x => x.Count()).ThenBy(x => x.Key)
                .Select(x => (int?)x.Key)
                .FirstOrDefault();
        }

        /// <summary>
        /// Gets the median of a list.
        /// </summary>
        /// <param name="list">A list.</param>
        /// <returns>The median of the list.</returns>
        public static decimal Median(List<dynamic> list)
        {
            var ys = list.OrderBy(x => x).ToList();
            double mid = (ys.Count - 1) / 2.0;
            return (ys[(int)mid] + ys[(int)(mid + 0.5)]) / 2;
        }

        /// <summary>
        /// Gets the square with the most hit connected squares (HCS) from a list.
        /// </summary>
        /// <param name="list">The list of squares.</param>
        /// <returns>The square with the highest HCS.</returns>
        public static List<Square> HighestHCS(List<Square> list)
        {
            Dictionary<Square, int> d = new Dictionary<Square, int>();

            foreach (Square sq in list)
            {
                d.Add(sq, sq.GetNumberOfHitConnectedSquares());
            }

            return d.Where(pair => pair.Value == d.Values.Max())
                 .Select(pair => pair.Key).ToList();
        }
    }
}
