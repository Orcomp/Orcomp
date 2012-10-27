// // --------------------------------------------------------------------------------------------------------------------
// // <copyright file="DateIntervalCollectionTesBase.cs" company="ORC">
// //   MS-PL
// // </copyright>
// // <summary>
// //  The date interval collection tests base. Is used for common functionality.
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------
namespace Orc.Tests
{
    using System;

    using NUnit.Framework;

    using Orc.Entities;

    /// <summary>
    /// Common logic for date interval collection tests 
    /// </summary>
    public class DateIntervalCollectionTestBase
    {
        protected DateTime now;

        protected DateInterval nowAndTenDaysInterval;
        protected DateInterval nowAndFiveDaysInterval;
        protected DateInterval twoDaysAndFiveDaysInterval;
        protected DateInterval threeDaysAgoAndTwelveDaysInterval;
        protected DateInterval thirteenDaysAndFourteenDaysInterval;

        /// <summary>
        /// Setups tests common data.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            now = DateTime.Now;

            nowAndTenDaysInterval = new DateInterval(now, now.AddDays(10));
            nowAndFiveDaysInterval = new DateInterval(now, now.AddDays(5));
            twoDaysAndFiveDaysInterval = new DateInterval(now.AddDays(2), now.AddDays(5));
            threeDaysAgoAndTwelveDaysInterval = new DateInterval(now.AddDays(-3), now.AddDays(12));
            thirteenDaysAndFourteenDaysInterval = new DateInterval(now.AddDays(13), now.AddDays(14));
        }
    }
}