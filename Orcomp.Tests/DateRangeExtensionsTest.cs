using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Orcomp.Entities;
using Orcomp.Extensions;
using Orcomp.Interfaces;

using Assert = Xunit.Assert;

namespace Orcomp.Tests
{
    [TestClass]
    public class DateRangeExtensionsTest
    {
        #region Simple Fixed Start

        [TestMethod]
        public void AccountForEfficiencies_EmptyListOfDateRangeEfficiencies_ReturnsUnchangedDateRange()
        {
            // Arrange
            var now = DateTime.Now;
            var dateRange = new DateRange( now, now.AddDays( 1 ) );

            var dateRangeEfficiencies = new List<DateRangeEfficiency>();

            // Act
            var newDateRange = dateRange.AccountForEfficiencies( dateRangeEfficiencies );

            // Assert
            Assert.Equal( dateRange, newDateRange );
        }

        [TestMethod]
        public void AccountForEfficiencies_OneZeroEfficiencyCalendarStartAndEndWithDateRange_ReturnsDateRangeOfSameDurationWhichStartsWhenTheEfficiencyFinishes()
        {
            // +-----------+ 
            // |-----0-----| 

            // Result:
            // +-----------------------+ 
            // |-----0-----|
             

            // Arrange
            var now = DateTime.Now;
            var dateRange = new DateRange(now, now.AddDays(1));
              
            var dateRangeEfficiencies = new List<DateRangeEfficiency>();
            var efficiency1 = new DateRangeEfficiency( dateRange.StartTime, dateRange.EndTime, 0 );
            dateRangeEfficiencies.Add(efficiency1);

            // Act
            var newDateRange = dateRange.AccountForEfficiencies(dateRangeEfficiencies);

            var correctDateRange = new DateRange( dateRange.StartTime, efficiency1.EndTime.Add(dateRange.Duration) );

            // Assert
            Assert.Equal(correctDateRange, newDateRange);
        }

        [TestMethod]
        public void AccountForEfficiencies_OneZeroEfficiencyCalendarStartAndEndsBeforeDateRangeStarts_ReturnsSameDateRange()
        {
            //             +-----------+ 
            // |-----0-----| 

            // Result:
            //             +-----------+  
            // |-----0-----|
            

            // Arrange
            var now = DateTime.Now;
            var dateRange = new DateRange(now, now.AddDays(1));

            var dateRangeEfficiencies = new List<DateRangeEfficiency>();
            dateRangeEfficiencies.Add(new DateRangeEfficiency(now.AddDays( -4 ), now, 0));

            // Act
            var newDateRange = dateRange.AccountForEfficiencies(dateRangeEfficiencies);

            var correctDateRange = dateRange;

            // Assert
            Assert.Equal(correctDateRange, newDateRange);
        }

        [TestMethod]
        public void AccountForEfficiencies_OneZeroEfficiencyCalendarStartAndEndsAfterDateRangeEnds_ReturnsSameDateRange()
        {
            // +-----------+ 
            //             |-----0-----| 

            // Result:
            // +-----------+ 
            //             |-----0-----|  

            // Arrange
            var now = DateTime.Now;
            var dateRange = new DateRange(now, now.AddDays(1));

            var dateRangeEfficiencies = new List<DateRangeEfficiency>();
            dateRangeEfficiencies.Add(new DateRangeEfficiency(now.AddDays(1), now.AddDays(3), 0));

            // Act
            var newDateRange = dateRange.AccountForEfficiencies(dateRangeEfficiencies);

            var correctDateRange = dateRange;

            // Assert
            Assert.Equal(correctDateRange, newDateRange);
        }

        [TestMethod]
        public void AccountForEfficiencies_OneZeroEfficiencyCalendarStartAndEndsWithinDateRange_ReturnsCorrectResult()
        {  
            // +-----------+ 
            //    |--0--| 

            // Result:
            // +-----------------+
            //    |--0--|  

            // Arrange
            var now = DateTime.Now;
            var dateRange = new DateRange(now, now.AddDays(4));

            var dateRangeEfficiencies = new List<DateRangeEfficiency>();
            var efficiency1 = new DateRangeEfficiency( now.AddDays( 1 ), now.AddDays( 2 ), 0 );
            dateRangeEfficiencies.Add(efficiency1);

            // Act
            var newDateRange = dateRange.AccountForEfficiencies(dateRangeEfficiencies);

            var correctDateRange = new DateRange( dateRange.StartTime, dateRange.Duration.Add(efficiency1.Duration) );

            // Assert
            Assert.Equal(correctDateRange, newDateRange);
        }

