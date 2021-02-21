using Battleship.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace Battleship.GUI
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class ShipEditor : Window
    {
        ObservableCollection<dynamic> IDSource = new ObservableCollection<dynamic>();

        Ship currentShip;

        public ShipEditor()
        {
            InitializeComponent();

            Settings.Player.AddShip(Settings.Player.Squares[0], new Ship(Settings.Player, 5, 1), true);

            ID.SelectedIndex = 1;

            IDSource.Add("");

            foreach (Ship sp in Settings.Player.Ships)
            {
                IDSource.Add(sp.ID);
            }

            ID.ItemsSource = IDSource;

            currentShip = Settings.Player.Ships[0];

            Add.IsEnabled = false;
            Remove.IsEnabled = false;
            Update.IsEnabled = true;

            this.UpdateText();
        }

        public void Click_Update(object e, EventArgs args)
        {
            Status.Content = string.Empty;

            string _name = Name.Text;
            bool _isSunk = (bool)IsSunk.IsChecked;

            (bool, List<Square>, int, int) res = AbleToProceed(currentShip, true);
            if (res.Item1)
            {
                foreach (Square sq in res.Item2)
                {
                    currentShip.OriginalOccupiedSquares.Add(sq);
                    currentShip.CurrentOccupiedSquares.Add(sq);
                    Settings.Player.UnoccupiedSquares.Remove(sq);

                    sq.HadShip = true;

                    if (!sq.BeenSearched)
                    {
                        sq.HasShip = true;
                    }
                    if (sq.IsSunk == true)
                    {
                        currentShip.CurrentOccupiedSquares.Remove(sq);
                    }
                }

                currentShip.Name = _name;
                currentShip.Length = res.Item3;
                currentShip.Breadth = res.Item4;
                currentShip.IsSunk = _isSunk;

                Status.Content = $"Successfully updated {currentShip.Name}.";
            }
        }

        private void Click_Add(object sender, EventArgs e)
        {
            Status.Content = string.Empty;

            string _name = Name.Text;
            bool _isSunk = (bool)IsSunk.IsChecked;

            int _breadth = 0;
            int _length = 0;

            Ship ship = new Ship(Settings.Player, _length, _breadth);

            (bool, List<Square>, int, int) res = AbleToProceed(ship, false);

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
                ship.Name = _name;
                ship.IsSunk = _isSunk;

                Settings.Player.Ships.Add(ship);
                Settings.Player.OriginalShips.Add(ship);

                IDSource.Add(ship.ID);
                Status.Content = $"Successfully added {ship.Name}.";
            }
        }

        private void Click_Remove(object sender, EventArgs e)
        {
            int shipID = currentShip.ID;

            Settings.Player.Ships.Remove(currentShip);
            Settings.Player.OriginalShips.Remove(currentShip);

            int id = 0;

            foreach (Ship ship in Settings.Player.OriginalShips)
            {
                ship.ID = id;
                id++;
            }

            ID.SelectedIndex--;
            this.UpdateText();
            IDSource.Remove(IDSource[IDSource.Count - 1]);
        }

        private void ID_DropDownClosed(object sender, EventArgs e)
        {
            Status.Content = string.Empty;

            if (ID.SelectedIndex == 0)
            {
                Add.IsEnabled = true;
                Remove.IsEnabled = false;
                Update.IsEnabled = false;

                Name.Text = string.Empty;
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

                this.UpdateText();
            }
        }

        private void UpdateText()
        {
            currentShip = Settings.Player.OriginalShips[ID.SelectedIndex - 1];

            Status.Content = string.Empty;

            Name.Text = currentShip.Name;
            Length.Text = currentShip.Length.ToString();
            Breadth.Text = currentShip.Breadth.ToString();
            IsSunk.IsChecked = currentShip.IsSunk;

            string text = string.Empty;
            foreach (Square sq in currentShip.OriginalOccupiedSquares)
            {
                text += $",{sq.ToCoor()}";
            }

            OccupiedSqs.Text = text.Remove(0, 1).Replace(" ", "");
        }

        private (bool, List<Square>, int, int) AbleToProceed(Ship ship, bool isUpdate)
        {
            (bool, List<Square>, int, int) res = (true, new List<Square>(), 0, 0);

            Status.Content = string.Empty;

            string _occupiedSqs = OccupiedSqs.Text;
            string _lengthS = Length.Text;
            string _breadthS = Breadth.Text;

            int _breadth = 0;
            int _length = 0;

            List<string> sqs = new List<string>();
            try
            {
                sqs = _occupiedSqs.Remove(0, 1).Remove(_occupiedSqs.Length - 2).Split("),(").ToList();
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
                    potentialSqs.Add(Settings.Player.Squares[id]);
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

                foreach (Ship ship1 in Settings.Player.Ships)
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

            bool xBool = int.TryParse(_breadthS, out _breadth);
            bool yBool = int.TryParse(_lengthS, out _length);

            if (!xBool || !yBool)
            {
                Status.Content = "Error: Height and Width property must be an integer!";
                res.Item1 = false;
            }

            if (_length * _breadth != potentialSqs.Count)
            {
                Status.Content = "Error: Number of squares must be equal to length * breadth.";
                res.Item1 = false;
            }

            res.Item2 = potentialSqs;
            res.Item3 = _length;
            res.Item4 = _breadth;

            if (!this.ValidShipSquares(res.Item3, res.Item4, res.Item2))
            {
                Status.Content = "Error: Squares must be arranged together.";
                res.Item1 = false;
            }

            return res;
        }

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
