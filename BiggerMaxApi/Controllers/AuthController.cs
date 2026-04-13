using Application.DTOs;
using Application.Interfaces;
using BiggerMaxApi.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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

        // ======================
        // REGISTER
        // ======================
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            try
            {
                var result = await _authService.RegisterAsync(dto);

                if (result == "Registration successful")
                {
                    return Ok(new ApiResponse<string>
                    {
                        Success = true,
                        Message = result,
                        Data = null
                    });
                }
                else
                {
                    return BadRequest(new ApiResponse<string>
                    {
                        Success = false,
                        Message = result,
                        Data = null
                    });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        // ======================
        // LOGIN
        // ======================
        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Login(LoginDto dto)
        {
            try
            {
                var result = await _authService.LoginAsync(dto);

                Response.Cookies.Append("refreshToken", result.RefreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = DateTime.UtcNow.AddDays(7)
                });

                return Ok(new ApiResponse<AuthResponseDto>
                {
                    Success = true,
                    Message = "Login successful",
                    Data = new AuthResponseDto
                    {
                        AccessToken = result.AccessToken,
                        RefreshToken = result.RefreshToken,
                        Email = result.Email,
                        Role = result.Role
                    }
                });
            }
            catch (Exception ex)
            {
                if (ex.Message == "Invalid credentials")
                {
                    return Unauthorized(new ApiResponse<string>
                    {
                        Success = false,
                        Message = ex.Message,
                        Data = null
                    });
                }

                if (ex.Message.Contains("blocked"))
                {
                    return StatusCode(403, new ApiResponse<string>
                    {
                        Success = false,
                        Message = ex.Message,
                        Data = null
                    });
                }

                return BadRequest(new ApiResponse<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        // ======================
        // LOGOUT
        // ======================
        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var refreshToken = Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(refreshToken))
            {
                return BadRequest(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Refresh token not found",
                    Data = null
                });
            }

            var result = await _authService.LogoutAsync(refreshToken);

            if (!result)
            {
                return BadRequest(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Invalid refresh token",
                    Data = null
                });
            }

            Response.Cookies.Delete("refreshToken");

            return Ok(new ApiResponse<string>
            {
                Success = true,
                Message = "Logout successful",
                Data = null
            });
        }

        // ======================
        // PROFILE
        // ======================
        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Invalid token",
                    Data = null
                });
            }

            var profile = await _authService.GetProfileAsync(userId);

            if (profile == null)
            {
                return NotFound(new ApiResponse<string>
                {
                    Success = false,
                    Message = "User not found",
                    Data = null
                });
            }

            return Ok(new ApiResponse<UserProfileDto>
            {
                Success = true,
                Message = "Profile fetched successfully",
                Data = profile
            });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenDto? dto)
        {
            try
            {
                var refreshToken = dto?.RefreshToken ?? Request.Cookies["refreshToken"];

                if (string.IsNullOrEmpty(refreshToken))
                {
                    return Unauthorized(new ApiResponse<string>
                    {
                        Success = false,
                        Message = "Refresh token missing",
                        Data = null
                    });
                }

                var authResponse = await _authService.RefreshTokenAsync(refreshToken);

                return Ok(new ApiResponse<AuthResponseDto>
                {
                    Success = true,
                    Message = "Token refreshed successfully",
                    Data = authResponse
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }
    }
}