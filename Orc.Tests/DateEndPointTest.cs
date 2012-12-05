// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DateEndpointTest.cs" company="">
//   
// </copyright>
// <summary>
//   The date endpoint test.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Orc.Tests
{
    using System;

    using NUnit.Framework;

    using Orc.Interval;

    [TestFixture]
    public class DateEndPointTest
    {
        protected DateTime now;

        protected DateTime inOneHour;

        protected DateTime inTwoHours;

        protected DateTime inThreeHours;

        /// <summary>
        /// Setups tests common data.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            now = DateTime.Now;
            inOneHour = now.AddHours(1);
            inTwoHours = now.AddHours(2);
            inThreeHours = now.AddHours(3);
        }

        #region Equals
    
        [Test]
        public void Equals_WithNull_ReturnsFalse()
        {
            //Arrange
            var endPoint = new DateEndPoint(new DateInterval(now, inOneHour), EndPointType.Min);

            //Act
            bool equals = endPoint.Equals(null);

            //Assert
            Assert.IsFalse(equals);
        }

        [Test]
        [TestCase(true, true, true)]
        [TestCase(true, false, false)]
        [TestCase(false, true, false)]
        [TestCase(false, false, true)]
        public void Equals_TheSameEndpointValue_ReturnsCorrectExpectedResult(bool includeFirstIntervalEdge, bool includeSecondIntervalEdge, bool expectedResult)
        {
            //Arrange
            var endPoint1 = new DateEndPoint(new DateInterval(now, inOneHour, includeFirstIntervalEdge), EndPointType.Min);
            var endPoint2 = new DateEndPoint(new DateInterval(now, inTwoHours, includeSecondIntervalEdge), EndPointType.Min);

            //Act
            bool equals = endPoint1.Equals(endPoint2);

            //Assert
            Assert.AreEqual(expectedResult, equals);
        }

        [Test]
        [TestCase(true, true, false)]
        [TestCase(false, true, false)]
        [TestCase(true, false, false)]
        [TestCase(false, false, false)]
        public void Equals_DifferentEndpointValues_ReturnsFalse(bool includeFirstIntervalEdge, bool includeSecondIntervalEdge, bool expectedResult)
        {
            //Arrange
            var endPoint1 = new DateEndPoint(new DateInterval(now, inOneHour), EndPointType.Min);
            var endPoint2 = new DateEndPoint(new DateInterval(inTwoHours, inThreeHours), EndPointType.Min);

            //Act
            bool equals = endPoint1.Equals(endPoint2);

            //Assert
            Assert.AreEqual(expectedResult, equals);
        }

        [Test]
        public void Equals_SameEnpontValueDifferentEndpointType_ReturnsFalse()
        {
            var endPoint1 = new DateEndPoint(new DateInterval(now, inOneHour), EndPointType.Max);
            var endPoint2 = new DateEndPoint(new DateInterval(inOneHour, inTwoHours), EndPointType.Min);

            //Act
            bool equals = endPoint1.Equals(endPoint2);

            //Assert
            Assert.IsFalse(equals);
        }

        #endregion

        #region CompareTo

        [Test]
        public void CompareTo_Null_Returns_1()
        {
            //Arrange
            var endPoint = new DateEndPoint(new DateInterval(now, inOneHour), EndPointType.Min);

            //Act
            int compareResult = endPoint.CompareTo(null);

            //Assert
            Assert.AreEqual(1, compareResult);
        }

        // (,) - means exclusive enpoints. [,] - means inclusive endpoints
        [Test]
        //endPoint1: [
        //endPoint2: [
        [TestCase(true, true, EndPointType.Min, EndPointType.Min, 0)]
        //endPoint1: [
        //endPoint2: (
        [TestCase(true, false, EndPointType.Min, EndPointType.Min, -1)]
        //endPoint1: (
        //endPoint2: [
        [TestCase(false, true, EndPointType.Min, EndPointType.Min, 1)]
        //endPoint1: (
        //endPoint2: (
        [TestCase(false, false, EndPointType.Min, EndPointType.Min, 0)]

        //endPoint1: [
        //endPoint2: ]
        [TestCase(true, true, EndPointType.Min, EndPointType.Max, 0)]
        //endPoint1: [
        //endPoint2: )
        [TestCase(true, false, EndPointType.Min, EndPointType.Max, 1)]
        //endPoint1: (
        //endPoint2: ]
        [TestCase(false, true, EndPointType.Min, EndPointType.Max, 1)]
        //endPoint1: (
        //endPoint2: )
        [TestCase(false, false, EndPointType.Min, EndPointType.Max, 1)]

        //endPoint1: ]
        //endPoint2: [
        [TestCase(true, true, EndPointType.Max, EndPointType.Min, 0)]
        //endPoint1: ]
        //endPoint2: (
        [TestCase(true, false, EndPointType.Max, EndPointType.Min, -1)]
        //endPoint1: )
        //endPoint2: [
        [TestCase(false, true, EndPointType.Max, EndPointType.Min, -1)]
        //endPoint1: )
        //endPoint2: (
        [TestCase(false, false, EndPointType.Max, EndPointType.Min, -1)]

        //endPoint1: ]
        //endPoint2: ]
        [TestCase(true, true, EndPointType.Max, EndPointType.Max, 0)]
        //endPoint1: ]
        //endPoint2: )
        [TestCase(true, false, EndPointType.Max, EndPointType.Max, 1)]
        //endPoint1: )
        //endPoint2: ]
        [TestCase(false, true, EndPointType.Max, EndPointType.Max, -1)]
        //endPoint1: ]
        //endPoint2: ]
        [TestCase(false, false, EndPointType.Max, EndPointType.Max, 0)]
        public void CompareTo_TheSameEndpointValue_ReturnsCorrectExpectedResult(
            bool includeFirstIntervalEdge, bool includeSecondIntervalEdge,
            EndPointType firstEndPointType, EndPointType secondEndPointType,
            int expectedResult)
        {
            //Arrange
            // use 0 length intervals here for easier testing
            var endPoint1 = new EndPoint<DateTime>(new Interval<DateTime>(now, now, firstEndPointType != EndPointType.Min || includeFirstIntervalEdge, firstEndPointType != EndPointType.Max || includeFirstIntervalEdge), firstEndPointType);
            var endPoint2 = new EndPoint<DateTime>(new Interval<DateTime>(now, now, secondEndPointType != EndPointType.Min || includeSecondIntervalEdge, secondEndPointType != EndPointType.Max || includeSecondIntervalEdge), secondEndPointType);

            //Act
            int compareResult = endPoint1.CompareTo(endPoint2);

            //Assert
            Assert.AreEqual(expectedResult, compareResult);
        }

        [Test]
        [TestCase(true, true, EndPointType.Min, EndPointType.Min, -1)]
        [TestCase(true, false, EndPointType.Min, EndPointType.Min, -1)]
        [TestCase(false, true, EndPointType.Min, EndPointType.Min, -1)]
        [TestCase(false, false, EndPointType.Min, EndPointType.Min, -1)]

        [TestCase(true, true, EndPointType.Min, EndPointType.Max, -1)]
        [TestCase(true, false, EndPointType.Min, EndPointType.Max, -1)]
        [TestCase(false, true, EndPointType.Min, EndPointType.Max, -1)]
        [TestCase(false, false, EndPointType.Min, EndPointType.Max, -1)]

        [TestCase(true, true, EndPointType.Max, EndPointType.Min, -1)]
        [TestCase(true, false, EndPointType.Max, EndPointType.Min, -1)]
        [TestCase(false, true, EndPointType.Max, EndPointType.Min, -1)]
        [TestCase(false, false, EndPointType.Max, EndPointType.Min, -1)]

        [TestCase(true, true, EndPointType.Max, EndPointType.Max, -1)]
        [TestCase(true, false, EndPointType.Max, EndPointType.Max, -1)]
        [TestCase(false, true, EndPointType.Max, EndPointType.Max, -1)]
        [TestCase(false, false, EndPointType.Max, EndPointType.Max, -1)]
        public void CompareTo_DifferentSmallerAndLargerEndpointValues_Returns_Minus_1(
            bool includeFirstIntervalEdge, bool includeSecondIntervalEdge,
            EndPointType firstEndPointType, EndPointType secondEndPointType,
            int expectedResult)
        {
            //Arrange
            var endPoint1 = new EndPoint<DateTime>(new Interval<DateTime>(now, inOneHour, firstEndPointType != EndPointType.Min || includeFirstIntervalEdge, firstEndPointType != EndPointType.Max || includeFirstIntervalEdge), firstEndPointType);
            var endPoint2 = new EndPoint<DateTime>(new Interval<DateTime>(inTwoHours, inThreeHours, secondEndPointType != EndPointType.Min || includeSecondIntervalEdge, secondEndPointType != EndPointType.Max || includeSecondIntervalEdge), secondEndPointType);


            //Act
            int compareResult = endPoint1.CompareTo(endPoint2);

            //Assert
            Assert.AreEqual(expectedResult, compareResult);
        }

        #endregion
    }
}