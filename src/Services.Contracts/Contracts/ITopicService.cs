using Common.DTOs;
using Common.DTOs.Comment.Response;
using Common.DTOs.Topic.Request;
using Common.DTOs.Topic.Response;
using Common.Parameters;

namespace Services.Contracts.Contracts;

public interface ITopicService
{
    Task<TopicResponseModel> GetTopicById(uint topicId, bool trackChanges, bool includeUser, CancellationToken cancellationToken);
    Task<IEnumerable<CommentResponseModel>> GetCommentsByTopicId(uint topicId, bool includeUser, CancellationToken cancellationToken, RequestParameters requestParameters);
    Task<IEnumerable<TopicResponseModel>> GetAllTopics(bool includeUser, CancellationToken cancellationToken,
        TopicParameters requestParameters);
    Task<IEnumerable<TopicResponseModel>> GetUnconfirmedTopics(CancellationToken cancellationToken, RequestParameters parameters);
    
    Task<TopicResponseModel> CreateTopic(string userId, TopicCreateModel topicCreateModel, CancellationToken cancellationToken);
    Task<TopicResponseModel> UpdateTopic(string userId, uint topicId, TopicUpdateModel topicUpdateModel, CancellationToken cancellationToken);
    Task ChangeTopicState(uint topicId, bool isConfirmed, CancellationToken cancellationToken);
    Task DeleteTopic(string userId, uint topicId, CancellationToken cancellationToken);
    
    Task<IEnumerable<ActionLogResponse>> GetTopicLogs(uint topicId, CancellationToken cancellationToken);
    Task ArchiveOldTopics(CancellationToken cancellationToken);
}