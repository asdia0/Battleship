namespace Battleship.GUI
{
    using System;
    using System.IO;
    using System.Windows;

    using Battleship.Core;
    using Microsoft.Win32;

    /// <summary>
    /// Interaction logic for MainWindow.xaml.
    /// </summary>
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

        /// <summary>
        /// Determines whether the user is in play mode.
        /// </summary>
        public bool IsInPlayMode = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();
            Application.Current.MainWindow.WindowState = WindowState.Maximized;
            this.UpdateSaveGame();

            Settings.Player.AddShip(Settings.Player.Squares[0], new Ship(Settings.Player, 5, 1), true);
        }

        // CONTROLS
        #region

        // FILE

        /// <summary>
        /// Fired when the New Game button is clicked.
        /// </summary>
        /// <param name="sender">Reference.</param>
        /// <param name="e">Event.</param>
        public void Click_New(object sender, RoutedEventArgs e)
        {
        }

        /// <summary>
        /// Fired when the Load Game button is clicked.
        /// </summary>
        /// <param name="sender">Reference.</param>
        /// <param name="e">Event.</param>
        public void Click_Load(object sender, RoutedEventArgs e)
        {
        }

        /// <summary>
        /// Fired when the Save Game button is clicked.
        /// </summary>
        /// <param name="sender">Reference.</param>
        /// <param name="e">Event.</param>
        public void Click_Save(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllText(saveFileDialog.FileName, "placeholder");
            }
        }

        // EDIT

        /// <summary>
        /// Fired when the ShipEditor button is clicked.
        /// </summary>
        /// <param name="sender">Reference.</param>
        /// <param name="e">Event.</param>
        public void Click_ShipEditor(object sender, RoutedEventArgs e)
        {
            Window shipEditor = new ShipEditor();
            shipEditor.Show();
        }

        /// <summary>
        /// Fired when the GridEditor button is clicked.
        /// </summary>
        /// <param name="sender">Reference.</param>
        /// <param name="e">Event.</param>
        public void Click_GridEditor(object sender, RoutedEventArgs e)
        {
            Window gridEditor = new GridEditor();
            gridEditor.Show();
        }

        /// <summary>
        /// Fired when the SquareEditor button is clicked.
        /// </summary>
        /// <param name="sender">Reference.</param>
        /// <param name="e">Event.</param>
        public void Click_SquareEditor(object sender, RoutedEventArgs e)
        {
            Window squareEditor = new SquareEditor();
            squareEditor.Show();
        }

        // MODE

        /// <summary>
        /// Fired when the Play button is clicked.
        /// </summary>
        /// <param name="sender">Reference.</param>
        /// <param name="e">Event.</param>
        public void Click_Play(object sender, RoutedEventArgs e)
        {
            this.IsInPlayMode = true;
            this.GameString = string.Empty;
            this.UpdateSaveGame();
        }

        /// <summary>
        /// Fired when the Simulate button is clicked.
        /// </summary>
        /// <param name="sender">Reference.</param>
        /// <param name="e">Event.</param>
        public void Click_Simulate(object sender, RoutedEventArgs e)
        {
            this.IsInPlayMode = false;
            this.UpdateSaveGame();
        }

        /// <summary>
        /// Fired when the Find button is clicked.
        /// </summary>
        /// <param name="sender">Reference.</param>
        /// <param name="e">Event.</param>
        public void Click_Find(object sender, RoutedEventArgs e)
        {
            this.IsInPlayMode = false;
            this.UpdateSaveGame();
        }
        #endregion

        // METHODS
        #region

        /// <summary>
        /// Enables or disables the Save Game button depending on whether the user in Play mode.
        /// </summary>
        public void UpdateSaveGame()
        {
            this.SaveGame.IsEnabled = this.IsInPlayMode;
        }
        #endregion

        /// <summary>
        /// Kills the application when the <see cref="MainWindow"/> is closed.
        /// </summary>
        /// <param name="e">Event.</param>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            Application.Current.Shutdown();
        }
    }
}
