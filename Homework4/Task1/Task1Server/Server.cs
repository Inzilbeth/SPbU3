using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Task1Server
{
    /// <summary>
    /// FTP server.
    /// </summary>
    public class Server
    {
        private readonly IPAddress localAddress = IPAddress.Parse("127.0.0.1");
        private readonly TcpListener listener;
        private readonly CancellationTokenSource cts;

        /// <summary>
        /// Sets up a server using the selected port.
        /// </summary>
        public Server(int port)
        {
            cts = new CancellationTokenSource();
            listener = new TcpListener(localAddress, port);
        }

        /// <summary>
        /// Launch the server.
        /// </summary>
        public void Start()
        {
            try
            {
                listener.Start();

                while (!cts.IsCancellationRequested)
                {
                    HandleClientCommunication();
                }
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message);
                Stop();
            }
        }

        /// <summary>
        /// Checks if specified client is connected to the server.
        /// </summary>
        private bool IsConnected(TcpClient client)
        {
            try
            {
                if (client?.Client != null && client.Client.Connected)
                {
                    if (client.Client.Poll(0, SelectMode.SelectRead))
                    {
                        var buffer = new byte[1];
                        if (client.Client.Receive(buffer, SocketFlags.Peek) == 0)
                        {
                            return false;
                        }
                    }

                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Handles requests incoming from client.
        /// </summary>
        private void HandleClientCommunication()
        {
            Task.Run(async () =>
            {
                using var client = await listener.AcceptTcpClientAsync();

                while (!cts.IsCancellationRequested)
                {
                    if (!IsConnected(client))
                    {
                        break;
                    }

                    using var reader = new StreamReader(client.GetStream());
                    using var writer = new StreamWriter(client.GetStream()) {AutoFlush = true};

                    var received = await reader.ReadLineAsync();

                    await RequestHandler.HandleRequest(received, writer);
                }
            });
        }

        /// <summary>
        /// Stops the server.
        /// </summary>
        public void Stop()
        {
            cts.Cancel();

            listener?.Stop();
        }
    }
}
