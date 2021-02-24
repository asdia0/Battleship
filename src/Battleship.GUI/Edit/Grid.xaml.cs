namespace Battleship.GUI
{
    using Battleship.Core;
    using System.Windows;

    /// <summary>
    /// Interaction logic for Grid.xaml.
    /// </summary>
    public partial class GridEditor : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GridEditor"/> class.
        /// </summary>
        public GridEditor()
        {
            InitializeComponent();
            UpdateText();
        }

        /// <summary>
        /// Updates the screen.
        /// </summary>
        public void UpdateText()
        {
            Height_.Text = Settings.GridHeight.ToString();
            Width_.Text = Settings.GridWidth.ToString();
        }

        /// <summary>
        /// Fired when the Update button is clicked.
        /// </summary>
        /// <param name="sender">Reference.</param>
        /// <param name="e">Event.</param>
        public void Click_Update(object sender, RoutedEventArgs e)
        {
            Status.Content = string.Empty;

            string heightS = Height_.Text;
            string widthS = Width_.Text;

            bool heightB = int.TryParse(heightS, out int heightN);
            bool widthB = int.TryParse(widthS, out int widthN);

            if ((!heightB || !widthB) || (heightN <= 0 || widthN <= 0) || (heightN % 1 != 0 || widthN % 1 != 0))
            {
                Status.Content = "Error: Height and Width must be positive integers.";
            }
            else
            {
                MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure? Changing the grid dimensions will reset all ships and squares.", "Confirmation", MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    Settings.GridHeight = heightN;
                    Settings.GridWidth = widthN;
                    Core.Settings.GridHeight = heightN;
                    Core.Settings.GridWidth = widthN;

                    Settings.Grid = new Grid();
                    Settings.Grid.AddDefaultShips();

                    MainWindow.grid = new Grid(Settings.Grid);


                    Status.Content = "Successfully edited grid.";

                    UpdateText();
                }
            }
        }
    }
}
