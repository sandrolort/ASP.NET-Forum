using Common.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
public class Error : Controller
{
    [HttpGet("Error/{statusCode}/{message}")]
    public IActionResult ErrorPage(int statusCode, string? message)
    {
        
        return View(new ErrorDetails(statusCode, message ?? "An error occurred"));
    }
    
    [HttpGet("Error/{statusCode}")]
    public IActionResult ErrorPage(int statusCode)
    {
        return View(new ErrorDetails(statusCode, ""));
    }
}