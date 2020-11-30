using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Chat
{
    public class ChatServer
    {
        private TcpListener listener;

        /// <summary>
        /// Initializes a ChatServer.
        /// </summary>
        /// <param name="address">Adress to use.</param>
        /// <param name="port">Port to use.</param>
        public ChatServer(IPAddress address, int port)
        {
            listener = new TcpListener(address, port);
            listener.Start();
        }

        /// <summary>
        /// Runs the cycle.
        /// </summary>
        /// <param name="cancellationToken">CancellationToken to use.</param>
        public async Task Run(CancellationToken cancellationToken = default)
        {
            using (cancellationToken.Register(() => listener.Stop()))
            {
                try
                {
                    var tasks = new Queue<Task>();

                    while (!cancellationToken.IsCancellationRequested)
                    {
                        var client = await listener.AcceptTcpClientAsync();
                        tasks.Enqueue(ProcessMessage(client));
                    }

                    while (tasks.Count > 0)
                    {
                        var task = tasks.Dequeue();

                        await task;
                    }
                }
                catch (InvalidOperationException)
                {
                }
            }
        }

        /// <summary>
        /// Process incoming messages and print them.
        /// </summary>
        private async Task ProcessMessage(TcpClient client)
        {
            using var reader = new StreamReader(client.GetStream());
            using var writer = new StreamWriter(client.GetStream()) { AutoFlush = true };

            while (!reader.EndOfStream)
            {
                var message = await reader.ReadLineAsync();

                if (message.Equals("exit"))
                {
                    client.Close();
                }

                Console.WriteLine(message);
            }

            client.Close();
        }

        /// <summary>
        /// Send a message to the chat.
        /// </summary>
        /// <param name="message">Message text.</param>
        /// <returns>If the operation was successful.</returns>
        public async Task<bool> SendMessage(TcpClient client)
        {
            var message = Console.ReadLine();
            using var writer = new StreamWriter(client.GetStream()) { AutoFlush = true };

            await writer.WriteLineAsync(message);

            if (message.Equals("exit"))
            {
                client.Close();
            }

            return true;
        }
    }
}
