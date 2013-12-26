namespace Orc.Benchmarks
{
    using System;
    using System.Collections.Generic;

    using Orc.Interval;

    public delegate IEnumerable<DateTime> GetSortedDateTimesSoftwareBender(IEnumerable<DateInterval> orderedDateRanges);
    class Program
    {
        static void Main(string[] args)
        {
            // ListWeaverBenchmark.Run();

            DateIntervalSortBenchmark.RunMulti();
        }
    }
}
