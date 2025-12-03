using System.ComponentModel;
using System.Reflection;
using aquilosaurios_backend_core.Shared;
using FluentAssertions;
using Xunit;

namespace aquilosaurios_backend_core.Tests.Shared.Tests
{
	public class EnumTests
	{
		private string GetEnumDescription(Enum value)
		{
			var fi = value.GetType().GetField(value.ToString());
			var attribute = fi.GetCustomAttribute<DescriptionAttribute>();
			return attribute?.Description;
		}

		[Theory]
		[InlineData(AchievementType.KEEP_IT_MOVING, "Keep moving to survive")]
		[InlineData(AchievementType.AIRBORNE_ADDICT, "Master of aerial maneuvers")]
		[InlineData(AchievementType.SHADOW_SURVIVOR, "Survived in the shadows")]
		[InlineData(AchievementType.FIRST_BLOOD_WINNER, "First victory in battle")]
		[InlineData(AchievementType.TRIPLE_TAKEDOWN, "Achieved a triple takedown")]
		[InlineData(AchievementType.PERPETUAL_MOTION, "Maintained perpetual motion")]
		[InlineData(AchievementType.ROUND_DOMINATOR, "Dominated the round")]
		[InlineData(AchievementType.FLAWLESS_VICTORY, "Achieved a flawless victory")]
		public void AchievementType_ShouldHaveCorrectDescription(AchievementType type, string expectedDescription)
		{
			GetEnumDescription(type).Should().Be(expectedDescription);
		}

		[Theory]
		[InlineData(SubscriptionPlans.AD_FREE_MONTHLY, "Monthly ad-free subscription")]
		[InlineData(SubscriptionPlans.AD_FREE_YEARLY, "Yearly ad-free subscription")]
		public void SubscriptionPlans_ShouldHaveCorrectDescription(SubscriptionPlans plan, string expectedDescription)
		{
			GetEnumDescription(plan).Should().Be(expectedDescription);
		}

		[Theory]
		[InlineData(AuditAction.CREATE, "Created a resource")]
		[InlineData(AuditAction.UPDATE, "Updated a resource")]
		[InlineData(AuditAction.DELETE, "Deleted a resource")]
		[InlineData(AuditAction.VIEW, "Viewed a resource")]
		[InlineData(AuditAction.LOGIN, "User logged in")]
		[InlineData(AuditAction.LOGOUT, "User logged out")]
		[InlineData(AuditAction.PURCHASE, "Made a purchase")]
		[InlineData(AuditAction.MATCH_PLAYED, "Played a match")]
		[InlineData(AuditAction.SYSTEM_EVENT, "System generated event")]
		public void AuditAction_ShouldHaveCorrectDescription(AuditAction action, string expectedDescription)
		{
			GetEnumDescription(action).Should().Be(expectedDescription);
		}

		[Theory]
		[InlineData(PurchaseType.COSMETIC, "Cosmetic item")]
		[InlineData(PurchaseType.SUBSCRIPTION, "Subscription plan")]
		public void PurchaseType_ShouldHaveCorrectDescription(PurchaseType type, string expectedDescription)
		{
			GetEnumDescription(type).Should().Be(expectedDescription);
		}

		[Theory]
		[InlineData(PurchaseStatus.PENDING, "Transaction pending")]
		[InlineData(PurchaseStatus.PROCESSING, "Transaction processing")]
		[InlineData(PurchaseStatus.COMPLETED, "Transaction completed successfully")]
		[InlineData(PurchaseStatus.FAILED, "Transaction failed")]
		[InlineData(PurchaseStatus.REFUNDED, "Transaction refunded")]
		[InlineData(PurchaseStatus.DISPUTED, "Transaction disputed")]
		public void PurchaseStatus_ShouldHaveCorrectDescription(PurchaseStatus status, string expectedDescription)
		{
			GetEnumDescription(status).Should().Be(expectedDescription);
		}

		[Theory]
		[InlineData(aquilosaurios_backend_core.Shared.LogLevel.INFO, "Informational message")]
		[InlineData(aquilosaurios_backend_core.Shared.LogLevel.WARNING, "Warning message")]
		[InlineData(aquilosaurios_backend_core.Shared.LogLevel.ERROR, "Error message")]
		public void LogLevel_ShouldHaveCorrectDescription(aquilosaurios_backend_core.Shared.LogLevel level, string expectedDescription)
		{
			GetEnumDescription(level).Should().Be(expectedDescription);
		}

		[Theory]
		[InlineData(ProviderName.STRIPE, "Stripe payment provider")]
		[InlineData(ProviderName.PAYPAL, "PayPal payment provider")]
		public void ProviderName_ShouldHaveCorrectDescription(ProviderName provider, string expectedDescription)
		{
			GetEnumDescription(provider).Should().Be(expectedDescription);
		}

		[Theory]
		[InlineData(AuthProvider.LOCAL, "Local authentication provider (email and password)")]
		[InlineData(AuthProvider.GOOGLE, "Google authentication provider")]
		[InlineData(AuthProvider.FACEBOOK, "Facebook authentication provider")]
		public void AuthProvider_ShouldHaveCorrectDescription(AuthProvider provider, string expectedDescription)
		{
			GetEnumDescription(provider).Should().Be(expectedDescription);
		}
	}
}
