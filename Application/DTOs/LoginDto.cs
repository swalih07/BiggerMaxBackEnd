using System.ComponentModel.DataAnnotations;

namespace Application.DTOs;

public class LoginDto
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [RegularExpression(@"^[^\s@]+@[^\s@]+\.[^\s@]+$",
    ErrorMessage = "Email must not contain spaces")]
    public string Email { get; set; } = string.Empty;


    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; } = string.Empty;
}
