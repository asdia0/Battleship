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

    public partial class MainWindow : Window
    {
        Square recommendedSq;

        List<Square> searchedSquares = new List<Square>();

        public void Click_Find_Button(object sender, RoutedEventArgs args)
        {
            Grid player = Settings.Player;

            player.ToSearch.Remove(recommendedSq);
            player.ToAttack.Remove(recommendedSq);
            player.UnsearchedSquares.Remove(recommendedSq);

            recommendedSq.BeenSearched = true;
            searchedSquares.Add(recommendedSq);

            switch (this.Find_Combo.SelectedIndex)
            {
                // MISS
                case 0:
                    recommendedSq.BeenSearched = true;
                    recommendedSq.IsMiss = true;
                    this.Find();
                    break;
                // HIT
                case 1:
                    foreach (Square sq1 in recommendedSq.GetAdjacentSquares())
                    {
                        if (!sq1.BeenSearched && !player.SearchedSquares.Contains(sq1))
                        {
                            player.ToSearch.Add(sq1);
                        }
                    }

                    recommendedSq.IsHit = true;
                    recommendedSq.HadShip = true;
                    this.Find();
                    break;
                // SINK
                case 2:
                    this.SP_A.Visibility = Visibility.Collapsed;
                    this.SP_B.Visibility = Visibility.Visible;
                    break;
            }
        }

        public void Click_Find_Sunk_Button(object sender, RoutedEventArgs args)
        {
            Grid player = Settings.Player;
            string raw = this.Find_Sunk_SquaresText.Text;
            string shipIDS = this.Find_Sunk_ShipText.Text;

            if (!int.TryParse(shipIDS, out int shipID))
            {
                this.Find_Sunk_Label.Content = "Error: Ship ID should be an integer.";
            }

            if (player.OriginalShips.Count - 1 < shipID)
            {
                this.Find_Sunk_Label.Content = "Error: No ship was found.";
            }

            string[] xy = raw.Replace(")", string.Empty).Split(",(");
            foreach (Ship ship in player.Ships.ToList())
            {
                if (ship.ID == shipID)
                {
                    foreach (string xys in xy)
                    {
                        string s = xys.Replace("(", string.Empty);
                        int x = int.Parse(s.Split(",")[0]);
                        int y = int.Parse(s.Split(",")[1]);

                        int sqID1 = ((y - 1) * Settings.GridWidth) + x - 1;

                        ship.OriginalOccupiedSquares.Add(player.Squares[sqID1]);
                    }

                    foreach (Square square in ship.OriginalOccupiedSquares)
                    {
                        square.IsSunk = true;
                        square.HadShip = true;
                        square.IsHit = false;
                    }

                    ship.IsSunk = true;
                    player.Ships.Remove(ship);
                }
            }

            this.Find();
        }

        private void Find()
        {
            this.SP_A.Visibility = Visibility.Visible;
            this.SP_B.Visibility = Visibility.Collapsed;

            Grid player = Settings.Player;

            // Find Best Square
            (Square sq, Dictionary<int, int> prob) = Program.FindBestSquare(player);

            this.recommendedSq = sq;

            // Update Screen
            string text = "Dimensions || ID || Is Sunk";
            foreach (Ship ship in player.OriginalShips)
            {
                text += $"\n{ship.Length}x{ship.Breadth} || {ship.ID} || {ship.IsSunk}";
            }

            this.Find_ShipStats.Text = text;
            this.Find_Label.Content = $"Search {sq.ToCoor()} ({prob.Values.Max()}%)";
            this.Find_Combo.SelectedIndex = 0;

            // Update Image
            Bitmap bitmap = new Bitmap(Settings.GridWidth, Settings.GridHeight);

            for (var x = 0; x < bitmap.Width; x++)
            {
                for (var y = 0; y < bitmap.Height; y++)
                {
                    Color color = default(Color);
                    double percentage = (double)decimal.Divide(prob[x + (y * Settings.GridWidth)], prob.Values.Max());

                    Color Start = Color.FromArgb(1, 84, 90);
                    Color Center = Color.FromArgb(57, 122, 126);
                    Color End = Color.FromArgb(255, 255, 255);

                    Color Pick = GradientPick(1 - percentage, Start, Center, End);

                    color = Pick;
                    bitmap.SetPixel(x, y, color);
                }
            }

            foreach (Square square in this.searchedSquares)
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

            bitmap.SetPixel(this.recommendedSq.ToCoor().Item1 - 1, this.recommendedSq.ToCoor().Item2 - 1, Color.FromArgb(66, 155, 66));

            this.Image2.Source = this.BitmapToImageSource(this.ResizeBitmap(bitmap, 500, 500));
        }

        void HsvToRgb(double h, double S, double V, out int r, out int g, out int b)
        {
            double H = h;
            while (H < 0) { H += 360; };
            while (H >= 360) { H -= 360; };
            double R, G, B;
            if (V <= 0)
            { R = G = B = 0; }
            else if (S <= 0)
            {
                R = G = B = V;
            }
            else
            {
                double hf = H / 60.0;
                int i = (int)Math.Floor(hf);
                double f = hf - i;
                double pv = V * (1 - S);
                double qv = V * (1 - S * f);
                double tv = V * (1 - S * (1 - f));
                switch (i)
                {

                    // Red is the dominant color

                    case 0:
                        R = V;
                        G = tv;
                        B = pv;
                        break;

                    // Green is the dominant color

                    case 1:
                        R = qv;
                        G = V;
                        B = pv;
                        break;
                    case 2:
                        R = pv;
                        G = V;
                        B = tv;
                        break;

                    // Blue is the dominant color

                    case 3:
                        R = pv;
                        G = qv;
                        B = V;
                        break;
                    case 4:
                        R = tv;
                        G = pv;
                        B = V;
                        break;

                    // Red is the dominant color

                    case 5:
                        R = V;
                        G = pv;
                        B = qv;
                        break;

                    // Just in case we overshoot on our math by a little, we put these here. Since its a switch it won't slow us down at all to put these here.

                    case 6:
                        R = V;
                        G = tv;
                        B = pv;
                        break;
                    case -1:
                        R = V;
                        G = pv;
                        B = qv;
                        break;

                    // The color is not defined, we should throw an error.

                    default:
                        //LFATAL("i Value error in Pixel conversion, Value is %d", i);
                        R = G = B = V; // Just pretend its black/white
                        break;
                }
            }
            r = Clamp((int)(R * 255.0));
            g = Clamp((int)(G * 255.0));
            b = Clamp((int)(B * 255.0));
        }

        /// <summary>
        /// Clamp a value to 0-255
        /// </summary>
        int Clamp(int i)
        {
            if (i < 0) return 0;
            if (i > 255) return 255;
            return i;
        }

        public int LinearInterp(int start, int end, double percentage) => start + (int)Math.Round(percentage * (end - start));
        public Color ColorInterp(Color start, Color end, double percentage) =>
            Color.FromArgb(LinearInterp(start.A, end.A, percentage),
                           LinearInterp(start.R, end.R, percentage),
                           LinearInterp(start.G, end.G, percentage),
                           LinearInterp(start.B, end.B, percentage));
        public Color GradientPick(double percentage, Color Start, Color Center, Color End)
        {
            if (percentage < 0.5)
                return ColorInterp(Start, Center, percentage / 0.5);
            else if (percentage == 0.5)
                return Center;
            else
                return ColorInterp(Center, End, (percentage - 0.5) / 0.5);
        }
    }
}
