using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Orcomp.Entities;

using Assert = Xunit.Assert;

namespace Orcomp.Tests
{
    [TestClass]
    public class DateRangeTest
    {
        #region Intersect

        [TestMethod]
        public void Intersect_DateBeforeDateRange_ReturnFalse()
        {
            // Arrange
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours( 1 );
            var dateRange = new DateRange( t1, t2 );

            // Act
            bool intersect = dateRange.Intersect( t1.AddHours( -1 ) );

            // Assert
            Assert.False(intersect);
        }

        [TestMethod]
        public void Intersect_DateAfterDateRange_ReturnFalse()
        {
            // Arrange
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(1);
            var dateRange = new DateRange(t1, t2);

            // Act
            bool interesct = dateRange.Intersect( t1.AddHours( 2 ) );

            // Assert
            Assert.False(interesct);
        }

        [TestMethod]
        public void Intersect_DateInDateRange_ReturnTrue()
        {
            // Arrange
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(1);
            var dateRange = new DateRange(t1, t2);

            // Act
            bool interesct = dateRange.Intersect( t1.AddHours( 0.5 ) );

            // Assert
            Assert.True(interesct);
        }

        [TestMethod]
        public void Intersect_DateOnStartDateRange_ReturnTrue()
        {
            // Arrange
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(1);
            var dateRange = new DateRange(t1, t2);

            // Act
            bool interesct = dateRange.Intersect(t1);

            // Assert
            Assert.True(interesct);
        }

        [TestMethod]
        public void Intersect_DateOnEndDateRange_ReturnTrue()
        {
            // Arrange
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(1);
            var dateRange = new DateRange(t1, t2);

            // Act
            bool interesct = dateRange.Intersect(t2);

            // Assert
            Assert.True(interesct);
        }

        #endregion

        #region GetOverlap

        [TestMethod]
        public void Overlap_DateRangeBeforeDateRange_ReturnNull()
        {
            // Arrange
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(1);
            DateTime t3 = t1.AddHours( 2 );
            DateTime t4 = t1.AddHours( 3 );
            var dateRange = new DateRange(t3, t4);
            var beforeDateRange = new DateRange(t1, t2); 

            // Act
            DateRange result = dateRange.GetOverlap( beforeDateRange );

            // Assert
            Assert.Null( result );
        }

        [TestMethod]
        public void Overlap_DateRangeAfterDateRange_ReturnNull()
        {
            // Arrange
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(1);
            DateTime t3 = t1.AddHours(2);
            DateTime t4 = t1.AddHours(3);
            var dateRange = new DateRange(t1, t2);
            var afterDateRange = new DateRange(t3, t4);

            // Act
            DateRange result = dateRange.GetOverlap(afterDateRange);

            // Assert
            Assert.Null(result);
        }

        [TestMethod]
        public void Overlap_DateRangeBeforeDateRangeInteresct_ReturnNull()
        {
            // Arrange
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(1);
            DateTime t3 = t1.AddHours(2);
            var dateRange = new DateRange(t2, t3);
            var beforeDateRange = new DateRange(t1, t2);

            // Act
            DateRange result = dateRange.GetOverlap(beforeDateRange);

            // Assert
            Assert.Null(result);
        }

        [TestMethod]
        public void Overlap_DateRangeAfterDateRangeInterect_ReturnNull()
        {
            // Arrange
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(1);
            DateTime t3 = t1.AddHours(2);
            var dateRange = new DateRange(t1, t2);
            var afterDateRange = new DateRange(t2, t3);

            // Act
            DateRange result = dateRange.GetOverlap(afterDateRange);

            // Assert
            Assert.Null(result);
        }

        [TestMethod]
        public void Overlap_DateRangeStartBeforeEndIn_ReturnCorrectDateRange()
        {
            // Arrange
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(1);
            DateTime t3 = t1.AddHours(2);
            DateTime t4 = t1.AddHours(3);
            var dateRange = new DateRange(t2, t4);
            var startBeforeEndInDateRange = new DateRange(t1, t3);

            // Act
            DateRange result = dateRange.GetOverlap(startBeforeEndInDateRange);
            DateRange correctResult = new DateRange( t2, t3 );

            // Assert
            Assert.Equal(correctResult, result);
        }

        [TestMethod]
        public void Overlap_DateRangeStartBeforeEndAfter_ReturnCorrectDateRange()
        {
            // Arrange
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(1);
            DateTime t3 = t1.AddHours(2);
            DateTime t4 = t1.AddHours(3);
            var dateRange = new DateRange(t2, t3);
            var startBeforeEndAfterDateRange = new DateRange(t1, t4);

            // Act
            DateRange result = dateRange.GetOverlap(startBeforeEndAfterDateRange);
            DateRange correctResult = new DateRange(t2, t3);

            // Assert
            Assert.Equal(correctResult, result);
        }

