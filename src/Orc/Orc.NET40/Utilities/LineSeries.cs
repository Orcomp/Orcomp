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
        public IEnumerable<Point> Points { get; private set; }

        protected Point[] PointsArray;
        protected List<Point> PointsList;
        protected IList<Point> PointsIndexed;

        /// <summary>
        ///  Points must be sorted by their x value.
        /// </summary>
        /// <param name="points"></param>
        public LineSeries(IEnumerable<Point> points)
        {
            Debug.Assert(points.Count() >= 2);
            
            Points = points;

            PointsArray = points as Point[];
            PointsList = points as List<Point>;
            PointsIndexed = points as IList<Point>;
        }

        /// <summary>
        /// Assume xValues to be sorted
        /// </summary>
        /// <param name="xValues"></param>
        /// <returns></returns>
        public IEnumerable<Point> Interpolate(IEnumerable<double> xValues, SearchType search_type = SearchType.Best)
        {
            switch (search_type)
            {
                case SearchType.Best:
                    if (PointsArray != null)
                        return InterpolateBinary(xValues, SearchArray);
                    else if (PointsList != null)
                        return InterpolateBinary(xValues, SearchList);
                    else
                        return InterpolateLinear(xValues);

                case SearchType.Binary:
                    if (PointsArray != null)
                        return InterpolateBinary(xValues, SearchArray);
                    else if (PointsList != null)
                        return InterpolateBinary(xValues, SearchList);
                    else
                        throw new ArgumentException();
                
                case SearchType.Linear:
                    return InterpolateLinear(xValues);
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private int SearchArray(double value, int start_index = 0)
        {
            var value_point = new Point(value, 0.0);
            return SearchArray(value_point, start_index);
        }

        private int SearchArray(Point value, int start_index)
        {
            return Array.BinarySearch<Point>(PointsArray, start_index, PointsArray.Length - start_index, value, PointXComparer.Instance);
        }

        private int SearchList(double value, int start_index = 0)
        {
            var value_point = new Point(value, 0.0);
            return SearchList(value_point, start_index);
        }

        private int SearchList(Point value, int start_index)
        {
            return PointsList.BinarySearch(start_index, PointsList.Count - start_index, value, PointXComparer.Instance);
        }

        private IEnumerable<Point> InterpolateBinary(IEnumerable<double> values, Func<double, int, int> search_func)
        {
            var iter_values = values.GetEnumerator();
            var prev_value_x = Double.MinValue;
            var curr_value_X = Double.MaxValue;

            var points_index = 0;

            // TODO - do we want custom exceptions for too small collections?

            while (iter_values.MoveNext())
            {
                curr_value_X = iter_values.Current;

                if (curr_value_X < prev_value_x)
                {
                    throw new ArgumentOutOfRangeException();
                }

                points_index = search_func(curr_value_X, points_index);

                if (points_index >= 0)
                {
                    yield return PointsIndexed[points_index];
                }
                else
                {
                    points_index = ~points_index - 1;
                    var prev_point = PointsIndexed[points_index];
                    var next_point = PointsIndexed[points_index + 1];
                    yield return new Point(curr_value_X,
                                           prev_point.Y + (next_point.Y - prev_point.Y)
                                                        * (curr_value_X - prev_point.X)
                                                        / (next_point.X - prev_point.X));
                }

                prev_value_x = iter_values.Current;
            }
        }

        private IEnumerable<Point> InterpolateLinear(IEnumerable<double> values)
        {
            var iter_values = values.GetEnumerator();
            var prev_value_x = Double.MinValue;
            var curr_value_X = Double.MaxValue;

            var iter_points = Points.GetEnumerator();
            var prev_point = new Point();
            var next_point = new Point();

            // TODO - do we want custom exceptions for too small collections?

            iter_points.MoveNext();
            prev_point = iter_points.Current;

            iter_points.MoveNext();
            next_point = iter_points.Current;

            while (iter_values.MoveNext())
            {
                curr_value_X = iter_values.Current;

                if (curr_value_X < prev_value_x)
                {
                    throw new ArgumentOutOfRangeException();
                }

                while (next_point.X < curr_value_X)
                {
                    prev_point = iter_points.Current;

                    if (iter_points.MoveNext())
                    {
                        next_point = iter_points.Current;
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException();
                    }
                }

                yield return new Point(curr_value_X,
                                       prev_point.Y + (next_point.Y - prev_point.Y)
                                                    * (curr_value_X - prev_point.X)
                                                    / (next_point.X - prev_point.X));

                prev_value_x = iter_values.Current;
            }
        }

        public enum SearchType
        {
            Best,
            Binary,
            Linear,
        }
    }

    public sealed class PointXComparer : IComparer<Point>
    {
        public static readonly PointXComparer Instance = new PointXComparer();

        private static Comparer<double> double_cmp = Comparer<double>.Default;

        public int Compare(Point x, Point y)
        {
 	        return double_cmp.Compare(x.X, y.X);
        }
    }
}