namespace Battleship.GUI
{
    using Battleship.Core;

    /// <summary>
    /// Contains custom settings.
    /// </summary>
    public class Settings
    {
        /// <summary>
        /// The Player's grid.
        /// </summary>
        public static Grid Grid = new Grid();

        /// <summary>
        /// The height of the player and computer's grids.
        /// </summary>
        public static int GridHeight = 10;

        /// <summary>
        /// The width of the player and computer's grids.
        /// </summary>
        public static int GridWidth = 10;
    }
}
