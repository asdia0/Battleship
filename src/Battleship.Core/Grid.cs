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
        /// <summary>
        /// List of squares searched by opponent.
        /// </summary>
        public List<Square> SearchedSquares = new List<Square>();

        /// <summary>
        /// List of all the ships.
        /// </summary>
        public List<Ship> OriginalShips = new List<Ship>();

        /// <summary>
        /// The active ships on the grid.
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
            AddSquares();
        }

        public Grid(Grid grid)
        {
            AddSquares();

            foreach (Ship ship in grid.OriginalShips)
            {
                Square sq = Squares[ship.OriginalOccupiedSquares[0].ID];
                Ship ship1 = new Ship(this, ship.Length, ship.Breadth);
                AddShip(sq, ship1, (bool)ship.Alignment);
            }

            foreach (Square square in grid.Squares)
            {
                Square sq = Squares[square.ID];

                sq.HadShip = square.HadShip;
                sq.HasShip = square.HasShip;
                sq.IsHit = square.IsHit;
                sq.IsMiss = square.IsMiss;
                sq.IsSunk = square.IsSunk;
                if (square.Ship != null)
                {
                    sq.Ship = Ships[square.Ship.ID];
                }
            }
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
            AddSquares();

            for (int i = 0; i < format.Count(); i++)
            {
                char c = format.ToCharArray()[i];

                switch (c)
                {
                    case 'O':
                        Squares[i].HadShip = true;
                        Squares[i].BeenSearched = true;
                        UnoccupiedSquares.Remove(Squares[i]);
                        break;
                    case '\'':
                        Squares[i].BeenSearched = true;
                        UnoccupiedSquares.Remove(Squares[i]);
                        break;
                    case '@':
                        Squares[i].IsSunk = true;
                        Squares[i].BeenSearched = true;
                        UnoccupiedSquares.Remove(Squares[i]);
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
                Squares.Add(sq);
                UnsearchedSquares.Add(sq);
                UnoccupiedSquares.Add(sq);
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

            foreach (Ship sp in Ships)
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

                    if (sqID >= Squares.Count || Squares[sqID].HasShip == true)
                    {
                        return false;
                    }

                    Squares[sqID].Ship = ship;
                    Squares[sqID].HasShip = true;
                    Squares[sqID].HadShip = true;
                    ship.CurrentOccupiedSquares.Add(Squares[sqID]);
                    ship.OriginalOccupiedSquares.Add(Squares[sqID]);
                    UnoccupiedSquares.Remove(Squares[sqID]);
                }

                Ships.Add(ship);
                OriginalShips.Add(ship);
                ship.Alignment = true;
                return true;
            }

            // vertical
            else if (!alignment && ship.Length <= availCols)
            {
                for (int i = 0; i < ship.Length; i++)
                {
                    int sqID = (i * Settings.GridHeight) + square.ID;

                    if (sqID >= Squares.Count || Squares[sqID].HasShip == true)
                    {
                        return false;
                    }

                    Squares[sqID].Ship = ship;
                    Squares[sqID].HasShip = true;
                    Squares[sqID].HadShip = true;
                    ship.CurrentOccupiedSquares.Add(Squares[sqID]);
                    ship.OriginalOccupiedSquares.Add(Squares[sqID]);
                    UnoccupiedSquares.Remove(Squares[sqID]);
                }

                OriginalShips.Add(ship);
                Ships.Add(ship);
                ship.Alignment = false;

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

            foreach (Square sq in Squares)
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

        public void AddDefaultShips()
        {
            AddShip(Squares[0], new Ship(this, 5, 1), true);
            AddShip(Squares[10], new Ship(this, 4, 1), true);
            AddShip(Squares[20], new Ship(this, 3, 1), true);
            AddShip(Squares[30], new Ship(this, 3, 1), true);
            AddShip(Squares[40], new Ship(this, 2, 1), true);
        }

    }
}
