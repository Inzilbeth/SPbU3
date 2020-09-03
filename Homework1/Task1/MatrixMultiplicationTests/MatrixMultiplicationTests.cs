using NUnit.Framework;
using System;
using System.IO;
using Task1;

namespace Task1Tests
{
    class MatrixMultiplicationTests
    {
        private Matrix left;
        private Matrix right;
        private Matrix expectedResult;

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

            File.WriteAllText(leftPath, leftString);
            File.WriteAllText(rightPath, rightString);
            File.WriteAllText(expectedResultPath, expectedResultString);

            left = new Matrix(leftPath);
            right = new Matrix(rightPath);
            expectedResult = new Matrix(expectedResultPath);
        }

        [Test]
        public void SimpleMultiplicationTest()
        {
            Assert.AreEqual(
                expectedResult, Matrix.SingleThreadedMultiply(left, right));
        }

        [Test]
        public void SimpleMultiplicationShouldThrowIfDimensionsDontMatch()
        {
            Assert.That(() => Matrix.SingleThreadedMultiply(right, left),
                Throws.TypeOf<ArgumentException>()
                    .With.Message.EqualTo(
                    "Invalid input: matrices have invalid dimensions."));
        }

        [Test]
        public void SimpleMultiplicationWithWritingTest()
        {
            Matrix.MultiplyAndWrite(left, right, false, outputFilePath);
            var actualString = File.ReadAllText(outputFilePath);
            
            Assert.AreEqual(expectedResultString, actualString);
        }

        [Test]
        public void ThreadedMultiplicationTest()
        {
            Assert.AreEqual(expectedResult, Matrix.MultiThreadedMultiply(left, right));
        }

        [Test]
        public void ThreadedMultiplicationShouldThrowIfDimensionsDontMatch()
        {
            Assert.That(() => Matrix.MultiThreadedMultiply(right, left),
                Throws.TypeOf<ArgumentException>()
                    .With.Message.EqualTo(
                    "Invalid input: matrices have invalid dimensions."));
        }

        [Test]
        public void ThreadedMultiplicationWithWritingTest()
        {
            Matrix.MultiplyAndWrite(left, right, true, outputFilePath);
            var actualString = File.ReadAllText(outputFilePath);

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
