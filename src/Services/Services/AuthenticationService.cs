using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Common.DTOs;
using Common.DTOs.User.Request;
using Common.Exceptions;
using Domain.Entities;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Services.Contracts.Contracts;

namespace Services.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly UserManager<User> _userManager;
    private readonly IConfiguration _configuration;
    private readonly ILoggerService _logger;
    
    private User? _user;
    
    public AuthenticationService(UserManager<User> userManager, IConfiguration configuration, ILoggerService logger)
    {
        _userManager = userManager;
        _configuration = configuration;
        _logger = logger;
    }
    
    public async Task<IdentityResult> RegisterUser(UserCreateModel userCreateModel)
    {
        var user = userCreateModel.Adapt<User>();
        
        if (string.IsNullOrWhiteSpace(user.ProfilePicUrl))
        {
            user.ProfilePicUrl = _configuration["ForumConfiguration:DefaultProfilePic"];
        }
        
        var result = await _userManager.CreateAsync(user, userCreateModel.Password);
        
        if (!result.Succeeded) return result;
        
        await _userManager.AddToRoleAsync(user, "User");
        await _userManager.AddClaimAsync(user, new Claim("IsBanned", false.ToString()));

        return result;
    }

    public async Task<bool> ValidateUser(UserLoginModel userLoginModel)
    {
        _user = await _userManager.FindByNameAsync(userLoginModel.UserName);
        var result = _user != null && await _userManager.CheckPasswordAsync(_user, userLoginModel.Password);
        
        return result;
    }

    public async Task<TokenDto> GenerateJwtToken(bool populateExpirationTime)
    {
        var signingCredentials = GetSigningCredentials();
        var claims = await GetClaims();
        var tokenOptions = GenerateTokenOptions(signingCredentials, claims);
        
        var refreshToken = GenerateRefreshToken();
        _user!.RefreshToken = refreshToken;
        if(populateExpirationTime)
            _user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
        
        var accessToken = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        _user.LastJwtToken = accessToken;
        
        await _userManager.UpdateAsync(_user);
        
        return new TokenDto(accessToken, refreshToken);
    }

    public async Task EditPassword(string userId, UserChangePasswordModel userChangePasswordModel)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            throw new NotFound("User not found");
        
        if (!await _userManager.CheckPasswordAsync(user, userChangePasswordModel.OldPassword))
            throw new BadRequest("Old password is incorrect");
        
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        
        await _userManager.ResetPasswordAsync(user, token, userChangePasswordModel.NewPassword);
    }

    public async Task<TokenDto> RefreshToken(TokenDto refreshTokenModel)
    {
        var principal = GetPrincipalFromExpiredToken(refreshTokenModel.AccessToken);
        var user = await _userManager.FindByNameAsync(principal.Identity?.Name);
        _logger.LogInfo($"User: {user.UserName} RefreshToken: {user.RefreshToken} RefreshTokenExpiryTime: {user.RefreshTokenExpiryTime}");
        if (user == null || user.RefreshToken != refreshTokenModel.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
            throw new Unauthorized("Invalid token");
        _user = user;
        return await GenerateJwtToken(true);
    }
    
    private SigningCredentials GetSigningCredentials()
    {
        var key = Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("SECRET")!);
        var secret = new SymmetricSecurityKey(key);
        return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
    }
    
    private async Task<List<Claim>> GetClaims()
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, _user!.UserName),
            new(ClaimTypes.NameIdentifier, _user.Id)
        };
        
        var roles = await _userManager.GetRolesAsync(_user);

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        return claims;
    }
    
    private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        
        return new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(Convert.ToDouble(jwtSettings["expires"])),
            signingCredentials: signingCredentials
        );
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
    
    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = Environment.GetEnvironmentVariable("SECRET");
        
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateLifetime = false,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new
                SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!))
        };
        
        var tokenHandler = new JwtSecurityTokenHandler();
        
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
        
        if (securityToken is not JwtSecurityToken jwtSecurityToken || 
            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token");
        }
        
        return principal;
    }
    
    public Task<string> GetUserId(string jwtToken)
    {
        var token = new JwtSecurityTokenHandler().ReadJwtToken(jwtToken);
        if (token == null) 
            throw new Unauthorized("Invalid token");
        return Task.FromResult(token.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value);
    }

    public Task<string> GetUserName(string jwtToken)
    {
        var token = new JwtSecurityTokenHandler().ReadJwtToken(jwtToken);
        if (token == null) 
            throw new Unauthorized("Invalid token");
        return Task.FromResult(token.Claims.First(claim => claim.Type == ClaimTypes.Name).Value);
    }
    
    public static void BanCheck(bool isBanned)
    {
        if (isBanned) throw new Forbidden("You are banned.");
    }
}