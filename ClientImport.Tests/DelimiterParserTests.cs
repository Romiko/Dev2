using System.Linq;
using ClientImport.Parsers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClientImport.Tests
{
    [TestClass]
    public class DelimiterParserTests
    {
        [TestMethod]
        public void ShouldLoadTextDataIntoDynamicObject()
        {
            //Arrange
            var content = "Name|Surname|Age\n";
            content += "Romiko|Derbynew|34";

            //Act
            var result = (from dynamic c in new DelimiterParser(content, '|')
                         select c).ToList();
            //Assert
            Assert.AreEqual("Romiko", result.ToList().First().Name);
            Assert.AreEqual("Derbynew", result.ToList().First().Surname);
            Assert.AreEqual("34", result.ToList().First().Age);
        }

        [TestMethod]
        public void LeadingSpacesinColumnNamesAreIgnored()
        {
            //Arrange
            var content = "Name|         Surname|Age\n";
            content += "Romiko|Derbynew|34";

            //Act
            var result = (from dynamic c in new DelimiterParser(content, '|')
                         select c).ToList();
            //Assert
            Assert.AreEqual("Derbynew", result.ToList().First().Surname);
        }

        [TestMethod]
        public void TrailingSpacesinColumnNamesAreIgnored()
        {
            //Arrange
            var content = "Name|Surname    |Age\n";
            content += "Romiko|Derbynew|34";

            //Act
            var result = (from dynamic c in new DelimiterParser(content, '|')
                         select c).ToList();
            //Assert
            Assert.AreEqual("Derbynew", result.ToList().First().Surname);
        }
    }
}
