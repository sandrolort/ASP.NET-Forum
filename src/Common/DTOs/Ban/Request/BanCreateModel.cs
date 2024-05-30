using System.ComponentModel.DataAnnotations;

namespace Common.DTOs.Ban.Request;

public record BanCreateModel(
    [Required]
    string? UserId, 
    [Required]
    [MaxLength(100)]
    string? Reason,
    DateTime? BanEndDate
);