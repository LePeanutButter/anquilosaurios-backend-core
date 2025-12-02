using aquilosaurios_backend_core.Shared;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Security.Cryptography;
using System.Text;

namespace aquilosaurios_backend_core.Domain.Models
{
    #region Achievement Model

    /// <summary>
    /// Represents a single achievement unlocked by a user.
    /// Embedded within the <see cref="User"/> document.
    /// </summary>
    /// Initializes a new instance of the <see cref="Achievement"/> class.
    /// </summary>
    /// <param name="name">The name of the achievement.</param>
    /// <param name="description">A description of the achievement.</param>
    /// <param name="unlockedDate">The date when the achievement was unlocked.</param>
    public class Achievement(string name, string description, DateTime unlockedDate)
    {
        #region Properties

        /// <summary>
        /// The name of the achievement.
        /// </summary>
        [BsonElement("name")]
        public string Name { get; set; } = name;

        /// <summary>
        /// A short description of what the achievement represents.
        /// </summary>
        [BsonElement("description")]
        public string Description { get; set; } = description;

        /// <summary>
        /// The date and time (in UTC) when the achievement was unlocked.
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        [BsonElement("unlockedDate")]
        public DateTime UnlockedDate { get; set; } = unlockedDate;

        #endregion
    }

    #endregion

    #region Profile Stats Model

    /// <summary>
    /// Represents cumulative gameplay statistics for a user profile.
    /// Embedded within the <see cref="User"/> document.
    /// </summary>
    public class ProfileStats
    {
        #region Properties

        /// <summary>
        /// Total number of matches played by the user.
        /// </summary>
        [BsonElement("playedMatches")]
        public int PlayedMatches { get; set; } = 0;

        /// <summary>
        /// Total number of matches won by the user.
        /// </summary>
        [BsonElement("winnedMatches")]
        public int WinnedMatches { get; set; } = 0;

        /// <summary>
        /// Total number of times the user has died in matches.
        /// </summary>
        [BsonElement("deaths")]
        public int Deaths { get; set; } = 0;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ProfileStats"/> class.
        /// </summary>
        public ProfileStats() { }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sets the total number of matches played.
        /// </summary>
        public void SetPlayedMatches(int matches) => PlayedMatches = matches;

        /// <summary>
        /// Sets the total number of matches won.
        /// </summary>
        public void SetWinnedMatches(int matches) => WinnedMatches = matches;

        /// <summary>
        /// Sets the total number of player deaths.
        /// </summary>
        public void SetDeaths(int deaths) => Deaths = deaths;

        #endregion
    }

    #endregion

    #region User Model

    /// <summary>
    /// Represents a registered player or user in the Aquilosaurios system.
    /// Contains authentication data, account state, achievements, and gameplay stats.
    /// </summary>
    public class User
    {
        #region MongoDB Identifiers

        /// <summary>
        /// Unique identifier for the user document.
        /// </summary>
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; } = Guid.NewGuid();

        #endregion

        #region Basic User Info

        /// <summary>
        /// The display name of the user.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The user's registered email address.
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// The unique username used for login or in-game display.
        /// </summary>
        public string? Username { get; set; }

        /// <summary>
        /// The date and time (in UTC) when the account was created.
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        #endregion

        #region Authentication & Security

        /// <summary>
        /// The hashed password string (SHA256-based).
        /// </summary>
        public string? PasswordHash { get; set; }

        /// <summary>
        /// The authentication provider used to register or log in the user.
        /// </summary>
        public AuthProvider AuthProvider { get; set; } = AuthProvider.LOCAL;

        #endregion

        #region Relationships

        /// <summary>
        /// A collection of match IDs associated with the user.
        /// </summary>
        public List<Guid> MatchesIds { get; set; } = [];
        
        /// <summary>
        /// A collection of purchase IDs made by the user.
        /// </summary>
        public List<Guid> PurchasesIds { get; set; } = [];

        /// <summary>
        /// A list of achievements unlocked by the user.
        /// </summary>
        public List<Achievement> Achievements { get; set; } = [];

