// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DateIntervalCollectionExtensionsTest.cs" company="">
//   
// </copyright>
// <summary>
//   The date interval collection extensions test.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Orc.Tests
{
    using System.Collections.Generic;

    using NUnit.Framework;

    using Orc.Extensions;
    using Orc.Interval;

    /// <summary>
    /// The date interval collection extensions test.
    /// </summary>
    [TestFixture]
    public class DateIntervalCollectionExtensionsTest : DateIntervalCollectionTestBase
    {
        /// <summary>
        /// The overlaps with_ multiple date intervals_ return correct answer.
        /// </summary>
        [Test]
        public void OverlapsWith_MultipleDateIntervals_ReturnCorrectAnswer()
        {
            //Arrange
            var dateIntervalCollection = new DateIntervalCollection();

            dateIntervalCollection.Add(nowAndTenDaysInterval);
            dateIntervalCollection.Add(nowAndFiveDaysInterval);
            dateIntervalCollection.Add(twoDaysAndFiveDaysInterval);
            dateIntervalCollection.Add(thirteenDaysAndFourteenDaysInterval);

            var correctResult = new List<DateInterval> { nowAndTenDaysInterval, nowAndFiveDaysInterval, twoDaysAndFiveDaysInterval };

            //Act
            var result = dateIntervalCollection.OverlapsWith(threeDaysAgoAndTwelveDaysInterval);

            //Assert
            CollectionAssert.AreEquivalent(correctResult, result);
        }

        /// <summary>
        /// The overlaps with_ multiple date intervals_ return correct answer 2.
        /// </summary>
        [Test]
        [Ignore("Not implemented properly yet.")]
        public void OverlapsWith_MultipleDateIntervals_ReturnCorrectAnswer2()
        {
            //Arrange
            var dateIntervalCollection = new DateIntervalCollection();

            dateIntervalCollection.Add(nowAndTenDaysInterval);
            dateIntervalCollection.Add(nowAndFiveDaysInterval);
            dateIntervalCollection.Add(threeDaysAgoAndTwelveDaysInterval);
            dateIntervalCollection.Add(thirteenDaysAndFourteenDaysInterval);

            var correctResult = new List<DateInterval> { nowAndTenDaysInterval, nowAndFiveDaysInterval, threeDaysAgoAndTwelveDaysInterval };

            //Act
            var result = dateIntervalCollection.OverlapsWith(twoDaysAndFiveDaysInterval);

            //Assert
            CollectionAssert.AreEquivalent(correctResult, result);
        }
    }
}