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
        /// <summary>
        /// The current grid to work with. Cloned from <see cref="Settings.Grid"/>.
        /// </summary>
        public static Grid Grid;

        /// <summary>
        /// The mode the user is currently in. 
        /// (  True: Play
        ///   False: Simulate
        ///    Null: Find)
        /// </summary>
        public bool? CurrentMode = null;

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
        /// Algorithm options.
        /// </summary>
        public List<string> Algorithms = new List<string>()
        {
            "Random",
            "Hunt Target",
            "Probability Density",
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
            Grid = new Grid(Settings.Grid);

            this.ResetWindow();
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
            Grid = new Grid(Settings.Grid);

            this.SearchedSquares.Clear();

            this.ResetWindow();

            if (CurrentMode == true)
            {
                this.GameString = string.Empty;
                this.UpdateSaveGame();
            }
            else if (CurrentMode == false)
            {
                this.UpdateSaveGame();
                this.Sim_SP.Visibility = Visibility.Visible;

            }
            else
            {
                this.UpdateSaveGame();
                this.Find();
            }
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
            this.Title = "Battleship: Play";

            this.CurrentMode = true;
            this.GameString = string.Empty;

            this.ResetWindow();

            this.UpdateSaveGame();
        }

        /// <summary>
        /// Fired when the Simulate button is clicked.
        /// </summary>
        /// <param name="sender">Reference.</param>
        /// <param name="e">Event.</param>
        public void Click_Simulate(object sender, RoutedEventArgs e)
        {
            this.Title = "Battleship: Simulate";

            this.CurrentMode = false;
            this.UpdateSaveGame();
            this.ResetWindow();

            this.Sim_SP.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Fired when the Find button is clicked.
        /// </summary>
        /// <param name="sender">Reference.</param>
        /// <param name="e">Event.</param>
        public void Click_Find(object sender, RoutedEventArgs e)
        {
            this.Title = "Battleship: Find";

            this.CurrentMode = null;
            this.UpdateSaveGame();

            this.ResetWindow();

            this.Find();
        }
        #endregion

        // METHODS
        #region

        /// <summary>
        /// Enables or disables the Save Game button depending on whether the user in Play mode.
        /// </summary>
        public void UpdateSaveGame()
        {
            if (this.CurrentMode == true)
            {
                this.SaveGame.IsEnabled = true;
            }
            else
            {
                this.SaveGame.IsEnabled = false;
            }
        }

        /// <summary>
        /// Converts a Bitmap to an Image Source.
        /// </summary>
        /// <param name="bitmap">Bitmap to convert.</param>
        /// <returns>An Image Source.</returns>
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

        /// <summary>
        /// Kills the application when the <see cref="MainWindow"/> is closed.
        /// </summary>
        /// <param name="e">Event.</param>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            Application.Current.Shutdown();
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

        private void tb_LostKeyboardFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            //Make sure sender is the correct Control.
            if (sender is System.Windows.Controls.TextBox)
            {
                System.Windows.Controls.TextBox tb = (System.Windows.Controls.TextBox)sender;
                //If nothing was entered, reset default text.
                if (((System.Windows.Controls.TextBox)sender).Text.Trim().Equals(string.Empty))
                {
                    tb.Foreground = System.Windows.Media.Brushes.Gray;
                    if (tb == this.Sim_Games)
                    {
                        tb.Text = "Number of games to simulate";
                    }
                    if (tb == this.Find_Sunk_ShipText)
                    {
                        tb.Text = "Ship ID";

                    }
                    if (tb == this.Find_Sunk_SquaresText)
                    {
                        tb.Text = "Sunk squares";
                    }
                }
            }
        }

        private void tb_GotKeyboardFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            if (sender is System.Windows.Controls.TextBox)
            {
                //If nothing has been entered yet.
                if (((System.Windows.Controls.TextBox)sender).Foreground == System.Windows.Media.Brushes.Gray)
                {
                    ((System.Windows.Controls.TextBox)sender).Text = string.Empty;
                    ((System.Windows.Controls.TextBox)sender).Foreground = System.Windows.Media.Brushes.Black;
                }
            }
        }

        private void ResetWindow()
        {
            // SIMULATE
            this.Sim_Games.Foreground = System.Windows.Media.Brushes.Gray;
            this.Sim_Games.Text = "Number of games to simulate";

            this.Sim_Algo1.SelectedIndex = 0;
            this.Sim_Algo2.SelectedIndex = 0;

            this.Sim_Games.GotKeyboardFocus += new System.Windows.Input.KeyboardFocusChangedEventHandler(this.tb_GotKeyboardFocus);
            this.Sim_Games.LostKeyboardFocus += new System.Windows.Input.KeyboardFocusChangedEventHandler(this.tb_LostKeyboardFocus);

            this.Sim_Status.Content = string.Empty;

            this.Sim_Stats.Visibility = Visibility.Collapsed;

            this.Sim_Algo1.ItemsSource = this.Algorithms;
            this.Sim_Algo2.ItemsSource = this.Algorithms;

            this.Sim_SP.Visibility = Visibility.Collapsed;

            // FIND
            this.Find_Combo.ItemsSource = this.States;

            this.SP_A.Visibility = Visibility.Collapsed;
            this.SP_B.Visibility = Visibility.Collapsed;

            this.Image2.Visibility = Visibility.Collapsed;
            this.Find_ShipStats.Visibility = Visibility.Collapsed;
        }

        #endregion
    }
}
