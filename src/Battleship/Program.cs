namespace Battleship
{
    using System;
    using System.Collections.Generic;

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
            Simulate(100);
        }

        /// <summary>
        /// Simulates a number of games.
        /// </summary>
        /// <param name="numberOfGames">Number of games to simulate.</param>
        public static void Simulate(int numberOfGames)
        {
            Console.WriteLine($"Simulating {numberOfGames} games...");
            int first = 0;
            int second = 0;

            Grid template = new (10, 10);

            List<Ship> templateList = new List<Ship>()
            {
                new (template, 1, 2),
                new (template, 1, 3),
                new (template, 1, 3),
                new (template, 1, 4),
                new (template, 1, 5),
            };

            for (int i = 0; i < numberOfGames; i++)
            {
                Grid g1 = new (10, 10);
                Grid g2 = new (10, 10);
                g1.AddShipsRandomly(templateList);
                g2.AddShipsRandomly(templateList);
                Player p1 = new ("Player 1", g1);
                Player p2 = new ("Player 2", g2);
                Game g = new (p1, p2);

                if (g.Winner.Name == "Player 1")
                {
                    first++;
                }
                else if (g.Winner.Name == "Player 2")
                {
                    second++;
                }
            }

            Console.WriteLine($"The first player won {first} games.\nThe second player won {second} games.");
        }
    }
}
