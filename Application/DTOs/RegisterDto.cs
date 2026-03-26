using System.ComponentModel.DataAnnotations;

namespace Application.DTOs;

public class RegisterDto
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(50, MinimumLength = 3,
        ErrorMessage = "Name must be between 3 and 50 characters")]
    [RegularExpression(@"^[a-zA-Z\s]+$",
        ErrorMessage = "Name must contain only letters")]
    public string Name { get; set; } = string.Empty;


    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [RegularExpression(@"^[^\s@]+@[^\s@]+\.[^\s@]+$",
        ErrorMessage = "Email must not contain spaces and must be valid")]
    public string Email { get; set; } = string.Empty;


    [Required(ErrorMessage = "Password is required")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
    [RegularExpression(@"^(?=.*[!@#$%^&*(),.?""':{}|<>]).+$",
        ErrorMessage = "Password must contain at least one special character")]
    public string Password { get; set; } = string.Empty;
}
