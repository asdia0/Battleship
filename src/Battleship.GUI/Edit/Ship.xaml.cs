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
        public ObservableCollection<dynamic> IDSource = new ObservableCollection<dynamic>();

        /// <summary>
        /// Current ship selected.
        /// </summary>
        public Ship CurrentShip;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShipEditor"/> class.
        /// </summary>
        public ShipEditor()
        {
            this.InitializeComponent();

            Settings.Player.AddShip(Settings.Player.Squares[0], new Ship(Settings.Player, 5, 1), true);

            this.ID.SelectedIndex = 1;

            this.IDSource.Add(string.Empty);

            foreach (Ship sp in Settings.Player.Ships)
            {
                this.IDSource.Add(sp.ID);
            }

            this.ID.ItemsSource = this.IDSource;

            this.CurrentShip = Settings.Player.Ships[0];

            this.Add.IsEnabled = false;
            this.Remove.IsEnabled = false;
            this.Update.IsEnabled = true;

            this.UpdateText();
        }

        /// <summary>
        /// Fired when the Update button is clicked.
        /// </summary>
        /// <param name="sender">Reference.</param>
        /// <param name="e">Event.</param>
        public void Click_Update(object sender, EventArgs e)
        {
            this.Status.Content = string.Empty;

            string name = this.Name_.Text;
            bool isSunk = (bool)this.IsSunk.IsChecked;

            (bool, List<Square>, int, int) res = this.AbleToProceed(this.CurrentShip, true);
            if (res.Item1)
            {
                foreach (Square sq in res.Item2)
                {
                    this.CurrentShip.OriginalOccupiedSquares.Add(sq);
                    this.CurrentShip.CurrentOccupiedSquares.Add(sq);
                    Settings.Player.UnoccupiedSquares.Remove(sq);

                    sq.HadShip = true;

                    if (!sq.BeenSearched)
                    {
                        sq.HasShip = true;
                    }

                    if (sq.IsSunk == true)
                    {
                        this.CurrentShip.CurrentOccupiedSquares.Remove(sq);
                    }
                }

                this.CurrentShip.Name = name;
                this.CurrentShip.Length = res.Item3;
                this.CurrentShip.Breadth = res.Item4;
                this.CurrentShip.IsSunk = isSunk;

                this.Status.Content = $"Successfully updated {this.CurrentShip.Name}.";
            }
        }

        /// <summary>
        /// Fired when the Add button is clicked.
        /// </summary>
        /// <param name="sender">Reference.</param>
        /// <param name="e">Event.</param>
        private void Click_Add(object sender, EventArgs e)
        {
            this.Status.Content = string.Empty;

            string nameS = this.Name_.Text;
            bool isSunk = (bool)this.IsSunk.IsChecked;

            int breadthN = 0;
            int lengthN = 0;

            Ship ship = new Ship(Settings.Player, lengthN, breadthN);

            (bool, List<Square>, int, int) res = this.AbleToProceed(ship, false);

            if (res.Item1)
            {
                foreach (Square sq in res.Item2)
                {
                    ship.OriginalOccupiedSquares.Add(sq);
                    ship.CurrentOccupiedSquares.Add(sq);
                    Settings.Player.UnoccupiedSquares.Remove(sq);

                    sq.HadShip = true;

                    if (!sq.BeenSearched)
                    {
                        sq.HasShip = true;
                    }

                    if (sq.IsSunk == true)
                    {
                        ship.CurrentOccupiedSquares.Remove(sq);
                    }
                }

                ship.Length = res.Item3;
                ship.Breadth = res.Item4;
                ship.Name = nameS;
                ship.IsSunk = isSunk;

                Settings.Player.Ships.Add(ship);
                Settings.Player.OriginalShips.Add(ship);

                this.IDSource.Add(ship.ID);
                this.Status.Content = $"Successfully added {ship.Name}.";
            }
        }

        /// <summary>
        /// Fired when the Remove button is clicked.
        /// </summary>
        /// <param name="sender">Reference.</param>
        /// <param name="e">Event.</param>
        private void Click_Remove(object sender, EventArgs e)
        {
            int shipID = this.CurrentShip.ID;

            Settings.Player.Ships.Remove(this.CurrentShip);
            Settings.Player.OriginalShips.Remove(this.CurrentShip);

            int id = 0;

            foreach (Ship ship in Settings.Player.OriginalShips)
            {
                ship.ID = id;
                id++;
            }

            this.ID.SelectedIndex--;
            this.UpdateText();
            this.IDSource.Remove(this.IDSource[this.IDSource.Count - 1]);
        }

        /// <summary>
        /// Fired when the ID Combobox closes.
        /// </summary>
        /// <param name="sender">Reference.</param>
        /// <param name="e">Event.</param>
        private void ID_DropDownClosed(object sender, EventArgs e)
        {
            this.Status.Content = string.Empty;

            if (this.ID.SelectedIndex == 0)
            {
                this.Add.IsEnabled = true;
                this.Remove.IsEnabled = false;
                this.Update.IsEnabled = false;

                this.Name_.Text = string.Empty;
                this.OccupiedSqs.Text = string.Empty;
                this.Length.Text = string.Empty;
                this.Breadth.Text = string.Empty;
                this.IsSunk.IsChecked = false;
            }
            else
            {
                this.Add.IsEnabled = false;
                this.Remove.IsEnabled = true;
                this.Update.IsEnabled = true;

                if (this.ID.SelectedIndex == 1)
                {
                    this.Remove.IsEnabled = false;
                }

                this.UpdateText();
            }
        }

        /// <summary>
        /// Updates the screen.
        /// </summary>
        private void UpdateText()
        {
            this.CurrentShip = Settings.Player.OriginalShips[this.ID.SelectedIndex - 1];

            this.Status.Content = string.Empty;

            this.Name_.Text = this.CurrentShip.Name;
            this.Length.Text = this.CurrentShip.Length.ToString();
            this.Breadth.Text = this.CurrentShip.Breadth.ToString();
            this.IsSunk.IsChecked = this.CurrentShip.IsSunk;

            string text = string.Empty;
            foreach (Square sq in this.CurrentShip.OriginalOccupiedSquares)
            {
                text += $",{sq.ToCoor()}";
            }

            this.OccupiedSqs.Text = text.Remove(0, 1).Replace(" ", string.Empty);
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

            string occupiedSqs = this.OccupiedSqs.Text;
            string lengthS = this.Length.Text;
            string breadthS = this.Breadth.Text;

            int breadthN = 0;
            int lengthN = 0;

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

                int id = x - 1 + ((y - 1) * Settings.GridWidth);

                try
                {
                    potentialSqs.Add(Settings.Player.Squares[id]);
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

                foreach (Ship ship1 in Settings.Player.Ships)
                {
                    if (ship1.OriginalOccupiedSquares.Contains(sq))
                    {
                        this.Status.Content = "Error: Ship cannot overlap another ship.";
                        res.Item1 = false;
                    }
                }
            }

            if (this.ID.SelectedIndex == 0 && isUpdate == true)
            {
                this.Status.Content = "Error: ID must be an integer";
                res.Item1 = false;
            }

            bool xBool = int.TryParse(breadthS, out breadthN);
            bool yBool = int.TryParse(lengthS, out lengthN);

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
                    if (squares.Contains(Settings.Player.Squares[sqID]))
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
    }
}
