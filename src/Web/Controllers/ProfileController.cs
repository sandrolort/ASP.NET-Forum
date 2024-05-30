using Common.Authorization;
using Common.DTOs.Ban.Request;
using Common.DTOs.User.Request;
using Common.DTOs.User.Response;
using Common.Parameters;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;
using Services.Contracts.Contracts;
using Web.Models;

namespace Web.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
[ForbidBanned]
public class ProfileController : Controller
{
    private readonly IServiceManager _serviceManager;
    private readonly IAuthenticationService _authenticationService;
    
    public ProfileController(IServiceManager serviceManager, IAuthenticationService authenticationService)
    {
        _serviceManager = serviceManager;
        _authenticationService = authenticationService;
    }
    
    [HttpGet("ProfileSearch")]
    public async Task<IActionResult> ProfileSearch()
    {
        if (string.IsNullOrEmpty(Request.Query["searchTerm"]))
            return View();
        
        var search = Request.Query["searchTerm"].ToString();
        UserResponseModel user;
        if(search.Contains('@'))
            user = await _serviceManager.UserService.GetUserByEmail(search, HttpContext.RequestAborted);
        else
            user = await _serviceManager.UserService.GetUserById(search, HttpContext.RequestAborted);
        return View(user);
    }
    
    [HttpGet("Profile")]
    public async Task<IActionResult> Profile()
    {
        var userId = await _authenticationService.GetUserId(HttpContext.Request.Cookies["jwt"]!);
        
        var user = await _serviceManager.UserService.GetUserById(userId, HttpContext.RequestAborted);
        var topics = await _serviceManager.UserService.GetUserTopics(userId, HttpContext.RequestAborted, new RequestParameters());
        var comments = await _serviceManager.UserService.GetUserComments(userId, HttpContext.RequestAborted, new RequestParameters());
        
        return View(new ProfileViewModel(user, comments, topics, new BanCreateModel(null,null,null), true));
    }
    
    [HttpGet("Profile/{id}")]
    public async Task<IActionResult> Profile(string id)
    {
        var user = await _serviceManager.UserService.GetUserById(id, HttpContext.RequestAborted);
        var topics = await _serviceManager.UserService.GetUserTopics(id, HttpContext.RequestAborted, new RequestParameters());
        var comments = await _serviceManager.UserService.GetUserComments(id, HttpContext.RequestAborted, new RequestParameters());

        return View(new ProfileViewModel(user, comments, topics, new BanCreateModel(id,null,null)));
    }

    [HttpGet("EditProfile")]
    public async Task<IActionResult> EditProfile()
    {
        var userId = await _authenticationService.GetUserId(HttpContext.Request.Cookies["jwt"]!);
        if (string.IsNullOrEmpty(userId))
            return Redirect("Login");
        
        var user = await _serviceManager.UserService.GetUserById(userId, HttpContext.RequestAborted);

        var userUpdateModel = new UserUpdateModelWithImage(user.UserName, user.Email, null);
        
        return View(userUpdateModel);
    }
    
    [HttpPost("EditProfile")]
    public async Task<IActionResult> EditProfile(UserUpdateModelWithImage user)
    {
        var userId = await _authenticationService.GetUserId(HttpContext.Request.Cookies["jwt"]!);
        if (string.IsNullOrEmpty(userId))
            return Redirect("Login");

        var updateModel = new UserUpdateModel(user.UserName, user.Email, null);
        
        updateModel = updateModel with { ProfilePicUrl = await _serviceManager.ImageService.CreateImage(user.ProfilePic, Request.Host.Value) };

        await _serviceManager.UserService.UpdateUser(userId, updateModel, HttpContext.RequestAborted);
        return Redirect("Profile");
    }

    [HttpGet("ChangePassword")]
    public IActionResult ChangePassword() => View();
    
    [HttpPost("ChangePassword")]
    public async Task<IActionResult> ChangePassword(UserChangePasswordModel userChangePasswordModel)
    {
        var userId = await _authenticationService.GetUserId(HttpContext.Request.Cookies["jwt"]!);
        if (string.IsNullOrEmpty(userId))
            return Redirect("Login");
        
        await _authenticationService.EditPassword(userId, userChangePasswordModel);
        return Redirect("Profile");
    }
    
    [HttpGet("DeleteProfile")]
    public IActionResult DeleteProfile() => View();
    
    [HttpPost("DeleteProfile")]
    public async Task<IActionResult> DeleteProfileConfirmed()
    {
        var userId = await _authenticationService.GetUserId(HttpContext.Request.Cookies["jwt"]!);
        
        await _serviceManager.UserService.DeleteUser(userId, userId, HttpContext.RequestAborted);
        return Redirect("Logout");
    }
}