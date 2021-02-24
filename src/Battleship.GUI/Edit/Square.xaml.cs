namespace Battleship.GUI
{
    using Battleship.Core;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Windows;

    /// <summary>
    /// Interaction logic for Square.xaml.
    /// </summary>
    public partial class SquareEditor : Window
    {
        /// <summary>
        /// Square options.
        /// </summary>
        public ObservableCollection<int> SqSource = new ObservableCollection<int>();

        /// <summary>
        /// State options.
        /// </summary>
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
            InitializeComponent();

            Square.ItemsSource = SqSource;
            State.ItemsSource = States;
            Square.SelectedIndex = 0;

            for (int i = 0; i < (Settings.GridHeight * Settings.GridWidth); i++)
            {
                SqSource.Add(i);
            }

            UpdateText();
        }

        /// <summary>
        /// Fired when the Update button is clicked.
        /// </summary>
        /// <param name="sender">Reference.</param>
        /// <param name="e">Event.</param>
        public void Click_Update(object sender, RoutedEventArgs e)
        {
            Square sq = Settings.Grid.Squares[Square.SelectedIndex];

            switch (State.SelectedIndex)
            {
                case 0:
                    if (sq.Ship != null && !sq.Ship.IsSunk)
                    {
                        sq.BeenSearched = false;
                        sq.HadShip = true;
                        sq.HasShip = true;
                        sq.Ship.CurrentOccupiedSquares.Add(sq);
                        sq.IsSunk = false;
                        sq.IsMiss = false;
                        sq.IsHit = false;

                        Status.Content = $"Successfully edited Square {Square.SelectedIndex}.";
                    }
                    else
                    {
                        Status.Content = "Error: Ship is sunk.";
                    }

                    break;
                case 1:
                    if (sq.Ship != null)
                    {
                        Status.Content = "Error: Square has a ship.";
                    }
                    else
                    {
                        sq.HadShip = false;
                        sq.HasShip = false;
                        sq.BeenSearched = true;
                        sq.IsSunk = false;
                        sq.IsMiss = true;
                        sq.IsHit = false;

                        Status.Content = $"Successfully edited Square {Square.SelectedIndex}.";
                    }

                    break;
                case 2:
                    if (sq.Ship != null)
                    {
                        if (sq.Ship.IsSunk)
                        {
                            Status.Content = "Error: Ship is sunk.";
                        }
                        else if (sq.Ship.CurrentOccupiedSquares.Count == 1)
                        {
                            Status.Content = "Error: Active ship cannot have squares that are all hit.";
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

                            Status.Content = $"Successfully edited Square {Square.SelectedIndex}.";
                        }
                    }
                    else
                    {
                        Status.Content = "Error: Square does not have a ship.";
                    }

                    break;
                case 3:
                    if (sq.Ship != null)
                    {
                        if (!sq.Ship.IsSunk)
                        {
                            Status.Content = "Error: Ship is not sunk.";
                        }
                        else
                        {
                            sq.BeenSearched = true;
                            sq.HasShip = false;
                            sq.HadShip = true;
                            sq.IsSunk = true;
                            sq.IsHit = false;
                            sq.IsMiss = false;

                            Status.Content = $"Successfully edited Square {Square.SelectedIndex}.";
                        }
                    }
                    else
                    {
                        Status.Content = "Error: Square does not have a ship.";
                    }

                    break;
            }
        }

        /// <summary>
        /// Updates the screen.
        /// </summary>
        public void UpdateText()
        {
            Status.Content = string.Empty;

            Square sq = Settings.Grid.Squares[Square.SelectedIndex];

            if (sq.BeenSearched)
            {
                if (sq.IsMiss == true)
                {
                    State.SelectedIndex = 1;
                }
                else
                {
                    if (sq.IsHit == true)
                    {
                        State.SelectedIndex = 2;
                    }
                    else
                    {
                        State.SelectedIndex = 3;
                    }
                }
            }
            else
            {
                State.SelectedIndex = 0;
            }

            if (sq.Ship != null)
            {
                Ship.Text = sq.Ship.ID.ToString();
            }
            else
            {
                Ship.Clear();
            }
        }

        private void Square_DropDownClosed(object sender, System.EventArgs e)
        {
            UpdateText();
        }
    }
}