        /// <summary>
        /// The user's aggregated gameplay statistics.
        /// </summary>
        public ProfileStats Stats { get; set; } = new ProfileStats();

        #endregion

        #region Account State

        /// <summary>
        /// Indicates whether the user has administrative privileges.
        /// </summary>
        public bool IsAdmin { get; set; } = false;

        /// <summary>
        /// Indicates whether the user's email has been verified.
        /// </summary>
        public bool IsEmailVerified { get; set; } = false;

        /// <summary>
        /// Indicates whether the user's account is currently active.
        /// </summary>
        public bool IsAccountActive { get; set; } = true;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new local user with password-based authentication.
        /// </summary>
        /// <param name="name">User's display name.</param>
        /// <param name="email">User's email address.</param>
        /// <param name="username">User's unique username.</param>
        /// <param name="rawPassword">Plaintext password to be hashed and stored.</param>
        public User(string name, string email, string username, string rawPassword)
        {
            Name = name;
            Email = email;
            Username = username;
            SetPassword(rawPassword);
        }

        /// <summary>
        /// Initializes a new user associated with a specific authentication provider.
        /// </summary>
        /// <param name="name">User's display name.</param>
        /// <param name="authProvider">The authentication provider (e.g., Google, Facebook).</param>
        public User(string name, AuthProvider authProvider)
        {
            Name = name;
            AuthProvider = authProvider;
        }

        #endregion

        #region Private Helpers

        /// <summary>
        /// Hashes a password using the SHA-256 algorithm and returns a Base64-encoded string.
        /// </summary>
        /// <param name="password">The plaintext password to hash.</param>
        /// <returns>The hashed password string.</returns>
        private static string HashPassword(string password)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        #endregion

        #region Setters / Mutators

        /// <summary>
        /// Sets the user's password after securely hashing it.
        /// </summary>
        public void SetPassword(string rawPassword) => PasswordHash = HashPassword(rawPassword);

        /// <summary>
        /// Updates the user's display name.
        /// </summary>
        public void SetName(string name) => Name = name;

        /// <summary>
        /// Updates the authentication provider used by the user.
        /// </summary>
        public void SetAuthProvider(AuthProvider authProvider) => AuthProvider = authProvider;

        /// <summary>
        /// Enables or disables the user account.
        /// </summary>
        public void SetIsAccountActive(bool state) => IsAccountActive = state;

        /// <summary>
        /// Grants or revokes administrative privileges for the user.
        /// </summary>
        public void SetIsAdmin(bool state) => IsAdmin = state;

        #endregion

        #region Collection Management

        /// <summary>
        /// Adds a new achievement to the user's profile if not already added.
        /// </summary>
        public void AddAchievement(Achievement achievement)
        {
            Achievements ??= [];
            Achievements.Add(achievement);
        }

        /// <summary>
        /// Adds a new purchase ID to the user's purchase history.
        /// Ensures no duplicates are added.
        /// </summary>
        public void AddPurchaseId(Guid purchaseId)
        {
            PurchasesIds ??= [];
            if (!PurchasesIds.Contains(purchaseId))
                PurchasesIds.Add(purchaseId);
        }

        /// <summary>
        /// Adds a new match ID to the user's match history.
        /// Ensures no duplicates are added.
        /// </summary>
        public void AddMatchId(Guid matchId)
        {
            MatchesIds ??= [];
            if (!MatchesIds.Contains(matchId))
                MatchesIds.Add(matchId);
        }

        #endregion

        #region State Checks

        /// <summary>
        /// Checks if the user's email has been verified.
        /// </summary>
        public bool IsEmailVerifiedUser() => IsEmailVerified;

        /// <summary>
        /// Checks if the user's account is active.
        /// </summary>
        public bool IsAccountActiveUser() => IsAccountActive;

        /// <summary>
        /// Checks if the user has administrative privileges.
        /// </summary>
        public bool IsAdminUser() => IsAdmin;

        #endregion

        #region Verification

        /// <summary>
        /// Marks the user's email as verified.
        /// </summary>
        public void VerifyEmail() => IsEmailVerified = true;

        #endregion
    }

    #endregion
}
