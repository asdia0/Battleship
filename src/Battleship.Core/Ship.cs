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
        /// Determines if <see cref="ID"/> has been set.
        /// </summary>
        private bool IDSet = false;

        /// <summary>
        /// Determines if <see cref="Breadth"/> has been set.
        /// </summary>
        private bool BreadthSet = false;

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
        /// <see cref="Breadth"/>'s value.
        /// </summary>
        private int _Breadth;

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
                    if (this.Grid.OriginalShips.Count >= value)
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
        /// Gets or sets the ship's breadth.
        /// </summary>
        public int Breadth
        {
            get
            {
                return this._Breadth;
            }

            set
            {
                if (!this.BreadthSet)
                {
                    if (value < 1 || value > this.Grid.Breadth)
                    {
                        throw new BattleshipException($"Breadth must be between 1 and {this.Grid.Breadth} inclusive.");
                    }
                    else
                    {
                        this._Breadth = value;
                    }
                }
                else
                {
                    throw new BattleshipException("Breadth has already been set.");
                }
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
                HashSet<HashSet<int>> res = new HashSet<HashSet<int>>();

                foreach (Square sq in this.Grid.Squares)
                {
                    if (this.CanFit(sq, Alignment.Horizontal, true))
                    {
                        HashSet<int> arr = new HashSet<int>();

                        for (int i = 0; i < this.Length; i++)
                        {
                            arr.Add(sq.ID + i);
                        }

                        res.Add(arr);
                    }

                    if (this.CanFit(sq, Alignment.Vertical, true))
                    {
                        HashSet<int> arr = new HashSet<int>();

                        for (int i = 0; i < this.Length; i++)
                        {
                            arr.Add(sq.ID + (i * this.Grid.Length));
                        }

                        res.Add(arr);
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
        /// <param name="breadth">The ship's breadth.</param>
        public Ship(Grid grid, int length, int breadth)
        {
            this.Length = length;
            this.Grid = grid;
            this.ID = grid.OriginalShips.Count;
            this.Breadth = breadth;
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
            HashSet<Square> squares = new HashSet<Square>();

            switch (alignment)
            {
                case Alignment.Horizontal:
                    for (int i = 1; i <= this.Breadth; i++)
                    {
                        squares.UnionWith(square.GetNSquaresInDirection(this.Length, Direction.East));
                    }

                    break;
                case Alignment.Vertical:
                    for (int i = 1; i <= this.Length; i++)
                    {
                        squares.UnionWith(square.GetNSquaresInDirection(this.Breadth, Direction.East));
                    }

                    break;
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
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
