using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Orcomp.Extensions;

namespace Orcomp.Tests
{
    [TestClass]
    public class DateTimeUtilitiesTest
    {
        [TestMethod]
        public void Ceil_WhenExecuted_ReturnsCeilingValue()
        {
            // Arrange
            DateTime date = new DateTime(2012, 4, 26, 12, 13, 14, 451);
            TimeSpan span = new TimeSpan((long)10000000);
            DateTime expected = new DateTime(2012, 4, 26, 12, 13, 15, 0);

            // Act
            DateTime actual = date.Ceil(span);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Floor_WhenExecuted_ReturnsFloorValue()
        {
            // Arrange
            DateTime date = new DateTime(2012, 4, 26, 12, 13, 14, 951);
            TimeSpan span = new TimeSpan((long)10000000);
            DateTime expected = new DateTime(2012, 4, 26, 12, 13, 14, 0);

            // Act
            DateTime actual = date.Floor(span);
            
            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void IsEqualTo_WhenBothDatesEqual_IsTrue()
        {
            DateTime d1 = DateTime.Now;
            DateTime d2 = d1;
            const int AccuracyInMilliseconds = 50;

            bool actual = d1.IsEqualTo(d2, AccuracyInMilliseconds);

            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void IsEqualTo_WhenBothDatesDifferByLessThanAccuracy_IsTrue()
        {
            DateTime d1 = DateTime.Now;
            DateTime d2 = d1.AddMilliseconds(10);
            const int AccuracyInMilliseconds = 50;

            bool actual = d1.IsEqualTo(d2, AccuracyInMilliseconds);
            
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void IsEqualTo_WhenBothDatesDifferByMoreThanAccuracy_IsFalse()
        {
            DateTime d1 = DateTime.Now;
            DateTime d2 = d1.AddMilliseconds(100);
            const int AccuracyInMilliseconds = 50;

            bool actual = d1.IsEqualTo(d2, AccuracyInMilliseconds);
            
            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void IsGreaterThan_WhenBothDatesDifferByLessThanAccuracy_IsFalse()
        {
            DateTime d1 = DateTime.Now;
            DateTime d2 = d1.AddMilliseconds(50);
            const int AccuracyInMilliseconds = 100;

            bool actual = d2.IsGreaterThan(d1, AccuracyInMilliseconds);

            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void IsGreaterThan_WhenBothDatesDifferByMoreThanAccuracy_IsTrue()
        {
            DateTime d1 = DateTime.Now;
            DateTime d2 = d1.AddMilliseconds(100);
            const int AccuracyInMilliseconds = 50;

            bool actual = d2.IsGreaterThan(d1, AccuracyInMilliseconds);

            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void IsLessThan_WhenBothDatesDifferByMoreThanAccuracy_IsFalse()
        {
            DateTime d1 = DateTime.Now;
            DateTime d2 = d1.AddMilliseconds(-100);
            int accuracyInMilliseconds = 50;

            bool actual = d2.IsLessThan(d1, accuracyInMilliseconds);

            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void IsLessThan_WhenBothDatesDifferByMoreThanAccuracy_IsTrue()
        {
            DateTime d1 = DateTime.Now;
            DateTime d2 = d1.AddMilliseconds(-100);
            int accuracyInMilliseconds = 50;

            bool actual = d2.IsLessThan(d1, accuracyInMilliseconds);

            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void Round_WhenExecuted_ReturnsRoundedValue()
        {
            DateTime date = new DateTime(2012, 4, 26, 12, 13, 14, 951);
            TimeSpan span = new TimeSpan((long)100000);
            DateTime expected = new DateTime(2012, 4, 26, 12, 13, 14, 950);

            DateTime actual = date.Round(span);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ToNearestSecond_WhenExecuted_ReturnsValueToNearestSecond()
        {
            DateTime d1 = new DateTime(2012, 4, 26, 12, 13, 14, 950);
            DateTime expected = new DateTime(2012, 4, 26, 12, 13, 15, 0);

            DateTime actual = d1.ToNearestSecond();

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TruncateToSecond_WhenExecuted_ReturnsValueRuncatedToSecond()
        {
            DateTime d1 = new DateTime(2012, 4, 26, 12, 13, 14, 950);
            DateTime expected = new DateTime(2012, 4, 26, 12, 13, 14, 0);

            DateTime actual = d1.TruncateToSecond();

            Assert.AreEqual(expected, actual);
        }
    }
}
