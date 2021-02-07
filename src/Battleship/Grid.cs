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
        /// The ships on the grid.
        /// </summary>
        public List<Ship> Ships = new List<Ship>();

        /// <summary>
        /// The grid's squares.
        /// </summary>
        public List<Square> Squares = new List<Square>();

        /// <summary>
        /// List of enemy squares to attack.
        /// </summary>
        public List<Square> ToAttack = new List<Square>();

        public List<Square> UnsearchedSquares = new List<Square>();

        public List<Square> UnoccupiedSquares = new List<Square>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Grid"/> class.
        /// </summary>
        public Grid()
        {
            this.AddSquares();
        }

        public Grid(string format)
        {
            this.AddSquares();

            for (int i = 0; i < format.Count(); i++)
            {
                char c = format.ToCharArray()[i];

                if (c == 'O')
                {
                    this.Squares[i].HadShip = true;
                    this.UnoccupiedSquares.Remove(this.Squares[i]);
                }
            }
        }

        /// <summary>
        /// Adds 100 squares to the grid.
        /// </summary>
        public void AddSquares()
        {
            for (int i = 0; i < Settings.gridWidth * Settings.gridHeight; i++)
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
        public bool AddShip(Square square, Ship ship, bool alignment)
        {
            if (ship.Grid != square.Grid)
            {
                throw new Exception("Attempted to add ship to non-local square.");
            }

            foreach (Ship sp in this.Ships)
            {
                if (sp.Type == ship.Type)
                {
                    throw new Exception($"You cannot have two {ship.Type}s.");
                }
            }

            int availRows = 11 - (square.ID % Settings.gridWidth);
            int availCols = 11 - (int)Math.Floor((decimal)square.ID / Settings.gridHeight);

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
                    ship.OccupiedSquares.Add(this.Squares[sqID]);
                    ship.arrangement.Add(this.Squares[sqID]);
                    this.UnoccupiedSquares.Remove(this.Squares[sqID]);
                }

                this.Ships.Add(ship);
                return true;
            }

            // vertical
            else if (!alignment && ship.Length <= availCols)
            {
                for (int i = 0; i < ship.Length; i++)
                {
                    int sqID = (i * Settings.gridHeight) + square.ID;

                    if (sqID >= this.Squares.Count || this.Squares[sqID].HasShip == true)
                    {
                        return false;
                    }

                    this.Squares[sqID].HasShip = true;
                    this.Squares[sqID].HadShip = true;
                    ship.OccupiedSquares.Add(this.Squares[sqID]);
                    ship.arrangement.Add(this.Squares[sqID]);
                    this.UnoccupiedSquares.Remove(this.Squares[sqID]);
                }

                this.Ships.Add(ship);

                return true;
            }

            return false;
        }

        public string ToString()
        {
            // . = no ship
            // O = ship

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
