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
        /// <param name="numberOfGames">Number of games to simulate.</param>
        public static void Simulation(int numberOfGames)
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

        public static (Square, int) FindBestSquare(Grid player)
        {
            int prob = 0;

            List<Square> ToSearch = new List<Square>();
            List<Square> ToAttack = new List<Square>();

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
                    List<int> sqID = new List<int>()
                    {
                        sq.ID - 1,
                        sq.ID + 1,
                        sq.ID - Settings.GridWidth,
                        sq.ID + Settings.GridWidth,
                    };

                    List<Square> potentialSqs = new List<Square>();

                    foreach (int id in sqID)
                    {
                        if (id > -1 && id < (Settings.GridHeight * Settings.GridWidth))
                        {
                            // is a valid square if it has not been searched
                            if (!player.Squares[id].BeenSearched)
                            {
                                potentialSqs.Add(player.Squares[id]);
                            }
                        }
                    }

                    if (potentialSqs.Count == 1)
                    {
                        ToAttack.Add(potentialSqs.First());
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

                        if (!sq.BeenSearched && !ToAttack.Contains(sq))
                        {
                            ToAttack.Add(sq);
                        }
                    }
                }
            }

            if (ToAttack.Any())
            {
                attackedSq = ToAttack.First();
                prob = 100;
            }
            else
            {
                Dictionary<int, int> probability = new Dictionary<int, int>();

                if (!ToSearch.Any())
                {
                    foreach (Square sq in player.UnsearchedSquares)
                    {
                        probability.Add(sq.ID, 0);
                    }

                    foreach (Ship ship in player.Ships)
                    {
                        foreach (List<int> arr in ship.GetArrangements())
                        {
                            foreach (int sqID in arr)
                            {
                                if (!player.Squares[sqID].BeenSearched)
                                {
                                    probability[sqID]++;
                                }
                            }
                        }
                    }
                }
                else
                {
                    foreach (Square sq in ToSearch)
                    {
                        probability.Add(sq.ID, 0);
                    }

                    foreach (Ship ship in player.Ships)
                    {
                        foreach (List<int> arr in ship.GetArrangements())
                        {
                            foreach (int sqID in arr)
                            {
                                if (ToSearch.Contains(player.Squares[sqID]))
                                {
                                    probability[sqID]++;
                                }
                            }
                        }
                    }
                }

                int[] source = probability.Values.ToArray();
                int i = 0;
                int[][] result = source.GroupBy(s => i++ / 10).Select(g => g.ToArray()).ToArray();

                attackedSq = player.Squares[probability.Aggregate((l, r) => l.Value > r.Value ? l : r).Key];
                prob = probability[attackedSq.ID] / probability.Values.ToList().Sum() * 100;
            }

            return (attackedSq, prob);
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
            List<Ship> shipList = new List<Ship>
            {
                new Ship(5, 0),
                new Ship(4, 1),
                new Ship(3, 2),
                new Ship(3, 3),
                new Ship(2, 4),

            };

            Grid player = new Grid();
            foreach (Ship ship in shipList)
            {
                Ship sp = new Ship(player, ship.ID);
                player.Ships.Add(sp);
                player.OriginalShips.Add(sp);
            }

            while (player.Ships.Count > 0)
            {
                Console.Clear();
                (Square sq, int e) = FindBestSquare(player);
                Console.WriteLine($"Square Coordinates: {sq.ToCoor()}");
                Console.WriteLine($"Probability: {e}%");
                Console.WriteLine("M / H / S");
                string ans = Console.ReadLine();
                switch (ans)
                {
                    case "M":
                        player.Squares[sq.ID].IsMiss = true;
                        player.Squares[sq.ID].BeenSearched = true;
                        break;
                    case "H":
                        player.Squares[sq.ID].IsHit = true;
                        player.Squares[sq.ID].BeenSearched = true;
                        break;
                    case "S":
                        Console.Write("ID of ship: ");
                        int id = int.Parse(Console.ReadLine());
                        Console.WriteLine("Squares sunk: ");
                        string raw = Console.ReadLine();
                        string[] xy = raw.Replace(")", "").Split(",(");
                        foreach (Ship ship in player.Ships.ToList())
                        {
                            if (ship.ID == id)
                            {
                                foreach (string xys in xy)
                                {
                                    string s = xys.Replace("(", "");
                                    int x = int.Parse(s.Split(",")[0]);
                                    int y = int.Parse(s.Split(",")[1]);

                                    int sqID = ((y - 1) * Settings.GridWidth) + x - 1;

                                    ship.OriginalOccupiedSquares.Add(player.Squares[sqID]);
                                }
                                foreach (Square square in ship.OriginalOccupiedSquares)
                                {
                                    square.IsSunk = true;
                                    square.BeenSearched = true;
                                }

                                ship.IsSunk = true;
                                player.Ships.Remove(ship);
                            }
                        }
                        break;
                }
            }
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
