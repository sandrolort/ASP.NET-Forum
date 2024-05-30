using System.ComponentModel.DataAnnotations;

namespace Web.Models;

public record UserUpdateModelWithImage(
    string? UserName, 
    [EmailAddress]
    [MaxLength(80)]
    string? Email, 
    IFormFile? ProfilePic
);