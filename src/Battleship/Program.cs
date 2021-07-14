namespace Battleship
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
            //for (int i = 5; i <= 15; i++)
            //{
            //    SimulateMoves("data.tsv", i, i, 10000, StrategyType.Random);
            //    SimulateMoves("data.tsv", i, i, 10000, StrategyType.HuntTarget);
            //    SimulateMoves("data.tsv", i, i, 10000, StrategyType.ProbabilityDensity);
            //}


            Grid template = new(10, 10);

            List<Ship> templateList = new()
            {
                new(template, 2),
                new(template, 3),
                new(template, 3),
                new(template, 4),
                new(template, 5),
            };

            Grid optimal = Layout.Optimal(10, 10, templateList);

            Grid grid1 = new Grid(10, 10);
            Grid grid2 = new Grid(10, 10);
            grid1.AddShipsOptimally(templateList);
            grid2.AddShipsOptimally(templateList);

            Console.WriteLine(grid1);
            Console.WriteLine(grid2);

            Game game = new Game(new Player("Player 1", grid1), new Player("Player 2", grid2), StrategyType.ProbabilityDensity, StrategyType.ProbabilityDensity);

            foreach (Move move in game.MoveList)
            {
                if (move.Player.Name != "Player 1")
                {
                    continue;
                }

                Console.WriteLine((move.Player.Name, move.Square.Position, move.Square.Status));
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
                        StrategyType.ProbabilityDensity => Strategy.Optimal(random),
                        _ => throw new BattleshipException("Unrecognised strategy."),
                    };
                    square.Searched = true;
                }

                File.AppendAllText(path, $"{length}\t{breadth}\t{strategy}\t{count}\n");
            }
        }
    }
}
