using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClientImport.Tests
{
    [TestClass]
    public class ImporterTests
    {
        [TestMethod]
        public void ShouldLoadTextFileData()
        {
            //Arrange
            var asm = Assembly.GetExecutingAssembly();

            //Act
            var importer = new Importer(asm.GetManifestResourceStream("ClientImport.Tests.records.txt"), ',');

            //Assert
            var data = importer.Data.ToList();

            Assert.AreEqual("Adrie", data.First().FirstName);
            Assert.AreEqual("Ballingall", data.First().Surname);
            Assert.AreEqual("111", data.First().Age);
        }

        [TestMethod]
        public void ShouldLoadTextFileDataEvenWhenFileLayoutIsChanged()
        {
            //Arrange
            var asm = Assembly.GetExecutingAssembly();

            //Act
            var importer = new Importer(asm.GetManifestResourceStream("ClientImport.Tests.recordsColumnChange.txt"), ',');

            //Assert
            var data = importer.Data.ToList();

            Assert.AreEqual("Adrie", data.First().FirstName);
            Assert.AreEqual("Ballingall", data.First().Surname);
            Assert.AreEqual("111", data.First().Age);
        }
    }
}
