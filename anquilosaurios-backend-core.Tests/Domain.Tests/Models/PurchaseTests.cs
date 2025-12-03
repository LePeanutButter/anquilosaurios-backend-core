using aquilosaurios_backend_core.Domain.Models;
using aquilosaurios_backend_core.Shared;
using FluentAssertions;
using Xunit;

namespace aquilosaurios_backend_core.Tests.Domain.Tests.Models
{
    public class PurchaseTests
    {
        private Purchase CreateDefaultPurchase()
        {
            return new Purchase(
                PurchaseType.COSMETIC,
                DateTime.UtcNow,
                ProviderName.STRIPE,
                9.99,
                "USD",
                Guid.NewGuid().ToString(),
                "{}"
            )
            {
                ExternalPaymentId = "ext-123"
            };
        }

        [Fact]
        public void Purchase_ShouldInitializePropertiesCorrectly()
        {
            var purchase = CreateDefaultPurchase();

            purchase.Id.Should().NotBe(Guid.Empty);
            purchase.Type.Should().Be(PurchaseType.COSMETIC);
            purchase.PurchaseDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
            purchase.Amount.Should().Be(9.99);
            purchase.Currency.Should().Be("USD");
            purchase.DataJson.Should().Be("{}");
            purchase.IdempotencyKey.Should().NotBeNullOrEmpty();
            purchase.PaymentProviderName.Should().Be(ProviderName.STRIPE);
            purchase.ExternalPaymentId.Should().Be("ext-123");
            purchase.Status.Should().Be(PurchaseStatus.PENDING);
            purchase.ProcessedWebhookEventIds.Should().NotBeNull().And.BeEmpty();
            purchase.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
            purchase.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        }

        [Fact]
        public void IsSubscription_ShouldReturnTrue_WhenTypeIsSubscription()
        {
            var purchase = CreateDefaultPurchase();
            purchase.Type = PurchaseType.SUBSCRIPTION;

            purchase.IsSubscription().Should().BeTrue();
            purchase.IsCosmetic().Should().BeFalse();
        }

        [Fact]
        public void IsCosmetic_ShouldReturnTrue_WhenTypeIsCosmetic()
        {
            var purchase = CreateDefaultPurchase();

            purchase.IsCosmetic().Should().BeTrue();
            purchase.IsSubscription().Should().BeFalse();
        }

        [Fact]
        public void SetDataJson_ShouldUpdateDataJson()
        {
            var purchase = CreateDefaultPurchase();

            purchase.SetDataJson("{\"new\":true}");

            purchase.DataJson.Should().Be("{\"new\":true}");
        }

        [Fact]
        public void SetExternalPaymentId_ShouldUpdateExternalId()
        {
            var purchase = CreateDefaultPurchase();

            purchase.SetExternalPaymentId("new-ext-456");

            purchase.ExternalPaymentId.Should().Be("new-ext-456");
        }

        [Fact]
        public void SetPaymentProviderName_ShouldUpdateProvider()
        {
            var purchase = CreateDefaultPurchase();

            purchase.SetPaymentProviderName(ProviderName.PAYPAL);

            purchase.PaymentProviderName.Should().Be(ProviderName.PAYPAL);
        }

        [Fact]
        public void SetStatus_ShouldUpdateStatus()
        {
            var purchase = CreateDefaultPurchase();

            purchase.SetStatus(PurchaseStatus.COMPLETED);

            purchase.Status.Should().Be(PurchaseStatus.COMPLETED);
        }

        [Fact]
        public void MarkWebhookEventProcessed_ShouldAddUniqueEvents()
        {
            var purchase = CreateDefaultPurchase();

            purchase.MarkWebhookEventProcessed("event1");
            purchase.MarkWebhookEventProcessed("event2");
            purchase.MarkWebhookEventProcessed("event1");

            purchase.ProcessedWebhookEventIds.Should().HaveCount(2)
                .And.Contain(new[] { "event1", "event2" });
        }

        [Fact]
        public void UpdateTimestamps_ShouldSetUpdatedAtInUtc()
        {
            var purchase = CreateDefaultPurchase();
            var now = DateTime.Now;

            purchase.UpdateTimestamps(now);

            purchase.UpdatedAt.Should().Be(now.ToUniversalTime());
        }
    }
}
