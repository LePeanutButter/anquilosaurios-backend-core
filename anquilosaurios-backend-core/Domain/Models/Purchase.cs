using aquilosaurios_backend_core.Shared;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace aquilosaurios_backend_core.Domain.Models
{
    #region Purchase

    /// <summary>
    /// Represents a purchase made by a user in the Aquilosaurios ecosystem.
    /// Supports both cosmetic item purchases and subscription-based transactions.
    /// </summary>
    /// <param name="type">The type of purchase (Cosmetic or Subscription).</param>
    /// <param name="purchaseDate">The date and time the purchase was initiated.</param>
    /// <param name="paymentProviderName">The payment provider used for the transaction.</param>
    /// <param name="amount">The monetary amount of the purchase.</param>
    /// <param name="currency">The ISO 4217 currency code (e.g., "USD").</param>
    /// <param name="idempotencyKey">A unique key to prevent duplicate transaction processing.</param>
    /// <param name="dataJson">Optional serialized JSON data with extra purchase details.</param>
    public class Purchase(
        PurchaseType type,
        DateTime purchaseDate,
        ProviderName paymentProviderName,
        double amount,
        string currency,
        string idempotencyKey,
        string dataJson)
    {
        #region MongoDB Identifiers

        /// <summary>
        /// Unique identifier for the purchase record.
        /// </summary>
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; } = Guid.NewGuid();

        #endregion

        #region Core Purchase Data

        /// <summary>
        /// The type of purchase being made (Cosmetic or Subscription).
        /// </summary>
        public PurchaseType Type { get; set; } = type;

        /// <summary>
        /// The date and time the purchase was made, stored in UTC.
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime PurchaseDate { get; set; } = purchaseDate;
        
        /// <summary>
        /// The total amount charged for the purchase.
        /// </summary>
        public double Amount { get; set; } = amount;

        /// <summary>
        /// The ISO 4217 currency code used for the purchase.
        /// </summary>
        public string Currency { get; set; } = currency;

        /// <summary>
        /// Serialized JSON data containing metadata or contextual information about the purchase.
        /// </summary>
        public string DataJson { get; set; } = dataJson;

        /// <summary>
        /// Idempotency key to prevent duplicate payment processing.
        /// </summary>
        public string IdempotencyKey { get; set; } = idempotencyKey;

        #endregion

        #region Payment Provider Data

        /// <summary>
        /// The payment provider handling the transaction (e.g., Stripe, PayPal).
        /// </summary>
        public ProviderName PaymentProviderName { get; set; } = paymentProviderName;

        /// <summary>
        /// External identifier assigned by the payment provider (e.g., Stripe PaymentIntent ID).
        /// </summary>
        public required string ExternalPaymentId { get; set; }

        /// <summary>
        /// Current status of the purchase transaction.
        /// Defaults to <see cref="PurchaseStatus.Pending"/>.
        /// </summary>
        public PurchaseStatus Status { get; set; } = PurchaseStatus.PENDING;

        #endregion

        #region Timestamps

        /// <summary>
        /// The date and time when the record was created, stored in UTC.
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// The date and time when the record was last updated, stored in UTC.
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        #endregion

        #region Webhook Handling

        /// <summary>
        /// A list of webhook event identifiers that have already been processed for this purchase.
        /// Prevents handling the same event multiple times.
        /// </summary>
        public List<string> ProcessedWebhookEventIds { get; set; } = [];

        #endregion

        #region Behavior Helpers

        /// <summary>
        /// Determines whether the purchase is a subscription.
        /// </summary>
        public bool IsSubscription() => Type == PurchaseType.SUBSCRIPTION;

        /// <summary>
        /// Determines whether the purchase is a cosmetic item.
        /// </summary>
        public bool IsCosmetic() => Type == PurchaseType.COSMETIC;

        #endregion

        #region Update Methods

        /// <summary>
        /// Updates the serialized JSON data associated with the purchase.
        /// </summary>
        /// <param name="dataJson">New JSON string containing updated data.</param>
        public void SetDataJson(string dataJson) => DataJson = dataJson;

        /// <summary>
        /// Assigns an external payment identifier to this purchase.
        /// </summary>
        /// <param name="externalId">Unique identifier from the payment provider.</param>
        public void SetExternalPaymentId(string externalId) => ExternalPaymentId = externalId;

        /// <summary>
        /// Updates the payment provider name associated with the transaction.
        /// </summary>
        /// <param name="name">The payment provider to set.</param>
        public void SetPaymentProviderName(ProviderName name) => PaymentProviderName = name;

        /// <summary>
        /// Updates the current purchase status (e.g., Completed, Failed, Refunded).
        /// </summary>
        /// <param name="status">The new status to apply.</param>
        public void SetStatus(PurchaseStatus status) => Status = status;

        /// <summary>
        /// Marks a specific webhook event as processed to ensure idempotent handling.
        /// </summary>
        /// <param name="eventId">The identifier of the webhook event.</param>
        public void MarkWebhookEventProcessed(string eventId)
        {
            if (!ProcessedWebhookEventIds.Contains(eventId))
            {
                ProcessedWebhookEventIds.Add(eventId);
            }
        }

        /// <summary>
        /// Updates the <see cref="UpdatedAt"/> timestamp to the provided date in UTC format.
        /// </summary>
        /// <param name="now">The current timestamp.</param>
        public void UpdateTimestamps(DateTime now)
        {
            UpdatedAt = now.ToUniversalTime();
        }

        #endregion
    }

    #endregion
}