        [TestMethod]
        public void AccountForEfficiencies_OneZeroEfficiencyCalendarStartBeforeAndEndsWithinDateRange_ReturnsCorrectResult()
        {
            //       +-----------+ 
            //    |--0--| 

            // Result:
            //       +--------------+ 
            //    |--0--|  

            // Arrange
            var now = DateTime.Now;
            var dateRange = new DateRange(now, now.AddDays(4));

            var dateRangeEfficiencies = new List<DateRangeEfficiency>();
            var efficiency1 = new DateRangeEfficiency(now.AddDays(-1), now.AddDays(2), 0);
            dateRangeEfficiencies.Add(efficiency1);

            // Act
            var newDateRange = dateRange.AccountForEfficiencies(dateRangeEfficiencies);

            var correctDateRange = new DateRange(dateRange.StartTime, dateRange.Duration.Add( efficiency1.EndTime - dateRange.StartTime ));

            // Assert
            Assert.Equal(correctDateRange, newDateRange);
        }

        [TestMethod]
        public void AccountForEfficiencies_OneZeroEfficiencyCalendarStartsInAndEndsAfterDateRange_ReturnsCorrectResult()
        {
            // +-----------+ 
            //          |--0--| 

            // Result:
            // +-----------------+ 
            //          |--0--|  

            // Arrange
            var now = DateTime.Now;
            var dateRange = new DateRange(now, now.AddDays(4));

            var dateRangeEfficiencies = new List<DateRangeEfficiency>();
            var efficiency1 = new DateRangeEfficiency(now.AddDays(3), now.AddDays(5), 0);
            dateRangeEfficiencies.Add(efficiency1);

            // Act
            var newDateRange = dateRange.AccountForEfficiencies(dateRangeEfficiencies);

            var correctDateRange = new DateRange(dateRange.StartTime, dateRange.Duration.Add( efficiency1.Duration ));

            // Assert
            Assert.Equal(correctDateRange, newDateRange);
        }

        #endregion

        #region Simple Fixed End

        [TestMethod]
        public void AccountForEfficiencies_FixedEnd_EmptyListOfDateRangeEfficiencies_ReturnsUnchangedDateRange()
        {
            // Arrange
            var now = DateTime.Now;
            var dateRange = new DateRange(now, now.AddDays(1));

            var dateRangeEfficiencies = new List<DateRangeEfficiency>();

            // Act
            var newDateRange = dateRange.AccountForEfficiencies(dateRangeEfficiencies, FixedEdge.End);

            // Assert
            Assert.Equal(dateRange, newDateRange);
        }

        [TestMethod]
        public void AccountForEfficiencies_FixedEnd_OneZeroEfficiencyCalendarStartAndEndWithDateRange_ReturnsDateRangeOfSameDurationWhichStartsWhenTheEfficiencyFinishes()
        {
            // +-----------+ 
            // |-----0-----| 

            // Result:
            // +-----------------------+
            //             |-----0-----|
              

            // Arrange
            var now = DateTime.Now;
            var dateRange = new DateRange(now, now.AddDays(1));

            var dateRangeEfficiencies = new List<DateRangeEfficiency>();
            var efficiency1 = new DateRangeEfficiency( dateRange.StartTime, dateRange.EndTime, 0 );
            dateRangeEfficiencies.Add(efficiency1);

            // Act
            var newDateRange = dateRange.AccountForEfficiencies(dateRangeEfficiencies, FixedEdge.End);

            var correctDateRange = new DateRange(dateRange.StartTime.Add( - efficiency1.Duration ), dateRange.EndTime);

            // Assert
            Assert.Equal(correctDateRange, newDateRange);
        }

        [TestMethod]
        public void AccountForEfficiencies_FixedEnd_OneZeroEfficiencyCalendarStartAndEndsBeforeDateRangeStarts_ReturnsSameDateRange()
        {
            //             +-----------+ 
            // |-----0-----| 

            // Result:
            //             +-----------+  
            // |-----0-----|
            

            // Arrange
            var now = DateTime.Now;
            var dateRange = new DateRange(now, now.AddDays(1));

            var dateRangeEfficiencies = new List<DateRangeEfficiency>();
            dateRangeEfficiencies.Add(new DateRangeEfficiency(now.AddDays(-4), now, 0));

            // Act
            var newDateRange = dateRange.AccountForEfficiencies(dateRangeEfficiencies, FixedEdge.End);

            var correctDateRange = dateRange;

            // Assert
            Assert.Equal(correctDateRange, newDateRange);
        }

