using System.ComponentModel;

namespace aquilosaurios_backend_core.Shared
{
    #region Achievements

    /// <summary>
    /// Represents various achievement types a player can earn.
    /// </summary>
    public enum AchievementType
    {
        [Description("Keep moving to survive")]
        KEEP_IT_MOVING = 1,

        [Description("Master of aerial maneuvers")]
        AIRBORNE_ADDICT = 2,

        [Description("Survived in the shadows")]
        SHADOW_SURVIVOR = 3,

        [Description("First victory in battle")]
        FIRST_BLOOD_WINNER = 4,

        [Description("Achieved a triple takedown")]
        TRIPLE_TAKEDOWN = 5,

        [Description("Maintained perpetual motion")]
        PERPETUAL_MOTION = 6,

        [Description("Dominated the round")]
        ROUND_DOMINATOR = 7,

        [Description("Achieved a flawless victory")]
        FLAWLESS_VICTORY = 8
    }

    #endregion

    #region Subscriptions

    /// <summary>
    /// Available subscription plans for the user.
    /// </summary>
    public enum SubscriptionPlans
    {
        [Description("Monthly ad-free subscription")]
        AD_FREE_MONTHLY = 1,

        [Description("Yearly ad-free subscription")]
        AD_FREE_YEARLY = 2
    }

    #endregion

    #region Auditing

    /// <summary>
    /// Represents actions logged in the audit system.
    /// </summary>
    public enum AuditAction
    {
        [Description("Created a resource")]
        CREATE = 1,

        [Description("Updated a resource")]
        UPDATE = 2,

        [Description("Deleted a resource")]
        DELETE = 3,

        [Description("Viewed a resource")]
        VIEW = 4,

        [Description("User logged in")]
        LOGIN = 5,

        [Description("User logged out")]
        LOGOUT = 6,

        [Description("Made a purchase")]
        PURCHASE = 7,

        [Description("Played a match")]
        MATCH_PLAYED = 8,

        [Description("System generated event")]
        SYSTEM_EVENT = 9
    }

    #endregion

    #region Purchases

    /// <summary>
    /// Types of purchases a user can make.
    /// </summary>
    public enum PurchaseType
    {
        [Description("Cosmetic item")]
        COSMETIC = 1,

        [Description("Subscription plan")]
        SUBSCRIPTION = 2
    }


    /// <summary>
    /// Status of a purchase transaction.
    /// </summary>
    public enum PurchaseStatus
    {
        [Description("Transaction pending")]
        PENDING = 1,

        [Description("Transaction processing")]
        PROCESSING = 2,

        [Description("Transaction completed successfully")]
        COMPLETED = 3,

        [Description("Transaction failed")]
        FAILED = 4,

        [Description("Transaction refunded")]
        REFUNDED = 5,

        [Description("Transaction disputed")]
        DISPUTED = 6
    }

    #endregion

    #region Logging

    /// <summary>
    /// Represents the severity level of a log entry.
    /// </summary>
    public enum LogLevel
    {
        [Description("Informational message")]
        INFO = 1,

        [Description("Warning message")]
        WARNING = 2,

        [Description("Error message")]
        ERROR =3
    }

    #endregion

    #region Payments

    /// <summary>
    /// Names of payment providers.
    /// </summary>
    public enum ProviderName
    {
        [Description("Stripe payment provider")]
        STRIPE = 1,

        [Description("PayPal payment provider")]
        PAYPAL = 2
    }

    #endregion

    #region Cosmetics

    /// <summary>
    /// Catalog of cosmetic items available in the game.
    /// To be defined in future.
    /// </summary>
    public enum CosmeticCatalog
    {
        // Placeholder for future cosmetic items
    }

    #endregion

    #region Authentication

    /// <summary>
    /// Represents authentication providers available for user login.
    /// </summary>
    public enum AuthProvider
    {
        [Description("Local authentication provider (email and password)")]
        LOCAL = 1,

        [Description("Google authentication provider")]
        GOOGLE = 2,

        [Description("Facebook authentication provider")]
        FACEBOOK = 3
    }

    #endregion
}
