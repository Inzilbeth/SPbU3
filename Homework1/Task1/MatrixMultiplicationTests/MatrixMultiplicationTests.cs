using NUnit.Framework;
using System;
using System.IO;
using Task1;

namespace Task1Tests
{
    class MatrixMultiplicationTests
    {
        private Matrix Left;
        private Matrix Right;
        private Matrix ExpectedResult;

        private string leftPath = "BasicTestA.txt";
        private string rightPath = "BasicTestB.txt";
        private string expectedResultPath = "BasicTestC.txt";

        private string expectedResultString = "48 62 \n86 114 \n157 207 \n";
        private string outputFilePath = "output.txt";

        [SetUp]
        public void Setup()
        {
            var leftString = "4 3 7 1 \n2 4 8 10 \n15 4 6 9 \n";
            var rightString = "6 8 \n4 3 \n1 2 \n5 7 \n";

            Tools.Write(leftPath, leftString);
            Tools.Write(rightPath, rightString);
            Tools.Write(expectedResultPath, expectedResultString);

            Left = new Matrix(leftPath);
            Right = new Matrix(rightPath);
            ExpectedResult = new Matrix(expectedResultPath);
        }

        [Test]
        public void SimpleMultiplicationTest()
        {
            Assert.AreEqual(
                ExpectedResult, Matrix.SingleThreadedMultiply(Left, Right));
        }

        [Test]
        public void SimpleMultiplicationShouldThrowIfDimensionsDontMatch()
        {
            Assert.That(() => Matrix.SingleThreadedMultiply(Right, Left),
                Throws.TypeOf<ArgumentException>()
                    .With.Message.EqualTo(
                    "Invalid input: matrices have invalid dimensions."));
        }

        [Test]
        public void SimpleMultiplicationWithWritingTest()
        {
            Matrix.MultiplyAndWrite(Left, Right, false, outputFilePath);
            var actualString = Tools.Read(outputFilePath);
            
            Assert.AreEqual(expectedResultString, actualString);
        }

        [Test]
        public void ThreadedMultiplicationTest()
        {
            Assert.AreEqual(ExpectedResult, Matrix.MultiThreadedMultiply(Left, Right));
        }

        [Test]
        public void ThreadedMultiplicationShouldThrowIfDimensionsDontMatch()
        {
            Assert.That(() => Matrix.MultiThreadedMultiply(Right, Left),
                Throws.TypeOf<ArgumentException>()
                    .With.Message.EqualTo(
                    "Invalid input: matrices have invalid dimensions."));
        }

        [Test]
        public void ThreadedMultiplicationWithWritingTest()
        {
            Matrix.MultiplyAndWrite(Left, Right, true, outputFilePath);
            var actualString = Tools.Read(outputFilePath);

            Assert.AreEqual(expectedResultString, actualString);
        }

        [OneTimeTearDown]
        public void DeleteTemporaryFiles()
        {
            File.Delete(leftPath);
            File.Delete(rightPath);
            File.Delete(expectedResultPath);
            File.Delete(outputFilePath);
        }
    }
}
