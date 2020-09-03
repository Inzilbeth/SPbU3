using System;
using System.Diagnostics;

namespace Task1
{
    public class Program
    {
        /// <summary>
        /// Launches comparison of multi- and single-threaded matrices multiplication 
        /// and logs the data into the console.
        /// </summary>
        /// <param name="size">Size of the square matrices sides to multiply.</param>
        /// <param name="maxValue">Max values of matrices.</param>
        private static void Profile(int size, int maxValue)
        {
            Console.WriteLine(
                $"Comparing on {size}x{size} matrices filled with max values of {maxValue}");

            var timeSpanSingleThreaded = GetTimeSpan(size, maxValue, false);
            Console.WriteLine(
                $"Single-thread computation took {FormatTimeSpan(timeSpanSingleThreaded)}");

            var timeSpanMultiThreaded = GetTimeSpan(size, maxValue, true);
            Console.WriteLine(
                $"Multi-thread computation took  {FormatTimeSpan(timeSpanMultiThreaded)}");

            Console.WriteLine();

            if (timeSpanMultiThreaded < timeSpanSingleThreaded)
            {
                Console.WriteLine(
                    $"Multi-threaded was faster by  " +
                    $"{FormatTimeSpan(timeSpanSingleThreaded - timeSpanMultiThreaded)}");
            }
            else
            {
                Console.WriteLine(
                    $"Single-threaded was faster by  " +
                    $"{FormatTimeSpan(timeSpanMultiThreaded - timeSpanSingleThreaded)}");
            }
            Console.WriteLine("------------------------------------------------------------");
        }

        /// <summary>
        /// Formats <see cref="TimeSpan"/> into a nice <see cref="string"/>.
        /// </summary>
        /// <param name="timeSpan">Input timespan.</param>
        /// <returns>Formatted string.</returns>
        private static string FormatTimeSpan(TimeSpan timeSpan)
            => string.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds,
                timeSpan.Milliseconds / 10);

        /// <summary>
        /// Gets the time spent on computation of two square matrices 
        /// with input parameters using specified threading.
        /// </summary>
        /// <param name="size">Size of the square matrices sides.</param>
        /// <param name="maxValue">Max values of the matrices</param>
        /// <param name="isThreaded">Whether to use multi-threading or not.</param>
        /// <returns>Time spent.</returns>
        private static TimeSpan GetTimeSpan(int size, int maxValue, bool isThreaded)
        {
            var stopWatch = new Stopwatch();

            var a = new Matrix(size, size).RandomizeValues(maxValue);
            var b = new Matrix(size, size).RandomizeValues(maxValue);

            stopWatch.Start();
            if (isThreaded)
            {
                var res = Matrix.MultiThreadedMultiply(a, b);
            }
            else
            {
                var res = Matrix.SingleThreadedMultiply(a, b);
            }
            stopWatch.Stop();

            return stopWatch.Elapsed;
        }


        public static void Main()
        {
            Console.WriteLine("Do you want to see perfomance tests? (y/n)");
            var answer = Console.ReadLine();

            if (answer.Equals("y"))
            {
                Profile(50, 10);
                Profile(100, 10);
                Profile(600, 10);
                Profile(800, 10);
                Profile(1000, 10);
            }
            else if (answer.Equals("n"))
            {
                Console.WriteLine("Enter path to the left matrix file:");
                var pathLeft = Console.ReadLine();
                Console.WriteLine("Enter path to the right matrix file:");
                var pathRight = Console.ReadLine();
                Console.WriteLine("Enter path to the output matrix file:");
                var pathResult = Console.ReadLine();

                Matrix.MultiplyAndWrite(
                    new Matrix(pathLeft), new Matrix(pathRight), true, pathResult);
            }
        }
    }
}
