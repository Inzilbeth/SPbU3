using System.Threading.Tasks;

namespace Task1Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var client = new Client("127.0.0.1", 9999);
            await client.Start();
        }
    }
}
