using MediatR;
using Microsoft.EntityFrameworkCore;
using CushyPay.Application.Common.DTOs;
using CushyPay.Application.Common.Interfaces;
using CushyPay.Application.Common.Responses;
using CushyPay.Domain.Entities;
using CushyPay.Infrastructure.Data;

namespace CushyPay.Application.Features.Users.Commands.RefreshToken;

public class RefreshTokenCommand : IRequest<Result<AuthResponseDto>>
{
    public string RefreshToken { get; set; } = string.Empty;
}

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<AuthResponseDto>>
{
    private readonly AppDbContext _context;
    private readonly IJwtService _jwtService;

    public RefreshTokenCommandHandler(AppDbContext context, IJwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }

    public async Task<Result<AuthResponseDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var refreshToken = await _context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken, cancellationToken);

        if (refreshToken == null || !refreshToken.IsActive)
            return Result<AuthResponseDto>.Failure("Invalid or expired refresh token.");

        refreshToken.Revoke("Replaced by new token");

        var newAccessToken = _jwtService.GenerateToken(refreshToken.User);
        var newRefreshToken = _jwtService.GenerateRefreshToken();

        var newRefreshTokenEntity = CushyPay.Domain.Entities.RefreshToken.Create(
            refreshToken.UserId,
            newRefreshToken,
            DateTime.UtcNow.AddDays(7));

        await _context.RefreshTokens.AddAsync(newRefreshTokenEntity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        var authResponse = new AuthResponseDto
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(60),
            User = new UserDto
            {
                Id = refreshToken.User.Id,
                Email = refreshToken.User.Email,
                FirstName = refreshToken.User.FirstName,
                LastName = refreshToken.User.LastName,
                Role = refreshToken.User.Role,
                IsActive = refreshToken.User.IsActive,
                PhoneNumber = refreshToken.User.PhoneNumber,
                CreatedAt = refreshToken.User.CreatedAt
            }
        };

        return Result<AuthResponseDto>.Success(authResponse);
    }
}

