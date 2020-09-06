using System;
using System.Threading;
using System.IO;

namespace Task1
{
    /// <summary>
    /// Matrix class implementation including basic matrices' fields manipulation
    /// and matrice multiplication.
    /// </summary>
    public class Matrix
    {
        private long[,] matrixArray;

        /// <summary>
        /// Instantinates a <see cref="Matrix"/> class loading it's values from a file.
        /// </summary>
        /// <param name="path">Path to the input file.</param>
        public Matrix(string path)
        { 
            var parsedMatrix = Parser.Parse(path);

            matrixArray = parsedMatrix.matrixArray;
        }

        /// <summary>
        /// Instantinates a null <see cref="Matrix"/> with the input amount of rows & columns.
        /// </summary>
        /// <param name="rows">Rows amount.</param>
        /// <param name="columns">Columns amount.</param>
        public Matrix(int rows, int columns)
        {
            matrixArray = new long[rows, columns];
        }

        /// <summary>
        /// Fills the matrix with random integers.
        /// </summary>
        /// <returns>The matrix itself now filled with random values.</returns>
        public Matrix RandomizeValues(int maxValue)
        {
            var random = new Random();

            for (int i = 0; i < matrixArray.GetLength(0); i++)
            {
                for (int j = 0; j < matrixArray.GetLength(1); j++)
                {
                    matrixArray[i, j] = random.Next(maxValue);
                }
            }

            return this;
        }

        /// <summary>
        /// Transposes the matrix using cache-unfriendly algorithm and returns it.
        /// </summary>
        /// <returns>Transposed matrix.</returns>
        public Matrix SillyTranspose()
        {
            var temporaryArray = matrixArray;

            matrixArray = new long[temporaryArray.GetLength(1), temporaryArray.GetLength(0)];

            for (int i = 0; i < matrixArray.GetLength(0); i++)
            {
                for (int j = 0; j < matrixArray.GetLength(1); j++)
                {
                    matrixArray[i, j] = temporaryArray[j, i];
                }
            }

            return this;
        }

        /// <summary>
        /// Gets/sets the value with the input coordinates.
        /// </summary>
        /// <param name="i">First coordinate.</param>
        /// <param name="j">Second coordinate.</param>
        /// <returns>The value with the input coordinates.</returns>
        public long this[int i, int j]
        {
            get { return matrixArray[i, j]; }
            set { matrixArray[i, j] = value; }
        }

        /// <summary>
        /// Prints the matrix to the console.
        /// </summary>
        public void WriteToConsole()
        {
            for (int i = 0; i < matrixArray.GetLength(0); i++)
            {
                for (int j = 0; j < matrixArray.GetLength(1); j++)
                {
                    Console.Write(string.Format("{0,5}", matrixArray[i, j]));
                }

                Console.WriteLine();
            }
        }

        /// <summary>
        /// Multiplies two matrices with selected threading and writes result matrix to a file.
        /// </summary>
        /// <param name="a">Left matrix.</param>
        /// <param name="b">Right matrix.</param>
        /// <param name="isThreaded">Whether to use threading or not.</param>
        /// <param name="path">Path to the output file.</param>
        public static void MultiplyAndWrite(
            Matrix a, Matrix b, bool isThreaded, string path)
        {
            Matrix result;

            if (isThreaded)
            {
                result = MultiThreadedMultiply(a, b);
            }
            else
            {
                result = SingleThreadedMultiply(a, b);
            }

            result.WriteToFile(path);
        }

        /// <summary>
        /// Writes matrix to the file.
        /// </summary>
        /// <param name="path">Path of the output file.</param>
        public void WriteToFile(string path)
        {
            using (var streamWriter = new StreamWriter(path))
            {
                for (int i = 0; i < matrixArray.GetLength(0); i++)
                {
                    for (int j = 0; j < matrixArray.GetLength(1); j++)
                    {
                        streamWriter.Write($"{matrixArray[i, j]} ");
                    }

                    streamWriter.Write("\n");
                }
            }
        }

