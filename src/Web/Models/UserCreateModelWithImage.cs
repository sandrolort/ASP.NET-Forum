using System.ComponentModel.DataAnnotations;

namespace Web.Models;

public record UserCreateModelWithImage(
    [Required]
    string? UserName, 
    [Required]
    [MinLength(10)]
    [RegularExpression(@".*\d+.*", ErrorMessage = "Password must contain at least one digit")]
    string? Password, 
    [Required]
    [EmailAddress]
    [MaxLength(80)]
    string? Email,
    IFormFile? ProfilePic);
