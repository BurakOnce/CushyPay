using MediatR;
using Microsoft.EntityFrameworkCore;
using CushyPay.Application.Common.DTOs;
using CushyPay.Application.Common.Interfaces;
using CushyPay.Application.Common.Responses;
using CushyPay.Domain.Entities;
using CushyPay.Domain.Enums;
using CushyPay.Infrastructure.Data;

namespace CushyPay.Application.Features.Users.Commands.RegisterUser;

public class RegisterUserCommand : IRequest<Result<AuthResponseDto>>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.Standard;
    public string? PhoneNumber { get; set; }
}

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Result<AuthResponseDto>>
{
    private readonly AppDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtService _jwtService;

    public RegisterUserCommandHandler(
        AppDbContext context,
        IPasswordHasher passwordHasher,
        IJwtService jwtService)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
    }

    public async Task<Result<AuthResponseDto>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

        if (existingUser != null)
            return Result<AuthResponseDto>.Failure("User with this email already exists.");

        var passwordHash = _passwordHasher.HashPassword(request.Password);
        var user = User.Create(
            request.Email,
            passwordHash,
            request.FirstName,
            request.LastName,
            request.Role);

        if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
        {
            user.UpdateProfile(request.FirstName, request.LastName, request.PhoneNumber);
        }

        await _context.Users.AddAsync(user, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

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

