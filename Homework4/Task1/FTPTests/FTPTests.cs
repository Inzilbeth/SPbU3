using Task1Client;
using Task1Server;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using NUnit.Framework;

namespace Tests
{
    public class FTPTests
    {
        private Server server;
        private Client client;

        private string address;
        private int port;

        private string rootPath = "..\\..\\..\\..";
        private string pathToSavedFolder = "\\Downloads\\";
        private string savedFilesPath;

        private List<(string, bool)> expectedRequestList;
        private List<(string, bool)> expectedRequestListFolder;

        [SetUp]
        public void Initialize()
        {
            port = 9999;
            address = "127.0.0.1";

            rootPath = Path.GetFullPath(rootPath);
            savedFilesPath = Path.Combine(rootPath, pathToSavedFolder);

            server = new Server(port);
            client = new Client(address, port);

            expectedRequestList = new List<(string, bool)>();
            expectedRequestListFolder = new List<(string, bool)>();

            expectedRequestList.Add((".\\Example\\testPng.png", false));
            expectedRequestList.Add((".\\Example\\testTxt.txt", false));
            expectedRequestList.Add((".\\Example\\testFolder", true));

            expectedRequestListFolder.Add((".\\Example\\testFolder\\someText.txt", false));
        }

        [Test]
        public void ListFunctionalityTest()
        {
            Task.Run(async () => await server.Start());

            client.Connect();

            var response = client.List("Example").Result;

            server.Stop();
            client.Stop();

            for (var i = 0; i < expectedRequestList.Count; ++i)
            {
                Assert.AreEqual(expectedRequestList[i], response.Item2[i]);
            }

            Assert.AreEqual(3, response.Item1);
        }

        [Test]
        public void ListNestedFoldersTest()
        {
            Task.Run(async () => await server.Start());

            client.Connect();

            var response = client.List("Example\\testFolder").Result;

            server.Stop();
            client.Stop();

            for (var i = 0; i < expectedRequestListFolder.Count; ++i)
            {
                Assert.AreEqual(expectedRequestListFolder[i], response.Item2[i]);
            }

            Assert.AreEqual(1, response.Item1);
        }

        [Test]
        public void ListNonexistantDirectoryThrowsTest()
        {
            Task.Run(async () => await server.Start());

            client.Connect();

            Assert.Throws<AggregateException>(() =>
            {
                var res = client.List("nonexistant").Result;

                server.Stop();
                client.Stop();
            });
        }

        [Test]
        public void GetFunctionalityTest()
        {
            var pathToSavedFile = Path.Combine(savedFilesPath, "testTxt.txt");

            if (File.Exists(pathToSavedFile))
            {
                File.Delete(pathToSavedFile);
            }

            Task.Run(async () =>
            {
                await server.Start();

                client.Connect();

                await client.Get("Example\\testTxt.txt", savedFilesPath);

                server.Stop();

                Assert.IsTrue(File.Exists(pathToSavedFile));

                File.Delete(pathToSavedFile);

                client.Stop();
            });
        }

        [Test]
        public void GetNonexistantFileTest()
        {
            Task.Run(async () => await server.Start());

            client.Connect();

            Assert.Throws<AggregateException>(() =>
            {
                client.Get("olololo", savedFilesPath).Wait();
            });

            server.Stop();
            client.Stop();
        }

        [Test]
        public void SimoltaneousRequestsTest()
        {
            var pathToFile = Path.Combine(savedFilesPath, "testPng.png");

            if (File.Exists(pathToFile))
            {
                File.Delete(pathToFile);
            }

            Task.Run(async () =>
            {
                await server.Start();

                client.Connect();

                await client.Get("Example\\testPng.png", savedFilesPath);

                Assert.IsTrue(File.Exists(pathToFile));

                var listTestFolder = client.List("Example\\testFolder").Result;

                server.Stop();
                client.Stop();

                for (var i = 0; i < expectedRequestListFolder.Count; ++i)
                {
                    Assert.AreEqual(expectedRequestListFolder[i], listTestFolder.Item2[i]);
                }
            });
        }
    }
}