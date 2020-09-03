using System.IO;

namespace Task1Tests
{
    /// <summary>
    /// Testing tools to optimize codebase.
    /// </summary>
    public static class Tools
    {
        /// <summary>
        /// Writes specified text to the file with specified path.
        /// </summary>
        /// <param name="path">Path to the file.</param>
        /// <param name="text">Text to write.</param>
        public static void Write(string path, string text)
        {
            using (var streamWriter = new StreamWriter(path))
            {
                streamWriter.Write(text);
            }
        }

        /// <summary>
        /// Reads from the file and returns it's string.
        /// </summary>
        /// <param name="path">Path to read from.</param>
        public static string Read(string path)
        {
            using (var streamReader = new StreamReader(path))
            {
                return streamReader.ReadToEnd();
            }
        }
    }
}
