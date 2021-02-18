namespace Battleship.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Defines a grid.
    /// </summary>
    public class Grid
    {
        public List<Square> SearchedSquares = new List<Square>();

        public List<Ship> OriginalShips = new List<Ship>();

        /// <summary>
        /// The ships on the grid.
        /// </summary>
        public List<Ship> Ships = new List<Ship>();

        /// <summary>
        /// The grid's squares.
        /// </summary>
        public List<Square> Squares = new List<Square>();

        /// <summary>
        /// List of enemy squares to search.
        /// </summary>
        public HashSet<Square> ToSearch = new HashSet<Square>();

        /// <summary>
        /// List of enemy squares to attack.
        /// </summary>
        public HashSet<Square> ToAttack = new HashSet<Square>();

        /// <summary>
        /// List of the grid's unsearched squares.
        /// </summary>
        public List<Square> UnsearchedSquares = new List<Square>();

        /// <summary>
        /// List of the grid's unoccupied squares.
        /// </summary>
        public List<Square> UnoccupiedSquares = new List<Square>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Grid"/> class.
        /// </summary>
        public Grid()
        {
            this.AddSquares();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Grid"/> class from a string.
        /// </summary>
        /// <param name="format">The string to convert the grid from.</param>
        public Grid(string format)
        {
            // . = unsearched
            // ' = miss
            // O = hit
            // @ = sunk
            this.AddSquares();

            for (int i = 0; i < format.Count(); i++)
            {
                char c = format.ToCharArray()[i];

                switch (c)
                {
                    case 'O':
                        this.Squares[i].HadShip = true;
                        this.Squares[i].BeenSearched = true;
                        this.UnoccupiedSquares.Remove(this.Squares[i]);
                        break;
                    case '\'':
                        this.Squares[i].BeenSearched = true;
                        this.UnoccupiedSquares.Remove(this.Squares[i]);
                        break;
                    case '@':
                        this.Squares[i].IsSunk = true;
                        this.Squares[i].BeenSearched = true;
                        this.UnoccupiedSquares.Remove(this.Squares[i]);
                        break;
                }
            }
        }

        /// <summary>
        /// Adds 100 squares to the grid.
        /// </summary>
        public void AddSquares()
        {
            for (int i = 0; i < Settings.GridWidth * Settings.GridHeight; i++)
            {
                Square sq = new Square(this, i);
                this.Squares.Add(sq);
                this.UnsearchedSquares.Add(sq);
                this.UnoccupiedSquares.Add(sq);
            }
        }

        /// <summary>
        /// Adds a ship to the grid.
        /// </summary>
        /// <param name="square">The ship's starting square.</param>
        /// <param name="ship">The ship to be added.</param>
        /// <param name="alignment">The ship's alignment.</param>
        /// <returns>The task result.</returns>
        public bool AddShip(Square square, Ship ship, bool alignment)
        {
            if (ship.Grid != square.Grid)
            {
                throw new Exception("Attempted to add ship to non-local square.");
            }

            foreach (Ship sp in this.Ships)
            {
                if (sp.ID == ship.ID)
                {
                    throw new Exception($"You cannot have two {ship.ID}s.");
                }
            }

            int availRows = 11 - (square.ID % Settings.GridWidth);
            int availCols = 11 - (int)Math.Floor((decimal)square.ID / Settings.GridHeight);

            // horizontal
            if (alignment && ship.Length <= availRows)
            {
                for (int i = 0; i < ship.Length; i++)
                {
                    int sqID = i + square.ID;

                    if (sqID >= this.Squares.Count || this.Squares[sqID].HasShip == true)
                    {
                        return false;
                    }

                    this.Squares[sqID].HasShip = true;
                    this.Squares[sqID].HadShip = true;
                    ship.CurrentOccupiedSquares.Add(this.Squares[sqID]);
                    ship.OriginalOccupiedSquares.Add(this.Squares[sqID]);
                    this.UnoccupiedSquares.Remove(this.Squares[sqID]);
                }

                this.Ships.Add(ship);
                this.OriginalShips.Add(ship);
                return true;
            }

            // vertical
            else if (!alignment && ship.Length <= availCols)
            {
                for (int i = 0; i < ship.Length; i++)
                {
                    int sqID = (i * Settings.GridHeight) + square.ID;

                    if (sqID >= this.Squares.Count || this.Squares[sqID].HasShip == true)
                    {
                        return false;
                    }

                    this.Squares[sqID].HasShip = true;
                    this.Squares[sqID].HadShip = true;
                    ship.CurrentOccupiedSquares.Add(this.Squares[sqID]);
                    ship.OriginalOccupiedSquares.Add(this.Squares[sqID]);
                    this.UnoccupiedSquares.Remove(this.Squares[sqID]);
                }

                this.OriginalShips.Add(ship);
                this.Ships.Add(ship);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Converts a grid into a string.
        /// </summary>
        /// <returns>A string representing a grid.</returns>
        public override string ToString()
        {
            string res = string.Empty;

            foreach (Square sq in this.Squares)
            {
                if (sq.HadShip == true)
                {
                    res += "O";
                }
                else
                {
                    res += ".";
                }
            }

            return res;
        }
    }
}
