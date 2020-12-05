using System.Threading.Tasks;

namespace Task1Server
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var server = new Server(9999);
            await server.Start();
        }
    }
}
