namespace Battleship.GUI
{
    using Battleship.Core;
    using System.Windows;

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
