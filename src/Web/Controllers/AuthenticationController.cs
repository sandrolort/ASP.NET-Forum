using Common.DTOs;
using Common.DTOs.User.Request;
using Common.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;
using Services.Contracts.Contracts;
using Web.Models;

namespace Web.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
public class AuthenticationController : Controller
{
    private IAuthenticationService _authenticationService; 
    private IServiceManager _serviceManager;

    public AuthenticationController(IAuthenticationService authenticationService, IServiceManager serviceManager)
    {
        _authenticationService = authenticationService;
        _serviceManager = serviceManager;
    }

    [HttpGet("Login")]
    public IActionResult Login()
    {
        if (HttpContext.Request.Cookies.ContainsKey("refreshToken") && HttpContext.Request.Cookies.ContainsKey("jwt"))
            return Redirect("RefreshToken");
        if (HttpContext.Request.Cookies.ContainsKey("jwt"))
            return RedirectToAction("Index", "Index");
        return View();
    }
    
    [HttpPost("Login")]
    public async Task<IActionResult> Login(UserLoginModel userLoginModel)
    {
        if (!await _authenticationService.ValidateUser(userLoginModel))
        {
            ViewData["LoginFailed"] = true;
            return View();
        }
        
        var jwt = await _authenticationService.GenerateJwtToken(true);
        
        HttpContext.Response.Cookies.Append("jwt", jwt.AccessToken);
        HttpContext.Response.Cookies.Append("refreshToken", jwt.RefreshToken);
        
        return RedirectToAction("Index", "Index");
    }

    [HttpGet("Register")]
    public IActionResult Register()
    {
        if (HttpContext.Request.Cookies.ContainsKey("jwt"))
            return RedirectToAction("Index", "Index");
        
        return View();
    }

    [HttpPost("Register")]
    public async Task<IActionResult> Register(UserCreateModelWithImage userCreateModel)
    {
        var image = await _serviceManager.ImageService.CreateImage(userCreateModel.ProfilePic, Request.Host.Value);
        
        var user = new UserCreateModel(userCreateModel.UserName,  userCreateModel.Password, userCreateModel.Email, image);
        
        var res = await _authenticationService.RegisterUser(user);
        if (!res.Succeeded)
        {
            if (res.Errors.Any(e => e.Code == "DuplicateUserName"))
                throw new Conflict($"Username already exists! {res.Errors.Select(e => e.Description).Aggregate((a, b) => $"{a}, {b}")}");
            throw new BadRequest(res.Errors.Select(e => e.Description).Aggregate((a, b) => $"{a}, {b}"));
        }
            
        
        return Redirect("Login");
    }

    [HttpGet("Logout")]
    public IActionResult Logout()
    {
        HttpContext.Response.Cookies.Delete("jwt");
        HttpContext.Response.Cookies.Delete("refreshToken");
        return RedirectToAction("Index", "Index");
    }

    [HttpGet("RefreshToken")]
    public async Task<IActionResult> RefreshToken()
    {
        if (!HttpContext.Request.Cookies.ContainsKey("refreshToken"))
            return RedirectToAction("Login", "Authentication");

        var refreshToken = HttpContext.Request.Cookies["refreshToken"];
        var jwtToken = HttpContext.Request.Cookies["jwt"];

        if (string.IsNullOrEmpty(refreshToken) || string.IsNullOrEmpty(jwtToken))
            return Redirect("Login");

        try
        {
            var res = await _authenticationService.RefreshToken(new TokenDto(jwtToken, refreshToken));
            
            HttpContext.Response.Cookies.Append("jwt", res.AccessToken);
            HttpContext.Response.Cookies.Append("refreshToken", res.RefreshToken);
            
            return RedirectToAction("Index", "Index");
        }
        catch (Exception)
        {
            //reset cookies
            HttpContext.Response.Cookies.Delete("jwt");
            HttpContext.Response.Cookies.Delete("refreshToken");
            return Redirect("Login");
        }
}
}