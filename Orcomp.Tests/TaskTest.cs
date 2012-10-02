using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Orcomp.Entities;

using Assert = Xunit.Assert;

namespace Orcomp.Tests
{
    [TestClass]
    public class TaskTest
    {
        [TestMethod]
        public void TaskType_IfQuantityEqualToZero_ReturnsProducingType()
        {
            // Arrange
            var task = Task.CreateUsingQuantity(DateTime.Now, DateTime.Now.AddDays(2), 0F);

            // Act
            TaskType taskType = task.TaskType;

            // Assert
            Assert.Equal(TaskType.Produce, taskType);
        }

        [TestMethod]
        public void TaskType_IfQuantityGreaterThanZero_ReturnsProducingType()
        {
            // Arrange
            var task = Task.CreateUsingQuantity(DateTime.Now, DateTime.Now.AddDays(2), 1F);

            // Act
            TaskType taskType = task.TaskType;

            // Assert
            Assert.Equal(TaskType.Produce, taskType);
        }

        [TestMethod]
        public void TaskType_IfQuantityLessThanZero_ReturnsConsumingType()
        {
            // Arrange
            var task = Task.CreateUsingQuantity(DateTime.Now, DateTime.Now.AddDays(2), -1F);

            // Act
            TaskType taskType = task.TaskType;

            // Assert
            Assert.Equal(TaskType.Consume, taskType);
        }

        [TestMethod]
        public void GetQuantity_SingleTaskTypeProduceWhenExecuted_ReturnsQuantityProduced()
        {
            // Arrange
            var task = Task.CreateUsingQuantity(DateTime.Now, DateTime.Now.AddDays(2), 1000F);
            var date = DateTime.Now.AddDays(2);

            // Act
            double actual = task.GetQuantity(date);

            // Assert
            Assert.Equal(1000F, actual);
        }

        [TestMethod]
        public void GetQuantity_SingleTaskTypeConsumeWhenExecuted_ReturnsQuantityConsumed()
        {
            // Arrange
            var task = Task.CreateUsingQuantity(DateTime.Now, DateTime.Now.AddHours(1.5), -100F);
            var date = DateTime.Now.AddHours(1.5);

            // Act
            double actual = task.GetQuantity(date);

            // Assert
            Assert.Equal(-100F, actual);
        }


        [TestMethod]
        public void Equality_TwoTasksWithSameValues_ReturnsTrue()
        {
            // Arrange
            var t1 = DateTime.Now;
            var t2 = t1.AddHours( 1 );
            var task1 = Task.CreateUsingQuantity(t1, t2, -100F);
            var task2 = Task.CreateUsingQuantity(t1, t2, -100F);

            // Act
            bool value = task1.Equals( task2 );

            // Assert
            Assert.True(value  );
        }

    }
}
