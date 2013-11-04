using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace JTUtility.Operations {

    /// <summary>
    /// A container for the parameters and <see cref="PictureOperation"/>s needed to process a
    /// folder of pictures.
    /// </summary>
    public static class OperationControl {

        private static string[] picturePaths;
        private static List<PictureOperation> operations;
        private static ReadOnlyCollection<PictureOperation> roOperations;

        public static readonly string BwEnd = "(B&W)";
        public static readonly string SepiaEnd = "(Sepia)";
        public static readonly string UnalteredEnd = "(2)";
        public static readonly string SecondUnalteredEnd = "(3)";

        /// <summary>
        /// The path to the directory containing the picture files to process.
        /// </summary>
        public static string DirectoryPath = String.Empty;

        /// <summary>
        /// Indicates whether a black and white copy of each picture should be created with the
        /// "(B&W)" suffix.
        /// </summary>
        public static bool CreateBwCopy = true;

        /// <summary>
        /// Indicates whether a sepia copy of each picture should be created with the "(Sepia)"
        /// suffix.
        /// </summary>
        public static bool CreateSepiaCopy = true;

        /// <summary>
        /// Indicates whether an unaltered copy of each picture should be created with the "(2)"
        /// suffix.
        /// </summary>
        public static bool CreateUnalteredCopy = true;

        /// <summary>
        /// Indicates whether a second unaltered copy of each picture should be created with the
        /// "(3)" suffix.
        /// </summary>
        public static bool CreateSecondUnalteredCopy = true;

        /// <summary>
        /// Indicates whether the directory at <see cref="DirectoryPath"/> exists.
        /// </summary>
        /// <value></value>
        public static bool DirectoryExists {
            get { return Directory.Exists(DirectoryPath); }
        }

        /// <summary>
        /// The <see cref="PictureOperation"/>s for the current directory. Null until
        /// <see cref="InitializeOperations"/> is called.
        /// </summary>
        public static IList<PictureOperation> Operations {
            get { return roOperations; }
        }

        /// <summary>
        /// Gathers the paths to the picture files in the directory at <see cref="DirectoryPath"/>.
        /// </summary>
        /// <returns>The number of existing picture files that will need to be replaced.</returns>
        /// <exception cref="System.InvalidOperationException">
        /// DirectoryPath does not exist.
        /// </exception>
        public static int GatherPicturePaths() {
            if (!DirectoryExists) {
                throw new InvalidOperationException("DirectoryPath does not exist.");
            }
            picturePaths = Directory.GetFiles(DirectoryPath);
            //count the number of existing files that will need to be replaced
            int exists = picturePaths.Select(path => Path.GetFileNameWithoutExtension(path))
                                     .Count(path => (CreateBwCopy && path.EndsWith(BwEnd)) ||
                                                    (CreateSepiaCopy && path.EndsWith(SepiaEnd)) ||
                                                    (CreateUnalteredCopy &&
                                                     path.EndsWith(UnalteredEnd)) ||
                                                    (CreateSecondUnalteredCopy &&
                                                     path.EndsWith(SecondUnalteredEnd)));
            picturePaths = picturePaths.Where(path => !Path.GetExtension(path).EndsWith("tmp") &&
                                                      !Path.GetExtension(path).EndsWith("TMP") &&
                                                      !Path.GetFileNameWithoutExtension(path).EndsWith(BwEnd) &&
                                                      !Path.GetFileNameWithoutExtension(path).EndsWith(SepiaEnd) &&
                                                      !Path.GetFileNameWithoutExtension(path).EndsWith(UnalteredEnd) &&
                                                      !Path.GetFileNameWithoutExtension(path).EndsWith(SecondUnalteredEnd))
                                       .ToArray();
            return exists;
        }

        public static void InitializeOperations() {
            if (!DirectoryExists) {
                throw new InvalidOperationException("DirectoryPath does not exist.");
            }
            if (picturePaths == null) {
                throw new InvalidOperationException("GatherPicturePaths has not been called.");
            }
            operations = new List<PictureOperation>(picturePaths.Length);
            List<PictureProcessType> processTypes = new List<PictureProcessType>();
            if (CreateBwCopy) {
                processTypes.Add(PictureProcessType.CreateBwCopy);
            }
            if (CreateSepiaCopy) {
                processTypes.Add(PictureProcessType.CreateSepiaCopy);
            }
            if (CreateUnalteredCopy) {
                processTypes.Add(PictureProcessType.CreateUnalteredCopy);
            }
            if (CreateSecondUnalteredCopy) {
                processTypes.Add(PictureProcessType.CreateSecondUnalteredCopy);
            }
            foreach (string path in picturePaths) {
                operations.Add(new PictureOperation(path, processTypes));
            }
            roOperations = new ReadOnlyCollection<PictureOperation>(operations);
        }

        public static int Clamp255(this int rgbValue) {
            if (rgbValue < 0) {
                return 0;
            }
            else if (rgbValue > 255) {
                return 255;
            }
            return rgbValue;
        }
    }
}