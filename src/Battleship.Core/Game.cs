namespace Battleship.Core
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
        /// List of moves made.
        /// </summary>
        public List<Move> MoveList = new List<Move>();

        /// <summary>
        /// The winner of the game.
        /// </summary>
        public bool? Winner = null;

        /// <summary>
        /// Number of moves.
        /// </summary>
        public int Moves;

        /// <summary>
        /// Player 1's grid.
        /// </summary>
        public Grid Player1;

        /// <summary>
        /// Player 2's grid.
        /// </summary>
        public Grid Player2;

        /// <summary>
        /// Keeps track of which player to move. True = player 1, false = player 2.
        /// </summary>
        public bool Turn = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="Game"/> class.
        /// </summary>
        public Game()
        {
            this.Player1 = new Grid();
            this.Player2 = new Grid();
        }

        /// <summary>
        /// Converts a <see cref="Game"/> to a <see cref="string"/>.
        /// </summary>
        /// <returns>The stringified version of the game.</returns>
        public override string ToString()
        {
            string res = $"[Player 1 \"{this.Player1}\"]\n[Player 2 \"{this.Player2}\"]\n\n";

            foreach (Move move in this.MoveList)
            {
                res += $"{move.Player}: ({move.X},{move.Y})\n";
            }

            return res;
        }

        /// <summary>
        /// Attacks an enemy square randomly.
        /// </summary>
        public void Random()
        {
            string playername;
            Grid p1;
            Grid p2;

            if (this.Turn)
            {
                playername = "Player 1";
                p1 = this.Player1;
                p2 = this.Player2;
            }
            else
            {
                playername = "Player 2";
                p1 = this.Player2;
                p2 = this.Player1;
            }

            Square attackedSq = p2.UnsearchedSquares[new Random().Next(p2.UnsearchedSquares.Count)];

            this.Search(p1, p2, attackedSq);

            this.MoveList.Add(new Move(playername, attackedSq.ToCoor()));

            if (attackedSq.HadShip != true)
            {
                if (this.Turn)
                {
                    this.Moves++;
                }

                this.Turn ^= true;
            }
        }

        /// <summary>
        /// Attacks an enmy square that is adjacent to a hit square. Implements parity.
        /// </summary>
        public void HuntTarget()
        {
            Square attackedSquare;
            string playername;
            Grid p1;
            Grid p2;

            if (this.Turn)
            {
                playername = "Player 1";
                p1 = this.Player1;
                p2 = this.Player2;
            }
            else
            {
                playername = "Player 2";
                p1 = this.Player2;
                p2 = this.Player1;
            }

            this.UpdateToSearch(p1, p2);

            if (p1.ToSearch.Count == 0)
            {
                // HUNT
                attackedSquare = p2.UnsearchedSquares[new Random().Next(p2.UnsearchedSquares.Count)];
            }
            else
            {
                // TARGET
                attackedSquare = p1.ToSearch.ToList()[new Random().Next(p1.ToSearch.Count)];
            }

            this.Search(p1, p2, attackedSquare);

            this.MoveList.Add(new Move(playername, attackedSquare.ToCoor()));

            if (attackedSquare.HadShip != true)
            {
                if (this.Turn)
                {
                    this.Moves++;
                }

                this.Turn ^= true;
            }
        }

        /// <summary>
        /// Attacks an enemy square based on previous searches. Searches for all enemy ships at the same time.
        /// </summary>
        public void ProbabilityDensity()
        {
            string playername;
            Grid p1;
            Grid p2;

            if (this.Turn)
            {
                p1 = this.Player1;
                p2 = this.Player2;
                playername = "Player 1";
            }
            else
            {
                playername = "Player 2";
                p1 = this.Player2;
                p2 = this.Player1;
            }

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
            foreach (Square square in p2.Squares)
            {
                if (square.IsHit != true)
                {
                    continue;
                }

                List<Square> unsearchedSquares = new List<Square>();

                foreach (Square adjSquare in square.GetAdjacentSquares())
                {
                    if (!adjSquare.Searched)
                    {
                        unsearchedSquares.Add(adjSquare);
                    }
                }

                if (unsearchedSquares.Count == 1)
                {
                    Square attackSq = unsearchedSquares[0];
                    this.Search(p1, p2, attackSq);

                    this.MoveList.Add(new Move(playername, attackSq.ToCoor()));

                    if (attackSq.HadShip != true)
                    {
                        if (this.Turn)
                        {
                            this.Moves++;
                        }

                        this.Turn ^= true;
                    }

                    return;
                }
            }

            // 2. A ship only has 1 possible arrangement
            foreach (Ship ship in p2.OperationalShips)
            {
                List<List<int>> arrangements = ship.GetArrangements();

                if (arrangements.Count == 1)
                {
                    foreach (int squareID in arrangements[0])
                    {
                        Square square = p2.Squares[squareID];
                        if (!square.Searched)
                        {
                            this.Search(p1, p2, square);

                            this.MoveList.Add(new Move(playername, square.ToCoor()));

                            if (square.HadShip != true)
                            {
                                if (this.Turn)
                                {
                                    this.Moves++;
                                }

                                this.Turn ^= true;
                            }

                            return;
                        }
                    }
                }
            }

            // 3. A square has a hit adjacent square
            this.UpdateToSearch(p1, p2);

            Dictionary<Square, int> probability = new Dictionary<Square, int>();

            if (p1.ToSearch.Count == 0)
            {
                // All unsearched squares

                foreach (Square square in p2.Squares)
                {
                    {
                        if (!square.Searched)
                        {
                            probability.Add(square, 0);
                        }
                    }
                }
            }
            else
            {
                // All squares in .ToSearch()

                foreach (Square square in p1.ToSearch)
                {
                    probability.Add(square, 0);
                }
            }

            foreach (Ship ship in p2.OperationalShips)
            {
                foreach (List<int> arrangement in ship.GetArrangements())
                {
                    foreach (int squareID in arrangement)
                    {
                        Square sq = p2.Squares[squareID];
                        if (probability.ContainsKey(sq))
                        {
                            probability[sq]++;
                        }
                    }
                }
            }

            Square attackedSquare = probability.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;

            this.Search(p1, p2, attackedSquare);

            this.MoveList.Add(new Move(playername, attackedSquare.ToCoor()));

            if (attackedSquare.HadShip != true)
            {
                if (this.Turn)
                {
                    this.Moves++;
                }

                this.Turn ^= true;
            }
        }

        /// <summary>
        /// Searches a square.
        /// </summary>
        /// <param name="p1">Player 1's grid.</param>
        /// <param name="p2">Player 2's grid.</param>
        /// <param name="sq">The square to search.</param>
        public void Search(Grid p1, Grid p2, Square sq)
        {
            if (sq.Searched)
            {
                throw new Exception($"Square {sq.ID} has already been searched.");
            }

            sq.BeenSearched = true;
            p2.UnsearchedSquares.Remove(sq);

            if (sq.HasShip == true)
            {
                this.AddTargets(p1, p2, sq.ID);

                sq.HasShip = false;
                sq.IsHit = true;

                foreach (Ship sp in sq.Grid.OperationalShips.ToList())
                {
                    if (sp.CurrentOccupiedSquares.Contains(sq))
                    {
                        sp.CurrentOccupiedSquares.Remove(sq);

                        if (sp.CurrentOccupiedSquares.Count == 0)
                        {
                            p2.OperationalShips.Remove(sp);

                            sp.IsSunk = true;

                            foreach (Square square in sp.OriginalOccupiedSquares)
                            {
                                square.IsSunk = true;
                            }
                        }
                    }
                }
            }
            else
            {
                sq.IsMiss = true;
            }
        }

        /// <summary>
        /// Starts a new game.
        /// </summary>
        public void CreateGame()
        {
            this.StartGame();

            while (this.Player1.OperationalShips.Count > 0 && this.Player2.OperationalShips.Count > 0)
            {
                this.Random();
            }

            this.EndGame();
        }

        /// <summary>
        /// Starts a new game.
        /// </summary>
        /// <param name="algorithm1">The algorithm of Player 1.</param>
        /// <param name="algorithm2">The algorithm of Player 2.</param>
        public void CreateGame(int algorithm1, int algorithm2)
        {
            this.StartGame();

            while (this.Player1.OperationalShips.Count > 0 && this.Player2.OperationalShips.Count > 0)
            {
                if (this.Turn)
                {
                    switch (algorithm1)
                    {
                        case 0:
                            this.Random();
                            break;
                        case 1:
                            this.HuntTarget();
                            break;
                        case 2:
                            this.ProbabilityDensity();
                            break;
                    }
                }
                else
                {
                    switch (algorithm2)
                    {
                        case 0:
                            this.Random();
                            break;
                        case 1:
                            this.HuntTarget();
                            break;
                        case 2:
                            this.ProbabilityDensity();
                            break;
                    }
                }
            }

            this.EndGame();
        }

        /// <summary>
        /// Sets up the game.
        /// </summary>
        private void StartGame()
        {
            this.Player1.AddShipsRandomly(Settings.ShipList);
            this.Player2.AddShipsRandomly(Settings.ShipList);
        }

        /// <summary>
        /// Ends the game.
        /// </summary>
        private void EndGame()
        {
            if (this.Player1.OperationalShips.Any())
            {
                this.Winner = true;
            }

            if (this.Player2.OperationalShips.Any())
            {
                this.Winner = false;
            }
        }

        /// <summary>
        /// Adds squares to the target list.
        /// </summary>
        /// <param name="p1">Player 1.</param>
        /// <param name="p2">Player 2.</param>
        /// <param name="squareID">Square to check's ID.</param>
        private void AddTargets(Grid p1, Grid p2, int squareID)
        {
            if (p2.Squares[squareID].Searched && p2.Squares[squareID].HadShip == true && p2.Squares[squareID].IsSunk != true)
            {
                foreach (Square sq in p2.Squares[squareID].GetAdjacentSquares())
                {
                    if (!sq.Searched && !p1.ToSearch.Contains(sq))
                    {
                        p1.ToSearch.Add(sq);
                    }
                }
            }
        }

        private void UpdateToSearch(Grid p1, Grid p2)
        {
            p1.ToSearch.Clear();

            foreach (Square square in p2.Squares)
            {
                if (square.IsHit != true)
                {
                    continue;
                }

                foreach (Square adjSquare in square.GetAdjacentSquares())
                {
                    if (p1.ToSearch.Contains(adjSquare) || adjSquare.Searched)
                    {
                        continue;
                    }

                    p1.ToSearch.Add(adjSquare);
                }
            }
        }
    }
}
