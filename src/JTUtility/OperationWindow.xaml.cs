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
        private int workerIndex;
        private List<BackgroundWorker> workers;
        private List<TextBlock> filePathBlocks;
        private List<ProgressBar> progressBars;

        public OperationWindow() {
            InitializeComponent();
            OperationControl.InitializeOperations();
            completedCount = 0;
            workerIndex = 0;
            workers = new List<BackgroundWorker>(OperationControl.Operations.Count);
            filePathBlocks = new List<TextBlock>(OperationControl.Operations.Count);
            progressBars = new List<ProgressBar>(OperationControl.Operations.Count);
            PopulateGrid();
            UpdateTitle();
            CreateWorkers();
            AdvanceWorkers();
        }

        private void PopulateGrid() {
            for (int i = 0; i < OperationControl.Operations.Count; i++) {
                //add a row definition for this picture's row
                PictureGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(30) });
                //create a TextBlock to display the picture's file path
                TextBlock filePathBlock = new TextBlock {
                    Text = OperationControl.Operations[i].FilePath,
                    VerticalAlignment = System.Windows.VerticalAlignment.Center
                };
                Grid.SetColumn(filePathBlock, 0);
                Grid.SetRow(filePathBlock, i);
                PictureGrid.Children.Add(filePathBlock);
                filePathBlocks.Add(filePathBlock);
                //create a ProgressBar to display the picture's operation progress
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

        private void CreateWorkers() {
            for (int i = 0; i < OperationControl.Operations.Count; i++) {
                BackgroundWorker worker = new BackgroundWorker();
                worker.WorkerReportsProgress = true;
                worker.DoWork += OperationControl.Operations[i].ProcessPicture;
                worker.RunWorkerCompleted += OnWorkerComplete;
                worker.ProgressChanged += OnWorkerProgressChange;
                workers.Add(worker);
            }
        }

        private void AdvanceWorkers() {
            //if we've already ran all workers, just ignore this call
            if (workerIndex >= OperationControl.Operations.Count) {
                return;
            }
            int prevIndex = workerIndex;
            //we want to run at most 5 workers at a time
            workerIndex += 5;
            if (workerIndex > OperationControl.Operations.Count) {
                workerIndex = OperationControl.Operations.Count;
            }
            for (int i = prevIndex; i < workerIndex; i++) {
                workers[i].RunWorkerAsync();
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
            if (completedCount >= workerIndex) {
                AdvanceWorkers();
            }
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