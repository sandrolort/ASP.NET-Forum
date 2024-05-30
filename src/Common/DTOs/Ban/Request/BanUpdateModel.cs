using System.ComponentModel.DataAnnotations;

namespace Common.DTOs.Ban.Request;

public record BanUpdateModel(
    DateTime? BanEndDate,
    [MaxLength(100)]
    string? Reason
);