        [TestMethod]
        public void Overlap_DateRangeStartInEndIn_ReturnCorrectDateRange()
        {
            // Arrange
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(1);
            DateTime t3 = t1.AddHours(2);
            DateTime t4 = t1.AddHours(3);
            var dateRange = new DateRange(t1, t4);
            var startInEndInDateRange = new DateRange(t2, t3);

            // Act
            DateRange result = dateRange.GetOverlap(startInEndInDateRange);
            DateRange correctResult = new DateRange(t2, t3);

            // Assert
            Assert.Equal(correctResult, result);
        }

        [TestMethod]
        public void Overlap_DateRangeStartInteresctEndIntersect_ReturnCorrectDateRange()
        {
            // Arrange
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(1);
            var dateRange = new DateRange(t1, t2);
            var startInteresctEndInteresctDateRange = new DateRange(t1, t2);

            // Act
            DateRange result = dateRange.GetOverlap(startInteresctEndInteresctDateRange);
            DateRange correctResult = new DateRange(t1, t2);

            // Assert
            Assert.Equal(correctResult, result);
        }

        [TestMethod]
        public void Overlap_DateRangeStartInEndAfter_ReturnCorrectDateRange()
        {
            // Arrange
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(1);
            DateTime t3 = t1.AddHours(2);
            DateTime t4 = t1.AddHours(3);
            var dateRange = new DateRange(t1, t3);
            var startInEndAfterDateRange = new DateRange(t2, t4);

            // Act
            DateRange result = dateRange.GetOverlap(startInEndAfterDateRange);
            DateRange correctResult = new DateRange(t2, t3);

            // Assert
            Assert.Equal(correctResult, result);
        }

        #endregion

        #region Equality

        [TestMethod]
        public void Equal_DateRangeWithSameStartAndEndDates_ReturnTrue()
        {
            // Arrange
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(1);
            var dateRange1 = new DateRange(t1, t2);
            var dateRange2 = new DateRange(t1, t2);

            // Act
            bool result = dateRange1.Equals( dateRange2 );

            // Assert
            Assert.True(result);
        }

        [TestMethod]
        public void Equal_DateRangeWithSameStartAndDifferentEndDates_ReturnFalse()
        {
            // Arrange
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(1);
            DateTime t3 = t1.AddHours(3);
            var dateRange1 = new DateRange(t1, t2);
            var dateRange2 = new DateRange(t1, t3);

            // Act
            bool result = dateRange1.Equals(dateRange2);

            // Assert
            Assert.False(result);
        }

        [TestMethod]
        public void Equal_DateRangeWithDifferentStartAndSameEndDates_ReturnFalse()
        {
            // Arrange
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(1);
            DateTime t3 = t1.AddHours(3);
            var dateRange1 = new DateRange(t1, t3);
            var dateRange2 = new DateRange(t2, t3);

            // Act
            bool result = dateRange1.Equals(dateRange2);

            // Assert
            Assert.False(result);
        }

        #endregion

        #region compareTo

        [TestMethod]
        public void CompareTo_DateRangeWithSameStartAndSameEndDates_ReturnsZero()
        {
            // Arrange
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(1);
            var dateRange1 = new DateRange(t1, t2);
            var dateRange2 = new DateRange(t1, t2);

            // Act
            int value = dateRange1.CompareTo(dateRange2); // Shortest duration comes first

            // Assert
            Assert.True(value == 0);
        }

        [TestMethod]
        public void CompareTo_DateRangeWithSameStartAndDifferentEndDates_ReturnsShortestDurationFirst()
        {
            // Arrange
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(1);
            DateTime t3 = t1.AddHours(3);
            var dateRange1 = new DateRange(t1, t2);
            var dateRange2 = new DateRange(t1, t3);

            // Act
            int value = dateRange1.CompareTo( dateRange2 ); // Shortest duration comes first

            // Assert
            Assert.True( value == -1  );
        }

        [TestMethod]
        public void CompareTo_DateRangeWithSameStartAndDifferentEndDates_ReturnsShortestDurationFirstReversed()
        {
            // Arrange
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(1);
            DateTime t3 = t1.AddHours(3);
            var dateRange1 = new DateRange(t1, t3);
            var dateRange2 = new DateRange(t1, t2);

            // Act
            int value = dateRange1.CompareTo(dateRange2); // Shortest duration comes first

            // Assert
            Assert.True(value == 1);
        }

        [TestMethod]
        public void CompareTo_DateRangeWithDifferentStartAndDifferentEndDates_ReturnsEarliestStart()
        {
            // Arrange
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(1);
            DateTime t3 = t1.AddHours(3);
            var dateRange1 = new DateRange(t1, t2);
            var dateRange2 = new DateRange(t2, t3);

            // Act
            int value = dateRange1.CompareTo(dateRange2);

            // Assert   
            Assert.True(value == -1);
        }

        #endregion

        #region EqualOperation

        [TestMethod]
        public void CompareTo_DateRangeWithSameStartAndSameEndDates_ReturnsTrue()
        {
            // Arrange
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(1);
            var dateRange1 = new DateRange(t1, t2);
            var dateRange2 = new DateRange(t1, t2);

            // Act
            bool value = dateRange1 == dateRange2;

            // Assert   
            Assert.True(value);
        }

        #endregion
    }
}
