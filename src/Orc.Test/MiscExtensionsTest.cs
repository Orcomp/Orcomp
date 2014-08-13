﻿namespace Orc.Tests
{
    using System;
    using System.Collections.Generic;

    using NUnit.Framework;

    using Orc.Extensions;

    [TestFixture]
    public class MiscExtensionsTest
    {
        [Test]
        public void AddOrUpdate_KeyExists_KeyIsNotAddedAgain()
        {
            // Arrange
            var dictionary = new Dictionary<string, int> { };

            const string TestKey = "testKey";
            const int FirstValue = 1234;
            dictionary.Add(TestKey, FirstValue);

            // Act
            const int SecondValue = 4321;
            dictionary.AddOrUpdate(TestKey, SecondValue);

            // Assert
            Assert.AreEqual(1, dictionary.Count);
        }

        [Test]
        public void AddOrUpdate_KeyExists_ValueIsUpdated()
        {
            // Arrange
            var dictionary = new Dictionary<string, int>();
            const string TestKey = "testKey";
            const int FirstValue = 1234;
            const int SecondValue = 4321;
            dictionary.Add(TestKey, FirstValue);

            // Act
            dictionary.AddOrUpdate(TestKey, SecondValue);
            int dictionaryValue;
            dictionary.TryGetValue(TestKey, out dictionaryValue);

            // Assert
            Assert.AreEqual(SecondValue, dictionaryValue);
        }

        [Test]
        public void AddOrUpdate_KeyDoesNotExist_NewKeyIsAdded()
        {
            // Arrange
            var dictionary = new Dictionary<string, int> { };

            const string TestKey = "testKey";
            const int FirstValue = 1234;
            dictionary.Add(TestKey, FirstValue);

            // Act
            const int SecondValue = 4321;
            dictionary.AddOrUpdate("newKey", SecondValue);

            // Assert
            Assert.AreEqual(2, dictionary.Count);
        }

        [Test]
        public void FlattenToString_WhenExecutedOnAnIntegerArrayWithSeparatorSetToComma_ReturnsAllValuesInACommaSeparatedString()
        {
            IEnumerable<int> source = new int[] { 1, 2, 3, 4, 5 };
            const string Seperator = ",";
            const string Expected = "1,2,3,4,5";

            string actual = source.FlattenToString<int>(Seperator);
            Assert.AreEqual(Expected, actual);
        }

        [Test]
        public void FlattenToString_WhenExecutedOnAStringArrayWithSeparatorSetToTwoCharacters_ReturnsAllValuesInADelimeteredString()
        {
            IEnumerable<string> source = new string[] { "1", "2", "3", "4", "5" };
            const string Seperator = "~ ";
            const string Expected = "1~ 2~ 3~ 4~ 5";

            string actual = source.FlattenToString<string>(Seperator);
            Assert.AreEqual(Expected, actual);
        }

        [Test]
        public void Singular_WhenExcutedAgainstACollectionThatContainsASingleElement_ReturnsTheCollection()
        {
            // Arrange
            int value = 1234;
            IEnumerable<int> source = new int[] { value };
            const string Info = "info string if required for message";
            int expected = value;

            // Act
            int actual = source.Singular(Info);
            
            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Singular_WhenExcutedAgainstACollectionThatContainsMoreThanOneElement_ThrowsArgumentException()
        {
            IEnumerable<int> source = new int[] { 1234, 4321 };
            const string Info = "info string if required for message";
            
            int actual;

            Assert.Throws<ArgumentException>(() => actual = source.Singular(Info));
        }

        [Test]
        public void Singular_WhenExcutedAgainstAnEmptyCollection_ThrowsArgumentException()
        {
            IEnumerable<int> source = new int[] { };
            const string Info = "info string if required for message";

            int actual;

            Assert.Throws<ArgumentException>(() => actual = source.Singular(Info));
        }
    }
}
