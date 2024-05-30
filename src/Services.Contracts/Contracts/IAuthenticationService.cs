using Common.DTOs;
using Common.DTOs.User.Request;
using Microsoft.AspNetCore.Identity;

namespace Services.Contracts.Contracts;

public interface IAuthenticationService
{
    Task<IdentityResult> RegisterUser(UserCreateModel userCreateModel);
    Task<bool> ValidateUser(UserLoginModel userLoginModel);
    Task<TokenDto> GenerateJwtToken(bool populateExpirationTime);
    Task EditPassword(string userId, UserChangePasswordModel userChangePasswordModel);
    Task<TokenDto> RefreshToken(TokenDto refreshTokenModel);
    Task<string> GetUserId(string jwtToken);
    Task<string> GetUserName(string jwtToken);
}