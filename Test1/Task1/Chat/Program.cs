using System.Net;
using System;

namespace Chat
{
    class Program
    {
        static async void Main(string[] args)
        {
            var port = 8888;

            if (args.Length == 1)
            {
                var server = new ChatServer(IPAddress.Parse(args[0]), port);
                await server.Run();
            }
            else if (args.Length == 2)
            {
                var client = new ChatClient(args[0], Convert.ToInt32(args[1]));
                await client.Run();
            }
        }
    }
}
