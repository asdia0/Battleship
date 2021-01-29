namespace Battleships
{
    using System;

    /// <summary>
    /// Defines a square on the grid.
    /// </summary>
    public class Square
    {
        public Grid grid;

        /// <summary>
        /// Unique identification tag of the square.
        /// </summary>
        public int id;

        /* 
         * Row = floor(id / 10)
         * Column = id % 10 (0 to 9)
         */

        /// <summary>
        /// Determines whether the square has been searched.
        /// </summary>
        public bool beenSearched;

        /// <summary>
        /// Determines whether the square has an enemy ship on it.
        /// </summary>
        public bool? hasShip = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="Square"/> class.
        /// </summary>
        /// <param name="id">The unique identification tag of the square.</param>
        public Square(Grid grid, int id)
        {
            if (0 > id || id > 99)
            {
                throw new Exception("Invalid Square ID.");
            }

            this.grid = grid;
            this.id = id;
        }
    }
}