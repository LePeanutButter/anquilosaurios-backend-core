using aquilosaurios_backend_core.Domain;
using FluentAssertions;
using Xunit;

namespace aquilosaurios_backend_core.Tests.Domain.Tests
{
    public class ValueObjectsTests
    {
        #region CosmeticData Tests

        [Fact]
        public void CosmeticData_DefaultConstructor_ShouldInitializeEmptyProperties()
        {
            var cosmetic = new CosmeticData();

            cosmetic.ItemId.Should().BeEmpty();
            cosmetic.Rarity.Should().BeEmpty();
        }

        [Fact]
        public void CosmeticData_ParameterizedConstructor_ShouldSetProperties()
        {
            var itemId = "skin_001";
            var rarity = "epic";

            var cosmetic = new CosmeticData(itemId, rarity);

            cosmetic.ItemId.Should().Be(itemId);
            cosmetic.Rarity.Should().Be(rarity);
        }

        #endregion

        #region SubscriptionData Tests

        [Fact]
        public void SubscriptionData_DefaultConstructor_ShouldInitializeEmptyProperties()
        {
            var subscription = new SubscriptionData();

            subscription.Plan.Should().BeEmpty();
            subscription.ExpirationDate.Should().Be(default);
        }

        [Fact]
        public void SubscriptionData_ParameterizedConstructor_ShouldSetPlan()
        {
            var plan = "AD_FREE_MONTHLY";

            var subscription = new SubscriptionData(plan);

            subscription.Plan.Should().Be(plan);
            subscription.ExpirationDate.Should().Be(default);
        }

        #endregion

        #region UserAuditEntry Tests

        [Fact]
        public void UserAuditEntry_DefaultConstructor_ShouldInitializeDefaultUserId()
        {
            var audit = new UserAuditEntry();

            audit.UserId.Should().Be(Guid.Empty);
        }

        [Fact]
        public void UserAuditEntry_ParameterizedConstructor_ShouldSetUserId()
        {
            var userId = Guid.NewGuid();

            var audit = new UserAuditEntry(userId);

            audit.UserId.Should().Be(userId);
        }

        #endregion

        #region PurchaseAuditEntry Tests

        [Fact]
        public void PurchaseAuditEntry_DefaultConstructor_ShouldInitializeDefaults()
        {
            var purchaseAudit = new PurchaseAuditEntry();

            purchaseAudit.PurchaseId.Should().Be(Guid.Empty);
            purchaseAudit.Amount.Should().Be(0);
            purchaseAudit.Data.Should().BeEmpty();
        }

        [Fact]
        public void PurchaseAuditEntry_ParameterizedConstructor_ShouldSetProperties()
        {
            var purchaseId = Guid.NewGuid();
            var amount = 9.99;
            var data = "{\"item\":\"skin_001\"}";

            var purchaseAudit = new PurchaseAuditEntry(purchaseId, amount, data);

            purchaseAudit.PurchaseId.Should().Be(purchaseId);
            purchaseAudit.Amount.Should().Be(amount);
            purchaseAudit.Data.Should().Be(data);
        }

        #endregion

        #region MatchSummaryAuditEntry Tests

        [Fact]
        public void MatchSummaryAuditEntry_DefaultConstructor_ShouldInitializeDefaultMatchId()
        {
            var audit = new MatchSummaryAuditEntry();

            audit.MatchId.Should().Be(Guid.Empty);
        }

        [Fact]
        public void MatchSummaryAuditEntry_ParameterizedConstructor_ShouldSetMatchId()
        {
            var matchId = Guid.NewGuid();

            var audit = new MatchSummaryAuditEntry(matchId);

            audit.MatchId.Should().Be(matchId);
        }

        #endregion
    }
}
