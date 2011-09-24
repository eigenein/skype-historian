using System;
using System.IO;
using System.Text.RegularExpressions;

namespace SkypeHistorian.Helpers
{
    internal static class PathExtensions
    {
        private static readonly Regex InvalidCharactersRegex;

        static PathExtensions()
        {
            string regexSearch = new string(Path.GetInvalidFileNameChars()) 
                + new string(Path.GetInvalidPathChars());
            InvalidCharactersRegex =
                new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
        }

        public static string FixPath(string path)
        {
            return InvalidCharactersRegex.Replace(path, String.Empty);
        }
    }
}
