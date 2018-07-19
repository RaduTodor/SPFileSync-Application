namespace Common.Helpers
{
    using Common.Constants;
    using System.Linq;
    using System;

    /// <summary>
    /// This static class has some useful Parsing methods
    /// </summary>
    public static class ParsingHelpers
    {
        public const char Slash = '/';

        public const string Space = " ";

        /// <summary>
        /// From an url get the last part (filename...with extension)
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string ParseUrlFileName(string url)
        {
            url = url.Replace(HelpersConstant.SpaceReplaceUtfCode, Space);
            return url.Split(Slash).Last();
        }

        /// <summary>
        /// Returns the Parent Directory of an url file
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string ParseUrlParentDirectory(string url)
        {
            var uri = new Uri(url);
            var libraryUri = new Uri(uri.AbsoluteUri.Remove(uri.AbsoluteUri.Length - uri.Segments.Last().Length));
            var parentDirectory = libraryUri.Segments[DataAccessLayerConstants.LibrarySegmentNumber];
            parentDirectory = parentDirectory.Remove(parentDirectory.Length - 1);
            return parentDirectory.Replace(HelpersConstant.SpaceReplaceUtfCode, " ");
        }
    }
}
