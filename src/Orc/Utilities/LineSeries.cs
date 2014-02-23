// // --------------------------------------------------------------------------------------------------------------------
// // <copyright file="IDateInterval.cs" company="ORC">
// //   MS-PL
// // </copyright>
// // <summary>
// //  
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------
namespace Orc.Utilities
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Windows;

    public class LineSeries
    {
        IEnumerable<Point> Points { get; set; }

        /// <summary>
        ///  Points must be sorted by their x value.
        /// </summary>
        /// <param name="points"></param>
        public LineSeries(IEnumerable<Point> points )
        {
            Debug.Assert(points.Count() >= 2);

            Points = points;
        }

        /// <summary>
        /// Assume xValues to be sorted
        /// </summary>
        /// <param name="xValues"></param>
        /// <returns></returns>
        public IEnumerable<Point> Interpolate(IEnumerable<double> xValues)
        {
            Debug.Assert(Points.First().X >= xValues.First());
            Debug.Assert(Points.Last().X <= xValues.Last());

            throw new NotImplementedException();
        }
    }
}