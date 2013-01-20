namespace Orc.Tests.DateIntervalExtensionsAccountForEfficiencies
{
    using System;
    using System.Collections.Generic;

    using NUnit.Framework;

    using Orc.Entities;
    // using Orc.Extensions;
    using Orc.Competitions.AccountForEfficiencies;

    [TestFixture]
    public class AccountForEfficienciesTest
    {
        #region Simple Fixed Start

        [Test]
        public void AccountForEfficiencies_EmptyListOfDateIntervalEfficiencies_ReturnsUnchangedDateInterval()
        {
            // Arrange
            var now = DateTime.Now;
            var dateInterval = new DateInterval( now, now.AddDays( 1 ) );

            var dateIntervalEfficiencies = new List<DateIntervalEfficiency>();

            // Act
            var newDateInterval = dateInterval.AccountForEfficiencies( dateIntervalEfficiencies );

            // Assert
            Assert.AreEqual( dateInterval, newDateInterval );
        }

        [Test]
        public void AccountForEfficiencies_OneZeroEfficiencyCalendarStartsAndEndsWithDateInterval_ReturnsCorrectDateInterval()
        {
            // +-----------+ 
            // |-----0-----| 

            // Result:
            // +-----------------------+ 
            // |-----0-----|
             

            // Arrange
            var now = DateTime.Now;
            var dateInterval = new DateInterval(now, now.AddDays(1));
              
            var dateIntervalEfficiencies = new List<DateIntervalEfficiency>();
            var efficiency1 = new DateIntervalEfficiency( dateInterval, 0 );
            dateIntervalEfficiencies.Add(efficiency1);

            // Act
            var newDateInterval = dateInterval.AccountForEfficiencies(dateIntervalEfficiencies);

            var correctDateInterval = new DateInterval( dateInterval.Min.Value, efficiency1.Max.Value.Add(dateInterval.Duration) );

            // Assert
            Assert.AreEqual(correctDateInterval, newDateInterval);
        }

        [Test]
        public void AccountForEfficiencies_OneZeroEfficiencyCalendarEndsWhenDateIntervalStarts_ReturnsSameDateInterval()
        {
            //             +-----------+ 
            // |-----0-----| 

            // Result:
            //             +-----------+  
            // |-----0-----|
            

            // Arrange
            var now = DateTime.Now;
            var dateInterval = new DateInterval(now, now.AddDays(1));

            var dateIntervalEfficiencies = new List<DateIntervalEfficiency>();
            dateIntervalEfficiencies.Add(new DateIntervalEfficiency(now.AddDays( -4 ), now, 0));

            // Act
            var newDateInterval = dateInterval.AccountForEfficiencies(dateIntervalEfficiencies);

            var correctDateInterval = dateInterval;

            // Assert
            Assert.AreEqual(correctDateInterval, newDateInterval);
        }

        [Test]
        public void AccountForEfficiencies_OneZeroEfficiencyCalendarStartWhenDateIntervalEnds_ReturnsSameDateInterval()
        {
            // +-----------+ 
            //             |-----0-----| 

            // Result:
            // +-----------+ 
            //             |-----0-----|  

            // Arrange
            var now = DateTime.Now;
            var dateInterval = new DateInterval(now, now.AddDays(1));

            var dateIntervalEfficiencies = new List<DateIntervalEfficiency>();
            dateIntervalEfficiencies.Add(new DateIntervalEfficiency(now.AddDays(1), now.AddDays(3), 0));

            // Act
            var newDateInterval = dateInterval.AccountForEfficiencies(dateIntervalEfficiencies);

            var correctDateInterval = dateInterval;

            // Assert
            Assert.AreEqual(correctDateInterval, newDateInterval);
        }

        [Test]
        public void AccountForEfficiencies_OneZeroEfficiencyCalendarStartAndEndsWithinDateInterval_ReturnsCorrectResult()
        {  
            // +-----------+ 
            //    |--0--| 

            // Result:
            // +-----------------+
            //    |--0--|  

            // Arrange
            var now = DateTime.Now;
            var dateInterval = new DateInterval(now, now.AddDays(4));

            var dateIntervalEfficiencies = new List<DateIntervalEfficiency>();
            var efficiency1 = new DateIntervalEfficiency( now.AddDays( 1 ), now.AddDays( 2 ), 0 );
            dateIntervalEfficiencies.Add(efficiency1);

            // Act
            var newDateInterval = dateInterval.AccountForEfficiencies(dateIntervalEfficiencies);

            var correctDateInterval = new DateInterval( dateInterval.Min.Value, dateInterval.Duration.Add(efficiency1.Duration) );

            // Assert
            Assert.AreEqual(correctDateInterval, newDateInterval);
        }

        [Test]
        public void AccountForEfficiencies_OneZeroEfficiencyCalendarStartBeforeAndEndsWithinDateInterval_ReturnsCorrectResult()
        {
            //       +-----------+ 
            //    |--0--| 

            // Result:
            //       +--------------+ 
            //    |--0--|  

            // Arrange
            var now = DateTime.Now;
            var dateInterval = new DateInterval(now, now.AddDays(4));

            var dateIntervalEfficiencies = new List<DateIntervalEfficiency>();
            var efficiency1 = new DateIntervalEfficiency(now.AddDays(-1), now.AddDays(2), 0);
            dateIntervalEfficiencies.Add(efficiency1);

            // Act
            var newDateInterval = dateInterval.AccountForEfficiencies(dateIntervalEfficiencies);

            var correctDateInterval = new DateInterval(dateInterval.Min.Value, dateInterval.Duration.Add( efficiency1.Max.Value - dateInterval.Min.Value ));

            // Assert
            Assert.AreEqual(correctDateInterval, newDateInterval);
        }

        [Test]
        public void AccountForEfficiencies_OneZeroEfficiencyCalendarStartsInAndEndsAfterDateInterval_ReturnsCorrectResult()
        {
            // +-----------+ 
            //          |--0--| 

            // Result:
            // +-----------------+ 
            //          |--0--|  

            // Arrange
            var now = DateTime.Now;
            var dateInterval = new DateInterval(now, now.AddDays(4));

            var dateIntervalEfficiencies = new List<DateIntervalEfficiency>();
            var efficiency1 = new DateIntervalEfficiency(now.AddDays(3), now.AddDays(5), 0);
            dateIntervalEfficiencies.Add(efficiency1);

            // Act
            var newDateInterval = dateInterval.AccountForEfficiencies(dateIntervalEfficiencies);

            var correctDateInterval = new DateInterval(dateInterval.Min.Value, dateInterval.Duration.Add( efficiency1.Duration ));

            // Assert
            Assert.AreEqual(correctDateInterval, newDateInterval);
        }

        #endregion

        #region Simple Fixed End

        [Test]
        public void AccountForEfficiencies_FixedEnd_EmptyListOfDateIntervalEfficiencies_ReturnsUnchangedDateInterval()
        {
            // Arrange
            var now = DateTime.Now;
            var dateInterval = new DateInterval(now, now.AddDays(1));

            var dateIntervalEfficiencies = new List<DateIntervalEfficiency>();

            // Act
            var newDateInterval = dateInterval.AccountForEfficiencies(dateIntervalEfficiencies, FixedEndPoint.Max);

            // Assert
            Assert.AreEqual(dateInterval, newDateInterval);
        }

        [Test]
        public void AccountForEfficiencies_FixedEnd_OneZeroEfficiencyCalendarStartAndEndWithDateInterval_ReturnsCorrectResult()
        {
            // +-----------+ 
            // |-----0-----| 

            // Result:
            // +-----------------------+
            //             |-----0-----|
              

            // Arrange
            var now = DateTime.Now;
            var dateInterval = new DateInterval(now, now.AddDays(1));

            var dateIntervalEfficiencies = new List<DateIntervalEfficiency>();
            var efficiency1 = new DateIntervalEfficiency( dateInterval.Min.Value, dateInterval.Max.Value, 0 );
            dateIntervalEfficiencies.Add(efficiency1);

            // Act
            var newDateInterval = dateInterval.AccountForEfficiencies(dateIntervalEfficiencies, FixedEndPoint.Max);

            var correctDateInterval = new DateInterval(dateInterval.Min.Value.Add( - efficiency1.Duration ), dateInterval.Max.Value);

            // Assert
            Assert.AreEqual(correctDateInterval, newDateInterval);
        }

        [Test]
        public void AccountForEfficiencies_FixedEnd_OneZeroEfficiencyCalendarEndsWhenDateIntervalStarts_ReturnsSameDateInterval()
        {
            //             +-----------+ 
            // |-----0-----| 

            // Result:
            //             +-----------+  
            // |-----0-----|
            

            // Arrange
            var now = DateTime.Now;
            var dateInterval = new DateInterval(now, now.AddDays(1));

            var dateIntervalEfficiencies = new List<DateIntervalEfficiency>();
            dateIntervalEfficiencies.Add(new DateIntervalEfficiency(now.AddDays(-4), now, 0));

            // Act
            var newDateInterval = dateInterval.AccountForEfficiencies(dateIntervalEfficiencies, FixedEndPoint.Max);

            var correctDateInterval = dateInterval;

            // Assert
            Assert.AreEqual(correctDateInterval, newDateInterval);
        }

        [Test]
        public void AccountForEfficiencies_FixedEnd_OneZeroEfficiencyCalendarStartsWhenDateIntervalEnds_ReturnsSameDateInterval()
        {
            // +-----------+ 
            //             |-----0-----| 

            // Result:
            // +-----------+ 
            //             |-----0-----|  

            // Arrange
            var now = DateTime.Now;
            var dateInterval = new DateInterval(now, now.AddDays(1));

            var dateIntervalEfficiencies = new List<DateIntervalEfficiency>();
            dateIntervalEfficiencies.Add(new DateIntervalEfficiency(now.AddDays(1), now.AddDays(3), 0));

            // Act
            var newDateInterval = dateInterval.AccountForEfficiencies(dateIntervalEfficiencies, FixedEndPoint.Max);

            var correctDateInterval = dateInterval;

            // Assert
            Assert.AreEqual(correctDateInterval, newDateInterval);
        }

        [Test]
        public void AccountForEfficiencies_FixedEnd_OneZeroEfficiencyCalendarStartAndEndsWithinDateInterval_ReturnsCorrectResult()
        {
            //      +-----------+ 
            //         |--0--| 

            // Result:
            //  +---------------+ 
            //         |--0--| 

            // Arrange
            var now = DateTime.Now;
            var dateInterval = new DateInterval(now, now.AddDays(4));

            var dateIntervalEfficiencies = new List<DateIntervalEfficiency>();
            var efficiency1 = new DateIntervalEfficiency(now.AddDays(1), now.AddDays(2), 0);
            dateIntervalEfficiencies.Add(efficiency1);

            // Act
            var newDateInterval = dateInterval.AccountForEfficiencies(dateIntervalEfficiencies, FixedEndPoint.Max);

            var correctDateInterval = new DateInterval(dateInterval.Min.Value.Add( -efficiency1.Duration ), dateInterval.Max.Value);

            // Assert
            Assert.AreEqual(correctDateInterval, newDateInterval);
        }

        [Test]
        public void AccountForEfficiencies_FixedEnd_OneZeroEfficiencyCalendarStartBeforeAndEndsWithinDateInterval_ReturnsCorrectResult()
        {
            //       +-----------+ 
            //    |--0--| 

            // Result:
            // +-----------------+ 
            //    |--0--|  

            // Arrange
            var now = DateTime.Now;
            var dateInterval = new DateInterval(now, now.AddDays(4));

            var dateIntervalEfficiencies = new List<DateIntervalEfficiency>();
            var efficiency1 = new DateIntervalEfficiency(now.AddDays(-1), now.AddDays(2), 0);
            dateIntervalEfficiencies.Add(efficiency1);

            // Act
            var newDateInterval = dateInterval.AccountForEfficiencies(dateIntervalEfficiencies, FixedEndPoint.Max);

            var correctDateInterval = new DateInterval(efficiency1.Min.Value.Add( - (efficiency1.Max.Value - dateInterval.Min.Value) ), dateInterval.Max.Value);

            // Assert
            Assert.AreEqual(correctDateInterval, newDateInterval);
        }

        [Test]
        public void AccountForEfficiencies_FixedEnd_OneZeroEfficiencyCalendarStartsInAndEndsAfterDateInterval_ReturnsCorrectResult()
        {
            //      +-----------+ 
            //               |--0--| 

            // Result:
            //  +---------------+ 
            //               |--0--|

            // Arrange
            var now = DateTime.Now;
            var dateInterval = new DateInterval(now, now.AddDays(4));

            var dateIntervalEfficiencies = new List<DateIntervalEfficiency>();
            var efficiency1 = new DateIntervalEfficiency(now.AddDays(3), now.AddDays(5), 0);
            dateIntervalEfficiencies.Add(efficiency1);

            // Act
            var newDateInterval = dateInterval.AccountForEfficiencies(dateIntervalEfficiencies, FixedEndPoint.Max);

            var correctDateInterval = new DateInterval(dateInterval.Min.Value.Add( - (dateInterval.Max.Value - efficiency1.Min.Value) ), dateInterval.Max.Value);

            // Assert
            Assert.AreEqual(correctDateInterval, newDateInterval);
        }

        #endregion

        #region Fixed start

        [Test]
        public void AccountForEfficiencies_MultipleDateIntervalEfficiencies_ReturnsCorrectAnswer()
        {
            // +------------+ 
            //    |0|  |0|  |0|

            // Result:
            // +-------------------+ 
            //    |0|  |0|  |0|

            // Arrange
            var now = DateTime.Now;
            var dateInterval = new DateInterval(now, now.AddDays(5));

            var dateIntervalEfficiencies = new List<DateIntervalEfficiency>();
            var efficiency1 = new DateIntervalEfficiency(now.AddDays(1), now.AddDays(2), 0);
            var efficiency2 = new DateIntervalEfficiency(now.AddDays(3), now.AddDays(4), 0);
            var efficiency3 = new DateIntervalEfficiency(now.AddDays(5), now.AddDays(6), 0);

            dateIntervalEfficiencies.Add(efficiency1);
            dateIntervalEfficiencies.Add(efficiency2);
            dateIntervalEfficiencies.Add(efficiency3);


            // Act
            var newDateInterval = dateInterval.AccountForEfficiencies(dateIntervalEfficiencies);

            var correctDateInterval = new DateInterval(dateInterval.Min.Value, dateInterval.Duration.Add( efficiency1.Duration ).Add( efficiency2.Duration ).Add( efficiency3.Duration ));

            // Assert
            Assert.AreEqual(correctDateInterval, newDateInterval);
        }

        [Test]
        public void AccountForEfficiencies_OneEfficiencyCalendarStartAndEndWithDateInterval_ReturnsCorrectAnswer()
        {
            // +-----------+ 
            // |----200----| 

            // Result:
            // +-----+ 
            // |----200----|  

            // Arrange
            var now = DateTime.Now;
            var dateInterval = new DateInterval(now, now.AddDays(1));

            var dateIntervalEfficiencies = new List<DateIntervalEfficiency>();
            var efficiency1 = new DateIntervalEfficiency(dateInterval.Min.Value, dateInterval.Max.Value, 200);
            dateIntervalEfficiencies.Add(efficiency1);

            // Act
            var newDateInterval = dateInterval.AccountForEfficiencies(dateIntervalEfficiencies);

            var correctDateInterval = new DateInterval(dateInterval.Min.Value, dateInterval.Min.Value.AddTicks(dateInterval.Duration.Ticks / 2));

            // Assert
            Assert.AreEqual(correctDateInterval, newDateInterval);
        }

        #endregion

        #region Fixed End

        [Test]
        public void AccountForEfficiencies_FixedEnd_MultipleDateIntervalEfficiencies_ReturnsCorrectAnswer()
        {
            //         +------------+ 
            //       |0|  |0|  |0|

            // Result:
            //   +------------------+ 
            //       |0|  |0|  |0|

            // Arrange
            var now = DateTime.Now;
            var dateInterval = new DateInterval(now.AddDays(2), now.AddDays(7));

            var dateIntervalEfficiencies = new List<DateIntervalEfficiency>();
            var efficiency1 = new DateIntervalEfficiency(now.AddDays(1), now.AddDays(2), 0);
            var efficiency2 = new DateIntervalEfficiency(now.AddDays(3), now.AddDays(4), 0);
            var efficiency3 = new DateIntervalEfficiency(now.AddDays(5), now.AddDays(6), 0);

            dateIntervalEfficiencies.Add(efficiency1);
            dateIntervalEfficiencies.Add(efficiency2);
            dateIntervalEfficiencies.Add(efficiency3);


            // Act
            var newDateInterval = dateInterval.AccountForEfficiencies(dateIntervalEfficiencies, FixedEndPoint.Max);

            var correctDateInterval = new DateInterval(dateInterval.Max.Value.Add(-dateInterval.Duration).Add(-efficiency1.Duration).Add(-efficiency2.Duration).Add(-efficiency3.Duration), dateInterval.Max.Value);

            // Assert
            Assert.AreEqual(correctDateInterval, newDateInterval);
        }

        [Test]
        public void AccountForEfficiencies_FixedEnd_OneEfficiencyCalendarStartAndEndWithDateInterval_ReturnsCorrectAnswer()
        {
            // +-----------+ 
            // |----200----| 

            // Result:
            //       +-----+ 
            // |----200----|  

            // Arrange
            var now = DateTime.Now;
            var dateInterval = new DateInterval(now, now.AddDays(1));

            var dateIntervalEfficiencies = new List<DateIntervalEfficiency>();
            var efficiency1 = new DateIntervalEfficiency(dateInterval.Min.Value, dateInterval.Max.Value, 200);
            dateIntervalEfficiencies.Add(efficiency1);

            // Act
            var newDateInterval = dateInterval.AccountForEfficiencies(dateIntervalEfficiencies, FixedEndPoint.Max);

            var correctDateInterval = new DateInterval(dateInterval.Max.Value.AddTicks(-dateInterval.Duration.Ticks / 2), dateInterval.Max.Value);

            // Assert
            Assert.AreEqual(correctDateInterval, newDateInterval);
        }

        #endregion
    }
}
