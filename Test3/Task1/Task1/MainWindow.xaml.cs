using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Task1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Button> fieldButtons;
        private GameManager gameManager;

        public MainWindow()
        {
            InitializeComponent();

            gameManager = new GameManager();
            fieldButtons = new List<Button>();

            foreach (var child in FormField.Children)
            {
                fieldButtons.Add(child as Button);
            }
        }

        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var coords = GetCoordinates(button.Name);
            var currentSignString = gameManager.GetSign();

            if (gameManager.TryMakeTurn(coords.Item1, coords.Item2))
            {
                button.Content = currentSignString;
                button.IsEnabled = false;
            }

            StatusTxt.Content = gameManager.GetStatus().ToString();
        }

        private (int, int) GetCoordinates(string name)
            => (int.Parse(name[6].ToString()), int.Parse(name[7].ToString()));

        private void ResetClick(object sender, RoutedEventArgs e)
        {
            gameManager.Reset();
            StatusTxt.Content = gameManager.GetStatus().ToString();

            foreach (var button in fieldButtons)
            {
                button.IsEnabled = true;
                button.Content = "";
            }
        }
    }
}