        /// <summary>
        /// Multiplies two matrices using multithreading.
        /// </summary>
        /// <param name="a">Left matrix.</param>
        /// <param name="b">Right matrix.</param>
        /// <returns>The result of multiplication.</returns>
        public static Matrix MultiThreadedMultiply(Matrix a, Matrix b)
        {
            if (!AreMultipliable(a, b))
            {
                throw new ArgumentException(
                    "Invalid input: matrices have invalid dimensions.");
            }

            b.SillyTranspose();

            var result = new Matrix(a.matrixArray.GetLength(0), b.matrixArray.GetLength(0));

            var threadsAmount = 10;
            var threads = new Thread[threadsAmount];
            int chunkSize;

            if (a.matrixArray.GetLength(0) > (threads.Length + 1))
            {
                chunkSize = a.matrixArray.GetLength(0) / (threads.Length + 1);
            }
            else
            {
                chunkSize = a.matrixArray.GetLength(0);
            }

            for (int i = 0; i < threadsAmount; i++)
            {
                var localI = i;
                threads[i] = new Thread(() =>
                {
                    for (var j = localI * chunkSize; j < (localI + 1) * chunkSize
                        && j < a.matrixArray.GetLength(0); j++)
                    {
                        for (int f = 0; f < b.matrixArray.GetLength(0); f++)
                        {
                            result[j, f] = 0;

                            for (var k = 0; k < a.matrixArray.GetLength(1); k++)
                            {
                                result[j, f] += a[j, k] * b[f, k];
                            }
                        }
                    }
                });
            }

            foreach (var thread in threads)
            {
                thread.Start();
            }

            foreach (var thread in threads)
            {
                thread.Join();
            }

            return result;
        }

        /// <summary>
        /// Multiplies two matrices using a simple single-threaded algorithm.
        /// </summary>
        /// <param name="a">Left matrix.</param>
        /// <param name="b">Right matrix.</param>
        /// <returns>The result of multiplication.</returns>
        public static Matrix SingleThreadedMultiply(Matrix a, Matrix b)
        {
            if (!AreMultipliable(a, b))
            {
                throw new ArgumentException(
                    "Invalid input: matrices have invalid dimensions.");
            }

            var result = new Matrix(a.matrixArray.GetLength(0), b.matrixArray.GetLength(1));

            for (var i = 0; i < a.matrixArray.GetLength(0); i++)
            {
                for (var j = 0; j < b.matrixArray.GetLength(1); j++)
                {
                    result[i, j] = 0;

                    for (var k = 0; k < a.matrixArray.GetLength(1); k++)
                    {
                        result[i, j] += a[i, k] * b[k, j];
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Checks if two matrices are multipliable.
        /// </summary>
        /// <param name="a">First matrix.</param>
        /// <param name="b">Second matrix.</param>
        /// <returns>Whether two matrices are multipliable or not.</returns>
        private static bool AreMultipliable(Matrix a, Matrix b)
            => a.matrixArray.GetLength(1) == b.matrixArray.GetLength(0);

        public override bool Equals(object obj)
        {
            if (obj is Matrix && obj != null)
            {
                var that = obj as Matrix;

                if (matrixArray.GetLength(0) != that.matrixArray.GetLength(0) 
                    || matrixArray.GetLength(1) != that.matrixArray.GetLength(1))
                {
                    return false;
                }

                for (int i = 0; i < matrixArray.GetLength(0); i++)
                {
                    for (int j = 0; j < matrixArray.GetLength(1); j++)
                    {
                        if (matrixArray[i, j] != that.matrixArray[i, j])
                        {
                            return false;
                        }
                    }
                }

                return true;
            }

            return false;
        }

        public override int GetHashCode()
        {
            var hashCode = matrixArray.GetLength(0).GetHashCode();
            hashCode ^= matrixArray.GetLength(1).GetHashCode();
            hashCode ^= matrixArray.GetHashCode();
            return hashCode;
        }
    }
}