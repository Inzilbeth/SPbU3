using System;
using System.Threading.Tasks;
using NUnit.Framework;
using Task1Server;
using Task1;

namespace Task1Tests
{
    public class ClientViewModelTests
    {
        private Server server;
        private Task start;
        private string address;
        private int port;
        private ClientViewModel model;
        private string downloadPath = "..\\..\\..\\..\\Task1\\";
        private bool wasErrorThrown;

        private void ErrorThrown(object sender, string errorMessage)
        {
            wasErrorThrown = true;
        }

        [SetUp]
        public async Task Setup()
        {
            address = "127.0.0.1";
            port = 9999;
            server = new Server(port);
            start = Task.Run(() => server.Start());

            model = new ClientViewModel(downloadPath) {Port = "9999", Server = address};
            model.ThrowError += ErrorThrown;
            wasErrorThrown = false;
            model.isConnected = false;
            await model.Connect();
        }

        [Test]
        public void ConnectsTest()
        {
            server.Stop();
            Assert.IsTrue(model.isConnected);
        }

        [Test]
        public void CorrectlySetsClientFolders()
        {
            server.Stop();
            Assert.AreEqual(2, model.DisplayedClientList.Count);
        }

        [Test]
        public void CorrectlySetsServerFolders()
        {
            server.Stop();
            Assert.AreEqual(1, model.DisplayedServerList.Count);
        }

        [Test]
        public void GoBackInsideClientRootThrows()
        {
            model.GoBackClient();

            server.Stop();
            Assert.IsTrue(wasErrorThrown);
        }

        [Test]
        public async Task GoBackInsideServerRootThrows()
        {
            await model.GoBackServer();

            server.Stop();
            Assert.IsTrue(wasErrorThrown);
        }

        [TearDown]
        public async Task AwaitTask()
        {
            await start;
        }
    }
}