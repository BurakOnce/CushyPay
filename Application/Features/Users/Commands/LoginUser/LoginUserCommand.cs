using MediatR;
using Microsoft.EntityFrameworkCore;
using CushyPay.Application.Common.DTOs;
using CushyPay.Application.Common.Interfaces;
using CushyPay.Application.Common.Responses;
using CushyPay.Domain.Entities;
using CushyPay.Infrastructure.Data;

namespace CushyPay.Application.Features.Users.Commands.LoginUser;

public class LoginUserCommand : IRequest<Result<AuthResponseDto>>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, Result<AuthResponseDto>>
{
    private readonly AppDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtService _jwtService;

    public LoginUserCommandHandler(
        AppDbContext context,
        IPasswordHasher passwordHasher,
        IJwtService jwtService)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
    }

    public async Task<Result<AuthResponseDto>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

        if (user == null || !user.IsActive)
            return Result<AuthResponseDto>.Failure("Invalid email or password.");

        if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
            return Result<AuthResponseDto>.Failure("Invalid email or password.");

        var accessToken = _jwtService.GenerateToken(user);
        var refreshToken = _jwtService.GenerateRefreshToken();

        var refreshTokenEntity = CushyPay.Domain.Entities.RefreshToken.Create(
            user.Id,
            refreshToken,
            DateTime.UtcNow.AddDays(7));

        await _context.RefreshTokens.AddAsync(refreshTokenEntity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        var authResponse = new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(60),
            User = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role,
                IsActive = user.IsActive,
                PhoneNumber = user.PhoneNumber,
                CreatedAt = user.CreatedAt
            }
        };

        return Result<AuthResponseDto>.Success(authResponse);
    }
}

