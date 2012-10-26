namespace Orc.Tests.IntervalContainer
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using System.Text;

	using NUnit.Framework;

	using Orc.Entities;
	using Orc.Interface;

	public abstract class DateIntervalContainerTestBase
	{
		private Stopwatch stopwatch;

		private StringBuilder timeEllapsedReport;

		private DateTime now;

		protected DateTime inOneHour;

		protected DateTime inTwoHours;

		protected DateTime inThreeHours;

		protected abstract IIntervalContainer<DateTime> CreateIntervalContainer(IEnumerable<Interval<DateTime>> intervals);

		[SetUp]
		public void Setup()
		{
			stopwatch = null;
			timeEllapsedReport = new StringBuilder();

			now = DateTime.Now;
			inOneHour = now.AddHours(1);
			inTwoHours = now.AddHours(2);
			inThreeHours = now.AddHours(3);
		}


		#region Benchmark	              

        [Test]
		[Category("Benchmark")]
		public void Query_Benchmark_Test()
        {
            const int numberOfIntervals = 10000;

            var intervals = GetDateRangesAllDescendingEndTimes(now, numberOfIntervals).ToList();
            
            stopwatch = Stopwatch.StartNew();

			IIntervalContainer<DateTime> intervalContainer = CreateIntervalContainer(intervals);

            stopwatch.Stop();
            timeEllapsedReport.AppendLine("Time taken to build tree: " + stopwatch.ElapsedMilliseconds);

			var result1 = TestSearchForInterval(now, now.AddMinutes(numberOfIntervals), intervalContainer, "first");

			var result2 = TestSearchForInterval(now.AddMinutes(-1), now.AddMinutes(1), intervalContainer, "second");

			var result3 = TestSearchForInterval(now.AddMinutes(-numberOfIntervals), now.AddMinutes(numberOfIntervals), intervalContainer, "third");

			var result4 = TestSearchForInterval(now.AddMinutes(numberOfIntervals - 1), now.AddMinutes(numberOfIntervals), intervalContainer, "fourth");            

            Assert.AreEqual(numberOfIntervals, result1.Count());

            Assert.AreEqual(numberOfIntervals, result2.Count());

            Assert.AreEqual(numberOfIntervals, result3.Count());

            Assert.AreEqual(2, result4.Count());
            
            var timeElapsedSummary = timeEllapsedReport.ToString();
            Debug.WriteLine(timeElapsedSummary);
        }

		private IEnumerable<IInterval<DateTime>> TestSearchForInterval(DateTime startEdge, DateTime endEdge, IIntervalContainer<DateTime> searchIn, string testName)
        {
            stopwatch.Reset();
            stopwatch.Start();

            var foundIntervals = searchIn.Query(new Interval<DateTime>(startEdge, endEdge));

            stopwatch.Stop();
            timeEllapsedReport.AppendLine(string.Format("Time taken for {0}: {1}", testName, stopwatch.ElapsedMilliseconds));
            return foundIntervals;
        }

        private static IEnumerable<Interval<DateTime>> GetDateRangesAllDescendingEndTimes(DateTime date, int count)
        {
            var dateRanges = new Interval<DateTime>[count];

            for (int i = 1; i <= count; i++)
            {
                dateRanges[i - 1] = new Interval<DateTime>(date.AddMinutes(-i), date.AddMinutes(i));
            }

            return new List<Interval<DateTime>>(dateRanges).OrderBy(x => x.Min.Value);
        }
		#endregion
	}
}
