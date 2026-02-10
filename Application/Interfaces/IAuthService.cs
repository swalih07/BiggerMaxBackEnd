using Application.DTOs;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IAuthService
    {
        Task<string> RegisterAsync(RegisterDto dto);

        // 🔄 MODIFIED: Return AuthResponseDto instead of string
        Task<AuthResponseDto> LoginAsync(LoginDto dto);

        // ✅ ADDED: Refresh token method
        Task<string> RefreshAsync(string refreshToken);
    }
}
