using System;

namespace Task1Server
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var port = 9999;

            if (args.Length == 1)
            {
                if (int.TryParse(args[0], out var parsedPort))
                {
                    if (port <= 0 || port > 65535)
                    {
                        throw new ArgumentException("Port was out of bounds.");
                    }

                    port = parsedPort;
                }
                else
                {
                    throw new ArgumentException("Port was not a number.");
                }
            }
            else if (args.Length != 0)
            {
                throw new ArgumentException("Arguments count was neither zero nor one.");
            }

            var server = new Server(port);
            server.Start();
        }
    }
}
