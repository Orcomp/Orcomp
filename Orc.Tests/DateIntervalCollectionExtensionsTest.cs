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

    using Orc.Entities;
    using Orc.Extensions;

    /// <summary>
    /// The date interval collection extensions test.
    /// </summary>
    [TestFixture]
    public class DateIntervalCollectionExtensionsTest : DateIntervalCollectionTesBase
    {
        /// <summary>
        /// The overlaps with_ multiple date intervals_ return correct answer.
        /// </summary>
        [Test]
        public void OverlapsWith_MultipleDateIntervals_ReturnCorrectAnswer()
        {
            var dateIntervalCollection = new DateIntervalCollection();

            dateIntervalCollection.Add(nowAndTenDaysInterval);
            dateIntervalCollection.Add(nowAndFiveDaysInterval);
            dateIntervalCollection.Add(twoDaysAndFiveDaysInterval);
            dateIntervalCollection.Add(thirteenDaysAndFourteenDaysInterval);

            var correctResult = new List<DateInterval> { nowAndTenDaysInterval, nowAndFiveDaysInterval, twoDaysAndFiveDaysInterval };

            var result = dateIntervalCollection.OverlapsWith(threeDaysAgoAndTwelveDaysInterval);

            Assert.AreEqual(correctResult, result);
        }

        /// <summary>
        /// The overlaps with_ multiple date intervals_ return correct answer 2.
        /// </summary>
        [Test]
        public void OverlapsWith_MultipleDateIntervals_ReturnCorrectAnswer2()
        {
            var dateIntervalCollection = new DateIntervalCollection();

            dateIntervalCollection.Add(nowAndTenDaysInterval);
            dateIntervalCollection.Add(nowAndFiveDaysInterval);
            dateIntervalCollection.Add(threeDaysAgoAndTwelveDaysInterval);
            dateIntervalCollection.Add(thirteenDaysAndFourteenDaysInterval);

            var correctResult = new List<DateInterval> { nowAndTenDaysInterval, nowAndFiveDaysInterval, threeDaysAgoAndTwelveDaysInterval };

            var result = dateIntervalCollection.OverlapsWith(twoDaysAndFiveDaysInterval);

            Assert.AreEqual(correctResult, result);
        }
    }
}