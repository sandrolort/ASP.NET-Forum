using System.ComponentModel.DataAnnotations;

namespace Common.DTOs.User.Request;

public record UserUpdateModel(
    string? UserName, 
    [EmailAddress]
    [MaxLength(80)]
    string? Email, 
    [MaxLength(100)]
    string? ProfilePicUrl
);