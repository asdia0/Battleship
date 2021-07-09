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
        /// Determines if <see cref="ID"/> has been set.
        /// </summary>
        private bool IDSet = false;

        /// <summary>
        /// Determines if <see cref="Length"/> has been set.
        /// </summary>
        private bool LengthSet = false;

        /// <summary>
        /// Determines if <see cref="Grid"/> has been set.
        /// </summary>
        private bool GridSet = false;

        /// <summary>
        /// Determines if <see cref="Squares"/> has been set.
        /// </summary>
        private bool SquaresSet = false;

        /// <summary>
        /// <see cref="ID"/>'s value.
        /// </summary>
        private int _ID;

        /// <summary>
        /// <see cref="Length"/>'s value.
        /// </summary>
        private int _Length;

        /// <summary>
        /// <see cref="Grid"/>'s value.
        /// </summary>
        private Grid _Grid;

        /// <summary>
        /// <see cref="Squares"/>'s value.
        /// </summary>
        private HashSet<Square> _Squares;

        /// <summary>
        /// Gets or sets the ship's alignment.
        /// </summary>
        public Alignment Alignment { get; set; }

        /// <summary>
        /// Gets or sets the ship's name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the ship's ID.
        /// </summary>
        public int ID
        {
            get
            {
                return this._ID;
            }

            set
            {
                if (!this.IDSet)
                {
                    if (this.Grid.Ships.Where(i => i.ID == value).Any())
                    {
                        throw new BattleshipException($"Ship with ID {value} already exists.");
                    }
                    else
                    {
                        this._ID = value;
                    }
                }
                else
                {
                    throw new BattleshipException("ID has already been set.");
                }
            }
        }

        /// <summary>
        /// Gets the ship's breadth.
        /// </summary>
        public int Breadth
        {
            get
            {
                return 1;
            }
        }

        /// <summary>
        /// Gets or sets the ship's length.
        /// </summary>
        public int Length
        {
            get
            {
                return this._Length;
            }

            set
            {
                if (!this.LengthSet)
                {
                    if (value < 1 || value > this.Grid.Length)
                    {
                        throw new BattleshipException($"Length must be between 1 and {this.Grid.Length}.");
                    }
                    else
                    {
                        this._Length = value;
                    }
                }
                else
                {
                    throw new BattleshipException("Length has already been set.");
                }
            }
        }

        /// <summary>
        /// Gets or sets the ship's grid.
        /// </summary>
        public Grid Grid
        {
            get
            {
                return this._Grid;
            }

            set
            {
                if (!this.GridSet)
                {
                    this._Grid = value;
                }
                else
                {
                    throw new BattleshipException("Grid has already been set.");
                }
            }
        }

        /// <summary>
        /// Gets or sets the ship's squares.
        /// </summary>
        public HashSet<Square> Squares
        {
            get
            {
                return this._Squares;
            }

            set
            {
                if (!this.SquaresSet)
                {
                    if (value.Count != (this.Length * this.Breadth))
                    {
                        throw new BattleshipException("Number of squares must match the ship's length and breadth.");
                    }

                    this._Squares = value;
                }
                else
                {
                    throw new BattleshipException("Squares has already been set.");
                }
            }
        }

        /// <summary>
        /// Gets the unsearched squares in <see cref="Squares"/>.
        /// </summary>
        public HashSet<Square> UnsearchedSquares
        {
            get
            {
                return this.Squares.Where(i => i.Searched == false).ToHashSet();
            }
        }

        /// <summary>
        /// Gets the ship's status.
        /// </summary>
        public ShipStatus Status
        {
            get
            {
                return this.UnsearchedSquares.Count == 0 ? ShipStatus.Sunk : ShipStatus.Operational;
            }
        }

        /// <summary>
        /// Gets the collection of all of the ship's potential arrangements.
        /// </summary>
        public HashSet<HashSet<int>> Arrangements
        {
            get
            {
                HashSet<HashSet<int>> res = new ();

                foreach (Square square in this.Grid.Squares)
                {
                    if (this.CanFit(square, Alignment.Horizontal, true))
                    {
                        res.Add(square.GetNSquaresInDirection(this.Length, Direction.East).Select(i => i.ID).ToHashSet());
                    }

                    if (this.CanFit(square, Alignment.Vertical, true))
                    {
                        res.Add(square.GetNSquaresInDirection(this.Length, Direction.South).Select(i => i.ID).ToHashSet());
                    }
                }

                return res;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Ship"/> class.
        /// </summary>
        /// <param name="grid">The ship's grid.</param>
        /// <param name="length">The ship's length.</param>
        public Ship(Grid grid, int length)
        {
            this.Grid = grid;
            this.ID = grid.Ships.Count;
            this.Length = length;
        }

        /// <summary>
        /// Determines whether the ship can fit on the grid.
        /// </summary>
        /// <param name="square">The starting square where the ship will be "placed".</param>
        /// <param name="alignment">The alignment of the ship. True means horizontal and false means vertical.</param>
        /// <param name="acceptHitSquares">Determines if the ship can fit on squares whose status is <see cref="SquareStatus.Hit"/>.</param>
        /// <returns>A boolean.</returns>
        public bool CanFit(Square square, Alignment alignment, bool acceptHitSquares)
        {
            HashSet<Square> squares = new ();
            try
            {
                squares.Union(square.GetNSquaresInDirection(this.Length, alignment == Alignment.Horizontal ? Direction.East : Direction.South));
            }
            catch
            {
                return false;
            }

            // No obstructions
            if (!squares.Where(i => (i.Status == SquareStatus.Miss || i.Status == SquareStatus.Sunk)).Any())
            {
                if (acceptHitSquares)
                {
                    // Not all squares are hit
                    if (squares.Where(i => i.Status == SquareStatus.Hit).Count() != (this.Length * this.Breadth))
                    {
                        return true;
                    }
                }
                else
                {
                    // No squares are hit
                    if (!squares.Where(i => i.Status == SquareStatus.Hit).Any())
                    {
                        // No squares have a ship
                        if (!squares.Where(i => i.Ship != null).Any())
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }

            return false;
        }
    }
}
