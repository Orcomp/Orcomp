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
    using System;
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Orc.Entities;
    using Orc.Extensions;

    using Assert = Xunit.Assert;

    /// <summary>
    /// The date interval collection extensions test.
    /// </summary>
    [TestClass]
    public class DateIntervalCollectionExtensionsTest
    {
        /// <summary>
        /// The overlaps with_ multiple date intervals_ return correct answer.
        /// </summary>
        [TestMethod]
        public void OverlapsWith_MultipleDateIntervals_ReturnCorrectAnswer()
        {
            var dateIntervalCollection = new DateIntervalCollection();

            var now = DateTime.Now;

            var dateInterval1 = new DateInterval(now, now.AddDays(10));
            var dateInterval2 = new DateInterval(now, now.AddDays(5));
            var dateInterval3 = new DateInterval(now.AddDays(2), now.AddDays(5));
            var dateInterval4 = new DateInterval(now.AddDays(-3), now.AddDays(12));
            var dateInterval5 = new DateInterval(now.AddDays(13), now.AddDays(14));

            dateIntervalCollection.Add(dateInterval1);
            dateIntervalCollection.Add(dateInterval2);
            dateIntervalCollection.Add(dateInterval3);
            dateIntervalCollection.Add(dateInterval5);

            var correctResult = new List<DateInterval> { dateInterval1, dateInterval2, dateInterval3 };

            var result = dateIntervalCollection.OverlapsWith(dateInterval4);

            Assert.Equal(correctResult, result);
        }

        /// <summary>
        /// The overlaps with_ multiple date intervals_ return correct answer 2.
        /// </summary>
        [TestMethod]
        public void OverlapsWith_MultipleDateIntervals_ReturnCorrectAnswer2()
        {
            var dateIntervalCollection = new DateIntervalCollection();

            var now = DateTime.Now;

            var dateInterval1 = new DateInterval(now, now.AddDays(10));
            var dateInterval2 = new DateInterval(now, now.AddDays(5));
            var dateInterval3 = new DateInterval(now.AddDays(2), now.AddDays(5));
            var dateInterval4 = new DateInterval(now.AddDays(-3), now.AddDays(12));
            var dateInterval5 = new DateInterval(now.AddDays(13), now.AddDays(14));

            dateIntervalCollection.Add(dateInterval1);
            dateIntervalCollection.Add(dateInterval2);
            dateIntervalCollection.Add(dateInterval4);
            dateIntervalCollection.Add(dateInterval5);

            var correctResult = new List<DateInterval> { dateInterval1, dateInterval2, dateInterval4 };

            var result = dateIntervalCollection.OverlapsWith(dateInterval3);

            Assert.Equal(correctResult, result);
        }
    }
}