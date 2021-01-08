using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Task1
{
    /// <summary>
    /// Represents main window.
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Model for manipulation.
        /// </summary>
        private readonly ClientViewModel model;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public MainWindow()
        {
            model = new ClientViewModel("..\\..\\..\\..\\Task1\\");
            model.ThrowError += (_, message) => ShowMessage(message);
            DataContext = model;

            InitializeComponent();
        }

        /// <summary>
        /// Opens a tip when app is launched.
        /// </summary>
        private void Window_ContentRendered(object sender, System.EventArgs e)
        {
            MessageBox.Show(Application.Current.MainWindow ?? throw new InvalidOperationException(),
                "To see subfolders: double-click on folder\n" +
                "To connect to server and see its contents: input address and port and click on \"Connect\" button\n" +
                "To set download folder: press \"choose\" button when inside of the desired folder\n" +
                "To download file from server: double-click on file\n", "Pro Tip!");
        }

        /// <summary>
        /// Validator letting only numbers in port field.
        /// </summary>
        private void PortTextBoxValidation(object sender, TextCompositionEventArgs e)
        {
            var regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        /// <summary>
        /// Handler for changing connection address.
        /// </summary>
        private void addressTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            model.isConnected = false;
        }

        /// <summary>
        /// Handler for changing connection port.
        /// </summary>
        private void portTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            model.isConnected = false;
        }

        /// <summary>
        /// Handles double-clicks on folders or files in server explorer.
        /// </summary>
        private async void HandleServerDoubleClick(object sender, RoutedEventArgs e)
        {
            await model.OpenServerFolderOrDownloadFile((sender as ListViewItem)?.Content.ToString());
        }

        /// <summary>
        /// Handles double-clicks on folders or files in client explorer.
        /// </summary>
        private void HandleClientDoubleClick(object sender, RoutedEventArgs e)
        {
            model.OpenClientFolder((sender as ListViewItem)?.Content.ToString());
        }

        /// <summary>
        /// Select button handler.
        /// </summary>
        private void ChooseFolder_Click(object sender, RoutedEventArgs e)
        {
            model.UpdateDownloadFolder();
        }

        /// <summary>
        /// Connection button handler.
        /// </summary>
        private async void Connect_Click(object sender, RoutedEventArgs e)
        {
            await model.Connect();
        }

        /// <summary>
        /// Server explorer's Back button handler.
        /// </summary>
        private async void BackServer_Click(object sender, RoutedEventArgs e)
        {
            await model.GoBackServer();
        }

        /// <summary>
        /// Client explorer's Back button handler.
        /// </summary>
        private void BackClient_Click(object sender, RoutedEventArgs e)
        {
            model.GoBackClient();
        }

        /// <summary>
        /// Download everything button handler.
        /// </summary>
        private async void DownloadEverything_Click(object sender, RoutedEventArgs e)
        {
            await model.DownloadAllFilesInCurrentDirectory();
        }

        /// <summary>
        /// Shows error message with specified text.
        /// </summary>
        private void ShowMessage(string errorMessage)
        {
            MessageBox.Show(errorMessage, "Error");
        }
    }
}
