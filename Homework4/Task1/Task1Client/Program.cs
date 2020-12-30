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
                port = int.Parse(args[1]);
            }
            else if (args.Length != 0)
            {
                throw new ArgumentException();
            }

            var client = new Client(ip, port);
            await client.Start();

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
                        switch (input[0])
                        {
                            case '1':
                            {
                                var res = await client.List(input);

                                foreach (var e in res.Item2)
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
                catch (Exception e) when (e is SocketException || e is IOException)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}
