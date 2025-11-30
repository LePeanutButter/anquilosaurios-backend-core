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

    public abstract class FiltersDTO(Guid id, int page, int size, DateTime startDate, DateTime endDate)
    {
        public Guid Id { get; set; } = id;
        public int Page { get; set; } = page;
        public int Size { get; set; } = size;
        public DateTime StartDate { get; set; } = startDate;
        public DateTime EndDate { get; set; } = endDate;
    }

    public class UserFiltersDTO(
        Guid id,
        int page,
        int size,
        DateTime startDate,
        DateTime endDate,
        string name,
        string email,
        string username,
        bool activeStatus,
        bool adminPrivilege
    ) : FiltersDTO(id, page, size, startDate, endDate)
    {
        public string Name { get; set; } = name;
        public string Email { get; set; } = email;
        public string Username { get; set; } = username;
        public bool ActiveStatus { get; set; } = activeStatus;
        public bool AdminPrivilege { get; set; } = adminPrivilege;
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

    public class UserUpdateDTO(string name, string rawPassword)
    {
        public string Name { get; set; } = name;
        public string RawPassword { get; set; } = rawPassword;
    }

    public class UserRegisterDTO(string name, string username, string email, string rawPassword)
    {
        public string Name { get; set; } = name;
        public string Username { get; set; } = username;
        public string Email { get; set; } = email;
        public string RawPassword { get; set; } = rawPassword;
    }
}