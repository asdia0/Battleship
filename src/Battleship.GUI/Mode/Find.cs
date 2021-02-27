namespace Battleship.GUI
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Windows;

    using Battleship.Core;

    /// <summary>
    /// Find method.
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Number of moves so far.
        /// </summary>
        public int MoveCount = 0;

        /// <summary>
        /// Square to search.
        /// </summary>
        public Square BestSquare;

        /// <summary>
        /// The <see cref="MainWindow.Grid"/> one move back.
        /// </summary>
        public Grid PreviousGrid;

        /// <summary>
        /// List of searched squares.
        /// </summary>
        public List<Square> SearchedSquares = new List<Square>();

        /// <summary>
        /// Fired when the Submit button is clicked.
        /// </summary>
        /// <param name="sender">Reference.</param>
        /// <param name="e">Event.</param>
        public void Find_SubmitButton_OnClick(object sender, RoutedEventArgs e)
        {
            this.MoveCount++;

            if (this.MoveCount > 0)
            {
                this.Find_Undo.IsEnabled = true;
            }

            this.PreviousGrid = new Grid(Grid);

            Grid.ToSearch.Remove(this.BestSquare);
            Grid.ToAttack.Remove(this.BestSquare);
            Grid.UnsearchedSquares.Remove(this.BestSquare);

            this.BestSquare.BeenSearched = true;
            this.SearchedSquares.Add(this.BestSquare);

            this.Find_Sunk_Status.Content = this.Find_SquareStates.SelectedIndex.ToString();

            switch (this.Find_SquareStates.SelectedIndex)
            {
                // MISS
                case 0:
                    this.BestSquare.BeenSearched = true;
                    this.BestSquare.IsMiss = true;
                    this.Find();
                    break;

                // HIT
                case 1:
                    this.BestSquare.IsHit = true;
                    this.BestSquare.HadShip = true;
                    this.Find();
                    break;

                // SINK
                case 2:
                    this.Find_SP.Visibility = Visibility.Collapsed;
                    this.Find_Sunk_SP.Visibility = Visibility.Visible;
                    break;
            }
        }

        /// <summary>
        /// Fired when the Sunk Submit button is clicked.
        /// </summary>
        /// <param name="sender">Reference.</param>
        /// <param name="e">Event.</param>
        public void Find_SunkSubmitButton_OnClick(object sender, RoutedEventArgs e)
        {
            string raw = this.Find_Sunk_SquaresText.Text;
            string shipIDS = this.Find_Sunk_ShipText.Text;

            if (!int.TryParse(shipIDS, out int shipID))
            {
                this.Find_Sunk_Status.Content = "Error: Ship ID should be an integer.";
            }

            if (Grid.OriginalShips.Count - 1 < shipID)
            {
                this.Find_Sunk_Status.Content = "Error: No ship was found.";
            }

            string[] xy = raw.Replace(")", string.Empty).Split(",(");
            Ship ship = Grid.OriginalShips[shipID];

            ship.OriginalOccupiedSquares.Clear();

            foreach (string xys in xy)
            {
                string s = xys.Replace("(", string.Empty);
                int x = int.Parse(s.Split(",")[0]);
                int y = int.Parse(s.Split(",")[1]);

                int sqID1 = ((y - 1) * Settings.GridWidth) + x - 1;

                ship.OriginalOccupiedSquares.Add(Grid.Squares[sqID1]);
            }

            foreach (Square square in ship.OriginalOccupiedSquares)
            {
                square.IsSunk = true;
                square.HadShip = true;
                square.IsHit = false;
            }

            ship.IsSunk = true;
            Grid.Ships.Remove(ship);

            this.Find_Sunk_ShipText.Clear();
            this.Find_Sunk_SquaresText.Clear();

            this.Find();
        }

        /// <summary>
        /// Fired when the Undo button is clicked.
        /// </summary>
        /// <param name="sender">Reference.</param>
        /// <param name="e">Event.</param>
        public void Find_UndoButton_OnClick(object sender, RoutedEventArgs e)
        {
            Grid = this.PreviousGrid;

            this.Find_Undo.IsEnabled = false;

            this.MoveCount--;

            this.Find();
        }

        /// <summary>
        /// Gets the best square to search.
        /// </summary>
        public void Find()
        {
            if (this.MoveCount == 0)
            {
                this.Find_Undo.IsEnabled = false;
            }

            Grid.ToSearch.Clear();
            foreach (Square sq1 in Grid.Squares)
            {
                if (!this.SearchedSquares.Contains(sq1))
                {
                    foreach (Square adjSq in sq1.GetAdjacentSquares())
                    {
                        if (adjSq.IsHit == true)
                        {
                            Grid.ToSearch.Add(sq1);
                            break;
                        }
                    }
                }
            }

            this.Find_ShipStatus.Visibility = Visibility.Visible;
            this.Image2.Visibility = Visibility.Visible;
            this.Find_SP.Visibility = Visibility.Visible;
            this.Find_Sunk_SP.Visibility = Visibility.Collapsed;

            // Find Best Square
            (Square sq, Dictionary<int, int> prob) = Program.FindBestSquare(Grid);

            this.BestSquare = sq;

            // Update Screen
            string text = "Dimensions || ID || Is Sunk";
            foreach (Ship ship in Grid.OriginalShips)
            {
                text += $"\n{ship.Length}x{ship.Breadth} || {ship.ID} || {ship.IsSunk}";
            }

            this.Find_ShipStatus.Text = text;
            this.Find_Label.Content = $"Search {sq.ToCoor()} ({prob.Values.Max()}%)";
            this.Find_SquareStates.SelectedIndex = 0;

            // Update Image
            Bitmap bitmap = new Bitmap(Settings.GridWidth, Settings.GridHeight);

            for (var x = 0; x < bitmap.Width; x++)
            {
                for (var y = 0; y < bitmap.Height; y++)
                {
                    double percentage = (double)decimal.Divide(prob[x + (y * Settings.GridWidth)], prob.Values.Max());

                    Color start = Color.FromArgb(1, 84, 90);
                    Color center = Color.FromArgb(57, 122, 126);
                    Color end = Color.FromArgb(255, 255, 255);

                    bitmap.SetPixel(x, y, this.GradientPick(1 - percentage, start, center, end));
                }
            }

            foreach (Square square in this.SearchedSquares)
            {
                int x = square.ToCoor().Item1 - 1;
                int y = square.ToCoor().Item2 - 1;

                if (square.IsMiss == true)
                {
                    bitmap.SetPixel(x, y, Color.FromArgb(128, 128, 128));
                }

                if (square.IsHit == true)
                {
                    bitmap.SetPixel(x, y, Color.FromArgb(158, 80, 79));
                }

                if (square.IsSunk == true)
                {
                    bitmap.SetPixel(x, y, Color.Black);
                }
            }

            bitmap.SetPixel(this.BestSquare.ToCoor().Item1 - 1, this.BestSquare.ToCoor().Item2 - 1, Color.FromArgb(66, 155, 66));

            this.Image2.Source = this.BitmapToImageSource(this.ResizeBitmap(bitmap, 500, 500));
        }

        /// <summary>
        /// Clamp a value to 0-255.
        /// </summary>
        /// <param name="i">Integer to clamp.</param>
        /// <returns>An integer from 0 to 255.</returns>
        public int Clamp(int i)
        {
            if (i < 0)
            {
                return 0;
            }

            if (i > 255)
            {
                return 255;
            }

            return i;
        }

        /// <summary>
        /// Gets the linear intercept.
        /// </summary>
        /// <param name="start">First number.</param>
        /// <param name="end">Second number.</param>
        /// <param name="percentage">Percentage of line.</param>
        /// <returns>The linear intercept.</returns>
        public int LinearInterp(int start, int end, double percentage) => start + (int)Math.Round(percentage * (end - start));

        /// <summary>
        /// Gets the colour intercept.
        /// </summary>
        /// <param name="start">Start colour.</param>
        /// <param name="end">End colour.</param>
        /// <param name="percentage">Percentage of line.</param>
        /// <returns>The colour intercept.</returns>
        public Color ColorInterp(Color start, Color end, double percentage) =>
            Color.FromArgb(
                this.LinearInterp(start.A, end.A, percentage),
                this.LinearInterp(start.R, end.R, percentage),
                this.LinearInterp(start.G, end.G, percentage),
                this.LinearInterp(start.B, end.B, percentage));

        /// <summary>
        /// Gets a colour from the n-th% of a gradient.
        /// </summary>
        /// <param name="percentage">Percentage of gradient to get colour from.</param>
        /// <param name="start">Starting colour of gradient.</param>
        /// <param name="center">Center colour of gradient.</param>
        /// <param name="end">Ending colour of gradient.</param>
        /// <returns>A colour.</returns>
        public Color GradientPick(double percentage, Color start, Color center, Color end)
        {
            if (percentage < 0.5)
            {
                return this.ColorInterp(start, center, percentage / 0.5);
            }
            else if (percentage == 0.5)
            {
                return center;
            }
            else
            {
                return this.ColorInterp(center, end, (percentage - 0.5) / 0.5);
            }
        }
    }
}
