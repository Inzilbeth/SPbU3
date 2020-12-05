using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Task1Server
{
    /// <summary>
    /// Handles requests from client.
    /// </summary>
    public static class RequestHandler
    {
        /// <summary>
        /// Gets the string and writes it to the stream.
        /// </summary>
        public async static Task HandleRequest(string request, StreamWriter writer)
        {
            if (int.TryParse(request[0].ToString(), out var id))
            {
                var rootPath = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName, "storage\\");
                var path = Path.Combine(rootPath, request.Remove(0, 1));

                if (id == 1)
                {
                    var offset = rootPath.Length;
                    await HandleListRequest(path, offset, writer);
                }

                if (id == 2)
                {
                    await HandleGetRequest(path, writer);
                }

                return;
            }

            var errorText = "Wrong format error";
            await writer.WriteLineAsync(errorText);
        }

        /// <summary>
        /// List request handling.
        /// </summary>
        private static async Task HandleListRequest(string path, int offset, StreamWriter writer)
        {
            if (!Directory.Exists(path))
            {
                var errorResponse = "-1";
                await writer.WriteLineAsync(errorResponse);
                return;
            }

            var response = new StringBuilder();

            var files = Directory.GetFiles(path);
            var folders = Directory.GetDirectories(path);

            var responseSize = files.Length + folders.Length;

            response.Append($"{responseSize} ");

            foreach (var file in files)
            {
                var formattedName = file.Remove(0, offset);
                response.Append($".\\{formattedName} false ");
            }

            foreach (var folder in folders)
            {
                var formattedName = folder.Remove(0, offset);
                response.Append($".\\{formattedName} true ");
            }

            await writer.WriteLineAsync(response.ToString());
        }

        /// <summary>
        /// Get request handling.
        /// </summary>
        private static async Task HandleGetRequest(string path, StreamWriter writer)
        {
            if (!File.Exists(path))
            {
                await writer.WriteLineAsync("-1");
            }

            var size = new FileInfo(path).Length;
            await writer.WriteLineAsync($"{size}");

            using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite))
            {
                await fileStream.CopyToAsync(writer.BaseStream);
            }
        }
    }
}
