// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DateIntervalExtensionsTest.cs" company="ORC">
//   MS-PL
// </copyright>
// <summary>
//   The date interval extensions test.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Orc.Tests
{
    using NUnit.Framework;

    using Orc.Entities;
    using Orc.Extensions;

    /// <summary>
    /// The date interval extensions test.
    /// </summary>
    [TestFixture]
    public class DateIntervalExtensionsTest : DateIntervalTestBase
    {
        #region GetOverlap

        /// <summary>
        /// The get overlap_ date interval before date interval_ return null.
        /// </summary>
        [Test]
        public void GetOverlap_DateIntervalBeforeDateInterval_ReturnNull()
        {
            // Arrange
            var dateInterval = new DateInterval(inTwoHours, inThreeHours);
            var beforeDateInterval = new DateInterval(now, inOneHour);

            // Act
            DateInterval result = dateInterval.GetOverlap(beforeDateInterval);

            // Assert
            Assert.Null(result);
        }

        /// <summary>
        /// The get overlap_ date interval after date interval_ return null.
        /// </summary>
        [Test]
        public void GetOverlap_DateIntervalAfterDateInterval_ReturnNull()
        {
            // Arrange
            var dateInterval = new DateInterval(now, inOneHour);
            var afterDateInterval = new DateInterval(inTwoHours, inThreeHours);

            // Act
            DateInterval result = dateInterval.GetOverlap(afterDateInterval);

            // Assert
            Assert.Null(result);
        }

        /// <summary>
        /// The get overlap_ date interval before date interval interesct_ return null.
        /// </summary>
        [Test]
        public void GetOverlap_DateIntervalBeforeDateIntervalInteresct_ReturnNull()
        {
            // Arrange
            var dateInterval = new DateInterval(inOneHour, inTwoHours);
            var beforeDateInterval = new DateInterval(now, inOneHour);

            // Act
            DateInterval result = dateInterval.GetOverlap(beforeDateInterval);

            // Assert
            Assert.Null(result);
        }

        /// <summary>
        /// The get overlap_ date interval after date interval interect_ return null.
        /// </summary>
        [Test]
        public void GetOverlap_DateIntervalAfterDateIntervalInterect_ReturnNull()
        {
            // Arrange
            var dateInterval = new DateInterval(now, inOneHour);
            var afterDateInterval = new DateInterval(inOneHour, inTwoHours);

            // Act
            DateInterval result = dateInterval.GetOverlap(afterDateInterval);

            // Assert
            Assert.Null(result);
        }

        /// <summary>
        /// The get overlap_ date interval start before end in_ return correct date interval.
        /// </summary>
        [Test]
        public void GetOverlap_DateIntervalStartBeforeEndIn_ReturnCorrectDateInterval()
        {
            // Arrange
            var dateInterval = new DateInterval(inOneHour, inThreeHours);
            var startBeforeEndInDateInterval = new DateInterval(now, inTwoHours);

            // Act
            DateInterval result = dateInterval.GetOverlap(startBeforeEndInDateInterval);
            DateInterval correctResult = new DateInterval(inOneHour, inTwoHours);

            // Assert
            Assert.AreEqual(correctResult, result);
        }

        /// <summary>
        /// The get overlap_ date interval start before end after_ return correct date interval.
        /// </summary>
        [Test]
        public void GetOverlap_DateIntervalStartBeforeEndAfter_ReturnCorrectDateInterval()
        {
            // Arrange            
            var dateInterval = new DateInterval(inOneHour, inTwoHours);
            var startBeforeEndAfterDateInterval = new DateInterval(now, inThreeHours);

            // Act
            DateInterval result = dateInterval.GetOverlap(startBeforeEndAfterDateInterval);
            DateInterval correctResult = new DateInterval(inOneHour, inTwoHours);

            // Assert
            Assert.AreEqual(correctResult, result);
        }

        /// <summary>
        /// The get overlap_ date interval start in end in_ return correct date interval.
        /// </summary>
        [Test]
        public void GetOverlap_DateIntervalStartInEndIn_ReturnCorrectDateInterval()
        {
            // Arrange
            var dateInterval = new DateInterval(now, inThreeHours);
            var startInEndInDateInterval = new DateInterval(inOneHour, inTwoHours);

            // Act
            DateInterval result = dateInterval.GetOverlap(startInEndInDateInterval);
            DateInterval correctResult = new DateInterval(inOneHour, inTwoHours);

            // Assert
            Assert.AreEqual(correctResult, result);
        }

        /// <summary>
        /// The get overlap_ date interval start interesct end intersect_ return correct date interval.
        /// </summary>
        [Test]
        public void GetOverlap_DateIntervalStartInteresctEndIntersect_ReturnCorrectDateInterval()
        {
            // Arrange
            var dateInterval = new DateInterval(now, inOneHour);
            var startInteresctEndInteresctDateInterval = new DateInterval(now, inOneHour);

            // Act
            DateInterval result = dateInterval.GetOverlap(startInteresctEndInteresctDateInterval);
            DateInterval correctResult = new DateInterval(now, inOneHour);

            // Assert
            Assert.AreEqual(correctResult, result);
        }

        /// <summary>
        /// The get overlap_ date interval start in end after_ return correct date interval.
        /// </summary>
        [Test]
        public void GetOverlap_DateIntervalStartInEndAfter_ReturnCorrectDateInterval()
        {
            // Arrange
            var dateInterval = new DateInterval(now, inTwoHours);
            var startInEndAfterDateInterval = new DateInterval(inOneHour, inThreeHours);

            // Act
            DateInterval result = dateInterval.GetOverlap(startInEndAfterDateInterval);
            DateInterval correctResult = new DateInterval(inOneHour, inTwoHours);

            // Assert
            Assert.AreEqual(correctResult, result);
        }

        #endregion
    }
}