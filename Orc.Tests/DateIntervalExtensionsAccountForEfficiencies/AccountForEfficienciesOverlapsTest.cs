namespace Orc.Tests.DateIntervalExtensionsAccountForEfficiencies
{
    using System;
    using System.Collections.Generic;

    using NUnit.Framework;

    using Orc.Entities;
    using Orc.Extensions;

    [TestFixture]
    public class AccountForEfficienciesOverlapsTest
    {
        DateTime now;
        DateTime start;
        List<DateIntervalEfficiency> dateIntervalEfficiencies;

        [SetUp]
        public void Init()
        {
            this.now = DateTime.Now;
            this.start = new DateTime(this.now.Year, this.now.Month, this.now.Day, 13, 0, 0);
            this.dateIntervalEfficiencies = new List<DateIntervalEfficiency>();
        }

        [TearDown]
        public void Cleanup()
        {
            this.dateIntervalEfficiencies = null;
        }

        [Test]
        public void AccountForEfficiencies_Overlap1()
        {
            // 300% has higher priority than 200%, 
            // 50% and 200% have same priority so 50% takes precedence.

            // +--------------------------+            Duration: 60 mins
            // |-----------200%-----------|			   Duration: 50 mins
            // |--50%--|           |-500%-|			   Duration (1): 30 min, (2): 10min


            // Frist we must work out the calendar efficiencies:

            // |--50%--|                               Duration: 30 mins
            //         |---200%---|                    Duration: 20 mins
            //                    |-500%-|             Duration: 10 mins

            // 30 mins at 50%	   => 15   mins at 100%
            // 20 mins at 200%     => 40   mins at 100%
            // 10 mins at 500%     => 50   mins at 100%

            // Result:
            // 60 - 15 - 40 = 5 mins
            // 5 mins at 100% is equivalend to 5/5 = 1 min at 500%
            // Total duration is: 30 + 20 + 1 = 51 mins

            // Arrange
            DateTime end = this.start.AddMinutes(60);
            var dateInterval = new DateInterval(this.start, end);

            var efficiency1 = new DateIntervalEfficiency(new DateInterval(this.start, this.start.AddMinutes(30)), 50);
            var efficiency2 = new DateIntervalEfficiency(new DateInterval(this.start.AddMinutes(30), this.start.AddMinutes(50)), 200);
            var efficiency3 = new DateIntervalEfficiency(new DateInterval(this.start.AddMinutes(50), this.start.AddMinutes(60)), 500, 1);
            this.dateIntervalEfficiencies.Add(efficiency1);
            this.dateIntervalEfficiencies.Add(efficiency2);
            this.dateIntervalEfficiencies.Add(efficiency3);

            // Act
            var newDateInterval = dateInterval.AccountForEfficiencies(this.dateIntervalEfficiencies);

            // Assert
            var result = new DateInterval(this.start, this.start.AddMinutes(51));
            Assert.AreEqual(result, newDateInterval);
        }

        [Test]
        public void AccountForEfficiencies_Overlap2()
        {
            // +--------------------------+            Duration: 80 mins
            // |--------200%----|			           Duration: 50 mins, Priority
            //    |--50%--|                            Duration: 30 mins starts at 10
            //                     |-300%-|			   Duration: 20 mins finishes at 80 mins

            // Work out the calendar efficiences

            // |--|                                    Duration: 10 mins at 200%
            //    |--50%--|                            Duration: 30 mins
            //            |--200%-|                    Duration: 10 mins
            //                    |-100%-|             Duration: 10 mins
            //                           |-300%-|      Duration: 20 mins

            // 10 mins at 200% => 20 mins at 100%
            // 30 mins at 50%  => 15 mins at 100%
            // 10 mins at 200% => 20 mins at 100%
            // 10 mins at 100% => 10 mins at 100%
            // 20 mins at 300% => 60 mins at 100%

            // 80 - 20 - 15 - 20 - 10 = 15 mins
            // 15 mins at 100% => 5 mins at 300%
            // Total duration is: 10 + 30 + 10 + 10 + 5 = 65 mins

            // Arrange
            DateTime end = this.start.AddMinutes(80);

            var dateInterval = new DateInterval(this.start, end);

            var efficiency1 = new DateIntervalEfficiency(new DateInterval(this.start, this.start.AddMinutes(50)), 200);
            var efficiency2 = new DateIntervalEfficiency(new DateInterval(this.start.AddMinutes(10), this.start.AddMinutes(40)), 50);
            var efficiency3 = new DateIntervalEfficiency(new DateInterval(this.start.AddMinutes(60), this.start.AddMinutes(80)), 300);
            this.dateIntervalEfficiencies.Add(efficiency1);
            this.dateIntervalEfficiencies.Add(efficiency2);
            this.dateIntervalEfficiencies.Add(efficiency3);

            // Act
            var newDateInterval = dateInterval.AccountForEfficiencies(this.dateIntervalEfficiencies);

            // Assert
            var result = new DateInterval(this.start, this.start.AddMinutes(65));
            Assert.AreEqual(result, newDateInterval);
        }

        [Test]
        public void AccountForEfficiencies_Overlap3()
        {
            // +--------------------------+                Duration: 55 mins
            //    |------80%-------|			  	       Duration: 30 mins, starts at 15
            //                  |-200%-|	               Duration: 20 mins starts at 40 

            //Work out the calendar efficiences
            // |--|                                         Duration: 15 mins at 100%
            //    |----80%-----|                            Duration: 30 mins
            //                 |--200%-|                    Duration: 15 mins

            // 15 mins at 100%     => 15 mins at 100%
            // 30 mins at 80%	   => 24 mins at 100%
            // 15 mins at 200%     => 30 mins at 100%

            // Result:

            // 55 - 15 - 24 = 16 mins
            // 16 min at 100% is equivalend to 8 mins at 200%
            // Total duration is: 15 + 30 + 8 = 53 min

            // Arrange
            DateTime end = this.start.AddMinutes(55);

            var dateInterval = new DateInterval(this.start, end);

            var efficiency1 = new DateIntervalEfficiency(new DateInterval(this.start.AddMinutes(15), this.start.AddMinutes(45)), 80, 1);
            var efficiency2 = new DateIntervalEfficiency(new DateInterval(this.start.AddMinutes(40), this.start.AddMinutes(60)), 200);
            this.dateIntervalEfficiencies.Add(efficiency1);
            this.dateIntervalEfficiencies.Add(efficiency2);

            // Act
            var newDateInterval = dateInterval.AccountForEfficiencies(this.dateIntervalEfficiencies);

            // Assert
            var result = new DateInterval(this.start, this.start.AddMinutes(53));
            Assert.AreEqual(result, newDateInterval);
        }

        [Test]
        public void AccountForEfficiencies_Overlap4()
        {
            // +---------------------------------+       Duration: 90 mins
            //   |------90%----------|			   	     Duration: 40 mins, starts at 10
            //   |--30%--|                               Duration: 15 mins starts at 10
            //                          |-150%-|	     Duration: 10 mins finishes at 80 mins

            // Work out the calendar efficiences

            // |--|										   Duration: 10 mins at 100%
            //     |--30%--|                               Duration: 15 mins
            //             |-----90%----|                  Duration: 25 mins
            //                          |---|              Duration: 20 mins at 100%
            //                               |-150%-|      Duration: 10 mins
            //                                      |--|   Duration: 10 mins at 100%

            // 10 mins at 10%	   => 10   mins at 100%
            // 15 mins at 30%	   => 4.5  mins at 100%
            // 25 mins at 90%      => 22.5   mins at 100%
            // 20 mins at 100%	   => 20   mins at 100%
            // 10 mins at 150%     => 15   mins at 100%
            // 10 mins at 100%	   => 10   mins at 100%

            // Result:
            // 90 - 10 - 4.5 - 22.5 - 20 - 15 - 10 = 8 mins
            // Total duration is: 10 + 15 + 25 + 20 + 10 + 10 + 8 = 98 mins

            // Arrange
            DateTime end = this.start.AddMinutes(90);

            var dateInterval = new DateInterval(this.start, end);

            var efficiency1 = new DateIntervalEfficiency(new DateInterval(this.start.AddMinutes(10), this.start.AddMinutes(50)), 90);
            var efficiency2 = new DateIntervalEfficiency(new DateInterval(this.start.AddMinutes(10), this.start.AddMinutes(25)), 30, 1);
            var efficiency3 = new DateIntervalEfficiency(new DateInterval(this.start.AddMinutes(70), this.start.AddMinutes(80)), 150);
            this.dateIntervalEfficiencies.Add(efficiency1);
            this.dateIntervalEfficiencies.Add(efficiency2);
            this.dateIntervalEfficiencies.Add(efficiency3);

            // Act
            var newDateInterval = dateInterval.AccountForEfficiencies(this.dateIntervalEfficiencies);

            // Assert
            var result = new DateInterval(this.start, this.start.AddMinutes(98));
            Assert.AreEqual(result, newDateInterval);
        }

        [Test]
        public void AccountForEfficiencies_Overlap5()
        {
            //+--------------------------+            Duration: 100 mins
            //   |------120%---------|				  Duration: 45 mins, starts at 10 (PRIORITY)
            //     |----100%-------|                  Duration: 35 mins, starts at 15
            //         |-30%-|			      		  Duration: 25 mins, starts at 20

            // Work out the calendar efficiences:
            // If 120% is the priority then it takes precedence over the 100% and the 30% calendar period
            // Because 100% and 30% have the same priority, 30% takes precedence (lower efficiency)

            // |--|									Duration: 10 mins
            //    |---120%-----------|              Duration: 45 mins
            //                       |-100%-|		Duration: 45 mins


            // 10 mins at 100%	  => 10   mins at 100%
            // 45 mins at 120%	  => 54   mins at 100%
            // 45 mins at 100%    => 45   mins at 100%

            // Result:

            // 100 - 10 - 54 = 36 mins
            // Total duration is: 10 + 45 + 36 = 91

            // Arrange
            DateTime end = this.start.AddMinutes(100);

            var dateInterval = new DateInterval(this.start, end);

            var efficiency1 = new DateIntervalEfficiency(new DateInterval(this.start.AddMinutes(10), this.start.AddMinutes(55)), 120, 1);
            var efficiency2 = new DateIntervalEfficiency(new DateInterval(this.start.AddMinutes(15), this.start.AddMinutes(50)), 100);
            var efficiency3 = new DateIntervalEfficiency(new DateInterval(this.start.AddMinutes(20), this.start.AddMinutes(45)), 30);
            this.dateIntervalEfficiencies.Add(efficiency1);
            this.dateIntervalEfficiencies.Add(efficiency2);
            this.dateIntervalEfficiencies.Add(efficiency3);

            // Act
            var newDateInterval = dateInterval.AccountForEfficiencies(this.dateIntervalEfficiencies);

            // Assert
            var result = new DateInterval(this.start, this.start.AddMinutes(91));
            Assert.AreEqual(result, newDateInterval);
        }

        [Test]
        public void AccountForEfficiencies_Overlap6()
        {
            // +--------------------------+           Duration: 100 mins
            //   |------120%---------|				  Duration: 45 mins, starts at 10
            //     |----100%-------|                  Duration: 35 mins, starts at 15
            //         |-30%-|			      		  Duration: 25 mins, starts at 20

            // Work out the calendar efficiences:

            // Assumption all the calendar periods have the same efficiency. To the lowest efficiency takes precedence

            //|--|									  Duration: 10 mins
            //    |-|                                 Duration: 5 mins at 120%
            //      |-|                               Duration: 5 mins at 100%
            //        |-----|                         Duration: 25 mins at 30%
            //              |-|                       Duration: 5 mins at 100%
            //                |-|                     Duration: 5 mins at 120%
            //                  |-----|               Duration: 45 mins at 100% (which is the default value)

            // 10 mins at 100%	  => 10   mins at 100%
            // 5 mins at 120%	  => 6    mins at 100%
            // 5 mins at 100%     => 5    mins at 100%
            // 25 mins at 30%	  => 7.5  mins at 100%
            // 5 mins at 100%	  => 5    mins at 100%
            // 5 mins at 120%	  => 6    mins at 100%
            // 45 mins at 100%    => 45   mins at 100%

            // Result:
            // 100 - 10 - 6 - 5 - 7.5 - 5 - 6 - 45 = 15.5 mins
            // Total duration is: 10 + 5 + 5 + 25 + 5 + 5 + 45 + 15.5 = 115.5 mins

            // Arrange
            DateTime end = this.start.AddMinutes(100);

            var dateInterval = new DateInterval(this.start, end);

            var efficiency1 = new DateIntervalEfficiency(new DateInterval(this.start.AddMinutes(10), this.start.AddMinutes(55)), 120);
            var efficiency2 = new DateIntervalEfficiency(new DateInterval(this.start.AddMinutes(15), this.start.AddMinutes(50)), 100);
            var efficiency3 = new DateIntervalEfficiency(new DateInterval(this.start.AddMinutes(20), this.start.AddMinutes(45)), 30);
            this.dateIntervalEfficiencies.Add(efficiency1);
            this.dateIntervalEfficiencies.Add(efficiency2);
            this.dateIntervalEfficiencies.Add(efficiency3);

            // Act
            var newDateInterval = dateInterval.AccountForEfficiencies(this.dateIntervalEfficiencies);

            // Assert
            var result = new DateInterval(this.start, this.start.AddMinutes(115.5));
            Assert.AreEqual(result, newDateInterval);
        }
    }
}