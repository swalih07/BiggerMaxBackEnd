using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly ITokenService _tokenService;

    public AuthService(AppDbContext context, ITokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
    }

    // =========================
    // REGISTER
    // =========================
    public async Task<string> RegisterAsync(RegisterDto dto)
    {
        var userExists = await _context.Users
            .AnyAsync(x => x.Email.ToLower() == dto.Email.ToLower());

        if (userExists)
            return "User already exists";

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        var user = new User
        {
            Name = dto.Name,
            Email = dto.Email.ToLower(),
            PasswordHash = passwordHash,
            Role = "User",
            IsActive = true,   // Active by default
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return "Registration successful";
    }

    // =========================
    // LOGIN
    // =========================
    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Email.ToLower() == dto.Email.ToLower());

        if (user == null)
            throw new Exception("Invalid credentials");

        // 🔴 BLOCK CHECK
        if (!user.IsActive)
            throw new Exception("Your account has been blocked by admin");

        var isPasswordValid =
            BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);

        if (!isPasswordValid)
            throw new Exception("Invalid credentials");

        var accessToken = _tokenService.CreateAccessToken(
            user.Id.ToString(),
            user.Email,
            user.Role
        );

        var refreshToken = _tokenService.GenerateRefreshToken();

        var refreshTokenEntity = new RefreshToken
        {
            Token = refreshToken,
            UserId = user.Id,
            ExpiryDate = DateTime.UtcNow.AddDays(7),
            IsRevoked = false
        };

        _context.RefreshTokens.Add(refreshTokenEntity);
        await _context.SaveChangesAsync();

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            Role = user.Role
        };
    }

    // =========================
    // REFRESH TOKEN
    // =========================
    public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken)
    {
        var tokenInDb = await _context.RefreshTokens
            .FirstOrDefaultAsync(x =>
                x.Token == refreshToken &&
                !x.IsRevoked);

        if (tokenInDb == null || tokenInDb.ExpiryDate < DateTime.UtcNow)
            throw new Exception("Invalid refresh token");

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == tokenInDb.UserId);

        if (user == null)
            throw new Exception("User not found");

        if (!user.IsActive)
            throw new Exception("Your account has been blocked by admin");

        var accessToken = _tokenService.CreateAccessToken(
            user.Id.ToString(),
            user.Email,
            user.Role
        );

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken, // Send back the same refresh token or generate a new one
            Role = user.Role
        };
    }

    // =========================
    // LOGOUT
    // =========================
    public async Task<bool> LogoutAsync(string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            return false;

        var tokenInDb = await _context.RefreshTokens
            .FirstOrDefaultAsync(x =>
                x.Token == refreshToken &&
                !x.IsRevoked);

        if (tokenInDb == null)
            return false;

        tokenInDb.IsRevoked = true;

        await _context.SaveChangesAsync();
        return true;
    }

    // =========================
    // GET PROFILE
    // =========================
    public async Task<UserProfileDto?> GetProfileAsync(string userId)
    {
        if (!int.TryParse(userId, out int userIdInt))
            return null;

        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Id == userIdInt);

        if (user == null)
            return null;

        return new UserProfileDto
        {
            Username = user.Name,
            Email = user.Email,
            Role = user.Role
        };
    }
}