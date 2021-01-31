﻿namespace Battleships
{
    using System;
    using System.Linq;
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
                    if (this.squares[square.id + i].hasShip == true)
                    {
                        throw new Exception($"Square is already occupied by a {ship}.");
                    }

                    this.squares[square.id + i].hasShip = true;
                    this.squares[square.id + i].hadShip = true;
                    ship.occupiedSquares.Add(this.squares[square.id + i]);
                }

                this.ships.Add(ship);
            }

            // vertical
            else if (!alignment && ship.length <= availCols)
            {
                for (int i = 0; i < ship.length; i++)
                {
                    if (this.squares[i * 10 + square.id].hasShip == true)
                    {
                        throw new Exception($"Square is already occupied by a {ship}.");
                    }

                    this.squares[i * 10 + square.id].hasShip = true;
                    this.squares[i * 10 + square.id].hadShip = true;
                    ship.occupiedSquares.Add(this.squares[i * 10 + square.id]);
                }

                this.ships.Add(ship);
            }
        }

        public void Search(Square sq)
        {
            if (sq.beenSearched)
            {
                throw new Exception($"Square {sq.id} has already been searched.");
            }
            else
            {
                sq.beenSearched = true;

                if (sq.hasShip == true)
                {
                    sq.hasShip = false;

                    foreach (Ship sp in sq.grid.ships.ToList())
                    {
                        if (sp.occupiedSquares.Contains(sq))
                        {
                            sp.occupiedSquares.Remove(sq);

                            if (sp.occupiedSquares.Count == 0)
                            {
                                sq.grid.ships.Remove(sp);
                            }
                        }
                    }

                    if (sq.grid == Game.player)
                    {
                        Game.update += $"Your ship has been struck at Square {sq.id}.{Environment.NewLine}";
                    }
                    else
                    {
                        Game.update += $"You have struck an enemy ship.{Environment.NewLine}";
                    }
                }
            }
        }
    }
}
