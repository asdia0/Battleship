namespace Battleship.Core
{
    /// <summary>
    /// Defines a move.
    /// </summary>
    public class Move
    {
        /// <summary>
        /// Player that searched the square.
        /// </summary>
        public string Player;

        /// <summary>
        /// The x-coordinate of the square searched.
        /// </summary>
        public int X;

        /// <summary>
        /// The y-coordinate of the square searched.
        /// </summary>
        public int Y;

        /// <summary>
        /// Initializes a new instance of the <see cref="Move"/> class.
        /// </summary>
        /// <param name="player">The player who made the move.</param>
        /// <param name="tuple">The coordinates of the square changed.</param>
        public Move(string player, (int, int) tuple)
        {
            this.Player = player;

            this.X = tuple.Item1;
            this.Y = tuple.Item2;
        }
    }
}
