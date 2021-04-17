﻿namespace Battleship.Core
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
            this.AddSquares();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Grid"/> class.
        /// </summary>
        /// <param name="grid">Grid to clone from.</param>
        public Grid(Grid grid)
        {
            this.AddSquares();

            foreach (Ship ship in grid.OriginalShips)
            {
                Square sq = this.Squares[ship.OriginalOccupiedSquares[0].ID];
                Ship ship1 = new Ship(this, ship.Length, ship.Breadth);
                this.AddShip(sq, ship1, (bool)ship.Alignment);
            }

            foreach (Square square in grid.Squares)
            {
                Square sq = this.Squares[square.ID];

                sq.HadShip = square.HadShip;
                sq.HasShip = square.HasShip;
                sq.IsHit = square.IsHit;
                sq.IsMiss = square.IsMiss;
                sq.IsSunk = square.IsSunk;
                if (square.Ship != null)
                {
                    sq.Ship = this.Ships[square.Ship.ID];
                }
            }
        }

        /// <summary>
        /// Adds 100 squares to the grid.
        /// </summary>
        public void AddSquares()
        {
            for (int i = 0; i < (Settings.GridWidth * Settings.GridHeight); i++)
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

            int availRows = Settings.GridWidth + 1 - (square.ID % Settings.GridWidth);
            int availCols = Settings.GridHeight + 1 - (int)Math.Floor((decimal)square.ID / Settings.GridHeight);

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
                }

                for (int i = 0; i < ship.Length; i++)
                {
                    int sqID = i + square.ID;

                    this.Squares[sqID].Ship = ship;
                    this.Squares[sqID].HasShip = true;
                    this.Squares[sqID].HadShip = true;
                    ship.CurrentOccupiedSquares.Add(this.Squares[sqID]);
                    ship.OriginalOccupiedSquares.Add(this.Squares[sqID]);
                    this.UnoccupiedSquares.Remove(this.Squares[sqID]);
                }

                this.Ships.Add(ship);
                this.OriginalShips.Add(ship);
                ship.Alignment = true;
                return true;
            }

            // vertical
            else if (!alignment && ship.Length <= availCols)
            {
                for (int i = 0; i < ship.Length; i++)
                {
                    int sqID = (i * Settings.GridWidth) + square.ID;

                    if (sqID >= this.Squares.Count || this.Squares[sqID].HasShip == true)
                    {
                        return false;
                    }
                }

                for (int i = 0; i < ship.Length; i++)
                {
                    int sqID = (i * Settings.GridWidth) + square.ID;

                    this.Squares[sqID].Ship = ship;
                    this.Squares[sqID].HasShip = true;
                    this.Squares[sqID].HadShip = true;
                    ship.CurrentOccupiedSquares.Add(this.Squares[sqID]);
                    ship.OriginalOccupiedSquares.Add(this.Squares[sqID]);
                    this.UnoccupiedSquares.Remove(this.Squares[sqID]);
                }

                this.OriginalShips.Add(ship);
                this.Ships.Add(ship);
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

        /// <summary>
        /// Adds the default ships in the original version of Battleship to the grid.
        /// </summary>
        public void AddDefaultShips()
        {
            this.AddShip(this.Squares[0], new Ship(this, 5, 1), false);
            this.AddShip(this.Squares[1], new Ship(this, 4, 1), false);
            this.AddShip(this.Squares[2], new Ship(this, 3, 1), false);
            this.AddShip(this.Squares[3], new Ship(this, 3, 1), false);
            this.AddShip(this.Squares[4], new Ship(this, 2, 1), false);
        }

        /// <summary>
        /// Adds ships in random positions.
        /// </summary>
        /// <param name="shipList">List of ships to add.</param>
        public void AddShipsRandomly(List<Ship> shipList)
        {
            foreach (Ship ship in shipList)
            {
                Random rnd = new Random();

                while (true)
                {
                    bool horizontal = false;

                    if (rnd.Next(2) == 0)
                    {
                        horizontal = true;
                    }

                    if (this.AddShip(this.UnoccupiedSquares[rnd.Next(this.UnoccupiedSquares.Count)], new Ship(this, ship.Length, ship.Breadth), horizontal))
                    {
                        break;
                    }
                }
            }
        }
    }
}
