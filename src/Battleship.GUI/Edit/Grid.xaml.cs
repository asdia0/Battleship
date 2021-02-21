namespace Battleship.GUI
{
    using System.Windows;
    using Battleship.Core;

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
            this.InitializeComponent();
            this.UpdateText();
        }

        /// <summary>
        /// Updates the screen.
        /// </summary>
        public void UpdateText()
        {
            this.Height_.Text = Settings.GridHeight.ToString();
            this.Width_.Text = Settings.GridWidth.ToString();
        }

        /// <summary>
        /// Fired when the Update button is clicked.
        /// </summary>
        /// <param name="sender">Reference.</param>
        /// <param name="e">Event.</param>
        public void Click_Update(object sender, RoutedEventArgs e)
        {
            this.Status.Content = string.Empty;

            string heightS = this.Height_.Text;
            string widthS = this.Width_.Text;

            int heightN = 0;
            int widthN = 0;

            bool heightB = int.TryParse(heightS, out heightN);
            bool widthB = int.TryParse(widthS, out widthN);

            if ((!heightB || !widthB) || (heightN <= 0 || widthN <= 0) || (heightN % 1 != 0 || widthN % 1 != 0))
            {
                this.Status.Content = "Error: Height and Width must be positive integers.";
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

                    Settings.Player = new Grid();

                    this.Status.Content = "Successfully edited grid.";

                    this.UpdateText();
                }
            }
        }
    }
}
