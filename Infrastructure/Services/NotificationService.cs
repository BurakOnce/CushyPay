using CushyPay.Application.Common.Interfaces;

namespace CushyPay.Infrastructure.Services;

public class NotificationService : INotificationService
{
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(ILogger<NotificationService> logger)
    {
        _logger = logger;
    }

    public Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Email would be sent to {To} with subject: {Subject}", to, subject);
        return Task.CompletedTask;
    }

    public Task SendSmsAsync(string phoneNumber, string message, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("SMS would be sent to {PhoneNumber} with message: {Message}", phoneNumber, message);
        return Task.CompletedTask;
    }
}

