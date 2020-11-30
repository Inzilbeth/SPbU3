using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Chat
{
    /// <summary>
    /// Chat client class
    /// </summary>
    public class ChatClient
    {
        private TcpClient client;
        private StreamReader reader;
        private StreamWriter writer;
        private NetworkStream stream;

        /// <summary>
        /// Initializes new ChatCient instance.
        /// </summary>
        /// <param name="address">Adress to use.</param>
        /// <param name="port">Port to use.</param>
        public ChatClient(string address, int port)
        {
            client = new TcpClient(address, port);
            stream = client.GetStream();

            writer = new StreamWriter(stream) { AutoFlush = true };

            reader = new StreamReader(stream);
        }

        /// <summary>
        /// Runs the cycle.
        /// </summary>
        /// <param name="cancellationToken">CancellationToken to use.</param>
        public async Task Run(CancellationToken cancellationToken = default)
        {
            using (cancellationToken.Register(() => Close()))
            {
                try
                {
                    var tasks = new Queue<Task>();

                    while (!cancellationToken.IsCancellationRequested)
                    {
                        tasks.Enqueue(ProcessMessage());
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
        /// Send a message to the chat.
        /// </summary>
        /// <param name="message">Message text.</param>
        /// <returns>If the operation was successful.</returns>
        public async Task<bool> SendMessage()
        {
            var message = Console.ReadLine();

            await writer.WriteLineAsync(message);

            if (message.Equals("exit"))
            {
                Close();
            }

            return true;
        }

        /// <summary>
        /// Process incoming messages and print them.
        /// </summary>
        private async Task ProcessMessage()
        {
            using var reader = new StreamReader(client.GetStream());
            using var writer = new StreamWriter(client.GetStream()) { AutoFlush = true };

            while (!reader.EndOfStream)
            {
                var message = await reader.ReadLineAsync();

                if (message.Equals("exit"))
                {
                    Close();
                }

                Console.WriteLine(message);
            }

            Close();
        }

        /// <summary>
        /// Closes the client.
        /// </summary>
        public void Close()
        {
            reader.Close();
            writer.Close();
            client.Close();
        }
    }
}
