using aquilosaurios_backend_core.Shared;
using FluentAssertions;
using Xunit;

namespace aquilosaurios_backend_core.Tests.Shared.Tests
{
    public class AnquilosauriosExceptionTests
    {
        [Fact]
        public void Exception_ShouldStoreMessageCorrectly()
        {
            string errorMessage = "Something went wrong";
            var exception = new AnquilosauriosException(errorMessage);
            exception.Message.Should().Be(errorMessage);
        }

        [Fact]
        public void ExceptionConstants_ShouldHaveExpectedValues()
        {
            AnquilosauriosException.INVALID_DATA_FORMAT
                .Should().Be("Invalid data format in fields like dataJson or externalPaymentId");

            AnquilosauriosException.MISSING_FIELD
                .Should().Be("Missing required field");

            AnquilosauriosException.INVALID_ENUM_VALUE
                .Should().Be("Invalid value for enum type");

            AnquilosauriosException.DUPLICATE_IDEMPOTENCY_KEY
                .Should().Be("Duplicate idempotency key");

            AnquilosauriosException.UNAUTHORIZED_ACCESS
                .Should().Be("User is not authorized to access this resource");

            AnquilosauriosException.INACTIVE_ACCOUNT
                .Should().Be("User account is inactive or email not verified");

            AnquilosauriosException.INVALID_MATCH_DATA
                .Should().Be("Match data is inconsistent or corrupt");

            AnquilosauriosException.INVALID_POSITION
                .Should().Be("Position coordinates are invalid");
        }

        [Fact]
        public void Exception_ShouldBeAssignableToException()
        {
            var exception = new AnquilosauriosException("Test");
            exception.Should().BeAssignableTo<Exception>();
        }
    }
}
