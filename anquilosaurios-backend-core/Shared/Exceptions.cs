namespace aquilosaurios_backend_core.Shared
{
    /// <summary>
    /// Custom exception type used across the Aquilosaurios backend system.
    /// Provides predefined error messages for common domain-specific exceptions.
    /// </summary>
    /// <param name="message">The error message describing the exception.</param>
    public class AnquilosauriosException(string message) : Exception(message)
    {
        /// <summary>
        /// Indicates that a field contains invalid data format, 
        /// e.g., incorrect JSON in 'dataJson' or malformed 'externalPaymentId'.
        /// </summary>
        public const string INVALID_DATA_FORMAT = "Invalid data format in fields like dataJson or externalPaymentId";

        /// <summary>
        /// Indicates that a required field is missing in the request or input data.
        /// </summary>
        public const string MISSING_FIELD = "Missing required field";

        /// <summary>
        /// Indicates that a provided value does not match the expected enumeration type.
        /// </summary>
        public const string INVALID_ENUM_VALUE = "Invalid value for enum type";

        /// <summary>
        /// Indicates a duplicate idempotency key was detected, preventing repeat operations.
        /// </summary>
        public const string DUPLICATE_IDEMPOTENCY_KEY = "Duplicate idempotency key";

        /// <summary>
        /// Indicates that the current user is not authorized to perform the requested operation.
        /// </summary>
        public const string UNAUTHORIZED_ACCESS = "User is not authorized to access this resource";

        /// <summary>
        /// Indicates that the user's account is inactive, suspended, or the email has not been verified.
        /// </summary>
        public const string INACTIVE_ACCOUNT = "User account is inactive or email not verified";

        /// <summary>
        /// Indicates that match-related data is inconsistent, corrupt, or invalid.
        /// </summary>
        public const string INVALID_MATCH_DATA = "Match data is inconsistent or corrupt";

        /// <summary>
        /// Indicates that provided position coordinates are invalid or out of expected bounds.
        /// </summary>
        public const string INVALID_POSITION = "Position coordinates are invalid";
    }
}
