using CushyPay.Domain.Entities;

namespace CushyPay.Application.Common.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user);
    string GenerateRefreshToken();
    bool ValidateToken(string token);
}

