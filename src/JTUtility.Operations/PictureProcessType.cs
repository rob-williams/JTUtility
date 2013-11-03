namespace JTUtility.Operations {

    /// <summary>
    /// Represents the type of process to apply to a picture.
    /// </summary>
    public enum PictureProcessType {

        /// <summary>
        /// Creates a black and white copy of the picture.
        /// </summary>
        CreateBwCopy,

        /// <summary>
        /// Creates a sepia copy of the picture.
        /// </summary>
        CreateSepiaCopy,

        /// <summary>
        /// Creates an unaltered copy of the picture.
        /// </summary>
        CreateUnalteredCopy,

        /// <summary>
        /// Creates a second unaltered copy of the picture.
        /// </summary>
        CreateSecondUnalteredCopy
    }
}