namespace Battleship.GUI
{
    using Battleship.Core;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows;

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
            InitializeComponent();

            ID.SelectedIndex = 1;

            IDSource.Add(string.Empty);

            foreach (Ship sp in Settings.Grid.Ships)
            {
                IDSource.Add(sp.ID);
            }

            ID.ItemsSource = IDSource;

            CurrentShip = Settings.Grid.Ships[0];

            Add.IsEnabled = false;
            Remove.IsEnabled = false;
            Update.IsEnabled = true;

            UpdateText();
        }

        /// <summary>
        /// Fired when the Update button is clicked.
        /// </summary>
        /// <param name="sender">Reference.</param>
        /// <param name="e">Event.</param>
        public void Click_Update(object sender, EventArgs e)
        {
            Status.Content = string.Empty;

            (bool, List<Square>, int, int) res = AbleToProceed(CurrentShip, true);

            if (res.Item1)
            {
                (bool?, Square) hoz = HorizontalOrVertical(res.Item3, res.Item4, res.Item2);

                CurrentShip.Name = Name_.Text;
                CurrentShip.IsSunk = (bool)IsSunk.IsChecked;
                CurrentShip.OriginalOccupiedSquares = res.Item2;

                foreach (Square square in res.Item2)
                {
                    if (!square.BeenSearched)
                    {
                        CurrentShip.CurrentOccupiedSquares.Add(square);
                    }
                }

                Status.Content = $"Successfully edited Ship {CurrentShip.ID}.";
            }
        }

        /// <summary>
        /// Fired when the Add button is clicked.
        /// </summary>
        /// <param name="sender">Reference.</param>
        /// <param name="e">Event.</param>
        private void Click_Add(object sender, EventArgs e)
        {
            Status.Content = string.Empty;

            string nameS = Name_.Text;
            bool isSunk = (bool)IsSunk.IsChecked;

            int breadthN = 0;
            int lengthN = 0;

            Ship ship = new Ship(Settings.Grid, lengthN, breadthN);

            (bool, List<Square>, int, int) res = AbleToProceed(ship, false);

            if (res.Item1)
            {
                (bool?, Square) hoz = HorizontalOrVertical(res.Item3, res.Item4, res.Item2);

                if (hoz.Item1 != null)
                {
                    Settings.Grid.AddShip(hoz.Item2, new Ship(Settings.Grid, res.Item3, res.Item4), (bool)hoz.Item1);
                }

                IDSource.Add(ship.ID);
                Status.Content = $"Successfully added {ship.Name}.";
            }
        }

        private (bool?, Square) HorizontalOrVertical(int length, int breadth, List<Square> list)
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
        /// Fired when the Remove button is clicked.
        /// </summary>
        /// <param name="sender">Reference.</param>
        /// <param name="e">Event.</param>
        private void Click_Remove(object sender, EventArgs e)
        {
            Settings.Grid.Ships.Remove(CurrentShip);
            Settings.Grid.OriginalShips.Remove(CurrentShip);

            int id = 0;

            foreach (Ship ship in Settings.Grid.OriginalShips)
            {
                ship.ID = id;
                id++;
            }

            ID.SelectedIndex--;
            UpdateText();
            IDSource.Remove(IDSource[^1]);
        }

        /// <summary>
        /// Fired when the ID Combobox closes.
        /// </summary>
        /// <param name="sender">Reference.</param>
        /// <param name="e">Event.</param>
        private void ID_DropDownClosed(object sender, EventArgs e)
        {
            Status.Content = string.Empty;

            if (ID.SelectedIndex == 0)
            {
                Add.IsEnabled = true;
                Remove.IsEnabled = false;
                Update.IsEnabled = false;

                Name_.Text = string.Empty;
                OccupiedSqs.Text = string.Empty;
                Length.Text = string.Empty;
                Breadth.Text = string.Empty;
                IsSunk.IsChecked = false;
            }
            else
            {
                Add.IsEnabled = false;
                Remove.IsEnabled = true;
                Update.IsEnabled = true;

                if (ID.SelectedIndex == 1)
                {
                    Remove.IsEnabled = false;
                }

                UpdateText();
            }
        }

        /// <summary>
        /// Updates the screen.
        /// </summary>
        private void UpdateText()
        {
            CurrentShip = Settings.Grid.OriginalShips[ID.SelectedIndex - 1];

            Status.Content = string.Empty;

            Name_.Text = CurrentShip.Name;
            Length.Text = CurrentShip.Length.ToString();
            Breadth.Text = CurrentShip.Breadth.ToString();
            IsSunk.IsChecked = CurrentShip.IsSunk;

            string text = string.Empty;
            foreach (Square sq in CurrentShip.OriginalOccupiedSquares)
            {
                text += $",{sq.ToCoor()}";
            }

            OccupiedSqs.Text = text.Remove(0, 1).Replace(" ", string.Empty);
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

            Status.Content = string.Empty;

            string occupiedSqs = OccupiedSqs.Text;
            string lengthS = Length.Text;
            string breadthS = Breadth.Text;

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
                        Status.Content = "Error: Squares must be represented in the following format: (x1,y1),(x2,y2) etc.";
                        res.Item1 = false;
                    }
                }
                catch
                {
                    Status.Content = "Error: Squares must be represented in the following format: (x1,y1),(x2,y2) etc.";
                    res.Item1 = false;
                }

                int id = x - 1 + ((y - 1) * Settings.GridWidth);

                try
                {
                    potentialSqs.Add(Settings.Grid.Squares[id]);
                }
                catch
                {
                    Status.Content = $"Error: {id} is an invalid ID";
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
                        Status.Content = "Error: Ship cannot overlap another ship.";
                        res.Item1 = false;
                    }
                }
            }

            if (ID.SelectedIndex == 0 && isUpdate == true)
            {
                Status.Content = "Error: ID must be an integer";
                res.Item1 = false;
            }

            bool xBool = int.TryParse(breadthS, out int breadthN);
            bool yBool = int.TryParse(lengthS, out int lengthN);

            if (!xBool || !yBool)
            {
                Status.Content = "Error: Height and Width property must be an integer!";
                res.Item1 = false;
            }

            if (lengthN * breadthN != potentialSqs.Count)
            {
                Status.Content = "Error: Number of squares must be equal to length * breadth.";
                res.Item1 = false;
            }

            res.Item2 = potentialSqs;
            res.Item3 = lengthN;
            res.Item4 = breadthN;

            ship.Length = res.Item3;
            ship.Breadth = res.Item4;

            if (!ValidShipSquares(res.Item3, res.Item4, res.Item2))
            {
                Status.Content = "Error: Squares must be arranged together.";
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
    }
}
