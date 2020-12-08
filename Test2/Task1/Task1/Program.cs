using System;
using System.Diagnostics;
using System.IO;

namespace Task1
{
    class Program
    {
        static void Main(string[] args)
        {
            var path = Console.ReadLine();
            var stopWatch = new Stopwatch();

            stopWatch.Start();
            var hash1 = CheckSumCalculator.ComputeHash(path);
            stopWatch.Stop();
            var time1 = stopWatch.Elapsed;

            stopWatch.Reset();

            stopWatch.Start();
            var hash2 = CheckSumCalculator.ComputeHashParallel(path);
            stopWatch.Stop();
            var time2 = stopWatch.Elapsed;

            Console.WriteLine($"Hash & elapsed time in ms for simple algo: {BitConverter.ToString(hash1)}, {time1}");
            Console.WriteLine($"Hash & elapsed time in ms for Parallel algo: {BitConverter.ToString(hash2)}, {time2}");
        }
    }
}
