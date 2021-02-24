namespace Battleship.GUI
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Windows;
    using System.Windows.Media.Imaging;

    using Battleship.Core;
    using Microsoft.Win32;

    /// <summary>
    /// Interaction logic for MainWindow.xaml.
    /// </summary>
    public partial class MainWindow : Window
    {
        public static Grid grid;

        /// <summary>
        /// Determines whether the user is in play mode.
        /// </summary>
        public bool IsInPlayMode = false;

        /// <summary>
        /// State options.
        /// </summary>
        public List<string> States = new List<string>()
        {
            "Miss",
            "Hit",
            "Sunk",
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();
            Application.Current.MainWindow.WindowState = WindowState.Maximized;
            this.UpdateSaveGame();

            Settings.Grid.AddDefaultShips();
            grid = new Grid(Settings.Grid);

            this.Find_Combo.ItemsSource = this.States;

            this.SP_A.Visibility = Visibility.Collapsed;
            this.SP_B.Visibility = Visibility.Collapsed;
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
            grid = new Grid(Settings.Grid);

            this.searchedSquares.Clear();
            // if current mode == find:
            this.Find();
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
            this.Find();
        }
        #endregion

        // METHODS
        #region
        public BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }

        private Bitmap ResizeBitmap(Bitmap sourceBMP, int width, int height)
        {
            Bitmap result = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(result))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                g.DrawImage(sourceBMP, 0, 0, width, height);
            }
            return result;
        }

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
