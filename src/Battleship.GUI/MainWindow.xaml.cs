using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using Microsoft.Win32;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Battleship.Core;

namespace Battleship.GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Game currentGame = new Game();

        public bool IsInPlayMode = false;

        public MainWindow()
        {
            InitializeComponent();
            Application.Current.MainWindow.WindowState = WindowState.Maximized;
            this.UpdateSaveGame();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            Application.Current.Shutdown();
        }

        // CONTROLS
        #region
        // FILE
        public void Click_New(object sender, RoutedEventArgs e)
        {

        }

        public void Click_Load(object sender, RoutedEventArgs e)
        {

        }

        public void Click_Save(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() == true)
                File.WriteAllText(saveFileDialog.FileName, "placeholder");
        }

        // EDIT
        public void Click_ShipEditor(object sender, RoutedEventArgs e)
        {
            Window shipEditor = new ShipEditor();
            shipEditor.Show();
        }

        public void Click_GridEditor(object sender, RoutedEventArgs e)
        {
            Window gridEditor = new GridEditor();
            gridEditor.Show();
        }

        // MODE
        public void Click_Play(object sender, RoutedEventArgs e)
        {
            this.IsInPlayMode = true;
            this.UpdateSaveGame();
        }

        public void Click_Simulate(object sender, RoutedEventArgs e)
        {
            this.IsInPlayMode = false;
            this.UpdateSaveGame();
        }

        public void Click_Find(object sender, RoutedEventArgs e)
        {
            this.IsInPlayMode = false;
            this.UpdateSaveGame();
        }
        #endregion

        //METHODS
        public void UpdateSaveGame()
        {
            SaveGame.IsEnabled = this.IsInPlayMode;
        }
    }
}
