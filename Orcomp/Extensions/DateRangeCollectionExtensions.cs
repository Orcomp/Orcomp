using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

using Orcomp.Entities;

namespace Orcomp.Extensions
{
    public static class DateRangeCollectionExtensions
    {
        
        /// <summary>
        /// Assumption: The list parameter is already pre sorted.
        /// </summary>
        /// <param name="orderedDateRanges"></param>
        /// <returns></returns>
        public static IEnumerable<DateTime> GetSortedDateTimes(this List<DateRange> orderedDateRanges)
        {
            //throw new NotImplementedException();

            return Submissions.GetSortedDateTimes.Aus1( orderedDateRanges );
        }
    }
}
