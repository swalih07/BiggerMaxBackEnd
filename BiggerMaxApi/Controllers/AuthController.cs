using BiggerMaxApi.Common;
using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BiggerMaxApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var result = await _authService.RegisterAsync(dto);

            return Ok(new ApiResponse<string>
            {
                Success = true,
                Message = result,
                Data = null
            });
        }

        // 🔄 MODIFIED: Return AuthResponseDto instead of string
        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Login(LoginDto dto)
        {
            var result = await _authService.LoginAsync(dto); // returns AuthResponseDto

            return Ok(new ApiResponse<AuthResponseDto>
            {
                Success = true,
                Message = "Login successful",
                Data = result   // contains AccessToken + RefreshToken
            });
        }
    }
}
