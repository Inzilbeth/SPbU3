using NUnit.Framework;
using System.IO;
using Task1;

namespace Task1Tests
{
    class OtherMatrixTests
    {
        private string[] paths = new string[2] { "first.txt", "second.txt" };

        [Test]
        public void EqualMatricesShouldPassEqualityCheck()
        {
            var matrixString = "4 3 7 1 \n2 4 8 10 \n15 4 6 9 \n";

            File.WriteAllText(paths[0], matrixString);

            var a = new Matrix(paths[0]);
            var b = new Matrix(paths[0]);

            Assert.IsTrue(a.Equals(b));
        }

        [Test]
        public void EqualMatricesShouldNotPassEqualityCheck()
        {
            var firstMatrixString = "4 3 7 1 \n2 4 8 10 \n15 4 6 9 \n";
            var secondMatrixString = "2 3 7 1 \n2 4 8 10 \n15 4 6 9 \n";

            File.WriteAllText(paths[0], firstMatrixString);
            File.WriteAllText(paths[1], secondMatrixString);

            var a = new Matrix(paths[0]);
            var b = new Matrix(paths[1]);

            Assert.IsFalse(a.Equals(b));
        }

        [Test]
        public void GettingValueByItsIndexTest()
        {
            var matrixString = "10 20 \n30 45 \n";

            File.WriteAllText(paths[0], matrixString);

            var matrix = new Matrix(paths[0]);

            Assert.AreEqual(10, matrix[0, 0]);
            Assert.AreEqual(20, matrix[0, 1]);
            Assert.AreEqual(30, matrix[1, 0]);
            Assert.AreEqual(45, matrix[1, 1]);
        }

        [Test]
        public void SettingValueByIndexTest()
        {
            var matrix = new Matrix(1, 1);

            var expectedValue = 777;
            matrix[0, 0] = expectedValue;

            Assert.AreEqual(expectedValue, matrix[0, 0]);
        }

        [OneTimeTearDown]
        public void DeleteFiles()
        {
            foreach (var path in paths)
            {
                File.Delete(path);
            }
        }
    }
}
