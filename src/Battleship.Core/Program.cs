﻿namespace Battleship.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Timers;

    /// <summary>
    /// Main class of the project.
    /// </summary>
    public class Program
    {
        private static Timer delayTimer;

        /// <summary>
        /// The entry point of the program.
        /// </summary>
        public static void Main()
        {
            Simulate(1000);
        }

        /// <summary>
        /// Simulates a number of games.
        /// </summary>
        /// <param name="numberOfGames">Number of games to simulate.</param>
        public static void Simulate(int numberOfGames)
        {
            Console.WriteLine($"n = {numberOfGames}\n");

            for (int gameType = 0; gameType < 9; gameType++)
            {
                int moveCount = 0;
                int firstWins = 0;
                int secondWins = 0;

                switch (gameType)
                {
                    case 0:
                        for (int gameCount = 0; gameCount < numberOfGames; gameCount++)
                        {
                            Game game = new Game();
                            game.CreateGame(0, 0);
                            moveCount += game.Moves;
                            if (game.Winner == true)
                            {
                                firstWins++;
                            }
                            else if (game.Winner == false)
                            {
                                secondWins++;
                            }
                        }
                        Console.WriteLine("Random - Random");
                        break;
                    case 1:
                        for (int gameCount = 0; gameCount < numberOfGames; gameCount++)
                        {
                            Game game = new Game();
                            game.CreateGame(0, 1);
                            moveCount += game.Moves;
                            if (game.Winner == true)
                            {
                                firstWins++;
                            }
                            else if (game.Winner == false)
                            {
                                secondWins++;
                            }
                        }
                        Console.WriteLine("Random - Hunt Target");
                        break;
                    case 2:
                        for (int gameCount = 0; gameCount < numberOfGames; gameCount++)
                        {
                            Game game = new Game();
                            game.CreateGame(0, 2);
                            moveCount += game.Moves;
                            if (game.Winner == true)
                            {
                                firstWins++;
                            }
                            else if (game.Winner == false)
                            {
                                secondWins++;
                            }
                        }
                        Console.WriteLine("Random - Probability Density");
                        break;
                    case 3:
                        for (int gameCount = 0; gameCount < numberOfGames; gameCount++)
                        {
                            Game game = new Game();
                            game.CreateGame(1, 0);
                            moveCount += game.Moves;
                            if (game.Winner == true)
                            {
                                firstWins++;
                            }
                            else if (game.Winner == false)
                            {
                                secondWins++;
                            }
                        }
                        Console.WriteLine("Hunt Target - Random");
                        break;
                    case 4:
                        for (int gameCount = 0; gameCount < numberOfGames; gameCount++)
                        {
                            Game game = new Game();
                            game.CreateGame(1, 1);
                            moveCount += game.Moves;
                            if (game.Winner == true)
                            {
                                firstWins++;
                            }
                            else if (game.Winner == false)
                            {
                                secondWins++;
                            }
                        }
                        Console.WriteLine("Hunt Target - Hunt Target");
                        break;
                    case 5:
                        for (int gameCount = 0; gameCount < numberOfGames; gameCount++)
                        {
                            Game game = new Game();
                            game.CreateGame(1, 2);
                            moveCount += game.Moves;
                            if (game.Winner == true)
                            {
                                firstWins++;
                            }
                            else if (game.Winner == false)
                            {
                                secondWins++;
                            }
                        }
                        Console.WriteLine("Hunt Target - Probability Density");
                        break;
                    case 6:
                        for (int gameCount = 0; gameCount < numberOfGames; gameCount++)
                        {
                            Game game = new Game();
                            game.CreateGame(2, 0);
                            moveCount += game.Moves;
                            if (game.Winner == true)
                            {
                                firstWins++;
                            }
                            else if (game.Winner == false)
                            {
                                secondWins++;
                            }
                        }
                        Console.WriteLine("Probability Density - Random");
                        break;
                    case 7:
                        for (int gameCount = 0; gameCount < numberOfGames; gameCount++)
                        {
                            Game game = new Game();
                            game.CreateGame(2, 1);
                            moveCount += game.Moves;
                            if (game.Winner == true)
                            {
                                firstWins++;
                            }
                            else if (game.Winner == false)
                            {
                                secondWins++;
                            }
                        }
                        Console.WriteLine("Probability Density - Hunt Target");
                        break;
                    case 8:
                        for (int gameCount = 0; gameCount < numberOfGames; gameCount++)
                        {
                            Game game = new Game();
                            game.CreateGame(2, 2);
                            moveCount += game.Moves;
                            if (game.Winner == true)
                            {
                                firstWins++;
                            }
                            else if (game.Winner == false)
                            {
                                secondWins++;
                            }
                        }
                        Console.WriteLine("Probability Density - Probability Density");
                        break;
                }
                Console.WriteLine($"| 1st player won\t| {firstWins}\t|\n| 2nd player won\t| {secondWins}\t|\n| Avg. moves\t\t| {Math.Round(decimal.Divide(moveCount, numberOfGames), 2)}\t|\n");
            } 
        }

        /// <summary>
        /// Finds the best square to search.
        /// </summary>
        /// <param name="player">The grid to analyse.</param>
        /// <returns>The square to search and the probability of it having a ship on it.</returns>
        public static (Square, Dictionary<Square, int>) FindBestSquare(Grid player)
        {
            /*
             * MUST
             * 1. A hit square only has 1 unsearched adjacent square
             * 2. A ship only has 1 possible arrangement

             * MIGHT
             * 3. A square has a hit adjacent square
             * 
             * MUST NOT
             * 4. Ships that are not sunk can't be located entirely on 'hit' squares.
             */

            // 1. A hit square only has 1 unsearched adjacent square
            foreach (Square square in player.Squares)
            {
                if (square.IsHit != true)
                {
                    continue;
                }

                List<Square> unsearchedSquares = new List<Square>();

                foreach (Square adjSquare in square.GetAdjacentSquares())
                {
                    if (!adjSquare.BeenSearched)
                    {
                        unsearchedSquares.Add(adjSquare);
                    }
                }

                if (unsearchedSquares.Count == 1)
                {
                    return (unsearchedSquares[0], new Dictionary<Square, int>() { { unsearchedSquares[0], 100 } });
                }
            }

            // 2. A ship only has 1 possible arrangement
            foreach (Ship ship in player.Ships)
            {
                List<List<int>> arrangements = ship.GetArrangements();

                if (arrangements.Count == 1)
                {
                    foreach (int squareID in arrangements[0])
                    {
                        Square square = player.Squares[squareID];
                        if (!square.BeenSearched)
                        {
                            return (square, new Dictionary<Square, int>(){{ square, 100 }});
                        }
                    }
                }
            }

            // 3. A square has a hit adjacent square
            player.ToSearch.Clear();

            foreach (Square square in player.Squares)
            {
                if (square.IsHit != true)
                {
                    continue;
                }

                foreach (Square adjSquare in square.GetAdjacentSquares())
                {
                    if (player.ToSearch.Contains(adjSquare) || adjSquare.BeenSearched)
                    {
                        continue;
                    }

                    player.ToSearch.Add(adjSquare);
                }
            }

            Dictionary<Square, int> probability = new Dictionary<Square, int>();

            if (player.ToSearch.Count == 0)
            {
                // All unsearched squares

                foreach (Square square in player.Squares)
                {
                    {
                        if (!square.BeenSearched)
                        {
                            probability.Add(square, 0);
                        }
                    }
                }
            }
            else
            {
                // All squares in .ToSearch()

                foreach (Square square in player.ToSearch)
                {
                    probability.Add(square, 0);
                }
            }

            foreach (Ship ship in player.Ships)
            {
                foreach (List<int> arrangement in ship.GetArrangements())
                {
                    foreach (int squareID in arrangement)
                    {
                        Square sq = player.Squares[squareID];
                        if (probability.ContainsKey(sq))
                        {
                            probability[sq]++;
                        }
                    }
                }
            }

            return (probability.Aggregate((l, r) => l.Value > r.Value ? l : r).Key, probability);
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
                (Square sq, Dictionary<Square, int> e) = FindBestSquare(player);
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

                                    int sqID1 = ((y - 1) * (int)Settings.GridWidth) + x - 1;

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
