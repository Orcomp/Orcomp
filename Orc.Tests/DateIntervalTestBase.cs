// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DateIntervalTestBase.cs" company="ORC">
//   MS-PL
// </copyright>
// <summary>
//   The date interval tests base. Is used for common functionality.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Orc.Tests
{
    using System;

    using NUnit.Framework;

    /// <summary>
    /// Common logic for date interval tests 
    /// </summary>
    public class DateIntervalTestBase
    {
        protected DateTime now;

        protected DateTime inOneHour;

        protected DateTime inTwoHours;

        protected DateTime inThreeHours;

        /// <summary>
        /// Setups tests common data.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            now = DateTime.Now;
            inOneHour = now.AddHours(1);
            inTwoHours = now.AddHours(2);
            inThreeHours = now.AddHours(3);
        }
    }
}
