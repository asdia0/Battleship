namespace Battleships
{
    using System;
    using System.Collections.Generic;

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
                this.AttackHuntTarget();
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

                while (true)
                {
                    bool horizontal = false;

                    if (rnd.Next(2) == 0)
                    {
                        horizontal = true;
                    }

                    try
                    {
                        this.Player1.AddShip(this.Player1.Squares[rnd.Next(100)], new Ship(this.Player1, type), horizontal);
                        break;
                    }
                    catch
                    { }
                }

                while (true)
                {
                    bool horizontal = false;

                    if (rnd.Next(2) == 0)
                    {
                        horizontal = true;
                    }

                    try
                    {
                        this.Player2.AddShip(this.Player2.Squares[rnd.Next(100)], new Ship(this.Player2, type), horizontal);
                        break;
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
                this.Winner = true;
            }
            else if (this.Player1.Ships.Count == 0)
            {
                this.Winner = false;
            }
        }

        /// <summary>
        /// Attacks an enemy square randomly.
        /// </summary>
        private void AttackRandom()
        {
            Random rnd = new Random();

            Grid P1 = new Grid();
            Grid P2 = new Grid();
            
            if (this.turn)
            {
                P1 = this.Player1;
                P2 = this.Player2;

                this.turn = false;
            }
            else
            {
                P1 = this.Player2;
                P2 = this.Player1;

                this.Move++;
                this.turn = true;
            }

            int rannum = 0;

            while (true)
            {
                rannum = rnd.Next(100);
                if (!P2.Squares[rannum].BeenSearched)
                {
                    break;
                }
            }

            P1.Search(P2.Squares[rannum]);
        }

        /// <summary>
        /// Attacks an enemy square that is adjacent to a hit square.
        /// </summary>
        private void AttackHuntTarget()
        {
            Random rnd = new Random();

            Grid P1 = new Grid();
            Grid P2 = new Grid();

            if (this.turn)
            {
                P1 = this.Player1;
                P2 = this.Player2;

                this.turn = false;
            }
            else
            {
                P1 = this.Player2;
                P2 = this.Player1;

                this.Move++;
                this.turn = true;
            }

            // HUNT
            if (P1.ToAttack.Count == 0)
            {
                int rannum = 0;

                while (true)
                {
                    rannum = rnd.Next(100);
                    if (!P2.Squares[rannum].BeenSearched)
                    {
                        break;
                    }
                }

                P1.Search(P2.Squares[rannum]);

                this.HTAddTargets(P1, P2, rannum);
            }
            // TARGET
            else
            {
                P1.Search(P1.ToAttack[0]);

                this.HTAddTargets(P1, P2, P1.ToAttack[0].ID);

                P1.ToAttack.Remove(P1.ToAttack[0]);
            }
        }

        /// <summary>
        /// Attacks an enmy square that is adjacent to a hit square. Implements parity.
        /// </summary>
        private void AttackHuntTargetParity()
        {

        }

        /// <summary>
        /// Adds squares to the target list.
        /// </summary>
        /// <param name="P1">Player 1</param>
        /// <param name="P2">Player 2</param>
        /// <param name="squareID">Square to check's ID.</param>
        private void HTAddTargets(Grid P1, Grid P2, int squareID)
        {
            if (P2.Squares[squareID].HadShip == true)
            {
                List<int> sqID = new List<int>()
                {
                    squareID - 1,
                    squareID + 1,
                    squareID - 10,
                    squareID + 10,
                };

                foreach (int id in sqID)
                {
                    try
                    {
                        if (!P2.Squares[id].BeenSearched && !P1.ToAttack.Contains(P2.Squares[id])) // if has not been searched AND has ship
                        {
                            P1.ToAttack.Add(P2.Squares[id]);
                        }
                    }
                    catch
                    { }
                }
            }
        }

        /// <summary>
        /// Attacks an enemy square based on previous searches. Searches for enemy ships one at a time.
        /// </summary>
        private void AttackProbabilityDensityEach()
        {

        }

        /// <summary>
        /// Attacks an enemy square based on previous searches. Searches for all enemy ships at the same time.
        /// </summary>
        private void AttackProbabilityDensityAll()
        {

        }
    }
}
