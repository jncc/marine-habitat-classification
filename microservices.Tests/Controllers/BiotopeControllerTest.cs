using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using FluentAssertions;
using microservices.Controllers;
using microservices.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Moq;

namespace microservices.Tests.Controllers
{
    class BiotopeControllerTest
    {
        [Test]
        public void GetBiotopeByKeyTest()
        {
            // Arrange
            var biotopeDbMock = new Mock<BiotopeDB>();

            BiotopeController controller = new BiotopeController();

            var expectedBiotope = new Mock<DbSet<WEB_BIOTOPE>>();
//            expectedBiotope.BIOTOPE_KEY = "JNCCMNCR0000TEST";
//            expectedBiotope.DESCRIPTION = "test biotope";

            biotopeDbMock.Setup(m => m.WEB_BIOTOPE).Returns(expectedBiotope.Object);

            // Act
            JsonResult result = controller.Index("JNCCMNCR0000TEST");

            // Assert
            Assert.IsNotNull(result);

            var actualBiotope = JsonConvert.DeserializeObject<WEB_BIOTOPE>(JArray.Parse(result.ToString()).First.ToString());
            actualBiotope.BIOTOPE_KEY.Should().Be("JNCCMNCR0000TEST");
            actualBiotope.DESCRIPTION.Should().Be("test biotope");
        }
    }
}
