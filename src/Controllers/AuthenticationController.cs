using Common.DTOs;
using Common.DTOs.User.Request;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts.Contracts;
using Swashbuckle.AspNetCore.Annotations;
using Validators.Token;
using Validators.User;

namespace Controllers;

[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "v1")]
[SwaggerTag("User Management. Register and login users. Does not require authentication.")]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;
    
    public AuthenticationController(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }
    
    [HttpPost("login")]
    [SwaggerOperation(
        Summary = "Logs in a user",
        Description = "Authenticates the user with the provided details and returns the user object along with the access token",
        OperationId = "Authentication.Login"
    )]
    [SwaggerResponse(200, "User logged in successfully", typeof(TokenDto))]
    [SwaggerResponse(401, "Unauthorized. Invalid login credentials")]
    public async Task<IActionResult> Login(UserLoginModel loginRequest)
    {
        var validator = new UserLoginValidation(); 
        await validator.ValidateAndThrowAsync(loginRequest, HttpContext.RequestAborted);
        
        var response = await _authenticationService.ValidateUser(loginRequest);
        
        if (!response) return Unauthorized();
        
        var token = await _authenticationService.GenerateJwtToken(true);
        
        return Ok(token);
    }
    
    [HttpPost("refresh")]
    [SwaggerOperation(
        Summary = "Refreshes a user's token",
        Description = "Refreshes a user's token with the provided refresh token",
        OperationId = "Authentication.RefreshToken"
    )]
    [SwaggerResponse(200, "Token refreshed successfully", typeof(TokenDto))]
    [SwaggerResponse(401, "Unauthorized. Invalid refresh token (possibly expired) or invalid JWT token.")]
    public async Task<IActionResult> RefreshToken(TokenDto refreshTokenRequest)
    {
        var validator = new TokenDtoValidation();
        await validator.ValidateAndThrowAsync(refreshTokenRequest, HttpContext.RequestAborted);
        
        var response = await _authenticationService.RefreshToken(refreshTokenRequest);
        return Ok(response);
    }
    
    [HttpPost("register")]
    [SwaggerOperation(
        Summary = "Registers a new user",
        Description = "Creates a new user account with the provided details and returns the created user object",
        OperationId = "Authentication.Register"
    )]
    [SwaggerResponse(200, "User registered successfully")]
    [SwaggerResponse(409, "User already exists")]
    public async Task<IActionResult> Register(UserCreateModel registerRequest)
    {
        var validator = new UserCreateValidation();
        await validator.ValidateAndThrowAsync(registerRequest, HttpContext.RequestAborted);
        
        var response = await _authenticationService.RegisterUser(registerRequest);
        return Ok(response);
    }
}