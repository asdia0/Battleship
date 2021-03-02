namespace Battleship.GUI
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows;

    using Battleship.Core;

    /// <summary>
    /// Interaction logic for Ship.xaml.
    /// </summary>
    public partial class ShipEditor : Window
    {
        /// <summary>
        /// ID options.
        /// </summary>
        public ObservableCollection<dynamic> ShipIDSource = new ObservableCollection<dynamic>();

        /// <summary>
        /// Current ship selected.
        /// </summary>
        public Ship SelectedShip;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShipEditor"/> class.
        /// </summary>
        public ShipEditor()
        {
            this.InitializeComponent();

            this.ShipID.SelectedIndex = 1;

            this.ShipIDSource.Add(string.Empty);

            foreach (Ship sp in Settings.Grid.Ships)
            {
                this.ShipIDSource.Add(sp.ID);
            }

            this.ShipID.ItemsSource = this.ShipIDSource;

            this.SelectedShip = Settings.Grid.Ships[0];

            this.AddShip.IsEnabled = false;
            this.RemoveShip.IsEnabled = false;
            this.Submit.IsEnabled = true;

            this.Update();
        }

        /// <summary>
        /// Fired when the Submit button is clicked.
        /// </summary>
        /// <param name="sender">Reference.</param>
        /// <param name="e">Event.</param>
        private void Submit_OnClick(object sender, EventArgs e)
        {
            this.Status.Content = string.Empty;

            (bool, List<Square>, int, int) res = this.AbleToProceed(this.SelectedShip, true);

            if (res.Item1)
            {
                this.SelectedShip.Name = this.ShipName.Text;
                this.SelectedShip.IsSunk = (bool)this.ShipIsSunk.IsChecked;
                this.SelectedShip.OriginalOccupiedSquares = res.Item2;

                foreach (Square square in res.Item2)
                {
                    if (!square.BeenSearched)
                    {
                        this.SelectedShip.CurrentOccupiedSquares.Add(square);
                    }
                }

                this.UpdateSettings();

                this.Status.Content = $"Successfully edited Ship {this.SelectedShip.ID}.";
            }
        }

        /// <summary>
        /// Fired when the Add Ship button is clicked.
        /// </summary>
        /// <param name="sender">Reference.</param>
        /// <param name="e">Event.</param>
        private void AddShip_OnClick(object sender, EventArgs e)
        {
            this.Status.Content = string.Empty;

            int breadthN = 0;
            int lengthN = 0;

            Ship ship = new Ship(Settings.Grid, lengthN, breadthN);

            (bool, List<Square>, int, int) res = this.AbleToProceed(ship, false);

            if (res.Item1)
            {
                (bool?, Square) alignment = this.DetermineAlignment(res.Item3, res.Item4, res.Item2);

                if (alignment.Item1 != null)
                {
                    Ship addedShip = new Ship(Settings.Grid, res.Item3, res.Item4);

                    Settings.Grid.AddShip(alignment.Item2, addedShip, (bool)alignment.Item1);

                    addedShip.Name = this.ShipName.Text;
                    addedShip.IsSunk = (bool)this.ShipIsSunk.IsChecked;
                }

                this.UpdateSettings();

                this.ShipIDSource.Add(ship.ID);
                this.Status.Content = $"Successfully added {ship.Name}.";
            }
        }

        /// <summary>
        /// Fired when the Remove button is clicked.
        /// </summary>
        /// <param name="sender">Reference.</param>
        /// <param name="e">Event.</param>
        private void RemoveShip_OnClick(object sender, EventArgs e)
        {
            Settings.Grid.Ships.Remove(this.SelectedShip);
            Settings.Grid.OriginalShips.Remove(this.SelectedShip);

            int id = 0;

            foreach (Ship ship in Settings.Grid.OriginalShips)
            {
                ship.ID = id;
                id++;
            }

            this.ShipID.SelectedIndex--;
            this.Update();
            this.ShipIDSource.Remove(this.ShipIDSource[^1]);

            this.UpdateSettings();

            this.Status.Content = $"Successfully removed {this.SelectedShip.Name}.";
        }

        /// <summary>
        /// Fired when the ID Combobox closes.
        /// </summary>
        /// <param name="sender">Reference.</param>
        /// <param name="e">Event.</param>
        private void ShipID_OnDropDownClosed(object sender, EventArgs e)
        {
            this.Status.Content = string.Empty;

            if (this.ShipID.SelectedIndex == 0)
            {
                this.AddShip.IsEnabled = true;
                this.RemoveShip.IsEnabled = false;
                this.Submit.IsEnabled = false;

                this.ShipName.Text = string.Empty;
                this.ShipOccupiedSqs.Text = string.Empty;
                this.ShipLength.Text = string.Empty;
                this.ShipBreadth.Text = string.Empty;
                this.ShipIsSunk.IsChecked = false;
            }
            else
            {
                this.AddShip.IsEnabled = false;
                this.RemoveShip.IsEnabled = true;
                this.Submit.IsEnabled = true;

                if (this.ShipID.SelectedIndex == 1)
                {
                    this.RemoveShip.IsEnabled = false;
                }

                this.Update();
            }
        }

        /// <summary>
        /// Updates the screen.
        /// </summary>
        private void Update()
        {
            this.SelectedShip = Settings.Grid.OriginalShips[this.ShipID.SelectedIndex - 1];

            this.Status.Content = string.Empty;

            this.ShipName.Text = this.SelectedShip.Name;
            this.ShipLength.Text = this.SelectedShip.Length.ToString();
            this.ShipBreadth.Text = this.SelectedShip.Breadth.ToString();
            this.ShipIsSunk.IsChecked = this.SelectedShip.IsSunk;

            string text = string.Empty;
            foreach (Square sq in this.SelectedShip.OriginalOccupiedSquares)
            {
                text += $",{sq.ToCoor()}";
            }

            this.ShipOccupiedSqs.Text = text.Remove(0, 1).Replace(" ", string.Empty);
        }

        /// <summary>
        /// Determines the alignment of a given ship.
        /// </summary>
        /// <param name="length">The length of the ship.</param>
        /// <param name="breadth">The breadth of the ship.</param>
        /// <param name="list">The list of squares the ship occupies.</param>
        /// <returns>A boolean and <see cref="Square"/> tuples. True means horizontal, false means vertical. The square is the ship's starting square.</returns>
        private (bool?, Square) DetermineAlignment(int length, int breadth, List<Square> list)
        {
            List<int> sqIDs = new List<int>();
            foreach (Square sq in list)
            {
                sqIDs.Add(sq.ID);
            }

            sqIDs.Sort();

            if ((sqIDs.First() % Settings.GridWidth) - length == (sqIDs.Last() % Settings.GridWidth))
            {
                return (true, Settings.Grid.Squares[sqIDs.First()]);
            }
            else if ((sqIDs.First() % Settings.GridWidth) - breadth == (sqIDs.Last() % Settings.GridWidth))
            {
                return (false, Settings.Grid.Squares[sqIDs.First()]);
            }
            else
            {
                return (null, Settings.Grid.Squares[sqIDs.First()]);
            }
        }

        /// <summary>
        /// Determines if the inputs are valid.
        /// </summary>
        /// <param name="ship">Ship.</param>
        /// <param name="isUpdate">Is fired from Click_Update().</param>
        /// <returns>A boolean determining if the inputs are valid.</returns>
        private (bool, List<Square>, int, int) AbleToProceed(Ship ship, bool isUpdate)
        {
            (bool, List<Square>, int, int) res = (true, new List<Square>(), 0, 0);

            this.Status.Content = string.Empty;

            string occupiedSqs = this.ShipOccupiedSqs.Text;
            string lengthS = this.ShipLength.Text;
            string breadthS = this.ShipBreadth.Text;

            List<string> sqs = new List<string>();
            try
            {
                sqs = occupiedSqs.Remove(0, 1).Remove(occupiedSqs.Length - 2).Split("),(").ToList();
            }
            catch
            {
            }

            ship.OriginalOccupiedSquares.Clear();

            List<Square> potentialSqs = new List<Square>();

            foreach (string sqS in sqs)
            {
                int x = 0;
                int y = 0;

                try
                {
                    bool xBool1 = int.TryParse(sqS.Split(",")[0], out x);
                    bool yBool1 = int.TryParse(sqS.Split(",")[1], out y);

                    if (!xBool1 || !yBool1)
                    {
                        this.Status.Content = "Error: Squares must be represented in the following format: (x1,y1),(x2,y2) etc.";
                        res.Item1 = false;
                    }
                }
                catch
                {
                    this.Status.Content = "Error: Squares must be represented in the following format: (x1,y1),(x2,y2) etc.";
                    res.Item1 = false;
                }

                int id = x - 1 + ((y - 1) * (int)Settings.GridWidth);

                try
                {
                    potentialSqs.Add(Settings.Grid.Squares[id]);
                }
                catch
                {
                    this.Status.Content = $"Error: {id} is an invalid ID";
                    res.Item1 = false;
                }
            }

            foreach (Square sq in potentialSqs)
            {
                res.Item2.Add(sq);

                foreach (Ship ship1 in Settings.Grid.OriginalShips)
                {
                    if (ship1.OriginalOccupiedSquares.Contains(sq))
                    {
                        this.Status.Content = "Error: Ship cannot overlap another ship.";
                        res.Item1 = false;
                    }
                }
            }

            if (this.ShipID.SelectedIndex == 0 && isUpdate == true)
            {
                this.Status.Content = "Error: ID must be an integer";
                res.Item1 = false;
            }

            bool xBool = int.TryParse(breadthS, out int breadthN);
            bool yBool = int.TryParse(lengthS, out int lengthN);

            if (!xBool || !yBool)
            {
                this.Status.Content = "Error: Height and Width property must be an integer!";
                res.Item1 = false;
            }

            if (lengthN * breadthN != potentialSqs.Count)
            {
                this.Status.Content = "Error: Number of squares must be equal to length * breadth.";
                res.Item1 = false;
            }

            res.Item2 = potentialSqs;
            res.Item3 = lengthN;
            res.Item4 = breadthN;

            ship.Length = res.Item3;
            ship.Breadth = res.Item4;

            if (!this.ValidShipSquares(res.Item3, res.Item4, res.Item2))
            {
                this.Status.Content = "Error: Squares must be arranged together.";
                res.Item1 = false;
            }

            return res;
        }

        /// <summary>
        /// Determines if the inputted squares match up with the ship's dimensions.
        /// </summary>
        /// <param name="length">Length of ship.</param>
        /// <param name="breadth">Breadth of ship.</param>
        /// <param name="squares">List of squares inputted by user.</param>
        /// <returns>A boolean determining if the inputted squares match up with the ship's dimensions.</returns>
        private bool ValidShipSquares(int length, int breadth, List<Square> squares)
        {
            Ship ship = new Ship(new Grid(), length, breadth);

            foreach (List<int> arr in ship.GetArrangements())
            {
                int counter = 0;

                foreach (int sqID in arr)
                {
                    if (squares.Contains(Settings.Grid.Squares[sqID]))
                    {
                        counter++;
                    }
                }

                if (counter == squares.Count)
                {
                    return true;
                }
            }

            return false;
        }

        private void UpdateSettings()
        {
            Core.Settings.ShipList.Clear();

            foreach (Ship ship in Settings.Grid.Ships)
            {
                Core.Settings.ShipList.Add(ship);
            }
        }
    }
}
