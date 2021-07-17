namespace Battleship.Example
{
    using System;
    using System.Collections.Generic;

    class Program
    {
        public static void Main()
        {
            Console.WriteLine(GetOptimalLayout());
            Console.WriteLine(GetRandomGame());
        }

        /// <summary>
        /// Gets the optimal layout for side length 10
        /// </summary>
        /// <returns></returns>
        public static Grid GetOptimalLayout()
        {
            // Create template grid
            Grid template = new(10, 10);
            template.Ships.AddRange(new List<Ship>()
            {
                new Ship(template, 2),
                new Ship(template, 3),
                new Ship(template, 3),
                new Ship(template, 4),
                new Ship(template, 5),
            });

            // Return optimal layout
            return Layout.Optimal(10, 10, template.Ships);
        }

        /// <summary>
        /// Simualate a completely random game.
        /// </summary>
        /// <returns></returns>
        public static Game GetRandomGame()
        {
            // Create template grid
            Grid template = new(10, 10);
            template.Ships.AddRange(new List<Ship>()
            {
                new Ship(template, 2),
                new Ship(template, 3),
                new Ship(template, 3),
                new Ship(template, 4),
                new Ship(template, 5),
            });

            // Create grids
            Grid grid1 = new(10, 10);
            Grid grid2 = new(10, 10);

            // Add ships to grids
            grid1.AddShipsRandomly(template.Ships);
            grid2.AddShipsRandomly(template.Ships);

            // Simulate and return game
            return new Game(new("Player 1", grid1), new("Player 2", grid2), StrategyType.Random, StrategyType.Random);
        }
    }
}
