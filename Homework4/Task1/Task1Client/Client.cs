using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Task1Client
{
    /// <summary>
    /// FTP client.
    /// </summary>
    public class Client : IDisposable
    {
        private readonly string server;
        private readonly int port;
        private TcpClient client;

        /// <summary>
        /// Default constructor, connects considering server & port.
        /// </summary>
        public Client(string server, int port)
        {
            this.server = server;
            this.port = port;
        }

        /// <summary>
        /// Connection to the server.
        /// </summary>
        public void Connect()
        {
            client = new TcpClient(server, port);
        }

        /// <summary>
        /// Simple demonstration of functionality.
        /// </summary>
        public async Task Start() => Connect();

        /// <summary>
        /// Request for a files and folders list by directory.
        /// </summary>
        /// <returns>List of (name, folder - true / file - false) </returns>
        public async Task<(int, List<(string, bool)>)> List(string path)
        {
            var writer = new StreamWriter(client.GetStream()) { AutoFlush = true };
            var reader = new StreamReader(client.GetStream());

            await writer.WriteLineAsync("1" + path);

            var response = await reader.ReadLineAsync();

            return ResponseHandler.HandleListResponse(response);
        }

        /// <summary>
        /// File download.
        /// </summary>
        /// <param name="pathFrom">Path to file on server.</param>
        /// <param name="pathTo">Where to save the loaded file.</param>
        public async Task Get(string pathFrom, string pathTo)
        {
            var temp = pathFrom.Split('\\');
            var fileName = temp[^1];

            using var writer = new StreamWriter(client.GetStream()) { AutoFlush = true };
            using var reader = new StreamReader(client.GetStream());

            await writer.WriteLineAsync("2" + pathFrom);

            var response = await reader.ReadLineAsync();

            if (!long.TryParse(response, out long fileSize))
            {
                throw new Exception(response);
            }

            if (fileSize == -1)
            {
                throw new FileNotFoundException();
            }

            using var fileStream = new FileStream(pathTo + fileName, FileMode.CreateNew);
            await reader.BaseStream.CopyToAsync(fileStream);
        }

        /// <summary>
        /// Stops client.
        /// </summary>
        public void Stop() => client.Close();

        /// <summary>
        /// Frees resources.
        /// </summary>
        public void Dispose() => client.Dispose();
    }
}
