using Battleship.Core;
using System.Collections.Generic;

namespace Battleship.GUI
{
    public class Settings
    {
        public static Grid Player = new Grid();

        public static Grid Computer;

        public static List<Ship> Ships;

        public static List<Square> Squares;

        public static int GridHeight = 10;

        public static int GridWidth = 10;
    }
}
