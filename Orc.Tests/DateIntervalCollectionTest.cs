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

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Orc.Entities;
    using Orc.Extensions;
    using Orc.Interface;

    using Assert = Xunit.Assert;

    /// <summary>
    /// The date interval collection test.
    /// </summary>
    [TestClass]
    public class DateIntervalCollectionTest
    {
        /// <summary>
        /// The date edges_ add multiple date intervals_ return sorted date edges.
        /// </summary>
        [TestMethod]
        public void DateEdges_AddMultipleDateIntervals_ReturnSortedDateEdges()
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
            dateIntervalCollection.Add(dateInterval4);
            dateIntervalCollection.Add(dateInterval5);

            var dateIntervalList = new List<DateInterval>
                {
                   dateInterval1, dateInterval2, dateInterval3, dateInterval4, dateInterval5 
                };
            var correctResult = new List<IEndPoint<DateTime>>();

            var result = dateIntervalCollection.DateEdges;

            dateIntervalList.ForEach(x => correctResult.AddRange(x.GetEndPoints()));
            correctResult.Sort();

            Assert.Equal(correctResult, result);
        }
    }
}