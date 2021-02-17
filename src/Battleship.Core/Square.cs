namespace Battleship.Core
{
    using System;

    /// <summary>
    /// Defines a square on the grid.
    /// </summary>
    public class Square
    {
        /// <summary>
        /// The square's grid.
        /// </summary>
        public Grid Grid;

        /// <summary>
        /// The square's unique identification tag.
        /// </summary>
        public int ID;

        /// <summary>
        /// Determines whether the square has been searched.
        /// </summary>
        public bool BeenSearched;

        /// <summary>
        /// Determines whether the square ever had a ship on it.
        /// </summary>
        public bool? HadShip = null;

        /// <summary>
        /// Determines whether the square has an enemy ship on it.
        /// </summary>
        public bool? HasShip = null;

        /// <summary>
        /// Determines whether the square has a sunken ship on it.
        /// </summary>
        public bool? IsSunk = null;

        /// <summary>
        /// Determines whether the square has been searched and does not have a ship on it.
        /// </summary>
        public bool? IsMiss = null;

        /// <summary>
        /// Determines whether the square has been searched and does has a ship on it.
        /// </summary>
        public bool? IsHit = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="Square"/> class.
        /// </summary>
        /// <param name="grid">The square's grid.</param>
        /// <param name="id">The square's unique identification tag.</param>
        public Square(Grid grid, int id)
        {
            if (id < 0 || id > (Settings.GridWidth * Settings.GridHeight) - 1)
            {
                throw new Exception("Invalid Square ID.");
            }

            this.Grid = grid;
            this.ID = id;
        }

        public (int, int) ToCoor()
        {
            int xCoor = this.ID % Settings.GridWidth + 1;
            int yCoor = (int)Math.Floor((double)(this.ID / Settings.GridWidth)) + 1;

            return (xCoor, yCoor);
        }

    }
}