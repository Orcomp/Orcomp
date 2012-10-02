using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Orcomp.Entities;
using Orcomp.Extensions;

using Assert = Xunit.Assert;

namespace Orcomp.Tests
{
    [TestClass]
    public class DateRangeCollectionExtensionsTest
    {
        [TestMethod]
        public void GetSortedDateTimes_ReturnsCorrectList()
        {
            DateTime dateNow = DateTime.Now;

            var orderedDateRanges = new List<DateRange>();
            orderedDateRanges.Add( new DateRange( dateNow, dateNow.AddMinutes( 100 )));             // +-----------------------------------------------+
            orderedDateRanges.Add(new DateRange(dateNow.AddMinutes(10), dateNow.AddMinutes(10)));   //   +
            orderedDateRanges.Add(new DateRange(dateNow.AddMinutes(10), dateNow.AddMinutes(10)));   //   +
            orderedDateRanges.Add(new DateRange(dateNow.AddMinutes(10), dateNow.AddMinutes(20)));   //   +---+
            orderedDateRanges.Add(new DateRange(dateNow.AddMinutes(10), dateNow.AddMinutes(30)));   //   +-------+
            orderedDateRanges.Add(new DateRange(dateNow.AddMinutes(20), dateNow.AddMinutes(60)));   //       +----------------------+
            orderedDateRanges.Add(new DateRange(dateNow.AddMinutes(30), dateNow.AddMinutes(60)));   //           +------------------+
            orderedDateRanges.Add(new DateRange(dateNow.AddMinutes(40), dateNow.AddMinutes(50)));   //              +------------+
            orderedDateRanges.Add(new DateRange(dateNow.AddMinutes(42), dateNow.AddMinutes(47)));   //                 +-----+
            orderedDateRanges.Add(new DateRange(dateNow.AddMinutes(45), dateNow.AddMinutes(45)));   //                    +
            orderedDateRanges.Add(new DateRange(dateNow.AddMinutes(70), dateNow.AddMinutes(75)));   //                                  +---+
            orderedDateRanges.Add(new DateRange(dateNow.AddMinutes(90), dateNow.AddMinutes(110)));  //                                          +--------------+
            orderedDateRanges.Add(new DateRange(dateNow.AddMinutes(120), dateNow.AddMinutes(140))); //                                                            +-----------+

            var result = orderedDateRanges.GetSortedDateTimes("");

            var correctDateTimes = new List<DateTime>();
            orderedDateRanges.ForEach(x => correctDateTimes.AddRange(new List<DateTime> { x.StartTime, x.EndTime }));
            correctDateTimes.Sort();

            Assert.Equal( correctDateTimes, result);
        }
    }
}
