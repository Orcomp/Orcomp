// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DateIntervalTest.cs" company="">
//   
// </copyright>
// <summary>
//   The date interval test.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Orc.Tests
{
    using System;

    using NUnit.Framework;

    using Orc.Entities;

    /// <summary>
    /// The date interval test.
    /// </summary>
    [TestFixture]
    public class DateIntervalTest : DateIntervalTestBase
    {
        #region Intersects

        /// <summary>
        /// The intersects_ date before date interval_ return false.
        /// </summary>
        [Test]
        public void Intersects_DateBeforeDateInterval_ReturnFalse()
        {
            // Arrange
            var dateInterval = new DateInterval(now, inOneHour);

            // Act
            bool intersect = dateInterval.Intersects(now.AddHours(-1));

            // Assert
            Assert.False(intersect);
        }

        /// <summary>
        /// The intersects_ date after date interval_ return false.
        /// </summary>
        [Test]
        public void Intersects_DateAfterDateInterval_ReturnFalse()
        {
            // Arrange
            var dateInterval = new DateInterval(now, inOneHour);

            // Act
            bool interesct = dateInterval.Intersects(now.AddHours(2));

            // Assert
            Assert.False(interesct);
        }

        /// <summary>
        /// The intersects_ date in date interval_ return true.
        /// </summary>
        [Test]
        public void Intersects_DateInDateInterval_ReturnTrue()
        {
            // Arrange
            var dateInterval = new DateInterval(now, inOneHour);

            // Act
            bool interesct = dateInterval.Intersects(now.AddHours(0.5));

            // Assert
            Assert.True(interesct);
        }

        /// <summary>
        /// The intersects_ date on start date interval_ return true.
        /// </summary>
        [Test]
        public void Intersects_DateOnStartDateInterval_ReturnTrue()
        {
            // Arrange
            var dateInterval = new DateInterval(now, inOneHour);

            // Act
            bool interesct = dateInterval.Intersects(now);

            // Assert
            Assert.True(interesct);
        }

        /// <summary>
        /// The intersects_ date on end date interval_ return false.
        /// </summary>
        [Test]
        public void Intersects_DateOnEndDateInterval_ReturnFalse()
        {
            // Arrange
            var dateInterval = new DateInterval(now, inOneHour);

            // Act
            bool interesct = dateInterval.Intersects(inOneHour);

            // Assert
            Assert.False(interesct);
        }

        /// <summary>
        /// The intersects_ date on end date interval end date inclusive_ return true.
        /// </summary>
        [Test]
        public void Intersects_DateOnEndDateIntervalEndDateInclusive_ReturnTrue()
        {
            // Arrange
            var dateInterval = new DateInterval(now, inOneHour, isMaxInclusive: true);

            // Act
            bool interesct = dateInterval.Intersects(inOneHour);

            // Assert
            Assert.True(interesct);
        }

        #endregion

        #region Overlaps

        /// <summary>
        /// The overlaps_ date interval before date interval_ return false.
        /// </summary>
        [Test]
        public void Overlaps_DateIntervalBeforeDateInterval_ReturnFalse()
        {
            // Arrange
            var dateInterval = new DateInterval(inTwoHours, inThreeHours);
            var beforeDateInterval = new DateInterval(now, inOneHour);

            // Act
            bool overlaps = dateInterval.Overlaps(beforeDateInterval);

            // Assert
            Assert.False(overlaps);
        }

        /// <summary>
        /// The overlaps_ date interval after date interval_ return false.
        /// </summary>
        [Test]
        public void Overlaps_DateIntervalAfterDateInterval_ReturnFalse()
        {
            // Arrange
            var dateInterval = new DateInterval(now, inOneHour);
            var afterDateInterval = new DateInterval(inTwoHours, inThreeHours);

            // Act
            bool overlaps = dateInterval.Overlaps(afterDateInterval);

            // Assert
            Assert.False(overlaps);
        }

        /// <summary>
        /// The overlaps_ date interval inside date interval_ return true.
        /// </summary>
        [Test]
        public void Overlaps_DateIntervalInsideDateInterval_ReturnTrue()
        {
            // Arrange
            var dateInterval = new DateInterval(now, inThreeHours);
            var afterDateInterval = new DateInterval(inOneHour, inTwoHours);

            // Act
            bool overlaps = dateInterval.Overlaps(afterDateInterval);

            // Assert
            Assert.True(overlaps);
        }

        #endregion

        #region Equality

        /// <summary>
        /// The equal_ date interval with same start and end dates_ return true.
        /// </summary>
        [Test]
        public void Equal_DateIntervalWithSameStartAndEndDates_ReturnTrue()
        {
            // Arrange
            var dateInterval1 = new DateInterval(now, inOneHour);
            var dateInterval2 = new DateInterval(now, inOneHour);

            // Act
            bool equals = dateInterval1.Equals(dateInterval2);

            // Assert
            Assert.True(equals);
        }

        /// <summary>
        /// The equal_ date interval with same start and end dates include end points_ return true.
        /// </summary>
        [Test]
        public void Equal_DateIntervalWithSameStartAndEndDatesIncludeEndPoints_ReturnTrue()
        {
            // Arrange
            var dateInterval1 = new DateInterval(now, inOneHour, true, true);
            var dateInterval2 = new DateInterval(now, inOneHour, true, true);

            // Act
            bool equals = dateInterval1.Equals(dateInterval2);

            // Assert
            Assert.True(equals);
        }

        /// <summary>
        /// The equal_ date interval with same start and end dates exclude end points_ return true.
        /// </summary>
        [Test]
        public void Equal_DateIntervalWithSameStartAndEndDatesExcludeEndPoints_ReturnTrue()
        {
            // Arrange
            var dateInterval1 = new DateInterval(now, inOneHour, false, false);
            var dateInterval2 = new DateInterval(now, inOneHour, false, false);

            // Act
            bool equals = dateInterval1.Equals(dateInterval2);

            // Assert
            Assert.True(equals);
        }

        /// <summary>
        /// The equal_ date interval with same start and different end dates_ return false.
        /// </summary>
        [Test]
        public void Equal_DateIntervalWithSameStartAndDifferentEndDates_ReturnFalse()
        {
            // Arrange
            var dateInterval1 = new DateInterval(now, inOneHour);
            var dateInterval2 = new DateInterval(now, inThreeHours);

            // Act
            bool equals = dateInterval1.Equals(dateInterval2);

            // Assert
            Assert.False(equals);
        }

        /// <summary>
        /// The equal_ date interval with different start and same end dates_ return false.
        /// </summary>
        [Test]
        public void Equal_DateIntervalWithDifferentStartAndSameEndDates_ReturnFalse()
        {
            // Arrange
            var dateInterval1 = new DateInterval(now, inThreeHours);
            var dateInterval2 = new DateInterval(inOneHour, inThreeHours);

            // Act
            bool equals = dateInterval1.Equals(dateInterval2);

            // Assert
            Assert.False(equals);
        }

        /// <summary>
        /// The equal_ date interval with same start and same end dates exclude start include end_ returns true.
        /// </summary>
        [Test]
        public void Equal_DateIntervalWithSameStartAndSameEndDatesExcludeStartIncludeEnd_ReturnsTrue()
        {
            // Arrange
            var dateInterval1 = new DateInterval(now, inOneHour, false, true);
            var dateInterval2 = new DateInterval(now, inOneHour, false, true);

            // Act
            bool value = dateInterval1.Equals(dateInterval2);

            // Assert   
            Assert.True(value);
        }

        /// <summary>
        /// The equal_ date interval with same start and same end different inclusions_ returns false.
        /// </summary>
        [TestCase(true, false, false, false)]
        [TestCase(true, true, false, false)]
        [TestCase(true, true, true, false)]
        public void Equal_DateIntervalWithSameStartAndSameEndDifferentInclusions_ReturnsFalse(
            bool minInclusive1, bool maxInclusive1,
            bool minInclusive2, bool maxInclusive2)
        {
            // Arrange
            var dateInterval1 = new DateInterval(now, inOneHour, minInclusive1, maxInclusive1);
            var dateInterval2 = new DateInterval(now, inOneHour, minInclusive2, maxInclusive2);

            // Act
            bool equals = dateInterval1.Equals(dateInterval2);

            // Assert   
            Assert.False(equals);
        }

        #endregion

        #region CompareTo

        /// <summary>
        /// The compare to_ date interval with same start and same end dates_ returns zero.
        /// </summary>
        [Test]
        public void CompareTo_DateIntervalWithSameStartAndSameEndDates_ReturnsZero()
        {
            // Arrange
            var dateInterval1 = new DateInterval(now, inOneHour);
            var dateInterval2 = new DateInterval(now, inOneHour);

            // Act
            int compareToValue = dateInterval1.CompareTo(dateInterval2); // Shortest duration comes first

            // Assert
            Assert.True(compareToValue == 0);
        }

        /// <summary>
        /// The compare to_ date interval with same start and different end dates_ returns shortest duration first.
        /// </summary>
        [Test]
        public void CompareTo_DateIntervalWithSameStartAndDifferentEndDates_ReturnsShortestDurationFirst()
        {
            // Arrange            
            var dateInterval1 = new DateInterval(now, inOneHour);
            var dateInterval2 = new DateInterval(now, inThreeHours);

            // Act
            int compareToValue = dateInterval1.CompareTo(dateInterval2); // Shortest duration comes first

            // Assert
            Assert.True(compareToValue == -1);
        }

        /// <summary>
        /// The compare to_ date interval with same start and different end dates_ returns shortest duration first reversed.
        /// </summary>
        [Test]
        public void CompareTo_DateIntervalWithSameStartAndDifferentEndDates_ReturnsShortestDurationFirstReversed()
        {
            // Arrange
            var dateInterval1 = new DateInterval(now, inThreeHours);
            var dateInterval2 = new DateInterval(now, inOneHour);

            // Act
            int compareToValue = dateInterval1.CompareTo(dateInterval2); // Shortest duration comes first

            // Assert
            Assert.True(compareToValue == 1);
        }

        /// <summary>
        /// The compare to_ date interval with different start and different end dates_ returns earliest start.
        /// </summary>
        [Test]
        public void CompareTo_DateIntervalWithDifferentStartAndDifferentEndDates_ReturnsEarliestStart()
        {
            // Arrange
            var dateInterval1 = new DateInterval(now, inOneHour);
            var dateInterval2 = new DateInterval(inOneHour, inThreeHours);

            // Act
            int compareToValue = dateInterval1.CompareTo(dateInterval2);

            // Assert   
            Assert.True(compareToValue == -1);
        }

        #endregion
    }
}