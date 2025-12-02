using aquilosaurios_backend_core.API.Controllers;
using Xunit;

namespace aquilosaurios_backend_core.Tests.API.Controllers
{
    public class ControllerResponseTests
    {
        [Fact]
        public void DefaultConstructor_SetsPropertiesToDefault()
        {
            var response = new ControllerResponse<string>();

            Assert.Null(response.Data);
            Assert.Null(response.Message);
        }

        [Fact]
        public void Constructor_WithDataAndMessage_SetsPropertiesCorrectly()
        {
            var data = "TestData";
            var message = "Custom message";

            var response = new ControllerResponse<string>(data, message);

            Assert.Equal(data, response.Data);
            Assert.Equal(message, response.Message);
        }

        [Fact]
        public void Constructor_WithMessageOnly_SetsMessageAndNullData()
        {
            var message = "Only message";

            var response = new ControllerResponse<string>(message);

            Assert.Null(response.Data);
            Assert.Equal(message, response.Message);
        }

        [Fact]
        public void Constructor_WithNullDataAndDefaultMessage_SetsMessageToDefault()
        {
            string? data = null;

            var response = new ControllerResponse<string>(data);

            Assert.Null(response.Data);
            Assert.Equal("OK", response.Message);
        }

        [Fact]
        public void Constructor_WithValueTypeData_SetsPropertiesCorrectly()
        {
            var data = 42;
            var message = "Value type test";

            var response = new ControllerResponse<int>(data, message);

            Assert.Equal(data, response.Data);
            Assert.Equal(message, response.Message);
        }
    }
}
