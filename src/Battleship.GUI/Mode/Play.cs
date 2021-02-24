namespace Battleship.GUI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;

    using Battleship.Core;

    public partial class MainWindow : Window
    {
        /// <summary>
        /// The string version of <see cref="MainWindow.CurrentGame"/>.
        /// </summary>
        public string GameString;

        /// <summary>
        /// The current game.
        /// </summary>
        public Game CurrentGame = new Game();

        private void Play()
        {

        }
    }
}
