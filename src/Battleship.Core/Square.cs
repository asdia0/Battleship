﻿namespace Battleship.Core
{
    using System;
    using System.Collections.Generic;

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

        public Ship Ship;

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

        /// <summary>
        /// Gets the x- and y-coordinates.
        /// </summary>
        /// <returns>The x- and y-coordinates.</returns>
        public (int, int) ToCoor()
        {
            int xCoor = (this.ID % Settings.GridWidth) + 1;
            int yCoor = (int)Math.Floor((double)(this.ID / Settings.GridWidth)) + 1;

            return (xCoor, yCoor);
        }

        /// <summary>
        /// Gets a list of adjacent squares.
        /// </summary>
        /// <returns>A list of adjacent squares.</returns>
        public List<Square> GetAdjacentSquares()
        {
            List<Square> res = new List<Square>();

            List<int> sqID = new List<int>()
            {
                this.ID - 1,
                this.ID + 1,
                this.ID - Settings.GridWidth,
                this.ID + Settings.GridWidth,
            };

            foreach (int id1 in sqID)
            {
                if (id1 > -1 && id1 < (Settings.GridHeight * Settings.GridWidth))
                {
                    res.Add(this.Grid.Squares[id1]);
                }
            }

            return res;
        }

        /// <summary>
        /// Gets the number of hit adjacent squares (HAS).
        /// </summary>
        /// <returns>The number of HAS.</returns>
        public int GetNumberOfHitAdjacentSquares()
        {
            int numberOfAdjacentHitSquares = 0;
            foreach (Square sq in this.GetAdjacentSquares())
            {
                if (sq.HadShip == true)
                {
                    numberOfAdjacentHitSquares++;
                }
            }

            return numberOfAdjacentHitSquares;
        }

        /// <summary>
        /// Gets the number of hit connected squares (HCS).
        /// </summary>
        /// <returns>The number of HCS.</returns>
        public int GetNumberOfHitConnectedSquares()
        {
            int res = 0;
            for (int x = 1; x <= (9 - this.ToCoor().Item1); x++)
            {
                Square sq = this.Grid.Squares[this.ID + x];
                if (sq.IsHit == true)
                {
                    res++;
                }
                else
                {
                    break;
                }
            }

            for (int x = 1; x <= (this.ToCoor().Item1 - 1); x++)
            {
                Square sq = this.Grid.Squares[this.ID - x];
                if (sq.IsHit == true)
                {
                    res++;
                }
                else
                {
                    break;
                }
            }

            for (int y = 1; y <= (9 - this.ToCoor().Item2); y++)
            {
                Square sq = this.Grid.Squares[this.ID + (y * Settings.GridWidth)];
                if (sq.IsHit == true)
                {
                    res++;
                }
                else
                {
                    break;
                }
            }

            for (int y = 1; y < (this.ToCoor().Item2 - 1); y++)
            {
                Square sq = this.Grid.Squares[this.ID - (y * Settings.GridWidth)];
                if (sq.IsHit == true)
                {
                    res++;
                }
                else
                {
                    break;
                }
            }

            return res;
        }
    }
}