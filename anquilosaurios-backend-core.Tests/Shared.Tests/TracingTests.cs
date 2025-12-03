using aquilosaurios_backend_core.Shared;
using FluentAssertions;
using Xunit;

namespace aquilosaurios_backend_core.Tests.Shared.Tests
{
    public class LogEntryTests
    {
        [Fact]
        public void LogEntry_ShouldInitializePropertiesCorrectly()
        {
            var level = aquilosaurios_backend_core.Shared.LogLevel.ERROR;
            var message = "An error occurred";
            var stackTrace = "Stack trace details";

            var logEntry = new LogEntry(level, message, stackTrace);

            logEntry.Id.Should().NotBe(Guid.Empty);
            logEntry.Timestamp.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
            logEntry.Level.Should().Be(level);
            logEntry.Message.Should().Be(message);
            logEntry.StackTrace.Should().Be(stackTrace);
        }

        [Fact]
        public void LogEntry_Id_ShouldBeUniqueForDifferentInstances()
        {
            var log1 = new LogEntry(aquilosaurios_backend_core.Shared.LogLevel.INFO, "Message 1", null);
            var log2 = new LogEntry(aquilosaurios_backend_core.Shared.LogLevel.INFO, "Message 2", null);

            log1.Id.Should().NotBe(log2.Id);
        }
    }

    public class AuditEntryTests
    {
        [Fact]
        public void AuditEntry_ShouldInitializePropertiesCorrectly()
        {
            var performedBy = "user123";
            var action = AuditAction.CREATE;
            var dataJson = "{\"field\":\"value\"}";

            var auditEntry = new AuditEntry(performedBy, action, dataJson);

            auditEntry.Id.Should().NotBe(Guid.Empty);
            auditEntry.Timestamp.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
            auditEntry.PerformedBy.Should().Be(performedBy);
            auditEntry.Action.Should().Be(action);
            auditEntry.DataJson.Should().Be(dataJson);
        }

        [Fact]
        public void AuditEntry_Id_ShouldBeUniqueForDifferentInstances()
        {
            var audit1 = new AuditEntry("user1", AuditAction.UPDATE, null);
            var audit2 = new AuditEntry("user2", AuditAction.DELETE, null);

            audit1.Id.Should().NotBe(audit2.Id);
        }
    }
}
