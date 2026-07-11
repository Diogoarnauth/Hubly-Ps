namespace Hubly.api.Infrastructure.Audit;

public sealed record AuditEntry(
    string Action,
    object Payload,
    int? UserId,
    int? CoWorkerId);
