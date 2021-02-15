namespace Battleship
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Defines a ship on the grid.
    /// </summary>
    public class Ship
    {
        /// <summary>
        /// The squares the ship had occupied.
        /// </summary>
        public List<Square> OriginalOccupiedSquares = new List<Square>();

        /// <summary>
        /// The ship's type.
        /// </summary>
        public string Type;

        /// <summary>
        /// The ship's length.
        /// </summary>
        public int Length;

        /// <summary>
        /// The squares that the ship currently occupies.
        /// </summary>
        public List<Square> CurrentOccupiedSquares = new List<Square>();

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

        /// <summary>
        /// Increases the probability of the squares that a ship can fit on.
        /// </summary>
        /// <param name="probability">The current probability dictionary.</param>
        /// <param name="sq">Base square to place the ship on.</param>
        /// <param name="alignment">Alignment of the ship.</param>
        /// <returns>The new probability dictionary.</returns>
        public Dictionary<int, int> IncreaseProbability(Dictionary<int, int> probability,  Square sq, bool alignment)
        {
            Dictionary<int, int> res = probability;

            int availRows = Settings.GridWidth - (sq.ID % Settings.GridHeight);
            int availCols = Settings.GridHeight - (int)Math.Floor((double)(sq.ID / Settings.GridWidth));

            // horizontal
            if (alignment && this.Length <= availRows)
            {
                int failures = 0;

                for (int j = 0; j < this.Length; j++)
                {
                    Square square = this.Grid.Squares[j + sq.ID];
                    if ((square.BeenSearched && square.HadShip == true) || square.IsSunk == true)
                    {
                        failures++;
                    }
                }

                if (failures == 0)
                {
                    for (int j = 0; j < this.Length; j++)
                    {
                        if (res.ContainsKey(j + sq.ID))
                        {
                            res[j + sq.ID]++;
                        }
                    }
                }
            }

            // vertical
            if (!alignment && this.Length <= availCols)
            {
                int failures = 0;

                for (int j = 0; j < this.Length; j++)
                {
                    Square square = this.Grid.Squares[(j * Settings.GridWidth) + sq.ID];
                    if ((square.BeenSearched && square.HadShip == true) || square.IsSunk == true)
                    {
                        failures++;
                    }
                }

                if (failures == 0)
                {
                    for (int j = 0; j < this.Length; j++)
                    {
                        if (res.ContainsKey((j * Settings.GridWidth) + sq.ID))
                        {
                            res[(j * Settings.GridWidth) + sq.ID]++;
                        }
                    }
                }
            }

            return res;
        }
    }
}
