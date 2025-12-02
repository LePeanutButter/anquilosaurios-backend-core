using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace aquilosaurios_backend_core.Shared
{
    /// <summary>
    /// Represents a single log entry in the system.
    /// Used for tracing application events, errors, or informational messages.
    /// </summary>
    /// <param name="level">The severity level of the log entry (Info, Warning, Error).</param>
    /// <param name="message">The descriptive message of the log entry.</param>
    /// <param name="stackTrace">Optional stack trace information for errors.</param>
    public class LogEntry(LogLevel level, string message, string stackTrace)
    {
        /// <summary>
        /// Unique identifier for the log entry.
        /// </summary>
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; init; } = Guid.NewGuid();

        /// <summary>
        /// The timestamp when the log entry was created.
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime Timestamp { get; init; } = DateTime.Now;

        /// <summary>
        /// The severity level of the log entry.
        /// </summary>
        public LogLevel Level { get; set; } = level;

        /// <summary>
        /// Descriptive message for the log entry.
        /// </summary>
        public string Message { get; set; } = message;

        /// <summary>
        /// Stack trace associated with the log entry, if applicable.
        /// Useful for debugging errors.
        /// </summary>
        public string StackTrace { get; set; } = stackTrace;
    }

    /// <summary>
    /// Represents a single audit record in the system.
    /// Used to track actions performed by users or the system for auditing purposes.
    /// </summary>
    /// <param name="performedBy">The user or system component that performed the action.</param>
    /// <param name="action">The type of action performed.</param>
    /// <param name="dataJson">Optional JSON string containing additional data relevant to the action.</param>
    public class AuditEntry(string performedBy, AuditAction action, string dataJson)
    {
        /// <summary>
        /// Unique identifier for the audit entry.
        /// </summary>
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; init; } = Guid.NewGuid();

        /// <summary>
        /// The timestamp when the audit entry was created.
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime Timestamp { get; init; } = DateTime.Now;

        /// <summary>
        /// The user or system component that performed the action.
        /// </summary>
        public string PerformedBy { get; set; } = performedBy;

        /// <summary>
        /// The type of action performed.
        /// </summary>
        public AuditAction Action { get; set; } = action;

        /// <summary>
        /// Optional JSON string containing additional data related to the action.
        /// </summary>
        public string DataJson { get; set; } = dataJson;
    }
}
