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
    using System.Linq;

    using NUnit.Framework;

    using Orc.Extensions;
    using Orc.Interval;
    using Orc.Interval.Extensions;

    /// <summary>
    /// The date interval collection extensions test.
    /// </summary>
    [TestFixture]
    public class DateIntervalCollectionExtensionsTest : DateIntervalCollectionTestBase
    {
        [Test]
        public void GetSortedDateTimes_ReturnsCorrectList()
        {
            DateTime dateNow = DateTime.Now;

            var orderedDateIntervals = new List<DateInterval>();
            orderedDateIntervals.Add( new DateInterval( dateNow, dateNow.AddMinutes( 100 )));             // +-----------------------------------------------+
            orderedDateIntervals.Add(new DateInterval(dateNow.AddMinutes(10), dateNow.AddMinutes(10)));   //   +
            orderedDateIntervals.Add(new DateInterval(dateNow.AddMinutes(10), dateNow.AddMinutes(10)));   //   +
            orderedDateIntervals.Add(new DateInterval(dateNow.AddMinutes(10), dateNow.AddMinutes(20)));   //   +---+
            orderedDateIntervals.Add(new DateInterval(dateNow.AddMinutes(10), dateNow.AddMinutes(30)));   //   +-------+
            orderedDateIntervals.Add(new DateInterval(dateNow.AddMinutes(20), dateNow.AddMinutes(60)));   //       +----------------------+
            orderedDateIntervals.Add(new DateInterval(dateNow.AddMinutes(30), dateNow.AddMinutes(60)));   //           +------------------+
            orderedDateIntervals.Add(new DateInterval(dateNow.AddMinutes(40), dateNow.AddMinutes(50)));   //              +------------+
            orderedDateIntervals.Add(new DateInterval(dateNow.AddMinutes(42), dateNow.AddMinutes(47)));   //                 +-----+
            orderedDateIntervals.Add(new DateInterval(dateNow.AddMinutes(45), dateNow.AddMinutes(45)));   //                    +
            orderedDateIntervals.Add(new DateInterval(dateNow.AddMinutes(70), dateNow.AddMinutes(75)));   //                                  +---+
            orderedDateIntervals.Add(new DateInterval(dateNow.AddMinutes(90), dateNow.AddMinutes(110)));  //                                          +--------------+
            orderedDateIntervals.Add(new DateInterval(dateNow.AddMinutes(120), dateNow.AddMinutes(140))); //                                                            +-----------+

            var result = orderedDateIntervals.GetSortedDateTimes();

            var correctDateTimes = new List<DateTime>();
            orderedDateIntervals.ForEach(x => correctDateTimes.AddRange(new List<DateTime> { x.Min.Value, x.Max.Value }));
            correctDateTimes.Sort();

            Assert.AreEqual( correctDateTimes, result);
        }

        [Test]
        public void GetSortedDateTimes_SimpleInput()
        {
            DateTime dateNow = DateTime.Now;

            var orderedDateIntervals = new List<DateInterval>();
            orderedDateIntervals.Add(new DateInterval(dateNow, dateNow.AddMinutes(9)));            
            orderedDateIntervals.Add(new DateInterval(dateNow.AddMinutes(69), dateNow.AddMinutes(69)));
            var result = orderedDateIntervals.GetSortedDateTimes();

            var correctDateTimes = new List<DateTime>();
            orderedDateIntervals.ForEach(x => correctDateTimes.AddRange(new List<DateTime> { x.Min.Value, x.Max.Value }));
            correctDateTimes.Sort();

            Assert.AreEqual(correctDateTimes, result);
        }

        [Test]
        public void GetSortedDateTimes_RandomEndTimes()
        {
            var date = DateTime.Now;

            var r = new Random(1);
            var count = 100000;
            var orderedDateIntervals = Enumerable.Range( 1, count ).Select( x => new DateInterval( date.AddMinutes( x ), date.AddMinutes( x + r.Next( 0, 100 ) ) ) ).OrderBy( x => x ).ToList();

            var result = orderedDateIntervals.GetSortedDateTimes();

            var correctDateTimes = new List<DateTime>();
            orderedDateIntervals.ForEach(x => correctDateTimes.AddRange(new List<DateTime> { x.Min.Value, x.Max.Value }));
            correctDateTimes.Sort();

            Assert.AreEqual(correctDateTimes, result);
        }
    }
}