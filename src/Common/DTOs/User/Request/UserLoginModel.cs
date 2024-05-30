using System.ComponentModel.DataAnnotations;

namespace Common.DTOs.User.Request;

public record UserLoginModel(
    [Required]
    string? UserName,
    [Required]
    [MinLength(10)]
    [RegularExpression(@".*\d+.*", ErrorMessage = "Password must contain at least one digit")]
    string? Password
);