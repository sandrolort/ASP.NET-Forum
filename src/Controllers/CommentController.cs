using System.Security.Claims;
using Common.Authorization;
using Common.DTOs.Comment.Request;
using Common.DTOs.Comment.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;
using FluentValidation;
using Swashbuckle.AspNetCore.Annotations;
using Validators.Comment;

namespace Controllers;

[ApiController]
[ForbidBanned]
[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "v1")]
[SwaggerTag("Comment Management.")]
[SwaggerResponse(401, "You are not authorized.")]
public class CommentController : ControllerBase
{
    private readonly IServiceManager _serviceManager;
    
    public CommentController(IServiceManager serviceManager)
    {
        _serviceManager = serviceManager;
    }
    
    [HttpPost]
    [SwaggerOperation(
        Summary = "Creates a new comment",
        Description = "Creates a new comment with the provided details",
        OperationId = "Comment.Create"
    )]
    [SwaggerResponse(400, "Invalid comment details")]
    [SwaggerResponse(200, "Comment created successfully", typeof(CommentResponseModel))]
    public async Task<IActionResult> CreateComment(CommentCreateModel comment)
    {
        var validator = new CommentCreateValidation();
        await validator.ValidateAndThrowAsync(comment);
        
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        var response = await _serviceManager.CommentService.CreateComment(userId, comment, HttpContext.RequestAborted);
        return Ok(response);
    }
    
    [HttpPut("{id}")]
    [SwaggerOperation(
        Summary = "Updates an existing comment",
        Description = "Updates a comment with the provided details",
        OperationId = "Comment.Update"
    )]
    [SwaggerResponse(400, "Invalid comment details")]
    [SwaggerResponse(404, "Comment not found")]
    [SwaggerResponse(200, "Comment updated successfully", typeof(CommentResponseModel))]
    public async Task<IActionResult> UpdateComment(uint id, CommentUpdateModel comment)
    {
        var validator = new CommentUpdateValidation();
        await validator.ValidateAndThrowAsync(comment);
        
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        var response = await _serviceManager.CommentService.UpdateComment(userId, id, comment, HttpContext.RequestAborted);
        return Ok(response);
    }
    
    [HttpDelete("{id}")]
    [SwaggerOperation(
        Summary = "Deletes an existing comment",
        Description = "Deletes a comment with the provided id",
        OperationId = "Comment.Delete"
    )]
    [SwaggerResponse(404, "Comment not found")]
    [SwaggerResponse(204, "Comment deleted successfully")]
    public async Task<IActionResult> DeleteComment(uint id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        
        await _serviceManager.CommentService.DeleteComment(userId, id, HttpContext.RequestAborted);
        
        return NoContent();
    }
    
    [AllowAnonymous]
    [HttpGet("{id}")]
    [SwaggerOperation(
        Summary = "Gets a comment",
        Description = "Gets a comment with the provided id",
        OperationId = "Comment.Get"
    )]
    [SwaggerResponse(404, "Comment not found")]
    [SwaggerResponse(200, "Comment retrieved successfully", typeof(CommentResponseModel))]
    public async Task<IActionResult> GetComment(uint id, [FromQuery] bool includeUser = false)
    {
        var comment = await _serviceManager.CommentService.GetCommentById(id, false, includeUser, HttpContext.RequestAborted);
        return Ok(comment);
    }
}