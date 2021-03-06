namespace Battleship.Core
{
    using System.Collections.Generic;

    /// <summary>
    /// Sets several variables for <see cref="Game"/>s.
    /// </summary>
    public class Settings
    {
        /// <summary>
        /// The number of columns in the grid.
        /// </summary>
        public static int GridWidth = 10;

        /// <summary>
        /// The number of rows in the grid.
        /// </summary>
        public static int GridHeight = 10;

        /// <summary>
        /// List of ships on the grid.
        /// </summary>
        public static List<Ship> ShipList = new List<Ship>();
    }
}
