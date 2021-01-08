using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Task1Client
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var ip = "127.0.0.1";
            var port = 9999;

            if (args.Length == 2)
            {
                ip = args[0];
                if (int.TryParse(args[1], out var parsedPort))
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
                throw new ArgumentException("Arguments count was neither zero nor two.");
            }

            var client = new Client(ip, port);
            client.Connect();

            while (true)
            {
                var input = Console.ReadLine();

                if (input == "stop")
                {
                    client.Stop();
                    Console.WriteLine("- Disconnected");
                    break;
                }

                try
                {
                    if (input != null)
                    {
                        switch (input[0])
                        {
                            case '1':
                            {
                                var (_, list) = await client.List(input);

                                foreach (var e in list)
                                {
                                    Console.WriteLine(e);
                                }

                                break;
                            }
                            case '2':
                                await client.Get(input, "");
                                break;
                        }
                    }
                }
                catch (Exception e) when (e is SocketException || e is IOException)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}
