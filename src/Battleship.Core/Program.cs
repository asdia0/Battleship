namespace Battleship.Core
{
    using System;

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

            for (int i = 0; i < numberOfGames; i++)
            {
                Game g = new Game(new Player("Player 1", new Grid(10, 10)), new Player("Player 2", new Grid(10, 10)));

                if (g.Winner.Name == "Player 1")
                {
                    first++;
                }
                else if (g.Winner.Name == "Player 2")
                {
                    second++;
                }
            }
        }
    }
}
