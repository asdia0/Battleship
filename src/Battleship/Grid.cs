namespace Battleships
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
        /// Initializes a new instance of the <see cref="Grid"/> class.
        /// </summary>
        public Grid()
        {
            this.AddSquares();
        }

        /// <summary>
        /// Adds 100 squares to the grid.
        /// </summary>
        public void AddSquares()
        {
            for (int i = 0; i < 100; i++)
            {
                this.Squares.Add(new Square(this, i));
            }
        }

        /// <summary>
        /// Adds a ship to the grid.
        /// </summary>
        /// <param name="square">The ship's starting square.</param>
        /// <param name="ship">The ship to be added.</param>
        /// <param name="alignment">The ship's alignment.</param>
        public void AddShip(Square square, Ship ship, bool alignment)
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

            int availRows = 11 - (square.ID % 10);
            int availCols = 11 - (int)Math.Floor((decimal)square.ID / 10);

            // horizontal
            if (alignment && ship.Length <= availRows)
            {
                for (int i = 0; i < ship.Length; i++)
                {
                    if (this.Squares[square.ID + i].HasShip == true)
                    {
                        throw new Exception($"Square is already occupied by a {ship}.");
                    }

                    this.Squares[square.ID + i].HasShip = true;
                    this.Squares[square.ID + i].HadShip = true;
                    ship.OccupiedSquares.Add(this.Squares[square.ID + i]);
                }

                this.Ships.Add(ship);
            }

            // vertical
            else if (!alignment && ship.Length <= availCols)
            {
                for (int i = 0; i < ship.Length; i++)
                {
                    if (this.Squares[(i * 10) + square.ID].HasShip == true)
                    {
                        throw new Exception($"Square is already occupied by a {ship}.");
                    }

                    this.Squares[(i * 10) + square.ID].HasShip = true;
                    this.Squares[(i * 10) + square.ID].HadShip = true;
                    ship.OccupiedSquares.Add(this.Squares[(i * 10) + square.ID]);
                }

                this.Ships.Add(ship);
            }
        }

        /// <summary>
        /// Searches a square.
        /// </summary>
        /// <param name="sq">The square to search.</param>
        public void Search(Square sq)
        {
            if (sq.BeenSearched)
            {
                throw new Exception($"Square {sq.ID} has already been searched.");
            }
            else
            {
                sq.BeenSearched = true;

                if (sq.HasShip == true)
                {
                    sq.HasShip = false;

                    foreach (Ship sp in sq.Grid.Ships.ToList())
                    {
                        if (sp.OccupiedSquares.Contains(sq))
                        {
                            sp.OccupiedSquares.Remove(sq);

                            if (sp.OccupiedSquares.Count == 0)
                            {
                                sq.Grid.Ships.Remove(sp);
                            }
                        }
                    }
                }
            }
        }
    }
}
