using Application.DTOs;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IAuthService
    {
        Task<string> RegisterAsync(RegisterDto dto);

        // Return AuthResponseDto instead of string
        Task<AuthResponseDto> LoginAsync(LoginDto dto);

        // Refresh token method
        //Task<string> RefreshAsync(string refreshToken);
        Task<bool> LogoutAsync(string refreshToken);
        Task<UserProfileDto> GetProfileAsync(string userId);
        Task<string> RefreshTokenAsync(string refreshToken);
        
    }
}
