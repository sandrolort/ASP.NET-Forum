using Common.DTOs.Ban.Request;
using Common.Parameters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;
using Web.Models;

namespace Web.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly IServiceManager _serviceManager;

    public AdminController(IServiceManager serviceManager)
    {
        _serviceManager = serviceManager;
    }

    [HttpPost("Ban")]
    public async Task<IActionResult> Ban(BanCreateModel model)
    {
        await _serviceManager.BanService.CreateBan(model, HttpContext.RequestAborted);
        return Redirect($"Profile/{model.UserId}");
    }
    
    [HttpPost("Unban")]
    public async Task<IActionResult> Unban(UnbanModel model)
    {
        await _serviceManager.BanService.DeleteBan(model.UserId, HttpContext.RequestAborted);
        return Redirect($"Profile/{model.UserId}");
    }
    
    [HttpGet("UserList")]
    public async Task<IActionResult> UserList([FromQuery]RequestParameters parameters)
    {
        var users = await _serviceManager.UserService.GetAllUsers(HttpContext.RequestAborted, parameters);
        return View(users);
    }
    
    [HttpGet("UnconfirmedTopics")]
    public async Task<IActionResult> UnconfirmedTopics([FromQuery]RequestParameters parameters)
    {
        var posts = await _serviceManager.TopicService.GetUnconfirmedTopics(HttpContext.RequestAborted, parameters);
        return View(posts);
    }
    
    [HttpGet("SetStatus/{topicId}/{confirm}")]
    public async Task<IActionResult> SetStatus(uint topicId, bool confirm)
    {
        await _serviceManager.TopicService.ChangeTopicState(topicId, confirm, HttpContext.RequestAborted);
        return RedirectToAction("UnconfirmedTopics");
    }
    
    [HttpGet("AdminPanel")]
    public IActionResult AdminPanel() => View();
}