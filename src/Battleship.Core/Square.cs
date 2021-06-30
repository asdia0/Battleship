namespace Battleship.Core
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Defines a square on the grid.
    /// </summary>
    public class Square
    {
        /// <summary>
        /// Determines whether <see cref="Grid"/> has been set.
        /// </summary>
        private bool GridSet = false;

        /// <summary>
        /// Determines whether <see cref="ID"/> has been set.
        /// </summary>
        private bool IDSet = false;

        /// <summary>
        /// <see cref="Grid"/>'s value.
        /// </summary>
        private Grid _Grid;

        /// <summary>
        /// <see cref="ID"/>'s value.
        /// </summary>
        private int _ID;

        /// <summary>
        /// <see cref="Searched"/>'s value.
        /// </summary>
        private bool _Searched = false;

        /// <summary>
        /// <see cref="Ship"/>'s value.
        /// </summary>
        private Ship? _Ship = null;

        /// <summary>
        /// Gets or sets the square's grid.
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
        /// Gets or sets the square's unique identification tag.
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
                    int maxID = (Settings.GridWidth * Settings.GridHeight) - 1;

                    if (value < 0 || value > maxID)
                    {
                        throw new BattleshipException($"ID must be between 0 and {maxID} inclusive.");
                    }

                    if (this.Grid.Squares[value] == null)
                    {
                        this._ID = value;
                    }
                    else
                    {
                        throw new BattleshipException($"Square with ID {value} already exists in grid.");
                    }
                }
                else
                {
                    throw new BattleshipException("ID has already been set.");
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the square has been searched.
        /// </summary>
        public bool Searched
        {
            get
            {
                return this._Searched;
            }

            set
            {
                if (!this._Searched)
                {
                    this._Searched = true;
                }
                else
                {
                    throw new BattleshipException("Searched has alredy been set.");
                }
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Core.Ship"/> on the square. Null if there is no such ship.
        /// </summary>
        public Ship? Ship
        {
            get
            {
                return this._Ship;
            }

            set
            {
                if (value == null)
                {
                    throw new BattleshipException("Cannot set Ship as null.");
                }
                else if (this._Ship == null)
                {
                    this._Ship = value;
                }
                else
                {
                    throw new BattleshipException("Ship has already been set.");
                }
            }
        }

        /// <summary>
        /// Gets the square's position.
        /// </summary>
        public Position Position
        {
            get
            {
                int x = (this.ID % this.Grid.Breadth) + 1;
                int y = (int)Math.Floor((double)(this.ID / this.Grid.Breadth)) + 1;

                return new Position(x, y);
            }
        }

        /// <summary>
        /// Gets a collection of adjacent squares.
        /// </summary>
        public HashSet<Square> AdjacentSquares
        {
            get
            {
                HashSet<Square> res = new HashSet<Square>();

                try
                {
                    res.UnionWith(this.GetNSquaresInDirection(1, Direction.North));
                }
                catch (BattleshipException e)
                { }

                try
                {
                    res.UnionWith(this.GetNSquaresInDirection(1, Direction.East));
                }
                catch (BattleshipException e)
                { }

                try
                {
                    res.UnionWith(this.GetNSquaresInDirection(1, Direction.South));
                }
                catch (BattleshipException e)
                { }

                try
                {
                    res.UnionWith(this.GetNSquaresInDirection(1, Direction.West));
                }
                catch (BattleshipException e)
                { }

                return res;
            }
        }

        /// <summary>
        /// Gets the square's status.
        /// </summary>
        public SquareStatus Status
        {
            get
            {
                if (this.Searched)
                {
                    if (this.Ship == null)
                    {
                        return SquareStatus.Miss;
                    }

                    if (this.Ship.Status == ShipStatus.Sunk)
                    {
                        return SquareStatus.Sunk;
                    }
                    else
                    {
                        return SquareStatus.Hit;
                    }
                }
                else
                {
                    return SquareStatus.Unsearched;
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Square"/> class.
        /// </summary>
        /// <param name="grid">The square's grid.</param>
        /// <param name="id">The square's unique identification tag.</param>
        public Square(Grid grid, int id)
        {
            this.Grid = grid;
            this.ID = id;
        }

        /// <summary>
        /// Gets n squares from a certain direction from the square.
        /// </summary>
        /// <param name="n">The number of squares to get.</param>
        /// <param name="direction">The direction to take.</param>
        /// <returns>A specified number of squares in a specified direction.</returns>
        public HashSet<Square> GetNSquaresInDirection(int n, Direction direction)
        {
            HashSet<Square> res = new HashSet<Square>();

            int available = 0;

            switch (direction)
            {
                case Direction.North:
                    available = this.Grid.Length - this.Position.Y;

                    if (n < available)
                    {
                        throw new BattleshipException($"{n} is too big. Try a smaller value.");
                    }

                    for (int north = 1; north <= available; north++)
                    {
                        int id = this.ID + (north * this.Grid.Breadth);

                        res.Add(this.Grid.Squares[id]);
                    }

                    break;
                case Direction.South:
                    available = this.Position.Y - 1;

                    if (n < available)
                    {
                        throw new BattleshipException($"{n} is too big. Try a smaller value.");
                    }

                    for (int south = 1; south <= available; south++)
                    {
                        int id = this.ID - (south * this.Grid.Breadth);

                        res.Add(this.Grid.Squares[id]);
                    }

                    break;
                case Direction.West:
                    available = this.Position.X - 1;

                    if (n < available)
                    {
                        throw new BattleshipException($"{n} is too big. Try a smaller value.");
                    }

                    for (int west = 1; west <= available; west++)
                    {
                        int id = this.ID - west;

                        res.Add(this.Grid.Squares[id]);
                    }

                    break;
                case Direction.East:
                    available = this.Grid.Breadth - this.Position.X;

                    if (n < available)
                    {
                        throw new BattleshipException($"{n} is too big. Try a smaller value.");
                    }

                    for (int east = 1; east <= available; east++)
                    {
                        int id = this.ID + east;

                        res.Add(this.Grid.Squares[id]);
                    }

                    break;
            }

            return res;
        }
    }
}