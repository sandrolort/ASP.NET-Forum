using Common.DTOs.Ban.Request;
using Common.DTOs.Ban.Response;
using Common.Parameters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;
using FluentValidation;
using Swashbuckle.AspNetCore.Annotations;
using Validators.Ban;

namespace Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "v1")]
[SwaggerTag("Ban Management. Ban and unban users.")]
[SwaggerResponse(401, "You are not authorized to access this resource.")]
[SwaggerResponse(403, "You do not have permission to access this resource.")]
public class BanController : ControllerBase
{
    private readonly IServiceManager _serviceManager;

    public BanController(IServiceManager serviceManager)
    {
        _serviceManager = serviceManager;
    }
    
    [HttpPost]
    [SwaggerOperation(
        Summary = "Bans a user",
        Description = "Bans a user with the provided details",
        OperationId = "Ban.Create"
    )]
    [SwaggerResponse(200, "User banned successfully", typeof(BanResponseModel))]
    [SwaggerResponse(400, "Invalid ban details")]
    public async Task<IActionResult> BanUser(BanCreateModel ban)
    {
        var validator = new BanCreateValidation();
        await validator.ValidateAndThrowAsync(ban, HttpContext.RequestAborted);
        
        var response = await _serviceManager.BanService.CreateBan(ban, HttpContext.RequestAborted);
        return Ok(response);
    }
    
    [HttpPut("{id}")]
    [SwaggerOperation(
        Summary = "Updates an existing ban",
        Description = "Updates a ban with the provided details",
        OperationId = "Ban.Update"
    )]
    [SwaggerResponse(400, "Invalid ban details")]
    [SwaggerResponse(404, "Ban not found")]
    [SwaggerResponse(200, "Ban updated successfully", typeof(BanResponseModel))]
    public async Task<IActionResult> UpdateBan(uint id, BanUpdateModel ban)
    {
        var validator = new BanUpdateValidation();
        await validator.ValidateAndThrowAsync(ban, HttpContext.RequestAborted);
        
        var response = await _serviceManager.BanService.UpdateBan(id, ban, HttpContext.RequestAborted);
        return Ok(response);
    }
    
    [HttpDelete("{id}")]
    [SwaggerOperation(
        Summary = "Deletes a ban",
        Description = "Deletes a ban with the provided id",
        OperationId = "Ban.Delete"
    )]
    [SwaggerResponse(404, "Ban not found")]
    [SwaggerResponse(204, "Ban deleted successfully")]
    public async Task<IActionResult> DeleteBan(uint id)
    {
        await _serviceManager.BanService.DeleteBan(id, HttpContext.RequestAborted);
        return NoContent();
    }
    
    [AllowAnonymous]
    [HttpGet("{id}")]
    [SwaggerOperation(
        Summary = "Gets a ban",
        Description = "Gets a ban with the provided id",
        OperationId = "Ban.Get"
    )]
    [SwaggerResponse(404, "Ban not found")]
    [SwaggerResponse(200, "Ban retrieved successfully", typeof(BanResponseModel))]
    public async Task<IActionResult> GetBan(uint id)
    {
        var ban = await _serviceManager.BanService.GetBanById(id, HttpContext.RequestAborted);
        return Ok(ban);
    }
    
    [HttpGet]
    [SwaggerOperation(
        Summary = "Gets all bans",
        Description = "Gets all bans",
        OperationId = "Ban.GetAll"
    )]
    [SwaggerResponse(200, "Bans retrieved successfully", typeof(IEnumerable<BanResponseModel>))]
    public async Task<IActionResult> GetBans([FromQuery] RequestParameters requestParameters)
    {
        var bans = await _serviceManager.BanService.GetAllBans(HttpContext.RequestAborted, requestParameters);
        return Ok(bans);
    }
}