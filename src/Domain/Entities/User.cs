using Microsoft.AspNetCore.Identity;

namespace Domain.Entities;

public class User : IdentityUser
{
    public bool IsAdmin { get; set; } 
    public bool IsBanned { get; set; }
    public string? ProfilePicUrl { get; set; }
    public string? LastJwtToken { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }

    public virtual Ban? BanInfo { get; set; }
    public virtual IEnumerable<Comment>? Comments { get; set; }
    public virtual IEnumerable<Topic>? Topics { get; set; }

    public override string ToString() => $"Id: {Id}, UserName: {UserName}, Email: {Email}, IsAdmin: {IsAdmin}, IsBanned: {IsBanned}";
}