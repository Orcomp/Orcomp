// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DateIntervalExtensionsTest.cs" company="ORC">
//   MS-PL
// </copyright>
// <summary>
//   The date interval extensions test.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Orc.Tests
{
    using System;

    using NUnit.Framework;

    using Orc.Entities;
    using Orc.Extensions;

    /// <summary>
    /// The date interval extensions test.
    /// </summary>
    [TestFixture]
    public class DateIntervalExtensionsTest
    {
        #region GetOverlap

        /// <summary>
        /// The get overlap_ date interval before date interval_ return null.
        /// </summary>
        [Test]
        public void GetOverlap_DateIntervalBeforeDateInterval_ReturnNull()
        {
            // Arrange
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(1);
            DateTime t3 = t1.AddHours(2);
            DateTime t4 = t1.AddHours(3);
            var dateInterval = new DateInterval(t3, t4);
            var beforeDateInterval = new DateInterval(t1, t2);

            // Act
            DateInterval result = dateInterval.GetOverlap(beforeDateInterval);

            // Assert
            Assert.Null(result);
        }

        /// <summary>
        /// The get overlap_ date interval after date interval_ return null.
        /// </summary>
        [Test]
        public void GetOverlap_DateIntervalAfterDateInterval_ReturnNull()
        {
            // Arrange
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(1);
            DateTime t3 = t1.AddHours(2);
            DateTime t4 = t1.AddHours(3);
            var dateInterval = new DateInterval(t1, t2);
            var afterDateInterval = new DateInterval(t3, t4);

            // Act
            DateInterval result = dateInterval.GetOverlap(afterDateInterval);

            // Assert
            Assert.Null(result);
        }

        /// <summary>
        /// The get overlap_ date interval before date interval interesct_ return null.
        /// </summary>
        [Test]
        public void GetOverlap_DateIntervalBeforeDateIntervalInteresct_ReturnNull()
        {
            // Arrange
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(1);
            DateTime t3 = t1.AddHours(2);
            var dateInterval = new DateInterval(t2, t3);
            var beforeDateInterval = new DateInterval(t1, t2);

            // Act
            DateInterval result = dateInterval.GetOverlap(beforeDateInterval);

            // Assert
            Assert.Null(result);
        }

        /// <summary>
        /// The get overlap_ date interval after date interval interect_ return null.
        /// </summary>
        [Test]
        public void GetOverlap_DateIntervalAfterDateIntervalInterect_ReturnNull()
        {
            // Arrange
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(1);
            DateTime t3 = t1.AddHours(2);
            var dateInterval = new DateInterval(t1, t2);
            var afterDateInterval = new DateInterval(t2, t3);

            // Act
            DateInterval result = dateInterval.GetOverlap(afterDateInterval);

            // Assert
            Assert.Null(result);
        }

        /// <summary>
        /// The get overlap_ date interval start before end in_ return correct date interval.
        /// </summary>
        [Test]
        public void GetOverlap_DateIntervalStartBeforeEndIn_ReturnCorrectDateInterval()
        {
            // Arrange
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(1);
            DateTime t3 = t1.AddHours(2);
            DateTime t4 = t1.AddHours(3);
            var dateInterval = new DateInterval(t2, t4);
            var startBeforeEndInDateInterval = new DateInterval(t1, t3);

            // Act
            DateInterval result = dateInterval.GetOverlap(startBeforeEndInDateInterval);
            DateInterval correctResult = new DateInterval(t2, t3);

            // Assert
            Assert.AreEqual(correctResult, result);
        }

        /// <summary>
        /// The get overlap_ date interval start before end after_ return correct date interval.
        /// </summary>
        [Test]
        public void GetOverlap_DateIntervalStartBeforeEndAfter_ReturnCorrectDateInterval()
        {
            // Arrange
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(1);
            DateTime t3 = t1.AddHours(2);
            DateTime t4 = t1.AddHours(3);
            var dateInterval = new DateInterval(t2, t3);
            var startBeforeEndAfterDateInterval = new DateInterval(t1, t4);

            // Act
            DateInterval result = dateInterval.GetOverlap(startBeforeEndAfterDateInterval);
            DateInterval correctResult = new DateInterval(t2, t3);

            // Assert
            Assert.AreEqual(correctResult, result);
        }

        /// <summary>
        /// The get overlap_ date interval start in end in_ return correct date interval.
        /// </summary>
        [Test]
        public void GetOverlap_DateIntervalStartInEndIn_ReturnCorrectDateInterval()
        {
            // Arrange
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(1);
            DateTime t3 = t1.AddHours(2);
            DateTime t4 = t1.AddHours(3);
            var dateInterval = new DateInterval(t1, t4);
            var startInEndInDateInterval = new DateInterval(t2, t3);

            // Act
            DateInterval result = dateInterval.GetOverlap(startInEndInDateInterval);
            DateInterval correctResult = new DateInterval(t2, t3);

            // Assert
            Assert.AreEqual(correctResult, result);
        }

        /// <summary>
        /// The get overlap_ date interval start interesct end intersect_ return correct date interval.
        /// </summary>
        [Test]
        public void GetOverlap_DateIntervalStartInteresctEndIntersect_ReturnCorrectDateInterval()
        {
            // Arrange
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(1);
            var dateInterval = new DateInterval(t1, t2);
            var startInteresctEndInteresctDateInterval = new DateInterval(t1, t2);

            // Act
            DateInterval result = dateInterval.GetOverlap(startInteresctEndInteresctDateInterval);
            DateInterval correctResult = new DateInterval(t1, t2);

            // Assert
            Assert.AreEqual(correctResult, result);
        }

        /// <summary>
        /// The get overlap_ date interval start in end after_ return correct date interval.
        /// </summary>
        [Test]
        public void GetOverlap_DateIntervalStartInEndAfter_ReturnCorrectDateInterval()
        {
            // Arrange
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(1);
            DateTime t3 = t1.AddHours(2);
            DateTime t4 = t1.AddHours(3);
            var dateInterval = new DateInterval(t1, t3);
            var startInEndAfterDateInterval = new DateInterval(t2, t4);

            // Act
            DateInterval result = dateInterval.GetOverlap(startInEndAfterDateInterval);
            DateInterval correctResult = new DateInterval(t2, t3);

            // Assert
            Assert.AreEqual(correctResult, result);
        }

        #endregion
    }
}