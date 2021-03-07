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
            this.Update();
        }

        /// <summary>
        /// Fired when the Submit button is clicked.
        /// </summary>
        /// <param name="sender">Reference.</param>
        /// <param name="e">Event.</param>
        private void SubmitButton_OnClick(object sender, RoutedEventArgs e)
        {
            bool heightB, widthB;

            this.Status.Content = string.Empty;

            heightB = uint.TryParse(this.GridHeight.Text, out uint height);
            widthB = uint.TryParse(this.GridWidth.Text, out uint width);

            if (!heightB || !widthB)
            {
                this.Status.Content = "Error: Height and Width must be positive integers.";
            }
            else
            {
                MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure? Changing the grid dimensions will reset all ships and squares.", "Confirmation", MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    Settings.GridHeight = Core.Settings.GridHeight = (int)height;
                    Settings.GridWidth = Core.Settings.GridWidth = (int)width;

                    Settings.Grid = new Grid();

                    MainWindow.Grid = new Grid();

                    this.Status.Content = "Successfully edited grid.";

                    this.Update();
                }
            }
        }

        /// <summary>
        /// Updates the screen.
        /// </summary>
        private void Update()
        {
            this.GridHeight.Text = Settings.GridHeight.ToString();
            this.GridWidth.Text = Settings.GridWidth.ToString();
        }
    }
}
