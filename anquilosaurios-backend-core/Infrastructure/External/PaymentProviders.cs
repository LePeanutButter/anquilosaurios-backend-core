using ApiPaymentIntent = aquilosaurios_backend_core.API.PaymentIntent;
using aquilosaurios_backend_core.Shared;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Stripe;

namespace aquilosaurios_backend_core.Infrastructure.External
{
    #region Interfaces

    /// <summary>
    /// Interface defining the contract for a payment provider.
    /// </summary>
    public interface IPaymentProvider
    {
        /// <summary>
        /// Charges the provided payment intent.
        /// </summary>
        /// <param name="intent">The payment intent object containing charge details.</param>
        /// <returns>A task that represents the result of the payment attempt.</returns>
        Task<PaymentResult> ChargeAsync(ApiPaymentIntent intent);

        /// <summary>
        /// Refunds the payment using the provided external payment ID.
        /// </summary>
        /// <param name="externalPaymentId">The external payment ID to refund.</param>
        /// <returns>A task that represents the result of the refund attempt.</returns>
        Task<RefundResult> RefundAsync(string externalPaymentId);

        /// <summary>
        /// Gets the name of the payment provider.
        /// </summary>
        /// <returns>The name of the provider (e.g., "PayPal", "Stripe").</returns>
        Enum GetProviderName();
    }

    #endregion

    #region PayPal Provider Implementation

    /// <summary>
    /// Implementation of the IPaymentProvider interface for the PayPal payment provider.
    /// </summary>
    public class PaypalProvider : IPaymentProvider
    {
        private readonly string _clientId;
        private readonly string _secret;
        private readonly HttpClient _httpClient;

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the PayPalProvider class with the specified client ID and secret.
        /// </summary>
        /// <param name="clientId">The PayPal API client ID.</param>
        /// <param name="secret">The PayPal API secret.</param>
        public PaypalProvider(string clientId, string secret)
        {
            _clientId = clientId;
            _secret = secret;
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://api-m.sandbox.paypal.com/")
            };
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Retrieves an access token from PayPal for authorization.
        /// </summary>
        /// <returns>The access token.</returns>
        private async Task<string> GetAccessTokenAsync()
        {
            var authToken = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_clientId}:{_secret}"));
            var request = new HttpRequestMessage(HttpMethod.Post, "v1/oauth2/token");
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", authToken);
            request.Content = new StringContent("grant_type=client_credentials", Encoding.UTF8, "application/x-www-form-urlencoded");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            return doc.RootElement.GetProperty("access_token").GetString() ?? string.Empty;
        }

        #endregion

        #region IPaymentProvider Methods

        public Enum GetProviderName() => ProviderName.PAYPAL;

        /// <summary>
        /// Charges a payment using PayPal.
        /// </summary>
        /// <param name="intent">The payment intent containing payment details.</param>
        /// <returns>A task representing the result of the charge attempt.</returns>
        public async Task<PaymentResult> ChargeAsync(ApiPaymentIntent intent)
        {
            var token = await GetAccessTokenAsync();

            var paymentRequest = new
            {
                intent = "CAPTURE",
                purchase_units = new[]
                {
                    new
                    {
                        amount = new { currency_code = intent.Currency, value = intent.Amount.ToString("F2") },
                        custom_id = intent.PurchaseId.ToString()
                    }
                },
                application_context = new
                {
                    brand_name = "Aquilosaurios",
                    user_action = "PAY_NOW"
                }
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "v2/checkout/orders")
            {
                Content = new StringContent(JsonSerializer.Serialize(paymentRequest), Encoding.UTF8, "application/json")
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);
            var raw = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return new PaymentResult
                {
                    Success = false,
                    ExternalPaymentId = string.Empty,
                    Status = "FAILED",
                    Message = "PayPal payment failed",
                    RequiresAction = false,
                    ProviderResponseRaw = raw
                };

            using var doc = JsonDocument.Parse(raw);
            var orderId = doc.RootElement.GetProperty("id").GetString() ?? string.Empty;

            return new PaymentResult
            {
                Success = true,
                ExternalPaymentId = orderId,
                Status = "CREATED",
                Message = "Payment order created via PayPal",
                RequiresAction = false,
                ProviderResponseRaw = raw
            };
        }

