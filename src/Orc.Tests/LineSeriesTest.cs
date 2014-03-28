// // --------------------------------------------------------------------------------------------------------------------
// // <copyright file="IDateInterval.cs" company="ORC">
// //   MS-PL
// // </copyright>
// // <summary>
// //  
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------
namespace Orc.Tests
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Design;
    using System.Linq;
    using System.Security.Cryptography.X509Certificates;

    using NUnit.Framework;

    using System.Windows;

    using Orc.DataStructures.AList;
//  using Orc.Linq;
    using Orc.Utilities;

    [TestFixture]
    public class LineSeriesTest
    {
        [Test]
        public void InterpolateTest()
        {
            var points = new List<Point>() { new Point(0, 0), new Point(10, 10), new Point(30, -10), new Point(50, 10) };

            var lineSeries = new LineSeries(points);

            var result = lineSeries.Interpolate(new Double[] { 0, 5, 10, 15, 20, 25, 35, 40, 45 });

            var expectedResult = new List<Point>()
                                     {
                                         new Point(0,0), 
                                         new Point(5,5), 
                                         new Point(10,10), 
                                         new Point(15,5), 
                                         new Point(20, 0), 
                                         new Point(25, -5), 
                                         new Point(35,-5),
                                         new Point(40,0),
                                         new Point(45,5)
                                     };

            CollectionAssert.AreEqual(expectedResult, result);
        }

        [TestCase(LineSeries.SearchType.Best)]
        [TestCase(LineSeries.SearchType.Binary)]
        [TestCase(LineSeries.SearchType.Linear)]
        public void InterpolatePerformance1(LineSeries.SearchType search_type)
        {
            var size = 1000000;

            var valuePoints = new List<Point>() {
                new Point(0, 0),
                new Point(size, size),
            };
            var interPoints = Enumerable.Range(0, size).Select(x => x * 1.0).ToList();

            var series = new LineSeries(valuePoints);
            var result = series.Interpolate(interPoints, search_type).ToList();

            var expected = Enumerable.Range(0, size).Select(x => new Point(x, x)).ToList();

            CollectionAssert.AreEqual(expected, result);
        }

        [TestCase(LineSeries.SearchType.Best)]
        [TestCase(LineSeries.SearchType.Binary)]
        [TestCase(LineSeries.SearchType.Linear)]
        public void InterpolatePerformance2(LineSeries.SearchType search_type)
        {
            var valueGroups = Enumerable.Range(0, 1000001).Select(x => x * 1.0).ToLookup(x => ((int)x) % 2);

            var interPoints = valueGroups[1].ToList();
            var valuePoints = valueGroups[0].Select(x => new Point(x, x)).ToList();

            var series = new LineSeries(valuePoints);
            var result = series.Interpolate(interPoints, search_type).ToList();
        }

        [TestCase(LineSeries.SearchType.Best)]
        [TestCase(LineSeries.SearchType.Binary)]
        [TestCase(LineSeries.SearchType.Linear)]
        public void InterpolatePerformance3(LineSeries.SearchType search_type)
        {
            var randNumbers = new Random(0);

            var interPoints = new List<double>();
            var valuePoints = new List<Point>();

            for (int i = 0; i < 100000; i++)
            {
                interPoints.Add(interPoints.LastOrDefault() + 100 * randNumbers.NextDouble());
            }

            for (int i = 0; i < 10000000; i++)
            {
                valuePoints.Add(new Point(valuePoints.LastOrDefault().X + randNumbers.NextDouble(), randNumbers.NextDouble()));
            }

            var series = new LineSeries(valuePoints);
            var result = series.Interpolate(interPoints, search_type).ToList();
        }
    }
}