        [TestMethod]
        public void AccountForEfficiencies_FixedEnd_OneZeroEfficiencyCalendarStartAndEndsAfterDateRangeEnds_ReturnsSameDateRange()
        {
            // +-----------+ 
            //             |-----0-----| 

            // Result:
            // +-----------+ 
            //             |-----0-----|  

            // Arrange
            var now = DateTime.Now;
            var dateRange = new DateRange(now, now.AddDays(1));

            var dateRangeEfficiencies = new List<DateRangeEfficiency>();
            dateRangeEfficiencies.Add(new DateRangeEfficiency(now.AddDays(1), now.AddDays(3), 0));

            // Act
            var newDateRange = dateRange.AccountForEfficiencies(dateRangeEfficiencies, FixedEdge.End);

            var correctDateRange = dateRange;

            // Assert
            Assert.Equal(correctDateRange, newDateRange);
        }

        [TestMethod]
        public void AccountForEfficiencies_FixedEnd_OneZeroEfficiencyCalendarStartAndEndsWithinDateRange_ReturnsCorrectResult()
        {
            //      +-----------+ 
            //         |--0--| 

            // Result:
            //  +---------------+ 
            //         |--0--| 

            // Arrange
            var now = DateTime.Now;
            var dateRange = new DateRange(now, now.AddDays(4));

            var dateRangeEfficiencies = new List<DateRangeEfficiency>();
            var efficiency1 = new DateRangeEfficiency(now.AddDays(1), now.AddDays(2), 0);
            dateRangeEfficiencies.Add(efficiency1);

            // Act
            var newDateRange = dateRange.AccountForEfficiencies(dateRangeEfficiencies, FixedEdge.End);

            var correctDateRange = new DateRange(dateRange.StartTime.Add( -efficiency1.Duration ), dateRange.EndTime);

            // Assert
            Assert.Equal(correctDateRange, newDateRange);
        }

        [TestMethod]
        public void AccountForEfficiencies_FixedEnd_OneZeroEfficiencyCalendarStartBeforeAndEndsWithinDateRange_ReturnsCorrectResult()
        {
            //       +-----------+ 
            //    |--0--| 

            // Result:
            // +-----------------+ 
            //    |--0--|  

            // Arrange
            var now = DateTime.Now;
            var dateRange = new DateRange(now, now.AddDays(4));

            var dateRangeEfficiencies = new List<DateRangeEfficiency>();
            var efficiency1 = new DateRangeEfficiency(now.AddDays(-1), now.AddDays(2), 0);
            dateRangeEfficiencies.Add(efficiency1);

            // Act
            var newDateRange = dateRange.AccountForEfficiencies(dateRangeEfficiencies, FixedEdge.End);

            var correctDateRange = new DateRange(efficiency1.StartTime.Add( - (efficiency1.EndTime - dateRange.StartTime) ), dateRange.EndTime);

            // Assert
            Assert.Equal(correctDateRange, newDateRange);
        }

        [TestMethod]
        public void AccountForEfficiencies_FixedEnd_OneZeroEfficiencyCalendarStartsInAndEndsAfterDateRange_ReturnsCorrectResult()
        {
            //      +-----------+ 
            //               |--0--| 

            // Result:
            //  +---------------+ 
            //               |--0--|

            // Arrange
            var now = DateTime.Now;
            var dateRange = new DateRange(now, now.AddDays(4));

            var dateRangeEfficiencies = new List<DateRangeEfficiency>();
            var efficiency1 = new DateRangeEfficiency(now.AddDays(3), now.AddDays(5), 0);
            dateRangeEfficiencies.Add(efficiency1);

            // Act
            var newDateRange = dateRange.AccountForEfficiencies(dateRangeEfficiencies, FixedEdge.End);

            var correctDateRange = new DateRange(dateRange.StartTime.Add( - (dateRange.EndTime - efficiency1.StartTime) ), dateRange.EndTime);

            // Assert
            Assert.Equal(correctDateRange, newDateRange);
        }

        #endregion

        #region Fixed start

