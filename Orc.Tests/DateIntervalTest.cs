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
    using NUnit.Framework;

    using Orc.Interval;

    /// <summary>
    /// The date interval test.
    /// </summary>
    [TestFixture]
    public class DateIntervalTest : DateIntervalTestBase
    {
        #region Contains point

        /// <summary>
        /// The intersects_ date before date interval_ return false.
        /// </summary>
        [Test]
        public void Contains_DateBeforeDateInterval_ReturnFalse()
        {
            // Arrange
            var dateInterval = new DateInterval(now, inOneHour);

            // Act
            bool intersect = dateInterval.Contains(now.AddHours(-1));

            // Assert
            Assert.False(intersect);
        }

        /// <summary>
        /// The intersects_ date after date interval_ return false.
        /// </summary>
        [Test]
        public void Contains_DateAfterDateInterval_ReturnFalse()
        {
            // Arrange
            var dateInterval = new DateInterval(now, inOneHour);

            // Act
            bool interesct = dateInterval.Contains(now.AddHours(2));

            // Assert
            Assert.False(interesct);
        }

        /// <summary>
        /// The intersects_ date in date interval_ return true.
        /// </summary>
        [Test]
        public void Contains_DateInDateInterval_ReturnTrue()
        {
            // Arrange
            var dateInterval = new DateInterval(now, inOneHour);

            // Act
            bool interesct = dateInterval.Contains(now.AddHours(0.5));

            // Assert
            Assert.True(interesct);
        }

        /// <summary>
        /// The intersects_ date on start date interval_ return true.
        /// </summary>
        [Test]
        public void Contains_DateOnStartDateInterval_ReturnTrue()
        {
            // Arrange
            var dateInterval = new DateInterval(now, inOneHour);

            // Act
            bool interesct = dateInterval.Contains(now);

            // Assert
            Assert.True(interesct);
        }

        /// <summary>
        /// The intersects_ date on end date interval_ return false.
        /// </summary>
        [Test]
        public void Contains_DateOnEndDateInterval_ReturnFalse()
        {
            // Arrange
            var dateInterval = new DateInterval(now, inOneHour);

            // Act
            bool interesct = dateInterval.Contains(inOneHour);

            // Assert
            Assert.False(interesct);
        }

        /// <summary>
        /// The intersects_ date on end date interval end date inclusive_ return true.
        /// </summary>
        [Test]
        public void Contains_DateOnEndDateIntervalEndDateInclusive_ReturnTrue()
        {
            // Arrange
            var dateInterval = new DateInterval(now, inOneHour, isMaxInclusive: true);

            // Act
            bool interesct = dateInterval.Contains(inOneHour);

            // Assert
            Assert.True(interesct);
        }

        #endregion

        #region Contains interval

        /// <summary>
        /// The parent interval contains the other interval
        /// </summary>
        [Test]
        public void Contains_CurrentIntervalStartsBeforeAndFinishedAfterOtherInterval_ReturnTrue()
        {
            // Arrange
            var dateInterval = new DateInterval(now, inThreeHours);
            var otherInterval = new DateInterval(inOneHour, inTwoHours);

            // Act
            bool contains = dateInterval.Contains(otherInterval);

            // Assert
            Assert.True(contains);
        }

        /// <summary>
        /// The parent interval contains the other interval, matching start and end points
        /// </summary>
        [Test]
        public void Contains_CurrentIntervalStartsAndFinishedWithOtherInterval_ReturnTrue()
        {
            // Arrange
            var dateInterval = new DateInterval(now, inThreeHours);
            var otherInterval = new DateInterval(now, inThreeHours);

            // Act
            bool contains = dateInterval.Contains(otherInterval);

            // Assert
            Assert.True(contains);
        }

        /// <summary>
        /// The parent interval does not contain the other interval, matching start and end point values
        /// </summary>
        [TestCase(false, false, true, false)]
        [TestCase(false, false, false, true)]
        [TestCase(true, false, true, true)]
        [TestCase(true, false, true, true)]
        [TestCase(true, false, false, true)]
        public void Contains_CurrentIntervalStartAndEndValuesAreTheSameAsOtherIntervalButDifferentEndPointInclusion_ReturnFalse(
            bool minInclusive1, bool maxInclusive1,
            bool minInclusive2, bool maxInclusive2)
        {
            // Arrange
            var dateInterval = new DateInterval(now, inThreeHours, minInclusive1, maxInclusive1);
            var otherInterval = new DateInterval(now, inThreeHours, minInclusive2, maxInclusive2);

            // Act
            bool contains = dateInterval.Contains(otherInterval);

            // Assert
            Assert.False(contains);
        }

        /// <summary>
        /// The parent interval contains the other interval, matching start and end point values
        /// </summary>
        [TestCase(true, true, true, true)]
        [TestCase(true, true, true, false)]
        [TestCase(true, true, false, true)]
        [TestCase(true, true, false, false)]
        [TestCase(true, false, true, false)]
        [TestCase(true, false, false, false)]
        [TestCase(false, false, false, false)]
        public void Contains_CurrentIntervalStartAndEndValuesAreTheSameAsOtherIntervalButDifferentEndPointInclusion_ReturnTrue(
            bool minInclusive1, bool maxInclusive1,
            bool minInclusive2, bool maxInclusive2)
        {
            // Arrange
            var dateInterval = new DateInterval(now, inThreeHours, minInclusive1, maxInclusive1);
            var otherInterval = new DateInterval(now, inThreeHours, minInclusive2, maxInclusive2);

            // Act
            bool contains = dateInterval.Contains(otherInterval);

            // Assert
            Assert.True(contains);
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
            var internalDateInterval = new DateInterval(inOneHour, inTwoHours);

            // Act
            bool overlaps = dateInterval.Overlaps(internalDateInterval);

            // Assert
            Assert.True(overlaps);
        }

        /// <summary>
        /// Overlaps the two date intervals: first ends where second starts return correct overlap.
        /// </summary>
        [Test]
        [TestCase(true, true, true)]
        [TestCase(true, false, false)]
        [TestCase(false, true, false)]
        [TestCase(false, false, false)]
        public void Overlaps_TwoDateIntervalsFirstEndsWhereSecondStarts_ReturnCorrectOverlap(bool isFirstMaxInclusive, bool isSecondMinInclusive, bool overlapExpected)
        {
            // Arrange            
            var dateInterval = new DateInterval(now, inTwoHours, isMaxInclusive: isFirstMaxInclusive);
            var afterDateInterval = new DateInterval(inTwoHours, inThreeHours, isMinInclusive: isSecondMinInclusive);

            // Act
            bool overlaps = dateInterval.Overlaps(afterDateInterval);

            // Assert
            Assert.AreEqual(overlapExpected, overlaps);
        }

        #endregion

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
            DateInterval result = dateInterval.GetOverlap(beforeDateInterval) as DateInterval;

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
            DateInterval result = dateInterval.GetOverlap(afterDateInterval) as DateInterval;

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
            DateInterval result = dateInterval.GetOverlap(beforeDateInterval) as DateInterval;

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
            DateInterval result = dateInterval.GetOverlap(afterDateInterval) as DateInterval;

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
            DateInterval result = dateInterval.GetOverlap(startBeforeEndInDateInterval) as DateInterval;
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
            DateInterval result = dateInterval.GetOverlap(startBeforeEndAfterDateInterval) as DateInterval;
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
            DateInterval result = dateInterval.GetOverlap(startInEndInDateInterval) as DateInterval;
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
            DateInterval result = dateInterval.GetOverlap(startInteresctEndInteresctDateInterval) as DateInterval;
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
            DateInterval result = dateInterval.GetOverlap(startInEndAfterDateInterval) as DateInterval;
            DateInterval correctResult = new DateInterval(inOneHour, inTwoHours);

            // Assert
            Assert.AreEqual(correctResult, result);
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