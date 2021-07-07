namespace Battleship
{
    /// <summary>
    /// Defines a player.
    /// </summary>
    public class Player
    {
        /// <summary>
        /// Gets the player's grid.
        /// </summary>
        public Grid Grid { get; }

        /// <summary>
        /// Gets the player's name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Player"/> class.
        /// </summary>
        /// <param name="name">The player's name.</param>
        /// <param name="grid">The player's grid.</param>
        public Player(string name, Grid grid)
        {
            this.Name = name;
            this.Grid = grid;
        }
    }
}
