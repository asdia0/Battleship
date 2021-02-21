using System.Windows;
using Battleship.Core;

namespace Battleship.GUI
{
    /// <summary>
    /// Interaction logic for Grid.xaml
    /// </summary>
    public partial class GridEditor : Window
    {
        public GridEditor()
        {
            InitializeComponent();
            Height.Text = Settings.GridHeight.ToString();
            Width.Text = Settings.GridWidth.ToString();
        }

        public void Click_Update(object e, RoutedEventArgs args)
        {
            Status.Content = string.Empty;

            string _heightS = Height.Text;
            string _widthS = Width.Text;

            int _height = 0;
            int _width = 0;

            bool _heightB = int.TryParse(_heightS, out _height);
            bool _widthB = int.TryParse(_widthS, out _width);

            if ((!_heightB || !_widthB) || (_height <= 0 || _width <= 0) || (_height % 1 != 0 || _width % 1 != 0))
            {
                Status.Content = "Error: Height and Width must be positive integers.";
            }
            else
            {
                Settings.GridHeight = _height;
                Settings.GridWidth = _width;
                Core.Settings.GridHeight = _height;
                Core.Settings.GridWidth = _width;

                Settings.Player = new Grid();

                Status.Content = "Successfully edited grid.";
            }
        }
    }
}
