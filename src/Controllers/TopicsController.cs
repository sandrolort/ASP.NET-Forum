using System.Security.Claims;
using Common.Authorization;
using Common.DTOs.Topic.Request;
using Common.DTOs.Topic.Response;
using Common.Parameters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;
using FluentValidation;
using Swashbuckle.AspNetCore.Annotations;
using Validators.Topic;

namespace Controllers;

[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "v1")]
[SwaggerTag("Topic Management.")]
public class TopicsController : ControllerBase
{
    private readonly IServiceManager _serviceManager;
    
    public TopicsController(IServiceManager serviceManager)
    {
        _serviceManager = serviceManager;
    }
    
    [HttpGet]
    [SwaggerOperation(
        Summary = "Retrieves all topics",
        Description = "Retrieves all topics",
        OperationId = "Topic.GetAll"
    )]
    [SwaggerResponse(200, "Topics retrieved successfully")]
    public async Task<IActionResult> GetTopics([FromQuery] TopicParameters requestParameters)
    {
        var topics = await _serviceManager.TopicService.GetAllTopics(false, cancellationToken: HttpContext.RequestAborted, requestParameters: requestParameters);
        return Ok(topics);
    }
    
    [Authorize(Roles = "Admin")]
    [HttpGet("Unconfirmed")]
    [SwaggerOperation(
        Summary = "Retrieves all unconfirmed topics",
        Description = "Retrieves all unconfirmed topics",
        OperationId = "Topic.GetUnconfirmed"
    )]
    [SwaggerResponse(401, "User is not authenticated. Please log in.")]
    [SwaggerResponse(403, "User is not an admin")]
    [SwaggerResponse(200, "Unconfirmed topics retrieved successfully")]
    public async Task<IActionResult> GetUnconfirmedTopics([FromQuery] TopicParameters requestParameters)
    {
        var topics = await _serviceManager.TopicService.GetUnconfirmedTopics(HttpContext.RequestAborted, requestParameters);
        return Ok(topics);
    }
    
    [HttpGet("{topicId}")]
    [SwaggerOperation(
        Summary = "Retrieves a topic by ID",
        Description = "Retrieves a topic with the provided ID",
        OperationId = "Topic.GetById"
    )]
    [SwaggerResponse(200, "Topic retrieved successfully")]
    [SwaggerResponse(404, "Topic not found")]
    public async Task<IActionResult> GetTopicById(uint topicId)
    {
        var topic = await _serviceManager.TopicService.GetTopicById(topicId, false, false, HttpContext.RequestAborted);
        return Ok(topic);
    }
    
    [HttpPost]
    [ForbidBanned]
    [SwaggerOperation(
        Summary = "Creates a new topic",
        Description = "Creates a new topic with the provided details",
        OperationId = "Topic.Create"
    )]
    [SwaggerResponse(401, "User is not authenticated. Please log in.")]
    [SwaggerResponse(400, "Invalid topic details")]
    [SwaggerResponse(200, "Topic created successfully", typeof(TopicResponseModel))]
    public async Task<IActionResult> CreateTopic([FromBody] TopicCreateModel topicCreateModel)
    {
        var validator = new TopicCreateValidation();
        await validator.ValidateAndThrowAsync(topicCreateModel, HttpContext.RequestAborted);
        
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        var topic = await _serviceManager.TopicService.CreateTopic(userId, topicCreateModel, HttpContext.RequestAborted);
        return CreatedAtAction(nameof(GetTopicById), new { topicId = topic.Id }, topic);
    }
    
    [HttpPut("{topicId}")]
    [ForbidBanned]
    [SwaggerOperation(
        Summary = "Updates an existing topic",
        Description = "Updates a topic with the provided details",
        OperationId = "Topic.Update"
    )]
    [SwaggerResponse(401, "User is not authenticated. Please log in.")]
    [SwaggerResponse(400, "Invalid topic details")]
    [SwaggerResponse(404, "Topic not found")]
    [SwaggerResponse(200, "Topic updated successfully", typeof(TopicResponseModel))]
    public async Task<IActionResult> UpdateTopic(uint topicId, [FromBody] TopicUpdateModel topicUpdateModel)
    {
        var validator = new TopicUpdateValidation();
        await validator.ValidateAndThrowAsync(topicUpdateModel, HttpContext.RequestAborted);
        
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        
        var topic = await _serviceManager.TopicService.UpdateTopic(userId, topicId, topicUpdateModel, HttpContext.RequestAborted);
        return Ok(topic);
    }
    
    [HttpPut("SetStatus/{topicId}/{confirm}")]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(
        Summary = "Sets the status of a topic",
        Description = "Sets the status of a topic with the provided ID",
        OperationId = "Topic.SetStatus"
    )]
    [SwaggerResponse(401, "User is not authenticated. Please log in.")]
    [SwaggerResponse(403, "User is not an admin")]
    [SwaggerResponse(404, "Topic not found")]
    [SwaggerResponse(204, "Topic status set successfully")]
    public async Task<IActionResult> SetStatus(uint topicId, bool confirm)
    {
        await _serviceManager.TopicService.ChangeTopicState(topicId, confirm, HttpContext.RequestAborted);
        return Ok();
    }
    
    [HttpDelete("{topicId}")]
    [ForbidBanned]
    [SwaggerOperation(
        Summary = "Deletes an existing topic",
        Description = "Deletes a topic with the provided ID",
        OperationId = "Topic.Delete"
    )]
    [SwaggerResponse(401, "User is not authenticated. Please log in.")]
    [SwaggerResponse(404, "Topic not found")]
    [SwaggerResponse(403, "User is not the author of the topic or is banned")]
    [SwaggerResponse(204, "Topic deleted successfully")]
    public async Task<IActionResult> DeleteTopic(uint topicId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        
        await _serviceManager.TopicService.DeleteTopic(userId, topicId, HttpContext.RequestAborted);
        
        return NoContent();
    }
}