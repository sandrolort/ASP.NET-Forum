namespace Domain.Entities;

// ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
public class Ban : BaseEntity
{
    public DateTime BanEndDate { get; set; }
    public string? Reason { get; set; }
    
    public string UserId { get; set; } = null!;
    public virtual User BannedUser { get; set; } = null!;

    public override string ToString() => $"Id: {Id}, BanEndDate: {BanEndDate}, Reason: {Reason}, UserId: {UserId}";
}