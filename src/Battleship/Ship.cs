namespace Battleships
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Defines a ship on the grid.
    /// </summary>
    public class Ship
    {
        public string type;

        public int length;

        public List<Square> occupiedSquares = new List<Square>();

        public Grid grid;

        public Ship(Grid grid, string type)
        {
            this.grid = grid;
            this.type = type;

            switch (type)
            {
                case "Carrier":
                    this.length = 5;
                    break;
                case "Battleship":
                    this.length = 4;
                    break;
                case "Cruiser":
                    this.length = 3;
                    break;
                case "Submarine":
                    this.length = 3;
                    break;
                case "Destroyer":
                    this.length = 2;
                    break;
                default:
                    throw new Exception("Ship type unknown.");
            }
        }
    }
}
