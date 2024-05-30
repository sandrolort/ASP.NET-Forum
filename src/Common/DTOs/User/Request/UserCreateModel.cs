using System.ComponentModel.DataAnnotations;

namespace Common.DTOs.User.Request;

public record UserCreateModel(
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
    [MaxLength(100)]
    string? ProfilePicUrl
);