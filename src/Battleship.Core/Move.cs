namespace Battleship.Core
{
    /// <summary>
    /// Defines a move.
    /// </summary>
    public struct Move
    {
        /// <summary>
        /// Gets player that searched the square.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the square searched.
        /// </summary>
        public Square Square { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Move"/> struct.
        /// </summary>
        /// <param name="player">The player who made the move.</param>
        /// <param name="square">The square searched.</param>
        public Move(Player player, Square square)
        {
            this.Player = player;
            this.Square = square;
        }
    }
}
