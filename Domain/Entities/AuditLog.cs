using CushyPay.Domain.Common;

namespace CushyPay.Domain.Entities;

public class AuditLog : BaseEntity
{
    public string EntityName { get; private set; } = string.Empty;
    public int EntityId { get; private set; }
    public string Action { get; private set; } = string.Empty;
    public string? Changes { get; private set; }
    public int? UserId { get; private set; }
    public string? UserEmail { get; private set; }
    public string? IpAddress { get; private set; }

    private AuditLog() { }

    private AuditLog(
        string entityName,
        int entityId,
        string action,
        string? changes = null,
        int? userId = null,
        string? userEmail = null,
        string? ipAddress = null)
    {
        EntityName = entityName;
        EntityId = entityId;
        Action = action;
        Changes = changes;
        UserId = userId;
        UserEmail = userEmail;
        IpAddress = ipAddress;
    }

    public static AuditLog Create(
        string entityName,
        int entityId,
        string action,
        string? changes = null,
        int? userId = null,
        string? userEmail = null,
        string? ipAddress = null)
    {
        if (string.IsNullOrWhiteSpace(entityName))
            throw new ArgumentException("Entity name cannot be null or empty.", nameof(entityName));

        if (string.IsNullOrWhiteSpace(action))
            throw new ArgumentException("Action cannot be null or empty.", nameof(action));

        return new AuditLog(entityName, entityId, action, changes, userId, userEmail, ipAddress);
    }
}

