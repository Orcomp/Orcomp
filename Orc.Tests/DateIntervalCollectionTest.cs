// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DateIntervalCollectionTest.cs" company="ORC">
//   MS-PL
// </copyright>
// <summary>
//   The date interval collection test.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Orc.Tests
{
    using System;
    using System.Collections.Generic;

    using NUnit.Framework;

    using Orc.Entities;
    using Orc.Extensions;
    using Orc.Interface;

    /// <summary>
    /// The date interval collection test.
    /// </summary>
    [TestFixture]
    public class DateIntervalCollectionTest : DateIntervalCollectionTesBase
    {
        /// <summary>
        /// The date edges_ add multiple date intervals_ return sorted date edges.
        /// </summary>
        [Test]
        [Ignore("This test causes test runners to fail with because of seckoverflow exception.")]
        public void DateEdges_AddMultipleDateIntervals_ReturnSortedDateEdges()
        {
            var dateIntervalCollection = new DateIntervalCollection();

            dateIntervalCollection.Add(nowAndTenDaysInterval);
            dateIntervalCollection.Add(nowAndFiveDaysInterval);
            dateIntervalCollection.Add(twoDaysAndFiveDaysInterval);
            dateIntervalCollection.Add(threeDaysAgoAndTwelveDaysInterval);
            dateIntervalCollection.Add(thirteenDaysAndFourteenDaysInterval);

            var dateIntervalList = new List<DateInterval>
            {
                nowAndTenDaysInterval, nowAndFiveDaysInterval, twoDaysAndFiveDaysInterval, threeDaysAgoAndTwelveDaysInterval, thirteenDaysAndFourteenDaysInterval 
            };

            var correctResult = new List<IEndPoint<DateTime>>();

            var result = dateIntervalCollection.DateEdges;

            dateIntervalList.ForEach(x => correctResult.AddRange(x.GetEndPoints()));

            CollectionAssert.AreEquivalent(correctResult, result);
        }
    }
}