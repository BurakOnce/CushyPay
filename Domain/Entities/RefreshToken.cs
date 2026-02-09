using CushyPay.Domain.Common;

namespace CushyPay.Domain.Entities;

public class RefreshToken : BaseEntity
{
    public int UserId { get; private set; }
    public string Token { get; private set; } = string.Empty;
    public DateTime ExpiresAt { get; private set; }
    public bool IsRevoked { get; private set; }
    public DateTime? RevokedAt { get; private set; }
    public string? ReplacedByToken { get; private set; }
    public string? ReasonRevoked { get; private set; }
    public string? IpAddress { get; private set; }
    public string? UserAgent { get; private set; }

    public User User { get; private set; } = null!;

    private RefreshToken() { }

    private RefreshToken(int userId, string token, DateTime expiresAt, string? ipAddress = null, string? userAgent = null)
    {
        UserId = userId;
        Token = token;
        ExpiresAt = expiresAt;
        IsRevoked = false;
        IpAddress = ipAddress;
        UserAgent = userAgent;
    }

    public static RefreshToken Create(int userId, string token, DateTime expiresAt, string? ipAddress = null, string? userAgent = null)
    {
        if (userId <= 0)
            throw new ArgumentException("User ID must be greater than zero.", nameof(userId));

        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentException("Token cannot be null or empty.", nameof(token));

        if (expiresAt <= DateTime.UtcNow)
            throw new ArgumentException("Expiration date must be in the future.", nameof(expiresAt));

        return new RefreshToken(userId, token, expiresAt, ipAddress, userAgent);
    }

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;

    public bool IsActive => !IsRevoked && !IsExpired;

    public void Revoke(string? reason = null, string? replacedByToken = null)
    {
        if (IsRevoked)
            throw new InvalidOperationException("Token is already revoked.");

        IsRevoked = true;
        RevokedAt = DateTime.UtcNow;
        ReasonRevoked = reason;
        ReplacedByToken = replacedByToken;
        MarkAsUpdated();
    }
}

