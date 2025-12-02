using System.Security.Cryptography;
using System.Text;
using aquilosaurios_backend_core.Domain.Models;
using aquilosaurios_backend_core.Shared;
using FluentAssertions;
using Xunit;

namespace aquilosaurios_backend_core.Tests.Domain.Tests.Models
{
    public class UserTests
    {
        [Fact]
        public void Achievement_ShouldInitializePropertiesCorrectly()
        {
            var now = DateTime.UtcNow;
            var achievement = new Achievement("First Kill", "Win your first match", now);

            achievement.Name.Should().Be("First Kill");
            achievement.Description.Should().Be("Win your first match");
            achievement.UnlockedDate.Should().Be(now);
        }

        [Fact]
        public void ProfileStats_ShouldInitializeWithDefaults()
        {
            var stats = new ProfileStats();

            stats.PlayedMatches.Should().Be(0);
            stats.WinnedMatches.Should().Be(0);
            stats.Deaths.Should().Be(0);
        }

        [Fact]
        public void ProfileStats_Setters_ShouldUpdateValues()
        {
            var stats = new ProfileStats();

            stats.SetPlayedMatches(10);
            stats.SetWinnedMatches(5);
            stats.SetDeaths(3);

            stats.PlayedMatches.Should().Be(10);
            stats.WinnedMatches.Should().Be(5);
            stats.Deaths.Should().Be(3);
        }

        [Fact]
        public void User_LocalConstructor_ShouldHashPassword()
        {
            var rawPassword = "password123";
            var user = new User("John", "john@example.com", "johnny", rawPassword);

            var expectedHash = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(rawPassword)));

            user.Name.Should().Be("John");
            user.Email.Should().Be("john@example.com");
            user.Username.Should().Be("johnny");
            user.PasswordHash.Should().Be(expectedHash);
            user.AuthProvider.Should().Be(AuthProvider.LOCAL);
        }

        [Fact]
        public void User_ProviderConstructor_ShouldSetAuthProvider()
        {
            var user = new User("Jane", AuthProvider.GOOGLE);

            user.Name.Should().Be("Jane");
            user.AuthProvider.Should().Be(AuthProvider.GOOGLE);
            user.PasswordHash.Should().BeNull();
        }

        [Fact]
        public void User_Setters_ShouldUpdateProperties()
        {
            var user = new User("Alice", AuthProvider.LOCAL);

            user.SetName("AliceUpdated");
            user.SetAuthProvider(AuthProvider.FACEBOOK);
            user.SetIsAccountActive(false);
            user.SetIsAdmin(true);

            user.Name.Should().Be("AliceUpdated");
            user.AuthProvider.Should().Be(AuthProvider.FACEBOOK);
            user.IsAccountActive.Should().BeFalse();
            user.IsAdmin.Should().BeTrue();
        }

        [Fact]
        public void User_AddCollections_ShouldAvoidDuplicates()
        {
            var user = new User("Bob", AuthProvider.LOCAL);
            var matchId = Guid.NewGuid();
            var purchaseId = Guid.NewGuid();
            var achievement = new Achievement("Champion", "Win 10 matches", DateTime.UtcNow);

            user.AddMatchId(matchId);
            user.AddMatchId(matchId);
            user.AddPurchaseId(purchaseId);
            user.AddPurchaseId(purchaseId);
            user.AddAchievement(achievement);
            user.AddAchievement(achievement);

            user.MatchesIds.Should().HaveCount(1).And.Contain(matchId);
            user.PurchasesIds.Should().HaveCount(1).And.Contain(purchaseId);
            user.Achievements.Should().HaveCount(2);
        }

        [Fact]
        public void User_StateChecks_ShouldReturnCorrectValues()
        {
            var user = new User("Sam", AuthProvider.LOCAL)
            {
                IsAdmin = true,
                IsEmailVerified = true,
                IsAccountActive = true
            };

            user.IsAdminUser().Should().BeTrue();
            user.IsEmailVerifiedUser().Should().BeTrue();
            user.IsAccountActiveUser().Should().BeTrue();
        }

        [Fact]
        public void User_VerifyEmail_ShouldSetIsEmailVerified()
        {
            var user = new User("Sam", AuthProvider.LOCAL);

            user.VerifyEmail();

            user.IsEmailVerified.Should().BeTrue();
        }

        [Fact]
        public void User_SetPassword_ShouldUpdateHash()
        {
            var user = new User("Test", AuthProvider.LOCAL);
            var newPassword = "newpass456";

            user.SetPassword(newPassword);
            var expectedHash = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(newPassword)));

            user.PasswordHash.Should().Be(expectedHash);
        }
    }
}
