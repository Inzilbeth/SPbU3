using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Task1Server
{
    class Program
    {
        static async Task Main(string[] args)
        {
            const int port = 8888; 
            var listener = new TcpListener(IPAddress.Any, port); 
            listener.Start(); 
            Console.WriteLine($"Listening on port {port}...");
            
            while (true)
            {
                var socket = await listener.AcceptSocketAsync();

                await Task.Run(async () =>
                {
                    var stream = new NetworkStream(socket);

                    var reader = new StreamReader(stream);
                    var writer = new StreamWriter(stream);

                    var data = await reader.ReadLineAsync();

                    var split = data.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                    if (split.Length < 2)
                    {
                        await writer.WriteAsync("-1");
                    }
                    else
                    {
                        var command = split[0];
                        var path = split[1];

                        if (split[0] == "1")
                        {
                            if (!(Directory.Exists(path)))
                            {
                                await writer.WriteAsync("-1");
                                await writer.FlushAsync();
                            }
                            else
                            {
                                var dirs = Directory.GetDirectories(path);
                                var files = Directory.GetFiles(path);

                                await writer.WriteAsync((dirs.Length + files.Length).ToString() + " ");

                                foreach (var dir in dirs)
                                {
                                    await writer.WriteAsync(dir + "true ");
                                }

                                foreach (var file in files)
                                {
                                    await writer.WriteAsync(file + "false ");
                                }

                                await writer.FlushAsync();
                            }
                        }
                        else if (command == "2")
                        {
                            if (!File.Exists(path))
                            {
                                await writer.WriteAsync("-1");
                            }
                            else
                            {
                                var content = await File.ReadAllBytesAsync(path);
                                await writer.WriteAsync(content.Length + content.ToString());
                                await writer.FlushAsync();
                            }
                        }
                    }

                    await writer.WriteAsync("Hi!");
                    await writer.FlushAsync(); 
                    
                    socket.Close();
                });
            }
        }
    }
}
