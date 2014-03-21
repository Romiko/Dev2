using System;
using System.Linq;
using System.Reflection;
using ClientImport.DTO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClientImport.Tests
{
    [TestClass]
    public class PersonStorageTests
    {
        [TestMethod]
        public void ContainABinarySeachTreeOnConstruction()
        {
            // Arrange
            var storage = new PersonStorage();

            // Assert
            Assert.IsNotNull(storage.Data);
        }

        [TestMethod]
        public void DefaultSortKeyIsSurnameFirstNameAge()
        {
            // Arrange
            var storage = new PersonStorage();

            // Assert
           Assert.AreEqual(SortKey.SurnameFirstNameAge, storage.DefaultSortKey);
        }

        [TestMethod]
        public void InitilisedSortKeyIsSurnameFirstName()
        {
            // Arrange
            var storage = new PersonStorage(SortKey.SurnameFirstName);

            // Assert
            Assert.AreEqual(SortKey.SurnameFirstName, storage.DefaultSortKey);
        }

        [TestMethod]
        public void DataIsSortedUsingDefaultSortKey()
        {
            // Arrange
            var storage = new PersonStorage();
            var person1 = new Person {Age = 1, FirstName = "Alpha", Surname = "Beta"};
            var person2 = new Person { Age = 2, FirstName = "Aleph", Surname = "Bate" };
            var person3 = new Person { Age = 0, FirstName = "Aleph", Surname = "Bates" };

            //Act
            storage.Add(person1);
            storage.Add(person3);
            storage.Add(person2);

            // Assert
            Assert.AreEqual(storage.Data.ToList().First().KeyValue.Value, person2);
            Assert.AreEqual(storage.Data.ToList().Last().KeyValue.Value, person1);
        }

        [TestMethod]
        public void DataIsSortedUsingExplicitSortKey()
        {
            // Arrange
            var storage = new PersonStorage(SortKey.SurnameFirstName);
            var person1 = new Person { Age = 1, FirstName = "Alpha", Surname = "Beta" };
            var person2 = new Person { Age = 2, FirstName = "Aleph", Surname = "Bite" };
            var person3 = new Person { Age = 0, FirstName = "Aleph", Surname = "Bate" };

            //Act
            storage.Add(person1);
            storage.Add(person2);
            storage.Add(person3);

            // Assert
            Assert.AreEqual(storage.Data.ToList().First().KeyValue.Value, person3);
            Assert.AreEqual(storage.Data.ToList().Last().KeyValue.Value, person2);
        }

        [TestMethod]
        public void AllDataIsLoadedFromTextFileIntoPersonBinaryTree()
        {
            // Arrange
            var storage = new PersonStorage(SortKey.SurnameFirstName);
            var asm = Assembly.GetExecutingAssembly();
            var importer = new Importer(asm.GetManifestResourceStream("ClientImport.Tests.records.txt"), ',');

            //Act
            storage.Import(importer.Data);

            // Assert
            Assert.AreEqual(49817, storage.Data.Count());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void WhenDataIsLoadedAndConatainsDuplicatesAnExceptionIsThrownWhenIgnoreIsTurnedOff()
        {
            // Arrange
            var storage = new PersonStorage(SortKey.SurnameFirstName);
            var asm = Assembly.GetExecutingAssembly();
            var importer = new Importer(asm.GetManifestResourceStream("ClientImport.Tests.records.txt"), ',');

            //Act
            storage.Import(importer.Data, false);
        }

        [TestMethod]
        public void FirstValueInTree()
        {
            // Arrange
            var storage = new PersonStorage(SortKey.SurnameFirstName);
            var asm = Assembly.GetExecutingAssembly();
            var importer = new Importer(asm.GetManifestResourceStream("ClientImport.Tests.records.txt"), ',');

            //Act
            storage.Import(importer.Data);

            // Assert
            var first = storage.Data.First();
            Assert.AreEqual("Abbot", first.KeyValue.Value.Surname);
            Assert.AreEqual("Abbas", first.KeyValue.Value.FirstName);
        }

        [TestMethod]
        public void LastValueInTree()
        {
            // Arrange
            var storage = new PersonStorage(SortKey.SurnameFirstName);
            var asm = Assembly.GetExecutingAssembly();
            var importer = new Importer(asm.GetManifestResourceStream("ClientImport.Tests.records.txt"), ',');

            //Act
            storage.Import(importer.Data);

            // Assert
            var last = storage.Data.Last();
            Assert.AreEqual("Zunkel", last.KeyValue.Value.Surname);
            Assert.AreEqual("Nj", last.KeyValue.Value.FirstName);
        }
    }
}
