namespace Battleships
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Timers;

    /// <summary>
    /// Defines a game of Battleship.
    /// </summary>
    public class Game
    {
        /// <summary>
        /// Player 1's grid.
        /// </summary>
        public Grid Player1;

        /// <summary>
        /// Player 2's grid.
        /// </summary>
        public Grid Player2;

        /// <summary>
        /// The winner of the game.
        /// </summary>
        public bool Winner;

        /// <summary>
        /// Number of moves.
        /// </summary>
        public int Move = 0;

        private bool turn = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="Game"/> class.
        /// </summary>
        public Game()
        {
            this.Player1 = new Grid();
            this.Player2 = new Grid();
        }

        /// <summary>
        /// Starts a new game.
        /// </summary>
        public void CreateGame()
        {
            this.StartGame();
            while (this.Player1.Ships.Count > 0 && this.Player2.Ships.Count > 0)
            {
                this.AttackRandom();
            }

            this.EndGame();
        }

        /// <summary>
        /// Sets up the game.
        /// </summary>
        private void StartGame()
        {
            List<string> shipTypes = new List<string>()
            {
                "Carrier",
                "Battleship",
                "Cruiser",
                "Submarine",
                "Destroyer",
            };

            foreach (string type in shipTypes)
            {
                Random rnd = new Random();

                bool isAvail1 = false;

                while (isAvail1 == false)
                {
                    bool horizontal = false;

                    if (rnd.Next(2) == 0)
                    {
                        horizontal = true;
                    }

                    try
                    {
                        this.Player1.AddShip(this.Player1.Squares[rnd.Next(100)], new Ship(this.Player1, type), horizontal);
                        isAvail1 = true;
                    }
                    catch
                    {   }
                }

                bool isAvail2 = false;

                while (isAvail2 == false)
                {
                    bool horizontal = false;

                    if (rnd.Next(2) == 0)
                    {
                        horizontal = true;
                    }

                    try
                    {
                        this.Player2.AddShip(this.Player2.Squares[rnd.Next(100)], new Ship(this.Player2, type), horizontal);
                        isAvail2 = true;
                    }
                    catch
                    { }
                }
            }
        }

        /// <summary>
        /// Ends the game.
        /// </summary>
        private void EndGame()
        {
            if (this.Player2.Ships.Count == 0)
            {
                Winner = true;
                //Console.WriteLine("Player 1 won.");
            }
            else if (this.Player1.Ships.Count == 0)
            {
                Winner = false;
                //Console.WriteLine("Player 2 won.");
            }
        }

        /// <summary>
        /// Attacks an enemy square.
        /// </summary>
        private void AttackRandom()
        {
            //this.UpdateConsole();
            Random rnd = new Random();

            if (this.turn)
            {
                int rannum = 0;

                bool isAvail = false;
                while (isAvail == false)
                {
                    rannum = rnd.Next(100);
                    if (!this.Player2.Squares[rannum].BeenSearched)
                    {
                        isAvail = true;
                    }
                }

                this.Player1.Search(this.Player2.Squares[rannum]);

                this.turn = false;
            }
            else
            {
                int rannum = 0;

                bool isAvail = false;
                while (isAvail == false)
                {
                    rannum = rnd.Next(100);
                    if (!this.Player1.Squares[rannum].BeenSearched)
                    {
                        isAvail = true;
                    }
                }

                this.Player2.Search(this.Player1.Squares[rannum]);

                this.Move++;

                this.turn = true;
            }
        }

        /// <summary>
        /// Updates the console with the latest data.
        /// </summary>
        private void UpdateConsole()
        {
            string p1 = string.Empty;

            foreach (Square sq in this.Player1.Squares)
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

            foreach (Square sq in this.Player2.Squares)
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
            Console.WriteLine($"Move {this.Move}\n");

            string visual = $"player 1       player 2\n{this.Player1.Ships.Count} ships        {this.Player2.Ships.Count} ships\n";

            for (int i = 0; i < 10; i++)
            {
                visual += $"\n{player1[i]}     {player2[i]}";
            }

            Console.WriteLine(visual);
        }
    }
}
