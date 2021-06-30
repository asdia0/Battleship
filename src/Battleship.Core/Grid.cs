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
        /// Determines whether <see cref="Length"/> has been set.
        /// </summary>
        private bool LengthSet = false;

        /// <summary>
        /// Determines whether <see cref="Breadth"/> has been set.
        /// </summary>
        private bool BreadthSet = false;

        /// <summary>
        /// <see cref="Length"/>'s value.
        /// </summary>
        private int _Length;

        /// <summary>
        /// <see cref="Breadth"/>'s value.
        /// </summary>
        private int _Breadth;

        /// <summary>
        /// Gets a list of all <see cref="Ship"/>s on the grid..
        /// </summary>
        public List<Ship> OriginalShips { get; }

        /// <summary>
        /// Gets a collection of all operational <see cref="Ship"/>s on the grid.
        /// </summary>
        public HashSet<Ship> OperationalShips
        {
            get
            {
                return this.OriginalShips.Where(i => i.Status == ShipStatus.Operational).ToHashSet();
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
        public int Length
        {
            get
            {
                return this._Length;
            }

            set
            {
                if (!LengthSet)
                {
                    this._Length = value;
                }
                else
                {
                    throw new BattleshipException("Length has already been set.");
                }
            }
        }

        /// <summary>
        /// Gets or sets the grid's length.
        /// </summary>
        public int Breadth
        {
            get
            {
                return this._Breadth;
            }

            set
            {
                if (!BreadthSet)
                {
                    this._Breadth = value;
                }
                else
                {
                    throw new BattleshipException("Breadth has already been set.");
                }
            }
        }

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
                    sq.Ship = this.OperationalShips[square.Ship.ID];
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

            if (ship.CanFit(square, alignment))
            {
                switch ()
            }
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

                    if (this.AddShip(this.UnoccupiedSquares[rnd.Next(this.UnoccupiedSquares.Count)], new Ship(this, ship.Length, ship.Breadth), horizontal))
                    {
                        break;
                    }
                }
            }
        }
    }
}
