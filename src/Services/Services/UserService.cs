using Common.DTOs;
using Common.DTOs.Comment.Response;
using Common.DTOs.Topic.Response;
using Common.DTOs.User.Request;
using Common.DTOs.User.Response;
using Common.Exceptions;
using Common.Parameters;
using Mapster;
using Repository.Contracts;
using Services.Contracts.Contracts;

namespace Services.Services;

public class UserService : IUserService
{
    private readonly IRepositoryManager _repositoryManager;
    private readonly ILoggerService _logger;

    public UserService(IRepositoryManager repositoryManager, ILoggerService logger)
    {
        _repositoryManager = repositoryManager;
        _logger = logger;
    }

    public async Task<UserResponseModel> GetUserById(string userId, CancellationToken cancellationToken)
    {
        var user = await _repositoryManager.UserRepository.GetUserById(userId, false, cancellationToken) 
                   ?? throw new NotFound("No such User found.");
        return user.Adapt<UserResponseModel>();
    }

    public async Task<UserResponseModel> GetUserByEmail(string email, CancellationToken cancellationToken)
    {
        var user = await _repositoryManager.UserRepository.GetUserByEmail(email, false, cancellationToken) 
                   ?? throw new NotFound("No such User found.");
        return user.Adapt<UserResponseModel>();
    }
    
    public async Task<UserResponseModel> GetUserByUserName(string userName, CancellationToken cancellationToken)
    {
        var user = await _repositoryManager.UserRepository.GetUserByUserName(userName, false, cancellationToken) 
                   ?? throw new NotFound("No such User found.");
        return user.Adapt<UserResponseModel>();
    }

    public async Task<IEnumerable<UserResponseModel>> GetAllUsers(CancellationToken cancellationToken, RequestParameters requestParameters)
    {
        var users = await _repositoryManager.UserRepository.GetAllUsers(cancellationToken, requestParameters);
        return users.Adapt<IEnumerable<UserResponseModel>>();
    }

    public async Task<IEnumerable<CommentResponseModel>> GetUserComments(string userId, CancellationToken cancellationToken, RequestParameters requestParameters)
    {
        var comments = await _repositoryManager.CommentRepository.GetCommentsByUserId(userId, cancellationToken, requestParameters);
        return comments.Adapt<IEnumerable<CommentResponseModel>>();
    }

    public async Task<IEnumerable<TopicResponseModel>> GetUserTopics(string userId, CancellationToken cancellationToken, RequestParameters requestParameters)
    {
        var topics = await _repositoryManager.TopicRepository.GetTopicsByUserId(userId, cancellationToken, requestParameters);
        return topics.Adapt<IEnumerable<TopicResponseModel>>();
    }

    public async Task<IEnumerable<ActionLogResponse>> GetUserLogs(string userId, CancellationToken cancellationToken)
    {
        var logs = await _repositoryManager.ActionLogRepository.GetUserLogs(userId, cancellationToken);
        return logs.Adapt<IEnumerable<ActionLogResponse>>();
    }
    
    public async Task<UserResponseModel> UpdateUser(string userId, UserUpdateModel userUpdateModel, CancellationToken cancellationToken)
    {
        var user = await _repositoryManager.UserRepository.GetUserById(userId, true, cancellationToken) 
                   ?? throw new NotFound("No such User found.");
        
        user.UserName = userUpdateModel.UserName ?? user.UserName;
        user.Email = userUpdateModel.Email ?? user.Email;
        user.ProfilePicUrl = userUpdateModel.ProfilePicUrl ?? user.ProfilePicUrl;
        
        _repositoryManager.UserRepository.Update(user);
        await _repositoryManager.SaveAsync(cancellationToken);
        
        return user.Adapt<UserResponseModel>();
    }

    public async Task<UserResponseModel> DeleteUser(string userId, string requestAuthorUserId, CancellationToken cancellationToken)
    {
        var user = await _repositoryManager.UserRepository.GetUserById(userId, false, cancellationToken) 
                   ?? throw new NotFound("No such User found.");
        
        var requestAuthor = await _repositoryManager.UserRepository.GetUserById(requestAuthorUserId, false, cancellationToken) 
                            ?? throw new NotFound("No such User found.");
        
        if (!requestAuthor.IsAdmin && requestAuthor.Id != user.Id)
            throw new Forbidden("You are not authorized to delete this user.");
        
        await _repositoryManager.CommentRepository.DeleteCommentsByUserId(userId);
        await _repositoryManager.TopicRepository.DeleteTopicsByUserId(userId);
        
        _repositoryManager.UserRepository.Delete(user);
        
        await _repositoryManager.SaveAsync(cancellationToken);
        
        return user.Adapt<UserResponseModel>();
    }
}