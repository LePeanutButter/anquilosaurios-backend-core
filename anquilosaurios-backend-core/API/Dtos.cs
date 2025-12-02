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

    public abstract class FiltersDTO
    {
        public Guid Id { get; set; } = Guid.Empty;
        public int Page { get; set; } = 1;
        public int Size { get; set; } = 10;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class UserFiltersDTO : FiltersDTO
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Username { get; set; }
        public bool? ActiveStatus { get; set; }
        public bool? AdminPrivilege { get; set; }
    }

    public class LoginDTO(string identifier, string rawPassword)
    {
        public string Identifier { get; set; } = identifier;
        public string RawPassword { get; set; } = rawPassword;
    }

    public class AuthCredentialDTO(string credential)
    {
        public string Credential { get; set; } = credential;
    }

    public class UserUpdateDTO
    {
        public string? Name { get; set; }
        public string? RawPassword { get; set; }
    }

    public class UserRegisterDTO
    {
        public string? Name { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? RawPassword { get; set; }
    }
}