using System;
using System.Linq;
using System.Reflection;
using ClientImport.DTO;
using ClientImport.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClientImport.Tests
{
    [TestClass]
    public class PersonRepositoryTests
    {
        const char Delimiter = ',';
        [TestMethod]
        public void ContainABinarySeachTreeOnConstruction()
        {
            // Arrange
            var repository = new PersonRepository();

            // Assert
            Assert.IsNotNull(repository.Data);
        }

        [TestMethod]
        public void DefaultSortKeyIsSurnameFirstNameAge()
        {
            // Arrange
            var repository = new PersonRepository();

            // Assert
           Assert.AreEqual(SortKey.SurnameFirstNameAge, repository.DefaultSortKey);
        }

        [TestMethod]
        public void InitilisedSortKeyIsSurnameFirstName()
        {
            // Arrange
            var repository = new PersonRepository(SortKey.SurnameFirstName);

            // Assert
            Assert.AreEqual(SortKey.SurnameFirstName, repository.DefaultSortKey);
        }

        [TestMethod]
        public void DataIsSortedUsingDefaultSortKey()
        {
            // Arrange
            var repository = new PersonRepository();
            var person1 = new Person {Age = 1, FirstName = "Alpha", Surname = "Beta"};
            var person2 = new Person { Age = 2, FirstName = "Aleph", Surname = "Bate" };
            var person3 = new Person { Age = 0, FirstName = "Aleph", Surname = "Bates" };

            //Act
            repository.Add(person1);
            repository.Add(person3);
            repository.Add(person2);

            // Assert
            Assert.AreEqual(repository.Data.ToList().First().KeyValue.Value, person2);
            Assert.AreEqual(repository.Data.ToList().Last().KeyValue.Value, person1);
        }

        [TestMethod]
        public void DataIsSortedUsingExplicitSortKey()
        {
            // Arrange
            var repository = new PersonRepository(SortKey.SurnameFirstName);
            var person1 = new Person { Age = 1, FirstName = "Alpha", Surname = "Beta" };
            var person2 = new Person { Age = 2, FirstName = "Aleph", Surname = "Bite" };
            var person3 = new Person { Age = 0, FirstName = "Aleph", Surname = "Bate" };

            //Act
            repository.Add(person1);
            repository.Add(person2);
            repository.Add(person3);

            // Assert
            Assert.AreEqual(repository.Data.ToList().First().KeyValue.Value, person3);
            Assert.AreEqual(repository.Data.ToList().Last().KeyValue.Value, person2);
        }

        [TestMethod]
        public void AllDataIsLoadedFromTextFileIntoPersonBinaryTree()
        {
            // Arrange
            var repository = new PersonRepository(SortKey.SurnameFirstName);
            var asm = Assembly.GetExecutingAssembly();
            var importer = new Importer(asm.GetManifestResourceStream("ClientImport.Tests.records.txt"), Delimiter);

            //Act
            repository.Import(importer.Data);

            // Assert
            Assert.AreEqual(49817, repository.Data.Count());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void WhenDataIsLoadedAndConatainsDuplicatesAnExceptionIsThrownWhenIgnoreIsTurnedOff()
        {
            // Arrange
            var repository = new PersonRepository(SortKey.SurnameFirstName);
            var asm = Assembly.GetExecutingAssembly();
            var importer = new Importer(asm.GetManifestResourceStream("ClientImport.Tests.records.txt"), Delimiter);

            //Act
            repository.Import(importer.Data, false);
        }

        [TestMethod]
        public void FirstValueInTree()
        {
            // Arrange
            var repository = new PersonRepository(SortKey.SurnameFirstName);
            var asm = Assembly.GetExecutingAssembly();
            var importer = new Importer(asm.GetManifestResourceStream("ClientImport.Tests.records.txt"), Delimiter);

            //Act
            repository.Import(importer.Data);

            // Assert
            var first = repository.Data.First();
            Assert.AreEqual("Abbot", first.KeyValue.Value.Surname);
            Assert.AreEqual("Abbas", first.KeyValue.Value.FirstName);
        }

        [TestMethod]
        public void LastValueInTree()
        {
            // Arrange
            var repository = new PersonRepository(SortKey.SurnameFirstName);
            var asm = Assembly.GetExecutingAssembly();
            var importer = new Importer(asm.GetManifestResourceStream("ClientImport.Tests.records.txt"), Delimiter);

            //Act
            repository.Import(importer.Data);

            // Assert
            var last = repository.Data.Last();
            Assert.AreEqual("Zunkel", last.KeyValue.Value.Surname);
            Assert.AreEqual("Nj", last.KeyValue.Value.FirstName);
        }
    }
}
