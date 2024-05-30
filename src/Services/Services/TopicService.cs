using Common.DTOs;
using Common.DTOs.Comment.Response;
using Common.DTOs.Topic.Request;
using Common.DTOs.Topic.Response;
using Common.Enums;
using Common.Exceptions;
using Common.Parameters;
using Domain.Entities;
using Mapster;
using Microsoft.Extensions.Configuration;
using Repository.Contracts;
using Services.Contracts.Contracts;

namespace Services.Services;

public class TopicService : ITopicService
{
    private readonly IRepositoryManager _repositoryManager;
    private readonly ILoggerService _logger;
    private readonly IConfiguration _configuration;

    public TopicService(IRepositoryManager repositoryManager, ILoggerService logger, IConfiguration configuration)
    {
        _repositoryManager = repositoryManager;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<TopicResponseModel> GetTopicById(uint topicId, bool trackChanges, bool includeUser, CancellationToken cancellationToken)
    {
        var topic = await _repositoryManager.TopicRepository.GetTopicById(topicId, trackChanges, includeUser, cancellationToken) ??
                    throw new NotFound("No such Topic found.");
        return topic.Adapt<TopicResponseModel>();
    }
    
    public async Task<IEnumerable<CommentResponseModel>> GetCommentsByTopicId
        (uint topicId, bool includeUser, CancellationToken cancellationToken, RequestParameters requestParameters)
    {
        _ = await CheckAndReturnTopic(topicId, false, false, cancellationToken);

        var comments = await _repositoryManager.CommentRepository.GetCommentsByTopicId(topicId, includeUser, cancellationToken, requestParameters);
        
        return comments.Adapt<IEnumerable<CommentResponseModel>>();
    }

    public async Task<IEnumerable<TopicResponseModel>> GetAllTopics(bool includeUser,
        CancellationToken cancellationToken, TopicParameters requestParameters)
    {
        var topics = await _repositoryManager.TopicRepository.GetAllTopics(includeUser, cancellationToken, requestParameters);
        
        return topics.Adapt<IEnumerable<TopicResponseModel>>();
    }

    public async Task<IEnumerable<TopicResponseModel>> GetUnconfirmedTopics(CancellationToken cancellationToken, RequestParameters parameters)
    {
        var topics = await _repositoryManager.TopicRepository.GetUnconfirmedTopics(cancellationToken, parameters, true);
        return topics.Adapt<IEnumerable<TopicResponseModel>>();
    }

    public async Task<TopicResponseModel> CreateTopic(string userId, TopicCreateModel topicCreateModel, CancellationToken cancellationToken)
    {
        var minCommentsRequired = _configuration["ForumConfiguration:MinCommentsRequired"];
        var userCommentCount = await _repositoryManager.CommentRepository.GetCommentsCountByUserId(userId, cancellationToken);
        var user = await _repositoryManager.UserRepository.GetUserById(userId, false, cancellationToken);
        if (userCommentCount < int.Parse(minCommentsRequired) && !user!.IsAdmin)
            throw new Forbidden($"You need to have at least {minCommentsRequired} comments to create a topic.");

        var topic = topicCreateModel.Adapt<Topic>();
        topic.AuthorId = userId;
        _repositoryManager.TopicRepository.Create(topic);
        await _repositoryManager.SaveAsync(cancellationToken);
        return topic.Adapt<TopicResponseModel>();
    }

    public async Task<TopicResponseModel> UpdateTopic(string userId, uint topicId, TopicUpdateModel topicUpdateModel, CancellationToken cancellationToken)
    {
        var topic = await CheckAndReturnTopic(topicId, false, true, cancellationToken);
        
        if (topic.AuthorId != userId)
            throw new Forbidden("You are not allowed to update this topic.");
        
        if (topic.Status is Status.Inactive)
            throw new BadRequest("You cannot update an inactive topic.");
        
        topic.Title = topicUpdateModel.Title ?? topic.Title;
        topic.Content = topicUpdateModel.Content ?? topic.Content;
        
        await _repositoryManager.SaveAsync(cancellationToken);
        return topic.Adapt<TopicResponseModel>();
    }

    public async Task ChangeTopicState(uint topicId, bool isConfirmed, CancellationToken cancellationToken)
    {
        var topic = await CheckAndReturnTopic(topicId, false, true, cancellationToken);
        topic.State = isConfirmed ? State.Show : State.Hide;
        await _repositoryManager.SaveAsync(cancellationToken);
    }

    public async Task DeleteTopic(string userId, uint topicId, CancellationToken cancellationToken)
    {
        var topic = await CheckAndReturnTopic(topicId, false, false, cancellationToken);
        
        var user = await _repositoryManager.UserRepository.GetUserById(userId, false, cancellationToken);
        
        if (user == null) 
            throw new NotFound("No such User found.");
        
        if (topic.AuthorId != userId && !user.IsAdmin)
            throw new Forbidden("You are not allowed to delete this topic.");
        
        _repositoryManager.TopicRepository.Delete(topic);
        await _repositoryManager.SaveAsync(cancellationToken);
    }

    public async Task<IEnumerable<ActionLogResponse>> GetTopicLogs(uint topicId, CancellationToken cancellationToken)
    {
        _ = await CheckAndReturnTopic(topicId, false, false, cancellationToken);
        
        var logs = await _repositoryManager.ActionLogRepository.GetTopicLogs(topicId, cancellationToken);
        return logs.Adapt<IEnumerable<ActionLogResponse>>();
    }

    public async Task ArchiveOldTopics(CancellationToken cancellationToken)
    {
        var archiveOldTopics = _configuration.GetSection("ForumConfiguration").GetSection("ArchiveAfterDays").Value;
        _logger.LogInfo($"Archiving topics older than {archiveOldTopics} days.");
        
        var archivalTopics = await _repositoryManager.TopicRepository.GetArchivableTopics(int.Parse(archiveOldTopics), cancellationToken);
        
        foreach (var topic in archivalTopics)
        {
            topic.Status = Status.Inactive;
            _repositoryManager.TopicRepository.Update(topic);
        }
        
        await _repositoryManager.SaveAsync(cancellationToken);
    }

    private async Task<Topic> CheckAndReturnTopic(uint topicId, bool includeUser, bool trackChanges, CancellationToken cancellationToken) =>
        await _repositoryManager.TopicRepository.GetTopicById(topicId, trackChanges, includeUser, cancellationToken) 
        ?? throw new NotFound("No such Topic found.");
}