using Common.DTOs;
using Common.DTOs.Comment.Request;
using Common.DTOs.Comment.Response;
using Common.Enums;
using Common.Exceptions;
using Common.Parameters;
using Domain.Entities;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Repository.Contracts;
using Services.Contracts.Contracts;

namespace Services.Services;

public class CommentService : ICommentService
{
    private readonly IRepositoryManager _repositoryManager;
    private readonly ILoggerService _logger;
    
    public CommentService(IRepositoryManager repositoryManager, ILoggerService logger)
    {
        _repositoryManager = repositoryManager;
        _logger = logger;
    }
    
    public async Task<CommentResponseModel> GetCommentById
    (uint commentId, bool trackChanges, bool includeUser, CancellationToken cancellationToken)
    {
        var comment = await _repositoryManager.CommentRepository.GetCommentById(commentId, trackChanges, includeUser, cancellationToken);
        
        if (comment == null)
            throw new NotFound("No such Comment found.");
        
        return comment.Adapt<CommentResponseModel>();
    }

    public async Task<CommentResponseModel> CreateComment
    (string userId, CommentCreateModel commentCreateModel, CancellationToken cancellationToken)
    {
        var comment = commentCreateModel.Adapt<Comment>();
        
        comment.AuthorId = userId;
        _repositoryManager.CommentRepository.Create(comment);
        
        var topic = await _repositoryManager.TopicRepository.
            GetTopicById(commentCreateModel.TopicId!.Value ,true, false, cancellationToken) 
                    ?? throw new NotFound("No such Topic found.");
        
        if (topic.Status is Status.Inactive)
            throw new BadRequest("You cannot comment on an inactive topic.");
        
        topic.CommentCount++;

        try
        {
            await _repositoryManager.SaveAsync(cancellationToken);

            return comment.Adapt<CommentResponseModel>();
        }
        catch (DbUpdateConcurrencyException e)
        {
            _logger.LogWarn(e.Message);
        }
        
        throw new Conflict("Failed to create Comment. Too many users are trying to access the same resource. Please try again later.");
    }

    public async Task<CommentResponseModel> UpdateComment
    (string userId, uint commentId, CommentUpdateModel commentUpdateModel, CancellationToken cancellationToken)
    {
        var comment = await _repositoryManager.CommentRepository.GetCommentById(commentId,
                          true,
                          false,
                          cancellationToken) ??
                      throw new NotFound("No such Comment found.");
        
        if (comment.AuthorId != userId)
            throw new Forbidden("You are not allowed to update this comment.");
        
        if  (comment.Topic.Status is Status.Inactive)
            throw new BadRequest("You cannot update a comment on an inactive topic.");
        
        comment.Content = commentUpdateModel.Content ?? comment.Content;
        
        await _repositoryManager.SaveAsync(cancellationToken);
        return comment.Adapt<CommentResponseModel>();
    }

    public async Task DeleteComment(string userId, uint commentId, CancellationToken cancellationToken)
    {
        var comment = await Comment(commentId, false, false, cancellationToken);
        var user = await _repositoryManager.UserRepository.GetUserById(userId, false, cancellationToken);
        
        if (user == null) 
            throw new NotFound("No such User found.");
        
        if (comment.AuthorId != userId && !user.IsAdmin)
            throw new Forbidden("You are not allowed to delete this comment.");
        
        var topic = await _repositoryManager.TopicRepository.GetTopicById(comment.TopicId, true, false, cancellationToken) 
                    ?? throw new NotFound("No such Topic found.");
        
        topic.CommentCount--;
        
        _repositoryManager.CommentRepository.Delete(comment);
        
        try
        {
            await _repositoryManager.SaveAsync(cancellationToken);
            
            return;
        }
        catch (DbUpdateConcurrencyException e)
        {
            _logger.LogWarn(e.Message);
        }
        
        throw new Conflict("Failed to delete Comment. Too many users are trying to access the same resource. Please try again later.");
    }

    public async Task<IEnumerable<ActionLogResponse>> GetCommentLogs(uint commentId, CancellationToken cancellationToken)
    {
        _ = await Comment(commentId, false, false, cancellationToken);
        
        var logs = await _repositoryManager.ActionLogRepository.GetCommentLogs(commentId, cancellationToken);
        return logs.Adapt<IEnumerable<ActionLogResponse>>();
    }

    public async Task ReevaluateCommentCount(CancellationToken cancellationToken)
    {
        var topics = await _repositoryManager.TopicRepository.GetAllTopics(cancellationToken);
        foreach (var topic in topics)
        {
            if (cancellationToken.IsCancellationRequested)
                break;
            topic.CommentCount = (uint)await _repositoryManager.CommentRepository.GetCommentsCountByTopicId(topic.Id, cancellationToken);
        }
        
        await _repositoryManager.SaveAsync(cancellationToken);
    }

    private async Task<Comment> Comment(uint commentId, bool trackChanges, bool includeUser, CancellationToken cancellationToken)
    {
        return await _repositoryManager.CommentRepository.GetCommentById(commentId, trackChanges, includeUser, cancellationToken) 
               ?? throw new NotFound("No such Comment found.");
    }
}