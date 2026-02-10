using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;

namespace Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;

    // ✅ ADDED: Inject TokenService
    private readonly ITokenService _tokenService;

    // 🔄 MODIFIED constructor
    public AuthService(AppDbContext context, ITokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService; // ✅ ADDED
    }

    public async Task<string> RegisterAsync(RegisterDto dto)
    {
        var userExists = await _context.Users
            .AnyAsync(x => x.Email == dto.Email);

        if (userExists)
            return "User already exists";

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        var user = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            PasswordHash = passwordHash
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return "Registration successful";
    }

    // 🔄 MODIFIED RETURN TYPE → AuthResponseDto
    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Email == dto.Email);

        if (user == null)
            throw new Exception("User not found"); // 🔄 MODIFIED (avoid returning string)

        var isPasswordValid =
            BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);

        if (!isPasswordValid)
            throw new Exception("Invalid password"); // 🔄 MODIFIED

        // ✅ ADDED: Create access token
        var accessToken = _tokenService.CreateAccessToken(
            user.Id.ToString(),
            user.Email
        );

        // ✅ ADDED: Generate refresh token
        var refreshToken = _tokenService.GenerateRefreshToken();

        // ✅ ADDED: Save refresh token in DB
        var refreshTokenEntity = new RefreshToken
        {
            Token = refreshToken,
            UserId = user.Id.ToString(),
            ExpiryDate = DateTime.UtcNow.AddDays(7), // 7 days validity
            IsRevoked = false
        };

        _context.RefreshTokens.Add(refreshTokenEntity);
        await _context.SaveChangesAsync();

        // 🔄 MODIFIED: Return both tokens
        return new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    // ✅ ADDED: Refresh method
    public async Task<string> RefreshAsync(string refreshToken)
    {
        var tokenInDb = await _context.RefreshTokens
            .FirstOrDefaultAsync(x =>
                x.Token == refreshToken &&
                !x.IsRevoked);

        if (tokenInDb == null || tokenInDb.ExpiryDate < DateTime.UtcNow)
            throw new Exception("Invalid refresh token");

        var user = await _context.Users
            .FindAsync(tokenInDb.UserId);

        return _tokenService.CreateAccessToken(
            user.Id.ToString(),
            user.Email
        );
    }
}
