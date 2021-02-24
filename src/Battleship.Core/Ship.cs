namespace Battleship.Core
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Defines a ship on the grid.
    /// </summary>
    public class Ship
    {
        public bool? Alignment = null;

        /// <summary>
        /// The ship's name.
        /// </summary>
        public string Name;

        /// <summary>
        /// The squares the ship had occupied.
        /// </summary>
        public List<Square> OriginalOccupiedSquares = new List<Square>();

        /// <summary>
        /// The ship's type.
        /// </summary>
        public int ID;

        /// <summary>
        /// The ship's breadth.
        /// </summary>
        public int Breadth = 1;

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
        /// Determines whether the ship is sunk.
        /// </summary>
        public bool IsSunk = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="Ship"/> class.
        /// </summary>
        /// <param name="grid">The ship's grid.</param>
        /// <param name="length">The ship's length.</param>
        /// <param name="breadth">The ship's breadth.</param>
        public Ship(Grid grid, int length, int breadth)
        {
            Length = length;
            Grid = grid;
            ID = grid.OriginalShips.Count;
            Breadth = breadth;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Ship"/> class.
        /// </summary>
        /// <param name="id">The ship's ID.</param>
        /// <param name="length">The ship's length.</param>
        /// <param name="breadth">The ship's breadth.</param>
        public Ship(int id, int length, int breadth)
        {
            Length = length;
            ID = id;
            Breadth = breadth;
        }

        /// <summary>
        /// Increases the probability of the squares that a ship can fit on.
        /// </summary>
        /// <param name="probability">The current probability dictionary.</param>
        /// <param name="sq">Base square to place the ship on.</param>
        /// <param name="alignment">Alignment of the ship.</param>
        /// <returns>The new probability dictionary.</returns>
        public Dictionary<int, int> IncreaseProbability(Dictionary<int, int> probability, Square sq, bool alignment)
        {
            Dictionary<int, int> res = probability;

            int availRows = Settings.GridWidth - (sq.ID % Settings.GridHeight);
            int availCols = Settings.GridHeight - (int)Math.Floor((double)(sq.ID / Settings.GridWidth));

            // horizontal
            if (alignment && Length <= availRows)
            {
                int failures = 0;

                for (int j = 0; j < Length; j++)
                {
                    Square square = Grid.Squares[j + sq.ID];
                    if ((square.BeenSearched && square.HadShip == true) || square.IsSunk == true)
                    {
                        failures++;
                    }
                }

                if (failures == 0)
                {
                    for (int j = 0; j < Length; j++)
                    {
                        if (res.ContainsKey(j + sq.ID))
                        {
                            res[j + sq.ID]++;
                        }
                    }
                }
            }

            // vertical
            if (!alignment && Length <= availCols)
            {
                int failures = 0;

                for (int j = 0; j < Length; j++)
                {
                    Square square = Grid.Squares[(j * Settings.GridWidth) + sq.ID];
                    if ((square.BeenSearched && square.HadShip == true) || square.IsSunk == true)
                    {
                        failures++;
                    }
                }

                if (failures == 0)
                {
                    for (int j = 0; j < Length; j++)
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
            if (alignment && Length <= availRows)
            {
                int hits = 0;

                for (int j = 0; j < Length; j++)
                {
                    Square square = Grid.Squares[j + sq.ID];

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

                if (hits == Length)
                {
                    return false;
                }

                return true;
            }

            // vertical
            if (!alignment && Length <= availCols)
            {
                int hits = 0;

                for (int j = 0; j < Length; j++)
                {
                    Square square = Grid.Squares[(j * Settings.GridWidth) + sq.ID];

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

                if (hits == Length)
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

            foreach (Square sq in Grid.Squares)
            {
                if (CanFit(sq, true))
                {
                    List<int> arr = new List<int>();

                    for (int i = 0; i < Length; i++)
                    {
                        arr.Add(sq.ID + i);
                    }

                    res.Add(arr);
                }

                if (CanFit(sq, false))
                {
                    List<int> arr = new List<int>();

                    for (int i = 0; i < Length; i++)
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
