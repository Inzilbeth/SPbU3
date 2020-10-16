using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;

namespace Task1Client
{
    class Program
    {
        static void Main(string[] args)
        {
            const int port = 8888;

            using (var client = new TcpClient("localhost", port))
            {
                Console.WriteLine($"Sending message to port {port}...");
                var stream = client.GetStream();
                var writer = new StreamWriter(stream);
                var reader = new StreamReader(stream);

                var path = Console.ReadLine();

                var data = List(path, writer, reader);
                Console.WriteLine($"Received: {data}");

                data = Get(path, writer, reader);
                Console.WriteLine($"Received: {data}");
            }
        }

        private static string List(string path, StreamWriter writer, StreamReader reader)
        {
            writer.Write($"1 {path}");
            writer.Flush();

            return reader.ReadToEnd();
        }

        private static string Get(string path, StreamWriter writer, StreamReader reader)
        {
            writer.Write($"2 {path}");
            writer.Flush();

            return reader.ReadToEnd();
        }
    }
}