        [TestMethod]
        public void AccountForEfficiencies_MultipleDateRangeEfficiencies_ReturnsCorrectAnswer()
        {
            // +------------+ 
            //    |0|  |0|  |0|

            // Result:
            // +-------------------+ 
            //    |0|  |0|  |0|

            // Arrange
            var now = DateTime.Now;
            var dateRange = new DateRange(now, now.AddDays(5));

            var dateRangeEfficiencies = new List<DateRangeEfficiency>();
            var efficiency1 = new DateRangeEfficiency(now.AddDays(1), now.AddDays(2), 0);
            var efficiency2 = new DateRangeEfficiency(now.AddDays(3), now.AddDays(4), 0);
            var efficiency3 = new DateRangeEfficiency(now.AddDays(5), now.AddDays(6), 0);

            dateRangeEfficiencies.Add(efficiency1);
            dateRangeEfficiencies.Add(efficiency2);
            dateRangeEfficiencies.Add(efficiency3);


            // Act
            var newDateRange = dateRange.AccountForEfficiencies(dateRangeEfficiencies);

            var correctDateRange = new DateRange(dateRange.StartTime, dateRange.Duration.Add( efficiency1.Duration ).Add( efficiency2.Duration ).Add( efficiency3.Duration ));

            // Assert
            Assert.Equal(correctDateRange, newDateRange);
        }

        [TestMethod]
        public void AccountForEfficiencies_OneEfficiencyCalendarStartAndEndWithDateRange_ReturnsCorrectAnswer()
        {
            // +-----------+ 
            // |----200----| 

            // Result:
            // +-----+ 
            // |----200----|  

            // Arrange
            var now = DateTime.Now;
            var dateRange = new DateRange(now, now.AddDays(1));

            var dateRangeEfficiencies = new List<DateRangeEfficiency>();
            var efficiency1 = new DateRangeEfficiency(dateRange.StartTime, dateRange.EndTime, 200);
            dateRangeEfficiencies.Add(efficiency1);

            // Act
            var newDateRange = dateRange.AccountForEfficiencies(dateRangeEfficiencies);

            var correctDateRange = new DateRange(dateRange.StartTime, dateRange.StartTime.AddTicks( dateRange.Duration.Ticks / 2 ));

            // Assert
            Assert.Equal(correctDateRange, newDateRange);
        }

        #endregion

        #region Fixed End

        [TestMethod]
        public void AccountForEfficiencies_FixedEnd_MultipleDateRangeEfficiencies_ReturnsCorrectAnswer()
        {
            //         +------------+ 
            //       |0|  |0|  |0|

            // Result:
            //   +------------------+ 
            //       |0|  |0|  |0|

            // Arrange
            var now = DateTime.Now;
            var dateRange = new DateRange(now, now.AddDays(5));

            var dateRangeEfficiencies = new List<DateRangeEfficiency>();
            var efficiency1 = new DateRangeEfficiency(now.AddDays(1), now.AddDays(2), 0);
            var efficiency2 = new DateRangeEfficiency(now.AddDays(3), now.AddDays(4), 0);
            var efficiency3 = new DateRangeEfficiency(now.AddDays(5), now.AddDays(6), 0);

            dateRangeEfficiencies.Add(efficiency1);
            dateRangeEfficiencies.Add(efficiency2);
            dateRangeEfficiencies.Add(efficiency3);


            // Act
            var newDateRange = dateRange.AccountForEfficiencies(dateRangeEfficiencies, FixedEdge.End);

            var correctDateRange = new DateRange(dateRange.EndTime.Add(-dateRange.Duration).Add(-efficiency1.Duration).Add(-efficiency2.Duration).Add(-efficiency3.Duration), dateRange.EndTime);

            // Assert
            Assert.Equal(correctDateRange, newDateRange);
        }

        [TestMethod]
        public void AccountForEfficiencies_FixedEnd_OneEfficiencyCalendarStartAndEndWithDateRange_ReturnsCorrectAnswer()
        {
            // +-----------+ 
            // |----200----| 

            // Result:
            //       +-----+ 
            // |----200----|  

            // Arrange
            var now = DateTime.Now;
            var dateRange = new DateRange(now, now.AddDays(1));

            var dateRangeEfficiencies = new List<DateRangeEfficiency>();
            var efficiency1 = new DateRangeEfficiency(dateRange.StartTime, dateRange.EndTime, 200);
            dateRangeEfficiencies.Add(efficiency1);

            // Act
            var newDateRange = dateRange.AccountForEfficiencies(dateRangeEfficiencies, FixedEdge.End);

            var correctDateRange = new DateRange(dateRange.EndTime.AddTicks( -dateRange.Duration.Ticks / 2), dateRange.EndTime);

            // Assert
            Assert.Equal(correctDateRange, newDateRange);
        }

        #endregion
    }
}