        /// <summary>
        /// Refunds a payment using PayPal.
        /// </summary>
        /// <param name="externalPaymentId">The external payment ID to refund.</param>
        /// <returns>A task representing the result of the refund attempt.</returns>
        public async Task<RefundResult> RefundAsync(string externalPaymentId)
        {
            var token = await GetAccessTokenAsync();
            var request = new HttpRequestMessage(HttpMethod.Post, $"v2/payments/captures/{externalPaymentId}/refund");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);
            var raw = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return new RefundResult
                {
                    Success = false,
                    RefundId = string.Empty,
                    Message = "PayPal refund failed"
                };
            }

            using var doc = JsonDocument.Parse(raw);
            var refundId = doc.RootElement.GetProperty("id").GetString() ?? string.Empty;

            return new RefundResult
            {
                Success = true,
                RefundId = refundId,
                Message = "Refund processed successfully via PayPal"
            };
        }

        #endregion
    }

    #endregion

    #region Stripe Provider Implementation

    /// <summary>
    /// Implementation of the IPaymentProvider interface for the Stripe payment provider.
    /// </summary>
    public class StripeProvider : IPaymentProvider
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the StripeProvider class with the specified api key.
        /// </summary>
        /// <param name="apiKey">The Stripe API Key.</param>
        public StripeProvider(string apiKey)
        {
            StripeConfiguration.ApiKey = apiKey;
        }

        #endregion

        #region IPaymentProvider Methods

        public Enum GetProviderName() => ProviderName.STRIPE;

        /// <summary>
        /// Charges a payment using Stripe.
        /// </summary>
        /// <param name="intent">The payment intent containing payment details.</param>
        /// <returns>A task representing the result of the charge attempt.</returns>
        public async Task<PaymentResult> ChargeAsync(ApiPaymentIntent intent)
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(intent.Amount * 100),
                Currency = intent.Currency.ToLower(),
                PaymentMethod = intent.PaymentToken,
                Confirm = true,
                Metadata = intent.Metadata
            };

            var service = new PaymentIntentService();
            PaymentIntent paymentIntent;

            if (!string.IsNullOrEmpty(intent.IdempotencyKey))
            {
                paymentIntent = await service.CreateAsync(options, new RequestOptions
                {
                    IdempotencyKey = intent.IdempotencyKey
                });
            }
            else
            {
                paymentIntent = await service.CreateAsync(options);
            }

            return new PaymentResult
            {
                Success = paymentIntent.Status == "succeeded",
                ExternalPaymentId = paymentIntent.Id,
                Status = paymentIntent.Status,
                Message = "Payment processed via Stripe",
                RequiresAction = paymentIntent.Status == "requires_action",
                ProviderResponseRaw = paymentIntent.ToJson()
            };
        }

        /// <summary>
        /// Refunds a payment using Stripe.
        /// </summary>
        /// <param name="externalPaymentId">The external payment ID to refund.</param>
        /// <returns>A task representing the result of the refund attempt.</returns>
        public async Task<RefundResult> RefundAsync(string externalPaymentId)
        {
            var options = new RefundCreateOptions
            {
                PaymentIntent = externalPaymentId
            };
            var service = new RefundService();
            var refund = await service.CreateAsync(options);

            return new RefundResult
            {
                Success = refund.Status == "succeeded",
                RefundId = refund.Id,
                Message = "Refund processed via Stripe"
            };
        }

        #endregion
    }

    #endregion

    #region Payment Result Models

    /// <summary>
    /// Represents the result of a payment attempt.
    /// </summary>
    public class PaymentResult
    {
        public bool Success { get; set; }
        public string ExternalPaymentId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool RequiresAction { get; set; }
        public string ProviderResponseRaw { get; set; } = string.Empty;
    }

    /// <summary>
    /// Represents the result of a refund attempt.
    /// </summary>
    public class RefundResult
    {
        public bool Success { get; set; }
        public string RefundId { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }

    #endregion
}
