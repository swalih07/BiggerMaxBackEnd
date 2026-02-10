using System;

namespace Application.Interfaces
{
    public interface ITokenService
    {
        // 🔄 MODIFIED: Renamed CreateToken → CreateAccessToken
        string CreateAccessToken(string userId, string email);

        string GenerateRefreshToken(); // ✅ already correct
    }
}
