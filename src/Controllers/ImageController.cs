using Common.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;
using Services.Contracts.Contracts;
using Swashbuckle.AspNetCore.Annotations;

namespace Controllers;

[ApiController]
[ApiExplorerSettings(GroupName = "v1")]
[Route("[controller]")]
[SwaggerTag("Image Management. Upload, retrieve and delete images.")]
public class ImageController : ControllerBase
{
    private readonly ILoggerService _logger;
    private readonly IServiceManager _serviceManager;
    
    public ImageController(ILoggerService logger, IServiceManager serviceManager)
    {
        _logger = logger;
        _serviceManager = serviceManager;
    }
    
    [HttpGet("{url}")]
    [SwaggerOperation(
        Summary = "Retrieves an image",
        Description = "Retrieves an image with the provided URL",
        OperationId = "Image.Get"
    )]
    [SwaggerResponse(200, "Image retrieved successfully")]
    [SwaggerResponse(404, "Image not found")]
    public async Task<IActionResult> GetImage(string url)
    {
        var image = await _serviceManager.ImageService.GetImage(url);
        return File(image, "image/jpeg");
    }

    [ForbidBanned]
    [HttpPost]
    [SwaggerOperation(
        Summary = "Uploads an image",
        Description = "Uploads an image with the provided details",
        OperationId = "Image.Create"
    )]
    [SwaggerResponse(201, "Image uploaded successfully")]
    [SwaggerResponse(400, "Invalid image")]
    [SwaggerResponse(401, "You are not authorized to access this resource.")]
    [SwaggerResponse(403, "You do not have permission to access this resource.")]
    public async Task<IActionResult> CreateImage(IFormFile file)
    {
        var imageBytes = new byte[file.Length];
        
        var res = await file.OpenReadStream().ReadAsync(imageBytes);
        if (res == 0) return BadRequest();
        
        var image = await _serviceManager.ImageService.CreateImage(file.FileName, imageBytes);
        
        return CreatedAtAction(nameof(GetImage), new { url = $"https://{Request.Host.Value}/image/{image}" }, $"https://{Request.Host.Value}/image/{image}");
    }
    
    [Authorize(Roles = "Admin")]
    [HttpDelete("{imageId}")]
    [SwaggerOperation(
        Summary = "Deletes an image",
        Description = "Deletes an image with the provided URL",
        OperationId = "Image.Delete"
    )]
    [SwaggerResponse(204, "Image deleted successfully")]
    [SwaggerResponse(404, "Image not found")]
    [SwaggerResponse(401, "You are not authorized to access this resource.")]
    [SwaggerResponse(403, "You do not have permission to access this resource.")]
    public IActionResult DeleteImage(string url)
    {
        _serviceManager.ImageService.DeleteImage(url);
        return NoContent();
    }
}