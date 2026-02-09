namespace CushyPay.Application.Common.Interfaces;

public interface IRateLimiter
{
    Task<bool> IsAllowedAsync(string identifier, int maxRequests, TimeSpan window, CancellationToken cancellationToken = default);
}

