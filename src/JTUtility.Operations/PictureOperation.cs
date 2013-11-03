using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace JTUtility.Operations {

    /// <summary>
    /// Represents an operation performed on a single picture file.
    /// </summary>
    public class PictureOperation {

        #region Private Implementation

        private BackgroundWorker worker;
        private float progress;
        private string filePath;
        private List<PictureProcessType> processTypes;

        private void CreateBWCopy() {
            string bwPath = AdjustPath(OperationControl.BwEnd);
            Bitmap pic = new Bitmap(filePath);
            Bitmap bwPic = new Bitmap(pic);
            ConvertToBW(bwPic);
            if (File.Exists(bwPath)) {
                File.Delete(bwPath);
            }
            bwPic.Save(bwPath, ImageFormat.Jpeg);
            pic.Dispose();
            bwPic.Dispose();
        }

        private void ConvertToBW(Bitmap bitmap) {
            float prevProgress = progress;
            float ratio = 1F / processTypes.Count;
            for (int x = 0; x < bitmap.Width; x++) {
                for (int y = 0; y < bitmap.Height; y++) {
                    Color pixel = bitmap.GetPixel(x, y);
                    int bw = (int)((pixel.R * 0.299) + (pixel.G * 0.587) + (pixel.B * 0.114));
                    bitmap.SetPixel(x, y, Color.FromArgb(bw, bw, bw));
                }
                progress = prevProgress + ((x / (float)bitmap.Width) * ratio);
                worker.ReportProgress((int)progress);
            }
        }

        private void CreateSepiaCopy() {
            string sepiaPath = AdjustPath(OperationControl.SepiaEnd);
            Bitmap pic = new Bitmap(filePath);
            Bitmap sepiaPic = new Bitmap(pic);
            ConvertToSepia(sepiaPic);
            if (File.Exists(sepiaPath)) {
                File.Delete(sepiaPath);
            }
            sepiaPic.Save(sepiaPath, ImageFormat.Jpeg);
            pic.Dispose();
            sepiaPic.Dispose();
        }

        private void ConvertToSepia(Bitmap bitmap) {
            float prevProgress = progress;
            float ratio = 1F / processTypes.Count;
            for (int x = 0; x < bitmap.Width; x++) {
                for (int y = 0; y < bitmap.Height; y++) {
                    Color pixel = bitmap.GetPixel(x, y);
                    int r = (int)((pixel.R * 0.393) + (pixel.G * 0.769) + (pixel.B * 0.189));
                    int g = (int)((pixel.R * 0.349) + (pixel.G * 0.686) + (pixel.B * 0.168));
                    int b = (int)((pixel.R * 0.272) + (pixel.G * 0.534) + (pixel.B * 0.131));
                    bitmap.SetPixel(x, y, Color.FromArgb(r.Clamp255(), g.Clamp255(), b.Clamp255()));
                }
                progress = prevProgress + ((x / (float)bitmap.Width) * ratio);
                worker.ReportProgress((int)progress);
            }
        }

        private void CreateUnalteredCopy() {
            string unalteredPath = AdjustPath(OperationControl.UnalteredEnd);
            File.Copy(filePath, unalteredPath);
            progress += (1F / processTypes.Count) * 100F;
            worker.ReportProgress((int)progress);
        }

        private void CreateSecondUnalteredCopy() {
            string secondUnalteredPath = AdjustPath(OperationControl.SecondUnalteredEnd);
            File.Copy(filePath, secondUnalteredPath);
            progress += (1F / processTypes.Count) * 100F;
            worker.ReportProgress((int)progress);
        }

        private string AdjustPath(string suffix) {
            string extension = Path.GetExtension(filePath);
            return filePath.TrimEnd(extension.ToCharArray()) + " " + suffix + extension;
        }

        #endregion

        /// <summary>
        /// The path to the picture file subject to the operation.
        /// </summary>
        public string FilePath {
            get { return filePath; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PictureOperation"/> class.
        /// </summary>
        /// <param name="filePath">The path to the picture file.</param>
        /// <param name="processTypes">The process types to perform on the picture.</param>
        public PictureOperation(string filePath, List<PictureProcessType> processTypes) {
            this.filePath = filePath;
            this.processTypes = processTypes;
        }

        /// <summary>
        /// Processes the picture file asynchronously as the <see cref="BackgroundWorker.DoWork"/>
        /// event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">
        /// The <see cref="DoWorkEventArgs"/> instance containing the event data.
        /// </param>
        public void ProcessPicture(object sender, DoWorkEventArgs e) {
            worker = (BackgroundWorker)sender;
            progress = 0;
            for (int i = 0; i < processTypes.Count; i++) {
                switch (processTypes[i]) {
                    case PictureProcessType.CreateBwCopy:
                        CreateBWCopy();
                        break;
                    case PictureProcessType.CreateSepiaCopy:
                        CreateSepiaCopy();
                        break;
                    case PictureProcessType.CreateUnalteredCopy:
                        CreateUnalteredCopy();
                        break;
                    case PictureProcessType.CreateSecondUnalteredCopy:
                        CreateSecondUnalteredCopy();
                        break;
                }
            }
        }
    }
}