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
        /// Gets the first player.
        /// </summary>
        public Player Player1 { get; }

        /// <summary>
        /// Gets the second player.
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
        /// <param name="player1Strategy">Player 1's strategy.</param>
        /// <param name="player2Strategy">Player 2's strategy.</param>
        public Game(Player player1, Player player2, StrategyType player1Strategy, StrategyType player2Strategy)
        {
            this.MoveList = new ();
            this.Player1 = player1;
            this.Player2 = player2;

            while (this.Winner == null)
            {
                Square square = null;
                (Player player, Grid p1, Grid p2) = this.ConfigureTurn();

                if (this.Turn == Turn.Player1)
                {
                    switch (player1Strategy)
                    {
                        case StrategyType.Random:
                            square = Strategy.Random(p2);
                            break;

                        case StrategyType.HuntTarget:
                            square = Strategy.HuntTarget(p2);
                            break;

                        case StrategyType.Optimal:
                            square = Strategy.Optimal(p2);
                            break;
                    }
                }
                else
                {
                    switch (player2Strategy)
                    {
                        case StrategyType.Random:
                            square = Strategy.Random(p2);
                            break;

                        case StrategyType.HuntTarget:
                            square = Strategy.HuntTarget(p2);
                            break;

                        case StrategyType.Optimal:
                            square = Strategy.Optimal(p2);
                            break;
                    }
                }

                this.Search(p1, square);

                this.MoveList.Add(new Move(player, square));
            }
        }

        /// <summary>
        /// Converts a <see cref="Game"/> to a <see cref="string"/>.
        /// </summary>
        /// <returns>The stringified version of the game.</returns>
        public override string ToString()
        {
            string res = $"[{this.Player1.Name}]\n{this.Player1.Grid}\n[{this.Player2.Name}] \n{this.Player2.Grid}\n\n";

            res += string.Join("\n", this.MoveList);

            return res;
        }

        /// <summary>
        /// Returns essential information for <see cref="Random"/>, <see cref="HuntTarget"/> and <see cref="ProbabilityDensity"/>.
        /// </summary>
        /// <returns>Essential information.</returns>
        public (Player player, Grid grid1, Grid grid2) ConfigureTurn()
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
