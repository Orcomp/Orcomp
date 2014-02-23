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

    using NUnit.Framework;

    using System.Windows;

    using Orc.Utilities;

    [TestFixture]
    public class LineSeriesTest
    {
        [Test]
        public void InterpolateTest()
        {
            var points = new List<Point>() { new Point(0, 0), new Point(10, 10), new Point(30, -10), new Point(40, 0) };

            var lineSeries = new LineSeries(points);

            var result = lineSeries.Interpolate(new Double[] { 0, 5, 10, 15, 20, 25, 35 });

            var expectedResult = new List<Point>() { new Point(0,0), new Point(5,5), new Point(10,10), new Point(15,5), new Point(20, 0), new Point(25, -5), new Point(35,-5)};

            CollectionAssert.AreEqual(expectedResult, result);
        }
         
    }
}