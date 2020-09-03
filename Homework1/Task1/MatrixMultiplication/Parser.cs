using System;
using System.IO;

namespace Task1
{
    /// <summary>
    /// Class used to parse .txt files and build a matrix from it.
    /// </summary>
    public static class Parser
    {
        /// <summary>
        /// Method which is called when building a matrix from a text file.
        /// </summary>
        /// <param name="path">Path to the file.</param>
        /// <returns>Built matrix.</returns>
        public static Matrix Parse(string path)
        {
            using (var streamReader = new StreamReader(path))
            {
                var width = 1;
                var height = 0;

                string currentRowString;
                var splitter = new char[] { ' ' };


                if ((currentRowString = streamReader.ReadLine()) != null)
                {
                    height++;

                    width = currentRowString.Split(
                        splitter, StringSplitOptions.RemoveEmptyEntries).Length;
                }

                while (streamReader.ReadLine() != null)
                {
                    height++;
                }

                var result = new Matrix(height, width);

                streamReader.BaseStream.Position = 0;

                for (int i = 0; i < height; i++)
                {
                    currentRowString = streamReader.ReadLine();

                    
                    var splittedCurrentRow = currentRowString.Split(
                        splitter, StringSplitOptions.RemoveEmptyEntries);

                    if (splittedCurrentRow.Length > width)
                    {
                        throw new ArgumentException(
                            "Invalid input: row sizes are unequal.");
                    }

                    for (int j = 0; j < width; j++)
                    {
                        if (int.TryParse(splittedCurrentRow[j], out int number))
                        {
                            result[i, j] = number;
                        }
                        else
                        {
                            throw new ArgumentException(
                                "Invalid input: row contains invalid symbols.");
                        }
                    }
                }

                return result;
            }
        }
    }
}
