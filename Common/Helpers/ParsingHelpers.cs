namespace Common.Helpers
{
    using System.Linq;

    public static class ParsingHelpers
    {
        public static string ParseUrlFileName(string url)
        {
            url = url.Replace("%20", " ");
            return url.Split('/').Last();
        }
    }
}
