using System.Security.Claims;
using Common.Authorization;
using Common.DTOs.Comment.Response;
using Common.DTOs.Topic.Response;
using Common.DTOs.User.Request;
using Common.DTOs.User.Response;
using Common.Parameters;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;
using Validators.User;
using Swashbuckle.AspNetCore.Annotations;

namespace Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "v1")]
[SwaggerTag("User Management.")]
[SwaggerResponse(401, "You are not authorized to access this resource.")]
public class UserController : ControllerBase
{
    private readonly IServiceManager _serviceManager;
    
    public UserController(IServiceManager serviceManager)
    {
        _serviceManager = serviceManager;
    }
    
    [HttpGet("me")]
    [ForbidBanned]
    [SwaggerOperation(
        Summary = "Retrieves the current user",
        Description = "Retrieves the current user",
        OperationId = "User.GetCurrent"
    )]
    [SwaggerResponse(200, "User retrieved successfully", typeof(UserResponseModel))]
    public async Task<IActionResult> GetCurrentUser()
    {
        var user = await _serviceManager.UserService.GetUserById(User.FindFirstValue(ClaimTypes.NameIdentifier), HttpContext.RequestAborted);
        return Ok(user);
    }
    
    [HttpGet("all")]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(
        Summary = "Retrieves all users",
        Description = "Retrieves all users",
        OperationId = "User.GetAll"
    )]
    [SwaggerResponse(200, "Users retrieved successfully", typeof(IEnumerable<UserResponseModel>))]
    [SwaggerResponse(403, "You do not have permission to access this resource.")]
    public async Task<IActionResult> GetUsers([FromQuery] RequestParameters requestParameters)
    {
        var users = await _serviceManager.UserService.GetAllUsers(HttpContext.RequestAborted, requestParameters);
        return Ok(users);
    }
    
    [HttpGet("id/{userId}")]
    [SwaggerOperation(
        Summary = "Retrieves a user by ID",
        Description = "Retrieves a user with the provided ID",
        OperationId = "User.GetById"
    )]
    [SwaggerResponse(200, "User retrieved successfully", typeof(UserResponseModel))]
    [SwaggerResponse(404, "User not found")]
    public async Task<IActionResult> GetUserById(string userId)
    {
        var user = await _serviceManager.UserService.GetUserById(userId, HttpContext.RequestAborted);
        return Ok(user);
    }
    
    [HttpGet("email/{email}")]
    [SwaggerOperation(
        Summary = "Retrieves a user by email",
        Description = "Retrieves a user with the provided email",
        OperationId = "User.GetByEmail"
    )]
    [SwaggerResponse(200, "User retrieved successfully", typeof(UserResponseModel))]
    [SwaggerResponse(404, "User not found")]
    public async Task<IActionResult> GetUserByEmail(string email)
    {
        var user = await _serviceManager.UserService.GetUserByEmail(email, HttpContext.RequestAborted);
        return Ok(user);
    }
    
    [HttpGet("id/{userId}/comments")]
    [SwaggerOperation(
        Summary = "Retrieves a user's comments",
        Description = "Retrieves a user's comments with the provided ID",
        OperationId = "User.GetComments"
    )]
    [SwaggerResponse(200, "User's comments retrieved successfully", typeof(IEnumerable<CommentResponseModel>))]
    [SwaggerResponse(404, "User not found")]
    public async Task<IActionResult> GetUserComments(string userId, [FromQuery] RequestParameters requestParameters)
    {
        var comments = await _serviceManager.UserService.GetUserComments(userId, HttpContext.RequestAborted, requestParameters);
        return Ok(comments);
    }

    [HttpGet("id/{userId}/topics")]
    [SwaggerOperation(
        Summary = "Retrieves a user's topics",
        Description = "Retrieves a user's topics with the provided ID",
        OperationId = "User.GetTopics"
    )]
    [SwaggerResponse(200, "User's topics retrieved successfully", typeof(IEnumerable<TopicResponseModel>))]
    [SwaggerResponse(404, "User not found")]
    public async Task<IActionResult> GetUserTopics(string userId, [FromQuery] RequestParameters requestParameters)
    {
        var topics = await _serviceManager.UserService.GetUserTopics(userId, HttpContext.RequestAborted, requestParameters);
        return Ok(topics);
    }

    [Authorize]
    [HttpPut("/edit")]
    [SwaggerOperation(
        Summary = "Updates a user",
        Description = "Updates a user with the provided details",
        OperationId = "User.Update"
    )]
    public async Task<IActionResult> UpdateUser([FromBody] UserUpdateModel userUpdateModel)
    {
        var validator = new UserUpdateValidation(); 
        await validator.ValidateAndThrowAsync(userUpdateModel, HttpContext.RequestAborted); //FluentValidation.ValidationException
        
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        var user = await _serviceManager.UserService.UpdateUser(userId, userUpdateModel, HttpContext.RequestAborted);
        return Ok(user);
    }
    
    [Authorize]
    [HttpDelete("/id/{deleteUserId}/delete")]
    [SwaggerOperation(
        Summary = "Deletes a user",
        Description = "Deletes a user with the provided ID"
    )]
    public async Task<IActionResult> DeleteUser(string deleteUserId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        await _serviceManager.UserService.DeleteUser(deleteUserId, userId, HttpContext.RequestAborted);
        return NoContent();
    }
}