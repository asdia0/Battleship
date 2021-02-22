namespace Battleship.GUI
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Windows;

    using Battleship.Core;

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

        /// <summary>
        /// Fired when the Update button is clicked.
        /// </summary>
        /// <param name="sender">Reference.</param>
        /// <param name="e">Event.</param>
        public void Click_Update(object sender, RoutedEventArgs e)
        {
            Square sq = Settings.Player.Squares[this.Square.SelectedIndex];

            switch (this.State.SelectedIndex)
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

                        this.Status.Content = $"Successfully edited Square {this.Square.SelectedIndex}.";
                    }
                    else
                    {
                        this.Status.Content = "Error: Ship is sunk.";
                    }

                    break;
                case 1:
                    if (sq.Ship != null)
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
                    if (sq.Ship != null)
                    {
                        if (sq.Ship.IsSunk)
                        {
                            this.Status.Content = "Error: Ship is sunk.";
                        }
                        else if (sq.Ship.CurrentOccupiedSquares.Count == 1)
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
                    if (sq.Ship != null)
                    {
                        if (!sq.Ship.IsSunk)
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

        /// <summary>
        /// Updates the screen.
        /// </summary>
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

            if (sq.Ship != null)
            {
                this.Ship.Text = sq.Ship.ID.ToString();
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
    }
}
