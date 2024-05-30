using Common.Authorization;
using Common.DTOs.Comment.Request;
using Common.DTOs.Comment.Response;
using Common.DTOs.Topic.Request;
using Common.DTOs.Topic.Response;
using Common.Parameters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;
using Services.Contracts.Contracts;
using Web.Models;

namespace Web.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
public class TopicsController : Controller
{ 
    private readonly IServiceManager _serviceManager;
    private readonly IAuthenticationService _authenticationService;
    
    public TopicsController(IServiceManager serviceManager, IAuthenticationService authenticationService)
    {
        _serviceManager = serviceManager;
        _authenticationService = authenticationService;
    }

    [HttpGet("Topics")]
    public async Task<IActionResult> Topics([FromQuery]TopicParameters parameters)
    {
        var pagedModel = new PagedModel<TopicResponseModel>(
            await _serviceManager.TopicService.GetAllTopics(true, HttpContext.RequestAborted, parameters),
            parameters.PageNumber,
            parameters.PageSize,
            parameters.Search,
            parameters.OrderBy
        );
       
        var res = pagedModel.Items.Select(t => t with { Content = InterpretMarkdown(t.Content) });
        
        var newpagedModel = new PagedModel<TopicResponseModel>(
            res,
            parameters.PageNumber,
            parameters.PageSize,
            parameters.Search,
            parameters.OrderBy
        );
        
        return View(newpagedModel);
    }
    
    [HttpGet("Topics/{id}")]
    public async Task<IActionResult> Topic(uint id)
    {
        var topic = await _serviceManager.TopicService.GetTopicById(id, false, true, HttpContext.RequestAborted);
        
        var newTopic = topic with { Content = InterpretMarkdown(topic.Content) };
        
        var comments = await _serviceManager.TopicService.GetCommentsByTopicId(id, true, HttpContext.RequestAborted, new RequestParameters());
        var commentResponseModels = comments as CommentResponseModel[] ?? comments.ToArray();
        var newComments = commentResponseModels.Select(c => c with { Content = InterpretMarkdown(c.Content) });
        
        return View(new TopicCommentResponseModel(newTopic,newComments));
    }
    
    [ForbidBanned]
    [HttpGet("Topics/{id}/Edit")]
    public async Task<IActionResult> EditTopic(uint id)
    {
        var topic = await _serviceManager.TopicService.GetTopicById(id, false, false, HttpContext.RequestAborted);
        
        var model = new TopicUpdateModelWithImage(topic.Title, topic.Content, null);
        
        return View(model);
    }
    
    [ForbidBanned]
    [HttpPost("Topics/{id}/Edit")]
    public async Task<IActionResult> EditTopic(uint id, TopicUpdateModelWithImage model)
    {
        var token = HttpContext.Request.Cookies["jwt"];
        if (string.IsNullOrEmpty(token))
            return Redirect("Login");
        var userId = await _authenticationService.GetUserId(token);
        
        var image = model.Image != null ? await _serviceManager.ImageService.CreateImage(model.Image, Request.Host.Value) : null;
        
        var topic = new TopicUpdateModel(model.Title, model.Content, image);
        
        await _serviceManager.TopicService.UpdateTopic(userId, id, topic, HttpContext.RequestAborted);
        
        return RedirectToAction("Topic", new { id });
    }
    
    [ForbidBanned]
    [Authorize(Roles = "Admin,User")]
    [HttpPost("Topics/{id}")]
    public async Task<IActionResult> CommentOnTopic(uint id, string comment)
    {
        var token = HttpContext.Request.Cookies["jwt"];
        if (string.IsNullOrEmpty(token))
            return Redirect("Login");
        var userId = await _authenticationService.GetUserId(token);

        var newComment = new CommentCreateModel(comment, id);
        
        await _serviceManager.CommentService.CreateComment(userId, newComment, HttpContext.RequestAborted);
        
        return RedirectToAction("Topic", new { id });
    }
    
    [ForbidBanned]
    [HttpDelete("Comments/{topicId}/{commentId}")]
    public async Task<IActionResult> DeleteComment(uint topicId, uint commentId)
    {
        var token = HttpContext.Request.Cookies["jwt"];
        if (string.IsNullOrEmpty(token))
            return Redirect("Login");
        var userId = await _authenticationService.GetUserId(token);
        
        await _serviceManager.CommentService.DeleteComment(userId, commentId, HttpContext.RequestAborted);
        
        return Redirect("Topic/" + topicId);
    }
    
    [ForbidBanned]
    [HttpDelete("Topics/{id}")]
    public async Task<IActionResult> DeleteTopic(uint id)
    {
        var token = HttpContext.Request.Cookies["jwt"];
        if (string.IsNullOrEmpty(token))
            return Redirect("Login");
        var userId = await _authenticationService.GetUserId(token);
        
        await _serviceManager.TopicService.DeleteTopic(userId, id, HttpContext.RequestAborted);
        
        return RedirectToAction("Topics");
    }
    
    [ForbidBanned]
    [HttpGet("Topics/Create")]
    public IActionResult CreateTopic()
    {
        return View();
    }
    
    [ForbidBanned]
    [HttpPost("Topics/Create")]
    public async Task<IActionResult> CreateTopic(TopicCreateModelWithImage model)
    {
        var token = HttpContext.Request.Cookies["jwt"];
        if (string.IsNullOrEmpty(token))
            return Redirect("Login");
        var userId = await _authenticationService.GetUserId(token);
        
        var image = await _serviceManager.ImageService.CreateImage(model.Image, Request.Host.Value);
        
        var topic = new TopicCreateModel(model.Title, model.Content, image);
        
        await _serviceManager.TopicService.CreateTopic(userId, topic, HttpContext.RequestAborted);
        
        return RedirectToAction("Topics");
    }
    
    private string InterpretMarkdown(string markdown)
    {
        var res = CommonMark.CommonMarkConverter.Convert(markdown);

        return res;
    }
}