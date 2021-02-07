namespace Battleship
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Defines a ship on the grid.
    /// </summary>
    public class Ship
    {
        public List<Square> arrangement = new List<Square>();

        /// <summary>
        /// The ship's type.
        /// </summary>
        public string Type;

        /// <summary>
        /// The ship's length.
        /// </summary>
        public int Length;

        /// <summary>
        /// The list of squares that the ship occupies.
        /// </summary>
        public List<Square> OccupiedSquares = new List<Square>();

        /// <summary>
        /// The ship's grid.
        /// </summary>
        public Grid Grid;

        /// <summary>
        /// Initializes a new instance of the <see cref="Ship"/> class.
        /// </summary>
        /// <param name="grid">The ship's grid.</param>
        /// <param name="type">The ship's type.</param>
        public Ship(Grid grid, string type)
        {
            this.Grid = grid;
            this.Type = type;

            switch (type)
            {
                case "Carrier":
                    this.Length = 5;
                    break;
                case "Battleship":
                    this.Length = 4;
                    break;
                case "Cruiser":
                    this.Length = 3;
                    break;
                case "Submarine":
                    this.Length = 3;
                    break;
                case "Destroyer":
                    this.Length = 2;
                    break;
                default:
                    throw new Exception("Ship type unknown.");
            }
        }

        public List<List<int>> GetArrangements()
        {
            List<List<int>> arrSq = new List<List<int>>();

            for (int i = 0; i < (Settings.gridHeight * Settings.gridWidth); i++)
            {
                int availRows = Settings.gridWidth - (i % Settings.gridWidth);
                int availCols = Settings.gridHeight - (int)Math.Floor((decimal)i / Settings.gridHeight);

                // horizontal
                if (this.Length <= availRows)
                {
                    List<int> arr = new List<int>();

                    for (int j = 0; j < this.Length; j++)
                    {
                        arr.Add(j + i);
                    }

                    arrSq.Add(arr);
                }

                // vertical
                if (this.Length <= availCols)
                {
                    List<int> arr = new List<int>();

                    for (int j = 0; j < this.Length; j++)
                    {
                        arr.Add((j * Settings.gridWidth) + i);
                    }

                    arrSq.Add(arr);
                }
            }

            return arrSq;
        }
    }
}
