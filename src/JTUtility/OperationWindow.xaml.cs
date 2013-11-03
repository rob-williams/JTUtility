using JTUtility.Operations;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace JTUtility {

    /// <summary>
    /// Interaction logic for OperationWindow.xaml
    /// </summary>
    public partial class OperationWindow : Window {

        private int completedCount;
        private List<BackgroundWorker> workers;
        private List<TextBlock> fileNameBlocks;
        private List<ProgressBar> progressBars;

        public OperationWindow() {
            InitializeComponent();
            OperationControl.InitializeOperations();
            completedCount = 0;
            workers = new List<BackgroundWorker>(OperationControl.Operations.Count);
            fileNameBlocks = new List<TextBlock>(OperationControl.Operations.Count);
            progressBars = new List<ProgressBar>(OperationControl.Operations.Count);
            PopulateGrid();
            UpdateTitle();
            StartWorkers();
        }

        private void PopulateGrid() {
            for (int i = 0; i < OperationControl.Operations.Count; i++) {
                PictureGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(30) });
                TextBlock fileNameBlock = new TextBlock {
                    Text = OperationControl.Operations[i].FilePath,
                    VerticalAlignment = System.Windows.VerticalAlignment.Center
                };
                Grid.SetColumn(fileNameBlock, 0);
                Grid.SetRow(fileNameBlock, i);
                PictureGrid.Children.Add(fileNameBlock);
                fileNameBlocks.Add(fileNameBlock);
                ProgressBar progressBar = new ProgressBar {
                    Margin = new Thickness(20, 5, 20, 5),
                    Minimum = 0,
                    Maximum = 100,
                    Value = 0
                };
                Grid.SetColumn(progressBar, 1);
                Grid.SetRow(progressBar, i);
                PictureGrid.Children.Add(progressBar);
                progressBars.Add(progressBar);
            }
        }

        private void StartWorkers() {
            for (int i = 0; i < OperationControl.Operations.Count; i++) {
                BackgroundWorker worker = new BackgroundWorker();
                worker.WorkerReportsProgress = true;
                worker.DoWork += OperationControl.Operations[i].ProcessPicture;
                worker.RunWorkerCompleted += OnWorkerComplete;
                worker.ProgressChanged += OnWorkerProgressChange;
                workers.Add(worker);
                worker.RunWorkerAsync();
            }
        }

        private void OnWorkerProgressChange(object sender, ProgressChangedEventArgs e) {
            int index = workers.IndexOf((BackgroundWorker)sender);
            progressBars[index].Value = e.ProgressPercentage;
        }

        private void OnWorkerComplete(object sender, RunWorkerCompletedEventArgs e) {
            int index = workers.IndexOf((BackgroundWorker)sender);
            progressBars[index].Value = 100;
            completedCount++;
            UpdateTitle();
        }

        private void UpdateTitle() {
            if (completedCount == OperationControl.Operations.Count) {
                TitleTextBlock.Text = "Complete!";
            }
            else {
                TitleTextBlock.Text = "Processed " + completedCount + " of "
                                      + OperationControl.Operations.Count + " Pictures";
            }
        }
    }
}