namespace Battleship
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

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
                this.Random();

                //if (this.turn)
                //{
                //    this.ProbabilityDensity();
                //}
                //else
                //{
                //    this.Random();
                //}
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

                    if (this.Player1.AddShip(this.Player1.UnoccupiedSquares[rnd.Next(this.Player1.UnoccupiedSquares.Count)], new Ship(this.Player1, type), horizontal))
                    {
                        break;
                    }
                }

                while (true)
                {
                    bool horizontal = false;

                    if (rnd.Next(2) == 0)
                    {
                        horizontal = true;
                    }

                    if (this.Player2.AddShip(this.Player2.UnoccupiedSquares[rnd.Next(this.Player2.UnoccupiedSquares.Count)], new Ship(this.Player2, type), horizontal))
                    {
                        break;
                    }
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
        private void Random()
        {
            Grid p2;

            if (this.turn)
            {
                p2 = this.Player2;

                this.turn = false;
            }
            else
            {
                p2 = this.Player1;

                this.Move++;
                this.turn = true;
            }

            this.Search(p2.UnsearchedSquares[new Random().Next(p2.UnsearchedSquares.Count)]);
        }

        /// <summary>
        /// Attacks an enmy square that is adjacent to a hit square. Implements parity.
        /// </summary>
        private void HuntTarget()
        {
            Grid p1;
            Grid p2;

            if (this.turn)
            {
                p1 = this.Player1;
                p2 = this.Player2;

                this.turn = false;
            }
            else
            {
                p1 = this.Player2;
                p2 = this.Player1;

                this.Move++;
                this.turn = true;
            }

            // HUNT
            if (p1.ToAttack.Count == 0)
            {
                Square sq = p2.UnsearchedSquares[new Random().Next(p2.UnsearchedSquares.Count)];

                this.Search(sq);

                this.AddTargets(p1, p2, sq.ID);
            }

            // TARGET
            else
            {
                this.Search(p1.ToAttack[0]);

                this.AddTargets(p1, p2, p1.ToAttack[0].ID);

                p1.ToAttack.Remove(p1.ToAttack[0]);
            }
        }

        /// <summary>
        /// Attacks an enemy square based on previous searches. Searches for all enemy ships at the same time.
        /// </summary>
        private void ProbabilityDensity()
        {
            Grid p1;
            Grid p2;

            if (this.turn)
            {
                p1 = this.Player1;
                p2 = this.Player2;

                this.turn = false;
            }
            else
            {
                p1 = this.Player2;
                p2 = this.Player1;

                this.Move++;
                this.turn = true;
            }

            // SqID : probability
            Dictionary<int, int> probability = new Dictionary<int, int>();

            if (p1.ToAttack.Count == 0)
            {
                foreach (Square sq in p2.UnsearchedSquares)
                {
                    probability.Add(sq.ID, 0);
                }
            }
            else
            {
                foreach (Square sq in p1.ToAttack)
                {
                    probability.Add(sq.ID, 0);
                }
            }

            if (p1.ToAttack.Count == 0)
            {
                foreach (Square sq in p2.Squares)
                {
                    foreach (Ship ship in p2.Ships)
                    {
                        probability = ship.IncreaseProbability(probability, sq, true);
                        probability = ship.IncreaseProbability(probability, sq, false);
                    }
                }
            }
            else
            {
                foreach (Square sq in p1.ToAttack)
                {
                    foreach (Ship ship in p2.Ships)
                    {
                        probability = ship.IncreaseProbability(probability, sq, true);
                        probability = ship.IncreaseProbability(probability, sq, false);
                    }
                }
            }

            Square attackedSquare = p2.Squares[probability.Aggregate((l, r) => l.Value > r.Value ? l : r).Key];

            //int[] source = probability.Values.ToArray();
            //int i = 0;
            //int chunkSize = 10;
            //int[][] result = source.GroupBy(s => i++ / chunkSize).Select(g => g.ToArray()).ToArray();

            //Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(result));

            this.Search(attackedSquare);

            if (attackedSquare.HadShip == true)
            {
                this.AddTargets(p1, p2, attackedSquare.ID);
            }

            p1.ToAttack.Remove(attackedSquare);
        }

        /// <summary>
        /// Adds squares to the target list.
        /// </summary>
        /// <param name="p1">Player 1.</param>
        /// <param name="p2">Player 2.</param>
        /// <param name="squareID">Square to check's ID.</param>
        private void AddTargets(Grid p1, Grid p2, int squareID)
        {
            if (p2.Squares[squareID].HadShip == true)
            {
                List<int> sqID = new List<int>()
                {
                    squareID - 1,
                    squareID + 1,
                    squareID - Settings.GridWidth,
                    squareID + Settings.GridWidth,
                };

                foreach (int id in sqID)
                {
                    if (id > -1 && id < (Settings.GridHeight * Settings.GridWidth))
                    {
                        if (!p2.Squares[id].BeenSearched && !p1.ToAttack.Contains(p2.Squares[id]))
                        {
                            p1.ToAttack.Add(p2.Squares[id]);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Searches a square.
        /// </summary>
        /// <param name="sq">The square to search.</param>
        private void Search(Square sq)
        {
            if (sq.BeenSearched)
            {
                throw new Exception($"Square {sq.ID} has already been searched.");
            }

            sq.BeenSearched = true;
            sq.Grid.UnsearchedSquares.Remove(sq);

            if (sq.HasShip == true)
            {
                sq.HasShip = false;

                foreach (Ship sp in sq.Grid.Ships.ToList())
                {
                    if (sp.CurrentOccupiedSquares.Contains(sq))
                    {
                        sp.CurrentOccupiedSquares.Remove(sq);

                        if (sp.CurrentOccupiedSquares.Count == 0)
                        {
                            sq.Grid.Ships.Remove(sp);

                            foreach (Square square in sp.OriginalOccupiedSquares)
                            {
                                square.IsSunk = true;
                            }
                        }
                    }
                }
            }
        }
    }
}
