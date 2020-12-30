using System;
using System.Threading.Tasks;

namespace Task1Server
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var port = 9999;

            if (args.Length == 1)
            {
                port = int.Parse(args[1]);
            }
            else if (args.Length != 0)
            {
                throw new ArgumentException();
            }

            var server = new Server(port);
            await server.Start();
        }
    }
}
