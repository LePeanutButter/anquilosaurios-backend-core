using aquilosaurios_backend_core.API;
using Xunit;

namespace aquilosaurios_backend_core.Tests.API.Tests
{
    public class PaymentIntentTests
    {
        [Fact]
        public void PaymentIntent_ShouldInitializeWithGivenValues()
        {
            var purchaseId = Guid.NewGuid();
            var amount = 100.0;
            var currency = "USD";
            var paymentToken = "sampleToken";

            var paymentIntent = new PaymentIntent(purchaseId, amount, currency, paymentToken);

            Assert.Equal(purchaseId, paymentIntent.PurchaseId);
            Assert.Equal(amount, paymentIntent.Amount);
            Assert.Equal(currency, paymentIntent.Currency);
            Assert.Equal(paymentToken, paymentIntent.PaymentToken);
            Assert.Null(paymentIntent.IdempotencyKey);
            Assert.Null(paymentIntent.Metadata);
        }

        [Fact]
        public void PaymentIntent_ShouldAllowSettingIdempotencyKey()
        {
            var paymentIntent = new PaymentIntent(Guid.NewGuid(), 100.0, "USD", "sampleToken");

            paymentIntent.IdempotencyKey = "sampleKey";

            Assert.Equal("sampleKey", paymentIntent.IdempotencyKey);
        }

        [Fact]
        public void PaymentIntent_ShouldAllowSettingMetadata()
        {
            var paymentIntent = new PaymentIntent(Guid.NewGuid(), 100.0, "USD", "sampleToken");

            paymentIntent.Metadata = new Dictionary<string, string> { { "key", "value" } };

            Assert.NotNull(paymentIntent.Metadata);
            Assert.Contains("key", paymentIntent.Metadata);
        }
    }

    public class FiltersDTOTests
    {
        [Fact]
        public void UserFiltersDTO_ShouldInitializeWithDefaultValues()
        {
            var userFilters = new UserFiltersDTO();

            Assert.Equal(Guid.Empty, userFilters.Id);
            Assert.Equal(1, userFilters.Page);
            Assert.Equal(10, userFilters.Size);
            Assert.Null(userFilters.StartDate);
            Assert.Null(userFilters.EndDate);
        }

        [Fact]
        public void UserFiltersDTO_ShouldAllowSettingStartDateAndEndDate()
        {
            var userFilters = new UserFiltersDTO();
            var startDate = DateTime.Now.AddDays(-1);
            var endDate = DateTime.Now;

            userFilters.StartDate = startDate;
            userFilters.EndDate = endDate;

            Assert.Equal(startDate, userFilters.StartDate);
            Assert.Equal(endDate, userFilters.EndDate);
        }
    }

    public class UserFiltersDTOTests
    {
        [Fact]
        public void UserFiltersDTO_ShouldInitializeWithDefaultValues()
        {
            var userFilters = new UserFiltersDTO();

            Assert.Null(userFilters.Name);
            Assert.Null(userFilters.Email);
            Assert.Null(userFilters.Username);
            Assert.Null(userFilters.ActiveStatus);
            Assert.Null(userFilters.AdminPrivilege);
        }

        [Fact]
        public void UserFiltersDTO_ShouldAllowSettingProperties()
        {
            var userFilters = new UserFiltersDTO
            {
                Name = "John Doe",
                Email = "john.doe@example.com",
                Username = "johndoe",
                ActiveStatus = true,
                AdminPrivilege = false
            };

            Assert.Equal("John Doe", userFilters.Name);
            Assert.Equal("john.doe@example.com", userFilters.Email);
            Assert.Equal("johndoe", userFilters.Username);
            Assert.True(userFilters.ActiveStatus);
            Assert.False(userFilters.AdminPrivilege);
        }
    }

    public class LoginDTOTests
    {
        [Fact]
        public void LoginDTO_ShouldInitializeWithGivenValues()
        {
            var identifier = "john.doe@example.com";
            var rawPassword = "password123";

            var loginDTO = new LoginDTO
            {
                Identifier = identifier,
                RawPassword = rawPassword
            };

            Assert.Equal(identifier, loginDTO.Identifier);
            Assert.Equal(rawPassword, loginDTO.RawPassword);
        }
    }

    public class AuthCredentialDTOTests
    {
        [Fact]
        public void AuthCredentialDTO_ShouldInitializeWithGivenCredential()
        {
            var credential = "someCredential";

            var authCredential = new AuthCredentialDTO(credential);

            Assert.Equal(credential, authCredential.Credential);
        }
    }

    public class UserUpdateDTOTests
    {
        [Fact]
        public void UserUpdateDTO_ShouldInitializeWithDefaultValues()
        {
            var userUpdateDTO = new UserUpdateDTO();

            Assert.Null(userUpdateDTO.Name);
            Assert.Null(userUpdateDTO.RawPassword);
        }

        [Fact]
        public void UserUpdateDTO_ShouldAllowSettingProperties()
        {
            var userUpdateDTO = new UserUpdateDTO
            {
                Name = "John Doe",
                RawPassword = "newPassword"
            };

            Assert.Equal("John Doe", userUpdateDTO.Name);
            Assert.Equal("newPassword", userUpdateDTO.RawPassword);
        }
    }

    public class UserRegisterDTOTests
    {
        [Fact]
        public void UserRegisterDTO_ShouldInitializeWithDefaultValues()
        {
            var userRegisterDTO = new UserRegisterDTO();

            Assert.Null(userRegisterDTO.Name);
            Assert.Null(userRegisterDTO.Username);
            Assert.Null(userRegisterDTO.Email);
            Assert.Null(userRegisterDTO.RawPassword);
        }

        [Fact]
        public void UserRegisterDTO_ShouldAllowSettingProperties()
        {
            var userRegisterDTO = new UserRegisterDTO
            {
                Name = "Jane Doe",
                Username = "janedoe",
                Email = "jane.doe@example.com",
                RawPassword = "password123"
            };

            Assert.Equal("Jane Doe", userRegisterDTO.Name);
            Assert.Equal("janedoe", userRegisterDTO.Username);
            Assert.Equal("jane.doe@example.com", userRegisterDTO.Email);
        }
    }
}