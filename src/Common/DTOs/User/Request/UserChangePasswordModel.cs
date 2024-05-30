using System.ComponentModel.DataAnnotations;

namespace Common.DTOs.User.Request;

public record UserChangePasswordModel(
    [Required]
    [MinLength(10)]
    [RegularExpression(@".*\d+.*", ErrorMessage = "Password must contain at least one digit")]
    string OldPassword, 
    [Required]
    [MinLength(10)]
    [RegularExpression(@".*\d+.*", ErrorMessage = "Password must contain at least one digit")]
    string NewPassword
);