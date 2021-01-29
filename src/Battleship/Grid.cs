namespace Battleships
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    /// <summary>
    /// Defines a grid.
    /// </summary>
    public class Grid
    {
        public (Square, bool)[] attacked = new (Square, bool)[100];

        public List<Ship> ships = new List<Ship>();

        public List<Square> squares = new List<Square>();

        public Grid()
        {
            AddSquares();
        }

        public void AddSquares()
        {
            for (int i = 0; i < 100; i++)
            {
                this.squares.Add(new Square(this, i));
            }
        }

        public void AddShip(Square square, Ship ship, bool alignment)
        {
            // alignment: true = horizontal, false = vertical
            // position: starting square

            if (ship.grid != square.grid)
            {
                throw new Exception("Attempted to add ship to non-local square.");
            }

            if (square.hasShip == true)
            {
                throw new Exception($"Square is already occupied by a {ship}.");
            }

            foreach (Ship sp in ships)
            {
                if (sp.type == ship.type)
                {
                    throw new Exception($"You cannot have two {ship.type}s.");
                }
            }

            int availRows = 11 - square.id % 10;
            int availCols = 11 - (int)Math.Floor((decimal)square.id / 10);

            // horizontal
            if (alignment && ship.length <= availRows)
            {
                for (int i = 0; i < ship.length; i++)
                {
                    this.squares[square.id + i].hasShip = true;
                    ship.occupiedSquares.Add(this.squares[i]);
                }

                this.ships.Add(ship);
            }

            // vertical
            else if (!alignment && ship.length <= availCols)
            {
                for (int i = 0; i < ship.length; i++)
                {
                    this.squares[i * 10 + square.id].hasShip = true;
                    ship.occupiedSquares.Add(this.squares[i * 10 + square.id]);
                }

                this.ships.Add(ship);
            }
        }

        public void Search(Square opponent)
        {
            if (opponent.beenSearched)
            {
                throw new Exception($"Square {opponent.id} has already been searched.");
            }

            else
            {
                opponent.beenSearched = true;

                if (opponent.hasShip == true)
                {
                    this.attacked[opponent.id] = (opponent, true);
                    opponent.hasShip = false;
                    foreach (Ship sp in opponent.grid.ships)
                    {
                        sp.occupiedSquares.Remove(opponent);
                    }

                    if (opponent.grid == Game.player)
                    {
                        Game.update += $"Your ship has been struck at Square {opponent.id}.{Environment.NewLine}";
                    }
                    else
                    {
                        Game.update += $"You have struck an enemy ship.{Environment.NewLine}";
                    }
                }

                else
                {
                    this.attacked[opponent.id] = (opponent, false);
                }
            }
        }
    }
}
