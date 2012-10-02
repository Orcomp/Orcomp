using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Orcomp.Entities;
using Orcomp.Extensions;
using Orcomp.Submissions;

namespace Orcomp.Benchmarks
{
    public delegate IEnumerable<DateTime> GetSortedDateTimesSoftwareBender(IEnumerable<DateRange> orderedDateRanges);
    class Program
    {
        static void Main(string[] args)
        {
            // ListWeaverBenchmark.Run();

            DateRangeSortBenchmark.Run();

        }
    }
}
