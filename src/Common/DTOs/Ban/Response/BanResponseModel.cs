namespace Common.DTOs.Ban.Response;

public record BanResponseModel(
    uint Id,
    string UserId, 
    string Reason, 
    DateTime BanEndDate
);