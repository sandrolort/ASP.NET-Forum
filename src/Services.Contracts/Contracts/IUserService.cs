using Common.DTOs;
using Common.DTOs.Comment.Response;
using Common.DTOs.Topic.Response;
using Common.DTOs.User.Request;
using Common.DTOs.User.Response;
using Common.Parameters;

namespace Services.Contracts.Contracts;

public interface IUserService
{
    Task<UserResponseModel> GetUserById(string userId, CancellationToken cancellationToken);
    Task<UserResponseModel> GetUserByEmail(string email, CancellationToken cancellationToken);
    Task<UserResponseModel> GetUserByUserName(string email, CancellationToken cancellationToken);
    Task<IEnumerable<UserResponseModel>> GetAllUsers(CancellationToken cancellationToken, RequestParameters requestParameters);
    
    Task<IEnumerable<CommentResponseModel>> GetUserComments(string userId, CancellationToken cancellationToken, RequestParameters requestParameters);
    Task<IEnumerable<TopicResponseModel>> GetUserTopics(string userId, CancellationToken cancellationToken, RequestParameters requestParameters);
    
    Task<IEnumerable<ActionLogResponse>> GetUserLogs(string userId, CancellationToken cancellationToken);
    Task<UserResponseModel> UpdateUser(string userId, UserUpdateModel userUpdateModel, CancellationToken cancellationToken);
    
    Task<UserResponseModel> DeleteUser(string userId, string requestAuthorId, CancellationToken cancellationToken);
}