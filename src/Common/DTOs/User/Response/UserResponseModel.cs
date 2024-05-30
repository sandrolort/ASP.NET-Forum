namespace Common.DTOs.User.Response;

public record UserResponseModel(
    string Id,
    string UserName, 
    string Email, 
    string ProfilePicUrl, 
    bool IsAdmin, 
    bool IsBanned
);