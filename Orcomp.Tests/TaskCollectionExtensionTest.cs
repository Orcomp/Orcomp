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
    public class TaskCollectionExtensionTest
    {
        #region Collapse()

        [TestMethod]
        public void Collapse_NoTasksInCollectionWhenExecuted_ReturnsEmptyList()
        {
            // Arrange
            var taskCollection = new Collection<ITask>();

            // Act
            IEnumerable<ITask> tasks = taskCollection.Collapse();

            // Assert
            Assert.False(tasks.Any());
        }

        [TestMethod]
        public void Collapse_TwoTasksWithSameDateRange_ReturnsOneItem()
        {
            // Arrange
            var taskCollection = new Collection<ITask>();
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(1.5);
            taskCollection.Add(Task.CreateUsingQuantity(t1, t2, 100F));
            taskCollection.Add(Task.CreateUsingQuantity(t1, t2, 200F));

            // Act
            IEnumerable<ITask> tasks = taskCollection.Collapse();

            // Assert
            Assert.True(tasks.Count() == 1);
        }

        [TestMethod]
        public void Collapse_TwoTasksWithSameDateRangeOneProducingOneConsumingWithSameQuantity_ReturnsOneItem()
        {
            // Arrange
            var taskCollection = new Collection<ITask>();
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(1.5);
            taskCollection.Add(Task.CreateUsingQuantity(t1, t2, 100F));
            taskCollection.Add(Task.CreateUsingQuantity(t1, t2, -100F));

            // Act
            IEnumerable<ITask> tasks = taskCollection.Collapse();

            // Assert
            Assert.True(tasks.Count() == 1);
            Assert.True(tasks.Single().Quantity == 0);
        }

        [TestMethod]
        public void Collapse_TwoTasksWithOverlappingDateRange_ReturnsThreeItems()
        {
            // Arrange
            var taskCollection = new Collection<ITask>();
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(1.5);
            DateTime t3 = t1.AddHours(1);
            DateTime t4 = t1.AddHours(3);
            taskCollection.Add(Task.CreateUsingQuantity(t1, t2, 100F));
            taskCollection.Add(Task.CreateUsingQuantity(t3, t4, 200F));

            // Act
            IEnumerable<ITask> tasks = taskCollection.Collapse();

            // Assert
            Assert.True(tasks.Count() == 3);
        }

        [TestMethod]
        public void Collapse_TwoTasksWithoutOverlappingDateRange_ReturnsTwoItems()
        {
            // Arrange
            var taskCollection = new Collection<ITask>();
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(1.5);
            DateTime t3 = t1.AddHours(2);
            DateTime t4 = t1.AddHours(3);
            taskCollection.Add(Task.CreateUsingQuantity(t1, t2, 100F));
            taskCollection.Add(Task.CreateUsingQuantity(t3, t4, 200F));

            // Act
            IEnumerable<ITask> tasks = taskCollection.Collapse();

            // Assert
            Assert.True(tasks.Count() == 2);
        }

        [TestMethod]
        public void Collapse_ThreeTasksWithEndTimeOfOneEqualToStartTimeOfOther_ReturnsThreeItem()
        {
            // Arrange
            var taskCollection = new Collection<ITask>();
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(1);
            DateTime t3 = t1.AddHours(2);
            DateTime t4 = t1.AddHours(3);
            taskCollection.Add(Task.CreateUsingQuantity(t1, t2, 100F)); // +----+
            taskCollection.Add(Task.CreateUsingQuantity(t2, t3, -100F));//      +----+
            taskCollection.Add(Task.CreateUsingQuantity(t3, t4, 100F)); //           +----+

            // Act
            IEnumerable<ITask> tasks = taskCollection.Collapse();

            // Assert
            Assert.True(tasks.Count() == 3);
        }

        [TestMethod]
        public void Collapse_ThreeTasksWithEndTimeOfOneEqualToStartTimeOfOtherReverseOrder_ReturnsThreeItem()
        {
            // Arrange
            var taskCollection = new Collection<ITask>();
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(1);
            DateTime t3 = t1.AddHours(2);
            DateTime t4 = t1.AddHours(3);
            taskCollection.Add(Task.CreateUsingQuantity(t3, t4, 100F)); //           +----+
            taskCollection.Add(Task.CreateUsingQuantity(t2, t3, -100F));//      +----+
            taskCollection.Add(Task.CreateUsingQuantity(t1, t2, 100F)); // +----+

            // Act
            IEnumerable<ITask> tasks = taskCollection.Collapse();

            // Assert
            Assert.True(tasks.Count() == 3);
        }

        [TestMethod]
        public void Collapse_TaskCollection_ReturnsCorrectEnumerable()
        {
            // Arrange
            var taskCollection = new Collection<ITask>();
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(1);
            DateTime t3 = t1.AddHours(2);
            DateTime t4 = t1.AddHours(3);
                                                                                // 1----2----3----4
            taskCollection.Add(Task.CreateUsingQuantityPerHour(t1, t3, 100F));  // +---------+
            taskCollection.Add(Task.CreateUsingQuantityPerHour(t2, t4, 200F));  //      +---------+
            taskCollection.Add(Task.CreateUsingQuantityPerHour(t2, t3, -300F)); //      +----+

            // Act
            IEnumerable<ITask> tasks = taskCollection.Collapse();

            // Assert
            var tasksCorrect = new Collection<ITask>();
                                                                                // 1----2----3----4
            tasksCorrect.Add(Task.CreateUsingQuantityPerHour(t1, t2, 100F));    // +----+
            tasksCorrect.Add(Task.CreateUsingQuantityPerHour(t2, t3, 0F));      //      +----+
            tasksCorrect.Add(Task.CreateUsingQuantityPerHour(t3, t4, 200F));    //           +----+

            Assert.Equal(tasksCorrect, tasks);
        }

        [TestMethod]
        public void Collapse_TaskCollection_A_ReturnsCorrectEnumerable()
        {
            // Arrange
            var taskCollection = new Collection<ITask>();
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(1);
            DateTime t3 = t1.AddHours(2);
            DateTime t4 = t1.AddHours(3);
            // 1----2----3----4
            taskCollection.Add(Task.CreateUsingQuantityPerHour(t1, t3, 100F));  // +---------+
            taskCollection.Add(Task.CreateUsingQuantityPerHour(t2, t4, 200F));  //      +---------+
            taskCollection.Add(Task.CreateUsingQuantityPerHour(t2, t3, -300F)); //      +----+
            taskCollection.Add(Task.CreateUsingQuantityPerHour(t2, t3, 100F));  //      +----+

            // Act
            IEnumerable<ITask> tasks = taskCollection.Collapse();

            // Assert
            var tasksCorrect = new Collection<ITask>();
            // 1----2----3----4
            tasksCorrect.Add(Task.CreateUsingQuantityPerHour(t1, t2, 100F));    // +----+
            tasksCorrect.Add(Task.CreateUsingQuantityPerHour(t2, t3, 100F));    //      +----+
            tasksCorrect.Add(Task.CreateUsingQuantityPerHour(t3, t4, 200F));    //           +----+

            Assert.Equal(tasksCorrect, tasks);
        }

        [TestMethod]
        public void Collapse_TaskCollection2_ReturnsCorrectEnumerable()
        {
            // Arrange
            var taskCollection = new Collection<ITask>();
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(1);
            DateTime t3 = t1.AddHours(2);
            DateTime t4 = t1.AddHours(3);
            DateTime t5 = t1.AddHours(4);
            DateTime t6 = t1.AddHours(5);
            DateTime t7 = t1.AddHours(6);
            DateTime t8 = t1.AddHours(7);
            DateTime t9 = t1.AddHours(8);
                                                                                // 1----2----3----4----5----6----7----8----9
            taskCollection.Add(Task.CreateUsingQuantityPerHour(t1, t6, 100F));  // +------------------------+
            taskCollection.Add(Task.CreateUsingQuantityPerHour(t8, t9, -200F)); //                                    +----+
            taskCollection.Add(Task.CreateUsingQuantityPerHour(t3, t5, 200F));  //           +---------+
            taskCollection.Add(Task.CreateUsingQuantityPerHour(t6, t7, 200F));  //                          +----+
            taskCollection.Add(Task.CreateUsingQuantityPerHour(t2, t4, 200F));  //      +---------+
            taskCollection.Add(Task.CreateUsingQuantityPerHour(t2, t3, -300F)); //      +----+

            // Act
            IEnumerable<ITask> tasks = taskCollection.Collapse();

            // Assert
            var tasksCorrect = new Collection<ITask>();
                                                                                // 1----2----3----4----5----6----7----8----9
            tasksCorrect.Add(Task.CreateUsingQuantityPerHour(t1, t2, 100F));    // +----+
            tasksCorrect.Add(Task.CreateUsingQuantityPerHour(t2, t3, 0F));      //      +----+
            tasksCorrect.Add(Task.CreateUsingQuantityPerHour(t3, t4, 500F));    //           +----+
            tasksCorrect.Add(Task.CreateUsingQuantityPerHour(t4, t5, 300F));    //                +----+
            tasksCorrect.Add(Task.CreateUsingQuantityPerHour(t5, t6, 100F));    //                     +----+
            tasksCorrect.Add(Task.CreateUsingQuantityPerHour(t6, t7, 200F));    //                          +----+
            tasksCorrect.Add(Task.CreateUsingQuantityPerHour(t8, t9, -200F));   //                                     +----+

            Assert.Equal(tasksCorrect, tasks);
        }

        #endregion

        #region GetQuantity()

        [TestMethod]
        public void GetQuantity_NoTasksInCollectionWhenExecuted_ReturnsZeroQuantity()
        {
            // Arrange
            var taskCollection = new Collection<ITask>();
            DateTime t1 = DateTime.Now;

            // Act
            double actual = taskCollection.GetQuantity(t1);

            // Assert
            Assert.Equal(0F, actual);
        }

        [TestMethod]
        public void GetQuantity_SingleTaskInCollectionWhenExecuted_ReturnsQuantity()
        {
            // Arrange
            var taskCollection = new Collection<ITask>();
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(1.5);
            taskCollection.Add(Task.CreateUsingQuantity(t1, t2, 100F));
            var date = t2;

            // Act
            double actual = taskCollection.GetQuantity(date);

            // Assert
            Assert.Equal(100F, actual);
        }

        [TestMethod]
        public void GetQuantity_MultipleTasksTypeProduceWhenExecuted_ReturnsTotalQuantityProduced()
        {
            // Arrange
            var taskCollection = new Collection<ITask>();
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(1.5);
            taskCollection.Add(Task.CreateUsingQuantity(t1, t2, 100F));
            taskCollection.Add(Task.CreateUsingQuantity(t1, t2, 100F));
            var date = t2;

            // Act
            double actual = taskCollection.GetQuantity(date);

            // Assert
            Assert.Equal(200F, actual);
        }

        [TestMethod]
        public void GetQuantity_MultipleTasksBothProduceAndConsumeWhenExecuted_ReturnsTotalOverallQuantity()
        {
            // Arrange
            var taskCollection = new Collection<ITask>();
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(1.5);
            taskCollection.Add(Task.CreateUsingQuantity(t1, t2, 100F));
            taskCollection.Add(Task.CreateUsingQuantity(t1, t2, 100F));
            taskCollection.Add(Task.CreateUsingQuantity(t1, t2, -100F));
            var date = t2;

            // Act
            double actual = taskCollection.GetQuantity(date);

            // Assert
            Assert.Equal(100F, actual);
        }

        [TestMethod]
        public void GetQuantity_MultipleTasksBothProduceAndConsumeWithStaggeredTimesWhenExecuted_ReturnsTotalOverallQuantity()
        {
            // Arrange
            var taskCollection = new Collection<ITask>();
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(1.5);
            DateTime t3 = t1.AddHours(1);
            DateTime t4 = t1.AddHours(4);
            DateTime t5 = t1.AddHours(0.5);
            DateTime t6 = t1.AddHours(2.5);
            taskCollection.Add(Task.CreateUsingQuantity(t1, t2, 100F)); // + 100
            taskCollection.Add(Task.CreateUsingQuantity(t3, t4, 300F)); // + 100
            taskCollection.Add(Task.CreateUsingQuantity(t5, t6, -100F)); // - 75
            var date = t1.AddHours(2);

            // Act
            double actual = taskCollection.GetQuantity(date);

            // Assert
            Assert.Equal(125F, actual);
        }

        [TestMethod]
        public void GetQuantity_MultipleTasksBothProduceAndConsumeWithStaggeredTimesButNotStartedWhenExecuted_ReturnsZeroQuantity()
        {
            // Arrange
            var taskCollection = new Collection<ITask>();
            DateTime t1 = DateTime.Now.AddDays(1);
            DateTime t2 = t1.AddHours(1.5);
            DateTime t3 = t1.AddHours(1);
            DateTime t4 = t1.AddHours(4);
            DateTime t5 = t1.AddHours(0.5);
            DateTime t6 = t1.AddHours(2.5);
            taskCollection.Add(Task.CreateUsingQuantity(t1, t2, 100F));
            taskCollection.Add(Task.CreateUsingQuantity(t3, t4, 300F));
            taskCollection.Add(Task.CreateUsingQuantity(t5, t6, -100F));
            var date = DateTime.Now;

            // Act
            double actual = taskCollection.GetQuantity(date);

            // Assert
            Assert.Equal(0F, actual);
        }

        #endregion

        #region WillBreachMinLevel

        [TestMethod]
        public void WillBreachMinLevel_ProducingTasksWithInitialValueEqualToMinLevel_ReturnsFalse()
        {
            // Arrange
            var taskCollection = new Collection<ITask>();
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(2);
            taskCollection.Add(Task.CreateUsingQuantity(t1, t2, 100));

            // Act
            bool willBreachMinLevel = taskCollection.WillBreachMinLevel(100, 100);

            // Assert
            Assert.False(willBreachMinLevel);
        }

        public void WillBreachMinLevel_ConsumingTasksWithEndValueEqualToMinLevel_ReturnsFalse()
        {
            // Arrange
            var taskCollection = new Collection<ITask>();
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(2);
            taskCollection.Add(Task.CreateUsingQuantity(t1, t2, -100));

            // Act
            bool willBreachMinLevel = taskCollection.WillBreachMinLevel(100, 200);

            // Assert
            Assert.False(willBreachMinLevel);
        }

        public void WillBreachMinLevel_ConsumingTasksWithEndValueLessThanMinLevel_ReturnsTrue()
        {
            // Arrange
            var taskCollection = new Collection<ITask>();
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(2);
            taskCollection.Add(Task.CreateUsingQuantity(t1, t2, -110));

            // Act
            bool willBreachMinLevel = taskCollection.WillBreachMinLevel(100, 200);

            // Assert
            Assert.True(willBreachMinLevel);
        }

        #endregion

        #region WillBreachMaxLevel

        [TestMethod]
        public void WillBreachMaxLevel_ConsumingTasksWithInitialValueEqualToMaxLevel_ReturnsFalse()
        {
            // Arrange
            var taskCollection = new Collection<ITask>();
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(2);
            taskCollection.Add(Task.CreateUsingQuantity(t1, t2, -100));

            // Act
            bool willBreachMaxLevel = taskCollection.WillBreachMaxLevel(100, 100);

            // Assert
            Assert.False(willBreachMaxLevel);
        }

        [TestMethod]
        public void WillBreachMaxLevel_ProducingTasksWithEndValueEqualToMaxLevel_ReturnsFalse()
        {
            // Arrange
            var taskCollection = new Collection<ITask>();
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(2);
            taskCollection.Add(Task.CreateUsingQuantity(t1, t2, 100));

            // Act
            bool willBreachMaxLevel = taskCollection.WillBreachMaxLevel(200, 100);

            // Assert
            Assert.False(willBreachMaxLevel);
        }

        [TestMethod]
        public void WillBreachMaxLevel_ProducingTasksWithEndValueGreaterThanMaxLevel_ReturnsTrue()
        {
            // Arrange
            var taskCollection = new Collection<ITask>();
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(2);
            taskCollection.Add(Task.CreateUsingQuantity(t1, t2, 110));

            // Act
            bool willBreachMaxLevel = taskCollection.WillBreachMaxLevel(200, 100);

            // Assert
            Assert.True(willBreachMaxLevel);
        }

        #endregion


        #region WillStayWithinRange

        [TestMethod]
        public void WillStayWithinRange_InitialValueOnMinLevelEndValueOnMaxValue_ReturnsTrue()
        {
            // Arrange
            var taskCollection = new Collection<ITask>();
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(2);
            taskCollection.Add(Task.CreateUsingQuantity(t1, t2, 100));

            // Act
            bool value = taskCollection.WillStayWithinRange( 100, 200, 100 );

            // Assert
            Assert.True(value);
        }

        [TestMethod]
        public void WillStayWithinRange_InitialValueOnMinLevelEndValueOverMaxValue_ReturnsFalse()
        {
            // Arrange
            var taskCollection = new Collection<ITask>();
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(2);
            taskCollection.Add(Task.CreateUsingQuantity(t1, t2, 110));

            // Act
            bool value = taskCollection.WillStayWithinRange(100, 200, 100);

            // Assert
            Assert.False(value);
        }

        [TestMethod]
        public void WillStayWithinRange_InitialValueOnMaxValueEndValueBelowMinLevel_ReturnsFalse()
        {
            // Arrange
            var taskCollection = new Collection<ITask>();
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(2);
            taskCollection.Add(Task.CreateUsingQuantity(t1, t2, -110));

            // Act
            bool value = taskCollection.WillStayWithinRange(100, 200, 200);

            // Assert
            Assert.False(value);
        }

        [TestMethod]
        public void WillStayWithinRange_InitialValueBelowMinLevel_ReturnsException()
        {
            // Arrange
            var taskCollection = new Collection<ITask>();
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(2);
            taskCollection.Add(Task.CreateUsingQuantity(t1, t2, -110));

            // Act
            bool value;

            // Assert
            Assert.Throws<ArgumentException>( () => value = taskCollection.WillStayWithinRange( 100, 200, 90 ) );
        }

        [TestMethod]
        public void WillStayWithinRange_InitialValueAboveMaxLevel_ReturnsException()
        {
            // Arrange
            var taskCollection = new Collection<ITask>();
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(2);
            taskCollection.Add(Task.CreateUsingQuantity(t1, t2, -110));

            // Act
            bool value;

            // Assert
            Assert.Throws<ArgumentException>(() => value = taskCollection.WillStayWithinRange(100, 200, 210));
        }

        #endregion

        #region MakeTasksComply()

        [TestMethod]
        public void TryToMakeTasksComply_IfImpossible_ReturnsNull()
        {
            // Arrange
            var taskCollection = new Collection<ITask>();
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(2);
            taskCollection.Add(Task.CreateUsingQuantity(t1, t2, 110));

            // Act
            IEnumerable<ITask> value = taskCollection.TryToMakeTasksComply(100, 200, 100);

            // Assert
            Assert.Null( value );
        }

        [TestMethod]
        public void TryToMakeTasksComply_WillGoAboveMaxValue_ReturnsCorrectCollection()
        {
            // Arrange
            double minLevel = 100;
            double maxLevel = 200;
            double initialLevel = 100;
            var taskCollection = new Collection<ITask>();
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(1);
            DateTime t3 = t1.AddHours(2);
            DateTime t4 = t1.AddHours(3);
            Task task1 = Task.CreateUsingQuantity( t1, t2, 200 );
            Task task2 = Task.CreateUsingQuantity( t3, t4, -200);
            taskCollection.Add(task1);
            taskCollection.Add(task2);

            // Act
            IEnumerable<ITask> newCollection = taskCollection.TryToMakeTasksComply(minLevel, maxLevel, initialLevel);

            // Assert
            var correctCollection = new Collection<ITask>();
            correctCollection.Add( task1.ChangeTimes( t1.AddHours( 1.5 ))); // The producing task moves forward
            correctCollection.Add( task2 );

            Assert.Equal( correctCollection , newCollection );
            Assert.True( newCollection.WillStayWithinRange(minLevel, maxLevel, initialLevel));
        }

        [TestMethod]
        public void TryToMakeTasksComply_WillGoBelowMinValue_ReturnsCorrectCollection()
        {
            // Arrange
            double minLevel = 100;
            double maxLevel = 200;
            double initialLevel = 100;
            var taskCollection = new Collection<ITask>();
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(1);
            DateTime t3 = t1.AddHours(2);
            DateTime t4 = t1.AddHours(3);
            Task task1 = Task.CreateUsingQuantity(t1, t2,-100);
            Task task2 = Task.CreateUsingQuantity(t3, t4, 100);
            taskCollection.Add(task1);
            taskCollection.Add(task2);

            // Act
            IEnumerable<ITask> newCollection = taskCollection.TryToMakeTasksComply(minLevel, maxLevel, initialLevel);

            // Assert
            var correctCollection = new Collection<ITask>();
            correctCollection.Add(task1.ChangeTimes(t3)); // The consuming task moves forward
            correctCollection.Add(task2);

            Assert.Equal(correctCollection, newCollection);
            Assert.True(newCollection.WillStayWithinRange(minLevel, maxLevel, initialLevel));
        }

        [TestMethod]
        public void TryToMakeTasksComply_WillGoAboveMaxValue_ReturnsCorrectCollection2()
        {
            // Arrange
            double minLevel = 100;
            double maxLevel = 200;
            double initialLevel = 200;
            var taskCollection = new Collection<ITask>();
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(1);
            DateTime t3 = t1.AddHours(2);
            DateTime t4 = t1.AddHours(3);
            Task task1 = Task.CreateUsingQuantity(t1, t2, 100);
            Task task2 = Task.CreateUsingQuantity(t2, t3, -50);
            Task task3 = Task.CreateUsingQuantity(t3, t4, -30);
            Task task4 = Task.CreateUsingQuantity(t2, t4, -20);
            taskCollection.Add(task1);
            taskCollection.Add(task2);
            taskCollection.Add(task3);
            taskCollection.Add(task4);

            // Act
            IEnumerable<ITask> newCollection = taskCollection.TryToMakeTasksComply(minLevel, maxLevel, initialLevel);

            // Assert
            var consumingTasks = newCollection.OrderBy(x => x.DateRange).Where(task => task.TaskType == TaskType.Consume).ToList();
            Assert.Equal(t4, newCollection.Single(task => task.TaskType == TaskType.Produce).DateRange.EndTime); // Don't move producing task forward more than you need to.
            Assert.Equal(task1.DateRange.Duration, newCollection.Single(task => task.TaskType == TaskType.Produce).DateRange.Duration);

            // Consuming tasks do not change. Only the producing tasks change since the level went over the max value.
            Assert.True(consumingTasks[0].Equals( task2 ));
            Assert.True(consumingTasks[1].Equals( task4 ));
            Assert.True(consumingTasks[2].Equals( task3 ));

            Assert.True(newCollection.WillStayWithinRange(minLevel, maxLevel, initialLevel));
        }

        [TestMethod]
        public void TryToMakeTasksComply_WillGoBelowMinValue_ReturnsCorrectCollection2()
        {
            // Arrange
            double minLevel = 100;
            double maxLevel = 200;
            double initialLevel = 100;
            var taskCollection = new Collection<ITask>();
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(1);
            DateTime t3 = t1.AddHours(2);
            DateTime t4 = t1.AddHours(3);
            Task task1 = Task.CreateUsingQuantity(t1, t2, -100);
            Task task2 = Task.CreateUsingQuantity(t2, t3, 50);
            Task task3 = Task.CreateUsingQuantity(t3, t4, 30);
            Task task4 = Task.CreateUsingQuantity(t2, t4, 20);
            taskCollection.Add(task1);
            taskCollection.Add(task2);
            taskCollection.Add(task3);
            taskCollection.Add(task4);

            // Act
            IEnumerable<ITask> newCollection = taskCollection.TryToMakeTasksComply(minLevel, maxLevel, initialLevel);

            // Assert
            var producingTasks = newCollection.OrderBy(x => x.DateRange).Where(task => task.TaskType == TaskType.Produce).ToList();

            Assert.Equal(t4, newCollection.Single(task => task.TaskType == TaskType.Consume).DateRange.EndTime);
            Assert.Equal(task1.DateRange.Duration, newCollection.Single(task => task.TaskType == TaskType.Consume).DateRange.Duration);
            Assert.True(producingTasks[0].Equals(task2));
            Assert.True(producingTasks[1].Equals(task4));
            Assert.True(producingTasks[2].Equals(task3));
            Assert.True(newCollection.WillStayWithinRange(minLevel, maxLevel, initialLevel));
        }

        [TestMethod]
        public void TryToMakeTasksComply_WillGoAboveMaxValue_ReturnsCorrectCollection3()
        {
            // Arrange
            double minLevel = 100;
            double maxLevel = 200;
            double initialLevel = 170;
            var taskCollection = new Collection<ITask>();
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(1);
            DateTime t3 = t1.AddHours(2);
            DateTime t4 = t1.AddHours(3);
            DateTime t5 = t1.AddHours(4);
            Task task1 = Task.CreateUsingQuantity(t2, t3, 100);
            Task task2 = Task.CreateUsingQuantity(t1, t3, -60);
            Task task3 = Task.CreateUsingQuantity(t4, t5, -10);
            taskCollection.Add(task1);
            taskCollection.Add(task2);
            taskCollection.Add(task3);

            // Act
            IEnumerable<ITask> newCollection = taskCollection.TryToMakeTasksComply(minLevel, maxLevel, initialLevel);

            // Assert
            Assert.NotEqual(newCollection.Single(task => task.TaskType == TaskType.Produce).DateRange.EndTime, t4);
            var consumingTasks = newCollection.Where(task => task.TaskType == TaskType.Consume).ToList();
            Assert.True(consumingTasks[0].Equals(task2));
            Assert.True(consumingTasks[1].Equals(task3));
            Assert.True( newCollection.GetDataPoints( initialLevel ).Select( x => x.Value ).Max() == maxLevel  ); // Quantity level just touches the max level
            Assert.True(newCollection.WillStayWithinRange(minLevel, maxLevel, initialLevel));
        }

        //-----------------------------
        // Mixed Data

        [TestMethod]
        public void TryToMakeTasksComply_MixedData_ReturnsCorrectCollection()
        {
            double minLevel = 100;
            double maxLevel = 200;
            double initialLevel = 100;
            var taskCollection = new Collection<ITask>();
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(1);
            DateTime t3 = t1.AddHours(2);
            DateTime t4 = t1.AddHours(3);

            Task task1 = Task.CreateUsingQuantity(t1, t2, -200);
            Task task2 = Task.CreateUsingQuantity(t2, t3, 400);
            Task task3 = Task.CreateUsingQuantity(t3, t4, -200);
            taskCollection.Add(task1);
            taskCollection.Add(task2);
            taskCollection.Add(task3);

            // Act
            IEnumerable<ITask> newCollection = taskCollection.TryToMakeTasksComply(minLevel, maxLevel, initialLevel);

            var correctCollection = new Collection<ITask>();
            correctCollection.Add(task1.ChangeTimes(t2.AddHours(0.5)));
            correctCollection.Add(task2.ChangeTimes(t2.AddHours(0.5)));
            correctCollection.Add(task3);

            // OLD TEST DATA
            //correctCollection.Add(task1.ChangeTimes(t3));
            //correctCollection.Add(task2.ChangeTimes(t3));
            //correctCollection.Add(task3);

            Assert.Equal(correctCollection, newCollection);
        }

        [TestMethod]
        public void TryToMakeTasksComply_MixedData_ReturnsCorrectCollection2()
        {
            double minLevel = 100;
            double maxLevel = 200;
            double initialLevel = 100;
            var taskCollection = new Collection<ITask>();
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(1);
            DateTime t3 = t1.AddHours(2);
            DateTime t4 = t1.AddHours(3);

            Task task1 = Task.CreateUsingQuantity(t1, t2, -200);
            Task task2 = Task.CreateUsingQuantity(t2, t3, -200);
            Task task3 = Task.CreateUsingQuantity(t3, t4, 400);
            taskCollection.Add(task1);
            taskCollection.Add(task2);
            taskCollection.Add(task3);

            // Act
            IEnumerable<ITask> newCollection = taskCollection.TryToMakeTasksComply(minLevel, maxLevel, initialLevel);

            var correctCollection = new Collection<ITask>();
            correctCollection.Add(task1.ChangeTimes(t3));
            correctCollection.Add(task2.ChangeTimes(t3));
            correctCollection.Add(task3);

            Assert.Equal(correctCollection, newCollection);
        }

        [TestMethod]
        public void TryToMakeTasksComply_MixedData_ReturnsCorrectCollection3()
        {
            double minLevel = 100;
            double maxLevel = 200;
            double initialLevel = 100;
            var taskCollection = new Collection<ITask>();
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(1);
            DateTime t3 = t1.AddHours(2);
            DateTime t4 = t1.AddHours(3);
            DateTime t5 = t1.AddHours(4);

            Task task1 = Task.CreateUsingQuantity(t1, t2, -200);
            Task task2 = Task.CreateUsingQuantity(t2, t3, -200);
            Task task3 = Task.CreateUsingQuantity(t3, t4, 600);
            Task task4 = Task.CreateUsingQuantity(t4, t5, -100);
            taskCollection.Add(task1);
            taskCollection.Add(task2);
            taskCollection.Add(task3);
            taskCollection.Add(task4);

            // Act
            IEnumerable<ITask> newCollection = taskCollection.TryToMakeTasksComply(minLevel, maxLevel, initialLevel);

            var correctCollection = new Collection<ITask>();
            correctCollection.Add(task1.ChangeTimes(t4));
            correctCollection.Add(task2.ChangeTimes(t4));
            correctCollection.Add(task3.ChangeTimes(t4));
            correctCollection.Add(task4);

            Assert.Equal(correctCollection, newCollection);
        }

        [TestMethod]
        public void TryToMakeTasksComply_MixedData_ReturnsCorrectCollection4()
        {
            double minLevel = 100;
            double maxLevel = 200;
            double initialLevel = 100;
            var taskCollection = new Collection<ITask>();
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(1);
            DateTime t3 = t1.AddHours(2);
            DateTime t4 = t1.AddHours(3);
            DateTime t5 = t1.AddHours(4);
            DateTime t6 = t1.AddHours(5);
            DateTime t7 = t1.AddHours(6);
            DateTime t8 = t1.AddHours(7);

            Task task1 = Task.CreateUsingQuantity(t1, t6, 500);
            Task task2 = Task.CreateUsingQuantity(t1, t2, -200);
            Task task3 = Task.CreateUsingQuantity(t3, t4, -100);
            Task task4 = Task.CreateUsingQuantity(t4, t5, -200);
            Task task5 = Task.CreateUsingQuantity(t6, t7, -100);
            Task task6 = Task.CreateUsingQuantity(t7, t8, -200);
            Task task7 = Task.CreateUsingQuantity(t7, t8, 300);
            taskCollection.Add(task1);
            taskCollection.Add(task2);
            taskCollection.Add(task3);
            taskCollection.Add(task4);
            taskCollection.Add(task5);
            taskCollection.Add(task6);
            taskCollection.Add(task7);

            // Act
            IEnumerable<ITask> newCollection = taskCollection.TryToMakeTasksComply(minLevel, maxLevel, initialLevel);

            var correctCollection = new Collection<ITask>();
            correctCollection.Add(task1);
            correctCollection.Add(task2.ChangeTimes(t2));
            correctCollection.Add(task3);
            correctCollection.Add(task4.ChangeTimes(t5));
            correctCollection.Add(task5.ChangeTimes(t7));
            correctCollection.Add(task6);
            correctCollection.Add(task7);

            Assert.Equal(correctCollection, newCollection);
        }

        [TestMethod]
        public void TryToMakeTasksComply_MixedData_ReturnsCorrectCollection5()
        {
            double minLevel = 100;
            double maxLevel = 200;
            double initialLevel = 100;
            var taskCollection = new Collection<ITask>();
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(1);
            DateTime t3 = t1.AddHours(2);
            DateTime t4 = t1.AddHours(3);
            DateTime t5 = t1.AddHours(4);
            DateTime t6 = t1.AddHours(5);
            DateTime t7 = t1.AddHours(6);
            DateTime t8 = t1.AddHours(7);

            initialLevel = 150;
            Task task1 = Task.CreateUsingQuantity(t2, t3, -150);
            Task task2 = Task.CreateUsingQuantity(t2.AddHours(0.5), t3.AddHours(0.5), -100);
            Task task3 = Task.CreateUsingQuantity(t3, t4, -50);
            Task task4 = Task.CreateUsingQuantity(t4, t5, 50);
            Task task5 = Task.CreateUsingQuantity(t5, t6, 50);
            Task task6 = Task.CreateUsingQuantity(t5, t7, 100);
            Task task7 = Task.CreateUsingQuantity(t7, t8, 150);
            taskCollection.Add(task1);
            taskCollection.Add(task2);
            taskCollection.Add(task3);
            taskCollection.Add(task4);
            taskCollection.Add(task5);
            taskCollection.Add(task6);
            taskCollection.Add(task7);

            // Act
            IEnumerable<ITask> newCollection = taskCollection.TryToMakeTasksComply(minLevel, maxLevel, initialLevel);

            var correctCollection = new Collection<ITask>();

            correctCollection.Add(task1.ChangeTimes(t4.AddMinutes(30)));
            correctCollection.Add(task2.ChangeTimes(t6.AddMinutes(20)));
            correctCollection.Add(task3.ChangeTimes(t6.AddMinutes(20)));
            correctCollection.Add(task4);
            correctCollection.Add(task5);
            correctCollection.Add(task6);
            correctCollection.Add(task7);

            // OLD TEST DATA
            //correctCollection.Add(task1.ChangeTimes(t5));
            //correctCollection.Add(task2.ChangeTimes(t6));
            //correctCollection.Add(task3.ChangeTimes(t7));
            //correctCollection.Add(task4);
            //correctCollection.Add(task5);
            //correctCollection.Add(task6);
            //correctCollection.Add(task7);

            Assert.Equal(correctCollection, newCollection);
        }

        [TestMethod]
        public void TryToMakeTasksComply_MixedData_ReturnsCorrectCollection6()
        {
            // Arrange
            double minLevel = 100;
            double maxLevel = 1000;
            double initialLevel = 100;
            var taskCollection = new Collection<ITask>();
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(1);
            DateTime t3 = t1.AddHours(2);
            DateTime t4 = t3.AddHours(10);
            Task task1 = Task.CreateUsingQuantityPerHour(t1, t2, 10000); // NOTE: Using QuantityPerHour here.
            Task task2 = Task.CreateUsingQuantityPerHour(t3, t4, -1000);
            taskCollection.Add(task1);
            taskCollection.Add(task2);

            // Act
            IEnumerable<ITask> newCollection = taskCollection.TryToMakeTasksComply(minLevel, maxLevel, initialLevel);

            // Assert

            // Assert
            Assert.Null(newCollection);
        }

        #endregion
    }
}
