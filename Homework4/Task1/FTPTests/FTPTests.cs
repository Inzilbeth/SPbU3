﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using Task1Client;
using Task1Server;

namespace FTPTests
{
    public class FtpTests
    {
        private Server server;
        private Task start;
        private Client client;

        private string address;
        private int port;

        private string rootPath = "..\\..\\..\\..";
        private const string PathToSavedFolder = "\\Downloads\\";
        private string savedFilesPath;

        private List<(string, bool)> expectedRequestList;
        private List<(string, bool)> expectedRequestListFolder;

        [SetUp]
        public void Initialize()
        {
            port = 9999;
            address = "127.0.0.1";

            rootPath = Path.GetFullPath(rootPath);
            savedFilesPath = Path.Combine(rootPath, PathToSavedFolder);

            server = new Server(port);
            start = Task.Run(() => server.Start());

            client = new Client(address, port);

            expectedRequestList = new List<(string, bool)>();
            expectedRequestListFolder = new List<(string, bool)>();

            expectedRequestList.Add((".\\Example\\testPng.png", false));
            expectedRequestList.Add((".\\Example\\testTxt.txt", false));
            expectedRequestList.Add((".\\Example\\testFolder", true));

            expectedRequestListFolder.Add((".\\Example\\testFolder\\someText.txt", false));
        }

        [Test]
        public async Task ListFunctionalityTest()
        {
            int amount;
            List<(string, bool)> list;

            try
            {
                await client.Connect();
                (amount, list) = await client.List("Example");
            }
            finally
            {
                server.Stop();
                client.Stop();
            }

            for (var i = 0; i < expectedRequestList.Count; ++i)
            {
                Assert.AreEqual(expectedRequestList[i], list[i]);
            }

            Assert.AreEqual(3, amount);
        }

        [Test]
        public async Task ListNestedFoldersTest()
        {
            int amount;
            List<(string, bool)> list;

            try
            {
                await client.Connect();

                (amount, list) = await client.List("Example\\testFolder");
            }
            finally
            {
                server.Stop();
                client.Stop();
            }

            for (var i = 0; i < expectedRequestListFolder.Count; ++i)
            {
                Assert.AreEqual(expectedRequestListFolder[i], list[i]);
            }

            Assert.AreEqual(1, amount);
        }

        [Test]
        public async Task ListNonexistantDirectoryThrowsTest()
        {
            try
            {
                await client.Connect();

                Assert.ThrowsAsync<DirectoryNotFoundException>(async () =>
                    await client.List("nonexistant"));
            }
            finally
            {
                server.Stop();
                client.Stop();
            }
        }

        [Test]
        public async Task GetFunctionalityTest()
        {
            var pathToSavedFile = Path.Combine(savedFilesPath, "testTxt.txt");
            Directory.CreateDirectory(savedFilesPath);

            if (File.Exists(pathToSavedFile))
            {
                File.Delete(pathToSavedFile);
            }

            try
            {
                await client.Connect();
                await client.Get("Example\\testTxt.txt", savedFilesPath);
            }
            finally
            {
                server.Stop();

                Assert.IsTrue(File.Exists(pathToSavedFile));

                File.Delete(pathToSavedFile);
                Directory.Delete(savedFilesPath);

                client.Stop();
            }
        }

        [Test]
        public async Task GetNonexistantFileTest()
        {
            try
            {
                await client.Connect();
                Assert.Throws<AggregateException>(() => { client.Get("olololo", savedFilesPath).Wait(); });
            }
            finally
            {
                server.Stop();
                client.Stop();
            }
        }

        [Test]
        public async Task SimultaneousRequestsTest()
        {
            var pathToSavedFile = Path.Combine(savedFilesPath, "testPng.png");

            if (File.Exists(pathToSavedFile))
            {
                File.Delete(pathToSavedFile);
            }
            Directory.CreateDirectory(savedFilesPath);

            (int amount, List<(string, bool)> list) result = (0, new List<(string, bool)>());

            try
            {
                await client.Connect();
                result = await client.List("Example\\testFolder");

                await client.Connect();
                await client.Get("Example\\testPng.png", savedFilesPath);
            }
            finally
            {
                server.Stop();
                client.Stop();

                Assert.IsTrue(File.Exists(pathToSavedFile));

                File.Delete(pathToSavedFile);
                Directory.Delete(savedFilesPath);

                for (var i = 0; i < expectedRequestListFolder.Count; ++i)
                {
                    Assert.AreEqual(expectedRequestListFolder[i], result.list[i]);
                }
            }
        }

        [TearDown]
        public async Task AwaitTask()
        {
            await start;
        }
    }
}