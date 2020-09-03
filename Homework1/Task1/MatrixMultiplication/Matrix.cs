using System;
using System.Threading;
using System.Collections.Generic;
using System.IO;

namespace Task1
{
    /// <summary>
    /// Matrix class implementation including basic matrices' fields manipulation
    /// and matrice multiplication.
    /// </summary>
    public class Matrix
    {
        public int Rows { get; set; }
        public int Columns { get; set; }

        private long[,] matrixArray;

        /// <summary>
        /// Instantinates a <see cref="Matrix"/> class loading it's values from a file.
        /// </summary>
        /// <param name="path">Path to the input file.</param>
        public Matrix(string path)
        { 
            var parsedMatrix = Parser.Parse(path);

            Rows = parsedMatrix.Rows;
            Columns = parsedMatrix.Columns;
            matrixArray = parsedMatrix.matrixArray;
        }

        /// <summary>
        /// Instantinates a null <see cref="Matrix"/> with the input amount of rows & columns.
        /// </summary>
        /// <param name="rows">Rows amount.</param>
        /// <param name="columns">Columns amount.</param>
        public Matrix(int rows, int columns)
        {
            Rows = rows;
            Columns = columns;

            matrixArray = new long[rows, columns];
        }

        /// <summary>
        /// Fills the matrix with random integers.
        /// </summary>
        /// <returns>The matrix itself now filled with random values.</returns>
        public Matrix RandomizeValues(int maxValue)
        {
            var random = new Random((int)DateTime.Now.Ticks);

            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    matrixArray[i, j] = random.Next(maxValue);
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
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    Console.Write($"{matrixArray[i, j]} ");
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
                for (int i = 0; i < Rows; i++)
                {
                    for (int j = 0; j < Columns; j++)
                    {
                        streamWriter.Write($"{matrixArray[i, j]} ");
                    }

                    streamWriter.Write("\n");
                }
            }
        }

        /// <summary>
        /// Multiplies two matrices using threading.
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

            var result = new Matrix(a.Rows, b.Columns);
            var threads = new List<Thread>();

            for (int i = 0; i < a.Rows; i++)
            {
                var localI = i;

                var thread = new Thread(() =>
                {
                    for (int j = 0; j < b.Columns; j++)
                    {
                        result[localI, j] = 0;

                        for (var k = 0; k < a.Columns; k++)
                        {
                            result[localI, j] += a[localI, k] * b[k, j];
                        }
                    }
                });

                thread.Start();
                threads.Add(thread);
            }

            foreach (Thread thread in threads)
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

            var result = new Matrix(a.Rows, b.Columns);

            for (var i = 0; i < a.Rows; i++)
            {
                for (var j = 0; j < b.Columns; j++)
                {
                    result[i, j] = 0;

                    for (var k = 0; k < a.Columns; k++)
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
            => a.Columns == b.Rows;

        public override bool Equals(object obj)
        {
            if (obj is Matrix)
            {
                var that = obj as Matrix;

                if (Rows != that.Rows || Columns != that.Columns)
                {
                    return false;
                }

                for (int i = 0; i < Rows; i++)
                {
                    for (int j = 0; j < Columns; j++)
                    {
                        if (matrixArray[i, j] != that.matrixArray[i, j])
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = Rows.GetHashCode();
            hashCode ^= Columns.GetHashCode();
            hashCode ^= matrixArray.GetHashCode();
            return hashCode;
        }
    }
}