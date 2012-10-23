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
    public class DateIntervalTest
    {
        #region Intersects

        /// <summary>
        /// The intersects_ date before date interval_ return false.
        /// </summary>
        [Test]
        public void Intersects_DateBeforeDateInterval_ReturnFalse()
        {
            // Arrange
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(1);
            var dateInterval = new DateInterval(t1, t2);

            // Act
            bool intersect = dateInterval.Intersects(t1.AddHours(-1));

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
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(1);
            var dateInterval = new DateInterval(t1, t2);

            // Act
            bool interesct = dateInterval.Intersects(t1.AddHours(2));

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
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(1);
            var dateInterval = new DateInterval(t1, t2);

            // Act
            bool interesct = dateInterval.Intersects(t1.AddHours(0.5));

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
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(1);
            var dateInterval = new DateInterval(t1, t2);

            // Act
            bool interesct = dateInterval.Intersects(t1);

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
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(1);
            var dateInterval = new DateInterval(t1, t2);

            // Act
            bool interesct = dateInterval.Intersects(t2);

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
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(1);
            var dateInterval = new DateInterval(t1, t2, isMaxInclusive: true);

            // Act
            bool interesct = dateInterval.Intersects(t2);

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
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(1);
            DateTime t3 = t1.AddHours(2);
            DateTime t4 = t1.AddHours(3);

            var dateInterval = new DateInterval(t3, t4);
            var beforeDateInterval = new DateInterval(t1, t2);

            // Act
            bool result = dateInterval.Overlaps(beforeDateInterval);

            // Assert
            Assert.False(result);
        }

        /// <summary>
        /// The overlaps_ date interval after date interval_ return false.
        /// </summary>
        [Test]
        public void Overlaps_DateIntervalAfterDateInterval_ReturnFalse()
        {
            // Arrange
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(1);
            DateTime t3 = t1.AddHours(2);
            DateTime t4 = t1.AddHours(3);
            var dateInterval = new DateInterval(t1, t2);
            var afterDateInterval = new DateInterval(t3, t4);

            // Act
            bool result = dateInterval.Overlaps(afterDateInterval);

            // Assert
            Assert.False(result);
        }

        /// <summary>
        /// The overlaps_ date interval inside date interval_ return true.
        /// </summary>
        [Test]
        public void Overlaps_DateIntervalInsideDateInterval_ReturnTrue()
        {
            // Arrange
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(1);
            DateTime t3 = t1.AddHours(2);
            DateTime t4 = t1.AddHours(3);

            var dateInterval = new DateInterval(t1, t4);
            var afterDateInterval = new DateInterval(t2, t3);

            // Act
            bool result = dateInterval.Overlaps(afterDateInterval);

            // Assert
            Assert.True(result);
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
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(1);
            var dateInterval1 = new DateInterval(t1, t2);
            var dateInterval2 = new DateInterval(t1, t2);

            // Act
            bool result = dateInterval1.Equals(dateInterval2);

            // Assert
            Assert.True(result);
        }

        /// <summary>
        /// The equal_ date interval with same start and end dates include end points_ return true.
        /// </summary>
        [Test]
        public void Equal_DateIntervalWithSameStartAndEndDatesIncludeEndPoints_ReturnTrue()
        {
            // Arrange
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(1);
            var dateInterval1 = new DateInterval(t1, t2, true, true);
            var dateInterval2 = new DateInterval(t1, t2, true, true);

            // Act
            bool result = dateInterval1.Equals(dateInterval2);

            // Assert
            Assert.True(result);
        }

        /// <summary>
        /// The equal_ date interval with same start and end dates exclude end points_ return true.
        /// </summary>
        [Test]
        public void Equal_DateIntervalWithSameStartAndEndDatesExcludeEndPoints_ReturnTrue()
        {
            // Arrange
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(1);
            var dateInterval1 = new DateInterval(t1, t2, false, false);
            var dateInterval2 = new DateInterval(t1, t2, false, false);

            // Act
            bool result = dateInterval1.Equals(dateInterval2);

            // Assert
            Assert.True(result);
        }

        /// <summary>
        /// The equal_ date interval with same start and different end dates_ return false.
        /// </summary>
        [Test]
        public void Equal_DateIntervalWithSameStartAndDifferentEndDates_ReturnFalse()
        {
            // Arrange
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(1);
            DateTime t3 = t1.AddHours(3);
            var dateInterval1 = new DateInterval(t1, t2);
            var dateInterval2 = new DateInterval(t1, t3);

            // Act
            bool result = dateInterval1.Equals(dateInterval2);

            // Assert
            Assert.False(result);
        }

        /// <summary>
        /// The equal_ date interval with different start and same end dates_ return false.
        /// </summary>
        [Test]
        public void Equal_DateIntervalWithDifferentStartAndSameEndDates_ReturnFalse()
        {
            // Arrange
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(1);
            DateTime t3 = t1.AddHours(3);
            var dateInterval1 = new DateInterval(t1, t3);
            var dateInterval2 = new DateInterval(t2, t3);

            // Act
            bool result = dateInterval1.Equals(dateInterval2);

            // Assert
            Assert.False(result);
        }

        #endregion

        #region compareTo

        /// <summary>
        /// The compare to_ date interval with same start and same end dates_ returns zero.
        /// </summary>
        [Test]
        public void CompareTo_DateIntervalWithSameStartAndSameEndDates_ReturnsZero()
        {
            // Arrange
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(1);
            var dateInterval1 = new DateInterval(t1, t2);
            var dateInterval2 = new DateInterval(t1, t2);

            // Act
            int value = dateInterval1.CompareTo(dateInterval2); // Shortest duration comes first

            // Assert
            Assert.True(value == 0);
        }

        /// <summary>
        /// The compare to_ date interval with same start and different end dates_ returns shortest duration first.
        /// </summary>
        [Test]
        public void CompareTo_DateIntervalWithSameStartAndDifferentEndDates_ReturnsShortestDurationFirst()
        {
            // Arrange
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(1);
            DateTime t3 = t1.AddHours(3);
            var dateInterval1 = new DateInterval(t1, t2);
            var dateInterval2 = new DateInterval(t1, t3);

            // Act
            int value = dateInterval1.CompareTo(dateInterval2); // Shortest duration comes first

            // Assert
            Assert.True(value == -1);
        }

        /// <summary>
        /// The compare to_ date interval with same start and different end dates_ returns shortest duration first reversed.
        /// </summary>
        [Test]
        public void CompareTo_DateIntervalWithSameStartAndDifferentEndDates_ReturnsShortestDurationFirstReversed()
        {
            // Arrange
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(1);
            DateTime t3 = t1.AddHours(3);
            var dateInterval1 = new DateInterval(t1, t3);
            var dateInterval2 = new DateInterval(t1, t2);

            // Act
            int value = dateInterval1.CompareTo(dateInterval2); // Shortest duration comes first

            // Assert
            Assert.True(value == 1);
        }

        /// <summary>
        /// The compare to_ date interval with different start and different end dates_ returns earliest start.
        /// </summary>
        [Test]
        public void CompareTo_DateIntervalWithDifferentStartAndDifferentEndDates_ReturnsEarliestStart()
        {
            // Arrange
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(1);
            DateTime t3 = t1.AddHours(3);
            var dateInterval1 = new DateInterval(t1, t2);
            var dateInterval2 = new DateInterval(t2, t3);

            // Act
            int value = dateInterval1.CompareTo(dateInterval2);

            // Assert   
            Assert.True(value == -1);
        }

        #endregion

        #region EqualOperation

        /// <summary>
        /// The compare to_ date interval with same start and same end dates_ returns true.
        /// </summary>
        [Test]
        public void CompareTo_DateIntervalWithSameStartAndSameEndDates_ReturnsTrue()
        {
            // Arrange
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(1);
            var dateInterval1 = new DateInterval(t1, t2);
            var dateInterval2 = new DateInterval(t1, t2);

            // Act
            bool value = dateInterval1.Equals(dateInterval2);

            // Assert   
            Assert.True(value);
        }

        /// <summary>
        /// The compare to_ date interval with same start and same end dates include start include end_ returns true.
        /// </summary>
        [Test]
        public void CompareTo_DateIntervalWithSameStartAndSameEndDatesIncludeStartIncludeEnd_ReturnsTrue()
        {
            // Arrange
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(1);
            var dateInterval1 = new DateInterval(t1, t2, true, true);
            var dateInterval2 = new DateInterval(t1, t2, true, true);

            // Act
            bool value = dateInterval1.Equals(dateInterval2);

            // Assert   
            Assert.True(value);
        }

        /// <summary>
        /// The compare to_ date interval with same start and same end dates exclude start include end_ returns true.
        /// </summary>
        [Test]
        public void CompareTo_DateIntervalWithSameStartAndSameEndDatesExcludeStartIncludeEnd_ReturnsTrue()
        {
            // Arrange
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(1);
            var dateInterval1 = new DateInterval(t1, t2, false, true);
            var dateInterval2 = new DateInterval(t1, t2, false, true);

            // Act
            bool value = dateInterval1.Equals(dateInterval2);

            // Assert   
            Assert.True(value);
        }

        /// <summary>
        /// The compare to_ date interval with same start and same end dates exclude both_ returns true.
        /// </summary>
        [Test]
        public void CompareTo_DateIntervalWithSameStartAndSameEndDatesExcludeBoth_ReturnsTrue()
        {
            // Arrange
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(1);
            var dateInterval1 = new DateInterval(t1, t2, false, false);
            var dateInterval2 = new DateInterval(t1, t2, false, false);

            // Act
            bool value = dateInterval1.Equals(dateInterval2);

            // Assert   
            Assert.True(value);
        }

        /// <summary>
        /// The compare to_ date interval with same start and same end different inclusions_ returns false.
        /// </summary>
        [Test]
        public void CompareTo_DateIntervalWithSameStartAndSameEndDifferentInclusions_ReturnsFalse()
        {
            // Arrange
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(1);
            var dateInterval1 = new DateInterval(t1, t2, true, false);
            var dateInterval2 = new DateInterval(t1, t2, false, false);

            // Act
            bool value = dateInterval1.Equals(dateInterval2);

            // Assert   
            Assert.False(value);
        }

        /// <summary>
        /// The compare to_ date interval with same start and same end different inclusions 2_ returns false.
        /// </summary>
        [Test]
        public void CompareTo_DateIntervalWithSameStartAndSameEndDifferentInclusions2_ReturnsFalse()
        {
            // Arrange
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(1);
            var dateInterval1 = new DateInterval(t1, t2, true, true);
            var dateInterval2 = new DateInterval(t1, t2, false, false);

            // Act
            bool value = dateInterval1.Equals(dateInterval2);

            // Assert   
            Assert.False(value);
        }

        /// <summary>
        /// The compare to_ date interval with same start and same end different inclusions 3_ returns false.
        /// </summary>
        [Test]
        public void CompareTo_DateIntervalWithSameStartAndSameEndDifferentInclusions3_ReturnsFalse()
        {
            // Arrange
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(1);
            var dateInterval1 = new DateInterval(t1, t2, true, true);
            var dateInterval2 = new DateInterval(t1, t2, true, false);

            // Act
            bool value = dateInterval1.Equals(dateInterval2);

            // Assert   
            Assert.False(value);
        }

        #endregion
    }
}