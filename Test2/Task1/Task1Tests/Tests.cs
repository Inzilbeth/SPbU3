using NUnit.Framework;
using System;
using System.IO;
using Task1;

namespace Task1Tests
{
    public class Tests
    {
        string path1;
        string path2;

        [SetUp]
        public void Setup()
        {
            var root = Directory.GetCurrentDirectory();
            path1 = root + "\\Example\\Folder";
            path2 = root + "\\Example\\utilityDirectory\\Folder";
        }

        [Test]
        public void SameDirectoriesGiveSameHashTest()
        {
            Assert.AreEqual(CheckSumCalculator.ComputeHash(path1),
                            CheckSumCalculator.ComputeHash(path2));
        }

        [Test]
        public void SameDirectoriesGiveSameHashTestParallel()
        {
            Assert.AreEqual(CheckSumCalculator.ComputeHashParallel(path1),
                            CheckSumCalculator.ComputeHashParallel(path2));
        }

        [Test]
        public void ThrowsOnWrongPathTest()
        {
            Assert.Throws<ArgumentException>(() => CheckSumCalculator.ComputeHash("nonexistantPath"));
        }
    }
}