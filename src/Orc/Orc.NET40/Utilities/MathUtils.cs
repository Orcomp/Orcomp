namespace Orc.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class MathUtils
    {
        // Code from http://stackoverflow.com/questions/3141692/c-sharp-standard-deviation-of-generic-list
        public static double StandardDeviation(this IEnumerable<double> values)
        {
            double avg = values.Average();
            return Math.Sqrt(values.Average(v => Math.Pow(v - avg, 2)));
        }

        public static IEnumerable<double> FilterData(IEnumerable<double> values)
        {
            var numbers = values.ToList();

            double min = numbers.Min();
            double max = numbers.Max();

            numbers.Remove(min);
            numbers.Remove(max);

            return numbers;
        }
    }
}
