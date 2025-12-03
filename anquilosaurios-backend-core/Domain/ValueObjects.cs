namespace aquilosaurios_backend_core.Domain
{
    #region Cosmetic Data

    /// <summary>
    /// Represents cosmetic information for a user or item.
    /// Stores the item identifier and its rarity.
    /// </summary>
    public class CosmeticData
    {
        #region Properties

        /// <summary>
        /// Identifier of the cosmetic item.
        /// </summary>
        public string ItemId { get; set; } = string.Empty;

        /// <summary>
        /// Rarity of the cosmetic item (e.g., common, rare, epic).
        /// </summary>
        public string Rarity { get; set; } = string.Empty;

        #endregion

        #region Constructors

        public CosmeticData() { }

        public CosmeticData(string itemId, string rarity)
        {
            ItemId = itemId;
            Rarity = rarity;
        }

        #endregion
    }

    #endregion

    #region Subscription Data

    /// <summary>
    /// Represents subscription information for a user.
    /// Stores the subscription plan and expiration date.
    /// </summary>
    public class SubscriptionData
    {
        #region Properties

        /// <summary>
        /// The subscription plan name (e.g., AD_FREE_MONTHLY).
        /// </summary>
        public string Plan { get; set; } = string.Empty;

        /// <summary>
        /// The expiration date of the subscription.
        /// </summary>
        public DateTime ExpirationDate { get; set; }

        #endregion

        #region Constructors

        public SubscriptionData() { }

        public SubscriptionData(string plan)
        {
            Plan = plan;
        }

        #endregion
    }

    #endregion

    #region Audit Entries

    /// <summary>
    /// Represents a simple audit entry for user actions.
    /// </summary>
    public class UserAuditEntry
    {
        #region Properties

        /// <summary>
        /// The ID of the user associated with this audit entry.
        /// </summary>
        public Guid UserId { get; set; }

        #endregion

        #region Constructors

        public UserAuditEntry() { }

        public UserAuditEntry(Guid userId)
        {
            UserId = userId;
        }

        #endregion
    }

    /// <summary>
    /// Represents a purchase-related audit entry.
    /// Stores purchase ID, amount, and additional data in JSON format.
    /// </summary>
    public class PurchaseAuditEntry
    {
        #region Properties

        /// <summary>
        /// The ID of the purchase.
        /// </summary>
        public Guid PurchaseId { get; set; }

        /// <summary>
        /// The monetary amount of the purchase.
        /// </summary>
        public double Amount { get; set; }

        /// <summary>
        /// Additional data related to the purchase, stored as a string.
        /// </summary>
        public string Data { get; set; } = string.Empty;

        #endregion

        #region Constructors

        public PurchaseAuditEntry() { }

        public PurchaseAuditEntry(Guid purchaseId, double amount, string data)
        {
            PurchaseId = purchaseId;
            Amount = amount;
            Data = data;
        }

        #endregion
    }

    /// <summary>
    /// Represents an audit entry for a match summary.
    /// Stores the ID of the match being audited.
    /// </summary>
    public class MatchSummaryAuditEntry
    {
        #region Properties

        /// <summary>
        /// The ID of the match associated with this audit entry.
        /// </summary>
        public Guid MatchId { get; set; }

        #endregion

        #region Constructors

        public MatchSummaryAuditEntry() { }

        public MatchSummaryAuditEntry(Guid matchId)
        {
            MatchId = matchId;
        }

        #endregion
    }

    #endregion
}
