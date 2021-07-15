﻿namespace Battleship
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// Main class of the project.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The entry point of the program.
        /// </summary>
        public static void Main()
        {
            for (int n = 6; n <= 15; n++)
            {
                string path = $"Layout Optimal-Random ({n}).tsv";

                File.WriteAllText(path, "Game ID\tWinner\tPlies");

                for (int i = 0; i < 10000; i++)
                {
                    Grid template = new(n, n);

                    List<Ship> templateList = new()
                    {
                        new(template, 2),
                        new(template, 3),
                        new(template, 3),
                        new(template, 4),
                        new(template, 5),
                    };

                    string res = "\n" + i.ToString();

                    Grid g1 = new(n, n);
                    Grid g2 = new(n, n);
                    g1.AddShipsOptimally(templateList);
                    g2.AddShipsRandomly(templateList);

                    Player p1 = new("Player 1", g1);
                    Player p2 = new("Player 2", g2);
                    Game g = new(p1, p2, StrategyType.Optimal, StrategyType.Optimal);

                    if (g.Winner == p1)
                    {
                        res += $"\t1";
                    }
                    else if (g.Winner == p2)
                    {
                        res += $"\t0";
                    }

                    res += $"\t{g.MoveList.Count}";

                    File.AppendAllText(path, res);
                }
            }
        }

        /// <summary>
        /// Simulates games and records win rates into a TSV files.
        /// </summary>
        /// <param name="length">The length of the grid to simulate.</param>
        /// <param name="breadth">The breadth of the grid to simulate.</param>
        /// <param name="numberOfGames">Number of games to simulate for each match.</param>
        /// <param name="path">The path of the file to save to.</param>
        public static void RecordResults(int length, int breadth, int numberOfGames, string path)
        {
            string res = "Player2\\Player1\tRandom\tHuntTarget\tProbabilityDensity";

            for (int i = 0; i < 3; i++)
            {
                res += $"\n{(StrategyType)i}";

                for (int j = 0; j < 3; j++)
                {
                    (int, int) wins = SimulateWins(length, breadth, numberOfGames, (StrategyType)j, (StrategyType)i);
                    res += $"\t{wins.Item1}";
                }
            }

            File.WriteAllText(path, res);
        }

        /// <summary>
        /// Simulates a number of games.
        /// </summary>
        /// <param name="length">The length of the grid to simulate.</param>
        /// <param name="breadth">The breadth of the grid to simulate.</param>
        /// <param name="numberOfGames">Number of games to simulate.</param>
        /// <param name="player1strategy">Player 1' strategy.</param>
        /// <param name="player2strategy">Player 2's strategy.</param>
        /// <returns>A tuple of the number of wins each player had.</returns>
        public static (int Player1, int Player2) SimulateWins(int length, int breadth, int numberOfGames, StrategyType player1strategy, StrategyType player2strategy)
        {
            int first = 0;
            int second = 0;

            Grid template = new (length, breadth);

            List<Ship> templateList = new ()
            {
                new (template, 2),
                new (template, 3),
                new (template, 3),
                new (template, 4),
                new (template, 5),
            };

            for (int i = 0; i < numberOfGames; i++)
            {
                Grid g1 = new (length, breadth);
                Grid g2 = new (length, breadth);
                g1.AddShipsRandomly(templateList);
                g2.AddShipsRandomly(templateList);

                Player p1 = new ("Player 1", g1);
                Player p2 = new ("Player 2", g2);
                Game g = new (p1, p2, player1strategy, player2strategy);

                if (g.Winner.Name == "Player 1")
                {
                    first++;
                }
                else if (g.Winner.Name == "Player 2")
                {
                    second++;
                }
            }

            return (first, second);
        }

        public static void SimulateMoves(string path, int length, int breadth, int numberOfGames, StrategyType strategy)
        {
            Grid template = new (length, breadth);

            List<Ship> templateList = new ()
            {
                new (template, 2),
                new (template, 3),
                new (template, 3),
                new (template, 4),
                new (template, 5),
            };

            for (int i = 0; i < numberOfGames; i++)
            {
                int count = 0;
                Grid random = new (length, breadth);
                random.AddShipsRandomly(templateList);

                while (random.OperationalShips.Count != 0)
                {
                    count++;
                    Square square = strategy switch
                    {
                        StrategyType.Random => Strategy.Random(random),
                        StrategyType.HuntTarget => Strategy.HuntTarget(random),
                        StrategyType.Optimal => Strategy.Optimal(random),
                        _ => throw new BattleshipException("Unrecognised strategy."),
                    };
                    square.Searched = true;
                }

                File.AppendAllText(path, $"{length}\t{breadth}\t{strategy}\t{count}\n");
            }
        }
    }
}
