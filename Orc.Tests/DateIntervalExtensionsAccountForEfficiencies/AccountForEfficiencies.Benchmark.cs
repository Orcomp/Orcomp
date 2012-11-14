using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

namespace Orc.Tests.DateIntervalExtensionsAccountForEfficiencies
{
    using System.Diagnostics;

    using Orc.Entities;
    using Orc.Extensions;

    public class AccountForEfficiencies
    {
        [Test]
        [Category("Benchmark")]
        public void benchamrk1()
        {

            var numberOfIntervals = 100000;

            var now = DateTime.Now.Date;

            var dateIntervalEfficiencies = Enumerable.Range(0, numberOfIntervals).Select(x => new DateIntervalEfficiency(now.AddMinutes(x), now.AddMinutes(x + 1), 1)).ToList();

            var initialInterval = new DateInterval(now, now.AddMinutes(numberOfIntervals/2));


            var stopwatch = Stopwatch.StartNew();
            var effectualInterval = initialInterval.AccountForEfficiencies(dateIntervalEfficiencies, FixedEndPoint.Min);
            stopwatch.Stop();

            Debug.WriteLine(string.Format("Elapsed time: {0} ms, result is: {1}", stopwatch.ElapsedMilliseconds, effectualInterval));
        }
    }
}
