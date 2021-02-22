namespace Battleship.GUI
{
    using System.Collections.ObjectModel;
    using System.Collections.Generic;
    using System.Windows;

    using Battleship.Core;

    /// <summary>
    /// Interaction logic for Square.xaml.
    /// </summary>
    public partial class SquareEditor : Window
    {
        public ObservableCollection<int> SqSource = new ObservableCollection<int>();

        public List<string> States = new List<string>()
        {
            "Unsearched",
            "Miss",
            "Hit",
            "Sunk",
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="SquareEditor"/> class.
        /// </summary>
        public SquareEditor()
        {
            this.InitializeComponent();

            this.Square.ItemsSource = this.SqSource;
            this.State.ItemsSource = this.States;
            this.Square.SelectedIndex = 0;

            for (int i = 0; i < (Settings.GridHeight * Settings.GridWidth); i++)
            {
                this.SqSource.Add(i);
            }

            this.UpdateText();
        }

        public void Click_Update(object sender, RoutedEventArgs e)
        {
            Square sq = Settings.Player.Squares[this.Square.SelectedIndex];
            (bool, Ship?) res = this.ShipsContainSquare(sq);

            switch (this.State.SelectedIndex)
            {
                case 0:
                    if (res.Item1 && !res.Item2.IsSunk)
                    {
                        sq.BeenSearched = false;
                        if (res.Item1 == true)
                        {
                            sq.HadShip = true;
                            sq.HasShip = true;
                        }
                        sq.Ship.CurrentOccupiedSquares.Add(sq);
                        sq.IsSunk = false;
                        sq.IsMiss = false;
                        sq.IsHit = false;

                        this.Status.Content = $"Successfully edited Square {this.Square.SelectedIndex}.";
                    }
                    else
                    {
                        this.Status.Content = "Error: Ship is sunk.";
                    }
                    break;
                case 1:
                    if (res.Item1 == true)
                    {
                        this.Status.Content = "Error: Square has a ship.";
                    }
                    else
                    {
                        sq.HadShip = false;
                        sq.HasShip = false;
                        sq.BeenSearched = true;
                        sq.IsSunk = false;
                        sq.IsMiss = true;
                        sq.IsHit = false;

                        this.Status.Content = $"Successfully edited Square {this.Square.SelectedIndex}.";
                    }

                    break;
                case 2:
                    if (res.Item1 == true)
                    {
                        if (res.Item2.IsSunk)
                        {
                            this.Status.Content = "Error: Ship is sunk.";
                        }
                        else if (res.Item2.CurrentOccupiedSquares.Count == 1)
                        {
                            this.Status.Content = "Error: Active ship cannot have squares that are all hit.";
                        }
                        else
                        {
                            sq.BeenSearched = true;
                            sq.HasShip = false;
                            sq.HadShip = true;
                            sq.IsSunk = false;
                            sq.IsHit = true;
                            sq.IsMiss = false;
                            sq.Ship.CurrentOccupiedSquares.Remove(sq);

                            this.Status.Content = $"Successfully edited Square {this.Square.SelectedIndex}.";
                        }
                    }
                    else
                    {
                        this.Status.Content = "Error: Square does not have a ship.";
                    }

                    break;
                case 3:
                    if (res.Item1 == true)
                    {
                        if (!res.Item2.IsSunk)
                        {
                            this.Status.Content = "Error: Ship is not sunk.";
                        }
                        else
                        {
                            sq.BeenSearched = true;
                            sq.HasShip = false;
                            sq.HadShip = true;
                            sq.IsSunk = true;
                            sq.IsHit = false;
                            sq.IsMiss = false;

                            this.Status.Content = $"Successfully edited Square {this.Square.SelectedIndex}.";
                        }
                    }
                    else
                    {
                        this.Status.Content = "Error: Square does not have a ship.";
                    }

                    break;
            }
        }

        public (bool, Ship?) ShipsContainSquare(Square sq)
        {
            foreach (Ship ship in Settings.Player.OriginalShips)
            {
                if (ship.OriginalOccupiedSquares.Contains(sq))
                {
                    return (true, ship);
                }
            }

            return (false, null);
        }

        public void UpdateText()
        {
            this.Status.Content = string.Empty;

            Square sq = Settings.Player.Squares[this.Square.SelectedIndex];

            if (sq.BeenSearched)
            {
                if (sq.IsMiss == true)
                {
                    this.State.SelectedIndex = 1;
                }
                else
                {
                    if (sq.IsHit == true)
                    {
                        this.State.SelectedIndex = 2;
                    }
                    else
                    {
                        this.State.SelectedIndex = 3;
                    }
                }
            }
            else
            {
                this.State.SelectedIndex = 0;
            }

            (bool, Ship?) res = this.ShipsContainSquare(sq);
            if (res.Item1)
            {
                this.Ship.Text = res.Item2.ID.ToString();
            }
            else
            {
                this.Ship.Clear();
            }
        }

        private void Square_DropDownClosed(object sender, System.EventArgs e)
        {
            this.UpdateText();
        }

        private void InputIsValid()
        {

        }
    }
}
