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
    using System.Linq;

    using NUnit.Framework;

    using System.Windows;

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

        [Test]
        public void InterpolatePerformance1()
        {
            var points = new List<Point>() { new Point(0, 0), new Point(1000000, 1000000)};

            var lineSeries = new LineSeries(points);

            var result = lineSeries.Interpolate(Enumerable.Range(1, 999999).Select(x => x * 1.0));
        }
         
    }
}