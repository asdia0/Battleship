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
        /// Gets the list of moves made.
        /// </summary>
        public List<Move> MoveList { get; }

        /// <summary>
        /// Gets the winner of the game.
        /// </summary>
        public Player? Winner
        {
            get
            {
                if (this.Player1.Grid.OperationalShips.Count == 0)
                {
                    return this.Player2;
                }
                else if (this.Player2.Grid.OperationalShips.Count == 0)
                {
                    return this.Player1;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Gets or sets the first player.
        /// </summary>
        public Player Player1 { get; }

        /// <summary>
        /// Gets or sets the second player.
        /// </summary>
        public Player Player2 { get; }

        /// <summary>
        /// Gets the current turn.
        /// </summary>
        public Turn Turn
        {
            get
            {
                return this.MoveList.Count % 2 == 0 ? Turn.Player1 : Turn.Player2;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Game"/> class.
        /// </summary>
        /// <param name="player1">The first player.</param>
        /// <param name="player2">The second player.</param>
        public Game(Player player1, Player player2)
        {
            this.MoveList = new();
            this.Player1 = player1;
            this.Player2 = player2;

            while (this.Winner == null)
            {
                if (this.Turn == Turn.Player1)
                {
                    this.ProbabilityDensity();
                }
                else
                {
                    this.Random();
                }

                Console.WriteLine(this.MoveList.Last());
            }
        }

        /// <summary>
        /// Converts a <see cref="Game"/> to a <see cref="string"/>.
        /// </summary>
        /// <returns>The stringified version of the game.</returns>
        public override string ToString()
        {
            string res = $"[{this.Player1.Name} \"{this.Player1}\"]\n[{this.Player2.Name} \"{this.Player2}\"]\n\n";

            foreach (Move move in this.MoveList)
            {
                res += move.ToString();
            }

            return res;
        }

        /// <summary>
        /// Returns essential information for <see cref="Random"/>, <see cref="HuntTarget"/> and <see cref="ProbabilityDensity"/>.
        /// </summary>
        /// <returns>Essential information.</returns>
        public (Player player, Grid player1, Grid player2) ConfigureTurn()
        {
            if (this.Turn == Turn.Player1)
            {
                return (this.Player1, this.Player1.Grid, this.Player2.Grid);
            }
            else
            {
                return (this.Player2, this.Player2.Grid, this.Player1.Grid);
            }
        }

        /// <summary>
        /// Attacks an enemy square randomly.
        /// </summary>
        public void Random()
        {
            (Player player, Grid p1, Grid p2) = this.ConfigureTurn();

            Square attackedSq = p2.UnsearchedSquares[new Random().Next(p2.UnsearchedSquares.Count)];

            this.Search(p1, attackedSq);

            this.MoveList.Add(new Move(player, attackedSq));
        }

        /// <summary>
        /// Attacks an enmy square that is adjacent to a hit square. Implements parity.
        /// </summary>
        public void HuntTarget()
        {
            Square attackedSquare;
            (Player player, Grid p1, Grid p2) = this.ConfigureTurn();

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

            this.Search(p1, attackedSquare);

            this.MoveList.Add(new Move(player, attackedSquare));
        }

        /// <summary>
        /// Attacks an enemy square based on previous searches. Searches for all enemy ships at the same time.
        /// </summary>
        public void ProbabilityDensity()
        {
            (Player player, Grid p1, Grid p2) = this.ConfigureTurn();

            /*
             * MUST
             * 1. A hit square only has 1 unsearched adjacent square
             * 2. A ship only has 1 possible arrangement
             * MIGHT
             * 3. A square has a hit adjacent square
             * MUST NOT
             * 4. Ships that are not sunk can't be located entirely on 'hit' squares.
             */

            // 1. A hit square only has 1 unsearched adjacent square
            foreach (Square square in p2.Squares)
            {
                if (square.Status != SquareStatus.Hit)
                {
                    continue;
                }

                List<Square> unsearchedSquares = new List<Square>();

                foreach (Square adjSquare in square.AdjacentSquares)
                {
                    if (!adjSquare.Searched)
                    {
                        unsearchedSquares.Add(adjSquare);
                    }
                }

                if (unsearchedSquares.Count == 1)
                {
                    Square attackSq = unsearchedSquares[0];
                    this.Search(p1, attackSq);

                    this.MoveList.Add(new Move(player, attackSq));

                    return;
                }
            }

            // 2. A ship only has 1 possible arrangement
            foreach (Ship ship in p2.OperationalShips)
            {
                HashSet<HashSet<int>> arrangements = ship.Arrangements;

                if (arrangements.Count == 1)
                {
                    foreach (int squareID in arrangements.First())
                    {
                        Square square = p2.Squares[squareID];
                        if (!square.Searched)
                        {
                            this.Search(p1, square);

                            this.MoveList.Add(new Move(player, square));

                            return;
                        }
                    }
                }
            }

            // 3. A square has a hit adjacent square
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

            // Get probability
            foreach (Ship ship in p2.OperationalShips)
            {
                foreach (HashSet<int> arrangement in ship.Arrangements)
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

            this.Search(p1, attackedSquare);

            this.MoveList.Add(new Move(player, attackedSquare));
        }

        /// <summary>
        /// Searches a square.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="square">The square to search.</param>
        public void Search(Grid player, Square square)
        {
            if (square.Searched)
            {
                throw new BattleshipException($"Square {square.ID} has already been searched.");
            }

            square.Searched = true;

            // square has ship
            if (square.Ship != null)
            {
                this.AddTargets(player, square);
            }

            player.ToSearch.Remove(square);
            player.ToAttack.Remove(square);
        }

        /// <summary>
        /// Adds squares to the target list.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="square">The square to check.</param>
        private void AddTargets(Grid player, Square square)
        {
            if (square.Searched && square.Ship != null && square.Status != SquareStatus.Sunk)
            {
                foreach (Square sq in square.AdjacentSquares)
                {
                    if (!sq.Searched && !player.ToSearch.Contains(sq))
                    {
                        player.ToSearch.Add(sq);
                    }
                }
            }
        }
    }
}
