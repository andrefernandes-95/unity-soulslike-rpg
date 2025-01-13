namespace AF
{
    using NUnit.Framework;

    public class AlgorithmsTests
    {
        [TestCase(0, false)]
        [TestCase(1, false)]
        [TestCase(2, true)]
        [TestCase(3, true)]
        [TestCase(4, false)]
        [TestCase(5, true)]
        [TestCase(6, false)]
        [TestCase(7, true)]
        [TestCase(8, false)]
        [TestCase(9, false)]
        [TestCase(10, false)]
        public void IsPrime(int number, bool expected)
        {
            Assert.AreEqual(expected, Algorithms.IsPrime(number));
        }
    }
}
