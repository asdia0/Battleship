namespace Battleships
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Defines a game of Battleship.
    /// </summary>
    public class Game
    {
        /// <summary>
        /// Player 1's grid.
        /// </summary>
        public static Grid Player1;

        /// <summary>
        /// Player 2's grid.
        /// </summary>
        public static Grid Player2;

        private static int move = 0;

        private static bool turn = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="Game"/> class.
        /// </summary>
        public Game()
        {
            Player1 = new Grid();
            Player2 = new Grid();
        }

        /// <summary>
        /// Starts a new game.
        /// </summary>
        public void CreateGame()
        {
            this.StartGame();
            this.Attack();
            this.EndGame();
        }

        /// <summary>
        /// Sets up the game.
        /// </summary>
        public void StartGame()
        {
            Random rnd = new Random();

            // replace with rng later
            Player2.AddShip(Player2.Squares[0], new Ship(Player2, "Carrier"), true);
            Player2.AddShip(Player2.Squares[10], new Ship(Player2, "Battleship"), true);
            Player2.AddShip(Player2.Squares[20], new Ship(Player2, "Cruiser"), true);
            Player2.AddShip(Player2.Squares[30], new Ship(Player2, "Submarine"), true);
            Player2.AddShip(Player2.Squares[40], new Ship(Player2, "Destroyer"), true);

            Player1.AddShip(Player1.Squares[0], new Ship(Player1, "Carrier"), true);
            Player1.AddShip(Player1.Squares[10], new Ship(Player1, "Battleship"), true);
            Player1.AddShip(Player1.Squares[20], new Ship(Player1, "Cruiser"), true);
            Player1.AddShip(Player1.Squares[30], new Ship(Player1, "Submarine"), true);
            Player1.AddShip(Player1.Squares[40], new Ship(Player1, "Destroyer"), true);
        }

        /// <summary>
        /// Ends the game.
        /// </summary>
        public void EndGame()
        {
            if (Player2.Ships.Count == 0)
            {
                Console.WriteLine("Congratulations on winning!");
            }
            else if (Player1.Ships.Count == 0)
            {
                Console.WriteLine("F.");
            }
        }

        /// <summary>
        /// Attacks an enemy square.
        /// </summary>
        public void Attack()
        {
            while (Player1.Ships.Count > 0 && Player2.Ships.Count > 0)
            {
                this.UpdateConsole();
                Random rnd = new Random();

                if (turn)
                {
                    bool isAvail = false;

                    int id = 0;

                    while (isAvail == false)
                    {
                        Console.WriteLine("Please select a square to attack");
                        id = int.Parse(Console.ReadLine());

                        if (!Player2.Squares[id].BeenSearched)
                        {
                            isAvail = true;
                        }
                    }

                    Player1.Search(Player2.Squares[id]);

                    turn = false;
                }
                else
                {
                    int rannum = 0;

                    bool isAvail = false;
                    while (isAvail == false)
                    {
                        rannum = rnd.Next(100);
                        if (!Player1.Squares[rannum].BeenSearched)
                        {
                            isAvail = true;
                        }
                    }

                    Player2.Search(Player1.Squares[rannum]);

                    move++;

                    turn = true;
                }
            }
        }

        /// <summary>
        /// Updates the console with the latest data.
        /// </summary>
        public void UpdateConsole()
        {
            string p1 = string.Empty;

            foreach (Square sq in Player1.Squares)
            {
                if (sq.HasShip == true)
                {
                    p1 += "x";
                }
                else if (sq.BeenSearched)
                {
                    p1 += "o";
                }
                else
                {
                    p1 += ".";
                }
            }

            string p2 = string.Empty;

            foreach (Square sq in Player2.Squares)
            {
                if (sq.HasShip == true)
                {
                    p2 += "x";
                }
                else if (sq.BeenSearched)
                {
                    p2 += "o";
                }
                else
                {
                    p2 += ".";
                }
            }

            List<string> player1 = (from Match m in Regex.Matches(p1, ".{1,10}")
                                   select m.Value).ToList();

            List<string> player2 = (from Match m in Regex.Matches(p2, ".{1,10}")
                                select m.Value).ToList();

            Console.Clear();
            Console.WriteLine($"Move {move}\n");

            string visual = $"player 1       player 2\n{Player1.Ships.Count} ships        {Player2.Ships.Count} ships\n";

            for (int i = 0; i < 10; i++)
            {
                visual += $"\n{player1[i]}     {player2[i]}";
            }

            Console.WriteLine(visual);
        }
    }
}
