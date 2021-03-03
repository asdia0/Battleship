namespace Battleship.GUI
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;

    using Battleship.Core;

    /// <summary>
    /// Main file.
    /// </summary>
    public partial class MainWindow : Window
    {
        public string GameString;

        private Grid human;

        private Grid computer;

        private int algorithmIndex;

        private Square selectedSquare;

        private List<Move> moveList;

        private void Play()
        {
            Game game = new Game();

            game.player1 = this.human;
            game.player2 = this.computer;

            game.Search(this.human, this.computer, this.selectedSquare);
            game.turn = false;

            switch (algorithmIndex)
            {
                // Random
                case 0:
                    game.Random();
                    break;

                // Hunt Target
                case 1:
                    game.HuntTarget();
                    break;
                
               // Probability Density
                case 2:
                    game.ProbabilityDensity();
                    break;
            }

            this.GameString = game.ToString();

            this.Play_UpdateScreen();

            if (!this.computer.Ships.Any())
            {
                MessageBox.Show("You won!", "Congratulations!");
                this.Play_SquareText.IsEnabled = false;
            }
            if (!this.human.Ships.Any())
            {
                MessageBox.Show("You lost.", "Try again next time.");
                this.Play_SquareText.IsEnabled = false;
            }
        }

        private void Play_Algorithm_SubmitButton_OnClick(object sender, RoutedEventArgs e)
        {
            this.algorithmIndex = this.Play_Algorithm_Choose.SelectedIndex;

            this.Play_Algorithm_SP.Visibility = Visibility.Hidden;
            this.Image1.Visibility = Visibility.Visible;
            this.Image2.Visibility = Visibility.Visible;
            this.Play_SP.Visibility = Visibility.Visible;
            this.Image1_Label.Visibility = Visibility.Visible;
            this.Image2_Label.Visibility = Visibility.Visible;

            this.human = new Grid(Settings.Grid);
            this.computer = new Grid();
            try
            {
                this.computer.AddShipsRandomly(Core.Settings.ShipList);
                this.Play_UpdateScreen();
            }
            catch
            {
                this.Play_Status.Content = "Error: Grid size too small";
            }
        }

        private void Play_SubmitButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (Play_CanProceed())
            {
                try
                {
                    this.Play();
                }
                catch
                {
                    this.Play_Status.Content = "Error: Square already searched.";
                }
            }
        }

        private bool Play_CanProceed()
        {
            // Parse square from string
            string sqS = this.Play_SquareText.Text;

            int x = 0;
            int y = 0;

            try
            {
                bool xBool1 = int.TryParse(sqS.Split(",")[0], out x);
                bool yBool1 = int.TryParse(sqS.Split(",")[1], out y);

                if (!xBool1 || !yBool1)
                {
                    this.Play_Status.Content = "Error: Square must be represented in the following format: x,y";
                    return false;
                }

                if (1 <= x && Settings.GridWidth >= x && 1 <=y && Settings.GridHeight >= y)
                {
                    int id = x - 1 + ((y - 1) * (int)Settings.GridWidth);

                    try
                    {
                        this.selectedSquare = this.computer.Squares[id];
                        return true;
                    }
                    catch
                    {
                        this.Play_Status.Content = $"Error: {id} is an invalid ID";
                        return false;
                    }
                }
                else
                {
                    this.Play_Status.Content = "Error: Invalid coordinates";
                    return false;
                }
            }
            catch
            {
                this.Play_Status.Content = "Error: Square must be represented in the following format: x,y";
                return false;
            }
        }

        private void Play_UpdateScreen()
        {
            string text = "Dimensions || ID || Is Sunk";
            foreach (Ship ship in this.computer.OriginalShips)
            {
                text += $"\n{ship.Length}x{ship.Breadth} || {ship.ID} || {ship.IsSunk}";
            }

            this.Play_ShipStats.Text = text;

            this.Play_Status.Content = string.Empty;

            this.Play_SquareText.Clear();


            // Bitmaps
            Bitmap bitmap1 = this.GetWhiteBitmap(Settings.GridWidth, Settings.GridHeight);
            Bitmap bitmap2 = this.GetWhiteBitmap(Settings.GridWidth, Settings.GridHeight);

            // Image 1: Human's grid (full view)
            foreach (Square square1 in this.human.Squares)
            {
                int x = square1.ToCoor().Item1 - 1;
                int y = square1.ToCoor().Item2 - 1;

                if (square1.BeenSearched)
                {
                    bitmap1.SetPixel(x, y, Color.FromArgb(128, 128, 128));
                }

                if (square1.IsHit == true)
                {
                    bitmap1.SetPixel(x, y, Color.FromArgb(158, 80, 79));
                }

                if (square1.IsSunk == true)
                {
                    bitmap1.SetPixel(x, y, Color.Black);
                }
            }

            this.Image1.Source = this.BitmapToImageSource(this.ResizeBitmap(bitmap1, 500, 500));

            // Image 2: Computer's grid (search view)
            foreach (Square square2 in this.computer.Squares)
            {
                int x = square2.ToCoor().Item1 - 1;
                int y = square2.ToCoor().Item2 - 1;

                if (square2.BeenSearched)
                {
                    bitmap2.SetPixel(x, y, Color.FromArgb(128, 128, 128));
                }

                if (square2.IsHit == true)
                {
                    bitmap2.SetPixel(x, y, Color.FromArgb(158, 80, 79));
                }

                if (square2.IsSunk == true)
                {
                    bitmap2.SetPixel(x, y, Color.Black);
                }
            }

            this.Image2.Source = this.BitmapToImageSource(this.ResizeBitmap(bitmap2, 500, 500));
        }

        private Bitmap GetWhiteBitmap(int width, int height)
        {
            Bitmap bitmap = new Bitmap(1, 1);

            bitmap.SetPixel(0, 0, Color.White);

            return this.ResizeBitmap(bitmap, width, height);
        }
    }
}
