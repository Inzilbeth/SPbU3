using NUnit.Framework;
using System;
using System.IO;
using Task1;

namespace Task1Tests
{
    [TestFixture]
    class ParserTests
    {
        private string writeFromFilePath = "matrix.txt";

        [Test]
        public void ParsesCommonMatrixCorrectlyTest()
        {
            var matrixString = "1 2 3 4 \n5 6 7 8 \n9 10 11 12 \n";

            File.WriteAllText(writeFromFilePath, matrixString);

            var rows = 3;
            var columns = 4;

            var expected = new Matrix(rows, columns);

            var currentValue = 1;

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++) 
                {
                    expected[i, j] = currentValue;
                    currentValue++;
                }
            }

            var actual = Parser.Parse(writeFromFilePath);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldThrowOnUnequalRowsSize()
        {
            var matrixString = "1 2 3 \n4 5 6 7 \n8 9 10 \n";

            File.WriteAllText(writeFromFilePath, matrixString);

            Assert.That(() => Parser.Parse(writeFromFilePath),
                Throws.TypeOf<ArgumentException>()
                    .With.Message.EqualTo(
                    "Invalid input: row sizes are unequal."));
        }

        [Test]
        public void ShouldThrowOnUnvalidSymbols()
        {
            var matrixString = "1 2 3 0 \n4 W 6 7 \n8 9 10 0 \n";

            File.WriteAllText(writeFromFilePath, matrixString);

            Assert.That(() => Parser.Parse(writeFromFilePath),
                Throws.TypeOf<ArgumentException>()
                    .With.Message.EqualTo(
                    "Invalid input: row contains invalid symbols."));
        }

        [TearDown]
        public void DeleteTemporaryFiles()
        {
            File.Delete(writeFromFilePath);
        }
    }
}
