namespace Battleship
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
        /// Determines whether <see cref="Ships"/> has been set.
        /// </summary>
        private bool OriginalShipsSet = false;

        /// <summary>
        /// <see cref="Ships"/> value.
        /// </summary>
        private List<Ship> _OriginalShips;

        /// <summary>
        /// Gets or sets a list of all <see cref="Ship"/>s on the grid..
        /// </summary>
        public List<Ship> Ships
        {
            get
            {
                return this._OriginalShips;
            }

            set
            {
                if (!this.OriginalShipsSet)
                {
                    this._OriginalShips = value;
                }
                else
                {
                    throw new BattleshipException("OriginalShips has already been set.");
                }
            }
        }

        /// <summary>
        /// Gets a collection of all operational <see cref="Ship"/>s on the grid.
        /// </summary>
        public HashSet<Ship> OperationalShips
        {
            get
            {
                return this.Ships.Where(i => i.Status == ShipStatus.Operational).ToHashSet();
            }
        }

        /// <summary>
        /// Gets a list of the grid's <see cref="Square"/>s.
        /// </summary>
        public List<Square> Squares { get; }

        /// <summary>
        /// Gets a collection of enemy squares to search.
        /// </summary>
        public HashSet<Square> ToSearch { get; }

        /// <summary>
        /// Gets a collection of enemy squares to attack.
        /// </summary>
        public HashSet<Square> ToAttack { get; }

        /// <summary>
        /// Gets a collection of the grid's unsearched <see cref="Square"/>s.
        /// </summary>
        public List<Square> UnsearchedSquares
        {
            get
            {
                return this.Squares.Where(i => i.Status == SquareStatus.Unsearched).ToList();
            }
        }

        /// <summary>
        /// Gets a collection of the grid's unoccupied <see cref="Square"/>s.
        /// </summary>
        public List<Square> UnoccupiedSquares
        {
            get
            {
                return this.Squares.Where(i => i.Ship == null).ToList();
            }
        }

        /// <summary>
        /// Gets or sets the grid's length.
        /// </summary>
        public int Length { get; }

        /// <summary>
        /// Gets or sets the grid's length.
        /// </summary>
        public int Breadth { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Grid"/> class.
        /// </summary>
        /// <param name="length">The length of the grid.</param>
        /// <param name="breadth">The braedth of the grid.</param>
        public Grid(int length, int breadth)
        {
            this.Ships = new();
            this.Squares = new();
            this.ToSearch = new();
            this.ToAttack = new();
            this.Length = length;
            this.Breadth = breadth;

            for (int id = 0; id < (this.Length * this.Breadth); id++)
            {
                this.Squares.Add(new Square(this, id));
            }
        }

        /// <summary>
        /// Adds a ship to the grid.
        /// </summary>
        /// <param name="square">The ship's starting square.</param>
        /// <param name="ship">The ship to be added.</param>
        /// <param name="alignment">The ship's alignment.</param>
        /// <returns>The task result.</returns>
        public bool AddShip(Square square, Ship ship, Alignment alignment)
        {
            if (ship.Grid != this)
            {
                throw new BattleshipException("Ship must be local.");
            }

            if (square.Grid != this)
            {
                throw new BattleshipException("Square must be local.");
            }

            if (ship.CanFit(square, alignment, false))
            {
                HashSet<Square> squares = new HashSet<Square>();

                switch (alignment)
                {
                    case Alignment.Horizontal:
                        for (int i = 0; i < ship.Breadth; i++)
                        {
                            int id = square.ID - (this.Length * i);

                            if (id < 0)
                            {
                                return false;
                            }

                            squares.UnionWith(this.Squares[id].GetNSquaresInDirection(ship.Length, Direction.East));
                        }

                        break;
                    case Alignment.Vertical:
                        for (int i = 0; i < ship.Length; i++)
                        {
                            int id = square.ID - (this.Breadth * i);

                            if (id < 0)
                            {
                                return false;
                            }

                            squares.UnionWith(this.Squares[id].GetNSquaresInDirection(ship.Breadth, Direction.East));
                        }

                        break;
                }

                if (squares.Where(i => i.Ship != null).Any())
                {
                    return false;
                }

                foreach (Square sq in squares)
                {
                    sq.Ship = ship;
                }

                ship.Squares = squares;

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
                if (sq.Ship != null)
                {
                    res += $"[{sq.Ship.ID}]";
                }
                else
                {
                    res += "[ ]";
                }
            }

            return res;
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

                    if (this.AddShip(this.UnoccupiedSquares[rnd.Next(this.UnoccupiedSquares.Count)], new Ship(this, ship.Length, ship.Breadth), horizontal ? Alignment.Horizontal : Alignment.Vertical))
                    {
                        break;
                    }
                }
            }
        }
    }
}
