using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Task1
{
    public static class CheckSumCalculator
    {
        public static byte[] ComputeHash(string path)
        {
            if (File.Exists(path))
            {
                return CalculateFileHash(path);
            }
            else if (Directory.Exists(path))
            {
                byte[] temp = new byte[0];

                using (var md = MD5.Create())
                {
                    if (Path.GetDirectoryName(path) != null)
                    {
                        temp.Concat(Encoding.ASCII.GetBytes(Path.GetDirectoryName(path)));
                    }
                }

                foreach (var entry in Directory.EnumerateFileSystemEntries(path))
                {
                    temp.Concat(ComputeHash(entry));
                }

                using (var md = MD5.Create())
                {
                    return md.ComputeHash(temp);
                }
            }
            else
            {
                throw new ArgumentException("Invalid path");
            }

        }

        public static byte[] ComputeHashParallel(string path)
        {
            if (File.Exists(path))
            {
                return CalculateFileHash(path);
            }
            else if (Directory.Exists(path))
            {
                byte[] temp = new byte[0];

                using (var md = MD5.Create())
                {
                    temp.Concat(Encoding.ASCII.GetBytes(Path.GetDirectoryName(path)));
                }

                Parallel.ForEach(Directory.EnumerateFileSystemEntries(path), entry =>
                {
                    temp.Concat(ComputeHash(entry));
                });

                using (var md = MD5.Create())
                {
                    return md.ComputeHash(temp);
                }
            }
            else
            {
                throw new ArgumentException("Invalid path");
            }
        }

        private static byte[] CalculateFileHash(string path)
        {
            using (Stream source = File.OpenRead(path))
            {
                using (var md = MD5.Create())
                {
                    var buffer = new byte[10240];
                    int bytesRead = 0;
                    int bytesRecieved = 0;

                    while ((bytesRead = source.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        bytesRecieved += bytesRead;
                        md.TransformBlock(buffer, 0, bytesRead, null, 0);
                    }

                    md.TransformFinalBlock(new byte[0], 0, 0);
                    return md.Hash;
                }
            }
        }
    }
}
