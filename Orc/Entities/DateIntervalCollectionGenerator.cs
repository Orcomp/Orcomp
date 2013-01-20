namespace Orc.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    using Orc.Interface;

    public static class DateIntervalCollectionGenerator
    {
        /// <summary>
        /// |---duration ---| endToStartOffset |--------------|
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="duration"></param>
        /// <param name="endToStartOffset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static IEnumerable<DateInterval> NoOverlaps(
            DateTime startDate, TimeSpan duration, TimeSpan endToStartOffset, int count)
        {
            Debug.Assert(endToStartOffset.Ticks >= 0, "endToStartOffset must be greater than or equal to zero.");
            Debug.Assert(count > 0, "count must be greater than zero.");

            foreach (var number in Enumerable.Range(0, count - 1))
            {
                var start = startDate.Add(TimeSpan.FromTicks(duration.Add(endToStartOffset).Ticks * number));
                var end = start.Add(duration);
                yield return new DateInterval(start, end);
            }
        }

        /// <summary>
        /// |-------------------|
        ///     |-------------------|
        ///          |-------------------|
        ///                |-------------------|
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="duration"></param>
        /// <param name="startToStartOffset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static IEnumerable<DateInterval> OverlapsWithConstantDuration(
            DateTime startDate, TimeSpan duration, TimeSpan startToStartOffset, int count)
        {
            Debug.Assert(
                startToStartOffset.Ticks < duration.Ticks && startToStartOffset.Ticks >= 0,
                "startToStartOffset must be greater or equal to zero and smaller than duration.");
            Debug.Assert(count > 0, "count must be greater than zero.");

            foreach (var number in Enumerable.Range(0, count - 1))
            {
                var start = startDate.Add(TimeSpan.FromTicks(startToStartOffset.Ticks * number));
                var end = start.Add(duration);
                yield return new DateInterval(start, end);
            }
        }

        /// <summary>
        /// |-------------------|
        ///   |---------------|
        ///      |---------|
        ///        |-----|
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static IEnumerable<DateInterval> OverlapsWithDecreasingDuration(
            DateTime startDate, TimeSpan offset, int count)
        {
            Debug.Assert(offset.Ticks > 0, "offset must be greater than zero.");
            Debug.Assert(count > 0, "count must be greater than zero.");

            var endDate = startDate.AddTicks((count - 1) * offset.Ticks * 2);

            foreach (var number in Enumerable.Range(0, count - 1))
            {
                var start = startDate.AddTicks(offset.Ticks * number);
                var end = endDate.AddTicks(-offset.Ticks * number);

                yield return new DateInterval(start, end);
            }
        }

        /// <summary>
        /// |-------------------|
        /// |---------------|
        /// |----------|
        /// |-----|
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endOffset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static IEnumerable<DateInterval> OverlapsSameStartTimeDecreasingEndTime(
            DateTime startDate, TimeSpan endOffset, int count)
        {
            Debug.Assert(endOffset.Ticks > 0, "offset must be greater than zero.");
            Debug.Assert(count > 0, "count must be greater than zero.");

            var endDate = startDate.AddTicks((count - 1) * endOffset.Ticks);

            foreach (var number in Enumerable.Range(0, count - 1))
            {
                var end = endDate.AddTicks( - endOffset.Ticks * number);
                yield return new DateInterval(startDate, end);
            }
        }


        /// <summary>
        /// |-------------------|
        ///     |---------------|
        ///          |----------|
        ///               |-----|
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endOffset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static IEnumerable<DateInterval> OverlapsSameEndTimeDecreasingDuration(
            DateTime startDate, TimeSpan startOffset, int count)
        {
            Debug.Assert(startOffset.Ticks > 0, "offset must be greater than zero.");
            Debug.Assert(count > 0, "count must be greater than zero.");

            var endDate = startDate.AddTicks((count - 1) * startOffset.Ticks);

            foreach (var number in Enumerable.Range(0, count - 1))
            {
                var start = endDate.AddTicks(startOffset.Ticks * number);
                yield return new DateInterval(start, endDate);
            }
        }
    }
}