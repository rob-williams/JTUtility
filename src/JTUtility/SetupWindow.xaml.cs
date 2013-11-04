using JTUtility.Operations;
using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using MessageBox = System.Windows.MessageBox;

namespace JTUtility {

    /// <summary>
    /// Interaction logic for SetupWindow.xaml
    /// </summary>
    public partial class SetupWindow : Window {

        public SetupWindow() {
            InitializeComponent();
            DirectoryName.Foreground = Brushes.DarkRed;
        }

        private void OnSelectDirectoryClick(object sender, RoutedEventArgs e) {
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
            folderBrowser.Description = "Select Folder to Process";
            folderBrowser.RootFolder = Environment.SpecialFolder.MyPictures;
            folderBrowser.ShowNewFolderButton = false;
            DialogResult result = folderBrowser.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK) {
                OperationControl.DirectoryPath = folderBrowser.SelectedPath;
            }
            if (OperationControl.DirectoryExists) {
                DirectoryName.Text = OperationControl.DirectoryPath;
                DirectoryName.Foreground = Brushes.Black;
                StartButton.IsEnabled = true;
            }
            else {
                DirectoryName.Text = "No Directory Selected";
                DirectoryName.Foreground = Brushes.DarkRed;
                StartButton.IsEnabled = false;
            }
        }

        private void OnBwChecked(object sender, RoutedEventArgs e) {
            OperationControl.CreateBwCopy = true;
        }

        private void OnBwUnchecked(object sender, RoutedEventArgs e) {
            OperationControl.CreateBwCopy = false;
        }

        private void OnSepiaChecked(object sender, RoutedEventArgs e) {
            OperationControl.CreateSepiaCopy = true;
        }

        private void OnSepiaUnchecked(object sender, RoutedEventArgs e) {
            OperationControl.CreateSepiaCopy = false;
        }

        private void OnUnalteredChecked(object sender, RoutedEventArgs e) {
            OperationControl.CreateUnalteredCopy = true;
        }

        private void OnUnalteredUnchecked(object sender, RoutedEventArgs e) {
            OperationControl.CreateUnalteredCopy = false;
        }

        private void OnSecondUnalteredChecked(object sender, RoutedEventArgs e) {
            OperationControl.CreateSecondUnalteredCopy = true;
        }

        private void OnSecondUnalteredUnchecked(object sender, RoutedEventArgs e) {
            OperationControl.CreateSecondUnalteredCopy = false;
        }

        private void OnStartClick(object sender, RoutedEventArgs e) {
            if (!OperationControl.DirectoryExists || !ConfirmOperation()) {
                return;
            }
            int conflictCount = OperationControl.GatherPicturePaths();
            if (conflictCount > 0 && !ConfirmFileOverwrite(conflictCount)) {
                return;
            }
            OperationWindow opWindow = new OperationWindow();
            opWindow.Show();
            Close();
        }

        private bool ConfirmOperation() {
            var result = MessageBox.Show(this, "Are you sure you want to process directory '"
                                             + OperationControl.DirectoryPath + "'?",
                                             "Confirm Operation", MessageBoxButton.YesNo);
            return result == MessageBoxResult.Yes;
        }

        private bool ConfirmFileOverwrite(int conflictCount) {
            var result = MessageBox.Show(this, conflictCount + " files have names that conflict "
                                         + "with the files to be created by this operation and "
                                         + "will be overwritten. Continue anyway?", "File Conflict",
                                         MessageBoxButton.YesNo);
            return result == MessageBoxResult.Yes;
        }
    }
}