namespace Battleship.Core
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
        public int ID;

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

        public bool IsSunk = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="Ship"/> class.
        /// </summary>
        /// <param name="grid">The ship's grid.</param>
        /// <param name="type">The ship's type.</param>
        public Ship(Grid grid, int length)
        {
            this.Length = length;
            this.Grid = grid;
            this.ID = grid.Ships.Count;
        }

        public Ship(int id,  int length)
        {
            this.Length = length;
            this.ID = id;
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

        /// <summary>
        /// Determines whether the ship can fit on the grid.
        /// </summary>
        /// <param name="sq">The starting square where the ship will be "placed".</param>
        /// <param name="alignment">The alignment of the ship. True means horizontal and false means vertical.</param>
        /// <returns>A boolean.</returns>
        public bool CanFit(Square sq, bool alignment)
        {
            int availRows = Settings.GridWidth - (sq.ID % Settings.GridHeight);
            int availCols = Settings.GridHeight - (int)Math.Floor((double)(sq.ID / Settings.GridWidth));

            // horizontal
            if (alignment && this.Length <= availRows)
            {
                int hits = 0;

                for (int j = 0; j < this.Length; j++)
                {
                    Square square = this.Grid.Squares[j + sq.ID];

                    // is an obstruction if it is a miss or has been sunk
                    if (square.IsMiss == true || square.IsSunk == true)
                    {
                        return false;
                    }

                    if (sq.IsHit == true)
                    {
                        hits++;
                    }
                }

                if (hits == this.Length)
                {
                    return false;
                }

                return true;
            }

            // vertical
            if (!alignment && this.Length <= availCols)
            {
                int hits = 0;

                for (int j = 0; j < this.Length; j++)
                {
                    Square square = this.Grid.Squares[(j * Settings.GridWidth) + sq.ID];

                    // is an obstruction if it is a miss or has been sunk
                    if (square.IsMiss == true || square.IsSunk == true)
                    {
                        return false;
                    }

                    if (sq.IsHit == true)
                    {
                        hits++;
                    }
                }

                if (hits == this.Length)
                {
                    return false;
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets all possible arrangements of the ship.
        /// </summary>
        /// <returns>A list of lists of <see cref="Square"/> IDs.</returns>
        public List<List<int>> GetArrangements()
        {
            List<List<int>> res = new List<List<int>>();

            foreach (Square sq in this.Grid.Squares)
            {
                if (this.CanFit(sq, true))
                {
                    List<int> arr = new List<int>();

                    for (int i = 0; i < this.Length; i++)
                    {
                        arr.Add(sq.ID + i);
                    }

                    res.Add(arr);
                }

                if (this.CanFit(sq, false))
                {
                    List<int> arr = new List<int>();

                    for (int i = 0; i < this.Length; i++)
                    {
                        arr.Add(sq.ID + (i * Settings.GridWidth));
                    }

                    res.Add(arr);
                }
            }

            return res;
        }
    }
}
