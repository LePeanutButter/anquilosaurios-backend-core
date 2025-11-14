namespace aquilosaurios_backend_core.API
{
    public class PaymentIntent(Guid purchaseId, double amount, string currency, string paymentToken)
    {
        public Guid PurchaseId { get; set; } = purchaseId;
        public double Amount { get; set; } = amount;
        public string Currency { get; set; } = currency;
        public string? IdempotencyKey { get; set; }
        public string PaymentToken { get; set; } = paymentToken;
        public Dictionary<string, string>? Metadata { get; set; }
    }
}
